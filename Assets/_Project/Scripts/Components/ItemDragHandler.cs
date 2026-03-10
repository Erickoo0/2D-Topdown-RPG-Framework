using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    
    [Header("References")]
    [SerializeField] private RectTransform iconTransform; // Drag the ItemIcon child here
    private Slot _thisSlot;
    private Vector2 _originalIconLocalPos;

    private void Awake()
    {
        _thisSlot = GetComponent<Slot>();
        _canvas = GetComponentInParent<Canvas>();
        
        // We need a CanvasGroup on the ICON to make it transparent to raycasts
        _canvasGroup = iconTransform.GetComponent<CanvasGroup>();
        if (_canvasGroup == null) _canvasGroup = iconTransform.gameObject.AddComponent<CanvasGroup>();
        
        _originalIconLocalPos = iconTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1. Validation: Only drag if the slot has data
        if (_thisSlot.itemData == null)
        {
            eventData.pointerDrag = null;
            return;
        }

        // 2. Visuals: Make icon semi-transparent and pop to the front of the UI
        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false; // Essential: allows "seeing" the slot behind the icon
        
        iconTransform.SetParent(_canvas.transform); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 3. Movement: Follow mouse correctly regardless of UI scale
        iconTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 4. Reset Visuals
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        
        // Return icon to its home inside this slot
        iconTransform.SetParent(transform); 
        iconTransform.anchoredPosition = _originalIconLocalPos;

        // 5. Logic: Determine drop location
        GameObject hoveredGo = eventData.pointerCurrentRaycast.gameObject;
        
        if (hoveredGo != null)
        {
            Slot targetSlot = hoveredGo.GetComponentInParent<Slot>();

            if (targetSlot != null && targetSlot != _thisSlot)
            {
                // Execute the swap through our Singleton Brain
                InventoryManager.Instance.SwapItems(_thisSlot.slotIndex, targetSlot.slotIndex);
            }
        }
    }
}