using HarmonyLib;
using LethalNetworkAPI;
using System.Linq.Expressions;
using System.Xml.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.XR.Haptics;
namespace MaskedAIRevamp.Patches
{
    public class SyncConfiguration : NetworkBehaviour
    {
        /*
        public LethalClientMessage<bool> EnableSkinWalkersConfig = new LethalClientMessage<bool>(identifier: "EnableSkinWalkers");
        public LethalClientMessage<bool> UseTerminalConfig = new LethalClientMessage<bool>("UseTerminal");
        public LethalClientMessage<bool> UseTerminalCreditConfig = new LethalClientMessage<bool>("UseTerminalCredit");
        public LethalClientMessage<bool> MaskedShipDepartureConfig = new LethalClientMessage<bool>("MaskedShipDeparture");
        public void Start()
        {
            UseTerminalConfig.OnReceived += SyncUseTerminalConfig;
            UseTerminalCreditConfig.OnReceived += SyncUseTerminalCreditConfig;
            MaskedShipDepartureConfig.OnReceived += SyncMaskedShipDepartureConfig;
            if (GameNetworkManager.Instance.isHostingGame)
            {
                if (Plugin.useTerminal)
                {
                    UseTerminalConfig.SendAllClients(true);
                }
                if (Plugin.useTerminalCredit)
                {
                    UseTerminalCreditConfig.SendAllClients(true);
                }
                if (Plugin.useTerminalCredit)
                {
                    MaskedShipDepartureConfig.SendAllClients(true);
                }
            }
        }

        void SyncUseTerminalConfig(bool isTrue)
        {
            Plugin.useTerminal = isTrue;
        }
        void SyncUseTerminalCreditConfig(bool isTrue)
        {
            Plugin.useTerminalCredit = isTrue;
        }
        void SyncMaskedShipDepartureConfig(bool isTrue) 
        {
            Plugin.maskedShipDeparture = isTrue;
        }
        */
    }
}