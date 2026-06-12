using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResponseHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ResponseData Response { get; set; }
    public Action<ResponseData> OnHoverEnter { get; set; }
    public Action OnHoverExit { get; set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Hover Enter on button '{Response?.text}' of type '{Response?.type}'");
        OnHoverEnter?.Invoke(Response);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"Hover Exit from button '{Response?.text}'");
        OnHoverExit?.Invoke();
    }
}
