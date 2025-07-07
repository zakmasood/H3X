using UnityEngine;
using System.Collections.Generic;

public class ResourceGenerator : MonoBehaviour
{
    [Header("Perlin Noise Settings")]
    [SerializeField] private float noiseScale = 30f; // Smaller scale for more variation
    [SerializeField] private int seed = 0;
    [SerializeField] private Vector2 noiseOffset = Vector2.zero;
    
    [Header("Resource Generation")]
    [SerializeField] private float ironThreshold = 0.2f; // Much lower threshold
    [SerializeField] private float copperThreshold = 0.4f; // Lower threshold
    [SerializeField] private float coalThreshold = 0.6f; // Lower threshold
    [SerializeField] private float resourceDensity = 0.7f; // Higher density
    
    [Header("Debug")]
    [SerializeField] private bool showResourceInfo = true;
    
    private System.Random random;
    
    private void Awake()
    {
        random = new System.Random(seed);
    }
    
    public void GenerateResources(HexGridGenerator gridGenerator)
    {
        if (gridGenerator == null) return;
        
        var hexTiles = gridGenerator.GetAllHexTiles();
        
        int grassCount = 0;
        int ironCount = 0;
        int copperCount = 0;
        int coalCount = 0;
        
        foreach (var hexTile in hexTiles)
        {
            if (hexTile == null) continue;
            
            Vector3Int coords = hexTile.GetComponent<HexTile>().CubeCoordinates;
            ResourceType resourceType = DetermineResourceType(coords);
            
            if (resourceType != ResourceType.Grass)
            {
                // For infinite harvesting, we set a high amount that won't be depleted
                float amount = 999999f; // Effectively infinite
                hexTile.GetComponent<HexTile>().SetResource(resourceType, amount);
                
                if (resourceType == ResourceType.Iron)
                    ironCount++;
                else if (resourceType == ResourceType.Copper)
                    copperCount++;
                else if (resourceType == ResourceType.Coal)
                    coalCount++;
            }
            else
            {
                hexTile.GetComponent<HexTile>().SetResource(ResourceType.Grass);
                grassCount++;
            }
        }
        
        Debug.Log($"Dense Perlin Resource Generation Complete:");
        Debug.Log($"  Grass: {grassCount}, Iron: {ironCount}, Copper: {copperCount}, Coal: {coalCount}");
        Debug.Log($"  Total tiles: {hexTiles.Count}");
        Debug.Log($"  Resource percentage: {((ironCount + copperCount + coalCount) * 100f / hexTiles.Count):F1}%");
    }
    
    private ResourceType DetermineResourceType(Vector3Int coords)
    {
        // Convert cube coordinates to 2D coordinates for noise
        Vector2 noiseCoords = new Vector2(coords.x, coords.z);
        
        // Apply noise scale and offset
        Vector2 scaledCoords = (noiseCoords + noiseOffset) / noiseScale;
        
        // Generate Perlin noise value
        float noiseValue = Mathf.PerlinNoise(scaledCoords.x, scaledCoords.y);
        
        // Add some randomness to break up patterns
        float randomValue = (float)random.NextDouble();
        float finalValue = Mathf.Lerp(noiseValue, randomValue, 0.2f); // Less randomness for more consistent patterns
        
        // Debug: Log some values to see what's happening
        if (coords.x == 0 && coords.z == 0)
        {
            Debug.Log($"Center tile - Noise: {noiseValue:F3}, Random: {randomValue:F3}, Final: {finalValue:F3}");
            Debug.Log($"Thresholds - Iron: {ironThreshold:F3}, Copper: {copperThreshold:F3}, Coal: {coalThreshold:F3}");
        }
        
        // Determine resource type based on thresholds (much lower now)
        if (finalValue > coalThreshold)
        {
            if (coords.x == 0 && coords.z == 0) Debug.Log("Center tile: COAL");
            return ResourceType.Coal;
        }
        else if (finalValue > copperThreshold)
        {
            if (coords.x == 0 && coords.z == 0) Debug.Log("Center tile: COPPER");
            return ResourceType.Copper;
        }
        else if (finalValue > ironThreshold)
        {
            if (coords.x == 0 && coords.z == 0) Debug.Log("Center tile: IRON");
            return ResourceType.Iron;
        }
        else
        {
            if (coords.x == 0 && coords.z == 0) Debug.Log("Center tile: GRASS");
            return ResourceType.Grass;
        }
    }
    
    public void RegenerateResources(HexGridGenerator gridGenerator)
    {
        seed = Random.Range(0, 10000);
        random = new System.Random(seed);
        GenerateResources(gridGenerator);
    }
    
    public void SetNoiseParameters(float scale, float ironThresh, float copperThresh, float coalThresh)
    {
        noiseScale = scale;
        ironThreshold = ironThresh;
        copperThreshold = copperThresh;
        coalThreshold = coalThresh;
        
        Debug.Log($"Dense noise parameters updated - Scale: {scale}, Iron: {ironThresh}, Copper: {copperThresh}, Coal: {coalThresh}");
        
        // Debug: Log expected resource distribution
        Debug.Log($"Expected dense resource distribution:");
        Debug.Log($"  Coal: Values > {coalThresh:F3} ({(1f - coalThresh) * 100:F1}% of tiles)");
        Debug.Log($"  Copper: Values > {copperThresh:F3} but ≤ {coalThresh:F3} ({(coalThresh - copperThresh) * 100:F1}% of tiles)");
        Debug.Log($"  Iron: Values > {ironThresh:F3} but ≤ {copperThresh:F3} ({(copperThresh - ironThresh) * 100:F1}% of tiles)");
        Debug.Log($"  Grass: Values ≤ {ironThresh:F3} ({ironThresh * 100:F1}% of tiles)");
    }
    
    public void SetResourceDensity(float density)
    {
        resourceDensity = Mathf.Clamp01(density);
        
        // Adjust thresholds based on desired density - much lower base values
        float baseIronThreshold = 0.1f; // Very low base threshold
        float baseCopperThreshold = 0.25f; // Low base threshold
        float baseCoalThreshold = 0.45f; // Low base threshold
        
        ironThreshold = Mathf.Lerp(baseIronThreshold, 0.6f, 1f - density);
        copperThreshold = Mathf.Lerp(baseCopperThreshold, 0.75f, 1f - density);
        coalThreshold = Mathf.Lerp(baseCoalThreshold, 0.85f, 1f - density);
        
        Debug.Log($"Dense resource density set to {density} - Iron: {ironThreshold:F3}, Copper: {copperThreshold:F3}, Coal: {coalThreshold:F3}");
    }
    
    // Debug method to visualize noise
    public float GetNoiseValueAt(Vector3Int coords)
    {
        Vector2 noiseCoords = new Vector2(coords.x, coords.z);
        Vector2 scaledCoords = (noiseCoords + noiseOffset) / noiseScale;
        return Mathf.PerlinNoise(scaledCoords.x, scaledCoords.y);
    }
    
    private void OnDrawGizmos()
    {
        if (!showResourceInfo) return;
        
        // This could be used to visualize resource distribution in the scene view
        // Implementation would depend on having access to the grid
    }
} 