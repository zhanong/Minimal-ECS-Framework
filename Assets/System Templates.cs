#region SystemBase

// using System.Collections;
// using System.Collections.Generic;
// using Unity.Entities;
// using Unity.Mathematics;
// using UnityEngine;

// namespace XXX
// {
//     public partial class XXXX : SystemBase
//     {
//         protected override void OnCreate()
//         {
//             // This system will not update until the scene transition is complete
//             RequireForUpdate<InitCompleted>();
//         }

//         protected override void OnUpdate()
//         {
//             // Check if the system needs to reset its state for a new scene
//             var msn = SystemAPI.GetSingletonEntity<SystemResetMSN>();
//             if (SystemAPI.IsComponentEnabled<SystemResetMSN>(msn))
//             {
//                 // reset your system if needed 
//             }

//             // Your main logic goes here
//         }
//     }
// }

#endregion SystemBase


#region ISystem

// using System.Collections;
// using System.Collections.Generic;
// using Unity.Burst;
// using Unity.Entities;
// using Unity.Mathematics;
// using UnityEngine;

// namespace XXX
// {
//     public partial struct XXXX : ISystem
//     {
//         [BurstCompile]
//         void OnCreate(ref SystemState state)
//         {
//             // This system will not update until the scene transition is complete
//             state.RequireForUpdate<InitCompleted>();
//         }

//         [BurstCompile]
//         void OnUpdate(ref SystemState state)
//         {
//             // Check if the system needs to reset its state for a new scene
//             var msn = SystemAPI.GetSingletonEntity<SystemResetMSN>();
//             if (SystemAPI.IsComponentEnabled<SystemResetMSN>(msn))
//             {
//                 // reset your system if needed 
//             }
            
//             // Your main logic goes here
//         }

//     }
// }

#endregion ISystem