using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainScript : MonoBehaviour
{
    // Use this for initialization
    public Terrain terrain;
    public TerrainData terrainData;
    public GameObject tracker;
    public TerrainSettings terrainSettings;
    public BoxCollider coll;
    public Dictionary<TerrainExtensions.Direction, GameObject> Neighbors = new Dictionary<TerrainExtensions.Direction, GameObject>();
    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        terrainSettings = GameMaster.gameMaster.terrainSettings;

        gameObject.AddComponent<BoxCollider>();
        coll = GetComponent<BoxCollider>();
        coll.isTrigger = true;
        coll.size = new Vector3(terrainSettings.MapSize.x, 200f, terrainSettings.MapSize.z);
        coll.center = new Vector3(terrainSettings.MapSize.x / 2, 0f, terrainSettings.MapSize.z / 2);
    }
    void Update()
    {
        if (tracker == null)
        {
            tracker = GameMaster.gameMaster.TrackerObject;
        }
        if (transform.childCount >= 1)
        {
            if (tracker.transform.localPosition.x >= terrainSettings.MapSize.x - terrainSettings.EdgeDistance)
            {
                if (!Neighbors.ContainsKey(TerrainExtensions.Direction.Right))
                {
                    Vector3 Pos = new Vector3(transform.position.x + terrainSettings.MapSize.x,
                        transform.position.y,
                        transform.position.z);
                    if (!GameMaster.gameMaster.terrainManager.TerrainPositions.ContainsKey(Pos))
                    {
                        GameObject NewTerrain = TerrainExtensions.CreateTerrain(Pos, terrain, TerrainExtensions.Direction.Right,null,TerrainBiome.Biomes.Mountain);
                        Neighbors.Add(TerrainExtensions.Direction.Right, NewTerrain);
                        NewTerrain.GetComponent<TerrainScript>().Neighbors.Add(TerrainExtensions.Direction.Left, gameObject);
                    }
                }
            }
            if (tracker.transform.localPosition.z >= terrainSettings.MapSize.z - terrainSettings.EdgeDistance)
            {

                if (!Neighbors.ContainsKey(TerrainExtensions.Direction.UP))
                {
                    Vector3 Pos = new Vector3(transform.position.x,
                        transform.position.y,
                        transform.position.z + terrainSettings.MapSize.z);
                    if (!GameMaster.gameMaster.terrainManager.TerrainPositions.ContainsKey(Pos))
                    {
                        GameObject NewTerrain = TerrainExtensions.CreateTerrain(Pos, terrain, TerrainExtensions.Direction.UP,null,TerrainBiome.Biomes.Planes);
                        Neighbors.Add(TerrainExtensions.Direction.UP, NewTerrain);
                        NewTerrain.GetComponent<TerrainScript>().Neighbors.Add(TerrainExtensions.Direction.Down, gameObject);
                    }
                }
            }
            if (tracker.transform.localPosition.x <= terrainSettings.EdgeDistance)
            {

                if (!Neighbors.ContainsKey(TerrainExtensions.Direction.Left))
                {
                    Vector3 Pos = new Vector3(transform.position.x - terrainSettings.MapSize.x,
                        transform.position.y,
                        transform.position.z);
                    if (!GameMaster.gameMaster.terrainManager.TerrainPositions.ContainsKey(Pos))
                    {
                        GameObject NewTerrain = TerrainExtensions.CreateTerrain(Pos, terrain, TerrainExtensions.Direction.Left,null,TerrainBiome.Biomes.Hills);
                        Neighbors.Add(TerrainExtensions.Direction.Left, NewTerrain);
                        NewTerrain.GetComponent<TerrainScript>().Neighbors.Add(TerrainExtensions.Direction.Right, gameObject);
                    }
                }
            }
            if (tracker.transform.localPosition.z <= terrainSettings.EdgeDistance)
            {

                if (!Neighbors.ContainsKey(TerrainExtensions.Direction.Down))
                {
                    Vector3 Pos = new Vector3(transform.position.x,
                        transform.position.y,
                        transform.position.z - terrainSettings.MapSize.z);
                    if (!GameMaster.gameMaster.terrainManager.TerrainPositions.ContainsKey(Pos))
                    {
                        GameObject NewTerrain = TerrainExtensions.CreateTerrain(Pos, terrain, TerrainExtensions.Direction.Down, null, TerrainBiome.Biomes.Hills);
                        Neighbors.Add(TerrainExtensions.Direction.Down, NewTerrain);
                        NewTerrain.GetComponent<TerrainScript>().Neighbors.Add(TerrainExtensions.Direction.UP, gameObject);
                    }
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            tracker.transform.SetParent(transform);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (transform.childCount <= 0 && other.gameObject.tag == "Player")
        {
            tracker.transform.SetParent(transform);
        }
    }
    private void OnTriggerExit(Collider other )
    {
        if (transform.childCount >= 1 && other.gameObject.tag == "Player")
        {
            transform.DetachChildren();
        }
    }
}
