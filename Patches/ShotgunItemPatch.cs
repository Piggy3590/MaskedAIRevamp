using HarmonyLib;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Linq;
using UnityEngine.AI;
using GameNetcodeStuff;

namespace MaskedAIRevamp.Patches
{
    [HarmonyPatch(typeof(ShotgunItem))]
    internal class ShotgunItemPatch
    {
        public bool GetVar(ref bool ___localClientSendingShootGunRPC)
        {
            return ___localClientSendingShootGunRPC;
        }
    }
}