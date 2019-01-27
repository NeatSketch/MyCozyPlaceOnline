using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolsMenu : MonoBehaviour
{
    public enum WallEditMode
    {
        None,
        Break,
        Build
    }

    public Animator animator;

    public Image editWallsBtnImage;

    public WallEditMode wallEditMode;

    public Color colorEditWallsBreak = Color.red;
    public Color colorEditWallsBuild = Color.green;


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
            case WallEditMode.None:
                {
                    wallEditMode = WallEditMode.Build;
                    editWallsBtnImage.color = colorEditWallsBuild;
                }
                break;
            case WallEditMode.Build:
                {
                    wallEditMode = WallEditMode.Break;
                    editWallsBtnImage.color = colorEditWallsBreak;
                }
                break;
            case WallEditMode.Break:
                {
                    wallEditMode = WallEditMode.Build;
                    editWallsBtnImage.color = colorEditWallsBuild;
                }
                break;
        }
    }

    
}
