using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBlock : Entity
{
    static GameObject prefab;

    public static Dictionary<WorldMap.BlockType, GameObject> loadedBlocks = new Dictionary<WorldMap.BlockType, GameObject>();

    GameObject blockInstance;
    WorldMap.BlockType curBlockType;

    public static EntityBlock GetPrefab()
    {
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>("Prefabs/EntityBlock");
        }

        GameObject go = Instantiate(prefab);

        return go.GetComponent<EntityBlock>();
    }

    public static GameObject LoadBlock(WorldMap.BlockType blockType)
    {
        GameObject block;

        if(!loadedBlocks.TryGetValue(blockType, out block))
        {
            block = Resources.Load<GameObject>("Prefabs/Blocks/" + blockType.ToString());
            loadedBlocks.Add(blockType, block);
        }

        block.name = blockType.ToString();

        return Instantiate(block);
    }

    public override GameObject CreateIt(int layer, EntityModel entityModel)
    {
        EntityModel_Block block = (EntityModel_Block)entityModel;

        transform.position = block.WorldPosition(layer);

        name = "Block " + entityModel.id;

        GameObject blockGO = LoadBlock((WorldMap.BlockType)block.blockType);
        blockGO.transform.SetParent(transform);
        blockGO.transform.localPosition = Vector3.zero;

        return gameObject;
    }

    public override GameObject UpdateIt(int layer, EntityModel entityModel)
    {
        EntityModel_Block block = (EntityModel_Block)entityModel;

        transform.position = block.WorldPosition(layer);

        if(curBlockType != block.blockType)
        {
            Destroy(blockInstance);
            blockInstance = LoadBlock(block.blockType);
        }

        return gameObject;
    }
}
