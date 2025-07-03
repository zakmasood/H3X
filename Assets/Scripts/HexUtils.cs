using UnityEngine;
using System.Collections.Generic;

public static class HexUtils
{
    // Cube coordinate directions (flat-topped hexagons)
    public static readonly Vector3Int[] CubeDirections = {
        new Vector3Int(1, -1, 0),   // Right
        new Vector3Int(1, 0, -1),   // Top-Right
        new Vector3Int(0, 1, -1),   // Top-Left
        new Vector3Int(-1, 1, 0),   // Left
        new Vector3Int(-1, 0, 1),   // Bottom-Left
        new Vector3Int(0, -1, 1)    // Bottom-Right
    };
    
    // Cube coordinate directions (pointy-topped hexagons)
    public static readonly Vector3Int[] CubeDirectionsPointy = {
        new Vector3Int(1, 0, -1),   // Right
        new Vector3Int(0, 1, -1),   // Top-Right
        new Vector3Int(-1, 1, 0),   // Top-Left
        new Vector3Int(-1, 0, 1),   // Left
        new Vector3Int(0, -1, 1),   // Bottom-Left
        new Vector3Int(1, -1, 0)    // Bottom-Right
    };
    
    /// <summary>
    /// Converts cube coordinates to world position (flat-topped hexagons)
    /// </summary>
    public static Vector3 CubeToWorldPosition(Vector3Int cubeCoord, float hexSize)
    {
        float x = hexSize * (Mathf.Sqrt(3) * cubeCoord.x + Mathf.Sqrt(3) / 2 * cubeCoord.y);
        float z = hexSize * (3f / 2 * cubeCoord.y);
        return new Vector3(x, 0, z);
    }
    
    /// <summary>
    /// Converts cube coordinates to world position (pointy-topped hexagons)
    /// </summary>
    public static Vector3 CubeToWorldPositionPointy(Vector3Int cubeCoord, float hexSize)
    {
        float x = hexSize * (3f / 2 * cubeCoord.x);
        float z = hexSize * (Mathf.Sqrt(3) / 2 * cubeCoord.x + Mathf.Sqrt(3) * cubeCoord.y);
        return new Vector3(x, 0, z);
    }
    
    /// <summary>
    /// Converts world position to cube coordinates (flat-topped hexagons)
    /// </summary>
    public static Vector3Int WorldToCubePosition(Vector3 worldPos, float hexSize)
    {
        float q = (Mathf.Sqrt(3) / 3 * worldPos.x - 1f / 3 * worldPos.z) / hexSize;
        float r = (2f / 3 * worldPos.z) / hexSize;
        float s = -q - r;
        
        return RoundCubeCoordinates(new Vector3(q, r, s));
    }
    
    /// <summary>
    /// Converts world position to cube coordinates (pointy-topped hexagons)
    /// </summary>
    public static Vector3Int WorldToCubePositionPointy(Vector3 worldPos, float hexSize)
    {
        float q = (2f / 3 * worldPos.x) / hexSize;
        float r = (-1f / 3 * worldPos.x + Mathf.Sqrt(3) / 3 * worldPos.z) / hexSize;
        float s = -q - r;
        
        return RoundCubeCoordinates(new Vector3(q, r, s));
    }
    
    /// <summary>
    /// Rounds cube coordinates to the nearest valid cube coordinate
    /// </summary>
    public static Vector3Int RoundCubeCoordinates(Vector3 cube)
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
    
    /// <summary>
    /// Calculates the distance between two cube coordinates
    /// </summary>
    public static float GetDistance(Vector3Int a, Vector3Int b)
    {
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2f;
    }
    
    /// <summary>
    /// Gets all neighbors of a cube coordinate
    /// </summary>
    public static Vector3Int[] GetNeighbors(Vector3Int cubeCoord, bool pointyTopped = false)
    {
        Vector3Int[] directions = pointyTopped ? CubeDirectionsPointy : CubeDirections;
        Vector3Int[] neighbors = new Vector3Int[6];
        
        for (int i = 0; i < 6; i++)
        {
            neighbors[i] = cubeCoord + directions[i];
        }
        
        return neighbors;
    }
    
    /// <summary>
    /// Gets all cube coordinates within a certain radius
    /// </summary>
    public static List<Vector3Int> GetArea(Vector3Int center, int radius)
    {
        List<Vector3Int> area = new List<Vector3Int>();
        
        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);
            
            for (int r = r1; r <= r2; r++)
            {
                int s = -q - r;
                Vector3Int coord = center + new Vector3Int(q, r, s);
                area.Add(coord);
            }
        }
        
        return area;
    }
    
    /// <summary>
    /// Gets all cube coordinates in a ring around a center point
    /// </summary>
    public static List<Vector3Int> GetRing(Vector3Int center, int radius, bool pointyTopped = false)
    {
        if (radius <= 0) return new List<Vector3Int> { center };
        
        List<Vector3Int> ring = new List<Vector3Int>();
        Vector3Int[] directions = pointyTopped ? CubeDirectionsPointy : CubeDirections;
        
        Vector3Int current = center + directions[4] * radius; // Start from bottom-left direction
        
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                ring.Add(current);
                current = current + directions[i];
            }
        }
        
        return ring;
    }
    
    /// <summary>
    /// Gets all cube coordinates in a spiral pattern around a center point
    /// </summary>
    public static List<Vector3Int> GetSpiral(Vector3Int center, int radius)
    {
        List<Vector3Int> spiral = new List<Vector3Int>();
        
        for (int r = 0; r <= radius; r++)
        {
            spiral.AddRange(GetRing(center, r));
        }
        
        return spiral;
    }
    
    /// <summary>
    /// Gets a line of cube coordinates from start to end
    /// </summary>
    public static List<Vector3Int> GetLine(Vector3Int start, Vector3Int end)
    {
        int distance = Mathf.RoundToInt(GetDistance(start, end));
        List<Vector3Int> line = new List<Vector3Int>();
        
        for (int i = 0; i <= distance; i++)
        {
            float t = distance == 0 ? 0 : (float)i / distance;
            Vector3 lerped = Vector3.Lerp((Vector3)start, (Vector3)end, t);
            line.Add(RoundCubeCoordinates(lerped));
        }
        
        return line;
    }
    
    /// <summary>
    /// Gets a line of cube coordinates from start in a direction for a certain distance
    /// </summary>
    public static List<Vector3Int> GetLine(Vector3Int start, Vector3Int direction, int distance)
    {
        Vector3Int end = start + direction * distance;
        return GetLine(start, end);
    }
    
    /// <summary>
    /// Rotates a direction vector by a certain number of steps (60 degrees each)
    /// </summary>
    public static Vector3Int RotateDirection(Vector3Int direction, int steps, bool pointyTopped = false)
    {
        Vector3Int[] directions = pointyTopped ? CubeDirectionsPointy : CubeDirections;
        int currentIndex = System.Array.IndexOf(directions, direction);
        
        if (currentIndex == -1) return direction;
        
        int newIndex = (currentIndex + steps + 6) % 6;
        return directions[newIndex];
    }
    
    /// <summary>
    /// Rotates a direction vector by a certain angle
    /// </summary>
    public static Vector3Int RotateDirection(Vector3Int direction, float angleDegrees, bool pointyTopped = false)
    {
        int steps = Mathf.RoundToInt(angleDegrees / 60f);
        return RotateDirection(direction, steps, pointyTopped);
    }
    
    /// <summary>
    /// Gets the opposite direction
    /// </summary>
    public static Vector3Int GetOppositeDirection(Vector3Int direction)
    {
        return -direction;
    }
    
    /// <summary>
    /// Checks if two cube coordinates are neighbors
    /// </summary>
    public static bool AreNeighbors(Vector3Int a, Vector3Int b)
    {
        return GetDistance(a, b) == 1f;
    }
    
    /// <summary>
    /// Gets the direction from one cube coordinate to another
    /// </summary>
    public static Vector3Int GetDirection(Vector3Int from, Vector3Int to)
    {
        return to - from;
    }
    
    /// <summary>
    /// Gets the angle between two directions
    /// </summary>
    public static float GetAngle(Vector3Int direction1, Vector3Int direction2, bool pointyTopped = false)
    {
        Vector3Int[] directions = pointyTopped ? CubeDirectionsPointy : CubeDirections;
        int index1 = System.Array.IndexOf(directions, direction1);
        int index2 = System.Array.IndexOf(directions, direction2);
        
        if (index1 == -1 || index2 == -1) return 0f;
        
        int diff = (index2 - index1 + 6) % 6;
        return diff * 60f;
    }
    
    /// <summary>
    /// Gets all cube coordinates in a wedge shape
    /// </summary>
    public static List<Vector3Int> GetWedge(Vector3Int center, Vector3Int direction, int radius, float angleDegrees, bool pointyTopped = false)
    {
        List<Vector3Int> wedge = new List<Vector3Int>();
        Vector3Int[] directions = pointyTopped ? CubeDirectionsPointy : CubeDirections;
        
        int startIndex = System.Array.IndexOf(directions, direction);
        if (startIndex == -1) return wedge;
        
        int steps = Mathf.RoundToInt(angleDegrees / 60f);
        
        for (int r = 1; r <= radius; r++)
        {
            for (int i = 0; i < steps; i++)
            {
                int currentIndex = (startIndex + i) % 6;
                Vector3Int currentDirection = directions[currentIndex];
                Vector3Int coord = center + currentDirection * r;
                wedge.Add(coord);
            }
        }
        
        return wedge;
    }
    
    /// <summary>
    /// Gets all cube coordinates in a cone shape
    /// </summary>
    public static List<Vector3Int> GetCone(Vector3Int center, Vector3Int direction, int radius, float angleDegrees, bool pointyTopped = false)
    {
        List<Vector3Int> cone = new List<Vector3Int> { center };
        cone.AddRange(GetWedge(center, direction, radius, angleDegrees, pointyTopped));
        return cone;
    }
    
    /// <summary>
    /// Checks if a cube coordinate is within a certain area
    /// </summary>
    public static bool IsInArea(Vector3Int coord, Vector3Int center, int radius)
    {
        return GetDistance(coord, center) <= radius;
    }
    
    /// <summary>
    /// Gets the closest cube coordinate to a world position
    /// </summary>
    public static Vector3Int GetClosestCubeCoordinate(Vector3 worldPos, float hexSize, bool pointyTopped = false)
    {
        return pointyTopped ? 
            WorldToCubePositionPointy(worldPos, hexSize) : 
            WorldToCubePosition(worldPos, hexSize);
    }
    
    /// <summary>
    /// Gets the center point of a hexagon in world space
    /// </summary>
    public static Vector3 GetHexCenter(Vector3Int cubeCoord, float hexSize, bool pointyTopped = false)
    {
        return pointyTopped ? 
            CubeToWorldPositionPointy(cubeCoord, hexSize) : 
            CubeToWorldPosition(cubeCoord, hexSize);
    }
    
    /// <summary>
    /// Gets the corners of a hexagon in world space
    /// </summary>
    public static Vector3[] GetHexCorners(Vector3Int cubeCoord, float hexSize, bool pointyTopped = false)
    {
        Vector3 center = GetHexCenter(cubeCoord, hexSize, pointyTopped);
        Vector3[] corners = new Vector3[6];
        
        float angleStep = 60f * Mathf.Deg2Rad;
        float startAngle = pointyTopped ? 0f : 30f * Mathf.Deg2Rad;
        
        for (int i = 0; i < 6; i++)
        {
            float angle = startAngle + i * angleStep;
            float x = Mathf.Cos(angle) * hexSize;
            float z = Mathf.Sin(angle) * hexSize;
            corners[i] = center + new Vector3(x, 0, z);
        }
        
        return corners;
    }
    
    /// <summary>
    /// Gets the area of a hexagon
    /// </summary>
    public static float GetHexArea(float hexSize)
    {
        return (3f * Mathf.Sqrt(3f) * hexSize * hexSize) / 2f;
    }
    
    /// <summary>
    /// Gets the perimeter of a hexagon
    /// </summary>
    public static float GetHexPerimeter(float hexSize)
    {
        return 6f * hexSize;
    }
    
    /// <summary>
    /// Gets the width of a hexagon (flat-topped)
    /// </summary>
    public static float GetHexWidth(float hexSize)
    {
        return 2f * hexSize;
    }
    
    /// <summary>
    /// Gets the height of a hexagon (flat-topped)
    /// </summary>
    public static float GetHexHeight(float hexSize)
    {
        return Mathf.Sqrt(3f) * hexSize;
    }
    
    /// <summary>
    /// Gets the width of a hexagon (pointy-topped)
    /// </summary>
    public static float GetHexWidthPointy(float hexSize)
    {
        return Mathf.Sqrt(3f) * hexSize;
    }
    
    /// <summary>
    /// Gets the height of a hexagon (pointy-topped)
    /// </summary>
    public static float GetHexHeightPointy(float hexSize)
    {
        return 2f * hexSize;
    }
} 