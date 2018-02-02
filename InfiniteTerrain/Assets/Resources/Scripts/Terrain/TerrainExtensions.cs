using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class TerrainExtensions 
{
    public enum Direction
    {
        UP,
        Down,
        Left,
        Right
    }
    public static bool FirstTerrain;
    public static GameObject CreateTerrain(Vector3 Position,
        Terrain PreviousTerrain = null,
        Direction direction = Direction.UP,
        Dictionary<Direction,GameObject> Neighbors = null
        )
    {
        TerrainData NewTerrainData = new TerrainData();
        // TerrainData settings
        NewTerrainData.heightmapResolution = GameMaster.gameMaster.terrainSettings.HeightMapResolution;
        NewTerrainData.size = GameMaster.gameMaster.terrainSettings.MapSize;
        NewTerrainData.GenerateHeights(Position,direction);

        GameObject NewTerrain = Terrain.CreateTerrainGameObject(NewTerrainData);
        // GameObject Settings
        NewTerrain.transform.position = Position;
        NewTerrain.AddComponent<TerrainScript>();
        // Terrain Settings
        Terrain ThisTerrain = NewTerrain.GetComponent<Terrain>();

        GameMaster.gameMaster.terrainManager.TerrainPositions.Add(Position,NewTerrain);
        return NewTerrain;
    }
    public static float[,] GetHeights(this Terrain terrain, Direction direction)
    {
        TerrainData terrainData = terrain.terrainData;
        return terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
    }
    public static Dictionary<Direction, GameObject> GetNeighbors(Vector3 TerrainPosition)
    {
        GameObject UP;
        GameObject Down;
        GameObject Left;
        GameObject Right;

        Dictionary<Direction, GameObject> NeighborDictionary = new Dictionary<Direction, GameObject>()
        {
            {Direction.UP,null },
            {Direction.Down,null },
            {Direction.Right,null },
            {Direction.Left,null },

        };

        Vector3 UpNeighbor = new Vector3(TerrainPosition.x,
            0f,
            TerrainPosition.z - GameMaster.gameMaster.terrainSettings.MapSize.z);

        if (GameMaster.gameMaster.terrainManager.TerrainPositions.ContainsKey(UpNeighbor))
        {
            UP = GameMaster.gameMaster.terrainManager.TerrainPositions.Where(x => x.Key == UpNeighbor).Select(y => y.Value).FirstOrDefault();
            NeighborDictionary[Direction.UP] = UP;
        }


        Vector3 DownNeighbor = new Vector3(TerrainPosition.x,
            0f,
            TerrainPosition.z + GameMaster.gameMaster.terrainSettings.MapSize.z);

        if (GameMaster.gameMaster.terrainManager.TerrainPositions.ContainsKey(DownNeighbor))
        {
            Down = GameMaster.gameMaster.terrainManager.TerrainPositions.Where(x => x.Key == DownNeighbor).Select(y => y.Value).FirstOrDefault();
            NeighborDictionary[Direction.Down] = Down;
        }


        Vector3 RightNeighbor = new Vector3(TerrainPosition.x - GameMaster.gameMaster.terrainSettings.MapSize.x,
            0f,
            TerrainPosition.z);

        if (GameMaster.gameMaster.terrainManager.TerrainPositions.ContainsKey(RightNeighbor))
        {
            Right = GameMaster.gameMaster.terrainManager.TerrainPositions.Where(x => x.Key == RightNeighbor).Select(y => y.Value).FirstOrDefault();
            NeighborDictionary[Direction.Right] = Right;
        }


        Vector3 LeftNeighbor = new Vector3(TerrainPosition.x + GameMaster.gameMaster.terrainSettings.MapSize.x,
            0f,
            TerrainPosition.z);

        if (GameMaster.gameMaster.terrainManager.TerrainPositions.ContainsKey(LeftNeighbor))
        {
            Left = GameMaster.gameMaster.terrainManager.TerrainPositions.Where(x => x.Key == LeftNeighbor).Select(y => y.Value).FirstOrDefault();
            NeighborDictionary[Direction.Left] = Left;
        }

        return NeighborDictionary;
    }
    public static void GenerateHeights(this TerrainData terrainData,
        Vector3 TerrainPosition,
        Direction direction = Direction.UP
        )
    {
        Dictionary<Direction, GameObject> Neighbors = GetNeighbors(TerrainPosition);
        float[,] NewHeights = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
        float RandomNoise = 200f;// Random.Range(50f, 300f);

        float[,] UpHeights;
        float[,] DownHeights;
        float[,] RightHeights;
        float[,] LeftHeights;

        if (Neighbors[Direction.UP] != null)
        {
            UpHeights = Neighbors[Direction.UP].GetComponent<Terrain>().GetHeights(Direction.UP);
        }
        else
        {
            UpHeights = new float[0, 0];
        }
        if (Neighbors[Direction.Down] != null)
        {
            DownHeights = Neighbors[Direction.Down].GetComponent<Terrain>().GetHeights(Direction.Down);

        }
        else
        {
            DownHeights = new float[0, 0];
        }
        if (Neighbors[Direction.Right] != null)
        {
            RightHeights = Neighbors[Direction.Right].GetComponent<Terrain>().GetHeights(Direction.Right);
        }
        else
        {
            RightHeights = new float[0, 0];
        }
        if (Neighbors[Direction.Left] != null)
        {
            LeftHeights = Neighbors[Direction.Left].GetComponent<Terrain>().GetHeights(Direction.Left);
        }
        else
        {
            LeftHeights = new float[0, 0];
        }
        int res = terrainData.heightmapResolution - 1;
        if (Neighbors[Direction.UP] == null && Neighbors[Direction.Down] == null && Neighbors[Direction.Left] == null && Neighbors[Direction.Right] == null)
        {
            FirstTerrain = true;
        }
        else
        {
            FirstTerrain = false;
        }
        int EithRes = res / 8;
        int NegRes = res - EithRes;
        for (int x = 0; x <= terrainData.heightmapResolution - 1; x++)
        {
            for (int z = 0; z <= terrainData.heightmapResolution - 1; z++)
            {
                if (FirstTerrain)
                {
                    NewHeights[x, z] = Mathf.PerlinNoise(x / RandomNoise, z / RandomNoise);
                }
                else
                {

                    if (UpHeights.Length > 6)
                    {
                        float Height1 = UpHeights[res, z];
                        NewHeights[0, z] = Height1; // Good
                        if (x >= 1 && (z < EithRes || z > NegRes))
                        {
                            float Height2 = NewHeights[x, z];
                            NewHeights[x, z] = Mathf.PerlinNoise(x / RandomNoise, z / RandomNoise);
                            float Height3 = Mathf.SmoothStep(NewHeights[x - 1, z], Height2, 0.1f);
                            NewHeights[x, z] = Height3; // Good
                        }
                        else if (z > EithRes || z < NegRes)
                        {
                            NewHeights[x, z] = Mathf.PerlinNoise(x / RandomNoise, z / RandomNoise);
                        }
                    }
                    if (DownHeights.Length > 6)
                    {
                        float Height1 = DownHeights[0, z];
                        NewHeights[res, z] = Height1; // Good
                        if (x >= 1 && (z < EithRes || z > NegRes))
                        {
                            float Height2 = NewHeights[res - x, z];
                            NewHeights[res - x, z] = Mathf.PerlinNoise(x / RandomNoise, z / RandomNoise);
                            float Height3 = Mathf.SmoothStep(NewHeights[res - x + 1, z], Height2, 0.1f);
                            NewHeights[res - x, z] = Height3; // Good
                        }
                        else if (z > EithRes || z < NegRes)
                        {
                            NewHeights[res - x, z] = Mathf.PerlinNoise(x / RandomNoise, z / RandomNoise);
                        }
                    }
                    if (RightHeights.Length > 6)
                    {
                        float Height1 = RightHeights[z, res];
                        NewHeights[z, 0] = Height1; // Good
                        if (x >= 1 && (z < EithRes || z > NegRes))
                        {
                            float Height2 = NewHeights[z, x];
                            NewHeights[z, x] = Mathf.PerlinNoise(x / RandomNoise, z / RandomNoise);
                            float Height3 = Mathf.SmoothStep(NewHeights[z, x - 1], Height2, 0.1f);
                            NewHeights[z, x] = Height3; // Good
                        }
                        else if (z > EithRes || z < NegRes)
                        {
                            NewHeights[z, x] = Mathf.PerlinNoise(x / RandomNoise, z / RandomNoise);
                        }
                    }
                    if (LeftHeights.Length > 6)
                    {
                        float Height1 = LeftHeights[z, 0];
                        NewHeights[z, res] = LeftHeights[z, 0]; // Good
                        if (x >= 1 && (z < EithRes || z > NegRes))
                        {
                            float Height2 = NewHeights[z, res - x];
                            NewHeights[z, res - x] = Mathf.PerlinNoise(x / RandomNoise, z / RandomNoise);
                            float Height3 = Mathf.SmoothStep(NewHeights[z, res - x + 1], Height2, 0.1f);
                            NewHeights[z, res - x] = Height3; // Good
                        }
                        else if (z > EithRes || z < NegRes)
                        {
                            NewHeights[z, res - x] = Mathf.PerlinNoise(x / RandomNoise, z / RandomNoise);
                        }
                    }
                }
            }
        }
        terrainData.SetHeights(0,0,NewHeights);
    }
}