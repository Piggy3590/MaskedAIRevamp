using HarmonyLib;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR.Haptics;
namespace MaskedAIRevamp.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        private static void Awake_Postfix()
        {
            StartOfRound.Instance.gameObject.AddComponent<SyncConfiguration>();
            StartOfRound.Instance.gameObject.AddComponent<GlobalItemList>();
        }
    }
}