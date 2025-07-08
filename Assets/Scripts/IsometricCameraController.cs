using UnityEngine;
using UnityEngine.InputSystem;

public class IsometricCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float smoothTime = 0.1f;
    
    [Header("Zoom Limits")]
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;
    
    [Header("Movement Limits")]
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minZ = -50f;
    [SerializeField] private float maxZ = 50f;
    
    [Header("Isometric Settings")]
    [SerializeField] private float isometricAngle = 45f;
    [SerializeField] private bool lockRotation = true;
    
    [Header("References")]
    [SerializeField] private Camera controlledCamera;
    [SerializeField] private HexGridGenerator hexGrid;
    
    // Input variables
    private Vector2 moveInput;
    private float zoomInput;
    private float rotationInput;
    private bool isRotating;
    
    // Smoothing variables
    private Vector3 velocity;
    private float zoomVelocity;
    private float rotationVelocity;
    
    // Target values for smooth movement
    private Vector3 targetPosition;
    private float targetZoom;
    private float targetRotation;
    
    private void Start()
    {
        // Get camera reference if not assigned
        if (controlledCamera == null)
        {
            controlledCamera = GetComponent<Camera>();
            if (controlledCamera == null)
            {
                controlledCamera = Camera.main;
            }
        }
        
        // Initialize target values
        targetPosition = transform.position;
        targetZoom = controlledCamera.orthographicSize;
        targetRotation = transform.eulerAngles.y;
        
        // Set up isometric view
        SetupIsometricView();
    }
    
    private void Update()
    {
        HandleInput();
        UpdateCamera();
    }
    
    private void HandleInput()
    {
        // Keyboard movement
        moveInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                moveInput.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                moveInput.y -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveInput.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveInput.x += 1f;
        }
        
        // Mouse movement (middle mouse button)
        if (Mouse.current != null && Mouse.current.middleButton.isPressed)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            moveInput += mouseDelta * 0.01f;
        }
        
        // Zoom input
        zoomInput = 0f;
        if (Mouse.current != null)
        {
            Vector2 scrollDelta = Mouse.current.scroll.ReadValue();
            zoomInput = scrollDelta.y;
        }
        
        // Rotation input
        rotationInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
                rotationInput -= 60f; // Snap to 60-degree increments for hexagons
            if (Keyboard.current.eKey.wasPressedThisFrame)
                rotationInput += 60f; // Snap to 60-degree increments for hexagons
        }
        
        // Mouse rotation (right mouse button)
        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            rotationInput += mouseDelta.x * 0.1f;
        }
        
        // Reset camera to center hexagon
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetCamera();
        }
    }
    
    private void UpdateCamera()
    {
        // Update target position
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        
        // Convert movement to hexagon-aligned directions
        if (moveDirection.magnitude > 0.1f)
        {
            // Get the current rotation angle in degrees
            float currentAngle = transform.eulerAngles.y;
            
            // Convert to hexagon-aligned movement
            Vector3 hexAlignedDirection = GetHexAlignedDirection(moveDirection, currentAngle);
            targetPosition += hexAlignedDirection * moveSpeed * Time.deltaTime;
        }
        
        // Clamp position to limits
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ);
        
        // Smooth position movement
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        
        // Update target zoom
        targetZoom -= zoomInput * zoomSpeed * Time.deltaTime;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        
        // Smooth zoom
        if (controlledCamera.orthographic)
        {
            controlledCamera.orthographicSize = Mathf.SmoothDamp(controlledCamera.orthographicSize, targetZoom, ref zoomVelocity, smoothTime);
        }
        else
        {
            // For perspective camera, adjust field of view
            float targetFOV = Mathf.Lerp(60f, 20f, (targetZoom - minZoom) / (maxZoom - minZoom));
            controlledCamera.fieldOfView = Mathf.SmoothDamp(controlledCamera.fieldOfView, targetFOV, ref zoomVelocity, smoothTime);
        }
        
        // Update target rotation
        if (!lockRotation)
        {
            if (rotationInput != 0f)
            {
                // Snap to 60-degree increments for hexagon alignment
                targetRotation += rotationInput;
                targetRotation = Mathf.Round(targetRotation / 60f) * 60f;
            }
            
            // Smooth rotation
            Vector3 currentRotation = transform.eulerAngles;
            float smoothRotation = Mathf.SmoothDamp(currentRotation.y, targetRotation, ref rotationVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(isometricAngle, smoothRotation, 0f);
        }
    }
    
    private void SetupIsometricView()
    {
        // Set up isometric angle
        transform.rotation = Quaternion.Euler(isometricAngle, 0f, 0f);
        
        // Set camera to orthographic for better isometric feel
        controlledCamera.orthographic = true;
        controlledCamera.orthographicSize = 10f;
        targetZoom = 10f;
        
        // Set camera position to look at grid center
        if (hexGrid != null)
        {
            Vector3 gridCenter = hexGrid.transform.position;
            transform.position = gridCenter + new Vector3(0, 15f, -15f);
            targetPosition = transform.position;
        }
    }
    
    public void FocusOnGrid(HexGridGenerator grid)
    {
        hexGrid = grid;
        if (hexGrid != null)
        {
            Vector3 gridCenter = hexGrid.transform.position;
            targetPosition = gridCenter + new Vector3(0, 15f, -15f);
        }
    }
    
    public void FocusOnPosition(Vector3 position)
    {
        targetPosition = position + new Vector3(0, 15f, -15f);
    }
    
    public void SetZoom(float zoom)
    {
        targetZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
    }
    
    public void SetRotation(float rotation)
    {
        if (!lockRotation)
        {
            targetRotation = rotation;
        }
    }
    
    public void ResetCamera()
    {
        if (hexGrid != null)
        {
            // Focus on the center hexagon (0,0,0)
            Vector3 centerHexPosition = hexGrid.CubeToWorldPosition(Vector3Int.zero);
            targetPosition = centerHexPosition + new Vector3(0, 15f, -15f);
        }
        else
        {
            // Fallback to grid center if no hex grid
            targetPosition = Vector3.zero + new Vector3(0, 15f, -15f);
        }
        
        SetZoom(10f);
        SetRotation(0f);
    }
    
    // Public properties for external control
    public Vector3 CameraPosition => transform.position;
    public float CameraZoom => controlledCamera.orthographic ? controlledCamera.orthographicSize : controlledCamera.fieldOfView;
    public float CameraRotation => transform.eulerAngles.y;
    
    // Convert movement input to hexagon-aligned directions
    private Vector3 GetHexAlignedDirection(Vector3 input, float currentAngle)
    {
        // Hexagon directions (flat-topped hexagons)
        Vector3[] hexDirections = {
            new Vector3(1, 0, 0),      // Right
            new Vector3(0.5f, 0, 0.866f),  // Top-Right
            new Vector3(-0.5f, 0, 0.866f), // Top-Left
            new Vector3(-1, 0, 0),     // Left
            new Vector3(-0.5f, 0, -0.866f), // Bottom-Left
            new Vector3(0.5f, 0, -0.866f)   // Bottom-Right
        };
        
        // Find the closest hex direction based on input
        Vector3 normalizedInput = input.normalized;
        float bestDot = -1f;
        Vector3 bestDirection = Vector3.zero;
        
        for (int i = 0; i < hexDirections.Length; i++)
        {
            float dot = Vector3.Dot(normalizedInput, hexDirections[i]);
            if (dot > bestDot)
            {
                bestDot = dot;
                bestDirection = hexDirections[i];
            }
        }
        
        // Apply current rotation to the hex direction
        Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
        return rotation * bestDirection;
    }
    
    // Gizmos for debugging movement limits
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minX + maxX) * 0.5f, 0, (minZ + maxZ) * 0.5f);
        Vector3 size = new Vector3(maxX - minX, 1, maxZ - minZ);
        Gizmos.DrawWireCube(center, size);
    }
} 