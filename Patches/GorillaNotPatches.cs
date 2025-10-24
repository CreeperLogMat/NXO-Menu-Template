using HarmonyLib;
using NXO.Utilities;
using Photon.Pun;

namespace NXO.Patches
{
    [HarmonyPatch(typeof(GorillaNot), "SendReport")]
    public static class NoSendReport
    {
        public static bool AntiCheatNotifications = false;

        private static bool Prefix(string susReason, string susId, string susNick)
        {
            if (AntiCheatNotifications)
            {
                if (susId == PhotonNetwork.LocalPlayer.UserId)
                {
                    NotificationLib.SendNotification("<color=blue>Anti-Cheat</color> : Reason: " + susReason);
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "CheckReports")]
    public class NoCheckReports
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(GorillaNot), "LogErrorCount")]
    public class NoLogErrorCount
    {
        private static bool Prefix(string logString, string stackTrace, UnityEngine.LogType type) => false;
    }

    [HarmonyPatch(typeof(GorillaNot), "DispatchReport")]
    public class NoDispatchReport
    {
        private static bool Prefix() => false;
    }
}
