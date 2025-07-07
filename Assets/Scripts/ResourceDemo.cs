using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ResourceDemo : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HexGridGenerator hexGrid;
    [SerializeField] private ResourceGenerator resourceGenerator;
    
    [Header("Demo Controls")]
    [SerializeField] private KeyCode regenerateKey = KeyCode.R;
    [SerializeField] private KeyCode toggleResourceInfoKey = KeyCode.I;
    [SerializeField] private KeyCode extractResourceKey = KeyCode.E;
    [SerializeField] private KeyCode highDensityKey = KeyCode.H;
    [SerializeField] private KeyCode lowDensityKey = KeyCode.L;
    
    // Key mappings for new Input System
    private Key regenerateKeyNew = Key.R;
    private Key toggleResourceInfoKeyNew = Key.I;
    private Key extractResourceKeyNew = Key.E;
    private Key highDensityKeyNew = Key.H;
    private Key lowDensityKeyNew = Key.L;
    
    private HexGridUI hexGridUI;
    private HexGridDemo hexGridDemo;
    private HexTile selectedTile;
    
    private void Start()
    {
        if (hexGrid == null)
            hexGrid = FindObjectOfType<HexGridGenerator>();
            
        if (resourceGenerator == null)
            resourceGenerator = FindObjectOfType<ResourceGenerator>();
            
        hexGridUI = FindObjectOfType<HexGridUI>();
        hexGridDemo = FindObjectOfType<HexGridDemo>();
        
        // Subscribe to grid generation events to setup hex tile events when grid is ready
        if (hexGrid != null)
        {
            hexGrid.OnGridGenerated.AddListener(OnGridGenerated);
        }
        
        // Setup initial hex tile events if grid already exists
        SetupHexTileEvents();
    }
    
    private void SetupHexTileEvents()
    {
        Debug.Log("SetupHexTileEvents called");
        
        if (hexGrid == null) 
        {
            Debug.LogWarning("hexGrid is null in SetupHexTileEvents");
            return;
        }
        
        var hexTiles = hexGrid.GetAllHexTiles();
        if (hexTiles == null) 
        {
            Debug.LogWarning("hexTiles list is null");
            return;
        }
        
        Debug.Log($"Found {hexTiles.Count} hex tiles to setup events for");
        
        int eventCount = 0;
        foreach (var hexTile in hexTiles)
        {
            if (hexTile != null)
            {
                var hexData = hexTile.GetComponent<HexTile>();
                if (hexData != null)
                {
                    hexData.OnHexClicked.AddListener(OnHexTileClicked);
                    eventCount++;
                }
                else
                {
                    Debug.LogWarning($"HexTile component not found on {hexTile.name}");
                }
            }
            else
            {
                Debug.LogWarning("Found null hexTile in list");
            }
        }
        
        Debug.Log($"Successfully setup events for {eventCount} hex tiles");
    }
    
    private void OnGridGenerated()
    {
        // Re-setup hex tile events when grid is regenerated
        SetupHexTileEvents();
    }
    
    private HexTile GetCurrentSelectedTile()
    {
        // Try to get the selected tile from HexGridDemo using reflection
        if (hexGridDemo != null)
        {
            var selectedHexField = typeof(HexGridDemo).GetField("selectedHex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (selectedHexField != null)
            {
                HexTile selectedHex = selectedHexField.GetValue(hexGridDemo) as HexTile;
                if (selectedHex != null)
                {
                    Debug.Log($"Found selected tile from HexGridDemo: {selectedHex.name} at {selectedHex.CubeCoordinates}");
                    return selectedHex;
                }
            }
        }
        
        // Fallback to our own selected tile
        if (selectedTile != null)
        {
            Debug.Log($"Using ResourceDemo's selected tile: {selectedTile.name} at {selectedTile.CubeCoordinates}");
            return selectedTile;
        }
        
        Debug.LogWarning("No selected tile found in either HexGridDemo or ResourceDemo");
        return null;
    }
    
    private void OnHexTileClicked(HexTile hexTile)
    {
        Debug.Log($"OnHexTileClicked called with tile: {hexTile?.name ?? "null"}");
        
        // Deselect previous tile
        if (selectedTile != null)
        {
            Debug.Log($"Deselecting previous tile at {selectedTile.CubeCoordinates}");
            selectedTile.SetSelected(false);
        }
        else
        {
            Debug.Log("No previous tile to deselect");
        }
        
        // Select new tile
        selectedTile = hexTile;
        if (selectedTile != null)
        {
            selectedTile.SetSelected(true);
            Debug.Log($"Selected tile at {hexTile.CubeCoordinates} with resource: {hexTile.ResourceType}");
        }
        else
        {
            Debug.LogError("HexTile parameter is null!");
        }
    }
    
    private void Update()
    {
        HandleInput();
    }
    
    private void HandleInput()
    {
#if ENABLE_INPUT_SYSTEM
        // Use new Input System
        var keyboard = Keyboard.current;
        if (keyboard == null) 
        {
            Debug.LogWarning("Keyboard.current is null - new Input System not properly initialized");
            return;
        }
        
        // Debug: Check if keys are being pressed
        if (keyboard[regenerateKeyNew].wasPressedThisFrame)
        {
            Debug.Log($"Regenerate key ({regenerateKeyNew}) pressed!");
            if (resourceGenerator != null && hexGrid != null)
            {
                resourceGenerator.RegenerateResources(hexGrid);
                Debug.Log("Resources regenerated!");
            }
            else
            {
                Debug.LogWarning($"Cannot regenerate: resourceGenerator={resourceGenerator != null}, hexGrid={hexGrid != null}");
            }
        }
        
        // Extract resource from selected tile
        if (keyboard[extractResourceKeyNew].wasPressedThisFrame)
        {
            Debug.Log($"Extract key ({extractResourceKeyNew}) pressed!");
            
            // Try to get the selected tile from HexGridDemo
            HexTile currentSelectedTile = GetCurrentSelectedTile();
            
            if (currentSelectedTile != null)
            {
                if (currentSelectedTile.HasResource)
                {
                    float extracted = currentSelectedTile.ExtractResource(1f);
                    Debug.Log($"Extracted {extracted} {currentSelectedTile.ResourceType} from tile at {currentSelectedTile.CubeCoordinates}");
                }
                else
                {
                    Debug.Log($"No resource to extract from tile at {currentSelectedTile.CubeCoordinates} (Resource: {currentSelectedTile.ResourceType})");
                }
            }
            else
            {
                Debug.LogWarning("No tile selected for extraction");
            }
        }
        
        // Toggle resource info display
        if (keyboard[toggleResourceInfoKeyNew].wasPressedThisFrame)
        {
            Debug.Log($"Info key ({toggleResourceInfoKeyNew}) pressed!");
            if (hexGridUI != null)
            {
                // This would toggle a debug display - you can implement this as needed
                Debug.Log("Resource info display toggled");
            }
            else
            {
                Debug.LogWarning("hexGridUI is null");
            }
        }
        
        // High density resources
        if (keyboard[highDensityKeyNew].wasPressedThisFrame)
        {
            Debug.Log($"High density key ({highDensityKeyNew}) pressed!");
            if (resourceGenerator != null)
            {
                resourceGenerator.SetResourceDensity(0.8f); // 80% density
                if (hexGrid != null)
                {
                    resourceGenerator.GenerateResources(hexGrid);
                }
            }
        }
        
        // Low density resources
        if (keyboard[lowDensityKeyNew].wasPressedThisFrame)
        {
            Debug.Log($"Low density key ({lowDensityKeyNew}) pressed!");
            if (resourceGenerator != null)
            {
                resourceGenerator.SetResourceDensity(0.2f); // 20% density
                if (hexGrid != null)
                {
                    resourceGenerator.GenerateResources(hexGrid);
                }
            }
        }
        
        // Debug: Log all key states for troubleshooting
        if (keyboard[regenerateKeyNew].isPressed || keyboard[extractResourceKeyNew].isPressed || keyboard[toggleResourceInfoKeyNew].isPressed)
        {
            Debug.Log($"Key states - R: {keyboard[regenerateKeyNew].isPressed}, E: {keyboard[extractResourceKeyNew].isPressed}, I: {keyboard[toggleResourceInfoKeyNew].isPressed}");
        }
        
        // Debug: Log selected tile status
        HexTile debugSelectedTile = GetCurrentSelectedTile();
        if (debugSelectedTile != null)
        {
            Debug.Log($"Selected tile: {debugSelectedTile.name} at {debugSelectedTile.CubeCoordinates}, Resource: {debugSelectedTile.ResourceType}, HasResource: {debugSelectedTile.HasResource}");
        }
#else
        // Use old Input System
        // Regenerate resources
        if (Input.GetKeyDown(regenerateKey))
        {
            Debug.Log($"Regenerate key ({regenerateKey}) pressed! (Old Input System)");
            if (resourceGenerator != null && hexGrid != null)
            {
                resourceGenerator.RegenerateResources(hexGrid);
                Debug.Log("Resources regenerated!");
            }
            else
            {
                Debug.LogWarning($"Cannot regenerate: resourceGenerator={resourceGenerator != null}, hexGrid={hexGrid != null}");
            }
        }
        
        // Extract resource from selected tile
        if (Input.GetKeyDown(extractResourceKey))
        {
            Debug.Log($"Extract key ({extractResourceKey}) pressed! (Old Input System)");
            
            // Try to get the selected tile from HexGridDemo
            HexTile currentSelectedTile = GetCurrentSelectedTile();
            
            if (currentSelectedTile != null)
            {
                if (currentSelectedTile.HasResource)
                {
                    float extracted = currentSelectedTile.ExtractResource(1f);
                    Debug.Log($"Extracted {extracted} {currentSelectedTile.ResourceType} from tile at {currentSelectedTile.CubeCoordinates}");
                }
                else
                {
                    Debug.Log($"No resource to extract from tile at {currentSelectedTile.CubeCoordinates} (Resource: {currentSelectedTile.ResourceType})");
                }
            }
            else
            {
                Debug.LogWarning("No tile selected for extraction");
            }
        }
        
        // Toggle resource info display
        if (Input.GetKeyDown(toggleResourceInfoKey))
        {
            Debug.Log($"Info key ({toggleResourceInfoKey}) pressed! (Old Input System)");
            if (hexGridUI != null)
            {
                // This would toggle a debug display - you can implement this as needed
                Debug.Log("Resource info display toggled");
            }
            else
            {
                Debug.LogWarning("hexGridUI is null");
            }
        }
        
        // High density resources
        if (Input.GetKeyDown(highDensityKey))
        {
            Debug.Log($"High density key ({highDensityKey}) pressed! (Old Input System)");
            if (resourceGenerator != null)
            {
                resourceGenerator.SetResourceDensity(0.8f); // 80% density
                if (hexGrid != null)
                {
                    resourceGenerator.GenerateResources(hexGrid);
                }
            }
        }
        
        // Low density resources
        if (Input.GetKeyDown(lowDensityKey))
        {
            Debug.Log($"Low density key ({lowDensityKey}) pressed! (Old Input System)");
            if (resourceGenerator != null)
            {
                resourceGenerator.SetResourceDensity(0.2f); // 20% density
                if (hexGrid != null)
                {
                    resourceGenerator.GenerateResources(hexGrid);
                }
            }
        }
        
        // Debug: Log all key states for troubleshooting (Old Input System)
        if (Input.GetKey(regenerateKey) || Input.GetKey(extractResourceKey) || Input.GetKey(toggleResourceInfoKey))
        {
            Debug.Log($"Key states (Old) - R: {Input.GetKey(regenerateKey)}, E: {Input.GetKey(extractResourceKey)}, I: {Input.GetKey(toggleResourceInfoKey)}");
        }
        
        // Debug: Log selected tile status (Old Input System)
        HexTile debugSelectedTileOld = GetCurrentSelectedTile();
        if (debugSelectedTileOld != null)
        {
            Debug.Log($"Selected tile: {debugSelectedTileOld.name} at {debugSelectedTileOld.CubeCoordinates}, Resource: {debugSelectedTileOld.ResourceType}, HasResource: {debugSelectedTileOld.HasResource}");
        }
#endif
    }
    
    private void OnGUI()
    {
        // Display controls
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Resource Demo Controls:");
        GUILayout.Label($"{regenerateKey} - Regenerate Resources");
        GUILayout.Label($"{extractResourceKey} - Extract Resource (if tile selected)");
        GUILayout.Label($"{toggleResourceInfoKey} - Toggle Resource Info");
        GUILayout.Label("Click tiles to select them");
        
        if (selectedTile != null)
        {
            GUILayout.Space(10);
            GUILayout.Label($"Selected Tile:");
            GUILayout.Label($"Position: {selectedTile.CubeCoordinates}");
            GUILayout.Label($"Resource: {selectedTile.ResourceType}");
            if (selectedTile.HasResource)
            {
                GUILayout.Label($"Amount: {selectedTile.ResourceAmount:F1}");
            }
        }
        
        GUILayout.EndArea();
    }
} 