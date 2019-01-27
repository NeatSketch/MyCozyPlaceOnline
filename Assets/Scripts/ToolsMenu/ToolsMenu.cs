using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolsMenu : MonoBehaviour
{
    

    public Animator animator;

    public Image editWallsBtnImage;
    public Image editModeBtnImage;

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
        switch(RoomEditor.WallMode)
        {
            case RoomEditor.WallEditMode.None:
                {
                    RoomEditor.WallMode = RoomEditor.WallEditMode.Build;
                    editWallsBtnImage.color = colorPositive;
                }
                break;
            case RoomEditor.WallEditMode.Build:
                {
                    RoomEditor.WallMode = RoomEditor.WallEditMode.Break;
                    editWallsBtnImage.color = colorNegative;
                }
                break;
            case RoomEditor.WallEditMode.Break:
                {
                    RoomEditor.WallMode = RoomEditor.WallEditMode.None;
                    editWallsBtnImage.color = colorNeutral;
                }
                break;
        }
    }

    public void SwitchEditModeToggle()
    {
        RoomEditor.EditMode ^= true;
        editModeBtnImage.color = (RoomEditor.EditMode) ? colorPositive : colorNeutral;
    } 

    
}
