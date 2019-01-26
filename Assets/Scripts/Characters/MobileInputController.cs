using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UnityEngine.UI.AspectRatioFitter))]
public class MobileInputController : MonoBehaviour
{
    static MobileInputController instance;

    public MobileJoystick mobileJoystick;

    public List<string> buttonsDown = new List<string>();

    public static bool HasInstance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public Vector2 Coordinate()
    {
        return mobileJoystick.PointPosition;
    }

    public static Vector2 GetCoordinates()
    {
        if (HasInstance)
        {
            return instance.Coordinate();
        }
        else
        {
            return Vector2.zero;
        }
    }

    public static void ButtonDown(string name)
    {
        if (HasInstance && !instance.buttonsDown.Contains(name))
        {
            instance.buttonsDown.Add(name);
        }
    }

    public static void ButtonUp(string name)
    {
        if(HasInstance && instance.buttonsDown.Contains(name))
        {
            instance.buttonsDown.Remove(name);
        }
    }    

    public static bool GetButton(string name)
    {
        if(HasInstance)
        {
            return instance.buttonsDown.Contains(name);
        }
        else
        {
            return false;
        }
    }
}
