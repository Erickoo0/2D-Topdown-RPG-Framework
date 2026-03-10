using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles the Visual and Logic movement of an item between slots
/// </summary>
public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform _originalSlot; // Remembers where the item started
    private CanvasGroup _canvasGroup; // Used to make the item transparent to raycasts

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalSlot = transform.parent; // Save OG parent
        
        // 2. Move item to root of UI
        //Ensures item renders on top of all other slots
        transform.SetParent(transform.root);
        
        // 3. Disable raycasts so the mouse sees through the item
        // If we dont do this, the mouse will not detect anything behind the item, like new slot
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; // Follow the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 1. Re-enable raycasts so the item can be dragged again later
        _canvasGroup.blocksRaycasts = true; 
        _canvasGroup.alpha = 1f; 
        
        // 2. Check what is directly under the mouse cursor
        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();
        Slot originalSlot = _originalSlot.GetComponent<Slot>();

        // 3. Check if item is dropped over a valid drop slot
        if (dropSlot != null)
        {
            // Checks if drop slot has an item
            if (dropSlot.currentItem != null)
            {
                // Take the item in the drop slot, and moves it to the original slot.
                dropSlot.currentItem.transform.SetParent(_originalSlot.transform); 
                originalSlot.currentItem = dropSlot.currentItem; 
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; 
            }
            else
            {
                // If drop slot is empty, original slot is now empty
                dropSlot.currentItem = null;
            }
            
            // 4. Finalize item move to drop slot
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            // 5. If no drop slot hovered, return to original slot
            transform.SetParent(_originalSlot);
        }
        
        // Ensures the item is perfectly centered to slot
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

}
