using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HexGridUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider radiusSlider;
    [SerializeField] private TextMeshProUGUI radiusText;
    [SerializeField] private Slider hexSizeSlider;
    [SerializeField] private TextMeshProUGUI hexSizeText;
    [SerializeField] private Button generateButton;
    [SerializeField] private Button clearButton;
    [SerializeField] private Toggle showCoordinatesToggle;
    [SerializeField] private Toggle showGridLinesToggle;
    [SerializeField] private Color hexColor = Color.white;
    [SerializeField] private Color borderColor = Color.black;
    
    [Header("Resource UI")]
    [SerializeField] private TextMeshProUGUI selectedTileInfo;
    [SerializeField] private Button regenerateResourcesButton;
    [SerializeField] private Slider noiseScaleSlider;
    [SerializeField] private TextMeshProUGUI noiseScaleText;
    [SerializeField] private Slider ironThresholdSlider;
    [SerializeField] private TextMeshProUGUI ironThresholdText;
    [SerializeField] private Slider copperThresholdSlider;
    [SerializeField] private TextMeshProUGUI copperThresholdText;
    [SerializeField] private Slider coalThresholdSlider;
    [SerializeField] private TextMeshProUGUI coalThresholdText;
    
    [Header("Settings")]
    [SerializeField] private int minRadius = 1;
    [SerializeField] private int maxRadius = 10;
    [SerializeField] private float minHexSize = 0.5f;
    [SerializeField] private float maxHexSize = 2f;
    
    private HexGridGenerator hexGrid;
    private ResourceGenerator resourceGenerator;
    private HexTile selectedTile;
    
    private void Start()
    {
        hexGrid = FindObjectOfType<HexGridGenerator>();
        if (hexGrid == null)
        {
            Debug.LogError("HexGridGenerator not found in scene!");
            return;
        }
        
        resourceGenerator = FindObjectOfType<ResourceGenerator>();
        if (resourceGenerator == null)
        {
            Debug.LogError("ResourceGenerator not found in scene!");
            return;
        }
        
        InitializeUI();
        SetupEventListeners();
        SetupResourceUI();
        
        // Subscribe to grid generation events
        if (hexGrid != null)
        {
            hexGrid.OnGridGenerated.AddListener(OnGridGenerated);
        }
    }
    
    private void InitializeUI()
    {
        // Initialize radius slider
        if (radiusSlider != null)
        {
            radiusSlider.minValue = minRadius;
            radiusSlider.maxValue = maxRadius;
            radiusSlider.value = 3; // Default radius
            UpdateRadiusText();
        }
        
        // Initialize hex size slider
        if (hexSizeSlider != null)
        {
            hexSizeSlider.minValue = minHexSize;
            hexSizeSlider.maxValue = maxHexSize;
            hexSizeSlider.value = 1f; // Default hex size
            UpdateHexSizeText();
        }
        
        // Initialize toggles
        if (showCoordinatesToggle != null)
        {
            showCoordinatesToggle.isOn = true;
        }
        
        if (showGridLinesToggle != null)
        {
            showGridLinesToggle.isOn = true;
        }
    }
    
    private void SetupEventListeners()
    {
        // Radius slider
        if (radiusSlider != null)
        {
            radiusSlider.onValueChanged.AddListener(OnRadiusChanged);
        }
        
        // Hex size slider
        if (hexSizeSlider != null)
        {
            hexSizeSlider.onValueChanged.AddListener(OnHexSizeChanged);
        }
        
        // Generate button
        if (generateButton != null)
        {
            generateButton.onClick.AddListener(OnGenerateClicked);
        }
        
        // Clear button
        if (clearButton != null)
        {
            clearButton.onClick.AddListener(OnClearClicked);
        }
        
        // Show coordinates toggle
        if (showCoordinatesToggle != null)
        {
            showCoordinatesToggle.onValueChanged.AddListener(OnShowCoordinatesChanged);
        }
        
        // Show grid lines toggle
        if (showGridLinesToggle != null)
        {
            showGridLinesToggle.onValueChanged.AddListener(OnShowGridLinesChanged);
        }
        
        // Resource UI event listeners
        if (regenerateResourcesButton != null)
        {
            regenerateResourcesButton.onClick.AddListener(OnRegenerateResourcesClicked);
        }
        
        if (noiseScaleSlider != null)
        {
            noiseScaleSlider.onValueChanged.AddListener(OnNoiseScaleChanged);
        }
        
        if (ironThresholdSlider != null)
        {
            ironThresholdSlider.onValueChanged.AddListener(OnIronThresholdChanged);
        }
        
        if (copperThresholdSlider != null)
        {
            copperThresholdSlider.onValueChanged.AddListener(OnCopperThresholdChanged);
        }
        
        if (coalThresholdSlider != null)
        {
            coalThresholdSlider.onValueChanged.AddListener(OnCoalThresholdChanged);
        }
        
        // Color settings can be changed in the inspector
        // You can add UI color pickers here if needed
    }
    
    private void OnRadiusChanged(float value)
    {
        int radius = Mathf.RoundToInt(value);
        UpdateRadiusText();
        
        if (hexGrid != null)
        {
            hexGrid.SetRadius(radius);
        }
    }
    
    private void OnHexSizeChanged(float value)
    {
        UpdateHexSizeText();
        
        if (hexGrid != null)
        {
            // You'll need to add a SetHexSize method to HexGridGenerator
            // hexGrid.SetHexSize(value);
        }
    }
    
    private void OnGenerateClicked()
    {
        if (hexGrid != null)
        {
            hexGrid.GenerateHexGrid();
        }
    }
    
    private void OnClearClicked()
    {
        if (hexGrid != null)
        {
            // You'll need to add a ClearGrid method to HexGridGenerator
            // hexGrid.ClearGrid();
        }
    }
    
    private void OnShowCoordinatesChanged(bool show)
    {
        if (hexGrid != null)
        {
            // You'll need to add a SetShowCoordinates method to HexGridGenerator
            // hexGrid.SetShowCoordinates(show);
        }
    }
    
    private void OnShowGridLinesChanged(bool show)
    {
        if (hexGrid != null)
        {
            // You'll need to add a SetShowGridLines method to HexGridGenerator
            // hexGrid.SetShowGridLines(show);
        }
    }
    
    public void SetHexColor(Color color)
    {
        hexColor = color;
        if (hexGrid != null)
        {
            // You'll need to add a SetHexColor method to HexGridGenerator
            // hexGrid.SetHexColor(color);
        }
    }
    
    public void SetBorderColor(Color color)
    {
        borderColor = color;
        if (hexGrid != null)
        {
            // You'll need to add a SetBorderColor method to HexGridGenerator
            // hexGrid.SetBorderColor(color);
        }
    }
    
    private void UpdateRadiusText()
    {
        if (radiusText != null && radiusSlider != null)
        {
            radiusText.text = $"Radius: {Mathf.RoundToInt(radiusSlider.value)}";
        }
    }
    
    private void UpdateHexSizeText()
    {
        if (hexSizeText != null && hexSizeSlider != null)
        {
            hexSizeText.text = $"Hex Size: {hexSizeSlider.value:F2}";
        }
    }
    
    public void SetRadius(int radius)
    {
        if (radiusSlider != null)
        {
            radiusSlider.value = Mathf.Clamp(radius, minRadius, maxRadius);
        }
    }
    
    public void SetHexSize(float size)
    {
        if (hexSizeSlider != null)
        {
            hexSizeSlider.value = Mathf.Clamp(size, minHexSize, maxHexSize);
        }
    }
    
    public int GetCurrentRadius()
    {
        return radiusSlider != null ? Mathf.RoundToInt(radiusSlider.value) : 3;
    }
    
    public float GetCurrentHexSize()
    {
        return hexSizeSlider != null ? hexSizeSlider.value : 1f;
    }
    
    public Color GetCurrentHexColor()
    {
        return hexColor;
    }
    
    public Color GetCurrentBorderColor()
    {
        return borderColor;
    }
    
    private void SetupResourceUI()
    {
        // Initialize noise scale slider
        if (noiseScaleSlider != null)
        {
            noiseScaleSlider.minValue = 10f;
            noiseScaleSlider.maxValue = 200f;
            noiseScaleSlider.value = 50f;
            UpdateNoiseScaleText();
        }
        
        // Initialize iron threshold slider
        if (ironThresholdSlider != null)
        {
            ironThresholdSlider.minValue = 0.1f;
            ironThresholdSlider.maxValue = 0.9f;
            ironThresholdSlider.value = 0.2f;
            UpdateIronThresholdText();
        }
        
        // Initialize copper threshold slider
        if (copperThresholdSlider != null)
        {
            copperThresholdSlider.minValue = 0.1f;
            copperThresholdSlider.maxValue = 0.9f;
            copperThresholdSlider.value = 0.4f;
            UpdateCopperThresholdText();
        }
        
        // Initialize coal threshold slider
        if (coalThresholdSlider != null)
        {
            coalThresholdSlider.minValue = 0.1f;
            coalThresholdSlider.maxValue = 0.9f;
            coalThresholdSlider.value = 0.6f;
            UpdateCoalThresholdText();
        }
        
        // Setup hex tile click events
        SetupHexTileEvents();
    }
    
    private void SetupHexTileEvents()
    {
        // Find all hex tiles and subscribe to their click events
        var hexTiles = hexGrid.GetAllHexTiles();
        foreach (var hexTile in hexTiles)
        {
            if (hexTile != null)
            {
                var hexData = hexTile.GetComponent<HexTile>();
                if (hexData != null)
                {
                    hexData.OnHexClicked.AddListener(OnHexTileClicked);
                }
            }
        }
    }
    
    private void OnHexTileClicked(HexTile hexTile)
    {
        // Deselect previous tile
        if (selectedTile != null)
        {
            selectedTile.SetSelected(false);
        }
        
        // Select new tile
        selectedTile = hexTile;
        selectedTile.SetSelected(true);
        
        // Update UI with tile information
        UpdateSelectedTileInfo();
    }
    
    private void UpdateSelectedTileInfo()
    {
        if (selectedTileInfo == null || selectedTile == null) return;
        
        string info = $"Selected Tile:\n";
        info += $"Coordinates: ({selectedTile.CubeCoordinates.x}, {selectedTile.CubeCoordinates.y}, {selectedTile.CubeCoordinates.z})\n";
        info += $"Resource: {selectedTile.ResourceType}\n";
        
        if (selectedTile.HasResource)
        {
            info += $"Amount: {selectedTile.ResourceAmount:F1}";
        }
        
        selectedTileInfo.text = info;
    }
    
    private void OnRegenerateResourcesClicked()
    {
        if (resourceGenerator != null && hexGrid != null)
        {
            resourceGenerator.RegenerateResources(hexGrid);
            UpdateSelectedTileInfo(); // Refresh info if a tile is selected
        }
    }
    
    private void OnNoiseScaleChanged(float value)
    {
        UpdateNoiseScaleText();
        if (resourceGenerator != null)
        {
            resourceGenerator.SetNoiseParameters(value, ironThresholdSlider.value, copperThresholdSlider.value, coalThresholdSlider.value);
        }
    }
    
    private void OnIronThresholdChanged(float value)
    {
        UpdateIronThresholdText();
        if (resourceGenerator != null)
        {
            resourceGenerator.SetNoiseParameters(noiseScaleSlider.value, value, copperThresholdSlider.value, coalThresholdSlider.value);
        }
    }
    
    private void OnCopperThresholdChanged(float value)
    {
        UpdateCopperThresholdText();
        if (resourceGenerator != null)
        {
            resourceGenerator.SetNoiseParameters(noiseScaleSlider.value, ironThresholdSlider.value, value, coalThresholdSlider.value);
        }
    }
    
    private void OnCoalThresholdChanged(float value)
    {
        UpdateCoalThresholdText();
        if (resourceGenerator != null)
        {
            resourceGenerator.SetNoiseParameters(noiseScaleSlider.value, ironThresholdSlider.value, copperThresholdSlider.value, value);
        }
    }
    
    private void UpdateNoiseScaleText()
    {
        if (noiseScaleText != null && noiseScaleSlider != null)
        {
            noiseScaleText.text = $"Noise Scale: {noiseScaleSlider.value:F0}";
        }
    }
    
    private void UpdateIronThresholdText()
    {
        if (ironThresholdText != null && ironThresholdSlider != null)
        {
            ironThresholdText.text = $"Iron Threshold: {ironThresholdSlider.value:F2}";
        }
    }
    
    private void UpdateCopperThresholdText()
    {
        if (copperThresholdText != null && copperThresholdSlider != null)
        {
            copperThresholdText.text = $"Copper Threshold: {copperThresholdSlider.value:F2}";
        }
    }
    
    private void UpdateCoalThresholdText()
    {
        if (coalThresholdText != null && coalThresholdSlider != null)
        {
            coalThresholdText.text = $"Coal Threshold: {coalThresholdSlider.value:F2}";
        }
    }
    
    private void OnGridGenerated()
    {
        // Re-setup hex tile events when grid is regenerated
        SetupHexTileEvents();
    }
} 