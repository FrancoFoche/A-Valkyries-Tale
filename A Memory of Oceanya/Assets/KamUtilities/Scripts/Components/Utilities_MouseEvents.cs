using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class Utilities_MouseEvents : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
{
    [Tooltip("Events that are triggered when the mouse enters the area of the object")]
    public UnityEvent OnPointer_Enter;

    [Tooltip("Events that are triggered when the mouse exits the area of the object")]
    public UnityEvent OnPointer_Exit;

    [Tooltip("Events that are triggered when the mouse clicks the area of the object")]
    public UnityEvent OnPointer_Click;

    public UnityEvent OnPointer_Down;
    public UnityEvent OnPointer_Up;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointer_Click.Invoke();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointer_Down.Invoke();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointer_Enter.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointer_Exit.Invoke();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointer_Up.Invoke();
    }
}
