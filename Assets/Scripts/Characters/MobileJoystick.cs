using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform Background;
    public RectTransform Knob;

    public Vector2 PointPosition { get; private set; }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        PointPosition = new Vector2(
            (eventData.position.x - Background.position.x) / ((Background.rect.size.x) / 2),
            (eventData.position.y - Background.position.y) / ((Background.rect.size.y) / 2)
            );

        PointPosition = (PointPosition.magnitude > 1.0f) ? PointPosition.normalized : PointPosition;

        float x = PointPosition.x * Background.rect.size.x / 2f;
        float y = PointPosition.y * Background.rect.size.y / 2f;

        Knob.position = new Vector3(x, y) + Background.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        PointPosition = new Vector2(0f, 0f);
        Knob.anchoredPosition = Background.anchoredPosition;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnEndDrag(eventData);
    }
}
