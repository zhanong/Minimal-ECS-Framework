using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace ECSFramework
{

    public class SceneLoadManager : MonoBehaviour
    {
        public static SceneLoadManager singleton;

        [SerializeField] SceneID startScene;

        // scene asset
        [SerializeField] string sceneAssetLoadPath = "Scene Assets";
        SceneAsset[] sceneAssets;
        SceneConfig sceneConfig;

        // current
        public SceneID currentScene;
        GameObject currentSceneObject;

        public static SceneAsset GetSceneAsset(SceneID sceneID) => singleton.sceneAssets[(int)sceneID - 1];

        void Awake()
        {
            if (singleton == null)
                singleton = this;
            else
                Destroy(this);

            LoadSceneAsset();
        }

        IEnumerator Start()
        {
            while (MonoMSN.singleton == null || !MonoMSN.singleton.basicEventRegistered)
            {
                yield return null;
            }

            BasicEventManager.singleton.questLoadScene += OnQuestLoadScene;
            BasicEventManager.singleton.questLoadScene(startScene);
        }

        void LoadSceneAsset()
        {
            sceneAssets = Resources.LoadAll<SceneAsset>(sceneAssetLoadPath)
            .OrderBy(asset => asset.sceneID).ToArray();

            // Check load number
            if (sceneAssets.Length == (int)SceneID.Count - 1)
                Debug.Log("[SceneManager] Loaded " + sceneAssets.Length + " scene assets.");
            else
                throw new Exception("[SceneManager] Loaded " + sceneAssets.Length + " scene assets. Should have loaded " + ((int)SceneID.Count - 1) + " instead.");
        }

        void OnQuestLoadScene(SceneID sceneID)
        {
            Debug.Log("[SceneManager] Load scene " + sceneID);

            // destroy old scene
            if (currentSceneObject != null)
                DestroyImmediate(currentSceneObject);

            // create new scene
            var sceneObject = GetSceneAsset(sceneID).sceneObject;
            currentSceneObject = Instantiate(sceneObject);

            currentScene = sceneID;
        }
    }
}
