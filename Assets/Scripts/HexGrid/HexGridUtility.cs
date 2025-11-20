using UnityEngine;

/// <summary>
/// Static utility class for hex grid calculations using cube coordinates.
/// All methods are static, meaning you can call them without creating an instance.
/// </summary>
public static class HexGridUtility
{
    // Hex size constants for flat-top orientation
    private const float SQRT_3 = 1.7320508075688772f; // Square root of 3

    /// <summary>
    /// Converts cube coordinates to world position for flat-top hex orientation
    /// </summary>
    /// <param name="x">X coordinate in cube space</param>
    /// <param name="y">Y coordinate in cube space</param>
    /// <param name="z">Z coordinate in cube space</param>
    /// <param name="hexSize">Size of a hex (distance from center to corner)</param>
    /// <returns>World position as Vector3</returns>
    public static Vector3 CubeToWorldPosition(int x, int y, int z, float hexSize)
    {
        // For flat-top hexes, width is sqrt(3) * size, height is 1.5 * size
        float worldX = hexSize * SQRT_3 * (x + z * 0.5f);
        float worldZ = hexSize * 1.5f * z;
        return new Vector3(worldX, 0f, worldZ);
    }

    /// <summary>
    /// Converts world position to cube coordinates (approximate)
    /// </summary>
    /// <param name="worldPos">World position</param>
    /// <param name="hexSize">Size of a hex</param>
    /// <returns>Cube coordinates as Vector3Int</returns>
    public static Vector3Int WorldToCubeCoordinates(Vector3 worldPos, float hexSize)
    {
        float q = (SQRT_3 / 3f * worldPos.x - 1f / 3f * worldPos.z) / hexSize;
        float r = (2f / 3f * worldPos.z) / hexSize;
        
        // Convert to cube coordinates
        int x = Mathf.RoundToInt(q);
        int z = Mathf.RoundToInt(r);
        int y = -x - z; // Enforce x + y + z = 0 constraint
        
        return new Vector3Int(x, y, z);
    }

    /// <summary>
    /// Gets the cube coordinate offset for a given direction (flat-top orientation)
    /// </summary>
    /// <param name="direction">Direction index (0-5) representing the 6 hex directions</param>
    /// <returns>Cube coordinate offset as Vector3Int</returns>
    public static Vector3Int GetDirectionOffset(int direction)
    {
        // Direction vectors for flat-top hex orientation
        // These represent the 6 neighbors: East, Northeast, Northwest, West, Southwest, Southeast
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(1, -1, 0),   // East
            new Vector3Int(1, 0, -1),   // Northeast
            new Vector3Int(0, 1, -1),   // Northwest
            new Vector3Int(-1, 1, 0),   // West
            new Vector3Int(-1, 0, 1),   // Southwest
            new Vector3Int(0, -1, 1)    // Southeast
        };

        direction = direction % 6; // Ensure direction is in valid range (0-5)
        if (direction < 0) direction += 6; // Handle negative directions
        
        return directions[direction];
    }

    /// <summary>
    /// Gets the cube coordinates of a neighbor in a specific direction
    /// </summary>
    /// <param name="x">Current X coordinate</param>
    /// <param name="y">Current Y coordinate</param>
    /// <param name="z">Current Z coordinate</param>
    /// <param name="direction">Direction index (0-5)</param>
    /// <returns>Neighbor's cube coordinates as Vector3Int</returns>
    public static Vector3Int GetNeighborCoordinates(int x, int y, int z, int direction)
    {
        Vector3Int offset = GetDirectionOffset(direction);
        return new Vector3Int(x + offset.x, y + offset.y, z + offset.z);
    }

    /// <summary>
    /// Calculates the distance between two hex cells using cube coordinates
    /// </summary>
    /// <param name="x1">First hex X coordinate</param>
    /// <param name="y1">First hex Y coordinate</param>
    /// <param name="z1">First hex Z coordinate</param>
    /// <param name="x2">Second hex X coordinate</param>
    /// <param name="y2">Second hex Y coordinate</param>
    /// <param name="z2">Second hex Z coordinate</param>
    /// <returns>Distance in hex cells</returns>

    // TODO: Implement the distance calculation
    // Hint: Calculate the differences (dx, dy, dz) between the two hexes
    // Then return the maximum of the absolute values: max(|dx|, |dy|, |dz|)
    // Learning: In cube coordinates, distance is elegantly simple - just the max of the coordinate differences
    // This is much simpler than Euclidean distance! Think about why this works geometrically.
    public static int GetDistance(int x1, int y1, int z1, int x2, int y2, int z2)
    {
        // Your code here - calculate dx, dy, dz, then return max(|dx|, |dy|, |dz|)
        return 0; // Placeholder - replace this with your implementation
    }

    /// <summary>
    /// Calculates the distance between two HexCell objects
    /// </summary>
    public static int GetDistance(HexCell cell1, HexCell cell2)
    {
        if (cell1 == null || cell2 == null) return -1;
        return GetDistance(cell1.X, cell1.Y, cell1.Z, cell2.X, cell2.Y, cell2.Z);
    }

    /// <summary>
    /// Gets all 6 neighbor coordinates for a given hex
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="z">Z coordinate</param>
    /// <returns>Array of 6 neighbor coordinates</returns>
    public static Vector3Int[] GetAllNeighborCoordinates(int x, int y, int z)
    {
        Vector3Int[] neighbors = new Vector3Int[6];
        for (int i = 0; i < 6; i++)
        {
            neighbors[i] = GetNeighborCoordinates(x, y, z, i);
        }
        return neighbors;
    }
}

