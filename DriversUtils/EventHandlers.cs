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
    using PluginAPI.Core.Interfaces;
    using PlayerRoles.PlayableScps.Scp939;
    using InventorySystem.Items.Pickups;
    using RemoteAdmin.Communication;
    using MapGeneration;
    using Interactables.Interobjects.DoorUtils;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.Scp106;
    using CommandSystem.Commands.RemoteAdmin;
    using PlayerRoles.PlayableScps.Scp079;
    using PluginAPI.Roles;
    using PlayerRoles.PlayableScps.Scp3114;
    using Hints;
    using InventorySystem.Items.Coin;
    using System.Data;
    using System.Numerics;

    public class EventHandlers : IComparable
    {
        int respawn_count = 0;
        HashSet<int> fbi = new HashSet<int>();
        static UnityEngine.Vector3 offset = new UnityEngine.Vector3(-40.021f, -8.119f, -36.140f);
        SpawnableTeamType spawning_team = SpawnableTeamType.None;
        static bool isSerpentSpawning = false;
        static bool isFlamingosSpawning = false;
        bool doWeHaveAnAlpha = false;





        [PluginEvent(ServerEventType.RoundStart)]
        void RoundStarted()
        {
            respawn_count = 0;
            fbi.Clear();
            spawning_team = SpawnableTeamType.None;
            isSerpentSpawning = false;
            doWeHaveAnAlpha = false;



        }



        static bool haveSerpentsSpawned = false;
        static bool haveFlamingosSpawned = false;




        void ChangeToTutorial(Player player, RoleTypeId role)
        {
            // player.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.Escaped, RoleSpawnFlags.None);
            Config config = Plugin.Singleton.Config;
            player.ReferenceHub.inventory.UserInventory.Items.Clear();
            player.Role = PlayerRoles.RoleTypeId.Tutorial;
            fbi.Add(player.PlayerId);


            // old code / testing crap
            //  player.SendBroadcast("", 15, shouldClearPrevious: true);
            // Teleport.RoomPos(player, RoomIdentifier.AllRoomIdentifiers.Where((r) => r.Zone == FacilityZone.Surface).First(), offset);
            // player.ClearInventory();
            // player.AddItem(ItemType.GunRevolver);
            //   player.AddItem(ItemType.GunShotgun);
            //   player.AddAmmo(ItemType.Ammo762x39, 200);


            // insert cod modern warfare juggernaut music here
            player.SendBroadcast(config.SerpentsHandText, 15);
            player.AddItem(ItemType.ArmorCombat);
            player.AddItem(ItemType.KeycardChaosInsurgency);
            player.AddItem(ItemType.Medkit);
            player.AddItem(ItemType.Adrenaline);
            // player.AddItem(ItemType.SCP1853);
            // player.AddItem(ItemType.AntiSCP207);

            player.AddAmmo(ItemType.Ammo12gauge, 40);
            AddOrDropFirearm(player, ItemType.GunShotgun, true);
            // credit to riptide for this code, i didnt feel like doing this system myself for the time being


            switch (UnityEngine.Random.Range(0, 8))
            {
                case 0: AddOrDropItem(player, ItemType.SCP2176); break;
                case 1: AddOrDropItem(player, ItemType.SCP500); break;
                case 2: AddOrDropItem(player, ItemType.SCP1853); break;
                case 3: AddOrDropItem(player, ItemType.SCP207); break;
                case 4: AddOrDropItem(player, ItemType.SCP018); break;
                case 5: AddOrDropItem(player, ItemType.SCP268); break;
                case 6: AddOrDropItem(player, ItemType.SCP244a); break;
                case 7: AddOrDropItem(player, ItemType.AntiSCP207); break;
                case 8: AddOrDropItem(player, ItemType.SCP244b); break;
            }



            // lets do weapons now

            player.AddAmmo(ItemType.Ammo44cal, 24);
            AddOrDropFirearm(player, ItemType.GunRevolver, true);

            //   player.AddAmmo(ItemType.Ammo9x19, 20);
            //   player.DisplayNickname = "Serpents Hand | " + player.Nickname;
            Player playertoTP = Player.Get(player.ReferenceHub);
            playertoTP.Position = new UnityEngine.Vector3(0.06f, 1000.96f, 0.33f);
            player.CustomInfo = $"<color=#FF96DE>{player.DisplayNickname}</color>" + "\n<color=#FF96DE>SERPENTS HAND</color>";
            // might add config for this in the future, dunno yet
            // fyi add +1000 to ur y coord if you wanna tp someone to somewhere on surface, learned that from axwabo. 

        }


        public bool waswave1mtf = false;
        public bool waswave2mtf = false;

        [PluginEvent(ServerEventType.TeamRespawn)]
        void OnRespawnWave(SpawnableTeamType team, List<Player> players, int max)
        {
            //    Log.Info($"Spawned team &6{team}&r");
            spawning_team = team;
            respawn_count++;
            // && new System.Random().Next(2) == 1
            Config config = Plugin.Singleton.Config;
            // thanks to my friend seagull101 for the help with system.random, i still have almost no idea what I am doing lol
            if (respawn_count == 1 && spawning_team == SpawnableTeamType.NineTailedFox)
            {
                waswave1mtf = true;
            }

            if (respawn_count == 2 && spawning_team == SpawnableTeamType.NineTailedFox)
            {
                waswave2mtf = true;

                Timing.CallDelayed(2f, () =>
                {
                    RespawnTokensManager.ForceTeamDominance(SpawnableTeamType.ChaosInsurgency, 85);
                });


            }


            if (respawn_count >= 2 && spawning_team == SpawnableTeamType.ChaosInsurgency && config.ShouldSerpentsSpawn == true && new System.Random().Next(4) == 1)
            {
                int randn = Random.RandomRange(1, 4);
                //int randn = 2;
                if (randn == 2 || randn == 3 || randn == 4 || randn == 1)
                {
                    if (haveSerpentsSpawned == false) {
                        isSerpentSpawning = true;

                        if (config.ShouldSerpentsHandSpawnMore == false)
                        {
                            haveSerpentsSpawned = true;
                        }
                        //  if (UnityEngine.Random.value < 0.10 && spawning_team == SpawnableTeamType.ChaosInsurgency)
                        //  {
                        if (config.ShouldCassie == true)
                        {
                            Cassie.Message(config.CassieMessage, true, config.CassieNoise, config.CassieText);
                        }



                        //       }
                        //    !player.TemporaryData.Contains("custom_class"))

                        Timing.CallDelayed(2f, () =>
                        {
                            isSerpentSpawning = false;
                            isFlamingosSpawning = false;
                        });
                    }
                }
                else

                if (config.CanFlamingosSpawn == true && randn == 69)
                {
                    isFlamingosSpawning = true;

                    if (config.ShouldSerpentsHandSpawnMore == false)
                    {
                        haveFlamingosSpawned = true;
                    }


                    //  if (UnityEngine.Random.value < 0.10 && spawning_team == SpawnableTeamType.ChaosInsurgency)
                    //  {
                    if (config.ShouldCassie == true)
                    {
                        Cassie.Message(config.FlamingoCassieMessage, true, config.CassieNoise, config.CassieText);
                    }



                    //       }
                    //    !player.TemporaryData.Contains("custom_class"))

                    Timing.CallDelayed(2f, () =>
                    {
                        isFlamingosSpawning = false;
                        isSerpentSpawning = false;
                    });
                }



            }
        }





        [PluginEvent(ServerEventType.PlayerSpawn)]
        void OnPlayerSpawned(Player player, RoleTypeId role)
        {
            


            if (respawn_count >= 2 && isSerpentSpawning == true)
            {
                if (spawning_team == SpawnableTeamType.ChaosInsurgency && role.GetTeam() == Team.ChaosInsurgency && !player.TemporaryData.Contains("custom_class"))
                {

                    // ewww formatting went bye bye, this is probably really inefficient but it seems to fix my original problem where players would infinitely be set to tutorial or have like thousands of each ammo type
                    Timing.CallDelayed(0.1f, () =>
                    {
                        if (player.Role != RoleTypeId.Tutorial)
                        {
                            ChangeToTutorial(player, role);
                        }



                        if (player.Role == RoleTypeId.Tutorial)
                        {

                            player.TemporaryData.Add("custom_class", this);
                            // player.SendBroadcast("", 10);
                            //   player.TemporaryData.Add("custom_class", this);
                            //  AddOrDropItem(player, ItemType.KeycardFacilityManager);
                            //   AddOrDropFirearm(player, ItemType.GunCOM15, true);
                            //  }
                            // Log.Debug("Serpents hand spawned.");
                        }
                    });
                }
            }

            if (respawn_count >= 2 && isFlamingosSpawning == true && haveFlamingosSpawned == false)
            {
                if (spawning_team == SpawnableTeamType.ChaosInsurgency && role.GetTeam() == Team.ChaosInsurgency)
                {

                    // ewww formatting went bye bye, this is probably really inefficient but it seems to fix my original problem where players would infinitely be set to tutorial or have like thousands of each ammo type
                    Timing.CallDelayed(0.1f, () =>
                    {
                        if (doWeHaveAnAlpha == true)
                        {
                            // player.Role = RoleTypeId.Flamingo;
                            Player playertoTP = Player.Get(player.ReferenceHub);
                            playertoTP.Position = new UnityEngine.Vector3(0.06f, 1000.96f, 0.33f);

                        }
                        if (doWeHaveAnAlpha == false)
                        {
                            // player.Role = RoleTypeId.AlphaFlamingo;
                            Player localplayertoTP = Player.Get(player.ReferenceHub);
                            localplayertoTP.Position = new UnityEngine.Vector3(0.06f, 1000.96f, 0.33f);
                            doWeHaveAnAlpha = true;

                        }

                    });
                }
            }

            


            if (role == RoleTypeId.Scp079)
            {
                Timing.CallDelayed(3f, () =>
                {
                    player.ReceiveHint("ComputerBuff: You have bonus abilities availible. Open your console (press ~ to open it) and run the command .pcbuff for more info.", 10f);
                });
            }


        }


        // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) received effect &6{effect}&r with an intensity of &6{intensity}&r.");

      //  public bool doesSubclassMTFexist = false; 
        [PluginEvent(ServerEventType.PlayerChangeRole)]
        void PlayerChangeRole(Player player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
        {

            if (player != null && newRole == RoleTypeId.NtfSergeant)
            {
               
               if ((Random.Range(1, 10) == 4))
               {

//                    doesSubclassMTFexist = true;
                    Timing.CallDelayed(0.1f, () =>
                    {
                        player.SendBroadcast("You are an MTF Boom Boom Boy. You have access to EXPLOSIVES!", 10);
                        player.AddItem(ItemType.GrenadeHE);
                        player.AddItem(ItemType.GrenadeHE);
                        player.CustomInfo = $"<color=#00B7EB>{player.DisplayNickname}</color>" + "\n<color=#00B7EB>Nine Tailed Fox Boom Boom Boy</color>";
                       
                       player.ReceiveHint(player.CustomInfo, 10);
                    });


               }
            }



            if (player != null && newRole == RoleTypeId.ChaosRifleman)
            {

                if ((Random.Range(1, 10) == 4))
                {


                    Timing.CallDelayed(0.1f, () =>
                    {
                        player.SendBroadcast("You are an Chaos Specialist. You have access to ???.", 10);

                        player.CustomInfo = $"<color=#228B22>{player.DisplayNickname}</color>" + "\n<color=#228B22>CHAOS SPECIALIST</color>";

                        switch (UnityEngine.Random.Range(0, 8))
                        {
                            case 0: AddOrDropItem(player, ItemType.SCP2176); break;
                            case 1: AddOrDropItem(player, ItemType.SCP500); break;
                            case 2: AddOrDropItem(player, ItemType.SCP1853); break;
                            case 3: AddOrDropItem(player, ItemType.SCP207); break;
                            case 4: AddOrDropItem(player, ItemType.SCP018); break;
                            case 5: AddOrDropItem(player, ItemType.SCP268); break;
                            case 6: AddOrDropItem(player, ItemType.SCP244a); break;
                            case 7: AddOrDropItem(player, ItemType.AntiSCP207); break;
                            case 8: AddOrDropItem(player, ItemType.SCP244b); break;
                        }

                        player.ReceiveHint(player.CustomInfo, 10);
                    });


                }
            }

            if (player != null && fbi.Contains(player.PlayerId))
            {
                fbi.Remove(player.PlayerId);
                player.TemporaryData.Remove("custom_class");
            }

            if (player != null && newRole == RoleTypeId.Scp0492)
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    SetScale(player, UnityEngine.Random.Range(0.7f, 1.2f));

                    if (UnityEngine.Random.Range(1, 4) == 1)
                    {
                        player.EffectsManager.EnableEffect<MovementBoost>(60f, true);
                    }
                    Timing.CallDelayed(1f, () =>
                    {
                        // player.Health = UnityEngine.Random.Range(400f, 450f);

                    });
                });
            }

            
            }

        [PluginEvent(ServerEventType.PlayerDeath)]
        void OnPlayerDied(Player player, Player attacker, DamageHandlerBase damageHandler)
        {
            if (player != null && fbi.Contains(player.PlayerId)) // && newRole.GetTeam() != Team.ChaosInsurgency
            {
                fbi.Remove(player.PlayerId);
                player.TemporaryData.Remove("custom_class");



            }
        }
        [PluginEvent(ServerEventType.Scp173SnapPlayer)]
        public bool OnScp173SnapPlayer(Player player, Player target)
        {
            if (target != null && fbi.Contains(target.PlayerId)) // && newRole.GetTeam() != Team.ChaosInsurgency
            {
                //fbi.Remove(player.PlayerId);
                //player.TemporaryData.Remove("custom_class");
                // Log.Info($"Player &6{player.Nickname}&r (&6{player.UserId}&r) playing as SCP-173 killed &6{target.Nickname}&r (&6{target.UserId}&r) by snapping his neck");
                return false;
            }
            return true;

        }





        [PluginEvent(ServerEventType.PlayerLeft)]
        void OnPlayerLeave(Player player)
        {
            if (player != null && fbi.Contains(player.PlayerId))
            {
                //  player.DisplayNickname = player.Nickname;
                fbi.Remove(player.PlayerId);
                player.TemporaryData.Remove("custom_class");
                player.DisplayNickname = null;
            }
        }

        [PluginEvent(ServerEventType.Scp939Attack)]
        public bool OnScp939Attack(Player player, IDestructible target)
        {
            if (!ReferenceHub.TryGetHubNetID(target.NetworkId, out ReferenceHub hub))
            {
                return false;
            }

            Player targetPlayer = Player.Get<Player>(hub);

            if (player != null && fbi.Contains(player.PlayerId)) // && newRole.GetTeam() != Team.ChaosInsurgency
            {
                //fbi.Remove(player.PlayerId);
                //player.TemporaryData.Remove("custom_class");
                // Log.Info($"Player &6{player.Nickname}&r (&6{player.UserId}&r) playing as SCP-173 killed &6{target.Nickname}&r (&6{target.UserId}&r) by snapping his neck");
                return true;
            }
            return true;
            // Log.Info($"Player &6{player.Nickname}&r (&6{player.UserId}&r) playing as SCP-939 attacked &6{targetPlayer.Nickname}&r (&6{targetPlayer.UserId}&r)!");
        }







        [PluginEvent(ServerEventType.Scp096AddingTarget)]
        public bool New096Target(Scp096AddingTargetEvent args)
        {
            if (fbi.Contains(args.Target.PlayerId))
            {
                return false;
            }
            else return true;
        }

        [PluginEvent(ServerEventType.Scp173NewObserver)]
        public bool New173Target(Scp173NewObserverEvent args)
        {
            if (fbi.Contains(args.Target.PlayerId))
            {
                return false;
            }
            else return true;
        }


        [PluginEvent(ServerEventType.LczDecontaminationStart)]
        void OnLczDecontaminationStarts()
        {
            //Config config = Plugin.Singleton.Config;
            Log.Debug("Started LCZ decontamination.");
            if (new System.Random().Next(5) == 1)
            {
                Timing.CallDelayed(9.5f, () =>
                {
                    Cassie.Message("SCP 9 9 9 Lost in Decontamination Sequence .G4", true, true, false);
                });
            }

        }


        [PluginEvent(ServerEventType.RoundEnd)]
        void OnRoundEnded(RoundSummary.LeadingTeam leadingTeam)
        {
            haveSerpentsSpawned = false;
            respawn_count = 0;
            fbi.Clear();


        }


        [PluginEvent(ServerEventType.RoundRestart)]
        void OnRoundRestart()
        {
            haveSerpentsSpawned = false;
            respawn_count = 0;
            fbi.Clear();
        }





        [PluginEvent(ServerEventType.PlayerDamage)]
        internal bool OnPlayerDamage(PlayerDamageEvent ev)
        {
            try
            {


                if (ev.Target is null)
                    return true;
                if (ev.DamageHandler is null)
                    return true;
                if (ev.DamageHandler is FirearmDamageHandler fdh && ev.Target.IsSCP == true && fbi.Contains(ev.Player.PlayerId))
                    return false;
                if (ev.DamageHandler is JailbirdDamageHandler jdh && ev.Target.IsSCP == true && fbi.Contains(ev.Player.PlayerId))
                    return false;
                if (ev.DamageHandler is ExplosionDamageHandler gdh && ev.Target.IsSCP == true && fbi.Contains(ev.Player.PlayerId))
                    return false;
                if (ev.DamageHandler is DisruptorDamageHandler ddh && ev.Target.IsSCP == true && fbi.Contains(ev.Player.PlayerId))
                    return false;
                if (ev.DamageHandler is MicroHidDamageHandler mhd && ev.Target.IsSCP == true && fbi.Contains(ev.Player.PlayerId))
                    return false;
                if (ev.DamageHandler is Scp939DamageHandler sc939dh && ev.Player.IsSCP == true && fbi.Contains(ev.Target.PlayerId))
                    return false;
                if (ev.DamageHandler is Scp049DamageHandler sc049dh && ev.Player.IsSCP == true && fbi.Contains(ev.Target.PlayerId))
                    return false;
                if (ev.DamageHandler is Scp3114DamageHandler scp3114dh && ev.Player.IsSCP == true && fbi.Contains(ev.Target.PlayerId))
                    return false;
                if (fbi.Contains(ev.Player.PlayerId) && ev.Player.IsTutorial && ev.Target.IsSCP == true)
                    return false;
                if (ev.Player.IsSCP == true && ev.Target.IsTutorial == true && fbi.Contains(ev.Target.PlayerId))
                    return false;

                return true;
            }
            catch (Exception e)
            {
                // Log.Error($"An error has occured while catching damage handlers.");
                //  Log.Debug($"Error: {e}");
                return true;
            }
        }






        //   [PluginEvent(ServerEventType.PlayerDamage)]
        //  public void OnPlayerDamage(Player player, Player attacker, DamageHandlerBase handler)
        //   {
        //       if (attacker == null)
        //        Log.Info($"Player &6{player.Nickname}&r (&6{player.UserId}&r) got damaged, cause {handler}.");
        //     else
        //        Log.Info($"Player &6{player.Nickname}&r (&6{player.UserId}&r) received damage from &6{attacker.Nickname}&r (&6{attacker.UserId}&r), cause {handler}.");
        //

        //    if (attacker.Role == RoleTypeId.Scp106)
        //    {
        //        player.EffectsManager.DisableEffect<Traumatized>();
        //        player.EffectsManager.DisableEffect<PocketCorroding>();
        //    }
        //  else if (attacker.Role == RoleTypeId.Scp096 && damageHandler is UniversalDamageHandler udh && udh.TranslationId == DeathTranslations.Scp096.Id)
        //   {
        // player.EffectsManager.DisableEffect<Traumatized>();
        //    return false;
        //    }
        //   else if (attacker.Role == RoleTypeId.Scp939 && damageHandler is UniversalDamageHandler udh2 && udh2.TranslationId == DeathTranslations.Scp939Lunge.Id)
        //  {
        // player.EffectsManager.DisableEffect<Traumatized>();
        //
        //   }
        //   }
        // }


        // cola ushort lists!

        public static HashSet<ushort> colas_oxygen = new HashSet<ushort>();
        public static HashSet<ushort> colas_speed = new HashSet<ushort>();
        public static HashSet<ushort> colas_water = new HashSet<ushort>();
        public static HashSet<ushort> colas_coffee = new HashSet<ushort>();
        public static HashSet<ushort> colas_atomkick = new HashSet<ushort>();
        public static HashSet<ushort> colas_nuclearkick = new HashSet<ushort>();
        public static HashSet<ushort> colas_invis = new HashSet<ushort>();
        public static HashSet<ushort> colas_me = new HashSet<ushort>();
        public static HashSet<ushort> colas_tea = new HashSet<ushort>();
        public static HashSet<ushort> colas_horror = new HashSet<ushort>();
        public static HashSet<ushort> colas_borgor = new HashSet<ushort>();
        public static HashSet<ushort> colas_antimatter = new HashSet<ushort>();
        public static HashSet<ushort> colas_zombie = new HashSet<ushort>();
        public static HashSet<ushort> colas_cherryatomkick = new HashSet<ushort>();
        public static HashSet<ushort> colas_explosion = new HashSet<ushort>();
        public static HashSet<ushort> colas_saltwater = new HashSet<ushort>();
        public static HashSet<ushort> colas_peanut = new HashSet<ushort>();
        public static HashSet<ushort> colas_gas = new HashSet<ushort>();
        public static HashSet<ushort> colas_teleportation = new HashSet<ushort>();
        public static HashSet<ushort> scp1499 = new HashSet<ushort>();
        public static HashSet<ushort> colas_medusa = new HashSet<ushort>();
        public static HashSet<ushort> colas_windex = new HashSet<ushort>();
        public static HashSet<ushort> colas_crazy = new HashSet<ushort>();
        public static HashSet<ushort> hats = new HashSet<ushort>();
        public static HashSet<ushort> colas_small = new HashSet<ushort>();
        public static HashSet<ushort> colas_bepis = new HashSet<ushort>();
        public static HashSet<ushort> colas_leaflover = new HashSet<ushort>();
        public static HashSet<ushort> colas_scp207 = new HashSet<ushort>();
        public static HashSet<ushort> colas_flamingo = new HashSet<ushort>();
        public static HashSet<ushort> colas_big = new HashSet<ushort>();
        public static HashSet<ushort> colas_sour_patch_kids_slushy = new HashSet<ushort>();
        public static HashSet<ushort> colas_ghost = new HashSet<ushort>();
        public static HashSet<ushort> colas_cold = new HashSet<ushort>();
        public static HashSet<ushort> colas_metal = new HashSet<ushort>();
        public static HashSet<ushort> colas_oil = new HashSet<ushort>();
        public static HashSet<ushort> colas_alpha = new HashSet<ushort>();
        public static HashSet<ushort> colas_quantam = new HashSet<ushort>();



        [CommandHandler(typeof(ClientCommandHandler))]
        public sealed class scp294 : ICommand
        {
            public string Command { get; } = "scp294";

            public string[] Aliases { get; } = new string[] { "294", "coffeemachine", "scp-294" };

            public string Description { get; } = "scp 294 command real";


            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                // var list = new List<string> { "deeznuts", "oxygen", "water", "uranium235" };
                Player player = Player.Get(((CommandSender)sender).SenderId);

                if (arguments.Count != 0)
                {
                    if (arguments.First().ToLower() == "list" || arguments.First().ToLower() == "help" || arguments.First().ToLower() == "drinks")
                    {
                        player.SendConsoleMessage("List of SCP-294 Drinks: oxygen, speed, SCP-207, Coffee, Espresso, GoldenAtomKick, NuclearKick, godmode, nuclearkick, Invisibility, scp268, Me, Tea, Horror, PocketDimension, Borgor, Cheeserburger, Antimatter, Nuke, 049, Zombie, CherryAtomKick, HealthPotion, grenade, pinkcandy, Boom, SCP-173, Peanut, Saltwater, Ocean, Teleportation, Teleport, Windex, Medusa, SCP-330, Candy, SeveredHands, BEPIS, Small, Big, grow, LeafLover, Water, Slushy, Ghost, Cold, Ice, Death, Metal, Steel, RazorBlade, Oil, Bose-Einstein, Condensate, Quantum Gas");
                        player.SendConsoleMessage("DUPLICATE ENTRIES ARE INCLUDED. SOME MAY BE CASE-SENSITIVE; MAKE SURE TO DOUBLE CHECK CAPS / LOWERCASE.");
                    }
                    if (ItemType.Coin.Equals(player.ReferenceHub.inventory.NetworkCurItem.TypeId) && player.Room.name == "EZ_Smallrooms2" || player.Room.name == "LCZ_TCross (11)")
                    {
                        // response = " Success, you gave your coin for: ";
                        // problem if statement, wants me to stop comparing a string to a system.predicate string I'm probably stupid but yeah  if (arguments.First() == list.Find("deeznuts"))
                        //  {

                        //}    
                        if (arguments.First().ToLower() == "alphaflamingo" || arguments.First().ToLower() == "alpha")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a cup of (VERY LOUD FLAMINGO BATTLE CRY).", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            colas_alpha.Add(thiscola.ItemSerial);

                        }
                        if (arguments.First().ToLower() == "oil")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a can of oil.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            colas_oil.Add(thiscola.ItemSerial);

                        }
                        if (arguments.First().ToLower() == "metal" || arguments.First().ToLower() == "steel" || arguments.First().ToLower() == "razorblade")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a cup of metal.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            colas_metal.Add(thiscola.ItemSerial);

                        }
                        if (arguments.First().ToLower() == "cold" || arguments.First().ToLower() == "ice")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a cup of ice.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.AntiSCP207);

                            colas_cold.Add(thiscola.ItemSerial);

                        }
                        if (arguments.First() == "Ghost" || arguments.First() == "ghost")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a cup of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            colas_ghost.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "oxygen" || arguments.First() == "Oxygen")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a cup of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            colas_oxygen.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "Speed" || arguments.First() == "speed")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise pitched up to high levels and dispensed you a cup of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            colas_speed.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "Coffee" || arguments.First() == "Espresso" || arguments.First() == "coffee" || arguments.First() == "espresso")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a cup of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_coffee.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "GoldenAtomKick" || arguments.First() == "goldenatomkick" || arguments.First() == "goldatom" || arguments.First() == "goldenatomkick")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a glittering sound and dispensed you a can of Golden Atom Kick.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.AntiSCP207);
                            colas_atomkick.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First().ToLower() == "god" || arguments.First() == "NuclearKick" || arguments.First() == "godmode" || arguments.First() == "nuclearkick")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a small noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_nuclearkick.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "Invisibility" || arguments.First() == "invis" || arguments.First() == "scp268" || arguments.First() == "invisibility")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a noise of fabric being cut and dispensed you a cup of SCP-268.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_invis.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "Me" || arguments.First() == "Myself" || arguments.First() == "me" || arguments.First() == "I" || arguments.First().ToLower() == "blood")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud rumbling noise and dispensed you a cup of yourself.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_me.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "Tea" || arguments.First() == "tea" || arguments.First() == "teadrink" || arguments.First() == "t")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a cup of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_tea.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "Horror" || arguments.First() == "horror" || arguments.First() == "scp106" || arguments.First() == "PocketDimension")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a crunchy noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_horror.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "Borgor" || arguments.First() == "borgor" || arguments.First() == "Cheeseburger" || arguments.First() == "cheseburger")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_borgor.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "antimatter" || arguments.First() == "Antimatter" || arguments.First() == "Nuke" || arguments.First() == "nuke" || arguments.First().ToLower() == "death")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_antimatter.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "049" || arguments.First() == "049-2" || arguments.First() == "Zombie" || arguments.First() == "zombie" || arguments.First() == "LeafLover" || arguments.First() == "leaflover" || arguments.First() == "Leaflover")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_zombie.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "CherryAtomKick" || arguments.First() == "cherryatomkick" || arguments.First() == "CherryatomKick" || arguments.First() == "atomkickcherry" || arguments.First() == "HealthPotion" || arguments.First() == "healthpotion" || arguments.First() == "potion" || arguments.First() == "Potion")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_cherryatomkick.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "grenade" || arguments.First() == "Grenade" || arguments.First() == "boom" || arguments.First() == "Pinkcandy" || arguments.First() == "Boom" || arguments.First() == "pinkcandy")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.AntiSCP207);
                            colas_explosion.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "SCP-173" || arguments.First() == "scp173" || arguments.First() == "Peanut" || arguments.First() == "peanut" || arguments.First() == "173" || arguments.First() == "ocean")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a crunchy noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_peanut.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "Saltwater" || arguments.First() == "saltwater" || arguments.First() == "SaltWater" || arguments.First() == "salt" || arguments.First() == "Ocean" || arguments.First() == "ocean")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_saltwater.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "Gasoline" || arguments.First() == "gas" || arguments.First() == "Petrol" || arguments.First() == "pterol" || arguments.First() == "gasoline" || arguments.First() == "Gascan")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_gas.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "teleportation" || arguments.First() == "teleport" || arguments.First() == "Random" || arguments.First() == "Teleport" || arguments.First() == "Teleportation" || arguments.First() == "TP")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_teleportation.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "Windex" || arguments.First() == "windex" || arguments.First() == "wind" || arguments.First() == "cleaningsupplies")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a glimmering noise and dispensed you a bottle of Windex.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_windex.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "Medusa" || arguments.First() == "medusa" || arguments.First() == "Rock" || arguments.First() == "rock" || arguments.First() == "Tank" || arguments.First() == "tank")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a glimmering noise and dispensed you a bottle of Medusa.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_medusa.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "Slushy" || arguments.First() == "slushy" || arguments.First() == "slush" || arguments.First() == "Slush")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a glimmering noise and dispensed you a bottle of Sour Patch Kids Slushy.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_sour_patch_kids_slushy.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "Crazy" || arguments.First() == "crazy" || arguments.First() == "Crazy?" || arguments.First() == "rubberroom" || arguments.First() == "arubberroom" || arguments.First() == "Iwascrazyonce")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine began to say Crazy? I was crazy once, they locked me in a room, a rubber room, a rubber room with rats, and rats make me crazy. In a robotic voice and dispensed you a drink.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_crazy.Add(thiscola.ItemSerial);
                        }
                        else if (arguments.First() == "Small" || arguments.First() == "small" || arguments.First() == "smol" || arguments.First() == "Tiny" || arguments.First() == "tiny")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a high-pitched noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_small.Add(thiscola.ItemSerial);
                        }
                        else if (arguments.First() == "Bepis" || arguments.First() == "bepis" || arguments.First() == "BEPIS")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a odd noise and dispensed you a can of Bepis.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_bepis.Add(thiscola.ItemSerial);
                        }
                        else if (arguments.First() == "SCP-207" || arguments.First() == "scp207" || arguments.First() == "207" || arguments.First() == "cola" || arguments.First() == "Cola")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a high-pitched noise and dispensed you a bottle of cola.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_scp207.Add(thiscola.ItemSerial);
                        }
                        else if (arguments.First() == "water" || arguments.First() == "Water" || arguments.First() == "h2o")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_water.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "flamingo" || arguments.First() == "Flamingo" || arguments.First() == "1507" || arguments.First() == "scp-1507" || arguments.First() == "SCP-1507")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a weird noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_flamingo.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First() == "big" || arguments.First() == "Big" || arguments.First() == "large" || arguments.First() == "Large" || arguments.First() == "grow")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_big.Add(thiscola.ItemSerial);

                        }
                        else if (arguments.First().ToLower() == "quantamgas" || arguments.First().ToLower() == "bose-einstein" || arguments.First().ToLower() == "condensate")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made the sound of gas being released and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.AntiSCP207);
                            colas_quantam.Add(thiscola.ItemSerial);

                        }



                    }
                }
                response = " If you were not holding a coin, did not enter a valid drink, and you did not get anything. You should run this command again. Please also make sure you are in the same room as the machine. You can get a list of valid drinks by running the command .scp294 list";
                return true;
            }
        }

        [PluginEvent(PluginAPI.Enums.ServerEventType.PlayerDeath)]
        void PlayerDead(Player player, Player attacker, DamageHandlerBase damageHandler)
        {
            try
            {
                if (player != null)
                {
                    if (player.Team != Team.SCPs)
                    {
                        if (player.GameObject.transform.localScale != new UnityEngine.Vector3(1.0f, 1.0f, 1.0f))
                        {
                            Timing.CallDelayed(1f, () =>
                            {
                                SetScale(player, 1.0f);
                                Log.Debug("role scale reset: ");
                            });

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Info("ERROR READ ME on player death! Custom Roles will be the same player next Round !!!");
            }
        }

        [PluginEvent(ServerEventType.PlayerUseItem)]
        void OnPlayerStartedUsingItem(Player plr, UsableItem item)
        {
            if (item.ItemTypeId == ItemType.SCP207 && colas_oxygen.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {
                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    
                    plr.EffectsManager.EnableEffect<CardiacArrest>(60, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You drank pure oxygen... You can no longer breathe.", 3);

                    if (new System.Random().Next(100) == 1)
                    {
                        //Warhead.Start();colas_saltwater.Add(thiscola.ItemSerial);
                    }
                });
                //  Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_speed.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    plr.EffectsManager.EnableEffect<MovementBoost>(30, true);
                    plr.EffectsManager.EnableEffect<Invigorated>(30, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You drank the cup of speed juice. It had a overly sweet taste. You feel like running a marathon.", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_water.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    //   plr.EffectsManager.EnableEffect<MovementBoost>(30, true);
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You drank a cup of water.", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_coffee.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true);
                    plr.Heal(20);
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You drank a cup of coffee. It was refreshing.", 3);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.AntiSCP207 && colas_atomkick.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {
                    
                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.AntiScp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.AntiScp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.AntiScp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.AntiScp207>();
                    }
                    //plr.EffectsManager.DisableEffect<CustomPlayerEffects.AntiScp207>();
                    plr.EffectsManager.EnableEffect<MovementBoost>(3, true);
                    plr.EffectsManager.ChangeState<MovementBoost>(255, 4, false);
                    plr.EffectsManager.EnableEffect<CardiacArrest>(60, true);
                    plr.EffectsManager.EnableEffect<Bleeding>(60, true);
                    // plr.Heal(50);
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You drank a can of golden atom kick. You feel amazing and think about the good times.", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.AntiSCP207 && colas_quantam.Contains(item.ItemSerial))
            {
               

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.AntiScp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.AntiScp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.AntiScp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.AntiScp207>();
                    }
                    
                    plr.EffectsManager.EnableEffect<CardiacArrest>(60, true);
                    plr.EffectsManager.EnableEffect<Bleeding>(60, true);
                    plr.ClearBroadcasts();
                });
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_medusa.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    plr.EffectsManager.EnableEffect<DamageReduction>(5, true);
                    plr.EffectsManager.ChangeState<DamageReduction>(4, 5, false);
                    plr.EffectsManager.EnableEffect<Ensnared>(15, true);
                    //plr.EffectsManager.EnableEffect<Bleeding>(60, true);
                    // plr.Heal(50);
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You drank a bottle of medusa. You begin to feel like a statue..", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_crazy.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    plr.EffectsManager.EnableEffect<SeveredHands>(10, true);
                    //plr.EffectsManager.EnableEffect<Bleeding>(60, true);
                    // plr.Heal(50);
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("I hope you realize what you've just done.", 10);
                    Cassie.Message("WEAPONS I use weapons once they locked me in a room a armory an armory with weapons and weapons make me kill", true, true, true);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_nuclearkick.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<MovementBoost>(3, true);
                    //   plr.EffectsManager.ChangeState<MovementBoost>(255, 4, false);
                    //    plr.EffectsManager.EnableEffect<Sinkhole>(20, true);
                    // plr.Heal(50);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("WHACK! Nuclear Kick!", 3);
                    plr.EffectsManager.EnableEffect<Invigorated>(3, true);
                    plr.IsGodModeEnabled = true;
                    Timing.CallDelayed(3f, () =>
                    {
                        plr.IsGodModeEnabled = false;
                        plr.EffectsManager.EnableEffect<Sinkhole>(10, true);
                    });
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_invis.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<MovementBoost>(3, true);
                    //   plr.EffectsManager.ChangeState<MovementBoost>(255, 4, false);
                    plr.EffectsManager.EnableEffect<Invisible>(15, true);
                    // plr.Heal(50);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("EW! It tastes like SCP-268 for some reason.", 3);
                    //  plr.EffectsManager.EnableEffect<Invigorated>(5, false);
                    //  plr.IsGodModeEnabled = true;
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_me.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<MovementBoost>(3, true);
                    //   plr.EffectsManager.ChangeState<MovementBoost>(255, 4, false);
                    plr.EffectsManager.EnableEffect<Flashed>(10, true);
                    // plr.EffectsManager.EnableEffect<SeveredHands>(0, true);
                    plr.EffectsManager.EnableEffect<CardiacArrest>(60, true);
                    /// plr.EffectsManager.EnableEffect<Bleeding>(60, true);
                    // plr.Heal(50);
                    //plr.Kill("You drank yourself, how could you?");
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You drank part of yourself, how could you?", 3);
                    //  plr.EffectsManager.EnableEffect<Invigorated>(5, false);
                    //  plr.IsGodModeEnabled = true;
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_tea.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true);
                    plr.Heal(20);
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You drank a cup of tea, it was refresing.", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_horror.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true);
                    //  plr.Heal(20);
                    plr.EffectsManager.EnableEffect<Flashed>(3, true);
                    plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("THE FOG IS COMING.", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_borgor.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true);
                    //  plr.Heal(20);
                    plr.EffectsManager.EnableEffect<Invigorated>(15, true);
                    //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("Borgor.", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_antimatter.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                    plr.EffectsManager.EnableEffect<Flashed>(20, true);
                    plr.Kill("I don't know what you expected.");

                    //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    // plr.ReceiveHint("Borgor.", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_gas.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                    plr.EffectsManager.EnableEffect<Blinded>(20, true);
                    plr.EffectsManager.EnableEffect<CardiacArrest>(20, true);
                    Timing.CallDelayed(3f, () =>
                    {
                        plr.Kill("Didn't you get taught better than to drink gasoline? WHAT WERE YOU THINKING?");
                    });


                    //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    // plr.ReceiveHint("Borgor.", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_cherryatomkick.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                    plr.EffectsManager.EnableEffect<Invigorated>(20, true);
                    plr.EffectsManager.EnableEffect<BodyshotReduction>(20, true);
                    plr.EffectsManager.EnableEffect<DamageReduction>(15, true);
                    plr.EffectsManager.EnableEffect<Scp1853>(20, true);
                    // plr.Kill("I don't know what you expected.");

                    //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You took a sip of Cherry Atom Kick. It was perfectly refreshing.", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_bepis.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                    plr.EffectsManager.EnableEffect<Invigorated>(20, true);
                    plr.EffectsManager.EnableEffect<BodyshotReduction>(20, true);
                    plr.EffectsManager.EnableEffect<DamageReduction>(15, true);
                    plr.EffectsManager.EnableEffect<Scp1853>(20, true);
                    // plr.Kill("I don't know what you expected.");

                    //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You took a drink from the can of bepis.", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_small.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                    // plr.EffectsManager.EnableEffect<Invigorated>(20, true);
                    //  plr.EffectsManager.EnableEffect<BodyshotReduction>(20, true);
                    //  plr.EffectsManager.EnableEffect<DamageReduction>(15, true);
                    //  plr.EffectsManager.EnableEffect<Scp1853>(20, true);
                    // plr.Kill("I don't know what you expected.");
                    // SetScale(plr, 0.85f);
                    SetScale(plr, plr.GameObject.transform.localScale.y - 0.15f);
                    // player.GameObject.transform.localScale.y
                    // plr.SCal
                    //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You feel smaller... (if you need to return to normal after a respawn or revive, run the command .fixmepls in your ~ console.)", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_big.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                    // plr.EffectsManager.EnableEffect<Invigorated>(20, true);
                    //  plr.EffectsManager.EnableEffect<BodyshotReduction>(20, true);
                    //  plr.EffectsManager.EnableEffect<DamageReduction>(15, true);
                    //  plr.EffectsManager.EnableEffect<Scp1853>(20, true);
                    // plr.Kill("I don't know what you expected.");
                    // SetScale(plr, 0.85f);
                    SetScale(plr, plr.GameObject.transform.localScale.y + 0.15f);
                    // player.GameObject.transform.localScale.y
                    // plr.SCal
                    //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You feel bigger... (if you need to return to normal after a respawn or revive, run the command .fixmepls in your ~ console.)", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_peanut.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                    //   plr.EffectsManager.EnableEffect<Invigorated>(20, true);
                    //  plr.EffectsManager.EnableEffect<BodyshotReduction>(20, true);
                    // plr.EffectsManager.EnableEffect<DamageReduction>(15, true);
                    // plr.EffectsManager.EnableEffect<Scp1853>(20, true);
                    // plr.Kill("I don't know what you expected.");
                    var prefab = NetworkClient.prefabs[1306864341];
                    var tantrumObj = UnityEngine.Object.Instantiate(prefab, plr.Position, UnityEngine.Quaternion.identity);
                    var comp = tantrumObj.GetComponent<TantrumEnvironmentalHazard>();
                    comp.SynchronizedPosition = new RelativePosition(plr.Position);

                    NetworkServer.Spawn(tantrumObj.gameObject);
                    //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You drank the bottle of SCP-173. You suddenly feel the need to go to the bathroom, will you make it?", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.AntiSCP207 && colas_explosion.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.3f, () =>
                {
                    // plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                    plr.EffectsManager.EnableEffect<CustomPlayerEffects.Scp207>(30, true);


                    //   ElevatorChamber.WorldspaceBounds.Contains(plr.Position)

                    // plr.EffectsManager.EnableEffect<BodyshotReduction>(20, true);
                    //plr.EffectsManager.EnableEffect<DamageReduction>(15, true);
                    //  plr.EffectsManager.EnableEffect<Scp1853>(20, true);
                    // plr.Kill("I don't know what you expected.");

                    //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.SendBroadcast("You inhaled the box of gunpowder. BOOM!", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP268 && scp1499.Contains(item.ItemSerial)) //&& ElevatorChamber.WorldspaceBounds.Contains(plr.Position))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(1f, () =>
                {
                    // plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                    //   plr.EffectsManager.EnableEffect<CustomPlayerEffects.AntiScp207>(30, true);
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You took a equipped SCP-1499", 3);

                    UnityEngine.Vector3 plrpos = new UnityEngine.Vector3(129.9321f, -13f, 25.997f);
                    UnityEngine.Vector3 dimension = new UnityEngine.Vector3(145.699997f, 1005.4000001f, 73.0999985f);


                    var relative = new RelativePosition(plr.Position);
                    if (relative.WaypointId != 0 && WaypointBase.TryGetWaypoint(relative.WaypointId, out var waypoint) && waypoint is ElevatorWaypoint)
                    {
                        plr.SendBroadcast("Unable to use SCP-1499 as you were in an elevator. It will act as a normal SCP-268.", 3);
                        //Timing.CallDelayed(0.5f, () =>
                        //{
                       //     plr.EffectsManager.DisableEffect<Invisible>();
                       // });
                    }
                    else if (plr.EffectsManager.TryGetEffect(out PocketCorroding sevHands) && sevHands.IsEnabled)
                    {
                        plr.SendBroadcast("Unable to use SCP-1499 as you were in the pocket dimension.", 3);
                        Timing.CallDelayed(0.5f, () =>
                        {
                            plr.EffectsManager.DisableEffect<Invisible>();
                        });
                    }
                    else
                    {
                        plrpos = plr.Position;

                        plr.Position = dimension;
                        plr.ReceiveHint("You put on SCP-1499.", 3);
                        Timing.CallDelayed(0.5f, () =>
                        {
                            plr.EffectsManager.DisableEffect<Invisible>();
                        });
                        plr.EffectsManager.EnableEffect<Sinkhole>(14, true);
                        plr.EffectsManager.EnableEffect<Deafened>(14, true);
                        //plr.EffectsManager.DisableEffect<Deafened>();
                        Timing.CallDelayed(14f, () =>
                        {
                            plr.Position = plrpos;
                            plr.EffectsManager.DisableEffect<Invisible>();

                        });
                    }

                    // plr.EffectsManager.EnableEffect<BodyshotReduction>(20, true);
                    //plr.EffectsManager.EnableEffect<DamageReduction>(15, true);
                    //  plr.EffectsManager.EnableEffect<Scp1853>(20, true);
                    // plr.Kill("I don't know what you expected.");

                    //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_antimatter.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                    plr.EffectsManager.EnableEffect<Flashed>(20, true);
                    plr.Kill("This used to explode the entire facility, but here you go.");

                    //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    // plr.ReceiveHint("Borgor.", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_ghost.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                    plr.EffectsManager.EnableEffect<Ghostly>(20, true);
                    plr.EffectsManager.EnableEffect<CustomPlayerEffects.Invisible>(20, true);
                    plr.ReceiveHint("You drank the ghastly brew...", 3);

                    //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    // plr.ReceiveHint("Borgor.", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_windex.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                    plr.EffectsManager.EnableEffect<Invigorated>(20, true);
                    //plr.Kill("I don't know what you expected.");


                    Timing.CallDelayed(4f, () =>
                    {

                        plr.Kill("Drank windex for some reason.");

                        //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    });
                    //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You drank the bottle and now feel like a new person. Not for long..", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_zombie.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {
                    UnityEngine.Vector3 plrpos = new UnityEngine.Vector3(129.9321f, -13f, 25.997f);
                    RoleTypeId lastrole = RoleTypeId.None;

                    plrpos = plr.Position;
                    lastrole = plr.Role;

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                    //plr.EffectsManager.EnableEffect<Flashed>(20, true);
                    //plr.Kill("I don't know what you expected.");

                    plr.ReceiveHint("You drank a cup of [REDACTED]. Your items magically disappeared!", 3);
                    plr.ClearBroadcasts();
                    plr.SetRole(RoleTypeId.Scp0492);
                    plr.Position = plrpos;
                    plr.EffectsManager.EnableEffect<SeveredHands>(10, true);
                    //plr.EffectsManager.EnableEffect<Deafened>(10, true);
                    Timing.CallDelayed(10f, () =>
                    {




                        // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true

                        //plr.Kill("I don't know what you expected.");
                        plr.SetRole(lastrole);

                        plr.ReceiveHint("I cannot believe you would betray your friends...", 3);
                        plr.Position = plrpos;
                        SetScale(plr, 1.0f);
                        Timing.CallDelayed(0.5f, () =>
                        {
                            plr.ClearInventory();

                        });

                        //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);

                        // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                        // plr.ReceiveHint("Borgor.", 3);
                    });


                    //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    // plr.ReceiveHint("Borgor.", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_flamingo.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {
                    UnityEngine.Vector3 plrpos = new UnityEngine.Vector3(129.9321f, -13f, 25.997f);
                    RoleTypeId lastrole = RoleTypeId.None;

                    plrpos = plr.Position;
                    // lastrole = plr.Role;

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                    //plr.EffectsManager.EnableEffect<Flashed>(20, true);
                    //plr.Kill("I don't know what you expected.");

                    plr.ReceiveHint("You drank a cup of (FLAMINGO BATTLE CRY). Your items magically disappeared!", 3);
                    plr.ClearBroadcasts();
                    // plr.SetRole(RoleTypeId.Flamingo);

                    Timing.CallDelayed(0.2f, () =>
                    {
                        plr.Position = plrpos;
                    });

                    //plr.EffectsManager.EnableEffect<SeveredHands>(10, true);
                    //plr.EffectsManager.EnableEffect<Deafened>(10, true);



                    //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    // plr.ReceiveHint("Borgor.", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_alpha.Contains(item.ItemSerial))
            {


                Timing.CallDelayed(3.4f, () =>
                {
                    UnityEngine.Vector3 plrpos = new UnityEngine.Vector3(129.9321f, -13f, 25.997f);
                    RoleTypeId lastrole = RoleTypeId.None;

                    plrpos = plr.Position;

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    plr.ReceiveHint("You drank a cup of (VERY LOUD FLAMINGO BATTLE CRY). Your items magically disappeared!", 3);
                    plr.ClearBroadcasts();
                    // plr.SetRole(RoleTypeId.AlphaFlamingo);
                    Timing.CallDelayed(0.2f, () =>
                    {
                        plr.Position = plrpos;
                    });
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_saltwater.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<MovementBoost>(3, true);
                    //   plr.EffectsManager.ChangeState<MovementBoost>(255, 4, false);
                    // plr.EffectsManager.EnableEffect<Invisible>(10, true);
                    // plr.Heal(50);
                    //   plr.Damage(damageHandlerBase);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You drank the saltwater. Salty!", 3);
                    //  plr.EffectsManager.EnableEffect<Invigorated>(5, false);
                    //  plr.IsGodModeEnabled = true;
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_oil.Contains(item.ItemSerial))
            {


                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<MovementBoost>(3, true);
                    //   plr.EffectsManager.ChangeState<MovementBoost>(255, 4, false);
                    // plr.EffectsManager.EnableEffect<Invisible>(10, true);
                    // plr.Heal(50);
                    //   plr.Damage(damageHandlerBase);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You drank oil. *SCREECH* MURICA!!!!!!!! FREEEEDOOMMMMM!!!!", 3);
                    Timing.CallDelayed(3f, () =>
                    {
                        plr.Kill("Drank oil.");
                    });
                    //  plr.EffectsManager.EnableEffect<Invigorated>(5, false);
                    //  plr.IsGodModeEnabled = true;
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.AntiSCP207 && colas_cold.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.AntiScp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.AntiScp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.AntiScp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.AntiScp207>();
                    }
                    // plr.EffectsManager.EnableEffect<MovementBoost>(3, true);
                    //   plr.EffectsManager.ChangeState<MovementBoost>(255, 4, false);
                    // plr.EffectsManager.EnableEffect<Snowed>(30, true);
                    plr.EffectsManager.EnableEffect<Stained>(30, true);
                    plr.EffectsManager.EnableEffect<Exhausted>(30, true);
                    // plr.Heal(50);
                    //   plr.Damage(damageHandlerBase);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You drank the cup of ice. You start to shiver...", 3);
                    //  plr.EffectsManager.EnableEffect<Invigorated>(5, false);
                    //  plr.IsGodModeEnabled = true;
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);




                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_sour_patch_kids_slushy.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<MovementBoost>(3, true);
                    //   plr.EffectsManager.ChangeState<MovementBoost>(255, 4, false);
                    // plr.EffectsManager.EnableEffect<Invisible>(10, true);
                    // plr.Heal(50);
                    //   plr.Damage(damageHandlerBase);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("Timeout for you!", 3);
                    UnityEngine.Vector3 plrpos = new UnityEngine.Vector3(40f, 1014f, -32.60f);
                    UnityEngine.Vector3 tppos = new UnityEngine.Vector3(40f, 1014f, -32.60f);
                    // plrpos = plr.Position;
                    plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    //  plr.ReceiveHint("You drank a cup of [REDACTED]. Your items magically disappeared!", 3);
                    plr.ClearBroadcasts();
                    plrpos = plr.Position;
                    plr.Position = tppos;


                    Timing.CallDelayed(10f, () =>
                    {
                        plr.Position = plrpos;
                    });

                    //  plr.EffectsManager.EnableEffect<Invigorated>(5, false);
                    //  plr.IsGodModeEnabled = true;
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.AntiSCP207 && colas_sour_patch_kids_slushy.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.AntiScp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.AntiScp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.AntiScp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.AntiScp207>();
                    }
                    // plr.EffectsManager.EnableEffect<MovementBoost>(3, true);
                    //   plr.EffectsManager.ChangeState<MovementBoost>(255, 4, false);
                    // plr.EffectsManager.EnableEffect<Invisible>(10, true);
                    // plr.Heal(50);
                    //   plr.Damage(damageHandlerBase);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("Timeout for you!", 3);
                    UnityEngine.Vector3 plrpos = new UnityEngine.Vector3(40f, 1014f, -32.60f);
                    UnityEngine.Vector3 tppos = new UnityEngine.Vector3(40f, 1014f, -32.60f);
                    // plrpos = plr.Position;
                    plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    //  plr.ReceiveHint("You drank a cup of [REDACTED]. Your items magically disappeared!", 3);
                    plr.ClearBroadcasts();
                    plrpos = plr.Position;
                    plr.Position = tppos;


                    Timing.CallDelayed(10f, () =>
                    {
                        plr.Position = plrpos;
                    });

                    //  plr.EffectsManager.EnableEffect<Invigorated>(5, false);
                    //  plr.IsGodModeEnabled = true;
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.AntiSCP207 && !colas_atomkick.Contains(item.ItemSerial) && !colas_cold.Contains(item.ItemSerial) && !colas_explosion.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {
                   // plr.Health = plr.Health + 15;
                });

            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_metal.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<MovementBoost>(3, true);
                    //   plr.EffectsManager.ChangeState<MovementBoost>(255, 4, false);
                    plr.EffectsManager.EnableEffect<Flashed>(10, true);
                    // plr.Heal(50);
                    //   plr.Damage(damageHandlerBase);
                    //plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    // plr.ReceiveHint("Good luck!", 3);
                    // Vector3 plrpos = new Vector3(40f, 1014f, -32.60f);
                    // plrpos = plr.Position;
                    //    plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    //  plr.ReceiveHint("You drank a cup of [REDACTED]. Your items magically disappeared!", 3);
                    //  plr.ClearBroadcasts();
                    //        plr.Position = plrpos;
                    //   plr.Position = GetBestExitPosition(plr);

                    plr.Kill("Swallowed metal.");

                    //  plr.EffectsManager.EnableEffect<Invigorated>(5, false);
                    //  plr.IsGodModeEnabled = true;
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_scp207.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {
                    //  plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.Scp207>(5, true);



                    //  if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 s207effect) && s207effect.IsEnabled)
                    // {

                    //    plr.EffectsManager.ChangeState<CustomPlayerEffects.Scp207>(s207effect.Intensity += 1, 5, false);
                    // }

                    // plr.EffectsManager.EnableEffect<Ensnared>(15, true);
                    //plr.EffectsManager.EnableEffect<Bleeding>(60, true);
                    // plr.Heal(50);
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);
                    plr.ClearBroadcasts();
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You drank a bottle of cola. You now feel faster...", 3);
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_teleportation.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    }
                    // plr.EffectsManager.EnableEffect<MovementBoost>(3, true);
                    //   plr.EffectsManager.ChangeState<MovementBoost>(255, 4, false);
                    // plr.EffectsManager.EnableEffect<Invisible>(10, true);
                    // plr.Heal(50);
                    //   plr.Damage(damageHandlerBase);
                    plr.ClearBroadcasts();

                    //var list = new List<RoomName> = {"e","e"} 
                    if (plr.IsAlive && plr.RoleBase is IFpcRole role)
                    {
                        plr.EffectsManager.EnableEffect<PocketCorroding>();
                        var position = Scp106PocketExitFinder.GetBestExitPosition(role);
                        plr.EffectsManager.DisableEffect<PocketCorroding>();
                        plr.EffectsManager.DisableEffect<Corroding>();
                        plr.Position = position;
                    }
                        


                    //   foreach (var room in Facility.Rooms)
                    //    {



                    //   }


                    //
                    List<Player> Playerss = Player.GetPlayers();

                    foreach (var randplr in Playerss)
                    {
                        if (randplr.IsSCP == true && randplr.Role != RoleTypeId.Scp079)
                        {
                            // plr.Position = randplr.Position;
                        }
                    }
                   // plr.Kill("Tried to drink the telportation potion.");
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    //plr.ReceiveHint("You have been teleported to the nearest SCP... Have fun!", 3);
                    //  plr.EffectsManager.EnableEffect<Invigorated>(5, false);
                    //  plr.IsGodModeEnabled = true;
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
        }




        [PluginEvent(ServerEventType.PlayerChangeItem)]
        void OnPlayerChangesItem(Player plr, ushort oldItem, ushort newItem)
        {

            if (plr.ReferenceHub.inventory.UserInventory.Items.TryGetValue(newItem, out ItemBase newItemBase))
            {
                if (!newItemBase == false && colas_oxygen.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a cup of pure oxygen.", 3);
                }
                if (!newItemBase == false && colas_quantam.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of quantum gas. It's slightly pink... for some reason.", 3);
                }
                else if (!newItemBase == false && colas_alpha.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of (VERY LOUD FLAMINGO BATTLE CRY). It seems very cool.", 3);
                }
                else if (!newItemBase == false && colas_metal.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of metal. It is very heavy.", 3);
                }
                else if (!newItemBase == false && colas_cold.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of ice. Your hands begin to freeze.", 3);
                }
                else if (!newItemBase == false && colas_speed.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of speed juice. The smell is terrible.", 3);
                }
                else if (!newItemBase == false && colas_water.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of water. It is just water.", 3);
                }
                else if (!newItemBase == false && colas_coffee.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a cup of coffee. You feel the sudden urge to drink it.", 3);
                }
                else if (!newItemBase == false && colas_atomkick.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a can of golden atom kick. You feel can feel the speed coming from the can.", 3);

                }
                else if (!newItemBase == false && colas_nuclearkick.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of Nuclear Kick!", 3);

                }
                else if (!newItemBase == false && colas_sour_patch_kids_slushy.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a sour patch kids slushy.", 3);

                }
                else if (!newItemBase == false && colas_invis.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of invisibility.", 3);

                }
                else if (!newItemBase == false && colas_oil.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a can of oil. You feel that this would be better somewhere else and not in your stomach.", 3);

                }
                else if (!newItemBase == false && colas_me.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of yourself. Why are you doing this?", 3);

                }
                else if (!newItemBase == false && colas_tea.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a cup of tea.", 3);

                }
                else if (!newItemBase == false && colas_horror.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of horror.", 3);

                }
                else if (!newItemBase == false && colas_borgor.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a cup containing a borgor.", 3);

                }
                else if (!newItemBase == false && colas_antimatter.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a cup of antimatter.", 3);

                }
                else if (!newItemBase == false && colas_zombie.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a cup of [REDACTED].", 3);

                }
                else if (!newItemBase == false && colas_flamingo.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a cup of (FLAMINGO BATTLE CRY).", 3);

                }
                else if (!newItemBase == false && colas_cherryatomkick.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of Cherry Atom Kick.", 3);

                }
                else if (!newItemBase == false && colas_bepis.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a can of Bepis.", 3);

                }
                else if (!newItemBase == false && colas_explosion.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of gunpowd- WAIT NO", 3);

                }
                else if (!newItemBase == false && colas_saltwater.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of saltwater.", 3);

                }
                else if (!newItemBase == false && colas_peanut.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of SCP-173.", 3);

                }
                else if (!newItemBase == false && colas_gas.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a can of gasoline.", 3);

                }
                else if (!newItemBase == false && colas_ghost.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a ghostly brew.", 3);

                }
                else if (!newItemBase == false && colas_scp207.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("COCA COLA ESPUMA", 3);

                }
                else if (!newItemBase == false && colas_big.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of growing juice.", 3);

                }
                else if (!newItemBase == false && colas_teleportation.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a teleportation potion.", 3);

                }
                else if (!newItemBase == false && colas_windex.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of windex.", 3);

                }
                else if (!newItemBase == false && colas_medusa.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of medusa.", 3);

                }
                else if (!newItemBase == false && colas_crazy.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("Crazy?", 3);

                }
                else if (!newItemBase == false && colas_small.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a potion of smol.", 3);

                }

                else if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP268 && scp1499.Contains(newItemBase.ItemSerial) && newItemBase.ItemTypeId == ItemType.SCP268)
                {
                    plr.ClearBroadcasts();
                   //   plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);

                     if (new System.Random().Next(5) == 1 && !scp1499.Contains(newItemBase.ItemSerial) && !hats.Contains(newItemBase.ItemSerial))
                       {
                 //   Warhead.Start();colas_saltwater.Add(thiscola.ItemSerial);
                        scp1499.Add(newItemBase.ItemSerial);
                        plr.ReceiveHint("You equipped SCP-1499.", 3);

                       }
                    else if (!hats.Contains(newItemBase.ItemSerial) && !scp1499.Contains(newItemBase.ItemSerial))
                        {
                          hats.Add(newItemBase.ItemSerial);
                      }
                      else if (scp1499.Contains(newItemBase.ItemSerial))
                    {
                       plr.ReceiveHint("You equipped SCP-1499.", 3);
                    }


                }


            }
        }





        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class GiveScp127 : ICommand
        {
            public string Command { get; } = "give1499";

            public string[] Aliases { get; } = new string[] { };

            public string Description { get; } = "give scp-1499";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player;
                if (Player.TryGet(sender, out player))
                {
                    ItemBase adminscp1499item = player.AddItem(ItemType.SCP268);
                    scp1499.Add(adminscp1499item.ItemSerial);
                    response = "success";
                    return true;
                }
                response = "failed";
                return false;
            }
        }


        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class forceserpentshand : ICommand
        {
            public string Command { get; } = "forceserpentshand";

            public string[] Aliases { get; } = new string[] { };

            public string Description { get; } = "Force a spawn wave of serpent's hand, requries chaos insurgency to be winning in tickets so that way spawn conditions are met.";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player;
                if (Player.TryGet(sender, out player))
                {
                    isSerpentSpawning = true;
                    haveSerpentsSpawned = false;
                    response = "success, MAKE SURE THAT CHAOS INSURGENCY HAVE ENOUGH TICKETS TO FORCE THIS SPAWN, OTHERWISE ISSUES WILL OCCUR.";
                    return true;
                }
                response = "failed";
                return false;
            }
        }


        [CommandHandler(typeof(ClientCommandHandler))]
        public class fixmepls : ICommand
        {
            public string Command { get; } = "fixmepls";

            public string[] Aliases { get; } = new string[] { };

            public string Description { get; } = "Hi!";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player;
                if (Player.TryGet(sender, out player))
                {
                    SetScale(player, 1.0f);
                    response = "success, your scale should be fixed. If issues persist, please rejoin the game.";
                    return true;
                }
                response = "failed";
                return false;
            }
        }


        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class setscale : ICommand
        {
            public string Command { get; } = "setscale";

            public string[] Aliases { get; } = new string[] { };

            public string Description { get; } = "Set player scale";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {


                Player player;

                if (Player.TryGet(sender, out player))
                {
                    if (arguments.Count != 0)
                    {

                        // response = " Success, you gave your coin for: ";
                        // problem if statement, wants me to stop comparing a string to a system.predicate string I'm probably stupid but yeah  if (arguments.First() == list.Find("deeznuts"))
                        //  {

                        //}    

                        if (Player.TryGet(sender, out player)) ;
                        SetScale(player, (float)int.Parse(arguments.First()));


                        response = "set player scale";
                        return true;
                    }






                }
                response = "failed";
                return false;
            }
        }

        static bool coolDowned2 = false;


        [CommandHandler(typeof(ClientCommandHandler))]
        public class forcepowerfailure : ICommand
        {
            public string Command { get; } = "forcepowerfailure";

            public string[] Aliases { get; } = new string[] { "fpf", "powerblackout", "blackout", "forceblackout" };

            public string Description { get; } = "Force a facility power blackout.";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player = Player.Get(((CommandSender)sender).SenderId);


                if (Player.TryGet(sender, out player) && player.Role == RoleTypeId.Scp079 && coolDowned2 == false)
                {
                    if (player.RoleBase is Scp079Role scp079Role && scp079Role.SubroutineModule.TryGetSubroutine(out Scp079TierManager tierManager)
                     && scp079Role.SubroutineModule.TryGetSubroutine(out Scp079AuxManager energyManager))
                    {


                        if (energyManager.CurrentAux >= 90 && tierManager.AccessTierLevel >= 3)
                        {
                            coolDowned2 = true;
                            // case Scp079Role scp079:
                            //  scp079.SubroutineModule.TryGetSubroutine(out Scp079TierManager tier);

                            player.SendConsoleMessage("Command is now on a 1m 30s cooldown.");

                            Cassie.Message("pitch_0.5 .g6 .g6 Pitch_0.9 jam_057_6 Attention pitch_0.7 .g3 Central pitch_0.89 Power pitch_0.85 jam_12_3 System .g4 pitch_0.7 failure jam_057_6 pitch_0.5 .g2 .g2 .G4 pitch_1.4 All personnel are to report to jam_056_4 .G1 .G2 .G4 immediately .G3 .G4", true, true, false);

                            energyManager.CurrentAux = energyManager.CurrentAux - 90;

                            Timing.CallDelayed(20f, () =>
                            {
                                Facility.TurnOffAllLights();


                                Timing.CallDelayed(Random.Range(15f, 20f), () =>
                                {
                                    Facility.TurnOnAllLights();
                                    Cassie.Message("Central Power System is back online", true, true, true);

                                });
                            });


                            Timing.CallDelayed(90f, () =>
                            {
                                coolDowned2 = false;
                                player.SendConsoleMessage("Power failure ability is ready!");
                                player.SendBroadcast("Power failure ability is ready!", 5);
                            });

                        }


                    }
                    response = "success. If you do not hear a cassie announcement, you likely do not have the right tier or command is on cooldown.";
                    return true;
                }

                else
                {
                    response = "failed";
                    return false;
                }

            }
        }

        static bool coolDowned = false;

        [CommandHandler(typeof(ClientCommandHandler))]
        public class findplr : ICommand
        {
            public string Command { get; } = "findally";

            public string[] Aliases { get; } = new string[] { "findallies", "find079" };

            public string Description { get; } = "Lists the rooms of allied classes.";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player = Player.Get(((CommandSender)sender).SenderId);


                if (Player.TryGet(sender, out player) && player.Role == RoleTypeId.Scp079 && coolDowned == false)
                {
                    coolDowned = true;
                    foreach (var currentplr in Player.GetPlayers())
                    {
                        if (currentplr.Team == Team.SCPs || currentplr.IsTutorial)
                        {
                            if (currentplr != player)
                            {
                                player.SendConsoleMessage(currentplr.Role.ToString() + currentplr.Room.Name.ToString(), "white");

                            }

                        }
                    }
                    player.SendConsoleMessage("Command is now on a 15s cooldown.");
                    Timing.CallDelayed(15f, () =>
                    {
                        coolDowned = false;
                        player.SendConsoleMessage("Find allies is ready!");
                        player.SendBroadcast("Find allies ability is ready!", 3);
                    });

                    response = "Success, outputting the rooms of allied classes. (Serpents Hand AND SCPs)";
                    return true;
                }
                else
                {
                    response = "That ability is on cooldown.";
                    return false;
                }

            }
        }


        [CommandHandler(typeof(ClientCommandHandler))]
        public class computerbuff : ICommand
        {
            public string Command { get; } = "computerbuff";

            public string[] Aliases { get; } = new string[] { "pcbuff", "079buff" };

            public string Description { get; } = "Help command";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player = Player.Get(((CommandSender)sender).SenderId);


                if (Player.TryGet(sender, out player) && player.Role == RoleTypeId.Scp079)
                {

                    player.SendConsoleMessage("TIP: You can cmdbind these commands. Look it up if needed in the scpsl wiki.");
                    response = "ComputerBuff Commands:";
                    player.SendConsoleMessage(".findallies - (ANY TIER) Finds allied classes and their current rooms.");
                    player.SendConsoleMessage(".blackout - (LOCKED TO TIER 3+) Forces a facility power failure. BLACKS OUT THE ENTIRE FACILITY FOR 15-20 seconds.");
                    return true;
                }
                else
                {
                    response = "You can't run that.";
                    return false;
                }

            }
        }

     



        public int CompareTo(object obj)
        {
            return Comparer<EventHandlers>.Default.Compare(this, obj as EventHandlers);
        }
    }
}
