using HarmonyLib;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Linq;
using UnityEngine.AI;
using GameNetcodeStuff;
using System.Reflection;
using LethalNetworkAPI;
using TMPro;
using System.Data.SqlTypes;
using static UnityEngine.Rendering.HighDefinition.CameraSettings;
using SkinwalkerMod;
using System.Collections.Generic;
using System;
using System.Security;
using System.Security.Permissions;
using System.Collections;
using System.IO;
using System.Security.Policy;
using static UnityEngine.GraphicsBuffer;
using System.Linq.Expressions;
using System.Xml.Linq;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace MaskedAIRevamp.Patches
{
    public class MaskedRevamp : NetworkBehaviour
    {
        public enum Personality
        {
            None,
            Aggressive,
            Stealthy,
            Cunning,
            Deceiving
        }

        public Personality maskedPersonality;

        public AISearchRoutine seachForItems;

        public float stopAndTbagTimer = 1.1f;
        public float stopAndTbagCooldown;
        public int randomPose;
        public bool isHoldingObject;
        public bool heldTwoHanded;
        
        public bool moveSpecial;

        public EnemyAI __instance;
        public MaskedPlayerEnemy maskedEnemy;
        public Animator creatureAnimator;
        public NavMeshAgent agent;

        public bool checkDestination;

        public bool wantItems = true;
        public GrabbableObject closestGrabbable;
        public CheckItemCollision itemSystem;

        public float enterTerminalCodeTimer;
        public int enterTermianlSpecialCodeTime;

        //public LethalClientMessage<int> SelectPersonalityInt = new LethalClientMessage<int>("SelectPersonalityInt");
        public LethalNetworkVariable<int> enterTermianlSpecialCodeInt = new LethalNetworkVariable<int>(identifier: "enterTermianlSpecialCodeInt");

        public LethalNetworkVariable<bool> isCrouched = new LethalNetworkVariable<bool>(identifier: "isCrouched");
        public LethalNetworkVariable<bool> dropItem = new LethalNetworkVariable<bool>(identifier: "dropItem");
        public LethalNetworkVariable<bool> isDancing = new LethalNetworkVariable<bool>(identifier: "isDancing");
        public LethalNetworkVariable<bool> useWalkie = new LethalNetworkVariable<bool>(identifier: "useWalkie");
        public LethalNetworkVariable<bool> isJumped = new LethalNetworkVariable<bool>(identifier: "isJumped");


        public LethalNetworkVariable<int> SelectPersonalityInt = new LethalNetworkVariable<int>(identifier: "SelectPersonalityInt");
        public LethalNetworkVariable<int> maxDanceCount = new LethalNetworkVariable<int>(identifier: "maxDanceCount");
        public LethalNetworkVariable<float> terminalTimeFloat = new LethalNetworkVariable<float>(identifier: "terminalTimeFloat");

        public float jumpTime = 1;

        private float dropTimerB;

        private Vector3 prevPos;
        private float velX;
        private float velZ;

        public float closetTimer;

        //public LethalNetworkMessage<string> crouchReceived = new LethalNetworkMessage<string>("GasReceive");

        //LethalNetworkMessage<bool> isCrouchedNetwork = new LethalNetworkMessage<bool>(identifier: "isCrouched");
        //private static NetworkVariable<bool> isCrouchedNetwork = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private bool enableDance;

        public float shovelTimer;
        public float hornTimer;
        public bool stunThrowed;

        public float angle1;
        public float angle2;

        public float dropTimer;
        public float shootTimer;
        public float rotationTimer;
        public float rotationCooldown;

        public bool itemDroped;
        public bool droppingItem;

        public Terminal terminal;
        public bool isUsingTerminal;
        public bool noMoreTerminal;

        public float dropShipTimer;
        public bool isDeliverEmptyDropship;

        public GameObject itemHolder;

        public float upperBodyAnimationsWeight;

        public float grabbableTime;

        public float distanceToPlayer = 1000;
        private float breakerBoxDistance = 1000;
        private float bushDistance = 1000;

        //DDD
        public bool isStaminaDowned;


        public Vector3 originDestination;

        //public LethalNetworkMessage<bool> walkieUsed = new LethalNetworkMessage<bool>("WalkieUsed");
        //public LethalNetworkMessage<bool> walkieVoiceTransmitted = new LethalNetworkMessage<bool>("WalkieVoiceTransmitted");
        //public LethalNetworkMessage<float> walkieTimer = new LethalNetworkMessage<float>("WalkieTimer");
        //public LethalNetworkMessage<float> walkieCooldown = new LethalNetworkMessage<float>("WalkieCooldown");

        public bool walkieUsed;
        public bool walkieVoiceTransmitted;
        public float walkieTimer;
        public float walkieCooldown;

        //Steathy
        public float originTimer;

        //Cunning
        private BreakerBox breakerBox;
        private AnimatedObjectTrigger powerBox;
        private GameObject[] bushes;
        private ItemDropship dropship;
        private TerminalAccessibleObject[] terminalAccessibleObject;

        //Aggressive
        private float lookTimer;
        private bool lookedPlayer;
        public bool notGrabClosestItem;

        //public MaskedNetworkVariable networkVariable;

        public void Start()
        {
            if (GameNetworkManager.Instance.isHostingGame)
            {
                enterTermianlSpecialCodeInt.Value = UnityEngine.Random.Range(0, 4);
            }

            if (GameObject.FindGameObjectWithTag("Bush") != null)
            {
                if (bushes != GameObject.FindGameObjectsWithTag("Bush"))
                {
                    bushes = GameObject.FindGameObjectsWithTag("Bush");
                }
                foreach (GameObject bush in bushes)
                {
                    if (bush.GetComponent<BushSystem>() == null)
                    {
                        bush.AddComponent<BushSystem>();
                    }
                }
            }
            terminal = GameObject.FindObjectOfType<Terminal>();
            __instance = this.GetComponent<MaskedPlayerEnemy>();
            maskedEnemy = this.GetComponent<MaskedPlayerEnemy>();
            creatureAnimator = this.transform.GetChild(0).GetChild(3).GetComponent<Animator>();
            itemHolder = new GameObject("ItemHolder");
            itemHolder.transform.parent = __instance.transform.GetChild(0).GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0);
            itemHolder.transform.localPosition = new Vector3(-0.002f, 0.036f, -0.042f);
            itemHolder.transform.localRotation = Quaternion.Euler(-3.616f, -2.302f, 0.145f);
            if (GameNetworkManager.Instance.isHostingGame)
            {
                maxDanceCount.Value = UnityEngine.Random.Range(2, 4);
            }
            if (creatureAnimator.runtimeAnimatorController != Plugin.MaskedAnimController)
            {
                creatureAnimator.runtimeAnimatorController = Plugin.MaskedAnimController;
            }

            if (__instance.transform.GetChild(3).GetChild(0).GetComponent<Animator>().runtimeAnimatorController != Plugin.MapDotRework)
            {
                __instance.transform.GetChild(3).GetChild(0).GetComponent<Animator>().runtimeAnimatorController = Plugin.MapDotRework;
            }

            dropship = GameObject.FindObjectOfType<ItemDropship>();
            TerminalAccessibleObject[] terminalAccessibleObject = GameObject.FindObjectsOfType<TerminalAccessibleObject>();
        }

        void Jump(bool enable)
        {
            if (jumpTime > 0 && !isJumped.Value)
            {
                jumpTime -= Time.deltaTime;
            }
            if (!isCrouched.Value && !isJumped.Value)
            {
                if (jumpTime < 1f && jumpTime > 0.9f)
                {
                    isJumped.Value = true;
                    creatureAnimator.SetTrigger("FakeJump");
                }
            }
        }
        void Dance(bool enabled)
        {
            if (enabled)
            {
                isDancing.Value = true;
                __instance.movingTowardsTargetPlayer = false;
                agent.speed = 0;
            }
            else
            {
                isDancing.Value = false;
            }
        }
        void InvokeAllClientsSynced() { Plugin.mls.LogWarning("InvokeAllClientsSynced"); }
        void InvokeOtherClientsSynced() { Plugin.mls.LogWarning("InvokeOtherClientsSynced"); }

        void SelectPersonality(int num)
        {
            if (num == 0)
            {
                maskedPersonality = Personality.Aggressive;
            }
            else if (num == 1)
            {
                maskedPersonality = Personality.Cunning;
            }
            else if (num == 2)
            {
                maskedPersonality = Personality.Deceiving;
            }
            else if (num == 3)
            {
                maskedPersonality = Personality.Stealthy;
            }
        }

        void SyncTermianlInt(int num)
        {
            enterTermianlSpecialCodeTime = num;
        }

        public void Update()
        {
            if (__instance.isEnemyDead)
            {
                agent.enabled = false;
            }
            if (useWalkie.Value)
            {
                HoldWalkie();
                useWalkie.Value = false;
            }

            if (GameNetworkManager.Instance.isHostingGame)
            {
                if (maskedPersonality == Personality.None)
                {
                    SelectPersonalityInt.Value = UnityEngine.Random.Range(0, 4);
                    //SelectPersonalityInt.SendAllClients(personalityint);
                }

                if (SelectPersonalityInt.Value == 0)
                {
                    maskedPersonality = Personality.Aggressive;
                }
                else if (SelectPersonalityInt.Value == 1)
                {
                    maskedPersonality = Personality.Cunning;
                }
                else if (SelectPersonalityInt.Value == 2)
                {
                    maskedPersonality = Personality.Deceiving;
                }
                else if (SelectPersonalityInt.Value == 3)
                {
                    maskedPersonality = Personality.Stealthy;
                }
            }

            if (!TryGetComponent(out agent))
            {
                agent = this.GetComponent<NavMeshAgent>();
            }
            if (Plugin.skinWalkersIntergrated && IsHost && maskedPersonality == Personality.Deceiving)
            {
                useWalkie.Value = true;
            }

            if (creatureAnimator == null) { Plugin.mls.LogError("VariableDeclarationClass.Update():  creatureAnimator is null!"); return; }
            if (agent == null) { Plugin.mls.LogError("VariableDeclarationClass.Update():  __agent is null!"); return; }
            if (__instance == null) { Plugin.mls.LogError("VariableDeclarationClass.Update():  __instance is null!"); return; }
            //networkVariable._isJumped = isJumped;
            //networkVariable._maxDanceCount = maxDanceCount;
            //networkVariable._randomPose = randomPose;
            //networkVariable._stopAndTbagCooldown = stopAndTbagCooldown;

            //Foliage

            if (maskedPersonality == Personality.Cunning)
            {
                MaskedCunning();
            }

            //CheckPathRotating(agent, __instance);
            if (Plugin.useTerminal && (maskedPersonality == Personality.Cunning || maskedPersonality == Personality.Deceiving))
            {
                UsingTerminal();
            }

            if (maskedEnemy.isInsidePlayerShip && isHoldingObject && maskedPersonality == Personality.Cunning)
            {
                dropItem.Value = true;
            }

            if (__instance.targetPlayer != null)
            {
                distanceToPlayer = Vector3.Distance(creatureAnimator.transform.position, __instance.targetPlayer.transform.position);
                maskedEnemy.lookAtPositionTimer = 0;
            }
            if (!__instance.isEnemyDead)
            {
                if (isCrouched.Value)
                {
                    creatureAnimator.SetTrigger("Crouching");
                }
                else
                {
                    creatureAnimator.ResetTrigger("Crouching");
                }
                if (isCrouched.Value && !maskedEnemy.running)
                {
                    agent.speed = 1.9f;
                }
                else if (maskedEnemy.running)
                {
                    maskedEnemy.creatureAnimator.SetBool("Running", true);
                    maskedEnemy.staminaTimer -= Time.deltaTime * 0.05f;
                    agent.speed = 7f;
                }
                if (isDancing.Value)
                {
                    creatureAnimator.ResetTrigger("Crouching");
                    creatureAnimator.SetTrigger("Dancing");
                    __instance.SetDestinationToPosition(__instance.transform.position, false);
                    agent.speed = 0;
                    Plugin.mls.LogInfo("Dancing");
                }
                else if (!maskedEnemy.running && !isCrouched.Value)
                {
                    agent.speed = 3.8f;
                    creatureAnimator.ResetTrigger("Dancing");
                }
            }

            //Kill Player
            if (!maskedEnemy.isEnemyDead && !isUsingTerminal && !(maskedPersonality == Personality.Aggressive && isHoldingObject && (closestGrabbable is Shovel || closestGrabbable is ShotgunItem)))
            {
                foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
                {
                    float distance = Vector3.Distance(player.transform.position, this.transform.position);
                    if (distance < 1)
                    {
                        maskedEnemy.KillPlayerAnimationServerRpc((int)player.playerClientId);
                        maskedEnemy.startingKillAnimationLocalClient = true;
                        if (player.isCrouching)
                        {
                            player.Crouch(false);
                        }
                    }
                }
            }

            //Stealthy
            if (!__instance.isEnemyDead)
            {
                if (IsHost && maskedPersonality == Personality.Stealthy)
                {
                    AwayFromPlayer();
                }

                if (IsHost && maskedPersonality == Personality.Stealthy)
                {
                    PlayerLikeAction();
                }
            }

            //Deceiving
            if (maskedPersonality == Personality.Deceiving || maskedPersonality == Personality.Cunning)
            {
                __instance.targetPlayer = null;
                if (__instance.isOutside && !__instance.isInsidePlayerShip && !__instance.isEnemyDead)
                {
                    maskedEnemy.lostLOSTimer = 0;
                    maskedEnemy.stopAndStareTimer = 0;
                    __instance.SetDestinationToPosition(GameObject.Find("LockerAudio").transform.position, true);
                    if (maskedEnemy.staminaTimer >= 5f && !isStaminaDowned)
                    {
                        if (!isJumped.Value)
                        {
                            isJumped.Value = true;
                        }else
                        {
                            creatureAnimator.ResetTrigger("FakeJump");
                        }
                        maskedEnemy.running = true;
                        //maskedEnemy.staminaTimer -= Time.deltaTime * 0.05f;
                        return;
                    }else if  (maskedEnemy.staminaTimer < 0)
                    {
                        isStaminaDowned = true;
                        maskedEnemy.running = false;
                        maskedEnemy.creatureAnimator.SetTrigger("Tired");
                    }
                    if (isStaminaDowned)
                    {
                        maskedEnemy.staminaTimer += Time.deltaTime * 0.2f;
                        if (maskedEnemy.staminaTimer < 3)
                        {
                            isStaminaDowned = false;
                            maskedEnemy.creatureAnimator.ResetTrigger("Tired");
                        }
                    }
                }
            }

            if (!maskedEnemy.isEnemyDead)
            {
                //Aggressive
                if (maskedPersonality == Personality.Aggressive && GlobalItemList.Instance.isShotgun)
                {
                    FindAndGrabShotgun();
                    if (isHoldingObject && closestGrabbable is ShotgunItem)
                    {
                        UseItem(maskedEnemy.targetPlayer, distanceToPlayer);
                    }
                }
                //Other
                else if (!__instance.isInsidePlayerShip && !isHoldingObject)
                {
                    GrabItem();
                }
            }

            if (maskedEnemy.isEnemyDead && isHoldingObject)
            {
                closestGrabbable.parentObject = null;
                closestGrabbable.isHeld = false;
                closestGrabbable.isHeldByEnemy = false;
                isHoldingObject = false;
            }

            if (__instance.targetPlayer == null && isHoldingObject)
            {
                dropTimer = 0f;
            }
            if (maskedPersonality != Personality.Cunning && __instance.targetPlayer != null && isHoldingObject && (closestGrabbable is not Shovel && closestGrabbable is not ShotgunItem && maskedPersonality == Personality.Aggressive))
            {
                dropTimer += Time.deltaTime;
                if (IsHost)
                {
                    if (!itemDroped)
                    {
                        dropTimerB = UnityEngine.Random.Range(0.2f, 4f);
                        itemDroped = true;
                    }
                    if (dropTimer > dropTimerB)
                    {
                        dropItem.Value = true;
                    }
                }
            }else if (__instance.targetPlayer != null && isHoldingObject && maskedPersonality != Personality.Aggressive && maskedPersonality != Personality.Stealthy && maskedPersonality != Personality.Cunning)
            {
                dropTimer += Time.deltaTime;
                if (IsHost)
                {
                    if (!itemDroped)
                    {
                        dropTimerB = UnityEngine.Random.Range(0.2f, 4f);
                        itemDroped = true;
                    }
                    if (dropTimer > dropTimerB)
                    {
                        dropItem.Value = true;
                    }
                }
            }

            if (isHoldingObject && __instance.targetPlayer != null && closestGrabbable is StunGrenadeItem && maskedPersonality == Personality.Aggressive)
            {
                StunGrenadeItem stunGrenade = closestGrabbable.GetComponent<StunGrenadeItem>();

                if (distanceToPlayer < 8f && !stunThrowed)
                {
                    stunThrowed = true;
                    creatureAnimator.SetTrigger("StunPin");

                    stunGrenade.inPullingPinAnimation = true;
                    stunGrenade.playerHeldBy.activatingItem = true;
                    stunGrenade.playerHeldBy.doingUpperBodyEmote = 1.16f;
                    stunGrenade.itemAnimator.SetTrigger("pullPin");
                    stunGrenade.itemAudio.PlayOneShot(stunGrenade.pullPinSFX);
                    WalkieTalkie.TransmitOneShotAudio(stunGrenade.itemAudio, stunGrenade.pullPinSFX, 0.8f);
                    stunGrenade.inPullingPinAnimation = false;
                    stunGrenade.pinPulled = true;
                    stunGrenade.itemUsedUp = true;
                    stunGrenade.grabbable = false;

                    Vector3 vector = stunGrenade.transform.position;
                    stunGrenade.grenadeThrowRay = new Ray(stunGrenade.playerHeldBy.gameplayCamera.transform.position, stunGrenade.playerHeldBy.gameplayCamera.transform.forward);
                    if (Physics.Raycast(stunGrenade.grenadeThrowRay, out stunGrenade.grenadeHit, 12f, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
                    {
                        vector = stunGrenade.grenadeThrowRay.GetPoint(stunGrenade.grenadeHit.distance - 0.05f);
                    }
                    else
                    {
                        vector = stunGrenade.grenadeThrowRay.GetPoint(10f);
                    }
                    Debug.DrawRay(vector, Vector3.down, Color.blue, 15f);
                    stunGrenade.grenadeThrowRay = new Ray(vector, Vector3.down);
                    Vector3 vector2;
                    if (Physics.Raycast(stunGrenade.grenadeThrowRay, out stunGrenade.grenadeHit, 30f, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
                    {
                        vector2 = stunGrenade.grenadeHit.point + Vector3.up * 0.05f;
                    }
                    else
                    {
                        vector2 = stunGrenade.grenadeThrowRay.GetPoint(30f);
                    }

                    closestGrabbable.parentObject = null;
                    closestGrabbable.transform.SetParent(StartOfRound.Instance.propsContainer, true);
                    closestGrabbable.EnablePhysics(true);
                    closestGrabbable.fallTime = 0f;
                    closestGrabbable.startFallingPosition = closestGrabbable.transform.parent.InverseTransformPoint(closestGrabbable.transform.position);
                    closestGrabbable.targetFloorPosition = closestGrabbable.transform.parent.InverseTransformPoint(vector2);
                    closestGrabbable.floorYRot = -1;
                    isHoldingObject = false;
                    closestGrabbable.DiscardItemFromEnemy();
                }
            }

            IdleAnimationSelector(creatureAnimator, __instance);

            if (!__instance.isEnemyDead && isHoldingObject && __instance.targetPlayer != null && !(maskedPersonality == Personality.Aggressive && (closestGrabbable is Shovel || closestGrabbable is ShotgunItem)))
            {
                if (__instance.isOutside)
                {
                    __instance.SetDestinationToPosition(maskedEnemy.shipHidingSpot, false);
                }
                else
                {
                    __instance.SetDestinationToPosition(maskedEnemy.mainEntrancePosition, false);
                }
            }

            if (__instance.isInsidePlayerShip)
            {
                if (maskedPersonality != Personality.Aggressive && isHoldingObject)
                {
                    float terminalDistance = Vector3.Distance(this.transform.position, terminal.transform.position);
                    if (terminalDistance < 6f)
                    {
                        dropItem.Value = true;
                    }
                }
            }

            if (noMoreTerminal && !__instance.isEnemyDead)
            {
                maskedEnemy.LookAndRunRandomly(true, true);
                if (maskedEnemy.isOutside)
                {
                    maskedEnemy.SetDestinationToPosition(maskedEnemy.mainEntrancePosition, true);
                }
            }

            if (maskedEnemy.stopAndStareTimer >= 0f && stopAndTbagCooldown <= 0 && !__instance.isEnemyDead)
            {
                if (GameNetworkManager.Instance.isHostingGame)
                {
                    if (stopAndTbagTimer <= 0)
                    {
                        randomPose = UnityEngine.Random.Range(0, 4);
                    }
                    stopAndTbagTimer -= Time.deltaTime;
                    if (randomPose == 0)
                    {
                        if (stopAndTbagTimer < 1f && stopAndTbagTimer > 0.8f)
                        {
                            isCrouched.Value = true;
                        }
                        else if (stopAndTbagTimer < 0.8f && stopAndTbagTimer > 0.6f)
                        {
                            isCrouched.Value = false;
                        }
                        else if (stopAndTbagTimer < 0.6f && stopAndTbagTimer > 0.4f)
                        {
                            isCrouched.Value = true;
                        }
                        else if (stopAndTbagTimer > 0 && stopAndTbagTimer < 0.4f)
                        {
                            isCrouched.Value = false;
                            stopAndTbagCooldown = 10;
                        }
                    }
                    else if (randomPose == 1 && maskedPersonality != Personality.Aggressive)
                    {
                        if (stopAndTbagTimer < 1.1f && stopAndTbagTimer > 0.8f)
                        {
                            isDancing.Value = true;
                        }
                        else if (stopAndTbagTimer < 0.7f && stopAndTbagTimer > 0.2f)
                        {
                            isDancing.Value = false;
                            isCrouched.Value = true;
                        }
                        else if (stopAndTbagTimer > 0 && stopAndTbagTimer < 0.2f)
                        {
                            stopAndTbagCooldown = 10;
                            isCrouched.Value = false;
                        }
                    }
                }
            }
            else
            {
                stopAndTbagTimer = 2.5f;
                if (stopAndTbagCooldown > 0)
                {
                    stopAndTbagCooldown -= Time.deltaTime;
                }
            }

            if (__instance.targetPlayer != null)
            {
                LookAtPos(maskedEnemy.targetPlayer.transform.position, 0.5f);
                if (maskedPersonality == Personality.Cunning)
                {
                    lookTimer += Time.deltaTime;
                    if (lookTimer < 1 && !lookedPlayer)
                    {
                        LookAtPos(maskedEnemy.targetPlayer.transform.position, 2.5f);
                        lookedPlayer = true;
                    }
                    if (lookTimer > 5f)
                    {
                        lookTimer = 0;
                        lookedPlayer = false;
                    }
                }
                if (enableDance)
                {
                    isDancing.Value = true;
                    maskedEnemy.stopAndStareTimer = 0.9f;
                    agent.speed = 0;
                }
                if (distanceToPlayer < 17f && __instance.targetPlayer.performingEmote && maxDanceCount.Value > 0)
                {
                    if (GameNetworkManager.Instance.isHostingGame && !enableDance)
                    {
                        maxDanceCount.Value -= 1;
                        randomPose = 1;
                        enableDance = true;
                    }
                    stopAndTbagTimer = 0.9f;
                    __instance.agent.speed = 0;
                }
                else if (isDancing.Value && GameNetworkManager.Instance.isHostingGame)
                {
                    isDancing.Value = false;
                    stopAndTbagTimer = 0.4f;
                    randomPose = 1;
                    enableDance = false;
                }
            }
        }

        public bool isReeledWithShovel;
        public bool isHittedWithShovel;
        public bool shovelHitConfirm;

        public void UseItem(PlayerControllerB target, float distance)
        {
            shovelTimer += Time.deltaTime;

            if (isHoldingObject)
            {
                if (closestGrabbable is FlashlightItem)
                {
                    if (shovelTimer < 0.7f)
                    {
                        closestGrabbable.GetComponent<FlashlightItem>().ItemActivate(false);
                    }
                    else if (shovelTimer > 0.7f && shovelTimer < 1.4f)
                    {
                        closestGrabbable.GetComponent<FlashlightItem>().ItemActivate(true);
                    }
                    else if (shovelTimer > 1)
                    {
                        shovelTimer = 0;
                    }
                }
                if (closestGrabbable is Shovel)
                {
                    if (shovelTimer < 0.5f)
                    {
                        if (!isReeledWithShovel)
                        {
                            creatureAnimator.SetTrigger("ShovelUp");
                            closestGrabbable.GetComponent<Shovel>().shovelAudio.PlayOneShot(closestGrabbable.GetComponent<Shovel>().reelUp);
                            isReeledWithShovel = true;
                        }
                    }
                    else if (shovelTimer > 0.5f && shovelTimer < 0.7f)
                    {
                        if (!isHittedWithShovel)
                        {
                            creatureAnimator.ResetTrigger("ShovelUp");
                            //closestGrabbable.GetComponent<Shovel>().HitShovel(!maskedEnemy);
                            if (distance < 3f)
                            {
                                target.movementAudio.PlayOneShot(StartOfRound.Instance.hitPlayerSFX);
                                if (target.health > 20)
                                {
                                    target.DamagePlayer(20, true, true, CauseOfDeath.Bludgeoning, 0, false, default(Vector3));
                                    Plugin.mls.LogInfo("Damage With Shovel");
                                }else
                                {
                                    target.KillPlayer(Vector3.zero, true, CauseOfDeath.Bludgeoning, 0);
                                    maskedEnemy.targetPlayer = null;
                                    maskedEnemy.lastPlayerKilled = null;
                                    maskedEnemy.inSpecialAnimation = false;
                                    Plugin.mls.LogInfo("Killed With Shovel");
                                }
                            }
                            closestGrabbable.GetComponent<Shovel>().shovelAudio.PlayOneShot(closestGrabbable.GetComponent<Shovel>().swing);
                            isHittedWithShovel = true;
                        }
                    }
                    else if (shovelTimer > 1.5)
                    {
                        shovelTimer = 0;
                        isReeledWithShovel = false;
                        isHittedWithShovel = false;
                    }
                }
                if (maskedEnemy.targetPlayer != null && closestGrabbable is ShotgunItem && maskedPersonality == Personality.Aggressive)
                {
                    Plugin.mls.LogInfo("Shotgun Guy targeted player");
                    ShotgunItem shotgun = closestGrabbable.GetComponent<ShotgunItem>();
                    if (shotgun.shellsLoaded > 0)
                    {
                        if (closestGrabbable.GetComponent<ShotgunItem>().shellsLoaded > 0)
                        {
                            if (shootTimer > 0)
                            {
                                shootTimer -= Time.deltaTime;
                            }
                            float distanceToPlayer = Vector3.Distance(creatureAnimator.transform.position, __instance.targetPlayer.transform.position);

                            if (shotgun.safetyOn && distanceToPlayer < 8f)
                            {
                                shotgun.safetyOn = false;
                                shotgun.gunAudio.PlayOneShot(shotgun.switchSafetyOffSFX);
                                WalkieTalkie.TransmitOneShotAudio(shotgun.gunAudio, shotgun.switchSafetyOffSFX, 1f);
                                Plugin.mls.LogInfo("Safety On");
                            }
                            else if (!shotgun.safetyOn && distanceToPlayer > 12f)
                            {
                                shotgun.safetyOn = true;
                                shotgun.gunAudio.PlayOneShot(shotgun.switchSafetyOnSFX);
                                WalkieTalkie.TransmitOneShotAudio(shotgun.gunAudio, shotgun.switchSafetyOnSFX, 1f);
                                Plugin.mls.LogInfo("Safety Off");
                            }
                            if (distanceToPlayer < 10f && shootTimer <= 0)
                            {
                                Vector3 vector;
                                Vector3 vector2;

                                vector = __instance.transform.GetChild(0).GetChild(3).GetChild(3).transform.position - __instance.transform.GetChild(0).GetChild(3).GetChild(3).transform.up * 0.45f;
                                vector2 = __instance.transform.GetChild(0).GetChild(3).GetChild(3).transform.forward;

                                Plugin.mls.LogInfo("Calling shoot gun....");
                                shotgun.ShootGun(vector, vector2);
                                Plugin.mls.LogInfo("Calling shoot gun and sync");
                                shotgun.localClientSendingShootGunRPC = true;
                                shotgun.ShootGunServerRpc(vector, vector2);
                                shootTimer = 3;
                            }
                            if (__instance.targetPlayer != null && distanceToPlayer > 10f && shootTimer <= 0)
                            {
                                maskedEnemy.running = true;
                            }
                        }
                        else if (!shotgun.safetyOn && shotgun.shellsLoaded > 0 && __instance.targetPlayer == null)
                        {
                            shotgun.safetyOn = true;
                            shotgun.gunAudio.PlayOneShot(shotgun.switchSafetyOnSFX);
                            WalkieTalkie.TransmitOneShotAudio(shotgun.gunAudio, shotgun.switchSafetyOnSFX, 1f);
                        }
                    }
                }
            }

            if (dropItem.Value)
            {
                DropItem();
            }
        }

        void DropItem()
        {
            if (closestGrabbable != null && isHoldingObject)
            {
                closestGrabbable.parentObject = null;
                closestGrabbable.transform.SetParent(StartOfRound.Instance.propsContainer, true);
                closestGrabbable.EnablePhysics(true);
                closestGrabbable.fallTime = 0f;
                closestGrabbable.startFallingPosition = closestGrabbable.transform.parent.InverseTransformPoint(closestGrabbable.transform.position);
                closestGrabbable.targetFloorPosition = closestGrabbable.transform.parent.InverseTransformPoint(closestGrabbable.GetItemFloorPosition(default(Vector3)));
                closestGrabbable.floorYRot = -1;
                closestGrabbable.isHeld = false;
                closestGrabbable.isHeldByEnemy = false;
                isHoldingObject = false;
                closestGrabbable.DiscardItemFromEnemy();
                PlayerControllerB playerControllerB = __instance.CheckLineOfSightForClosestPlayer(70f, 50, 1, 3f);
                __instance.movingTowardsTargetPlayer = true;
                __instance.targetPlayer = playerControllerB;
                __instance.SwitchToBehaviourState(2);
            }
        }

        [HarmonyPatch(typeof(MaskedPlayerEnemy), "OnCollideWithPlayer")]
        [HarmonyPrefix]
        static bool OnCollideWithPlayer_Prefix()
        {
            return false;
        }

        [HarmonyPatch(typeof(MaskedPlayerEnemy), "LookAtPosition")]
        [HarmonyPrefix]
        static bool LookAtPosition_Prefix()
        {
            return false;
        }

        public void LookAtPos(Vector3 pos, float lookAtTime = 1f)
        {
            Debug.Log(string.Format("Look at position {0} called! lookatpositiontimer setting to {1}", pos, lookAtTime));
            maskedEnemy.focusOnPosition = pos;
            maskedEnemy.lookAtPositionTimer = lookAtTime;
            float num = Vector3.Angle(base.transform.forward, pos - base.transform.position);
            if (pos.y - maskedEnemy.headTiltTarget.position.y < 0f)
            {
                num *= -1f;
            }
            maskedEnemy.verticalLookAngle = num;
        }

        [HarmonyPatch(typeof(MaskedPlayerEnemy), "LookAtPlayerServerRpc")]
        [HarmonyPrefix]
        static bool LookAtPlayerServerRpc_Prefix()
        {
            return false;
        }

        public void GrabItem()
        {
            if (!isHoldingObject && wantItems)
            {
                float closestDistance = Mathf.Infinity;

                foreach (GrabbableObject grabbableItem in GlobalItemList.Instance.allitems)
                {
                    float distanceToObject = Vector3.Distance(this.transform.position, grabbableItem.transform.position);

                    if (distanceToObject < closestDistance && distanceToObject <= 10 && !grabbableItem.isHeld && !grabbableItem.isHeldByEnemy && !notGrabClosestItem)
                    {
                        closestDistance = distanceToObject;
                        closestGrabbable = grabbableItem;
                        if (closestGrabbable != null && closestGrabbable.GetComponent<CheckItemCollision>() != null)
                        {
                            itemSystem = closestGrabbable.GetComponent<CheckItemCollision>();
                        }
                        if (!itemSystem.hidedByMasked && !closestGrabbable.isHeld && !closestGrabbable.isHeldByEnemy)
                        {
                            if (IsHost)
                            {
                                if (distanceToObject > 0.5f)
                                {
                                    __instance.SetDestinationToPosition(closestGrabbable.transform.position, true);
                                    __instance.moveTowardsDestination = true;
                                    __instance.movingTowardsTargetPlayer = false;
                                }
                                else
                                {
                                    __instance.SetDestinationToPosition(maskedEnemy.mainEntrancePosition, true);
                                    __instance.moveTowardsDestination = false;
                                }
                            }
                            if (distanceToObject > 0.5f && distanceToObject < 3f)
                            {
                                maskedEnemy.focusOnPosition = closestGrabbable.transform.position;
                                maskedEnemy.lookAtPositionTimer = 1.5f;
                            }
                            if (distanceToObject < 0.9f)
                            {
                                float num = Vector3.Angle(__instance.transform.forward, closestGrabbable.transform.position - __instance.transform.position);
                                if (closestGrabbable.transform.position.y - maskedEnemy.headTiltTarget.position.y < 0f)
                                {
                                    num *= -1f;
                                }
                                maskedEnemy.verticalLookAngle = num;
                                closestGrabbable.parentObject = itemHolder.transform;
                                closestGrabbable.hasHitGround = false;
                                closestGrabbable.isHeld = true;
                                closestGrabbable.isHeldByEnemy = true;
                                closestGrabbable.grabbable = false;
                                isHoldingObject = true;
                                itemDroped = false;
                                closestGrabbable.GrabItemFromEnemy(__instance);
                            }
                            if (distanceToObject < 4f && !isHoldingObject)
                            {
                                if (IsHost)
                                {
                                    isCrouched.Value = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GrabShotgunItem()
        {
            if (!isHoldingObject && wantItems)
            {
                float closestDistance = Mathf.Infinity;

                foreach (GrabbableObject grabbableItem in GlobalItemList.Instance.allitems)
                {
                    if (grabbableItem is ShotgunItem)
                    {

                        float distanceToObject = Vector3.Distance(this.transform.position, grabbableItem.transform.position);

                        if (distanceToObject < closestDistance && distanceToObject <= 10 && !grabbableItem.isHeld && !grabbableItem.isHeldByEnemy && !notGrabClosestItem)
                        {
                            closestDistance = distanceToObject;
                            closestGrabbable = grabbableItem;
                            if (closestGrabbable != null && closestGrabbable.GetComponent<CheckItemCollision>() != null)
                            {
                                itemSystem = closestGrabbable.GetComponent<CheckItemCollision>();
                            }
                            if (!itemSystem.hidedByMasked)
                            {
                                if (IsHost)
                                {
                                    if (distanceToObject > 0.5f)
                                    {
                                        __instance.SetDestinationToPosition(closestGrabbable.transform.position, true);
                                        __instance.moveTowardsDestination = true;
                                        __instance.movingTowardsTargetPlayer = false;
                                    }
                                    else
                                    {
                                        __instance.SetDestinationToPosition(maskedEnemy.mainEntrancePosition, true);
                                        __instance.moveTowardsDestination = false;
                                    }
                                }
                                if (distanceToObject > 0.5f && distanceToObject < 3f)
                                {
                                    maskedEnemy.focusOnPosition = closestGrabbable.transform.position;
                                    maskedEnemy.lookAtPositionTimer = 1.5f;
                                }
                                if (distanceToObject < 0.9f)
                                {
                                    float num = Vector3.Angle(__instance.transform.forward, closestGrabbable.transform.position - __instance.transform.position);
                                    if (closestGrabbable.transform.position.y - maskedEnemy.headTiltTarget.position.y < 0f)
                                    {
                                        num *= -1f;
                                    }
                                    maskedEnemy.verticalLookAngle = num;
                                    closestGrabbable.parentObject = itemHolder.transform;
                                    closestGrabbable.hasHitGround = false;
                                    closestGrabbable.isHeld = true;
                                    closestGrabbable.isHeldByEnemy = true;
                                    closestGrabbable.grabbable = false;
                                    isHoldingObject = true;
                                    itemDroped = false;
                                    closestGrabbable.GrabItemFromEnemy(__instance);
                                }
                                if (distanceToObject < 4f && !isHoldingObject && maskedPersonality != Personality.Aggressive)
                                {
                                    if (IsHost)
                                    {
                                        isCrouched.Value = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void DetectEnemy()
        {
            foreach (GrabbableObject grabbableItem in GlobalItemList.Instance.allitems)
            {
                float distanceToObject = Vector3.Distance(this.transform.position, grabbableItem.transform.position);

                if (distanceToObject < Mathf.Infinity && distanceToObject <= 10)
                {
                    if (IsHost)
                    {
                        if (distanceToObject > 0.5f)
                        {
                            __instance.SetDestinationToPosition(closestGrabbable.transform.position, true);
                            __instance.moveTowardsDestination = true;
                            __instance.movingTowardsTargetPlayer = false;
                        }
                        else
                        {
                            __instance.SetDestinationToPosition(maskedEnemy.mainEntrancePosition, true);
                            __instance.moveTowardsDestination = false;
                        }
                    }
                }
            }
        }

        public void ManuelGrabItem(GrabbableObject item)
        {
            if (!isHoldingObject)
            {
                float distanceToObject = Vector3.Distance(this.transform.position, item.transform.position);
                if (distanceToObject < 0.9f)
                {
                    float num = Vector3.Angle(__instance.transform.forward, closestGrabbable.transform.position - __instance.transform.position);
                    if (closestGrabbable.transform.position.y - maskedEnemy.headTiltTarget.position.y < 0f)
                    {
                        num *= -1f;
                    }
                    maskedEnemy.verticalLookAngle = num;
                    item.parentObject = itemHolder.transform;
                    item.hasHitGround = false;
                    item.isHeld = true;
                    item.isHeldByEnemy = true;
                    item.grabbable = false;
                    isHoldingObject = true;
                    itemDroped = false;
                    item.GrabItemFromEnemy(__instance);
                }
                if (distanceToObject < 4f && !isHoldingObject && maskedPersonality != Personality.Aggressive)
                {
                    if (IsHost)
                    {
                        isCrouched.Value = true;
                    }
                }
            }
        }

        public void ForceGrabCustomItem(GrabbableObject item)
        {
            float num = Vector3.Angle(__instance.transform.forward, closestGrabbable.transform.position - __instance.transform.position);
            if (closestGrabbable.transform.position.y - maskedEnemy.headTiltTarget.position.y < 0f)
            {
                num *= -1f;
            }
            maskedEnemy.verticalLookAngle = num;
            item.parentObject = itemHolder.transform;
            item.hasHitGround = false;
            item.isHeld = true;
            item.isHeldByEnemy = true;
            item.grabbable = false;
            isHoldingObject = true;
            itemDroped = false;
            item.GrabItemFromEnemy(__instance);
        }

        public void IdleAnimationSelector(Animator creatureAnimator, EnemyAI __instance)
        {
            if (isHoldingObject)
            {
                upperBodyAnimationsWeight = Mathf.Lerp(upperBodyAnimationsWeight, 0.9f, 25f * Time.deltaTime);
                creatureAnimator.SetLayerWeight(creatureAnimator.GetLayerIndex("Item"), upperBodyAnimationsWeight);
            }
            else
            {
                upperBodyAnimationsWeight = Mathf.Lerp(upperBodyAnimationsWeight, 0f, 25f * Time.deltaTime);
                creatureAnimator.SetLayerWeight(creatureAnimator.GetLayerIndex("Item"), upperBodyAnimationsWeight);
                creatureAnimator.SetLayerWeight(creatureAnimator.GetLayerIndex("Item"), upperBodyAnimationsWeight);
            }
            if (isHoldingObject && closestGrabbable.itemProperties.twoHandedAnimation && closestGrabbable is not ShotgunItem)
            {
                creatureAnimator.SetTrigger("HoldLung");
                creatureAnimator.ResetTrigger("HoldFlash");
                creatureAnimator.ResetTrigger("HoldShotgun");
                creatureAnimator.ResetTrigger("HoldOneItem");
            }
            else if (isHoldingObject && !closestGrabbable.itemProperties.twoHandedAnimation && closestGrabbable is FlashlightItem)
            {
                creatureAnimator.SetTrigger("HoldFlash");
                creatureAnimator.ResetTrigger("HoldLung");
                creatureAnimator.ResetTrigger("HoldShotgun");
                creatureAnimator.ResetTrigger("HoldOneItem");
            }
            else if (isHoldingObject && !closestGrabbable.itemProperties.twoHandedAnimation && closestGrabbable is not ShotgunItem && closestGrabbable is not FlashlightItem && closestGrabbable is not Shovel)
            {
                creatureAnimator.SetTrigger("HoldOneItem");
                creatureAnimator.ResetTrigger("HoldFlash");
                creatureAnimator.ResetTrigger("HoldLung");
                creatureAnimator.ResetTrigger("HoldShotgun");
            }
            else if (isHoldingObject && !closestGrabbable.itemProperties.twoHandedAnimation && closestGrabbable is Shovel)
            {
                creatureAnimator.SetTrigger("HoldLung");
                creatureAnimator.ResetTrigger("HoldOneItem");
                creatureAnimator.ResetTrigger("HoldFlash");
                creatureAnimator.ResetTrigger("HoldShotgun");
            }
            else if (isHoldingObject && closestGrabbable is ShotgunItem)
            {
                creatureAnimator.SetTrigger("HoldShotgun");
                creatureAnimator.ResetTrigger("HoldFlash");
                creatureAnimator.ResetTrigger("HoldLung");
                creatureAnimator.ResetTrigger("HoldOneItem");
            }
            else if (!isHoldingObject)
            {
                creatureAnimator.ResetTrigger("HoldFlash");
                creatureAnimator.ResetTrigger("HoldLung");
                creatureAnimator.ResetTrigger("HoldShotgun");
                creatureAnimator.ResetTrigger("HoldOneItem");
            }
        }

        public void CheckPathRotating(NavMeshAgent agent, EnemyAI __instance)
        {
            if (IsHost)
            {
                NavMeshPath path = new NavMeshPath();
                int cornerIndex = 1;

                if (agent.pathPending) return;

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        // 도착
                        return;
                    }
                }

                if (cornerIndex < agent.path.corners.Length && IsHost)
                {
                    Vector3 directionToNextCorner = agent.path.corners[cornerIndex] - agent.transform.position;

                    if (cornerIndex + 1 < agent.path.corners.Length)
                    {
                        Vector3 directionToFollowingCorner = agent.path.corners[cornerIndex + 1] - agent.path.corners[cornerIndex];
                        float angle = Vector3.Angle(directionToNextCorner, directionToFollowingCorner);
                        if (angle > 30)
                        {
                            // 회전 예정
                            rotationTimer += Time.deltaTime;
                            if (rotationTimer > 0 && rotationTimer < 0.5f)
                            {
                                if (angle1 == 0)
                                {
                                    angle1 = __instance.transform.localEulerAngles.y + 10f;
                                }
                                Plugin.mls.LogInfo("angle1: " + angle1);
                                __instance.transform.localEulerAngles = new Vector3(__instance.transform.localEulerAngles.x, angle1, __instance.transform.localEulerAngles.z);
                            }
                            else if (rotationTimer > 0.5 && rotationTimer < 1.1f)
                            {
                                if (angle2 == 0)
                                {
                                    angle1 = __instance.transform.localEulerAngles.y - 5f;
                                }
                                Plugin.mls.LogInfo("angle2: " + angle2);
                                __instance.transform.localEulerAngles = new Vector3(__instance.transform.localEulerAngles.x, angle1, __instance.transform.localEulerAngles.z);
                            }
                            Plugin.mls.LogWarning("곧 30도이상 회전");
                        }
                        else
                        {
                            rotationTimer = 0;
                            angle1 = 0;
                            angle2 = 0;
                        }
                    }
                    if (directionToNextCorner.magnitude <= agent.stoppingDistance)
                    {
                        Plugin.mls.LogWarning("코너에 거의 도착했으며 다음 코너를 검사하기 위해 인덱스 증가");
                        cornerIndex++;
                    }
                }
            }
        }

        void HoldWalkie()
        {
            if (Plugin.skinWalkersIntergrated && isHoldingObject && closestGrabbable is WalkieTalkie)
            {
                WalkieTalkie walkie = closestGrabbable.GetComponent<WalkieTalkie>();
                walkieCooldown += Time.deltaTime;
                if (walkieCooldown < 1f)
                {
                    creatureAnimator.ResetTrigger("UseWalkie");
                    walkieUsed = false;
                    walkieVoiceTransmitted = false;
                    walkieTimer = 0;
                }
                else if (walkieCooldown < 4f && walkieCooldown > 3f)
                {
                    if (!walkie.isBeingUsed)
                    {
                        walkie.isBeingUsed = true;
                    }
                }
                if (walkieCooldown > 10f)
                {
                    UseWalkie();
                }
            }
        }

        public void UseWalkie()
        {
            if (Plugin.skinWalkersIntergrated && isHoldingObject && closestGrabbable is WalkieTalkie)
            {
                walkieTimer += Time.deltaTime;

                WalkieTalkie walkie = closestGrabbable.GetComponent<WalkieTalkie>();

                if (walkieTimer > 1f)
                {
                    if (!walkieUsed)
                    {
                        if (!walkie.isBeingUsed)
                        {
                            walkie.isBeingUsed = true;
                            walkie.EnableWalkieTalkieListening(true);
                            walkie.mainObjectRenderer.sharedMaterial = walkie.onMaterial;
                            walkie.walkieTalkieLight.enabled = true;
                            walkie.thisAudio.PlayOneShot(walkie.switchWalkieTalkiePowerOn);
                        }
                        walkieUsed = true;
                    }
                }
                if (walkieTimer > 1.5f)
                {
                    if (!walkieVoiceTransmitted)
                    {
                        Plugin.mls.LogInfo("Walkie Voice Transmitted!");
                        foreach (WalkieTalkie walkietalkie in GlobalItemList.Instance.allWalkieTalkies)
                        {
                            if (walkietalkie.isBeingUsed)
                            {
                                walkietalkie.thisAudio.PlayOneShot(walkietalkie.startTransmissionSFX[UnityEngine.Random.Range(0, walkietalkie.startTransmissionSFX.Length + 1)]);
                            }
                            if (closestGrabbable.gameObject != walkietalkie.gameObject && walkietalkie.isBeingUsed)
                            {
                                walkietalkie.target.PlayOneShot(SkinwalkerModPersistent.Instance.GetSample());
                            }
                        }
                        creatureAnimator.SetTrigger("UseWalkie");
                        walkieVoiceTransmitted = true;
                    }
                }
                if (walkieTimer > 5f)
                {
                    foreach (WalkieTalkie walkietalkie in GlobalItemList.Instance.allWalkieTalkies)
                    {
                        if (walkietalkie.isBeingUsed)
                        {
                            walkietalkie.thisAudio.PlayOneShot(walkietalkie.stopTransmissionSFX[UnityEngine.Random.Range(0, walkietalkie.stopTransmissionSFX.Length + 1)]);
                        }
                        if (closestGrabbable.gameObject == walkietalkie.gameObject && walkietalkie.isBeingUsed)
                        {
                            walkie.isBeingUsed = false;
                            walkie.EnableWalkieTalkieListening(false);
                            walkie.mainObjectRenderer.sharedMaterial = walkie.offMaterial;
                            walkie.walkieTalkieLight.enabled = false;
                            walkie.thisAudio.PlayOneShot(walkie.switchWalkieTalkiePowerOff);
                        }
                    }
                    creatureAnimator.ResetTrigger("UseWalkie");
                    walkieCooldown = 0f;
                    walkieTimer = 0f;
                }
            }
        }

        public PlayerControllerB nearestPlayer;

        public void AwayFromPlayer()
        {
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                float distance = Vector3.Distance(__instance.transform.position, player.transform.position);

                if (distance < Mathf.Infinity)
                {
                    nearestPlayer = player;
                    float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);
                    if (distanceToTarget < 4f && __instance.targetPlayer != null)  // 플레이어 거리 5 이내
                    {
                        // 플레이어 회피

                        Vector3 runAwayDirection = transform.position - player.transform.position;
                        Vector3 runAwayDestination = transform.position + runAwayDirection.normalized * 5f;  // 10칸 ㅌㅌ
                        if (originDestination != agent.destination)
                        {
                            originDestination = agent.destination;
                        }
                        if (Vector3.Distance(originDestination, agent.destination) < 1.5f)
                        {
                            originTimer += Time.deltaTime;
                        }
                        if (originTimer > 3.5f)
                        {
                            if (__instance.isOutside)
                            {
                                __instance.SetDestinationToPosition(maskedEnemy.shipHidingSpot, true);
                            }
                            else
                            {
                                __instance.SetDestinationToPosition(maskedEnemy.mainEntrancePosition, true);
                            }
                            originTimer = 0;
                        }

                        __instance.SetDestinationToPosition(runAwayDestination, true);
                    }else
                    {
                        __instance.SetDestinationToPosition(originDestination, true);
                    }
                }
            }
        }

        public void PlayerLikeAction()
        {
            float distanceToTarget = Vector3.Distance(transform.position, nearestPlayer.transform.position);
            if (distanceToTarget < 4f && __instance.targetPlayer != null)  // 플레이어 거리 5 이내
            {
                // 플레이어 회피

                Vector3 runAwayDirection = transform.position - nearestPlayer.transform.position;
                Vector3 runAwayDestination = transform.position + runAwayDirection.normalized * 5f;  // 10칸 ㅌㅌ
                if (originDestination != agent.destination)
                {
                    originDestination = agent.destination;
                }
                if (Vector3.Distance(originDestination, agent.destination) < 1.5f)
                {
                    originTimer += Time.deltaTime;
                }
                if (originTimer > 3.5f)
                {
                    if (__instance.isOutside)
                    {
                        __instance.SetDestinationToPosition(maskedEnemy.shipHidingSpot, true);
                    }
                    else
                    {
                        __instance.SetDestinationToPosition(maskedEnemy.mainEntrancePosition, true);
                    }
                    originTimer = 0;
                }

                __instance.SetDestinationToPosition(runAwayDestination, true);
            }
            else
            {
                __instance.SetDestinationToPosition(originDestination, true);
            }
        }

        void UsingTerminal()
        {
            float terminalDistance = Vector3.Distance(this.transform.position, terminal.transform.position);
            if (terminalDistance < 40)
            {
                dropItem.Value = true;
                if (!terminal.terminalInUse && !noMoreTerminal)
                {
                    if (terminalDistance < 3.5f)
                    {
                        if (!isUsingTerminal)
                        {
                            terminal.terminalAudio.PlayOneShot(terminal.enterTerminalSFX);
                        }
                        isUsingTerminal = true;

                    }
                }
                if (!terminal.terminalInUse && !noMoreTerminal && !__instance.isEnemyDead)
                {
                    __instance.SetDestinationToPosition(terminal.transform.position, true);
                }
            }
            if (isUsingTerminal)
            {
                creatureAnimator.SetTrigger("Terminal");
                __instance.inSpecialAnimation = true;

                terminal.placeableObject.inUse = true;
                terminal.terminalLight.enabled = true;

                __instance.movingTowardsTargetPlayer = false;
                __instance.targetPlayer = null;

                maskedEnemy.headTiltTarget.gameObject.SetActive(false);
                isCrouched.Value = false;
                agent.speed = 0;
                creatureAnimator.ResetTrigger("IsMoving");

                this.transform.LookAt(new Vector3(terminal.transform.position.x, transform.position.y, terminal.transform.position.z));
                this.transform.localPosition = new Vector3(terminal.transform.localPosition.x + 7, terminal.transform.localPosition.y + 0.25f, terminal.transform.localPosition.z + -14.8f);
                //this.transform.localPosition = Vector3.Lerp(
                //this.transform.localPosition,
                //new Vector3(terminal.transform.localPosition.x + 7, terminal.transform.localPosition.y + 0.25f, terminal.transform.localPosition.z + -14.8f),
                //Time.deltaTime * 15
                //);

                if (maskedPersonality == Personality.Cunning)
                {
                    if (terminal.numberOfItemsInDropship <= 0
                        && !dropship.shipLanded
                        && dropship.shipTimer <= 0f
                        && !isDeliverEmptyDropship && !noMoreTerminal)
                    {
                        dropShipTimer += Time.deltaTime;
                        if (dropShipTimer > 10f)
                        {
                            dropship.LandShipOnServer();
                            isDeliverEmptyDropship = true;
                        }
                    }else if (isDeliverEmptyDropship && dropShipTimer <= 12f && !noMoreTerminal)
                    {
                        dropShipTimer += Time.deltaTime;
                    }
                    if (dropShipTimer > 12f)
                    {
                        dropShipTimer = 0;
                        terminal.terminalAudio.PlayOneShot(terminal.leaveTerminalSFX);
                        isUsingTerminal = false;
                        noMoreTerminal = true;
                        __instance.SwitchToBehaviourState(2);
                        //__instance.SetDestinationToPosition(GameObject.Find("ItemShip").transform.position, false);
                    }
                }else if (maskedPersonality == Personality.Deceiving)
                {
                    float terminalTimer = UnityEngine.Random.Range(0.2f, 1.5f);
                    enterTerminalCodeTimer += Time.deltaTime;
                    if (enterTerminalCodeTimer > terminalTimeFloat.Value && enterTermianlSpecialCodeTime > 0)
                    {
                        if (GameNetworkManager.Instance.isHostingGame)
                        {
                            terminalTimeFloat.Value = UnityEngine.Random.Range(0.2f, 1.5f);
                        }
                        terminal.CallFunctionInAccessibleTerminalObject(terminalAccessibleObject[UnityEngine.Random.Range(0, terminalAccessibleObject.Length)].objectCode);
                        terminal.terminalAudio.PlayOneShot(terminal.codeBroadcastSFX);
                        enterTermianlSpecialCodeTime -= 1;
                        enterTerminalCodeTimer = 0;
                    }
                    if (enterTermianlSpecialCodeTime == 0)
                    {
                        isUsingTerminal = false;
                        noMoreTerminal = true;
                        __instance.SwitchToBehaviourState(2);
                    }
                }
            }
            else
            {
                if (!maskedEnemy.headTiltTarget.gameObject.activeSelf)
                {
                    terminal.placeableObject.inUse = false;
                    terminal.terminalLight.enabled = false;
                    creatureAnimator.ResetTrigger("Terminal");
                    maskedEnemy.headTiltTarget.gameObject.SetActive(true);
                }
            }
        }

        public void MaskedCunning()
        {
            /*
            if  (!maskedEnemy.isOutside)
            {
                if (breakerBox == null)
                {
                    breakerBox = GameObject.FindObjectOfType<BreakerBox>();
                }
                breakerBoxDistance = Vector3.Distance(__instance.transform.position, breakerBox.transform.position);
                if (breakerBoxDistance < 30 && breakerBoxDistance > 3f && agent.destination != breakerBox.transform.position)
                {
                    __instance.SetDestinationToPosition(breakerBox.transform.position, true);
                    moveSpecial = true;
                }
                if (breakerBoxDistance <= 3f)
                {
                    if (powerBox != GameObject.Find("PowerBoxDoor").GetComponent<AnimatedObjectTrigger>())
                    {
                        powerBox = GameObject.Find("PowerBoxDoor").GetComponent<AnimatedObjectTrigger>();
                    }
                    if (powerBox != null)
                    {
                        powerBox.TriggerAnimation(StartOfRound.Instance.allPlayerScripts[0]);
                    }
                }
            }
            */

            if (isHoldingObject && maskedEnemy.isOutside && IsHost && bushes != null)
            {
                foreach (GameObject bush in bushes)
                {
                    bushDistance = Vector3.Distance(__instance.transform.position, bush.transform.position);

                    if (bushDistance < Mathf.Infinity && !bush.GetComponent<BushSystem>().bushWithItem)
                    {
                        if (bushDistance > 2 && bushDistance < Mathf.Infinity && !bush.GetComponent<BushSystem>().bushWithItem
                            && breakerBoxDistance < 20)
                        {
                            moveSpecial = true;
                        }
                        if (bushDistance < 2)
                        {
                            bush.GetComponent<BushSystem>().bushWithItem = true;
                            itemSystem.hidedByMasked = true;
                            dropItem.Value = true;
                        }
                    }
                }
            }
        }

        public bool canGoThroughItem;
        public bool isDroppedShotgunAvailable;

        void FindAndGrabShotgun()
        {
            if (closestGrabbable is not ShotgunItem && isHoldingObject && !isDroppedShotgunAvailable)
            {
                Plugin.mls.LogInfo("Drop Item!");
                dropItem.Value = true;
            }
            foreach (GrabbableObject item in GlobalItemList.Instance.allitems)
            {
                if (item is ShotgunItem)
                {
                    if (item.isHeld)
                    {
                        if (!item.isHeldByEnemy)
                        {
                            HandleShotgunHeldByPlayer();
                            Plugin.mls.LogInfo("Held Shotgun Found!");
                            isDroppedShotgunAvailable = false;
                        }
                    }
                    else
                    {
                        //HandleShotgunNotHeld();
                        GrabShotgunItem();
                        isDroppedShotgunAvailable = true;
                    }
                }
            }

            if (isHoldingObject && closestGrabbable is Shovel)
            {
                foreach (GrabbableObject shotgun in GlobalItemList.Instance.allitems)
                {
                    if (shotgun is ShotgunItem)
                    {
                        if (shotgun.playerHeldBy != null)
                        {
                            __instance.SetDestinationToPosition(shotgun.playerHeldBy.transform.position, true);
                            float distanceShotgunPlayer = Vector3.Distance(this.transform.position, shotgun.playerHeldBy.transform.position);
                            maskedEnemy.stopAndStareTimer = 0;
                            if (distanceShotgunPlayer < Mathf.Infinity &&  distanceShotgunPlayer < 4f)
                            {
                                maskedEnemy.headTiltTarget.LookAt(shotgun.playerHeldBy.transform);
                                LookAtPos(shotgun.playerHeldBy.transform.position, 0.2f);
                            }
                            if (distanceShotgunPlayer < 3.2f)
                            {
                                UseItem(shotgun.playerHeldBy, distanceShotgunPlayer);
                            }else
                            {
                                maskedEnemy.running = true;
                            }
                        }
                    }
                }
            }
        }

        void HandleShotgunHeldByPlayer()
        {
            if (!isHoldingObject)
            {
                foreach (GrabbableObject grabbableItem in GlobalItemList.Instance.allitems)
                {
                    if (grabbableItem is Shovel)
                    {
                        float distanceToObject = Vector3.Distance(this.transform.position, grabbableItem.transform.position);

                        if (distanceToObject < Mathf.Infinity && distanceToObject <= 10 && !grabbableItem.isHeld && !grabbableItem.isHeldByEnemy && !notGrabClosestItem)
                        {
                            closestGrabbable = grabbableItem;
                            if (closestGrabbable != null && closestGrabbable.GetComponent<CheckItemCollision>() != null)
                            {
                                itemSystem = closestGrabbable.GetComponent<CheckItemCollision>();
                            }
                            if (!itemSystem.hidedByMasked)
                            {
                                if (IsHost)
                                {
                                    if (distanceToObject > 0.5 )
                                    {
                                        __instance.SetDestinationToPosition(closestGrabbable.transform.position, true);
                                        __instance.moveTowardsDestination = true;
                                        __instance.movingTowardsTargetPlayer = false;
                                    }
                                    else
                                    {
                                        __instance.SetDestinationToPosition(maskedEnemy.mainEntrancePosition, true);
                                        __instance.moveTowardsDestination = false;
                                    }
                                }
                                if (distanceToObject > 0.5f && distanceToObject < 3f)
                                {
                                    maskedEnemy.focusOnPosition = closestGrabbable.transform.position;
                                    maskedEnemy.lookAtPositionTimer = 1.5f;
                                }
                                if (distanceToObject < 0.9f)
                                {
                                    float num = Vector3.Angle(__instance.transform.forward, closestGrabbable.transform.position - __instance.transform.position);
                                    if (closestGrabbable.transform.position.y - maskedEnemy.headTiltTarget.position.y < 0f)
                                    {
                                        num *= -1f;
                                    }
                                    maskedEnemy.verticalLookAngle = num;
                                    closestGrabbable.parentObject = itemHolder.transform;
                                    closestGrabbable.hasHitGround = false;
                                    closestGrabbable.isHeld = true;
                                    closestGrabbable.isHeldByEnemy = true;
                                    closestGrabbable.grabbable = false;
                                    isHoldingObject = true;
                                    itemDroped = false;
                                    closestGrabbable.GrabItemFromEnemy(__instance);
                                }
                                if (distanceToObject < 4f && !isHoldingObject && maskedPersonality != Personality.Aggressive && GameNetworkManager.Instance.isHostingGame)
                                {
                                    isCrouched.Value = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        void HandleShotgunNotHeld()
        {
            notGrabClosestItem = true;
            foreach (GrabbableObject grabbableItem in GlobalItemList.Instance.allitems)
            {
                if (grabbableItem is ShotgunItem)
                {
                    float distanceToObject = Vector3.Distance(this.transform.position, grabbableItem.transform.position);

                    if (distanceToObject < Mathf.Infinity && distanceToObject <= 10 && !grabbableItem.isHeld && !grabbableItem.isHeldByEnemy && !notGrabClosestItem)
                    {
                        closestGrabbable = grabbableItem;
                        if (closestGrabbable != null && closestGrabbable.GetComponent<CheckItemCollision>() != null)
                        {
                            itemSystem = closestGrabbable.GetComponent<CheckItemCollision>();
                        }
                        if (!itemSystem.hidedByMasked)
                        {
                            if (IsHost)
                            {
                                if (distanceToObject > 0.5)
                                {
                                    __instance.SetDestinationToPosition(closestGrabbable.transform.position, true);
                                    __instance.moveTowardsDestination = true;
                                    __instance.movingTowardsTargetPlayer = false;
                                }
                                else
                                {
                                    __instance.SetDestinationToPosition(maskedEnemy.mainEntrancePosition, true);
                                    __instance.moveTowardsDestination = false;
                                }
                            }
                            if (distanceToObject < 3f && isHoldingObject && closestGrabbable is not ShotgunItem)
                            {
                                dropItem.Value = true;
                            }
                            if (distanceToObject > 0.5f && distanceToObject < 3f)
                            {
                                maskedEnemy.focusOnPosition = closestGrabbable.transform.position;
                                maskedEnemy.lookAtPositionTimer = 1.5f;
                            }
                            if (distanceToObject < 0.9f)
                            {
                                float num = Vector3.Angle(__instance.transform.forward, closestGrabbable.transform.position - __instance.transform.position);
                                if (closestGrabbable.transform.position.y - maskedEnemy.headTiltTarget.position.y < 0f)
                                {
                                    num *= -1f;
                                }
                                maskedEnemy.verticalLookAngle = num;
                                closestGrabbable.parentObject = itemHolder.transform;
                                closestGrabbable.hasHitGround = false;
                                closestGrabbable.isHeld = true;
                                closestGrabbable.isHeldByEnemy = true;
                                closestGrabbable.grabbable = false;
                                isHoldingObject = true;
                                itemDroped = false;
                                closestGrabbable.GrabItemFromEnemy(__instance);
                            }
                            if (distanceToObject < 4f && !isHoldingObject && maskedPersonality != Personality.Aggressive)
                            {
                                if (IsHost)
                                {
                                    isCrouched.Value = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}