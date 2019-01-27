using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolsMenu : MonoBehaviour
{
    

    public Animator animator;

    public Image editWallsBtnImage;
    public Image editModeBtnImage;

    public RoomEditor.WallEditMode wallEditMode;

    public Color colorNeutral = new Color(1f / 236f, 1 / 236, 1f / 236f);
    public Color colorNegative = Color.red;
    public Color colorPositive = Color.green;


    public Stack<string> strings = new Stack<string>();

    void Start()
    {

    }

    void Update()
    {

    }

    public void OpenSubmenu(string name)
    {
        strings.Push(name);
        animator.SetBool(name, true);
    }

    public void BackButton()
    {
        if (!string.IsNullOrEmpty(strings.Peek()))
        {
            animator.SetBool(strings.Pop(), false);
        }
    } 


    public void EditWallsToggle()
    {
        switch(wallEditMode)
        {
            case RoomEditor.WallEditMode.None:
                {
                    wallEditMode = RoomEditor.WallEditMode.Build;
                    editWallsBtnImage.color = colorNeutral;
                }
                break;
            case RoomEditor.WallEditMode.Build:
                {
                    wallEditMode = RoomEditor.WallEditMode.Break;
                    editWallsBtnImage.color = colorNegative;
                }
                break;
            case RoomEditor.WallEditMode.Break:
                {
                    wallEditMode = RoomEditor.WallEditMode.None;
                    editWallsBtnImage.color = colorPositive;
                }
                break;
        }

        RoomEditor.WallMode = wallEditMode;
    }

    public void SwitchEditModeToggle()
    {
        RoomEditor.EditMode ^= true;
        editModeBtnImage.color = (RoomEditor.EditMode) ? colorPositive : Color.gray;
    } 

    
}
