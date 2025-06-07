/*
    MSN are centralized components that are used to pass common game information among systems. 
    All MSN components are:
        - attached to a single entity called `msn` (Messenger);
        - managed by SInitialize system regrading adding and resetting on new scene;
    After creating a new MSN component, you should register it in the `SInitialize` system.
*/
using Unity.Entities;

namespace ECSFramework
{
    public struct EcbMSN : IComponentData
    {
        public EntityCommandBuffer value;
    }

    public struct InputMSN : IComponentData
    {
        public bool mouse0Down, mouse0Up, mouse1Down, mouse1Up, mouse2Down, mouse2Up;
    }

    /// <summary>
    /// This is an example of ChangeMSN component. Make your own if needed.
    /// </summary>
    public struct ChangeMSN : IComponentData
    {
        bool isAnythingChanged;

        public void Reset()
        {
            isAnythingChanged = false;
        }
    }
}
