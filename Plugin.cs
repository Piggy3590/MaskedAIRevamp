using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using MaskedAIRevamp.Patches;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace MaskedAIRevamp
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "Piggy.MaskedAIRevamp";
        private const string modName = "MaskedAIRevamp";
        private const string modVersion = "0.2.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static Plugin Instance;

        public static ManualLogSource mls;
        public static AssetBundle Bundle;

        //General
        public static bool enableSkinWalkers;
        //public static bool enableMoreEmotes;

        //Masked
        public static bool useTerminal;
        public static bool useTerminalCredit;
        public static bool maskedShipDeparture;

        public static GameObject MapDotPrefab;
        public static RuntimeAnimatorController MaskedAnimController;
        public static RuntimeAnimatorController MapDotRework;

        public static string PluginDirectory;

        public static bool skinWalkersIntergrated;
        public static bool moreEmotesIntergrated;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            Plugin.PluginDirectory = base.Info.Location;

            this.LoadAssets();

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Masked AI Revamp is loaded");

            Plugin.enableSkinWalkers = (bool)base.Config.Bind<bool>("General", "SkinWalkers mod Compatibility", true, "Enables compatibility with the SkinWalkers mod. (Requires SkinWalkers mod installed, automatically disables on launch if not installed)").Value;
            //Plugin.enableMoreEmotes = (bool)base.Config.Bind<bool>("General", "More Emotes mod Compatibility", true, "Enables compatibility with the More Emotes mod. (Requires SkinWalkers mod installed, automatically disables on launch if not installed)").Value;
            
            Plugin.useTerminal = (bool)base.Config.Bind<bool>("Masked", "Masked terminal access", true , "Allows Masked to use the terminal.").Value;
            Plugin.useTerminalCredit = (bool)base.Config.Bind<bool>("Masked", "Masked uses credits", false, "(Not working rn) Allows Masked to use the terminal to spend credits.").Value;
            Plugin.maskedShipDeparture = (bool)base.Config.Bind<bool>("Masked", "Masked pulls the brake lever", false, "(Not working rn) Allows Masked to pull the brake lever. Um... really...?").Value;


            harmony.PatchAll(typeof(Plugin));

            harmony.PatchAll(typeof(MaskedPlayerEnemyPatch));
            harmony.PatchAll(typeof(ShotgunItemPatch));
            harmony.PatchAll(typeof(GrabbableObjectPatch));
            harmony.PatchAll(typeof(StartOfRoundPatch));
        }

        void Start()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.Keys.Any(k => k == "RugbugRedfern.SkinwalkerMod"))
            {
                mls.LogInfo("Masked AI Revamp <-> SkinWalker Intergrated!");
                skinWalkersIntergrated = true;
            }
            /*
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.Keys.Any(k => k == "MoreEmotes"))
            {
                mls.LogInfo("Masked AI Revamp <-> More Emotes Intergrated!");
                moreEmotesIntergrated = true;
            }
            */
        }
        private void LoadAssets()
        {
            try
            {
                Plugin.Bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Plugin.PluginDirectory), "mapdotanimpack"));
            }
            catch (Exception ex)
            {
                Plugin.mls.LogError("Couldn't load asset bundle: " + ex.Message);
                return;
            }
            try
            {
                Plugin.MapDotRework = Plugin.Bundle.LoadAsset<RuntimeAnimatorController>("MapDotRework.controller");
                Plugin.MapDotPrefab = Plugin.Bundle.LoadAsset<GameObject>("MaskedMapDot.prefab");
                Plugin.MaskedAnimController = Plugin.Bundle.LoadAsset<RuntimeAnimatorController>("MaskedMetarig.controller");
                base.Logger.LogInfo("Successfully loaded assets!");
            }
            catch (Exception ex2)
            {
                base.Logger.LogError("Couldn't load assets: " + ex2.Message);
            }
        }

        
    }
}
