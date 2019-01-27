using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBlock : Entity
{
    static GameObject prefab;    

    public static EntityBlock GetPrefab()
    {
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>("Prefabs/EntityBlock");
        }

        return prefab.GetComponent<EntityBlock>();
    }

    public override GameObject CreateIt(int layer, EntityModel entityModel)
    {
        EntityModel_Block block = (EntityModel_Block)entityModel;

        transform.position = entityModel.WorldPosition(layer);
        name = "Block " + entityModel.id;

        return gameObject;
    }

    public override GameObject UpdateIt(int layer, EntityModel entityModel)
    {
        return gameObject;
    }
}
