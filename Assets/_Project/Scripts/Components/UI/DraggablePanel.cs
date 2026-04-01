using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _rectTransform;
    private Canvas _canvas;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _rectTransform.SetAsLastSibling(); // Bring the dragged panel to the top of the stack
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Add the mouse delta (movement) to the panel's position.
        // Dividing by the canvas scale factor ensures the panel stays exactly under the mouse,
        // regardless of whether your canvas scales with screen size.
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        return;
    }
}
