using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class HexGridDemo : MonoBehaviour
{
    [Header("Demo Settings")]
    [SerializeField] private HexGridGenerator hexGrid;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask hexLayerMask = 1;
    
    [Header("Demo Features")]
    [SerializeField] private bool enableSelection = true;
    [SerializeField] private bool enableHighlighting = true;
    [SerializeField] private bool enablePathfinding = true;
    [SerializeField] private bool enableAreaSelection = true;
    
    [Header("Visual Settings")]
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color highlightedColor = Color.cyan;
    [SerializeField] private Color pathColor = Color.green;
    [SerializeField] private Color areaColor = Color.blue;
    
    private HexTile selectedHex;
    private HexTile hoveredHex;
    private List<HexTile> highlightedHexes = new List<HexTile>();
    private List<HexTile> pathHexes = new List<HexTile>();
    private List<HexTile> areaHexes = new List<HexTile>();
    
    private void Start()
    {
        if (hexGrid == null)
        {
            hexGrid = FindObjectOfType<HexGridGenerator>();
        }
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        SetupEventListeners();
    }
    
    private void SetupEventListeners()
    {
        // Subscribe to hex events
        if (hexGrid != null)
        {
            // You'll need to add these events to HexGridGenerator
            // hexGrid.OnHexCreated += OnHexCreated;
        }
    }
    
    private void Update()
    {
        HandleInput();
    }
    
    private void HandleInput()
    {
        if (!enableSelection && !enableHighlighting) return;
        
        // Mouse input
        if (Mouse.current.leftButton.wasPressedThisFrame && enableSelection)
        {
            HandleSelection();
        }
        
        if (enableHighlighting)
        {
            HandleHovering();
        }
        
        // Keyboard input for demo features
        if (Keyboard.current.pKey.wasPressedThisFrame && enablePathfinding)
        {
            ShowPathToSelected();
        }
        
        if (Keyboard.current.aKey.wasPressedThisFrame && enableAreaSelection)
        {
            ShowAreaAroundSelected();
        }
        
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetAllVisuals();
        }
        
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            ShowNeighborsOfSelected();
        }
        
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            ShowLineToMouse();
        }
        
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            ShowConeFromSelected();
        }
    }
    
    private void HandleSelection()
    {
        HexTile clickedHex = GetHexUnderMouse();
        
        if (clickedHex != null)
        {
            // Deselect previous hex
            if (selectedHex != null)
            {
                selectedHex.SetSelected(false);
            }
            
            // Select new hex
            selectedHex = clickedHex;
            selectedHex.SetSelected(true);
            
            Debug.Log($"Selected hex at {selectedHex.CubeCoordinates}");
        }
    }
    
    private void HandleHovering()
    {
        HexTile currentHoveredHex = GetHexUnderMouse();
        
        // Unhover previous hex
        if (hoveredHex != null && hoveredHex != currentHoveredHex)
        {
            hoveredHex.SetHighlighted(false);
        }
        
        // Hover new hex
        if (currentHoveredHex != null && currentHoveredHex != hoveredHex)
        {
            currentHoveredHex.SetHighlighted(true);
        }
        
        hoveredHex = currentHoveredHex;
    }
    
    private HexTile GetHexUnderMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hexLayerMask))
        {
            HexTile hex = hit.collider.GetComponent<HexTile>();
            return hex;
        }
        
        return null;
    }
    
    private void ShowPathToSelected()
    {
        if (selectedHex == null) return;
        
        ClearPathVisuals();
        
        HexTile targetHex = GetHexUnderMouse();
        if (targetHex != null && targetHex != selectedHex)
        {
            Vector3Int[] path = selectedHex.GetPathTo(targetHex);
            
            foreach (Vector3Int coord in path)
            {
                GameObject hexObj = hexGrid.GetHexAt(coord);
                if (hexObj != null)
                {
                    HexTile hex = hexObj.GetComponent<HexTile>();
                    if (hex != null)
                    {
                        hex.SetColor(pathColor);
                        pathHexes.Add(hex);
                    }
                }
            }
            
            Debug.Log($"Path from {selectedHex.CubeCoordinates} to {targetHex.CubeCoordinates}: {path.Length} steps");
        }
    }
    
    private void ShowAreaAroundSelected()
    {
        if (selectedHex == null) return;
        
        ClearAreaVisuals();
        
        int radius = 2; // You can make this configurable
        Vector3Int[] area = selectedHex.GetArea(radius);
        
        foreach (Vector3Int coord in area)
        {
            GameObject hexObj = hexGrid.GetHexAt(coord);
            if (hexObj != null)
            {
                HexTile hex = hexObj.GetComponent<HexTile>();
                if (hex != null)
                {
                    hex.SetColor(areaColor);
                    areaHexes.Add(hex);
                }
            }
        }
        
        Debug.Log($"Area around {selectedHex.CubeCoordinates} with radius {radius}: {area.Length} hexes");
    }
    
    private void ShowNeighborsOfSelected()
    {
        if (selectedHex == null) return;
        
        ClearHighlightedVisuals();
        
        Vector3Int[] neighbors = selectedHex.GetNeighborCoordinates();
        
        foreach (Vector3Int coord in neighbors)
        {
            GameObject hexObj = hexGrid.GetHexAt(coord);
            if (hexObj != null)
            {
                HexTile hex = hexObj.GetComponent<HexTile>();
                if (hex != null)
                {
                    hex.SetHighlighted(true);
                    highlightedHexes.Add(hex);
                }
            }
        }
        
        Debug.Log($"Neighbors of {selectedHex.CubeCoordinates}: {neighbors.Length} hexes");
    }
    
    private void ShowLineToMouse()
    {
        if (selectedHex == null) return;
        
        ClearPathVisuals();
        
        HexTile targetHex = GetHexUnderMouse();
        if (targetHex != null && targetHex != selectedHex)
        {
            Vector3Int[] line = selectedHex.GetLine(targetHex);
            
            foreach (Vector3Int coord in line)
            {
                GameObject hexObj = hexGrid.GetHexAt(coord);
                if (hexObj != null)
                {
                    HexTile hex = hexObj.GetComponent<HexTile>();
                    if (hex != null)
                    {
                        hex.SetColor(pathColor);
                        pathHexes.Add(hex);
                    }
                }
            }
            
            Debug.Log($"Line from {selectedHex.CubeCoordinates} to {targetHex.CubeCoordinates}: {line.Length} hexes");
        }
    }
    
    private void ShowConeFromSelected()
    {
        if (selectedHex == null) return;
        
        ClearAreaVisuals();
        
        // Get direction to mouse
        HexTile targetHex = GetHexUnderMouse();
        if (targetHex != null && targetHex != selectedHex)
        {
            Vector3Int direction = selectedHex.GetDirectionTo(targetHex);
            int radius = 3;
            float angle = 120f; // 120 degree cone
            
            // Use HexUtils for cone calculation
            List<Vector3Int> cone = HexUtils.GetCone(selectedHex.CubeCoordinates, direction, radius, angle);
            
            foreach (Vector3Int coord in cone)
            {
                GameObject hexObj = hexGrid.GetHexAt(coord);
                if (hexObj != null)
                {
                    HexTile hex = hexObj.GetComponent<HexTile>();
                    if (hex != null)
                    {
                        hex.SetColor(areaColor);
                        areaHexes.Add(hex);
                    }
                }
            }
            
            Debug.Log($"Cone from {selectedHex.CubeCoordinates} in direction {direction}: {cone.Count} hexes");
        }
    }
    
    private void ClearHighlightedVisuals()
    {
        foreach (HexTile hex in highlightedHexes)
        {
            if (hex != null)
            {
                hex.SetHighlighted(false);
            }
        }
        highlightedHexes.Clear();
    }
    
    private void ClearPathVisuals()
    {
        foreach (HexTile hex in pathHexes)
        {
            if (hex != null)
            {
                hex.ResetVisual();
            }
        }
        pathHexes.Clear();
    }
    
    private void ClearAreaVisuals()
    {
        foreach (HexTile hex in areaHexes)
        {
            if (hex != null)
            {
                hex.ResetVisual();
            }
        }
        areaHexes.Clear();
    }
    
    private void ResetAllVisuals()
    {
        ClearHighlightedVisuals();
        ClearPathVisuals();
        ClearAreaVisuals();
        
        if (hoveredHex != null)
        {
            hoveredHex.SetHighlighted(false);
            hoveredHex = null;
        }
        
        if (selectedHex != null)
        {
            selectedHex.SetSelected(false);
            selectedHex = null;
        }
        
        Debug.Log("All visuals reset");
    }
    
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Hex Grid Demo Controls", GUI.skin.box);
        
        GUILayout.Label("Mouse:");
        GUILayout.Label("  Left Click - Select hex");
        GUILayout.Label("  Hover - Highlight hex");
        
        GUILayout.Label("Keyboard:");
        GUILayout.Label("  P - Show path to mouse");
        GUILayout.Label("  A - Show area around selected");
        GUILayout.Label("  N - Show neighbors of selected");
        GUILayout.Label("  L - Show line to mouse");
        GUILayout.Label("  C - Show cone from selected");
        GUILayout.Label("  R - Reset all visuals");
        
        if (selectedHex != null)
        {
            GUILayout.Label($"Selected: {selectedHex.CubeCoordinates}");
        }
        
        if (hoveredHex != null)
        {
            GUILayout.Label($"Hovered: {hoveredHex.CubeCoordinates}");
        }
        
        GUILayout.EndArea();
    }
    
    private void OnDestroy()
    {
        // Clean up event listeners
        if (hexGrid != null)
        {
            // hexGrid.OnHexCreated -= OnHexCreated;
        }
    }
    
    // Example method for handling hex creation events
    private void OnHexCreated(HexTile hex)
    {
        // Set up event listeners for the new hex
        hex.OnHexClicked.AddListener(OnHexClicked);
        hex.OnHexHovered.AddListener(OnHexHovered);
        hex.OnHexUnhovered.AddListener(OnHexUnhovered);
    }
    
    private void OnHexClicked(HexTile hex)
    {
        Debug.Log($"Hex clicked: {hex.CubeCoordinates}");
    }
    
    private void OnHexHovered(HexTile hex)
    {
        Debug.Log($"Hex hovered: {hex.CubeCoordinates}");
    }
    
    private void OnHexUnhovered(HexTile hex)
    {
        Debug.Log($"Hex unhovered: {hex.CubeCoordinates}");
    }
} 