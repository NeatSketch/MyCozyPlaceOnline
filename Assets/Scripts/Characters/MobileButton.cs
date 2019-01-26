using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string buttonName;

    public void OnPointerDown(PointerEventData eventData)
    {
        MobileInputController.ButtonDown(buttonName);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MobileInputController.ButtonUp(buttonName);
    }
}
