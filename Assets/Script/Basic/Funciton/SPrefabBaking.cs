using Unity.Entities;
using Unity.Burst;

namespace ECSFramework
{
    public partial struct SPrefabSetup : ISystem
    {
        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            // add components to prefab here
        }

    }
}