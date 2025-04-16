using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{

    public RectTransform joystickBackground;
    public RectTransform joystickPoint;

    private Vector2 inputVector;

    public float Horizontal => inputVector.x;
    public float Vertical => inputVector.y;
    public Vector2 Direction => inputVector;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground, eventData.position, eventData.pressEventCamera, out pos);

        pos /= joystickBackground.sizeDelta / 2;

        inputVector = Vector2.ClampMagnitude(pos, 1.0f);
        joystickPoint.anchoredPosition = new Vector2(
            inputVector.x * (joystickBackground.sizeDelta.x / 2),
            inputVector.y * (joystickBackground.sizeDelta.y / 2));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        joystickPoint.anchoredPosition = Vector2.zero;
    }
}
