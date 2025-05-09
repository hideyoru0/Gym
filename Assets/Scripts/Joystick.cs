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
    public Vector2 InputVector => inputVector;  // 외부에서 접근 가능

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

        // 입력값을 반지름으로 제한
        position = Vector2.ClampMagnitude(position, radius);

        // 핸들 위치 설정
        handle.anchoredPosition = position;

        // 입력 벡터 계산
        inputVector = position / radius;
    }
}

