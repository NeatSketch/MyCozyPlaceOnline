using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public static GameObject CreateEntity(int layer, EntityModel entityModel)
    {
        switch(entityModel.GetType().ToString())
        {
            case "EntityModel_Block":
                {
                    EntityBlock entityGO = EntityBlock.GetPrefab();
                    entityGO.CreateIt(layer, entityModel);
                    return entityGO.gameObject;
                }
            case "EntityModel_Furniture":
                {
                    EntityFurniture entityGO = EntityFurniture.GetPrefab();
                    entityGO.CreateIt(layer, entityModel);
                    return entityGO.gameObject;
                }
            case "EntityModel_Player":
                {
                    EntityPlayer entityGO = EntityPlayer.GetPrefab();
                    entityGO.CreateIt(layer, entityModel);
                    return entityGO.gameObject;
                }
        }

        return null;
    }

    public static GameObject UpdateEntity(int layer, EntityModel entityModel)
    {
        switch (entityModel.GetType().ToString())
        {
            case "EntityModel_Block":
                {
                    EntityBlock entityGO = EntityBlock.GetPrefab();
                    entityGO.UpdateIt(layer, entityModel);
                    return entityGO.gameObject;
                }
            case "EntityModel_Furniture":
                {
                    EntityFurniture entityGO = EntityFurniture.GetPrefab();
                    entityGO.UpdateIt(layer, entityModel);
                    return entityGO.gameObject;
                }
            case "EntityModel_Player":
                {
                    EntityPlayer entityGO = EntityPlayer.GetPrefab();
                    entityGO.UpdateIt(layer, entityModel);
                    return entityGO.gameObject;
                }
        }

        return null;
    }

    public abstract GameObject CreateIt(int layer, EntityModel entityModel);
    public abstract GameObject UpdateIt(int layer, EntityModel entityModel);
}
