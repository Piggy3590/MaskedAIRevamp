using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace MaskedAIRevamp.Patches
{
    public class GlobalItemList : MonoBehaviour
    {
        public static GlobalItemList Instance { get; private set; }
        public List<GrabbableObject> allitems = new List<GrabbableObject>();
        private List<GrabbableObject> previtems = new List<GrabbableObject>();
        public List<WalkieTalkie> allWalkieTalkies = new List<WalkieTalkie>();
        public bool isShotgun;
        public bool isShovel;
        public bool isWalkie;

        private void Awake()
        {
            if (GlobalItemList.Instance == null)
            {
                GlobalItemList.Instance = this;
                return;
            }
            GameObject.Destroy(GlobalItemList.Instance.gameObject);
            GlobalItemList.Instance = this;
        }

        void Update()
        {
            if (allitems != previtems)
            {
                CheckItem(1);
                CheckItem(2);
                CheckItem(3);
                previtems = allitems;
            }
        }

        void CheckItem(int id)
        {
            foreach (GrabbableObject item in allitems)
            {
                if (id == 1)
                {
                    if (item is ShotgunItem)
                    {
                        isShotgun = true;
                        break;
                    }
                    isShotgun = false;
                }
                if (id == 2)
                {
                    if (item is Shovel)
                    {
                        isShovel = true;
                        break;
                    }
                    isShovel = false;
                }
                if (id == 3)
                {
                    if (item is WalkieTalkie)
                    {
                        isWalkie = true;
                        break;
                    }
                    isWalkie = false;
                }
            }
        }
    }
}
