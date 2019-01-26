using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunkModel
{
    public int x, z;
    public EntityModel[,] entityModels = new EntityModel[WorldMap.CHUNK_SIZE, WorldMap.CHUNK_SIZE];  
}

public class EntityModel
{
    public string id;
    public Vector2 position;
    public int type;
}

public class EntityModel_Block : EntityModel
{
    public WorldMap.BlockType blockType;
}

public class EntityModel_Furniture : EntityModel
{
    public string something;
}

public class EntityModel_Player : EntityModel
{
    public string nickname;
    public string[] skin;
}

[System.Serializable]
public class BlockVisual
{
    public WorldMap.BlockType blockType;
    public GameObject visualPrefab;
}

public class WorldLayer
{
    public WorldChunk[,] layerMap = new WorldChunk[3, 3];
}

public class WorldChunk
{
    public int x, z;

    public byte[,] map = new byte[WorldMap.CHUNK_SIZE, WorldMap.CHUNK_SIZE];

    public Dictionary<string, GameObject> instantiatedObject = new Dictionary<string, GameObject>();

    public void ClearAndFill(WorldChunkModel worldChunkModel)
    {

        Debug.LogFormat("Chunk x:{0} z:{1} cleared and filled with {2} entities", x, z, worldChunkModel.entityModels.Length);
    }

    public void Update(WorldChunkModel worldChunkModel)
    {
        Debug.LogFormat("Chunk x:{0} z:{1} updated with {2} entities", x, z, worldChunkModel.entityModels.Length);
    }
}

public class WorldMap : MonoBehaviour
{
    public enum BlockType
    {
        Empty = 0,
        Unbreakable = 1,
        Room = 2,
        Wall = 3
    }

    public const int LAYERS_COUNT = 100;
    public const int CHUNK_SIZE = 16;

    public const int CHUNKS_COUNT = 9;

    public List<BlockVisual> blockVisuals = new List<BlockVisual>();
    public WorldLayer[] worldMap = new WorldLayer[LAYERS_COUNT];

    public Dictionary<int, GameObject> objectPool = new Dictionary<int, GameObject>();

    int layers = 3, lx = 5, ly = 6;


    void Start()
    {

        Debug.Log("Filling map..");

        for (int l = 0; l < layers; l++)
        {
            for (int cy = 0; cy < 3; cy++)
            {
                for (int cx = 0; cx < 3; cx++)
                {
                    for (int y = 0; y < ly; y++)
                    {
                        for (int x = 0; x < lx; x++)
                        {
                            if (x == 0 || x == lx - 1 || y == 0 || y == ly - 1)
                            {
                                worldMap[l].layerMap[cx, cy].map[x, y] = (byte)BlockType.Unbreakable;
                            }
                            else
                            {
                                worldMap[l].layerMap[cx, cy].map[x, y] = (byte)BlockType.Unbreakable;
                            }
                        }
                    }
                }
            }
        }

        Debug.Log("Done filling map..");
    }

    public void SetLayer(int layer, WorldChunkModel[,] chunks)
    {
        int curChunkX = worldMap[layer].layerMap[0, 0].x;
        int curChunkZ = worldMap[layer].layerMap[0, 0].z;

        int newChunkX = chunks[0, 0].x;
        int newChunkZ = chunks[0, 0].z;

        int difX = newChunkX - curChunkX;
        int difZ = newChunkZ - curChunkZ;

        bool forceRefresh;

        int chunksRow = 3;

        // No Shift 
        if (difX == 0 && difZ == 0)
        {
        }

        // Shift is too big
        if (difX > 1 || difZ > 1)
        {
            forceRefresh = true;
            Debug.Log("Completely redrawing chunks");

        }

        // Shift right
        if (difX == 1)
        {
            Debug.Log("Shifting right");
            for (int z = 0; z < chunksRow; z++)
            {
                for (int x = 0; x < chunksRow; x++)
                {
                    if(x < chunksRow - 1)
                    {
                        worldMap[layer].layerMap[x, z].Update(chunks[x, z]);
                    }
                    else
                    {
                        worldMap[layer].layerMap[x, z].ClearAndFill(chunks[x, z]);
                    }
                }
            }
        }

        // Shift left
        if (difX == -1)
        {
            Debug.Log("Shifting left");
            for (int z = 0; z < chunksRow; z++)
            {
                for (int x = 0; x < chunksRow; x++)
                {
                    if (x > 0)
                    {
                        worldMap[layer].layerMap[x, z].Update(chunks[x, z]);
                    }
                    else
                    {
                        worldMap[layer].layerMap[x, z].ClearAndFill(chunks[x, z]);
                    }
                }
            }
        }

        // Shift up
        if (difZ == 1)
        {
            Debug.Log("Shifting up");
            for (int z = 0; z < chunksRow; z++)
            {
                for (int x = 0; x < chunksRow; x++)
                {
                    if (z > 0)
                    {
                        worldMap[layer].layerMap[x, z].Update(chunks[x, z]);
                    }
                    else
                    {
                        worldMap[layer].layerMap[x, z].ClearAndFill(chunks[x, z]);
                    }
                }
            }
        }

        // Shift down
        if (difZ == -1)
        {
            Debug.Log("Shifting down");
            for (int z = 0; z < chunksRow; z++)
            {
                for (int x = 0; x < chunksRow; x++)
                {
                    if (z < chunksRow - 1)
                    {
                        worldMap[layer].layerMap[x, z].Update(chunks[x, z]);
                    }
                    else
                    {
                        worldMap[layer].layerMap[x, z].ClearAndFill(chunks[x, z]);
                    }
                }
            }
        }
    }

    public void DrawMap()
    {
        for (int l = 0; l < layers; l++)
        {
            for (int cy = 0; cy < 3; cy++)
            {
                for (int cx = 0; cx < 3; cx++)
                {
                    for (int y = 0; y < ly; y++)
                    {
                        for (int x = 0; x < lx; x++)
                        {
                            var block = worldMap[l].layerMap[cx, cy].map[x, y];



                            DrawAt()
                        }
                    }
                }
            }
        }
    }

    public void DrawAt(int layer, int x, int y, BlockType blockType)
    {


        /*
        switch (blockType)
        {
            case BlockType.Empty:
                {
                    
                }
                break;
            case BlockType.Unbreakable:
                {

                }
                break;
            case BlockType.Room:
                {

                }
                break;
            case BlockType.Wall:
                {

                }
                break;
        }
        */
    }

    public void UpdateMap()
    {

    }

    void Update()
    {
        
    }
}
