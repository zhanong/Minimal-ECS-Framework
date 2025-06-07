using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using ZhTool.Entities;

namespace ECSFramework
{
    /// <summary>
    /// This system is used to store blob configs in the `configDic` for monobehaviour to access.
    /// you can also load managed configs in this system and store them in the same dictionary.
    /// </summary>
    public partial class SConfig : SystemBase
    {
        public static bool configLoaded = false;
        static Dictionary<Type, IComponentData> configDic;
        bool inUpdate;
        Entity configLoadedFlag;

        protected override void OnCreate()
        {
            RequireOrAddBlobConfigs();
        }

        protected override void OnDestroy()
        {
            configDic.Clear();
            configDic = null;
            configLoaded = false;
        }

        protected override void OnUpdate()
        {
            inUpdate = true;
            Enabled = false;

            configDic = new(10);
            RequireOrAddBlobConfigs();
            AddManagedConfigs();

            configLoaded = true;
            configLoadedFlag = EntityManager.CreateEntity(typeof(ConfigLoadedFlag));
        }

        #region IComponentData
        // Add Config Type here
        void RequireOrAddBlobConfigs()
        {
            // !!! Add blob configs here !!!
            RequireOrAddBlobConfig<SceneConfig>(true);
        }

        /// <summary>
        /// copy the blob ref to configDic.
        /// </summary>
        void RequireOrAddBlobConfig<T>(bool addToDic) where T : unmanaged, IComponentData
        {
            if (!inUpdate)
            {
                RequireForUpdate<T>();
                return;
            }

            if (addToDic)
            {
                var configComponent = SystemAPI.GetSingleton<T>();
                configDic.Add(typeof(T), configComponent);
            }
        }

        public static T GetConfig<T>() where T : IComponentData
        {
            if (configDic.TryGetValue(typeof(T), out IComponentData config))
            {
                return (T)config;
            }
            throw new Exception($"[SConfig] Config {typeof(T)} not found");
        }
        #endregion

        #region mono
        void AddManagedConfigs()
        {
            // !!! Add managed configs here !!!
            
            // AddManagedConfig<MyConfig, MyConfigAsset, MyConfigEnum>("Path/To/MyConfig");
        }

        public void AddManagedConfig<T, TAsset, TEnum>(string path)
        where T : ManagedConfig<TAsset, TEnum>, IComponentData, new()
        where TAsset : ConfigAsset<TEnum>
        where TEnum : Enum
        {
            T component = new();
            component.LoadConfig(path);
            configDic.Add(typeof(T), component);
        }

        #endregion
    }
    public struct ConfigLoadedFlag : IComponentData { }

    public abstract class ManagedConfig<TAsset, TEnum> : IComponentData where TAsset : ConfigAsset<TEnum> where TEnum : Enum
    {
        public void LoadConfig(string path)
        {
            var assets = Resources.LoadAll<TAsset>(path).OrderBy(asset => asset.Type).ToArray();
            OnLoadConfig(assets);
        }

        protected abstract void OnLoadConfig(TAsset[] assets);

    }

}