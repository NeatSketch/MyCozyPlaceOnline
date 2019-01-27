using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEditor : MonoBehaviour
{
    public enum WallEditMode
    {
        None,
        Break,
        Build
    }

    static RoomEditor instance;

    public CharacterCamera characterCamera;
    public NetworkClient networkClient;

    public WallEditMode wallEditMode;

    bool editMode;


    public static bool EditMode
    {
        get
        {
            return instance.editMode;
        }

        set
        {
            instance.editMode = value;

            if(value)
            {
                CharacterCamera.ToEditMode();
            }
            else
            {
                CharacterCamera.ToCharacterMode();
            }

            CharacterMovement.CanControl = !value;

        }
    }

    public static WallEditMode WallMode
    {
        get
        {
            return instance.wallEditMode;
        }

        set
        {
            instance.wallEditMode = value;
        }
    }

    Camera mainCam;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

        mainCam = Camera.main;

    }

    private void Update()
    {
        if (EditMode)
        {
            bool touch = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
            bool click = Input.GetMouseButtonDown(0);

            Vector2 inpPos = Input.mousePosition;

            if (click)
            {
                Ray ray = mainCam.ScreenPointToRay(inpPos);

                Plane p = new Plane(Vector3.up, Vector3.zero);

                float e = 0;

                if (p.Raycast(ray, out e))
                {
                    Vector3 pos = ray.GetPoint(e);
                    int posX = Mathf.RoundToInt(pos.x);
                    int posY = Mathf.RoundToInt(pos.y);

                    switch(WallMode)
                    {
                        case WallEditMode.Break:
                            networkClient.SetBlock(posX, posY, (int)WorldMap.BlockType.Empty);
                            break;
                        case WallEditMode.Build:
                            networkClient.SetBlock(posX, posY, (int)WorldMap.BlockType.Wall);
                            break;
                    }
                }
            }
        }
    }
}
