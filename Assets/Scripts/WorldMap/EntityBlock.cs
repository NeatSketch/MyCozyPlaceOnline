using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBlock : Entity
{
    static GameObject prefab;

    public static Dictionary<WorldMap.BlockType, GameObject> loadedBlocks = new Dictionary<WorldMap.BlockType, GameObject>();
 
    public static EntityBlock GetPrefab()
    {
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>("Prefabs/EntityBlock");
        }

        return prefab.GetComponent<EntityBlock>();
    }

    public static GameObject LoadBlock(WorldMap.BlockType blockType)
    {
        GameObject block;

        if(!loadedBlocks.TryGetValue(blockType, out block))
        {
            block = Resources.Load<GameObject>("Prefabs/Blocks/" + blockType.ToString());
            loadedBlocks.Add(blockType, block);
        }

        return Instantiate(block);
    }

    public override GameObject CreateIt(int layer, EntityModel entityModel)
    {
        EntityModel_Block block = (EntityModel_Block)entityModel;

        transform.position = block.WorldPosition(layer);
        name = "Block " + entityModel.id;

        return gameObject;
    }

    public override GameObject UpdateIt(int layer, EntityModel entityModel)
    {
        EntityModel_Block block = (EntityModel_Block)entityModel;

        transform.position = block.WorldPosition(layer);

        return gameObject;
    }
}
