using UnityEngine;
using System.Collections;

public class TrackerObject : MonoBehaviour
{
    // Update is called once per frame
    private void Awake()
    {
        GameMaster.gameMaster.TrackerObject = gameObject;
    }
    void Update()
    {
        transform.position = GameMaster.gameMaster.PlayerPosition;
    }
}
