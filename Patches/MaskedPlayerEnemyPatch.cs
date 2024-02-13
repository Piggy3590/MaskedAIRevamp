using HarmonyLib;
using UnityEngine;
using UnityEngine.AI;
namespace MaskedAIRevamp.Patches
{
    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    internal class MaskedPlayerEnemyPatch
    {
        public static MaskedRevamp vd;

        [HarmonyPrefix]
        [HarmonyPatch("Awake")]
        private static void Awake_Prefix(EnemyAI __instance)
        {
            vd = __instance.gameObject.AddComponent<MaskedRevamp>();
        }

        //[HarmonyPatch(typeof(MaskedPlayerEnemy), "OnCollideWithPlayer")]
        //[HarmonyPrefix]
        //static bool Prefix()
        //{
        //    return false;
        //}
    }
}