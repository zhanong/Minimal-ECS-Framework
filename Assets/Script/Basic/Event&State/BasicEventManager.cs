using ECSFramework;
using UnityEngine;
using UnityEngine.Events;


public class BasicEventManager
{
    public static BasicEventManager singleton;
    public UnityAction<SceneID> questLoadScene;
    public UnityAction onEventManagerCreated;
}
