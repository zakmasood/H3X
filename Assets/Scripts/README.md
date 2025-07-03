# Unity Hexagonal Grid System

A comprehensive hexagonal grid system for Unity with cube coordinates, radius selection, and advanced grid operations.

## Features

- **Cube Coordinate System**: Uses the standard cube coordinate system (q, r, s) for precise hex positioning
- **Radius Selection**: Configurable grid radius with real-time updates
- **Visual Controls**: Color customization, highlighting, and selection
- **Grid Operations**: Neighbor detection, pathfinding, area selection, and more
- **UI Integration**: Built-in UI controls for grid management
- **Debug Visualization**: Coordinate display and grid line rendering
- **Event System**: Comprehensive event handling for hex interactions

## Files Overview

### Core Scripts

1. **`HexGridGenerator.cs`** - Main grid generation and management
2. **`HexTile.cs`** - Individual hexagon behavior and data
3. **`HexUtils.cs`** - Static utility methods for cube coordinate operations
4. **`HexGridUI.cs`** - UI controller for grid settings
5. **`HexGridDemo.cs`** - Demo script showing system capabilities

## Setup Instructions

### 1. Basic Setup

1. Create a new Unity project or open an existing one
2. Copy all the `.cs` files to your `Assets/Scripts` folder
3. Create an empty GameObject in your scene
4. Add the `HexGridGenerator` component to the GameObject
5. Configure the grid settings in the inspector

### 2. Grid Configuration

In the `HexGridGenerator` component inspector:

- **Grid Settings**:

  - `Radius`: Number of hex rings around the center (default: 3)
  - `Hex Size`: Size of each hexagon (default: 1.0)
  - `Hex Height`: Height of hexagons for 3D rendering (default: 0.1)
- **Visual Settings**:

  - `Hex Material`: Material to apply to hexagons
  - `Hex Color`: Default color for hexagons
  - `Border Color`: Color for hex borders
  - `Border Width`: Width of hex borders
- **Debug Settings**:

  - `Show Coordinates`: Display cube coordinates above hexagons
  - `Show Grid Lines`: Display connections between neighbors

### 3. UI Setup (Optional)

1. Create a Canvas in your scene
2. Add UI elements (sliders, buttons, toggles) for grid control
3. Add the `HexGridUI` component to a GameObject
4. Assign the UI elements to the corresponding fields in the inspector
5. Link the `HexGridGenerator` reference

### 4. Demo Setup (Optional)

1. Add the `HexGridDemo` component to a GameObject
2. Assign the `HexGridGenerator` and `Camera` references
3. Configure the demo settings in the inspector

## Usage Examples

### Basic Grid Generation

```csharp
// Get the hex grid generator
HexGridGenerator grid = FindObjectOfType<HexGridGenerator>();

// Generate a grid with radius 5
grid.SetRadius(5);
grid.GenerateHexGrid();
```

### Working with Individual Hexagons

```csharp
// Get a hex at specific cube coordinates
Vector3Int coords = new Vector3Int(1, -1, 0);
GameObject hexObj = grid.GetHexAt(coords);
HexTile hex = hexObj.GetComponent<HexTile>();

// Get hex information
Debug.Log($"Hex coordinates: {hex.CubeCoordinates}");
Debug.Log($"Hex world position: {hex.WorldPosition}");

// Visual manipulation
hex.SetSelected(true);
hex.SetHighlighted(true);
hex.SetColor(Color.red);
```

### Cube Coordinate Operations

```csharp
// Convert between world and cube coordinates
Vector3 worldPos = new Vector3(2.5f, 0, 1.5f);
Vector3Int cubeCoord = HexUtils.WorldToCubePosition(worldPos, 1.0f);
Vector3 backToWorld = HexUtils.CubeToWorldPosition(cubeCoord, 1.0f);

// Get neighbors
Vector3Int[] neighbors = HexUtils.GetNeighbors(cubeCoord);

// Calculate distance
float distance = HexUtils.GetDistance(new Vector3Int(0, 0, 0), new Vector3Int(2, -1, -1));

// Get area within radius
List<Vector3Int> area = HexUtils.GetArea(new Vector3Int(0, 0, 0), 3);

// Get line between two points
List<Vector3Int> line = HexUtils.GetLine(new Vector3Int(0, 0, 0), new Vector3Int(3, -2, -1));
```

### Advanced Grid Operations

```csharp
// Get all hexagons in a ring
Vector3Int[] ring = hex.GetRing(2);

// Get all hexagons in a spiral
Vector3Int[] spiral = hex.GetSpiral(3);

// Get area around a hex
Vector3Int[] area = hex.GetArea(2);

// Get line to another hex
Vector3Int[] line = hex.GetLine(targetHex);

// Get path to another hex (with obstacles)
Vector3Int[] obstacles = { new Vector3Int(1, 0, -1), new Vector3Int(0, 1, -1) };
Vector3Int[] path = hex.GetPathTo(targetHex, obstacles);
```

### Event Handling

```csharp
// Subscribe to hex events
hex.OnHexClicked.AddListener((HexTile clickedHex) => {
    Debug.Log($"Clicked hex at {clickedHex.CubeCoordinates}");
});

hex.OnHexHovered.AddListener((HexTile hoveredHex) => {
    hoveredHex.SetHighlighted(true);
});

hex.OnHexUnhovered.AddListener((HexTile unhoveredHex) => {
    unhoveredHex.SetHighlighted(false);
});
```

## Demo Controls

When using the `HexGridDemo` component:

- **Mouse**:

  - Left Click: Select a hexagon
  - Hover: Highlight hexagons
- **Keyboard**:

  - `P`: Show path from selected hex to mouse position
  - `A`: Show area around selected hex
  - `N`: Show neighbors of selected hex
  - `L`: Show line from selected hex to mouse position
  - `C`: Show cone from selected hex
  - `R`: Reset all visual effects

## Cube Coordinate System

The system uses the standard cube coordinate system where:

- `q` = x-axis (right)
- `r` = y-axis (up-right)
- `s` = z-axis (up-left)

The constraint `q + r + s = 0` is always maintained.

### Coordinate Conversion

- **Flat-topped hexagons**: Use `CubeToWorldPosition()` and `WorldToCubePosition()`
- **Pointy-topped hexagons**: Use `CubeToWorldPositionPointy()` and `WorldToCubePositionPointy()`

## Performance Considerations

- Grid generation is done at startup and when radius changes
- Individual hex operations are O(1) for most operations
- Area calculations scale with radius²
- Consider object pooling for large grids with frequent updates

## Customization

### Adding Custom Hex Types

```csharp
public class CustomHexTile : HexTile
{
    [SerializeField] private int terrainType;
    [SerializeField] private float elevation;
  
    public void SetTerrainType(int type)
    {
        terrainType = type;
        UpdateVisual();
    }
  
    private void UpdateVisual()
    {
        // Custom visual logic based on terrain type
        switch (terrainType)
        {
            case 0: // Grass
                SetColor(Color.green);
                break;
            case 1: // Water
                SetColor(Color.blue);
                break;
            case 2: // Mountain
                SetColor(Color.gray);
                break;
        }
    }
}
```

### Custom Grid Operations

```csharp
public static class CustomHexOperations
{
    public static List<Vector3Int> GetVisibleArea(Vector3Int center, int radius, Vector3Int[] obstacles)
    {
        List<Vector3Int> visible = new List<Vector3Int>();
        List<Vector3Int> area = HexUtils.GetArea(center, radius);
      
        foreach (Vector3Int coord in area)
        {
            if (HasLineOfSight(center, coord, obstacles))
            {
                visible.Add(coord);
            }
        }
      
        return visible;
    }
  
    private static bool HasLineOfSight(Vector3Int from, Vector3Int to, Vector3Int[] obstacles)
    {
        List<Vector3Int> line = HexUtils.GetLine(from, to);
      
        foreach (Vector3Int coord in line)
        {
            if (System.Array.IndexOf(obstacles, coord) != -1)
            {
                return false;
            }
        }
      
        return true;
    }
}
```

## Troubleshooting

### Common Issues

1. **Hexagons not appearing**: Check that the `HexGridGenerator` component is enabled and the radius is greater than 0
2. **Wrong hex positions**: Verify that the `Hex Size` parameter matches your intended scale
3. **UI not working**: Ensure all UI references are properly assigned in the `HexGridUI` component
4. **Performance issues**: Reduce the grid radius or implement object pooling for large grids

### Debug Tips

- Enable `Show Coordinates` to verify cube coordinate calculations
- Use `Show Grid Lines` to check neighbor connections
- Check the console for coordinate conversion logsUse the demo controls to test various grid operations

## License

This hexagonal grid system is provided as-is for educational and development purposes. Feel free to modify and extend it for your projects.

## Contributing

Feel free to submit improvements, bug fixes, or additional features. The system is designed to be modular and extensible.
