using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerTracker : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(GameMaster.gameMaster.terrainSettings.MapSize.x / 2,
            100f,
            GameMaster.gameMaster.terrainSettings.MapSize.z / 2);
    }

    // Update is called once per frame
    void Update()
    {
        GameMaster.gameMaster.PlayerPosition = transform.position;
    }
}
