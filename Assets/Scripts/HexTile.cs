using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public enum ResourceType
{
    Grass,
    Iron,
    Copper,
    Coal
}

public class HexTile : MonoBehaviour
{
    [Header("Hex Data")]
    [SerializeField] private Vector3Int cubeCoordinates;
    [SerializeField] private Vector3 worldPosition;
    
    [Header("Resource Data")]
    [SerializeField] private ResourceType resourceType = ResourceType.Grass;
    [SerializeField] private float resourceAmount = 0f;
    [SerializeField] private bool hasResource = false;
    
    [Header("Visual State")]
    [SerializeField] private bool isSelected = false;
    [SerializeField] private bool isHighlighted = false;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color highlightedColor = Color.cyan;
    
    // Resource colors
    [Header("Resource Colors")]
    [SerializeField] private Color grassColor = new Color(0.2f, 0.8f, 0.2f); // Green
    [SerializeField] private Color ironColor = new Color(0.6f, 0.6f, 0.6f); // Gray
    [SerializeField] private Color copperColor = new Color(0.8f, 0.4f, 0.2f); // Orange
    [SerializeField] private Color coalColor = new Color(0.1f, 0.1f, 0.1f); // Black
    
    [Header("Events")]
    public UnityEvent<HexTile> OnHexClicked;
    public UnityEvent<HexTile> OnHexHovered;
    public UnityEvent<HexTile> OnHexUnhovered;
    
    private MeshRenderer meshRenderer;
    private Material originalMaterial;
    
    public Vector3Int CubeCoordinates => cubeCoordinates;
    public Vector3 WorldPosition => worldPosition;
    public bool IsSelected => isSelected;
    public bool IsHighlighted => isHighlighted;
    public ResourceType ResourceType => resourceType;
    public float ResourceAmount => resourceAmount;
    public bool HasResource => hasResource;
    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            originalMaterial = meshRenderer.material;
        }
    }
    
    public void Initialize(Vector3Int cubeCoord, Vector3 worldPos)
    {
        cubeCoordinates = cubeCoord;
        worldPosition = worldPos;
    }
    
    private void OnMouseDown()
    {
        OnHexClicked?.Invoke(this);
    }
    
    private void OnMouseEnter()
    {
        OnHexHovered?.Invoke(this);
    }
    
    private void OnMouseExit()
    {
        OnHexUnhovered?.Invoke(this);
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisual();
    }
    
    public void SetHighlighted(bool highlighted)
    {
        isHighlighted = highlighted;
        UpdateVisual();
    }
    
    private void UpdateVisual()
    {
        if (meshRenderer == null) return;
        
        Color targetColor = defaultColor;
        
        if (isSelected)
        {
            targetColor = selectedColor;
        }
        else if (isHighlighted)
        {
            targetColor = highlightedColor;
        }
        
        if (meshRenderer.material != null)
        {
            meshRenderer.material.color = targetColor;
        }
    }
    
    public void SetColor(Color color)
    {
        defaultColor = color;
        if (!isSelected && !isHighlighted)
        {
            UpdateVisual();
        }
    }
    
    public void SetResource(ResourceType type, float amount = 1f)
    {
        resourceType = type;
        resourceAmount = amount;
        hasResource = type != ResourceType.Grass;
        
        // Update default color based on resource type
        switch (type)
        {
            case ResourceType.Grass:
                defaultColor = grassColor;
                break;
            case ResourceType.Iron:
                defaultColor = ironColor;
                break;
            case ResourceType.Copper:
                defaultColor = copperColor;
                break;
            case ResourceType.Coal:
                defaultColor = coalColor;
                break;
        }
        
        if (!isSelected && !isHighlighted)
        {
            UpdateVisual();
        }
    }
    
    public void ClearResource()
    {
        SetResource(ResourceType.Grass, 0f);
    }
    
    public bool CanExtractResource(float amount = 1f)
    {
        // For infinite harvesting, always return true if the tile has a resource
        return hasResource && resourceType != ResourceType.Grass;
    }
    
    public float ExtractResource(float amount = 1f)
    {
        if (!CanExtractResource(amount))
        {
            return 0f;
        }
        
        // For infinite harvesting, always return the requested amount without reducing the tile's resource
        // The resource amount stays the same, allowing infinite extraction
        return amount;
    }
    
    public void ResetVisual()
    {
        isSelected = false;
        isHighlighted = false;
        UpdateVisual();
    }
    
    public Vector3Int[] GetNeighborCoordinates()
    {
        Vector3Int[] neighbors = new Vector3Int[6];
        
        // Cube coordinate directions
        neighbors[0] = cubeCoordinates + new Vector3Int(1, -1, 0);   // Right
        neighbors[1] = cubeCoordinates + new Vector3Int(1, 0, -1);   // Top-Right
        neighbors[2] = cubeCoordinates + new Vector3Int(0, 1, -1);   // Top-Left
        neighbors[3] = cubeCoordinates + new Vector3Int(-1, 1, 0);   // Left
        neighbors[4] = cubeCoordinates + new Vector3Int(-1, 0, 1);   // Bottom-Left
        neighbors[5] = cubeCoordinates + new Vector3Int(0, -1, 1);   // Bottom-Right
        
        return neighbors;
    }
    
    public float GetDistanceTo(HexTile other)
    {
        return GetDistanceTo(other.cubeCoordinates);
    }
    
    public float GetDistanceTo(Vector3Int otherCubeCoord)
    {
        return (Mathf.Abs(cubeCoordinates.x - otherCubeCoord.x) + 
                Mathf.Abs(cubeCoordinates.y - otherCubeCoord.y) + 
                Mathf.Abs(cubeCoordinates.z - otherCubeCoord.z)) / 2f;
    }
    
    public bool IsNeighborOf(HexTile other)
    {
        return GetDistanceTo(other) == 1f;
    }
    
    public bool IsNeighborOf(Vector3Int otherCubeCoord)
    {
        return GetDistanceTo(otherCubeCoord) == 1f;
    }
    
    public Vector3Int GetDirectionTo(HexTile other)
    {
        return other.cubeCoordinates - cubeCoordinates;
    }
    
    public Vector3Int GetDirectionTo(Vector3Int otherCubeCoord)
    {
        return otherCubeCoord - cubeCoordinates;
    }
    
    public Vector3Int GetOppositeDirection(Vector3Int direction)
    {
        return -direction;
    }
    
    public Vector3Int GetRotatedDirection(Vector3Int direction, int steps)
    {
        Vector3Int[] directions = {
            new Vector3Int(1, -1, 0), new Vector3Int(1, 0, -1), new Vector3Int(0, 1, -1),
            new Vector3Int(-1, 1, 0), new Vector3Int(-1, 0, 1), new Vector3Int(0, -1, 1)
        };
        
        int currentIndex = System.Array.IndexOf(directions, direction);
        if (currentIndex == -1) return direction;
        
        int newIndex = (currentIndex + steps + 6) % 6;
        return directions[newIndex];
    }
    
    public Vector3Int GetRotatedDirection(Vector3Int direction, bool clockwise)
    {
        return GetRotatedDirection(direction, clockwise ? 1 : -1);
    }
    
    public Vector3Int GetRotatedDirection(Vector3Int direction, float angleDegrees)
    {
        int steps = Mathf.RoundToInt(angleDegrees / 60f);
        return GetRotatedDirection(direction, steps);
    }
    
    public Vector3Int GetLineTo(Vector3Int target, int maxDistance = -1)
    {
        Vector3Int direction = target - cubeCoordinates;
        int distance = Mathf.RoundToInt((Mathf.Abs(direction.x) + Mathf.Abs(direction.y) + Mathf.Abs(direction.z)) / 2f);
        
        if (maxDistance > 0 && distance > maxDistance)
        {
            direction = Vector3Int.RoundToInt((Vector3)direction * maxDistance / distance);
        }
        
        return cubeCoordinates + direction;
    }
    
    public Vector3Int GetLineTo(HexTile target, int maxDistance = -1)
    {
        return GetLineTo(target.cubeCoordinates, maxDistance);
    }
    
    public Vector3Int[] GetLine(Vector3Int target, int maxDistance = -1)
    {
        Vector3Int endPoint = GetLineTo(target, maxDistance);
        int distance = Mathf.RoundToInt(GetDistanceTo(endPoint));
        
        Vector3Int[] line = new Vector3Int[distance + 1];
        line[0] = cubeCoordinates;
        
        for (int i = 1; i <= distance; i++)
        {
            float t = (float)i / distance;
            Vector3 lerped = Vector3.Lerp((Vector3)cubeCoordinates, (Vector3)endPoint, t);
            line[i] = new Vector3Int(Mathf.RoundToInt(lerped.x), Mathf.RoundToInt(lerped.y), Mathf.RoundToInt(lerped.z));
        }
        
        return line;
    }
    
    public Vector3Int[] GetLine(HexTile target, int maxDistance = -1)
    {
        return GetLine(target.cubeCoordinates, maxDistance);
    }
    
    public Vector3Int[] GetRing(int radius)
    {
        if (radius <= 0) return new Vector3Int[] { cubeCoordinates };
        
        List<Vector3Int> ring = new List<Vector3Int>();
        Vector3Int current = cubeCoordinates + new Vector3Int(0, -radius, radius);
        
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                ring.Add(current);
                current = GetRotatedDirection(current - cubeCoordinates, 1) + cubeCoordinates;
            }
        }
        
        return ring.ToArray();
    }
    
    public Vector3Int[] GetSpiral(int radius)
    {
        List<Vector3Int> spiral = new List<Vector3Int>();
        
        for (int r = 0; r <= radius; r++)
        {
            Vector3Int[] ring = GetRing(r);
            spiral.AddRange(ring);
        }
        
        return spiral.ToArray();
    }
    
    public Vector3Int[] GetArea(int radius)
    {
        List<Vector3Int> area = new List<Vector3Int>();
        
        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);
            
            for (int r = r1; r <= r2; r++)
            {
                int s = -q - r;
                Vector3Int coord = new Vector3Int(q, r, s) + cubeCoordinates;
                area.Add(coord);
            }
        }
        
        return area.ToArray();
    }
    
    public Vector3Int[] GetVisibleArea(int radius, Vector3Int[] obstacles)
    {
        Vector3Int[] area = GetArea(radius);
        List<Vector3Int> visible = new List<Vector3Int>();
        
        foreach (Vector3Int coord in area)
        {
            if (IsVisible(coord, obstacles))
            {
                visible.Add(coord);
            }
        }
        
        return visible.ToArray();
    }
    
    private bool IsVisible(Vector3Int target, Vector3Int[] obstacles)
    {
        Vector3Int[] line = GetLine(target);
        
        foreach (Vector3Int coord in line)
        {
            if (System.Array.IndexOf(obstacles, coord) != -1)
            {
                return false;
            }
        }
        
        return true;
    }
    
    public Vector3Int[] GetPathTo(Vector3Int target, Vector3Int[] obstacles = null)
    {
        // Simple line-of-sight pathfinding
        if (obstacles == null || obstacles.Length == 0)
        {
            return GetLine(target);
        }
        
        // For now, return direct line - you can implement A* here if needed
        return GetLine(target);
    }
    
    public Vector3Int[] GetPathTo(HexTile target, Vector3Int[] obstacles = null)
    {
        return GetPathTo(target.cubeCoordinates, obstacles);
    }
    
    public override string ToString()
    {
        return $"Hex({cubeCoordinates.x},{cubeCoordinates.y},{cubeCoordinates.z})";
    }
} 