using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : ScriptableObject
{
    public Dictionary<Vector3, GameObject> TerrainPositions = new Dictionary<Vector3, GameObject>();
    public List<GameObject> Terrains = new List<GameObject>();
    public List<Vector3> ExistingTerrains = new List<Vector3>();
}
public class TerrainSettings : ScriptableObject
{
    public int HeightMapResolution = 512;
    public Vector3 MapSize = new Vector3(100f,20f,100f);
    public float EdgeDistance = 512;
}
