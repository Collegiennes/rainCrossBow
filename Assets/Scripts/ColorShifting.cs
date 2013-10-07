using System;
using System.Collections.Generic;
using UnityEngine;

class ColorShifting : MonoBehaviour
{
    public static Dictionary<string, Material> Materials;

    public static Dictionary<Type, string> EnemyMaterials = new Dictionary<Type, string>
    {
        { typeof(CarrierBehaviour), "clr_Yellow" },
//        { typeof(BombBehaviour), "clr_Red" },
        { typeof(DrillerBehaviour), "clr_Blurple" },
        { typeof(WallBehaviour), "clr_Cyan" },
        { typeof(ClockBehaviour), "clr_GreenFluo" },
        { typeof(SingleShotBehaviour), "clr_Green" },
        { typeof(EverynianBehaviour), "clr_Orange" },
        //{ "wall", "clr_Blue" },
    };

    static string[] ShitfingMaterials = new []
    {
        "clr_Shift"
    };

    void FixedUpdate()
    {
        foreach (var name in ShitfingMaterials)
        {
            var m = Materials[name];
            var emissive = m.GetColor("_Emission");
            float h, s, v;
            ColorHelper.ColorToHSV(emissive, out h, out s, out v);
            h += 20;
            m.SetColor("_Emission", ColorHelper.ColorFromHSV(h, s, v));
        }
    }
}
