using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunkModel
{
    public int x, z;
    public List<EntityModel> entityModels;  
}

public abstract class EntityModel
{
    public string id;
    public Vector2 position;

    public Entity Create(int layer, EntityModel entityModel)
    {
        return Entity.CreateEntity(layer, entityModel);
    }

    public void Update(int layer, EntityModel entityModel, Entity entity)
    {
        Entity.UpdateEntity(layer, entityModel, entity);
    }

    public Vector3 WorldPosition(int layer)
    {
        return new Vector3(position.x, layer, position.y);
    }
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
    //public string[] skin;
    public Vector2 velocity;
}

public class WorldLayer
{
    public WorldChunk[,] layerMap = new WorldChunk[3, 3];
}

public class WorldChunk
{
    public int x, z;

    public byte[,] map = new byte[WorldMap.CHUNK_SIZE, WorldMap.CHUNK_SIZE];

    public Dictionary<string, Entity> instantiatedObjects = new Dictionary<string, Entity>();

    public void ClearAndFill(int layer, WorldChunkModel worldChunkModel)
    {
        x = worldChunkModel.x;
        z = worldChunkModel.z;

        foreach(var obj in instantiatedObjects)
        {
            Object.Destroy(obj.Value);
        }

        Update(layer, worldChunkModel);

        //Debug.LogFormat("Chunk x:{0} z:{1} cleared and filled with {2} entities", x, z, worldChunkModel.entityModels.Length);
    }

    public void Update(int layer, WorldChunkModel worldChunkModel)
    {
        Debug.LogFormat("Chunk x:{0} z:{1} updated with {2} entities", x, z, worldChunkModel.entityModels.Count);

        foreach(EntityModel entityModel in worldChunkModel.entityModels)
        {
            Entity entity;

            if (!WorldMap.GlobalInstantiatedObjects.TryGetValue(entityModel.id, out entity))
            {
                Entity newEntity = entityModel
                    .Create(layer, entityModel);

                instantiatedObjects.Add(entityModel.id, newEntity);
                WorldMap.GlobalInstantiatedObjects.Add(entityModel.id, newEntity);
            }
            else
            {
                entityModel.Update(layer, entityModel, entity);
            }
        }
    }
}

public class WorldMap : MonoBehaviour
{
    static WorldMap instance;
    public static Dictionary<string, Entity> GlobalInstantiatedObjects = new Dictionary<string, Entity>();

    public enum BlockType
    {
        Empty = 0,
        Unbreakable = 1,
        Room = 2,
        Wall = 3
    }

    public const int LAYERS_COUNT = 50;
    public const int CHUNK_SIZE = 16;

    public const int CHUNKS_COUNT = 9;

    public WorldLayer[] worldMap = new WorldLayer[LAYERS_COUNT];

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < LAYERS_COUNT; i++)
        {
            worldMap[i] = new WorldLayer();
            worldMap[i].layerMap = new WorldChunk[3, 3];
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    worldMap[i].layerMap[k, j] = new WorldChunk();
                }
            }
        }
    }

    public void SetLayer(int layer, WorldChunkModel[,] chunks)
    {
        int curChunkX = worldMap[layer].layerMap[0, 0].x;
        int curChunkZ = worldMap[layer].layerMap[0, 0].z;

        int newChunkX = chunks[0, 0].x;
        int newChunkZ = chunks[0, 0].z;

        int difX = newChunkX - curChunkX;
        int difZ = newChunkZ - curChunkZ;

        int chunksRow = 3;

        // No Shift 
        if (difX == 0 && difZ == 0)
        {
            Debug.Log("No shift");
            for (int z = 0; z < chunksRow; z++)
            {
                for (int x = 0; x < chunksRow; x++)
                {
                    worldMap[layer].layerMap[x, z].Update(layer, chunks[x, z]);
                }
            }
        }

        // Shift is too big
        if (difX > 1 || difZ > 1)
        {
            Debug.Log("Completely redrawing chunks");
            for (int z = 0; z < chunksRow; z++)
            {
                for (int x = 0; x < chunksRow; x++)
                {
                    worldMap[layer].layerMap[x, z].ClearAndFill(layer, chunks[x, z]);
                }
            }
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
                        worldMap[layer].layerMap[x, z].Update(layer, chunks[x, z]);
                    }
                    else
                    {
                        worldMap[layer].layerMap[x, z].ClearAndFill(layer, chunks[x, z]);
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
                        worldMap[layer].layerMap[x, z].Update(layer, chunks[x, z]);
                    }
                    else
                    {
                        worldMap[layer].layerMap[x, z].ClearAndFill(layer, chunks[x, z]);
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
                        worldMap[layer].layerMap[x, z].Update(layer, chunks[x, z]);
                    }
                    else
                    {
                        worldMap[layer].layerMap[x, z].ClearAndFill(layer, chunks[x, z]);
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
                        worldMap[layer].layerMap[x, z].Update(layer, chunks[x, z]);
                    }
                    else
                    {
                        worldMap[layer].layerMap[x, z].ClearAndFill(layer, chunks[x, z]);
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
