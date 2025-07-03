using UnityEngine;
using TMPro;

public class CameraControlsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI controlsText;
    [SerializeField] private TextMeshProUGUI cameraInfoText;
    [SerializeField] private IsometricCameraController cameraController;
    
    [Header("Settings")]
    [SerializeField] private bool showControls = true;
    [SerializeField] private bool showCameraInfo = true;
    
    private void Start()
    {
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<IsometricCameraController>();
        }
    }
    
    private void Update()
    {
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        if (showControls && controlsText != null)
        {
            controlsText.text = "Camera Controls:\n" +
                               "WASD / Arrow Keys - Move Camera\n" +
                               "Mouse Wheel - Zoom In/Out\n" +
                               "Middle Mouse - Pan Camera\n" +
                               "Right Mouse - Rotate Camera\n" +
                               "Q/E - Rotate Left/Right\n" +
                               "R - Reset Camera";
        }
        
        if (showCameraInfo && cameraInfoText != null && cameraController != null)
        {
            cameraInfoText.text = $"Camera Info:\n" +
                                 $"Position: {cameraController.CameraPosition}\n" +
                                 $"Zoom: {cameraController.CameraZoom:F1}\n" +
                                 $"Rotation: {cameraController.CameraRotation:F1}°";
        }
    }
    
    public void ToggleControls()
    {
        showControls = !showControls;
        if (controlsText != null)
        {
            controlsText.gameObject.SetActive(showControls);
        }
    }
    
    public void ToggleCameraInfo()
    {
        showCameraInfo = !showCameraInfo;
        if (cameraInfoText != null)
        {
            cameraInfoText.gameObject.SetActive(showCameraInfo);
        }
    }
    
    public void ResetCamera()
    {
        if (cameraController != null)
        {
            cameraController.ResetCamera();
        }
    }
} 