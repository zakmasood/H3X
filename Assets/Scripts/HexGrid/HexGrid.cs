using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour component that generates and manages a hex grid.
/// Attach this to a GameObject in your scene to create a hex grid.
/// </summary>
[RequireComponent(typeof(Transform))]
public class HexGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("Width of the grid in hex cells")]
    public int gridWidth = 10;

    [Tooltip("Height of the grid in hex cells")]
    public int gridHeight = 10;

    [Tooltip("Size of each hex (distance from center to corner)")]
    public float hexSize = 1f;

    [Header("Visual Settings")]
    [Tooltip("Prefab to instantiate for each hex cell. Should have a HexCell component or be a simple GameObject.")]
    public GameObject hexPrefab;

    [Tooltip("Parent transform to organize hex cells under (optional)")]
    public Transform hexParent;

    // Dictionary to store all hex cells by their cube coordinates
    // Key format: "x,y,z" as string for easy lookup
    private Dictionary<string, HexCell> hexCells;

    // List of all hex cells for easy iteration
    private List<HexCell> allCells;

    /// <summary>
    /// Gets all hex cells in the grid
    /// </summary>
    public List<HexCell> AllCells => new List<HexCell>(allCells);

    /// <summary>
    /// Gets a hex cell by its cube coordinates
    /// </summary>
    public HexCell GetCell(int x, int y, int z)
    {
        string key = $"{x},{y},{z}";
        hexCells.TryGetValue(key, out HexCell cell);
        return cell;
    }

    /// <summary>
    /// Generates the hex grid
    /// </summary>
    public void GenerateGrid()
    {
        // Initialize collections
        hexCells = new Dictionary<string, HexCell>();
        allCells = new List<HexCell>();

        // Clear existing grid if regenerating
        ClearGrid();

        // Create parent if not assigned
        if (hexParent == null)
        {
            GameObject parentObj = new GameObject("HexCells");
            parentObj.transform.SetParent(transform);
            hexParent = parentObj.transform;
        }

        // TODO: Implement the grid generation loop
        // Hint: You'll need nested loops to iterate through the grid
        // For a rectangular grid, you can use:
        //   - Outer loop: iterate through rows (z coordinate)
        //   - Inner loop: iterate through columns (x coordinate)
        //   - Calculate y coordinate using the constraint: y = -x - z (so x + y + z = 0)
        //   - For each (x, y, z), create a new HexCell
        //   - Convert cube coordinates to world position using HexGridUtility.CubeToWorldPosition()
        //   - Set the cell's WorldPosition property
        //   - Add the cell to both hexCells dictionary (key: "x,y,z") and allCells list
        //   - Instantiate the hexPrefab at the world position and set it as the cell's VisualObject
        // Learning: Nested loops are essential for grid generation. Think about how you'd fill a 2D array.
        // The cube coordinate constraint (x+y+z=0) means you only need to loop through two dimensions.

        // Your code here - replace this comment with your nested loop implementation
        // Example structure:
        // for (int z = 0; z < gridHeight; z++)
        // {
        //     for (int x = 0; x < gridWidth; x++)
        //     {
        //         // Calculate y, create cell, add to collections, instantiate visual
        //     }
        // }

        // After generating all cells, connect neighbors
        ConnectNeighbors();
    }

    /// <summary>
    /// Connects all hex cells to their neighbors
    /// </summary>
    private void ConnectNeighbors()
    {
        // TODO: Implement neighbor connection logic
        // Hint: Iterate through all cells in allCells
        // For each cell, check all 6 possible neighbor directions (0-5)
        // Use HexGridUtility.GetNeighborCoordinates() to get the neighbor's coordinates
        // Look up the neighbor cell using GetCell() method
        // If the neighbor exists, add it to the current cell's Neighbors list using AddNeighbor()
        // Learning: This demonstrates how to iterate through collections and look up related data.
        // Understanding neighbor relationships is crucial for pathfinding and grid-based algorithms.

        // Your code here - replace this comment with your neighbor connection implementation
        // Example structure:
        // foreach (HexCell cell in allCells)
        // {
        //     for (int direction = 0; direction < 6; direction++)
        //     {
        //         // Get neighbor coordinates, look up neighbor cell, add if exists
        //     }
        // }
    }

    /// <summary>
    /// Clears the existing grid and destroys all visual objects
    /// </summary>
    public void ClearGrid()
    {
        if (allCells != null)
        {
            foreach (HexCell cell in allCells)
            {
                if (cell.VisualObject != null)
                {
                    DestroyImmediate(cell.VisualObject);
                }
            }
        }

        hexCells?.Clear();
        allCells?.Clear();

        // Destroy all children of hexParent
        if (hexParent != null)
        {
            for (int i = hexParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(hexParent.GetChild(i).gameObject);
            }
        }
    }

    /// <summary>
    /// Unity callback - called when component is enabled
    /// </summary>
    private void OnEnable()
    {
        // Auto-generate grid when component is enabled (useful for testing)
        // Comment this out if you want manual control
        // GenerateGrid();
    }

    /// <summary>
    /// Unity callback - called in editor when values change
    /// </summary>
    private void OnValidate()
    {
        // Clamp values to prevent invalid inputs
        gridWidth = Mathf.Max(1, gridWidth);
        gridHeight = Mathf.Max(1, gridHeight);
        hexSize = Mathf.Max(0.1f, hexSize);
    }

    /// <summary>
    /// Unity callback - draws gizmos in the scene view for debugging
    /// </summary>
    private void OnDrawGizmos()
    {
        if (allCells == null || allCells.Count == 0) return;

        Gizmos.color = Color.yellow;
        foreach (HexCell cell in allCells)
        {
            if (cell != null)
            {
                Gizmos.DrawWireSphere(cell.WorldPosition, hexSize * 0.3f);
            }
        }
    }
}

