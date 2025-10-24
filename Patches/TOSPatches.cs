using HarmonyLib;
using System.Threading.Tasks;

namespace NXO.Patches
{
    [HarmonyPatch(typeof(LegalAgreements), "Update")]
    public class TOSPatch1
    {
        public static bool enabled = true;

        private static bool Prefix(LegalAgreements __instance)
        {
            if (!enabled) return true;

            var scrollSpeedField = AccessTools.Field(typeof(LegalAgreements), "scrollSpeed");
            var maxScrollSpeedField = AccessTools.Field(typeof(LegalAgreements), "_maxScrollSpeed");

            scrollSpeedField?.SetValue(__instance, 10f);
            maxScrollSpeedField?.SetValue(__instance, 10f);

            return false;
        }
    }

    [HarmonyPatch(typeof(ModIOTermsOfUse_v1), "PostUpdate")]
    public class TOSPatch2
    {
        private static bool Prefix(ModIOTermsOfUse_v1 __instance)
        {
            if (!TOSPatch1.enabled) return true;

            __instance.TurnPage(999);

            var holdTimeField = AccessTools.Field(typeof(ModIOTermsOfUse_v1), "holdTime");
            holdTimeField?.SetValue(__instance, 0.1f);

            return false;
        }
    }

    [HarmonyPatch(typeof(AgeSlider), "PostUpdate")]
    public class TOSPatch3
    {
        private static bool Prefix(AgeSlider __instance)
        {
            if (!TOSPatch1.enabled) return true;

            var holdTimeField = AccessTools.Field(typeof(AgeSlider), "holdTime");
            holdTimeField?.SetValue(__instance, 0.1f);

            return false;
        }
    }

    [HarmonyPatch(typeof(PrivateUIRoom), "StartOverlay")]
    public class TOSPatch4
    {
        private static bool Prefix() => !TOSPatch1.enabled;
    }

    [HarmonyPatch(typeof(KIDManager), "UseKID")]
    public class TOSPatch5
    {
        private static bool Prefix(ref Task<bool> __result)
        {
            if (!TOSPatch1.enabled) return true;

            __result = Task.FromResult(false);
            return false;
        }
    }
}
