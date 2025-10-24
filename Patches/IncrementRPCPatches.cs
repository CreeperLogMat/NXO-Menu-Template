using HarmonyLib;
using Photon.Pun;
using System;

namespace NXO.Patches
{
    [HarmonyPatch(typeof(VRRig), "IncrementRPC", new Type[] { typeof(PhotonMessageInfoWrapped), typeof(string) })]
    public class AntiIncrementRPC
    {
        private static bool Prefix(PhotonMessageInfoWrapped info, string sourceCall) => false;
    }

    [HarmonyPatch(typeof(GorillaNot), "IncrementRPCCall", new Type[] { typeof(PhotonMessageInfo), typeof(string) })]
    public class AntiIncrementRPCCall
    {
        private static bool Prefix(PhotonMessageInfo info, string callingMethod = "") => false;
    }

    [HarmonyPatch(typeof(GorillaNot), "IncrementRPCCallLocal")]
    public class AntiIncrementRPCCallLocal
    {
        private static bool Prefix(PhotonMessageInfoWrapped infoWrapped, string rpcFunction) => false;
    }
}
