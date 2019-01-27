using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CharacterSlotName
{
    Head,
    Necklace,
    Ass,
    Eyes,
    OnHead,
    Mouth
}

[System.Serializable]
public class AccessorySlot
{
    public CharacterSlotName slotName;
    public Transform boneTransform;
}

public class Character : MonoBehaviour
{
    private bool controllable;
    public float maxMoveSpeed = 6f;
    public Transform characterVisual;
    public CharacterController characterController;
    public Animator characterAnimator;    

    public float LERP_MOVE_TIME = 0.5f;
    public float MOVEMENT_PREDICTION_SPEED_MULT = 0.5f;
    public float LERP_MAX_DISTANCE = 6f;

    [Header("Customization")]
    public List<AccessorySlot> customizationSlots = new List<AccessorySlot>();
    public List<AccessoryItem> dressedItems = new List<AccessoryItem>();

    static List<AccessoryItem> accessoryItems;
    public static List<AccessoryItem> AccessoryItems
    {
        get
        {
            if(accessoryItems == null)
            {
                LoadAccessory();
            }

            return accessoryItems;
        }

        set
        {
            accessoryItems = value;
        }
    }

    public bool Controllable
    {
        get { return controllable; }

        set
        {
            controllable = value;

            if (!controllable)
            {
                gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }
    }

    private Vector3 oldPosition;

    Vector3 targetPosition;
    Vector3 velocity;

    Coroutine lerpRoutine;

    public UnityEngine.UI.Text usernameIndicatorText;

    public string Username
    {
        set
        {
            usernameIndicatorText.text = value;
        }
    }

    private void Start()
    {
        oldPosition = transform.position;
        Controllable = false;

        LoadAccessory();
    }

    private void Update()
    {
        if (!Controllable && lerpRoutine == null)
        {
            Vector3 oldPos = transform.position;
            characterController.Move(velocity * Time.deltaTime * MOVEMENT_PREDICTION_SPEED_MULT);
            UpdateRotation(oldPos, transform.position);
        }

        Vector3 delta = transform.position - oldPosition;
        CurrentVelocity = delta / Time.deltaTime;
        oldPosition = transform.position;

        UpdateAnimation();
    }


    void UpdateRotation(Vector3 oldPos, Vector3 newPos)
    {
        Vector3 desiredMoveDirection = newPos - oldPos;
        desiredMoveDirection = Vector3.ProjectOnPlane(desiredMoveDirection, Vector3.up);
        if (desiredMoveDirection != Vector3.zero)
        {
            Quaternion visualRotation = Quaternion.LookRotation(desiredMoveDirection, transform.up);
            characterVisual.localRotation = visualRotation;
        }
    }

    void UpdateAnimation()
    {
        characterAnimator.SetFloat("Speed", CurrentVelocity.magnitude / maxMoveSpeed);
    }

    void LerpMove()
    {
        if(lerpRoutine != null)
        {
            StopCoroutine(lerpRoutine);
        }

        lerpRoutine = StartCoroutine(LerpMove_Routine());
    }

    IEnumerator LerpMove_Routine()
    {
        if (Vector3.Distance(transform.position, targetPosition) < LERP_MAX_DISTANCE)
        {
            Vector3 startPos = transform.position;

            for (float _animTime = 0; _animTime < LERP_MOVE_TIME; _animTime += Time.deltaTime)
            {
                float t = _animTime / LERP_MOVE_TIME;

                Vector3 newPos = Vector3.Lerp(startPos, targetPosition, t);
                UpdateRotation(transform.position, newPos);
                transform.position = newPos;
                yield return null;
            }
        }

        transform.position = targetPosition;
  
        lerpRoutine = null;
    }

    public void TeleportTo(Vector3 newWorldPos)
    {
        transform.position = newWorldPos;
        targetPosition = transform.position;
    }

    public void MoveTo(Vector3 newWorldPos, Vector2 newVelocity)
    {
        targetPosition = newWorldPos;
        velocity = new Vector3(newVelocity.x, 0, newVelocity.y);
        LerpMove();
    }

    public Vector3 CurrentVelocity
    {
        private set;
        get;
    }   

    public void Move(Vector3 motion)
    {
        Vector3 oldPos = transform.position;
        characterController.Move(motion);

        UpdateRotation(oldPos, transform.position);
    }

    #region Accessory

    public static void LoadAccessory()
    {
        if(accessoryItems == null)
        {
            accessoryItems = new List<AccessoryItem>(Resources.LoadAll<AccessoryItem>("Prefabs/Accessory"));
            Debug.Log(accessoryItems.Count + " accessory items loaded");
        }
    }

    public void SetDressItems(List<AccessoryItem> items)
    {
        List<AccessoryItem> alreadyDressedItems = new List<AccessoryItem>();

        foreach(AccessoryItem item in dressedItems)
        {
            if(items.Exists(x => x.name == item.name))
            {
                alreadyDressedItems.Add(item);
            }
            else
            {
                UndressAccessoryItem(item);
            }
        }

        foreach(var item in items.Except(alreadyDressedItems))
        {
            DressAccessoryItem(item);
        }
    }

    public bool IsItemDressed(AccessoryItem item)
    {
        return dressedItems.Exists(x => x.name == item.name);
    }

    public void DressAccessoryItem(AccessoryItem item)
    {
        AccessorySlot slot = customizationSlots.Find(x => x.slotName == item.slotName);

        if(slot != null)
        {
            AccessoryItem newItemGO = Instantiate(item, slot.boneTransform, false);
            newItemGO.transform.localPosition = Vector3.zero;
            newItemGO.transform.localRotation = Quaternion.Euler(0, -90f, 90f);
            //newItemGO.transform.localRotation = Quaternion.identity;
            newItemGO.name = item.name;
            dressedItems.Add(newItemGO);
        }
        else
        {
            Debug.Log("Cant find slot " + item.slotName);
        }

        CustomizeMenu.RefreshMenu();
    }

    public bool DressOrUndress(AccessoryItem accessoryItem)
    {
        Debug.Log("Dressing item " + accessoryItem.name);

        if(IsItemDressed(accessoryItem))
        {
            UndressAccessoryItem(accessoryItem);
            return false;
        }
        else
        {
            UndressSlot(accessoryItem.slotName);

            DressAccessoryItem(accessoryItem);
            return true;
        }
    }

    public void UndressAll()
    {
        foreach(AccessoryItem item in dressedItems)
        {
            UndressAccessoryItem(item);
        }
    }

    public void UndressAccessoryItem(AccessoryItem item)
    {
        AccessoryItem dressedItem = dressedItems.Find(x => x.slotName == item.slotName);

        if(dressedItem)
        {
            Destroy(dressedItem.gameObject);
            dressedItems.Remove(dressedItem);
        }

        CustomizeMenu.RefreshMenu();
    }

    public void UndressSlot(CharacterSlotName slot)
    {
        AccessoryItem item = dressedItems.Find(x => x.slotName == slot);

        if(item)
        {
            dressedItems.Remove(item);
            Destroy(item.gameObject);
            CustomizeMenu.RefreshMenu();
        }

    }

    #endregion
}
