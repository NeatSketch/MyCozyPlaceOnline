using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
