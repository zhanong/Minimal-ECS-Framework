/*
    This system is responsible for managing the centralized components attached to the msn entity.
    It's behavior includes:
        1. At the very beginning, it creates the msn entity and adds all centralized components to it.
           it also creates native collections for all centralize native collection components and add them to the msn entity.
        2. When a new scene is loaded, it resets all the centralized components and native collections.
        3. When it is destroyed, it disposes all the centralized native collections.
*/
using Unity.Entities;
using Unity.Collections;

namespace ECSFramework
{
    public partial class SMessenger : SystemBase
    {
        Entity msn;
        bool reset_or_add;

        bool onNewScene;
        bool newSceneInitialized;

        Entity initializedFlag;

        protected override void OnCreate()
        {
            // wait for config loaded and scene loaded for update
            RequireForUpdate<ConfigLoadedFlag>();
            RequireForUpdate<SceneLoaded>();

            initializedFlag = EntityManager.CreateEntity();

            // add centralized components that doesn't need to be reset on new scene
            msn = EntityManager.CreateEntity
            (typeof(Messenger), typeof(SystemResetMSN),
            typeof(EcbMSN), typeof(InputMSN));

            // add all centralized native collection components
            EntityCommandBuffer ecb = new(Allocator.Temp);
            BatchProcessCollection(State.OnInitialize, ref ecb, default, default);
            ecb.Playback(EntityManager);

            // initialize all basic mono-behaviour managers
            if (!GameManager.IsInitialized)
                GameManager.Initialize();

            // register event on new scene load
            BasicEventManager.singleton.questLoadScene += OnLoadScene;

            // set up monoMSN
            MonoMSN.singleton.entityManager = EntityManager;
            MonoMSN.singleton.msn = msn;
            MonoMSN.singleton.basicEventRegistered = true;
        }

        void OnLoadScene(SceneID sceneID)
        {
            // stop all systems that require scene initialization completion for update
            EntityManager.RemoveComponent<InitCompleted>(initializedFlag);

            onNewScene = true;
        }

        protected override void OnUpdate()
        {
            // Turn off Reset-System Flag
            if (SystemAPI.HasComponent<SystemResetMSN>(msn))
            {
                SystemAPI.SetComponentEnabled<SystemResetMSN>(msn, false);
            }

            // initialize/reset the msn entity on new scene
            if (onNewScene)
            {
                var ecb = new EntityCommandBuffer(Allocator.Temp);

                // reset centralized components
                AddOrResetComponents(ref ecb);
                ResetComponentEnable(ref ecb);
                reset_or_add = true;

                // reset centralized native collection compoents
                BatchProcessCollection(State.OnNewScene, ref ecb, SystemAPI.GetSingleton<SceneConfig>(), SystemAPI.GetSingleton<SceneLoaded>().sceneID);

                ecb.AddComponent<InitCompleted>(initializedFlag);
                ecb.Playback(EntityManager);

                onNewScene = false;
                newSceneInitialized = true;
                return;
            }

            // turn on the `SystemResetMSN` one frame after the `InitCompleted` component is added to ensure all systems reset properly.
            if (newSceneInitialized)
            {
                newSceneInitialized = false;
                SystemAPI.SetComponentEnabled<SystemResetMSN>(msn, true);
            }
        }

        void AddOrResetComponents(ref EntityCommandBuffer ecb)
        {
            // !!! Register centralized components here !!!
            AddOrResetComponent<InitCompleted>(ref ecb, reset_or_add);
        }

        void AddOrResetComponent<T>(ref EntityCommandBuffer ecb, bool _reset_or_add) where T : unmanaged, IComponentData
        {
            if (!_reset_or_add)
                ecb.AddComponent<T>(msn);
            else
                ecb.SetComponent<T>(msn, new());
        }

        void ResetComponentEnable(ref EntityCommandBuffer ecb)
        {
            // !!! Reset any enablable centralized compoennts here !!!
        }
    }
    
    public struct InitCompleted : IComponentData { }
    public struct Messenger : IComponentData { }
    public struct SystemResetMSN : IComponentData, IEnableableComponent { }
}
