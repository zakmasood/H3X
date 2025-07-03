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
    
    [Header("Settings")]
    [SerializeField] private int minRadius = 1;
    [SerializeField] private int maxRadius = 10;
    [SerializeField] private float minHexSize = 0.5f;
    [SerializeField] private float maxHexSize = 2f;
    
    private HexGridGenerator hexGrid;
    
    private void Start()
    {
        hexGrid = FindObjectOfType<HexGridGenerator>();
        if (hexGrid == null)
        {
            Debug.LogError("HexGridGenerator not found in scene!");
            return;
        }
        
        InitializeUI();
        SetupEventListeners();
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
} 