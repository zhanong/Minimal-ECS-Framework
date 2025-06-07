using System;
using UnityEngine;
using ZhTool;

namespace ECSFramework
{
    public enum SceneID : byte { None, MainMenu, Level1, Count }

    [Flags]
    public enum BuildingTriggerChangeType : byte
    {
        None = 0,
        Structure = 1 << 0,
        LightBlock = 1 << 1,
        RobberyTarget = 1 << 2,
        Last = 1 << 4
    }
}