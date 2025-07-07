using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class HexGridGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int radius = 3;
    [SerializeField] private float hexSize = 1f;
    [SerializeField] private float hexHeight = 0.1f;
    
    [Header("Visual Settings")]
    [SerializeField] private Material hexMaterial;
    [SerializeField] private Color hexColor = Color.white;
    [SerializeField] private Color borderColor = Color.black;
    [SerializeField] private float borderWidth = 0.05f;
    
    [Header("Resource Generation")]
    [SerializeField] private bool generateResources = true;
    [SerializeField] private ResourceGenerator resourceGenerator;
    
    [Header("Debug")]
    [SerializeField] private bool showCoordinates = true;
    [SerializeField] private bool showGridLines = true;
    
    private List<GameObject> hexTiles = new List<GameObject>();
    private Dictionary<Vector3Int, GameObject> hexMap = new Dictionary<Vector3Int, GameObject>();
    
    // Events
    public UnityEvent OnGridGenerated;
    
    // Cube coordinate constants
    private static readonly Vector3Int[] CubeDirections = {
        new Vector3Int(1, -1, 0), new Vector3Int(1, 0, -1), new Vector3Int(0, 1, -1),
        new Vector3Int(-1, 1, 0), new Vector3Int(-1, 0, 1), new Vector3Int(0, -1, 1)
    };
    
    private void Start()
    {
        GenerateHexGrid();
    }
    
    [ContextMenu("Generate Hex Grid")]
    public void GenerateHexGrid()
    {
        ClearGrid();
        CreateHexGrid();
    }
    
    private void ClearGrid()
    {
        foreach (var hex in hexTiles)
        {
            if (hex != null)
            {
                DestroyImmediate(hex);
            }
        }
        hexTiles.Clear();
        hexMap.Clear();
    }
    
    private void CreateHexGrid()
    {
        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);
            
            for (int r = r1; r <= r2; r++)
            {
                int s = -q - r;
                Vector3Int cubeCoord = new Vector3Int(q, r, s);
                CreateHexTile(cubeCoord);
            }
        }
        
        // Generate resources after grid is created
        if (generateResources && resourceGenerator != null)
        {
            resourceGenerator.GenerateResources(this);
        }
        
        // Notify that grid generation is complete
        OnGridGenerated?.Invoke();
    }
    
    private void CreateHexTile(Vector3Int cubeCoord)
    {
        GameObject hexTile = new GameObject($"Hex_{cubeCoord.x}_{cubeCoord.y}_{cubeCoord.z}");
        hexTile.transform.SetParent(transform);
        
        // Add mesh components
        MeshFilter meshFilter = hexTile.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = hexTile.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = hexTile.AddComponent<MeshCollider>();
        
        // Create hex mesh
        Mesh hexMesh = CreateHexMesh();
        meshFilter.mesh = hexMesh;
        meshCollider.sharedMesh = hexMesh;
        
        // Set material
        if (hexMaterial != null)
        {
            meshRenderer.material = hexMaterial;
        }
        else
        {
            meshRenderer.material = new Material(Shader.Find("Standard"));
            meshRenderer.material.color = hexColor;
        }
        
        // Configure material for proper lighting
        meshRenderer.material.SetFloat("_Glossiness", 0.1f); // Low glossiness for flat surface
        meshRenderer.material.SetFloat("_Metallic", 0.0f); // Non-metallic
        
        // Position the hex
        Vector3 worldPos = CubeToWorldPosition(cubeCoord);
        hexTile.transform.position = worldPos;
        
        // Add hex data component
        HexTile hexData = hexTile.AddComponent<HexTile>();
        hexData.Initialize(cubeCoord, worldPos);
        
        hexTiles.Add(hexTile);
        hexMap[cubeCoord] = hexTile;
        
        // Notify UI that new tiles are available
        NotifyTilesCreated();
    }
    
    private Mesh CreateHexMesh()
    {
        Mesh mesh = new Mesh();
        
        Vector3[] vertices = new Vector3[7]; // 6 corners + center
        int[] triangles = new int[18]; // 6 triangles * 3 vertices
        Vector3[] normals = new Vector3[7];
        
        float angleStep = 60f * Mathf.Deg2Rad;
        float startAngle = 30f * Mathf.Deg2Rad; // Start from top
        
        // Center vertex
        vertices[0] = Vector3.zero;
        normals[0] = Vector3.up; // Normal pointing up
        
        // Corner vertices
        for (int i = 0; i < 6; i++)
        {
            float angle = startAngle + i * angleStep;
            float x = Mathf.Cos(angle) * hexSize;
            float z = Mathf.Sin(angle) * hexSize;
            vertices[i + 1] = new Vector3(x, 0, z);
            normals[i + 1] = Vector3.up; // All normals pointing up
        }
        
        // Create triangles (clockwise winding for proper front face)
        for (int i = 0; i < 6; i++)
        {
            int baseIndex = i * 3;
            triangles[baseIndex] = 0; // Center
            triangles[baseIndex + 1] = (i + 1) % 6 + 1;
            triangles[baseIndex + 2] = i + 1;
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.RecalculateBounds();
        
        return mesh;
    }
    
    public Vector3 CubeToWorldPosition(Vector3Int cubeCoord)
    {
        float x = hexSize * (Mathf.Sqrt(3) * cubeCoord.x + Mathf.Sqrt(3) / 2 * cubeCoord.y);
        float z = hexSize * (3f / 2 * cubeCoord.y);
        return new Vector3(x, 0, z);
    }
    
    public Vector3Int WorldToCubePosition(Vector3 worldPos)
    {
        float q = (Mathf.Sqrt(3) / 3 * worldPos.x - 1f / 3 * worldPos.z) / hexSize;
        float r = (2f / 3 * worldPos.z) / hexSize;
        float s = -q - r;
        
        return RoundCubeCoordinates(new Vector3(q, r, s));
    }
    
    private Vector3Int RoundCubeCoordinates(Vector3 cube)
    {
        float rx = Mathf.Round(cube.x);
        float ry = Mathf.Round(cube.y);
        float rz = Mathf.Round(cube.z);
        
        float xDiff = Mathf.Abs(rx - cube.x);
        float yDiff = Mathf.Abs(ry - cube.y);
        float zDiff = Mathf.Abs(rz - cube.z);
        
        if (xDiff > yDiff && xDiff > zDiff)
        {
            rx = -ry - rz;
        }
        else if (yDiff > zDiff)
        {
            ry = -rx - rz;
        }
        else
        {
            rz = -rx - ry;
        }
        
        return new Vector3Int((int)rx, (int)ry, (int)rz);
    }
    
    public List<Vector3Int> GetNeighbors(Vector3Int cubeCoord)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        
        foreach (Vector3Int direction in CubeDirections)
        {
            Vector3Int neighbor = cubeCoord + direction;
            if (hexMap.ContainsKey(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }
        
        return neighbors;
    }
    
    public float GetDistance(Vector3Int a, Vector3Int b)
    {
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2f;
    }
    
    public GameObject GetHexAt(Vector3Int cubeCoord)
    {
        hexMap.TryGetValue(cubeCoord, out GameObject hex);
        return hex;
    }
    
    public List<GameObject> GetAllHexTiles()
    {
        return new List<GameObject>(hexTiles);
    }
    
    public void SetRadius(int newRadius)
    {
        radius = newRadius;
        GenerateHexGrid();
    }
    
    private void NotifyTilesCreated()
    {
        // This method is called when new tiles are created
        // The UI will handle the event subscription
    }
    
    private void OnDrawGizmos()
    {
        if (!showGridLines) return;
        
        Gizmos.color = Color.yellow;
        foreach (var kvp in hexMap)
        {
            Vector3Int cubeCoord = kvp.Key;
            Vector3 worldPos = kvp.Value.transform.position;
            
            // Draw coordinate text
            if (showCoordinates)
            {
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(worldPos + Vector3.up * 0.5f, 
                    $"({cubeCoord.x},{cubeCoord.y},{cubeCoord.z})");
                #endif
            }
            
            // Draw neighbor connections
            foreach (Vector3Int neighbor in GetNeighbors(cubeCoord))
            {
                if (hexMap.TryGetValue(neighbor, out GameObject neighborHex))
                {
                    Gizmos.DrawLine(worldPos, neighborHex.transform.position);
                }
            }
        }
    }
} 