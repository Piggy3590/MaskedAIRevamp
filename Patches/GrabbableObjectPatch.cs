using HarmonyLib;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR.Haptics;
namespace MaskedAIRevamp.Patches
{
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class GrabbableObjectPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void Start_Postfix(GrabbableObject __instance)
        {
            __instance.gameObject.AddComponent<CheckItemCollision>();
            GlobalItemList.Instance.allitems.Add(__instance);
            if (__instance is WalkieTalkie) { GlobalItemList.Instance.allWalkieTalkies.Add(__instance.GetComponent<WalkieTalkie>()); }
        }

        [HarmonyPostfix]
        [HarmonyPatch("DestroyObjectInHand")]
        private static void DestroyObjectInHand_Postfix(GrabbableObject __instance)
        {
            GlobalItemList.Instance.allitems.Remove(__instance);
            if (__instance is WalkieTalkie) { GlobalItemList.Instance.allWalkieTalkies.Remove(__instance.GetComponent<WalkieTalkie>()); }
        }
    }
}