using CommandSystem;
using CustomPlayerEffects;
using GameCore;
using InventorySystem;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Pickups;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
//using Plugin;
//using Plugin.Commands;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Interfaces;
using PluginAPI.Enums;
using PluginAPI.Events;
using PluginAPI.Roles;
using RemoteAdmin.Communication;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using static TheRiptide.Utility;
using Log = PluginAPI.Core.Log;


namespace Plugin
{


    public class TheKid
    {
        //  public static Plugin Singleton { get; private set; }


        private Player player = null;



        static int guard_captain = -1;
        static int attempts = 0;
        public int candytaken = 0;

        // static int randomGlitchSound = new System.Random().Next(30, 150);








        [PluginEvent(ServerEventType.RoundStart)]
        void OnRoundStart()
        {
            Config config = Plugin.Singleton.Config;
            try
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    if (config.Debug == true)
                    {
                        Log.Debug("Picking player...");
                    }

                    if (new System.Random().Next(3) == 1)
                    {

                    
                    List<Player> players = Player.GetPlayers();
                    System.Random random = new System.Random();

                    guard_captain = -1;
                    attempts = 0;
                    while (guard_captain == -1)
                    {

                        int i = random.Next(0, players.Count);
                        player = players[i];
                        if (player.Role == PlayerRoles.RoleTypeId.ClassD)
                        {
                            guard_captain = 0;
                            player = players[i];

                            player.SendBroadcast(config.KidText, 10);
                            SetScale(player, 0.85f);

                            //player.EffectsManager.EnableEffect<Scp559Effect>(99999, true);
                            player.AddItem(ItemType.SCP330);
                            player.AddItem(ItemType.SCP330);
                            player.AddItem(ItemType.SCP330);
                           

                            if (config.Debug == true)
                            {
                                Log.Debug("Finished setting up kid role yippee");
                            }



                                player.CustomInfo = $"<color=#FF9966>{player.DisplayNickname}</color>" + "\n<color=#FF9966>THE KID</color>";

                                //player.DisplayNickname = "Facility Guard Captain | " + player.Nickname;
                                // player.GameObject.transform.localScale = new UnityEngine.Vector3(0.5f, 0.5f, 0.5f);
                                // Log.Info("set player's scale, they may get dcd");
                                break;
                        }
                        else
                        {
                            attempts++;
                            if (attempts >= 30)
                                break;
                        }
                    }
                    }



                });

            }
            catch (Exception e)
            {
              //  Log.Info("ERROR: At round start, setting up guard captain.");
            }
        }





     

        [PluginEvent(ServerEventType.PlayerChangeRole)]
        private void OnPlayerChangeRole(PlayerChangeRoleEvent ev)
        {

        
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
                        player = null;
                        SetScale(player, 1.0f);
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
                Log.Info("hi");
            }
        }


       



        [PluginEvent(ServerEventType.RoundEnd)]
        void OnRoundEnded(RoundSummary.LeadingTeam leadingTeam)
        {
            guard_captain = -1;
            attempts = 0;
            candytaken = 0;
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
                    player.DisplayNickname = null;
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



