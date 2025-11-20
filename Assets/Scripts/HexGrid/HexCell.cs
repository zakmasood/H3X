using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single hex cell in the grid.
/// Uses cube coordinates (x, y, z) where x + y + z = 0
/// </summary>
public class HexCell
{
    // Cube coordinates - these three values must always sum to zero
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Z { get; private set; }

    // World position in Unity's 3D space
    public Vector3 WorldPosition { get; set; }

    // Reference to the GameObject representing this cell visually
    public GameObject VisualObject { get; set; }

    // List of neighboring hex cells
    // TODO: Initialize this list in the constructor
    // Hint: Use 'new List<HexCell>()' to create an empty list
    // Learning: Collections like List<T> allow you to store multiple objects dynamically
    // The angle brackets <HexCell> specify what type of objects this list will hold
    public List<HexCell> Neighbors { get; private set; }

    /// <summary>
    /// Creates a new hex cell with cube coordinates
    /// </summary>
    /// <param name="x">X coordinate in cube space</param>
    /// <param name="y">Y coordinate in cube space</param>
    /// <param name="z">Z coordinate in cube space</param>
    public HexCell(int x, int y, int z)
    {
        // Validate that cube coordinates sum to zero
        if (x + y + z != 0)
        {
            Debug.LogWarning($"Invalid cube coordinates: ({x}, {y}, {z}). Sum must equal 0. Adjusting z to satisfy constraint.");
            z = -x - y; // Calculate z to satisfy x + y + z = 0
        }

        X = x;
        Y = y;
        Z = z;

        // TODO: Initialize the Neighbors list here
        // Your code: Neighbors = new List<HexCell>();
    }

    /// <summary>
    /// Adds a neighbor to this cell's neighbor list
    /// </summary>
    public void AddNeighbor(HexCell neighbor)
    {
        if (neighbor != null && !Neighbors.Contains(neighbor))
        {
            Neighbors.Add(neighbor);
        }
    }

    /// <summary>
    /// Gets the cube coordinates as a Vector3Int for convenience
    /// </summary>
    public Vector3Int GetCubeCoordinates()
    {
        return new Vector3Int(X, Y, Z);
    }

    /// <summary>
    /// Checks if this cell is equal to another based on coordinates
    /// </summary>
    public bool Equals(HexCell other)
    {
        if (other == null) return false;
        return X == other.X && Y == other.Y && Z == other.Z;
    }
}

