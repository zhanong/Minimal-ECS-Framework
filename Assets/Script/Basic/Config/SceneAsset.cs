/*

This class stores the data that needs to be loaded for the scene.

*/

using Unity.Entities.Serialization;
using UnityEngine;

namespace ECSFramework
{
    [CreateAssetMenu(fileName = "SceneAsset", menuName = "Custom/SceneAsset")]
    public class SceneAsset : ScriptableObject
    {
        public SceneID sceneID;
        public GameObject sceneObject;
        public EntitySceneReference entitySceneReference;
        public bool isLevelScene;
    }
}
