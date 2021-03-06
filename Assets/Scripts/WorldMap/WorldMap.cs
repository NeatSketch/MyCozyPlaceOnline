﻿using System.Collections;
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
    public int furnitureType;
    public int rotation;
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

    static GameObject planePrefab;

    public Dictionary<string, Entity> instantiatedObjects = new Dictionary<string, Entity>();

    GameObject groundPlane;
    /*
    public void FakeCreate(int x, int z, )
    {

    }
    */

    GameObject CreatePlane(WorldChunkModel worldChunkModel)
    {
        if(!planePrefab)
        {
            planePrefab = Resources.Load<GameObject>("Prefabs/GroundPlane");
        }

        GameObject plane = Object.Instantiate(planePrefab);

        plane.transform.localScale = Vector3.one * WorldMap.CHUNK_SIZE;

        return plane;
    }

    Vector3 GetChunkCenter(WorldChunkModel worldChunkModel)
    {
        return new Vector3
        (
        worldChunkModel.x * WorldMap.CHUNK_SIZE + (WorldMap.CHUNK_SIZE / 2f),
        0,
        worldChunkModel.z * WorldMap.CHUNK_SIZE + (WorldMap.CHUNK_SIZE / 2f)
        );
    }


    public WorldChunk(int layer, WorldChunkModel worldChunkModel)
    {
        Debug.Log("Creating " + worldChunkModel.x + " : " + worldChunkModel.z);

        ClearAndUpdate(layer, worldChunkModel);

        if(layer == 0)
        {
            groundPlane = CreatePlane(worldChunkModel);
        }
    }

    public void ClearAndUpdate(int layer, WorldChunkModel worldChunkModel)
    {
        x = worldChunkModel.x;
        z = worldChunkModel.z;

        Clear();

        Update(layer, worldChunkModel);

        //Debug.LogFormat("Chunk x:{0} z:{1} cleared and filled with {2} entities", x, z, worldChunkModel.entityModels.Length);
    }

    public void Update(int layer, WorldChunkModel worldChunkModel)
    {
        x = worldChunkModel.x;
        z = worldChunkModel.z;
        //Debug.LogFormat("Chunk x:{0} z:{1} updated with {2} entities", x, z, worldChunkModel.entityModels.Count);

        instantiatedObjects = new Dictionary<string, Entity>();

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
                instantiatedObjects.Add(entityModel.id, entity);
                entityModel.Update(layer, entityModel, entity);
            }
        }    

        if(!groundPlane)
        {
            groundPlane = CreatePlane(worldChunkModel);
        }
        else
        {
            groundPlane.transform.position = GetChunkCenter(worldChunkModel);
        }
        
    }

    public void Clear()
    {
        foreach (var obj in instantiatedObjects)
        {
            Debug.Log("Destr:" + obj.Value.name);
            Object.Destroy(obj.Value.gameObject);
            WorldMap.GlobalInstantiatedObjects.Remove(obj.Key);
        }

        instantiatedObjects.Clear();

        Object.Destroy(groundPlane);
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

    bool filled;

    public WorldLayer[] worldMap = new WorldLayer[LAYERS_COUNT];

    private void Awake()
    {
        instance = this;
        
    }

    public static WorldChunk ChunkFromPosition(int layer, int x, int z)
    {
        WorldChunk chunk = null;

        for (int j = 0; j < 3; j++)
        {
            for (int k = 0; k < 3; k++)
            {
                int minX = instance.worldMap[layer].layerMap[k, j].x * CHUNK_SIZE;
                int maxX = instance.worldMap[layer].layerMap[k, j].x * CHUNK_SIZE + CHUNK_SIZE;

                int minZ = instance.worldMap[layer].layerMap[k, j].z * CHUNK_SIZE;
                int maxZ = instance.worldMap[layer].layerMap[k, j].z * CHUNK_SIZE + CHUNK_SIZE;

                if(x <= maxX && x >= minX && z <= maxZ && z >= minZ)
                {
                    chunk = instance.worldMap[layer].layerMap[k, j];
                }
            }
        }

        return chunk;
    }

    //public void SetBlock(int layer,)

    public void SetLayer(int layer, WorldChunkModel[,] chunks)
    {

        if (!filled)
        {
            worldMap[layer] = new WorldLayer();
            worldMap[layer].layerMap = new WorldChunk[3, 3];
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    worldMap[layer].layerMap[k, j] = new WorldChunk(layer, chunks[k, j]);
                }
            }

            filled = true;
            return;
        }

        string debug = "";
        for (int z = 0; z < 3; z++)
        {
            if (z != 0)
                debug += " -----\t| -----\t| -----\t\n";
            for (int x = 0; x < 3; x++)
            {
                debug += " " + worldMap[layer].layerMap[x, z].x + ", " + worldMap[layer].layerMap[x, z].z + "\t|";
            }
            debug += "\n";
        }

        debug += "New:\n";

        for (int z = 0; z < 3; z++)
        {
            if (z != 0)
                debug += " -----\t| -----\t| -----\t\n";
            for (int x = 0; x < 3; x++)
            {
                debug += " " + chunks[x, z].x + ", " + chunks[x, z].z + "\t|";
            }
            debug += "\n";
        }

        // Debug.Log(debug);

        int curChunkX = worldMap[layer].layerMap[1, 1].x;
        int curChunkZ = worldMap[layer].layerMap[1, 1].z;

        int newChunkX = chunks[1, 1].x;
        int newChunkZ = chunks[1, 1].z;

        int difX = newChunkX - curChunkX;
        int difZ = newChunkZ - curChunkZ;

        int chunksRow = 3;

        // No Shift 
        if (difX == 0 && difZ == 0)
        {
            //Debug.Log("No shift");
            
        }        

        // Shift right
        if (difX == 1)
        {
            Debug.Log("Move right");
            for (int z = 0; z < chunksRow; z++)
            {
                worldMap[layer].layerMap[chunksRow - 1, z].Clear();
                for (int x = 2; x > 0; x--)
                {
                    worldMap[layer].layerMap[x, z] = worldMap[layer].layerMap[x - 1, z];
                }

                worldMap[layer].layerMap[0, z] = new WorldChunk(layer, chunks[0, z]);
            }
        }

        if (difX == -1)
        {
            Debug.Log("Move left");
            for (int z = 0; z < chunksRow; z++)
            {
                worldMap[layer].layerMap[0, z].Clear();

                for (int x = 0; x < chunksRow - 1; x++)
                {
                    worldMap[layer].layerMap[x, z] = worldMap[layer].layerMap[x + 1, z];
                }

                worldMap[layer].layerMap[chunksRow - 1, z] = new WorldChunk(layer, chunks[chunksRow - 1, z]);
            }
        }

        // Shift up
        if (difZ == 1)
        {
            Debug.Log("Move up");
            for (int x = 0; x < chunksRow; x++)
            {
                worldMap[layer].layerMap[x, chunksRow - 1].Clear();
                for (int z = chunksRow - 1; z > 0; z--)
                {
                    worldMap[layer].layerMap[x, z] = worldMap[layer].layerMap[x, z - 1];
                }

                worldMap[layer].layerMap[x, 0] = new WorldChunk(layer, chunks[x, 0]);
            }
        }

        // Shift down
        if (difZ == -1)
        {
            Debug.Log("Move down");
            for (int x = 0; x < chunksRow; x++)
            {
                worldMap[layer].layerMap[x, 0].Clear();

                for (int z = 0; z < chunksRow - 1; z++)
                {
                    worldMap[layer].layerMap[x, z] = worldMap[layer].layerMap[x, z + 1];
                }

                worldMap[layer].layerMap[x, chunksRow - 1] = new WorldChunk(layer, chunks[x, chunksRow - 1]);
            }
        }

        // Shift is too big
        if (Mathf.Abs(difX) > 1 || Mathf.Abs(difZ) > 1)
        {
            Debug.Log("Completely redrawing chunks");
            for (int z = 0; z < chunksRow; z++)
            {
                for (int x = 0; x < chunksRow; x++)
                {
                    worldMap[layer].layerMap[x, z].Clear();
                    worldMap[layer].layerMap[x, z] = new WorldChunk(layer, chunks[x, z]);
                }
            }
        }
        else
        {
            for (int z = 0; z < chunksRow; z++)
            {
                for (int x = 0; x < chunksRow; x++)
                {
                    worldMap[layer].layerMap[x, z].Update(layer, chunks[x, z]);
                }
            }
        }


        List<string> keys = new List<string>(GlobalInstantiatedObjects.Keys);

        foreach (string key in keys)
        {
            bool exist = false;
            for (int z = 0; z < chunksRow; z++)
            {
                for (int x = 0; x < chunksRow; x++)
                {
                    if(worldMap[layer].layerMap[x, z].instantiatedObjects.ContainsKey(key))
                    {
                        exist = true;
                    }
                }
            }

            if(!exist)
            {
                Destroy(GlobalInstantiatedObjects[key].gameObject);
                GlobalInstantiatedObjects.Remove(key);
            }
        }
    }
}
