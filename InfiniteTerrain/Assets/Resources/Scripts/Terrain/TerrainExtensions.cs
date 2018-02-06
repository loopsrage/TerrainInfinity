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
        TerrainSettingContainer BiomeSettings = GameMaster.gameMaster.terrainBiome.ReturnTerrainSettings(TerrainBiome.Biomes.Planes);
        TerrainData NewTerrainData = new TerrainData();
        // TerrainData settings
        NewTerrainData.heightmapResolution = GameMaster.gameMaster.terrainSettings.HeightMapResolution;
        NewTerrainData.size = GameMaster.gameMaster.terrainSettings.MapSize;
        NewTerrainData.TerrainTextures(BiomeSettings.Textures);
        NewTerrainData.GenerateHeights(Position,direction,BiomeSettings.NoiseMin,BiomeSettings.NoiseMax);
        NewTerrainData.GenerateAlphaMap();
        NewTerrainData.GenerateFoliage();
        NewTerrainData.GenerateGrass();

        // Texture Settings
        GameObject NewTerrain = Terrain.CreateTerrainGameObject(NewTerrainData);
        // GameObject Settings
        Vector3 NewTerrainPosition = new Vector3(Position.x,Position.y,Position.z);
        NewTerrain.transform.position = NewTerrainPosition;
        NewTerrain.AddComponent<TerrainScript>();
        // Terrain Settings
        Terrain ThisTerrain = NewTerrain.GetComponent<Terrain>();
        ThisTerrain.detailObjectDistance = 250;

        GameMaster.gameMaster.terrainManager.TerrainPositions.Add(Position,NewTerrain);
        return NewTerrain;
    }
    public static float[,] GetHeights(this Terrain terrain, Direction direction)
    {
        TerrainData terrainData = terrain.terrainData;
        return terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
    }
    public static void TerrainTextures(this TerrainData terrainData, SplatPrototype[] splatPrototype)
    {
        terrainData.splatPrototypes = splatPrototype;
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
        Direction direction = Direction.UP,
        float RangeMin = 50f,
        float RangeMax = 200f
        )
    {
        Dictionary<Direction, GameObject> Neighbors = GetNeighbors(TerrainPosition);
        float[,] NewHeights = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
        float RandomNoise = Random.Range(RangeMin, RangeMax) + TerrainPosition.x + TerrainPosition.z;

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
        int EithRes = res / 54;
        int NegRes = res - EithRes;
        for (int x = 0; x <= terrainData.heightmapResolution - 1; x++)
        {
            for (int z = 0; z <= terrainData.heightmapResolution - 1; z++)
            {
                if (FirstTerrain)
                {
                    NewHeights[x, z] = Mathf.PerlinNoise(x / RandomNoise , z / RandomNoise );
                }
                else
                {

                    if (UpHeights.Length > 6)
                    {
                        float Height1 = UpHeights[res, z];
                        NewHeights[0, z] = Height1; // Good
                        if (x >= 1)
                        {

                            NewHeights[x, z] = Mathf.PerlinNoise(x / RandomNoise, z / RandomNoise);
                            float Height2 = NewHeights[x, z];
                            float Height3 = 0f;
                            if (x < EithRes || z > NegRes)
                            {
                                Height3 = Mathf.SmoothStep(NewHeights[x - 1, z], Height2, 0.00001f);
                            }
                            if (x < EithRes || (z > NegRes || z < EithRes))
                            {
                                Height3 = Mathf.SmoothStep(UpHeights[res - x, z], Height2, 0.00001f);
                            }
                            else
                            {
                                Height3 = Mathf.MoveTowards(UpHeights[res - x + 1, z], Height2, 0.00001f);
                            }
                            NewHeights[x, z] = Height3; // Good
                        }
                    }
                    if (DownHeights.Length > 6)
                    {
                        float Height1 = DownHeights[0, z];
                        NewHeights[res, z] = Height1; // Good
                        if (x >= 1)
                        {
                            NewHeights[res - x, z] = Mathf.PerlinNoise(x / RandomNoise , z / RandomNoise );
                            float Height2 = NewHeights[res - x, z];
                            float Height3 = 0f;
                            if (x < EithRes || z > NegRes)
                            {
                                Height3 = Mathf.SmoothStep(NewHeights[res - x + 1, z], Height2, 0.00001f);
                            }
                            if (x < EithRes || (z > NegRes || z < EithRes))
                            {
                                Height3 = Mathf.SmoothStep(DownHeights[x,z], Height2, 0.00001f);
                            }
                            else
                            {
                                Height3 = Mathf.MoveTowards(DownHeights[x - 1, z], Height2, 0.00001f);
                            }
                            NewHeights[res - x, z] = Height3; // Good
                        }
                    }
                    if (RightHeights.Length > 6)
                    {
                        float Height1 = RightHeights[z, res];
                        NewHeights[z, 0] = Height1; // Good
                        if (x >= 1)
                        {
                            NewHeights[z, x] = Mathf.PerlinNoise(x / RandomNoise , z / RandomNoise );
                            float Height2 = NewHeights[z, x];
                            float Height3 = 0f;
                            if (x < EithRes || z > NegRes)
                            {
                                Height3 = Mathf.SmoothStep(NewHeights[z, x - 1], Height2, 0.00001f);
                            }
                            if (x < EithRes || (z > NegRes || z < EithRes))
                            {
                                Height3 = Mathf.SmoothStep(RightHeights[z, res - x], Height2, 0.00001f);
                            }
                            else
                            {
                                Height3 = Mathf.MoveTowards(RightHeights[z, res - x + 1], Height2, 0.00001f);
                            }
                            NewHeights[z, x] = Height3; // Good
                        }
                    }
                    if (LeftHeights.Length > 6)
                    {
                        float Height1 = LeftHeights[z, 0];
                        NewHeights[z, res] = LeftHeights[z, 0]; // Good
                        if (x >= 1)
                        {
                            NewHeights[z, res - x] = Mathf.PerlinNoise(x / RandomNoise, z / RandomNoise );
                            float Height2 = NewHeights[z, res - x];
                            float Height3 = 0f;
                            if (z > NegRes || z < EithRes)
                            {
                                Height3 = Mathf.SmoothStep(NewHeights[z, res - x + 1], Height2, 0.00001f);
                            }
                            if (x < EithRes || (z > NegRes || z < EithRes))
                            {
                                Height3 = Mathf.SmoothStep(LeftHeights[z,x], Height2, 0.00001f);
                            }
                            else
                            {
                                Height3 = Mathf.MoveTowards(LeftHeights[z, x - 1], Height2, 0.00001f);
                            }
                            NewHeights[z, res - x] = Height3; // Good
                        }
                    }
                }
            }
        }
        terrainData.SetHeights(0,0,NewHeights);
    }
    public static void GenerateAlphaMap(this TerrainData terrainData)
    {
        int Res = (int)GameMaster.gameMaster.terrainSettings.AlphaMapResolution;
        float[,,] AlphaMap = new float[Res, Res, 3];

        for (int x = 0; x < GameMaster.gameMaster.terrainSettings.AlphaMapResolution ; x++)
        {
            for (int y = 0; y < GameMaster.gameMaster.terrainSettings.AlphaMapResolution ; y++)
            {
                SplatPrototype[] Textures = terrainData.splatPrototypes;
                float[] splatWeights = new float[terrainData.alphamapLayers];
                float x_01 = (float)x / (float)Res;
                float y_01 = (float)y / (float)Res;
                float Height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapHeight), Mathf.RoundToInt(x_01 * terrainData.heightmapWidth));
                Vector3 Normal = terrainData.GetInterpolatedNormal(y_01, x_01);
                float steepness = terrainData.GetSteepness(y_01, x_01);

                splatWeights[2] = 0.1f;
                splatWeights[1] = Mathf.Clamp01(terrainData.heightmapHeight - Height);
                splatWeights[0] = 1.0f - Mathf.Clamp01(steepness * steepness / (terrainData.heightmapHeight / 5.0f));

                float z = splatWeights.Sum();

                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {
                    splatWeights[i] /= z;
                    AlphaMap[x, y, i] = splatWeights[i];
                }
            }
        }
        terrainData.SetAlphamaps(0,0,AlphaMap);
    }
    public static void GenerateFoliage(this TerrainData terrainData)
    {
        int TreeCount = Random.Range(10,20);
        int ScaleRand = Random.Range(5,15);
        GameObject[] Trees = Resources.LoadAll<GameObject>("Scripts/Terrain/Trees");
        TreeInstance[] treeInstance = new TreeInstance[TreeCount];
        TreePrototype[] treePrototype = new TreePrototype[Trees.Length];
        for (int p = 0; p < treePrototype.Length; p++)
        {
            treePrototype[p] = new TreePrototype();
            treePrototype[p].prefab = Trees[p];

        }
        terrainData.treePrototypes = treePrototype;
        for (int t = 0; t < treeInstance.Length; t++)
        {
            int RandomIndex = Random.Range(0, treePrototype.Length);
            Vector3 RandPosition = new Vector3(Random.Range(0,100) / 100f, 0f, Random.Range(0, 100) / 100f);
            float RandRot = Random.Range(0, 360);
            treeInstance[t] = new TreeInstance();
            treeInstance[t].prototypeIndex = RandomIndex;
            treeInstance[t].heightScale = ScaleRand;
            treeInstance[t].widthScale = ScaleRand;
            treeInstance[t].position = RandPosition;
            treeInstance[t].rotation = RandRot;
        }
        terrainData.treeInstances = treeInstance;
    }
    public static void GenerateGrass(this TerrainData terrainData)
    {
        int DetailResolution = 512;
        int DetailPerPatch = 8;
        Texture2D[] GrassTextures = Resources.LoadAll<Texture2D>("Scripts/Terrain/Grass");
        DetailPrototype[] detailPrototype = new DetailPrototype[GrassTextures.Length];
        int[,] DetailMap = new int[DetailResolution, DetailResolution];
        terrainData.SetDetailResolution(DetailResolution,DetailPerPatch);
        for (int g = 0; g < detailPrototype.Length; g++)
        {
            detailPrototype[g] = new DetailPrototype();
            detailPrototype[g].prototypeTexture = GrassTextures[g];
            detailPrototype[g].renderMode = DetailRenderMode.Grass;
            detailPrototype[g].minHeight = 1f;
            detailPrototype[g].minWidth= 1f;
            detailPrototype[g].maxHeight = 5f;
            detailPrototype[g].maxWidth = 2f;
            detailPrototype[g].noiseSpread = 0.1f;
        }
        for (int x = 0; x < DetailResolution; x++)
        {
            for (int z = 0; z < DetailResolution; z++)
            {
                if (terrainData.GetHeight(x, z) < 9.0f)
                {
                    DetailMap[x, z] = 1;
                }
                else
                {
                    if (Random.Range(0,20) < 2)
                    {
                        DetailMap[x, z] = 1;
                    }
                    else
                    {
                        DetailMap[x, z] = 0;
                    }
                }
            }
        }
        terrainData.detailPrototypes = detailPrototype;
        terrainData.SetDetailLayer(0, 0, 0, DetailMap);
    }
}