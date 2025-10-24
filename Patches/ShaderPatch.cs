using HarmonyLib;
using UnityEngine;
using static NXO.Utilities.Variables;
using static NXO.Utilities.ColorLib;

namespace NXO.Patches
{
    [HarmonyPatch(typeof(GameObject), "CreatePrimitive", MethodType.Normal)]
    public class ShaderPatch
    {
        public static void Postfix(GameObject __result)
        {
            if (__result.GetComponent<Renderer>() != null)
            {
                __result.GetComponent<Renderer>().material.shader = uberShader;
                __result.GetComponent<Renderer>().material.color = Blue;
            }
        }
    }
}
