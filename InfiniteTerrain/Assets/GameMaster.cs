using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameMaster : MonoBehaviour {

    public static GameMaster gameMaster;
    public TerrainManager terrainManager;
    public TerrainSettings terrainSettings;
    public Vector3 PlayerPosition;
    public GameObject TrackerObject;
    // Use this for initialization
    void Awake () {
        terrainManager = ScriptableObject.CreateInstance<TerrainManager>();
        terrainSettings = ScriptableObject.CreateInstance<TerrainSettings>();
        if (gameMaster != this)
        {
            gameMaster = this;
            DontDestroyOnLoad(this);
        }
        GameObject NewTerrain = TerrainExtensions.CreateTerrain(new Vector3(0f,0f,0f));
    }
}
