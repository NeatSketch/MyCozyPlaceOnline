using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFurniture : Entity
{
    static GameObject prefab;

    public static EntityFurniture GetPrefab()
    {
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>("/Prefabs/EntityPlayer");
        }

        return prefab.GetComponent<EntityFurniture>();
    }

    public override GameObject CreateIt(int layer, EntityModel entityModel)
    {
        EntityModel_Furniture furniture = (EntityModel_Furniture)entityModel;

        transform.position = entityModel.WorldPosition(layer);
        name = "Furniture " + entityModel.id;

        return gameObject;
    }

    public override GameObject UpdateIt(int layer, EntityModel entityModel)
    {
        return gameObject;
    }
}
