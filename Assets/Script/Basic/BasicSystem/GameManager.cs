/*
    GameManager is responsible for managing basic managers, including initialization, reset on new scene, and destruction.
    Basic managers are used to handle common game logic that are outside of the ECS system. They use singleton pattern for easy access.
    Currently, there are two basic managers: EventManager and MonoMSN. 
    Add more as needed.
*/
using UnityEngine;

namespace ECSFramework
{
    public class GameManager : MonoBehaviour
    {
        public static bool IsInitialized => initialized;
        static IBasicManager[] managers = new IBasicManager[]
        {
            new EventManager(),
            new MonoMSN(),
        };
        static bool initialized;

        public static void Initialize()
        {
            Debug.Log("game manager create");
            BasicEventManager.singleton = new();
            BasicEventManager.singleton.questLoadScene += OnNewScene;

            foreach (var manager in managers)
                manager.Initialize();

            initialized = true;
        }

        static void OnNewScene(SceneID sceneID)
        {
            for (int i = 0; i < managers.Length; i++)
            {
                managers[i] = managers[i].OnNewScene();
                managers[i].Stamp = sceneID.ToString();
            }
        }

        void OnDestroy()
        {
            initialized = false;
            foreach (var manager in managers)
                manager.OnDestroy();
        }
    }

    public interface IBasicManager
    {
        public void Initialize();
        public string Stamp { get; set;}
        public IBasicManager OnNewScene();
        public void OnDestroy();
    }
}
