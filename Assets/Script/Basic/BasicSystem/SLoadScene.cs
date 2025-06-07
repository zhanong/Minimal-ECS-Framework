/*
    This system is responsible for loading subscenes when a load scene request is sent.
*/

using Unity.Entities;
using Unity.Scenes;

namespace ECSFramework
{
    public partial class SLoadScene : SystemBase
    {
        Entity sceneLoadedFlag;

        Entity currentScene;
        SceneID currentSceneID;
        bool onQuestLoadScene;

        bool loading;

        protected override void OnCreate()
        {
            sceneLoadedFlag = EntityManager.CreateEntity();
            BasicEventManager.singleton.questLoadScene += OnQuestLoadScene;
        }

        void OnQuestLoadScene(SceneID sceneID)
        {
            onQuestLoadScene = true;
            currentSceneID = sceneID;
        }

        protected override void OnUpdate()
        {
            // load new subscene if requested
            if (onQuestLoadScene)
            {
                if (currentScene != Entity.Null)
                {
                    UnloadCurrent();
                    currentScene = Entity.Null;
                }

                LoadScene(currentSceneID);

                onQuestLoadScene = false;
                loading = true;

                // remove this flag to stop `SInitialize` system from updating
                EntityManager.RemoveComponent<SceneLoaded>(sceneLoadedFlag);
            }

            // check if scene has been loaded
            if (loading)
            {
                if (SceneSystem.IsSceneLoaded(World.Unmanaged, currentScene))
                {
                    SystemAPI.SetComponent(sceneLoadedFlag, new SceneLoaded { sceneID = currentSceneID });
                    loading = false;

                    // kick off `SInitialize` system to update
                    EntityManager.AddComponent<SceneLoaded>(sceneLoadedFlag);
                }
            }
        }

        void LoadScene(SceneID sceneID)
        {
            var sceneAsset = SceneLoadManager.GetSceneAsset(sceneID);
            currentScene = SceneSystem.LoadSceneAsync(World.Unmanaged, sceneAsset.entitySceneReference);
        }

        void UnloadCurrent()
        {
            SceneSystem.UnloadScene(World.Unmanaged, currentScene, SceneSystem.UnloadParameters.DestroyMetaEntities);
            EntityManager.DestroyEntity(currentScene);

            // All entities created under a subscene will be destroy when the subscene is unloaded
            // For entities that are not created under that subscene, for example, entities created from a prefab, 
            // you would need to add a `DestroyOnSceneUnload` component to them to destroy them when the scene is unloaded.
            Entities
            .WithAll<DestroyOnSceneUnload>() 
            .WithNone<Prefab>()
            .WithImmediatePlayback()
            .ForEach((Entity entity, EntityCommandBuffer ecb) =>
            {
                ecb.DestroyEntity(entity);
            }).Run();
        }
    }

    public struct SceneLoaded : IComponentData
    {
        public SceneID sceneID;
    }
    public struct DestroyOnSceneUnload : IComponentData { }

}