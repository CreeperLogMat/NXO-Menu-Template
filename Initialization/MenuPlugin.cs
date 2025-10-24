using BepInEx;
using HarmonyLib;
using System;
using static NXO.Initialization.PluginInfo;

namespace NXO.Initialization
{
    [BepInPlugin(menuGUID, menuName, menuVersion)]
    public class BepInExInitializer : BaseUnityPlugin
    {
        void Awake() 
        {
            try
            {
                var harmony = new Harmony(menuGUID);
                harmony.PatchAll();
                UnityEngine.Debug.Log($"{menuName} {menuVersion} initialized successfully.");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log($"Failed to initialize {menuName}: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
