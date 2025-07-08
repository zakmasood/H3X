# Using Blender Hexagon with Unity Hex Grid

## Setup Instructions

### 1. Import Your Blender Hexagon

1. **Export from Blender**:

   - Select your hexagon mesh
   - File → Export → FBX (.fbx)
   - Enable "Apply Transform" if needed
   - Set scale to 1.0
2. **Import to Unity**:

   - Drag the .fbx file into your Unity Assets folder
   - In the import settings:
     - Set **Scale Factor** to 1.0
     - Enable **Generate Colliders** if you want physics
     - Set **Import Normals** to "Calculate" for proper lighting

### 2. Create a Hex Tile Prefab

1. **Drag your imported hexagon** from the Project window into the Scene
2. **Add the HexTile component** to the hexagon GameObject
3. **Add a Collider** if needed (Mesh Collider recommended)
4. **Set up materials** and textures as desired
5. **Drag the hexagon** from the Scene back to the Project window to create a prefab
6. **Delete the hexagon** from the Scene (keep the prefab)

### 3. Configure the Hex Grid Generator

1. **Select your HexGridGenerator** GameObject in the scene
2. **Drag your hex prefab** into the "Hex Tile Prefab" field in the inspector
3. **Adjust the Hex Size** to match your Blender model's scale
4. **Test the grid generation** by pressing Play

### 4. Optional: Adjust Positioning

If your hexagons don't align properly:

1. **Check the origin point** of your Blender model

   - The hexagon should be centered at (0,0,0) in Blender
   - The bottom face should be at Y=0
2. **Adjust the Hex Size** in the HexGridGenerator:

   - This controls the spacing between hexagons
   - Should match the "radius" or "size" of your hexagon model
3. **Modify the CubeToWorldPosition method** if needed:

   ```csharp
   // If your hexagons are too close or far apart
   float x = hexSize * (Mathf.Sqrt(3) * cubeCoord.x + Mathf.Sqrt(3) / 2 * cubeCoord.y);
   float z = hexSize * (3f / 2 * cubeCoord.y);
   ```

## Tips for Blender Hexagon Design

### Optimal Hexagon Properties:

- **Flat-topped orientation** (edges parallel to X-axis)
- **Centered origin** at (0,0,0)
- **Bottom face at Y=0**
- **Consistent scale** (recommend 1 unit radius)
- **Proper normals** pointing upward

### Material Setup:

- **Use PBR materials** for best Unity compatibility
- **Export textures** as separate files
- **Set up UV maps** if using textures
- **Consider emission** for special effects

### Performance Considerations:

- **Keep polygon count reasonable** (under 1000 verts per hex)
- **Use LOD (Level of Detail)** for distant hexagons
- **Optimize textures** (power of 2 dimensions)
- **Consider instancing** for large grids

## Troubleshooting

### Hexagons Not Aligning:

- Check that your Blender model is centered
- Adjust the `hexSize` parameter
- Verify the hexagon orientation (flat-topped vs pointy-topped)

### Materials Not Showing:

- Ensure materials are properly assigned in Blender
- Check that textures are in the same folder as the .fbx
- Verify shader compatibility

### Performance Issues:

- Reduce polygon count in Blender
- Use texture atlasing
- Enable GPU instancing in Unity

### Collision Issues:

- Add a Mesh Collider to your prefab
- Ensure the mesh is properly closed
- Check that the collider matches the visual mesh

## Example Blender Export Settings

```
Format: FBX (.fbx)
Scale: 1.0
Apply Transform: ✓
Apply Modifiers: ✓
Include Edges: ✗
Include Custom Properties: ✗
Bake Animation: ✗
```

## Advanced: Custom Hex Variants

You can create multiple hex prefabs for different tile types:

1. **Create variants** in Blender (grass, water, mountain, etc.)
2. **Export each as separate .fbx files**
3. **Create prefabs** for each variant
4. **Modify the HexGridGenerator** to randomly select prefabs
5. **Or create a tile type system** that assigns specific prefabs to coordinates

This gives you a flexible system that can handle different terrain types and visual styles!
