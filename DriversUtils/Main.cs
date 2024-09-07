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
using HarmonyLib;


namespace Plugin
{


    public class Plugin
    {
        //  public static Plugin Singleton { get; private set; }
        public static Plugin Instance;
        public static Plugin Singleton;
        
        public static bool IsSerpentsSpawning;
        public static bool isScienceTeamSpawning;
        private Player player = null;



        [PluginConfig]
        public Config Config;

        // public static Config CassieSettings;


        private Harmony harmony;





        [PluginEntryPoint("DriversUtils", "1.7.1", "This plugin adds custom features to scpsl.", "itsyourdriver")]
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
            harmony = new Harmony("Patches");
            harmony.PatchAll();

            Log.Debug("Finished loading and initializing DriversUtils!");
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
                Timing.CallDelayed(0.2f, () => // 0.2f
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
                            player.AddItem(ItemType.ArmorCombat);
                            RemoveItem(player, ItemType.ArmorLight);
                            RemoveItem(player, ItemType.GunFSP9);
                            RemoveItem(player, ItemType.KeycardGuard);
                            AddOrDropFirearm(player, ItemType.GunCrossvec, true);
                            player.AddItem(ItemType.KeycardMTFPrivate);
                            //player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = $"<color=#727472>{player.DisplayNickname}</color>" + "\n<color=#727472>FACILITY GUARD CAPTAIN</color>";
                             //player.PlayerInfo.IsRoleHidden = true;
                             //player.PlayerInfo.IsNicknameHidden = true;
                             //player.PlayerInfo.IsUnitNameHidden = true;
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
                        this.player = null;
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
                    Config config = Plugin.Singleton.Config;
                    guard_captain = -1;
                    if (config.Debug == true)
                    {
                        Log.Debug("Reset guard captain as player left.");
                    }
                }
            }
        }
    }
}



