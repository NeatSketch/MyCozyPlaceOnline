using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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


    public static WallEditMode WallMode
    {
        get
        {
            return instance.wallEditMode;
        }

        set
        {
            instance.wallEditMode = value;

            if (value != WallEditMode.None)
            {
                CharacterCamera.ToEditMode();
            }
            else
            {
                CharacterCamera.ToCharacterMode();
            }

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
        if (WallMode != WallEditMode.None)
        {
            bool touch = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
            bool click = Input.GetMouseButtonDown(0);

            Vector2 inpPos = Input.mousePosition;

            if (click)
            {
                foreach (Touch t in Input.touches)
                {
                    int id = t.fingerId;
                    if (EventSystem.current.IsPointerOverGameObject(id))
                    {
                        return;
                    }
                }

                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                Ray ray = mainCam.ScreenPointToRay(inpPos);

                Plane p = new Plane(Vector3.up, Vector3.zero);

                float e = 0;

                if (p.Raycast(ray, out e))
                {
                    Vector3 pos = ray.GetPoint(e);
                    int posX = Mathf.RoundToInt(pos.x);
                    int posZ = Mathf.RoundToInt(pos.z);

                    Debug.LogFormat("Edit x: {0} z: {1} mode: {2}", posX, posZ, WallMode);
                    var chunk = WorldMap.ChunkFromPosition(0, posX, posX);


                    switch (WallMode)
                    {
                        case WallEditMode.Break:
                            networkClient.SetBlock(posX, posZ, (int)WorldMap.BlockType.Empty);
                            break;
                        case WallEditMode.Build:
                            networkClient.SetBlock(posX, posZ, (int)WorldMap.BlockType.Wall);
                            //chunk.FakeCreate(posX, posZ,  )

                            break;

                    }
                }
            }
        }
    }
}
