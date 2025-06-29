/*
    This class is responsible for holding game state and passing information from ECS to MonoBehaviours
    For example, the EntityManager and the msn entity are stored here.
*/

using System.Collections.Generic;
using ECSFramework;
using Unity.Entities;

public class MonoMSN : IBasicManager
{
    public static MonoMSN singleton;
    public bool basicEventRegistered;
    public EntityManager entityManager;

    public Dictionary<EntityQueryDesc, EntityQuery> queryMap;

    public string Stamp { get; set; }

    public void Initialize()
    {
        singleton = this;
        queryMap = new(20);
    }

    public void OnNewScene()
    {

    }

    IBasicManager IBasicManager.OnNewScene()
    {
        return this;
    }

    public void OnDestroy()
    {
        singleton = null;
    }

    public int GetQueryNumber(EntityQueryDesc desc)
    {
        if (!queryMap.ContainsKey(desc))
        {
            queryMap.Add(desc, entityManager.CreateEntityQuery(desc));
        }

        return queryMap[desc].CalculateEntityCount();
    }
}
