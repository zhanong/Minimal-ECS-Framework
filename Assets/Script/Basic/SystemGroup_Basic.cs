/* 
    This file is for setting the update order of the basic system
*/

using Unity.Entities;
using Unity.Burst;

namespace ECSFramework
{
    public partial class GBasicSystems : ComponentSystemGroup { }
    [UpdateInGroup(typeof(GBasicSystems))] public partial class SConfig : SystemBase { } [UpdateAfter(typeof(SConfig))]
    [UpdateInGroup(typeof(GBasicSystems))] public partial class SLoadScene : SystemBase { } [UpdateAfter(typeof(SLoadScene))]
    [UpdateInGroup(typeof(GBasicSystems))] public partial class SMessenger : SystemBase { } //[UpdateAfter(typeof(SMessenger))]    
}
