using Unity.Burst;
using Unity.Entities;

namespace ECSFramework
{
    public partial struct ResetChangeMSN : ISystem
    {
        Entity msn;

        [BurstCompile]
        void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InitCompleted>();
        }

        [BurstCompile]
        void OnUpdate(ref SystemState state)
        {
            if (msn == Entity.Null)
            {
                msn = SystemAPI.GetSingletonEntity<Messenger>();
                return;
            }

            SystemAPI.SetComponent<ChangeMSN>(msn, new());
        }
    }
}