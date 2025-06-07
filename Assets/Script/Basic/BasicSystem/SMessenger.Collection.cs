using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using System;
using Unity.Mathematics;
using System.Runtime.InteropServices;
using ZhTool;




namespace ECSFramework
{
    /// <summary>
    /// This is a part of SInitialize System. It handles centralized native collections.
    /// </summary>
    public partial class SMessenger : SystemBase
    {
        enum State { OnInitialize, OnNewScene, OnDestroy }


        void BatchProcessCollection(State state, ref EntityCommandBuffer ecb, in SceneConfig sceneConfig, in SceneID sceneID)
        {
            // !!! Register collection components here after creating !!!
            GeneralCollection<DestroyQuests>(state, ref ecb);
            SceneRelatedCollection<ExampleSceneRelatedNativeCollectionComponent>(state, ref ecb, in sceneConfig, sceneID);
        }

        void GeneralCollection<T>(State state, ref EntityCommandBuffer ecb) where T : unmanaged, IGeneralNativeCollectionComponent
        {
            switch (state)
            {
                case State.OnInitialize:
                    ecb.AddComponent<T>(msn);
                    break;
                case State.OnNewScene:
                    var component = EntityManager.GetComponentData<T>(msn);
                    component.Reset();
                    EntityManager.SetComponentData(msn, component);
                    break;
                case State.OnDestroy:
                    component = EntityManager.GetComponentData<T>(msn);
                    component.Dispose();
                    break;
            }
        }

        void SceneRelatedCollection<T>(State state, ref EntityCommandBuffer ecb, in SceneConfig sceneConfig, in SceneID sceneID)
        where T : unmanaged, ISceneRelatedNativeCollectionComponent
        {
            switch (state)
            {
                case State.OnInitialize:
                    ecb.AddComponent<T>(msn);
                    break;
                case State.OnNewScene:
                    var component = EntityManager.GetComponentData<T>(msn);
                    component.Reset(sceneConfig, sceneID);
                    EntityManager.SetComponentData(msn, component);
                    break;
                case State.OnDestroy:
                    component = EntityManager.GetComponentData<T>(msn);
                    component.Dispose();
                    break;
            }
        }

        protected override void OnDestroy()
        {
            Debug.Log("*** central data destroy ***");

            var ecb = new EntityCommandBuffer();
            BatchProcessCollection(State.OnDestroy, ref ecb, default, default);

        }
    }

    #region collection interface
    public interface IGeneralNativeCollectionComponent : IComponentData
    {
        public bool IsCreated { get; set; }
        public void OnDispose();
        public void OnCreate();
        public void Clear();

    }

    public static class IGeneralNativeCollectionComponentExtension
    {
        public static void Reset<T>(ref this T component) where T : unmanaged, IGeneralNativeCollectionComponent
        {
            if (component.IsCreated)
                component.Clear();
            else
            {
                component.OnCreate();
                component.IsCreated = true;
            }
        }
        public static void Dispose<T>(ref this T component) where T : unmanaged, IGeneralNativeCollectionComponent
        {
            if (component.IsCreated)
                component.OnDispose();
        }
    }

    public interface ISceneRelatedNativeCollectionComponent : IComponentData
    {
        public void OnDispose();
        public void OnCreate(in SceneConfig sceneConfig, in SceneID sceneID);
        public bool IsCreated { get; set; }
    }

    public static class ISceneRelatedNativeCollectionComponentExtension
    {
        public static void Reset<T>(ref this T component, in SceneConfig sceneConfig, in SceneID sceneID) where T : unmanaged, ISceneRelatedNativeCollectionComponent
        {
            if (component.IsCreated)
                component.OnDispose();
            component.OnCreate(in sceneConfig, in sceneID);
            component.IsCreated = true;
        }
        public static void Dispose<T>(ref this T component) where T : unmanaged, ISceneRelatedNativeCollectionComponent
        {
            if (component.IsCreated)
                component.OnDispose();
        }
    }
    #endregion




    #region DestroyQuests
    /// <summary>
    /// This component serve as a messenger to deliver all entity destroy requests to the SDestroy System.
    /// </summary>
    public partial struct DestroyQuests : IGeneralNativeCollectionComponent
    {
        public NativeQueue<Entity> quest;
        bool _isCreated;
        public bool IsCreated
        {
            get => _isCreated;
            set => _isCreated = value;
        }

        public void Clear()
        {
            quest.Clear();
        }

        public void OnCreate()
        {
            quest = new(Allocator.Persistent);
        }

        public void OnDispose()
        {
            if (_isCreated)
            {
                quest.Dispose();
                _isCreated = false;
            }
        }
    }
    #endregion

    #region Example Scene Related Native Collection Component
    /// <summary>
    /// This is a example of a scene related native collection component. Delete it if you don't need it.
    /// </summary>
    public partial struct ExampleSceneRelatedNativeCollectionComponent : ISceneRelatedNativeCollectionComponent
    {
        public NativeArray<Entity> myArray;

        bool _isCreated;
        public bool IsCreated
        {
            get => _isCreated;
            set => _isCreated = value;
        }

        public void OnCreate(in SceneConfig sceneConfig, in SceneID sceneID)
        {
            int arrayLengthFromConfig = 0; // Let's assume you get the length from the sceneConfig
            myArray = new(arrayLengthFromConfig, Allocator.Persistent);
        }

        public void OnDispose()
        {
            myArray.Dispose();
        }
    }
    #endregion
}
