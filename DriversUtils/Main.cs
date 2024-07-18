using CentralAuth;
using CommandSystem;
using CustomPlayerEffects;
//using DriversUtils;
using GameCore;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Pickups;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
//using Plugin;
//using Plugin.Commands;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Interfaces;
using PluginAPI.Enums;
using PluginAPI.Events;
using RemoteAdmin.Communication;
using SCPSLAudioApi.AudioCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using slocLoader;
using slocLoader.Objects;
using static TheRiptide.Utility;
using Log = PluginAPI.Core.Log;
using RueI;


namespace Plugin
{


    public class Plugin
    {
      //  public static Plugin Singleton { get; private set; }

        public static Plugin Singleton;
        
        private Player player = null;

        [PluginConfig]
        public Config Config;
       // public static Config CassieSettings;
       
        






        [PluginEntryPoint("DriversUtils", "1.0.0", "This plugin adds custom features to scpsl.", "itsyourdriver")]
        public void LoadPlugin()
        {
            if (!Config.IsEnabled)
                return;


//            Log.Info("Loading Item Commands...");
            //    Singleton = this;


            Singleton = this;
            Log.Info("Loading DriversUtils...");
            EventManager.RegisterEvents<Plugin>(this);
            EventManager.RegisterEvents<EventHandlers>(this);
            EventManager.RegisterEvents<MTFUnits>(this);
            EventManager.RegisterEvents<Coin914>(this);
            EventManager.RegisterEvents<TheKid>(this);
            Log.Debug("Finished loading and initializing DriversUtils. Thank you for downloading!");
            RueIMain.EnsureInit();
            //  Log.Debug("RueI Loaded and Initialized");


            
        }


        



        static int guard_captain = -1;
        static int attempts = 0;


        // static int randomGlitchSound = new System.Random().Next(30, 150);



        public ReferenceHub RadioHub;
        public Player Radio;

        [PluginEvent(ServerEventType.RoundStart)]
        void OnRoundStart()
        {
            Config config = Plugin.Singleton.Config;

            try
            {
                Timing.CallDelayed(0.2f, () =>
                {
                    if (config.Debug == true)
                    {
                        Log.Debug("Picking player...");
                    }
                    List<Player> players = Player.GetPlayers();
                    System.Random random = new System.Random();

                    guard_captain = -1;
                    attempts = 0;
                    while (guard_captain == -1)
                    {

                        int i = random.Next(0, players.Count);
                        player = players[i];
                        if (player.Role == PlayerRoles.RoleTypeId.FacilityGuard)
                        {
                            guard_captain = 0;
                            player = players[i];

                            player.SendBroadcast(config.GuardText, 10);
                            //  player.DisplayNickname = "Guard Captain | " + player.Nickname;
                            player.ReferenceHub.inventory.UserInventory.Items.Clear();
                            //player.AddItem(ItemType.GunE11SR
                            player.AddItem(ItemType.ArmorCombat);
                            AddOrDropFirearm(player, ItemType.GunCrossvec, true);
                           // player.AddAmmo(ItemType.Ammo556x45, 80);
                            player.AddAmmo(ItemType.Ammo9x19, 39); // funny number, doesnt look like it but it is
                            player.AddItem(ItemType.KeycardMTFPrivate);
                            player.AddItem(ItemType.GrenadeFlash);
                            player.AddItem(ItemType.Medkit);
                        //    player.AddItem(ItemType.GrenadeHE);
                           
                            Timing.CallDelayed(0.2f, () =>
                            {
                                player.AddItem(ItemType.Radio);
                            });
                            //player.DisplayNickname = "Facility Guard Captain | " + player.Nickname;
                         //   player.CustomInfo = $"<color=#727472>{player.DisplayNickname}</color>" + "\n<color=#727472>FACILITY GUARD CAPTAIN</color>";
                          //  player.PlayerInfo.IsRoleHidden = true;
                           // player.PlayerInfo.IsNicknameHidden = true;
                          //  player.PlayerInfo.IsUnitNameHidden = true;
                            // player.GameObject.transform.localScale = new UnityEngine.Vector3(0.5f, 0.5f, 0.5f);
                            // Log.Info("set player's scale, they may get dcd");
                            if (config.Debug == true)
                            {
                                Log.Debug("Finished setting up guard captain.");
                            }
                            break;
                        }
                        else
                        {
                            attempts++;
                            if (attempts >= 30)
                                break;
                        }
                    }

                });
                UnityEngine.Vector3 offset = new UnityEngine.Vector3(-40.021f, -8.119f, -36.140f);
                Timing.CallDelayed(7f, () =>
                {
                  //  slocLoader.API.SpawnObjectsFromFile("C:/Users/defin/SLoc/test.sloc",, offset, Quaternion.Euler(0, 0, 0));
                  //  slocLoader.API.AddTriggerAction()
                    //  ObjectsSource.From()
                   // ObjectsSource obj = ObjectsSource.FromFile("C:/object");


                   // obj.AddTriggerAction(data, customHandler)
                  // slocObjectData.FindObjectsOfType)
                });


            }
            catch (Exception e)
            {
                Log.Warning(e.ToString());
            }
        }







        [PluginEvent(PluginAPI.Enums.ServerEventType.PlayerDeath)]
        private void PlayerDead(Player player, Player attacker, DamageHandlerBase damageHandler)
        {
            try
            {
                if (this.player != null)
                {
                    if (player.UserId == this.player.UserId)
                    {
                        Config config = Plugin.Singleton.Config;
                        // player.DisplayNickname = player.Nickname;
                        //  player.SendBroadcast("You were killed by: " + attacker.Nickname, 5);
                        //  player.DisplayNickname = null;
                        this.player = null;
                        // Log.Info("WARNING: Chance to explode the server, ATTEMPTING TO SET NULL TO SOMETHING THAT SHOULD ALREADY BE NULL");
                      //  player = null;
                        guard_captain = -1;
                        if (config.Debug == true)
                        {
                            Log.Debug("Reset Guard Captain stats for next round :)");
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Log.Warning(e.ToString());
            }
        }






        [PluginEvent(ServerEventType.RoundEnd)]
        void OnRoundEnded(RoundSummary.LeadingTeam leadingTeam)
        {
            guard_captain = -1;
            attempts = 0;
            Config config = Plugin.Singleton.Config;
            if (config.Debug == true)
            {
                Log.Debug("Reset guard captain as round ended.");
                Log.Debug($"Round ended. {leadingTeam.ToString()} won!");
            }
        }


        [PluginEvent(ServerEventType.PlayerLeft)]
        void OnPlayerLeave(Player player)
        {
            if (this.player != null)
            {
                if (player.UserId == this.player.UserId)
                {
                    //  player.DisplayNickname = player.Nickname;
                    Config config = Plugin.Singleton.Config;
                   // player.DisplayNickname = null;
                    guard_captain = -1;
                    //Log.Info("Player left");

                    if (config.Debug == true)
                    {
                        Log.Debug("Reset guard captain as player left.");
                    }
                }
            }
        }
    }
}



