using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform background;
    public RectTransform handle;
    public float radius = 100f;

    Vector2 inputVector = Vector2.zero;
    public Vector2 InputVector => inputVector;  // �ܺο��� ���� ����

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out position);

        // �Է°��� ���������� ����
        position = Vector2.ClampMagnitude(position, radius);

        // �ڵ� ��ġ ����
        handle.anchoredPosition = position;

        // �Է� ���� ���
        inputVector = position / radius;
    }
}

