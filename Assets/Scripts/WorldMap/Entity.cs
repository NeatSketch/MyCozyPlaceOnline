using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public static Entity CreateEntity(int layer, EntityModel entityModel)
    {
        switch(entityModel.GetType().ToString())
        {
            case "EntityModel_Block":
                {
                    EntityBlock entity = EntityBlock.GetPrefab();
                    entity.CreateIt(layer, entityModel);
                    return entity;
                }
            case "EntityModel_Furniture":
                {
                    EntityFurniture entity = EntityFurniture.GetPrefab();
                    entity.CreateIt(layer, entityModel);
                    return entity;
                }
            case "EntityModel_Player":
                {
                    EntityPlayer entity = EntityPlayer.GetPrefab();
                    entity.CreateIt(layer, entityModel);
                    return entity;
                }
        }

        return null;
    }

    public static void UpdateEntity(int layer, EntityModel entityModel, Entity entity)
    {
        switch (entityModel.GetType().ToString())
        {
            case "EntityModel_Block":
                {
                    entity.UpdateIt(layer, entityModel);
                    break;
                }
            case "EntityModel_Furniture":
                {
                    entity.UpdateIt(layer, entityModel);
                    break;
                }
            case "EntityModel_Player":
                {
                    entity.UpdateIt(layer, entityModel);
                    break;
                }
        }
    }

    public abstract GameObject CreateIt(int layer, EntityModel entityModel);
    public abstract GameObject UpdateIt(int layer, EntityModel entityModel);
}
