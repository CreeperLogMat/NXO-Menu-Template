using HarmonyLib;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Patches
{
    [HarmonyPatch(typeof(PlayFabDeviceUtil), "SendDeviceInfoToPlayFab")]
    public class PlayfabPatch1
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(PlayFabClientInstanceAPI), "ReportDeviceInfo")]
    public class PlayfabPatch2
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(PlayFabClientAPI), "ReportDeviceInfo")]
    public class PlayfabPatch3
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(PlayFabDeviceUtil), "GetAdvertIdFromUnity")]
    public class PlayfabPatch4
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(PlayFabClientAPI), "AttributeInstall")]
    public class PlayfabPatch5
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(PlayFabHttp), "InitializeScreenTimeTracker")]
    public class PlayfabPatch6
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(PlayFabClientInstanceAPI), "ReportPlayer", MethodType.Normal)]
    public class NoPlayfabReportPlayer
    {
        static bool Prefix(ReportPlayerClientRequest request, Action<ReportPlayerClientResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabClientAPI), "ReportPlayer", MethodType.Normal)]
    public class NoPlayfabReportPlayer2
    {
        private static bool Prefix(ReportPlayerClientRequest request, Action<ReportPlayerClientResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            return false;
        }
    }
}
