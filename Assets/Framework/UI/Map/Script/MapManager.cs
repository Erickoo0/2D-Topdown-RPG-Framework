using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField] private GameObject mapMenuPanel;
    [SerializeField] private GameObject mapTilePanel;
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private Color defaultColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private RectTransform playerIconTransform;
    private string _currentLocationTag;
    
    private List<Image> _mapTiles;
    
    public string CurrentTileName { get; private set; }

    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.unityLogger.Log("Multiple MapManagers detected. Destroying one.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
        // Get all the map tiles 
        _mapTiles = mapTilePanel.GetComponentsInChildren<Image>().ToList();
    }

    public void ToggleMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!mapMenuPanel.activeSelf)EventBus.RequestOpenMenu(mapMenuPanel);
        else if (mapMenuPanel.activeSelf) EventBus.RequestCloseMenu(mapMenuPanel);
    }
    
    public void HighlightTile(string tileName)
    {
        CurrentTileName = tileName;
        
        foreach (Image tile in _mapTiles) tile.color = defaultColor;

        // Find current tile
        Image currentTile = _mapTiles.Find(x => x.name == CurrentTileName);

        // Change color of current tile and move the player icon
        if (currentTile != null)
        {
            currentTile.color = highlightColor;
            playerIconTransform.position = currentTile.GetComponent<RectTransform>().position;
        }
        else
        {
            Debug.unityLogger.Log("Map tile not found");
        }
    }

    public void SetLocationTag(string locationTag)
    {
        _currentLocationTag = locationTag;
    }
}
