/* Organize your custom systems here */

using Unity.Entities;

namespace ECSFramework
{
    [UpdateAfter(typeof(GBasicSystems))]

    /* UPDATE ON CHANGE */
    public partial class GUpdateOnChange : ComponentSystemGroup { }

    [UpdateAfter(typeof(GUpdateOnChange))]

    /* REGULAR UPDATE */
    public partial class GRegularUpdate : ComponentSystemGroup { }

    [UpdateAfter(typeof(GRegularUpdate))]

    /* HANDLE USER INPUT */
    public partial class GHandleUserInput : ComponentSystemGroup { }

    [UpdateAfter(typeof(GHandleUserInput))]

    /* CREATE/DESTORY ENTITY */
    public partial class GCreateDestroyEntities : ComponentSystemGroup { }

    //[UpdateAfter(typeof(GHandleUserInput))]
}
