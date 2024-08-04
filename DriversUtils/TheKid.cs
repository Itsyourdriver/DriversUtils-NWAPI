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
        static bool hasPrevented = false;
        // static int randomGlitchSound = new System.Random().Next(30, 150);

        Config config = Plugin.Singleton.Config;






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

                    if (new System.Random().Next(4) == 1)
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

                                int RandomNumber = UnityEngine.Random.RandomRange(1, 3);

                            if (RandomNumber == 1 || RandomNumber == 2)
                            {
                                    player.SendBroadcast(config.KidText, 10);
                                    SetScale(player, 0.8f);
                                    player.AddItem(ItemType.SCP330);
                                    player.AddItem(ItemType.SCP330);

                            }
                            else
                            {
                                    player.SendBroadcast($"You are <color={player.ReferenceHub.roleManager.CurrentRole.RoleColor.ToHex()}>The Brute</color>. You are slightly taller and start have damage resistance.", 10);
                                    player.AddItem(ItemType.ArmorHeavy);
                                    player.EffectsManager.EnableEffect<DamageReduction>(0, false);
                                    SetScale(player, 1.2f);
                            }

                            
                           




                            if (config.Debug == true)
                            {
                                Log.Debug("Finished setting up kid role yippee");
                            }


                                /*
                                                                player.CustomInfo = $"<color=#FF9966>{player.Nickname}</color>" + "\n<color=#FF9966>THE KID</color>";
                                                                player.PlayerInfo.IsRoleHidden = true;
                                                                player.PlayerInfo.IsNicknameHidden = true;
                                                                player.PlayerInfo.IsUnitNameHidden = true;
                                */
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
                if (config.Debug == true)
                {
                    Log.Debug($"Error: {e}");
                }
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
                       
                        // player.DisplayNickname = player.Nickname;
                        //  player.SendBroadcast("You were killed by: " + attacker.Nickname, 5);
                        //  player.DisplayNickname = null;
                        this.player = null;
                        // Log.Info("WARNING: Chance to explode the server, ATTEMPTING TO SET NULL TO SOMETHING THAT SHOULD ALREADY BE NULL");
                        // player = null;
                        hasPrevented = false;
                        candytaken = 0;
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
                if (config.Debug == true)
                {
                    Log.Debug($"Error: {e}");
                }
            }
        }

        /*
        [PluginEvent(ServerEventType.PlayerPickupScp330)]
        bool OnPlayerPickupScp330(Player plr, ItemPickupBase pickup)
        {
            Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) pickup scp330 {pickup.Info.ItemId}.");

            if (plr.UserId == player.UserId)
            {
                candytaken = +1;
                return true;
            }
            else
            {
               // return true;
            }

            if (plr.UserId == player.UserId && candytaken == 2 && hasPrevented == false)
            {
                hasPrevented = true;
                plr.AddItem(ItemType.SCP330);
                plr.EffectsManager.DisableEffect<SeveredHands>();
                Timing.CallDelayed(1f, () =>
                {
                    plr.EffectsManager.DisableEffect<SeveredHands>();
                });
                return false;
            }
            else {
                return true;
            }

        }

        
        [PluginEvent(ServerEventType.PlayerReceiveEffect)]
        bool OnReceiveEffect(Player plr, StatusEffectBase effect, byte intensity, float duration)
        {
            //  Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) received effect &6{effect}&r with an intensity of &6{intensity}&r.");
            // && kidCandyPickups >= 2 && kidCandyPickups <= 3
            if (plr.UserId == player.UserId && candytaken == 3 && hasPrevented == false)
            {
                hasPrevented = true;
                Timing.CallDelayed(0.5f, () =>
                {
                    // Log.Debug("Real");
                    plr.EffectsManager.DisableEffect<SeveredHands>();
                    return false;
                });

                plr.EffectsManager.DisableEffect<SeveredHands>();
            } else
            {
                return true;
            }
        }
        */


        [PluginEvent(ServerEventType.RoundEnd)]
        void OnRoundEnded(RoundSummary.LeadingTeam leadingTeam)
        {
            guard_captain = -1;
            attempts = 0;
            candytaken = 0;
            hasPrevented = false;
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
                  //  player.DisplayNickname = null;
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



