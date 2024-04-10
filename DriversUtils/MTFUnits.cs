namespace Plugin
{
    using CommandSystem;
    using CustomPlayerEffects;
    using InventorySystem.Items;
    using MEC;
    using PlayerRoles;
    using PlayerStatsSystem;
    using PluginAPI.Core;
    using PluginAPI.Core.Attributes;
    using PluginAPI.Enums;
    using PluginAPI.Events;
    using Respawning;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Mirror;
    using static TheRiptide.Utility;
    using InventorySystem.Items.Usables;
    using Hazards;
    using RelativePositioning;
    using Random = UnityEngine.Random;

    public class MTFUnits : IComparable
    {
        static int respawn_count = 0;
        static HashSet<int> nu7 = new HashSet<int>();
        static UnityEngine.Vector3 offset = new UnityEngine.Vector3(-40.021f, -8.119f, -36.140f);
        SpawnableTeamType spawning_team = SpawnableTeamType.None;
        static bool IsNu7Spawning = false;
        int scpsleft = 0;
        // kid class stuff :D



        [PluginEvent(ServerEventType.RoundStart)]
        void RoundStarted()
        {
            respawn_count = 0;
            nu7.Clear();
            spawning_team = SpawnableTeamType.None;
            IsNu7Spawning = false;
            scpsleft = 0;
        }



        static bool haveNU7Spawned = false;


        void ChangeToTutorial(Player player, RoleTypeId role)
        {
            // player.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.Escaped, RoleSpawnFlags.None);
           // Config config = Plugin.Singleton.Config;
            player.ReferenceHub.inventory.UserInventory.Items.Clear();
           // player.Role = PlayerRoles.RoleTypeId.Tutorial;
            nu7.Add(player.PlayerId);


            // old code / testing crap
             //player.SendBroadcast("You have spawned as a unit of the MTF NU-7 Faction.", 15, shouldClearPrevious: true);
            // Teleport.RoomPos(player, RoomIdentifier.AllRoomIdentifiers.Where((r) => r.Zone == FacilityZone.Surface).First(), offset);
            // player.ClearInventory();
            // player.AddItem(ItemType.GunRevolver);
            //   player.AddItem(ItemType.GunShotgun);
            //   player.AddAmmo(ItemType.Ammo762x39, 200);

            // insert cod modern warfare juggernaut music here
            //player.SendBroadcast(config.SerpentsHandText, 15);
            player.AddItem(ItemType.ArmorHeavy);
            player.AddItem(ItemType.KeycardMTFCaptain);
            player.AddItem(ItemType.Medkit);
            player.AddItem(ItemType.Adrenaline);
            // player.AddItem(ItemType.SCP1853);
            // player.AddItem(ItemType.AntiSCP207);
           // player.i

            player.AddAmmo(ItemType.Ammo762x39, 120);
            player.AddAmmo(ItemType.Ammo556x45, 100);
            player.AddAmmo(ItemType.Ammo9x19, 120);
            player.AddItem(ItemType.Radio);

            player.Health = 110;
            //  AddOrDropFirearm(player, ItemType.GunShotgun, true);
            // credit to riptide for this code, i didnt feel like doing this system myself for the time being


            switch (UnityEngine.Random.Range(0, 1))
            {
                case 0: AddOrDropFirearm(player, ItemType.GunFRMG0, true); break;
                case 1: AddOrDropFirearm(player, ItemType.GunLogicer, true); break;
            }

            AddOrDropFirearm(player, ItemType.GunCOM18, true);

            // lets do weapons now

           // player.AddAmmo(ItemType.Ammo44cal, 24);
            //AddOrDropFirearm(player, ItemType.GunRevolver, true);

            //   player.AddAmmo(ItemType.Ammo9x19, 20);
            //   player.DisplayNickname = "Serpents Hand | " + player.Nickname;
          //  Player playertoTP = Player.Get(player.ReferenceHub);
            //playertoTP.Position = new UnityEngine.Vector3(0.06f, 1000.96f, 0.33f);
            // might add config for this in the future, dunno yet
            // fyi add +1000 to ur y coord if you wanna tp someone to somewhere on surface, learned that from axwabo. 

        }

        [PluginEvent(ServerEventType.TeamRespawn)]
        void OnRespawnWave(SpawnableTeamType team, List<Player> players, int max)
        {
            //    Log.Info($"Spawned team &6{team}&r");
            spawning_team = team;
            respawn_count++;
            // && new System.Random().Next(2) == 1
            Config config = Plugin.Singleton.Config;
            // thanks to my friend seagull101 for the help with system.random, i still have almost no idea what I am doing lol
            if (respawn_count >= 0 && spawning_team == SpawnableTeamType.NineTailedFox && config.ShouldSerpentsSpawn == true && haveNU7Spawned == false && new System.Random().Next(3) == 1)
            {
                IsNu7Spawning = true;

                //List<Player> Players = Player.GetPlayers();
                
                foreach (var plrr in players)
                {
                 if (plrr.IsSCP)
                    {
                        scpsleft = scpsleft+1;
                        Log.Debug($"SCPS Left: {scpsleft}");
                    }
                }
                
                haveNU7Spawned = true;

                Timing.CallDelayed(0.4f, () =>
                {
                    Cassie.Clear();
                    Cassie.Message($"MTFUnit Nu 7 designated pitch_0.5 .G2 .G3 pitch_1 hasentered . allremaining . AWAITINGRECONTAINMENT {scpsleft} SCPSUBJECTS", true, true, false);
                });


                

                //       }
                //    !player.TemporaryData.Contains("custom_class"))

                Timing.CallDelayed(5f, () =>
                {
                    IsNu7Spawning = false;
                });
            }
        }





        [PluginEvent(ServerEventType.PlayerSpawn)]
        void OnPlayerSpawned(Player player, RoleTypeId role)
        {
            if (respawn_count >= 1 && IsNu7Spawning == true)
            {
                if (spawning_team == SpawnableTeamType.NineTailedFox && role.GetTeam() == Team.FoundationForces && !player.TemporaryData.Contains("custom_class"))
                {

                    // ewww formatting went bye bye, this is probably really inefficient but it seems to fix my original problem where players would infinitely be set to tutorial or have like thousands of each ammo type
                    Timing.CallDelayed(0.1f, () =>
                    {
                        if (player.Role != RoleTypeId.Tutorial)
                        {
                            ChangeToTutorial(player, role);
                        }



                     
                            
                            player.TemporaryData.Add("custom_class", this);
                            // player.SendBroadcast("", 10);
                            //   player.TemporaryData.Add("custom_class", this);
                            //  AddOrDropItem(player, ItemType.KeycardFacilityManager);
                            //   AddOrDropFirearm(player, ItemType.GunCOM15, true);
                            //  }
                            // Log.Debug("Serpents hand spawned.");
                    });
                }
            }

        }


        // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) received effect &6{effect}&r with an intensity of &6{intensity}&r.");


        [PluginEvent(ServerEventType.PlayerChangeRole)]
        void PlayerChangeRole(Player player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
        {
            if (player != null && nu7.Contains(player.PlayerId))
            {
                nu7.Remove(player.PlayerId);
                player.TemporaryData.Remove("custom_class");

               // SetScale(player, 1.0f);
            }

         
        }









        [PluginEvent(ServerEventType.PlayerDeath)]
        void OnPlayerDied(Player player, Player attacker, DamageHandlerBase damageHandler)
        {
            if (player != null && nu7.Contains(player.PlayerId)) // && newRole.GetTeam() != Team.ChaosInsurgency
            {
                nu7.Remove(player.PlayerId);
                player.TemporaryData.Remove("custom_class");



            }
            if (player != null && player.Team == Team.SCPs)
            {
                scpsleft = scpsleft-1;
            }
        }
       

        [PluginEvent(ServerEventType.PlayerLeft)]
        void OnPlayerLeave(Player player)
        {
            if (player != null && nu7.Contains(player.PlayerId))
            {
                //  player.DisplayNickname = player.Nickname;
                nu7.Remove(player.PlayerId);
                player.TemporaryData.Remove("custom_class");
                player.DisplayNickname = null;
            }
            if (player != null && player.Team == Team.SCPs)
            {
                scpsleft = scpsleft - 1;
            }
        }
       


        [PluginEvent(ServerEventType.RoundEnd)]
        void OnRoundEnded(RoundSummary.LeadingTeam leadingTeam)
        {
            haveNU7Spawned = false;
            respawn_count = 0;
            nu7.Clear();
            scpsleft = 0;

        }


        [PluginEvent(ServerEventType.RoundRestart)]
        void OnRoundRestart()
        {
            haveNU7Spawned = false;
            respawn_count = 0;
            nu7.Clear();
            scpsleft = 0;
        }


        



        public int CompareTo(object obj)
        {
            return Comparer<MTFUnits>.Default.Compare(this, obj as MTFUnits);
        }
    }
}
