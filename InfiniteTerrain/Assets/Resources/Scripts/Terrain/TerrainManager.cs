using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TerrainManager : ScriptableObject
{
    public Dictionary<Vector3, GameObject> TerrainPositions = new Dictionary<Vector3, GameObject>();
    public List<GameObject> Terrains = new List<GameObject>();
    public List<Vector3> ExistingTerrains = new List<Vector3>();
}
public class TerrainSettings : ScriptableObject
{
    public int HeightMapResolution = 1024;
    public Vector3 MapSize = new Vector3(512f,20f, 512f);
    public float EdgeDistance = 512;
}
public class TerrainBiome : ScriptableObject
{
    public enum Biomes
    {
        Planes,
        Hills,
        Mountain
    }
    public IEnumerable<UnityEngine.Object> LoadResource(string Path,string BiomeType)
    {
        return Resources.LoadAll(Path).Where(x => x.name.Contains(BiomeType));
    }
    public SplatPrototype[] GetTextures(Biomes biomes)
    {
        IEnumerable<UnityEngine.Object> Textures = LoadResource("Scripts/Terrain/Textures", biomes.ToString());
        List<SplatPrototype> splat = new List<SplatPrototype>();
        foreach (Texture2D Tex in Textures)
        {
            SplatPrototype Stex = new SplatPrototype();
            Stex.texture = Tex;
            splat.Add(Stex);
        }
        return splat.ToArray();
    }
    public TerrainSettingContainer ReturnTerrainSettings(Biomes biomes)
    {
        TerrainSettingContainer terrainSettingContainer = new TerrainSettingContainer();
        switch (biomes)
        {
            case Biomes.Planes:
                terrainSettingContainer.NoiseMin = 50f;
                terrainSettingContainer.NoiseMax = 200f;
                terrainSettingContainer.MaxHeight = 20f;
                terrainSettingContainer.MaxAngle = 1f;
                terrainSettingContainer.Textures = GetTextures(Biomes.Planes);
                break;
            case Biomes.Hills:
                break;
            case Biomes.Mountain:
                break;
            default:
                break;
        }
        return terrainSettingContainer;
    }
}
public class TerrainSettingContainer
{
    public float NoiseMin { get; set; }
    public float NoiseMax { get; set; }
    public float MaxAngle { get; set; }
    public SplatPrototype[] Textures { get; set; }
    public float MaxHeight { get; set; }
}