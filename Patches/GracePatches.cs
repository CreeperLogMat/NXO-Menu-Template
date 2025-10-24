using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Patches
{
    [HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin), "GracePeriod")]
    public class AntiGracePeriod1
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2), "GracePeriod")]
    public class AntiGracePeriod2
    {
        private static bool Prefix() => false;
    }
}
