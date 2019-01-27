using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer : Entity
{
    static GameObject prefab;

    Character character;

    public static EntityPlayer GetPrefab()
    {
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>("Prefabs/EntityPlayer");
        }

        GameObject go = Instantiate(prefab);
            
        return go.GetComponent<EntityPlayer>();
    }

    public override GameObject CreateIt(int layer, EntityModel entityModel)
    {
        EntityModel_Player player = (EntityModel_Player)entityModel;

        transform.position = entityModel.WorldPosition(layer);
        name = "Player " + player.nickname + " " + player.id;

        character = GetComponent<Character>();

        return gameObject;
    }

    public override GameObject UpdateIt(int layer, EntityModel entityModel)
    {
        EntityModel_Player player = (EntityModel_Player)entityModel;

        character.MoveTo(entityModel.WorldPosition(layer));

        return gameObject;
    }
}
