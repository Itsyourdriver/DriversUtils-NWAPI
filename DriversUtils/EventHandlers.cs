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
    using PlayerRoles.PlayableScps.Scp106;
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
    using InventorySystem.Items.ThrowableProjectiles;
    using InventorySystem;
    using SCPSLAudioApi.AudioCore;
    using System.IO;
    using VoiceChat;
    using DriversUtils;
    using InventorySystem.Items.Usables.Scp330;
    using Vector3 = UnityEngine.Vector3;
    using PluginAPI.Core.Items;
    using Footprinting;
    using PluginAPI.Core.Zones.Heavy;
    using MapGeneration.Distributors;
    using Object = UnityEngine.Object;
    using System.Security.Cryptography;
    using static UnityEngine.GraphicsBuffer;
    using CommandSystem.Commands.RemoteAdmin.Warhead;
    using static UnityEngine.Random;

    public class EventHandlers : IComparable
    {
        int respawn_count = 0;
        HashSet<int> fbi = new HashSet<int>();
        HashSet<int> sci = new HashSet<int>();
        HashSet<int> ambience914 = new HashSet<int>();
        static UnityEngine.Vector3 offset = new UnityEngine.Vector3(-40.021f, -8.119f, -36.140f);
        SpawnableTeamType spawning_team = SpawnableTeamType.None;
        public static bool isSerpentSpawning = false;
        public static bool isScienceTeamSpawning = false;

        private List<Scp079Generator> _generators = new List<Scp079Generator>();
        private ReferenceHub TempDummyy = null;
        private AudioPlayerBase audioPlayerr = null;
        int randomNumber;
        int generatorsActivated = 0;
        private HashSet<Player> _PlayersWithArmor = new HashSet<Player>();

        private CoroutineHandle _displayCoroutine;
        public static HashSet<ushort> ghostLantern = new HashSet<ushort>();
        public static HashSet<ushort> normalLantern = new HashSet<ushort>();

        public static HashSet<ushort> THEButton = new HashSet<ushort>();
        string RoundEvent;
        string LastRoundEvent;
        bool buttonused = false;

        [PluginEvent(ServerEventType.RoundStart)]
        void RoundStarted()
        {
            buttonused = false;
            respawn_count = 0;
            fbi.Clear();
            spawning_team = SpawnableTeamType.None;
            isSerpentSpawning = false;
            // TempDummyy = AddDummy2();

          //  RoundEvent = "";
            generatorsActivated = 0;
            // PlayAudio64("ninefourteen.ogg", (byte)65F, true, TempDummyy);
            // audioPlayerr = AudioPlayerBase.Get(TempDummyy);

            if (randomNumber <= cfg.EventRarity)
            {
                RoundEvent = cfg.Events.RandomItem();

                if (RoundEvent == LastRoundEvent)
                {
                    RoundEvent = cfg.Events.RandomItem();
                    if (RoundEvent == LastRoundEvent)
                    {
                        RoundEvent = cfg.Events.RandomItem();
                        if (RoundEvent == LastRoundEvent)
                        {
                            RoundEvent = cfg.Events.RandomItem();
                        }
                    }

                }

                LastRoundEvent = RoundEvent;
                Timing.CallDelayed(5f, () =>
                {




                    if (RoundEvent == "PowerBlackout")
                    {
                        Cassie.GlitchyMessage("Facility power system failure", 1f, 1f);
                    }
                    else
                    {
                        Cassie.Message(". . . . . . . . . . . . . . .", false, true, false);
                    }
                    foreach (var p in Player.GetPlayers())
                    {
                        if (p.IsHuman && p.Role != RoleTypeId.Overwatch && p.Role != RoleTypeId.Tutorial && p.Role != RoleTypeId.Spectator)
                        {



                            if (RoundEvent == "ChaosInvasion")
                            {
                                p.SendBroadcast("<color=#228B22>EVENT:</color> Chaos Invasion.", 13, Broadcast.BroadcastFlags.Normal, false);
                            }
                            if (RoundEvent == "PowerBlackout")
                            {
                                Facility.TurnOffAllLights();
                                p.SendBroadcast("<color=#228B22>EVENT:</color> Facility Power Blackout. Activate a generator to return facility power.", 13, Broadcast.BroadcastFlags.Normal, false);
                            }
                            /*
                             if (RoundEvent == "UnstablePower")
                             {
                                 p.SendBroadcast("<color=#228B22>EVENT:</color> Unstable Facility Power. ", 13, Broadcast.BroadcastFlags.Normal, false);
                             }
                            */
                            if (RoundEvent == "NukeDisabled")
                            {
                                p.SendBroadcast("<color=#228B22>EVENT:</color> Warhead Maitnence. Warhead can be turned on once at least 2 generators have been activated.", 13, Broadcast.BroadcastFlags.Normal, false);
                                Warhead.WarheadUnlocked = true;

                            }
                            if (RoundEvent == "ArmedDClass")
                            {
                                p.SendBroadcast("<color=#228B22>EVENT:</color> The Class-D Prisoners have taken up arms!", 13, Broadcast.BroadcastFlags.Normal, false);
                                Warhead.WarheadUnlocked = true;

                            }
                            if (RoundEvent == "Foggy")
                            {
                                p.SendBroadcast("<color=#228B22>EVENT:</color> THE FOG IS COMING THE FOG IS COMING THE FOG IS COMING THE FOG IS COMING THE FOG IS COMING", 13, Broadcast.BroadcastFlags.Normal, false);
                            }
                            if (RoundEvent == "SpecialOps")
                            {
                                p.SendBroadcast("<color=#228B22>EVENT:</color> Mobile Task Force Unit Epsilon-11 have been deployed to replace on-site security.", 13, Broadcast.BroadcastFlags.Normal, false);
                            }
                        }
                    }
                });

            }


            if (_displayCoroutine.IsRunning)
                Timing.KillCoroutines(_displayCoroutine);

            _displayCoroutine = Timing.RunCoroutine(ShowDisplay());

            List<String> RoomList = new List<String> { "EZ_Crossing (3)", "EZ_upstairs", "EZ_Smallrooms2", "EZ_PCs", "EZ_GateA", "EZ_GateB", "HCZ_049", "HCZ_Tesla", "LCZ_Cafe (15)", "LCZ_Plants", "LCZ_372 (18)" };
            System.Random random = new System.Random();
            String RandomRoom = RoomList.RandomItem();
            Log.Debug(RandomRoom);
            //RoomIdentifier.AllRoomIdentifiers.TryGetValue(Random.Range(1, RoomIdentifier.AllRoomIdentifiers.Count));
            //HashSet<RoomIdentifier> AllRooms = RoomIdentifier.AllRoomIdentifiers;
            // int rndmNumber = Random.Next(AllRooms.size());



          
           

            if (new System.Random().Next(25) == 1)
            {
                Timing.CallDelayed(Random.Range(60f, 300f), () =>
                   {
                       ItemBase itemBase = ReferenceHub.HostHub.inventory.ServerAddItem(ItemType.SCP1576);


                       ItemPickupBase itemPickup = itemBase.ServerDropItem();
                       itemPickup.transform.position = new Vector3((float)(RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == RandomRoom).transform.position.x), (float)RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == RandomRoom).transform.position.y + 2, (float)RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == RandomRoom).transform.position.z);
                       itemPickup.transform.rotation = UnityEngine.Quaternion.Euler(0, 0, 0);
                       itemPickup.transform.localScale = new Vector3(1f, 1f, 1f);
                       List<Player> Playerss = Player.GetPlayers();
                       Log.Warning("The Button has spawned.");
                       foreach (var allplrs in Playerss)
                       {
                           if (allplrs.Role != RoleTypeId.Overwatch)
                           {
                               allplrs.SendBroadcast("<color=#C50000>THE BUTTON</color> has spawned.", 3);


                           }
                       }
                       THEButton.Add(itemBase.ItemSerial);


                   });
            }
        }


        static bool haveSerpentsSpawned = false;
        static bool havetheScienceTeamSpawned = false;


        static bool serpentsCaptain = false;
        private Player captainplayer = null;

        void ChangeToTutorial(Player player, RoleTypeId role)
        {
            // player.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.Escaped, RoleSpawnFlags.None);
            Config config = Plugin.Singleton.Config;
            player.ReferenceHub.inventory.UserInventory.Items.Clear();
            player.Role = PlayerRoles.RoleTypeId.Tutorial;
            fbi.Add(player.PlayerId);

            player.ClearBroadcasts();
            // old code / testing crap
            //  player.SendBroadcast("", 15, shouldClearPrevious: true);
            // Teleport.RoomPos(player, RoomIdentifier.AllRoomIdentifiers.Where((r) => r.Zone == FacilityZone.Surface).First(), offset);
            // player.ClearInventory();
            // player.AddItem(ItemType.GunRevolver);
            //   player.AddItem(ItemType.GunShotgun);
            //   player.AddAmmo(ItemType.Ammo762x39, 200);
            if (serpentsCaptain == false)
            {
                serpentsCaptain = true;
                captainplayer = player;

                switch (UnityEngine.Random.Range(0, 5))
                {
                    case 0: AddOrDropItem(player, ItemType.SCP2176); break;
                    case 1: AddOrDropItem(player, ItemType.SCP500); break;
                    case 2: AddOrDropItem(player, ItemType.SCP1853); break;
                    case 3: AddOrDropItem(player, ItemType.SCP207); break;
                    case 4: AddOrDropItem(player, ItemType.SCP018); break;
                    case 5: AddOrDropItem(player, ItemType.SCP268); break;
                }
                player.AddItem(ItemType.ArmorHeavy);
                player.AddAmmo(ItemType.Ammo9x19, 120);
                AddOrDropFirearm(player, ItemType.GunCrossvec, true);

                player.AddAmmo(ItemType.Ammo44cal, 24);
                AddOrDropFirearm(player, ItemType.GunRevolver, true);
                // player.CustomInfo = $"<color=#FF96DE>{player.DisplayNickname}</color>" + "\n<color=#FF96DE>SERPENTS HAND CAPTAIN</color>";
                //  player.PlayerInfo.IsNicknameHidden = true;
                //  player.PlayerInfo.IsUnitNameHidden = true;
                //  player.PlayerInfo.IsRoleHidden = true;


                player.SendBroadcast(config.SerpentsHandCaptainText, 15);

                player.AddItem(ItemType.KeycardChaosInsurgency);
                player.AddItem(ItemType.Medkit);
                player.AddItem(ItemType.Adrenaline);
            }
            else
            {
                // insert cod modern warfare juggernaut music here
                player.SendBroadcast(config.SerpentsHandText, 15);
                player.AddItem(ItemType.ArmorCombat);
                player.AddItem(ItemType.KeycardChaosInsurgency);
                player.AddItem(ItemType.Medkit);
                player.AddItem(ItemType.Adrenaline);
                // player.AddItem(ItemType.SCP1853);
                // player.AddItem(ItemType.AntiSCP207);


                player.AddAmmo(ItemType.Ammo9x19, 120);
                AddOrDropFirearm(player, ItemType.GunFSP9, true);
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

                //     player.AddAmmo(ItemType.Ammo44cal, 24);
                //    AddOrDropFirearm(player, ItemType.GunRevolver, true);
                AddOrDropFirearm(player, ItemType.GunCOM18, true);
                //  player.CustomInfo = $"<color=#FF96DE>{player.DisplayNickname}</color>" + "\n<color=#FF96DE>SERPENTS HAND AGENT</color>";
                //  player.PlayerInfo.IsNicknameHidden = true;
                //   player.PlayerInfo.IsUnitNameHidden = true;
                //   player.PlayerInfo.IsRoleHidden = true;

            }


            Player playertoTP = Player.Get(player.ReferenceHub);
            playertoTP.Position = new UnityEngine.Vector3(0.06f, 1000.96f, 0.33f);

            // might add config for this in the future, dunno yet
            // fyi add +1000 to ur y coord if you wanna tp someone to somewhere on surface, learned that from axwabo. 

        }



        [PluginEvent(ServerEventType.GeneratorActivated)]
        void OnGeneratorActivated(Scp079Generator gen)
        {
            generatorsActivated =+ 1;

            if (generatorsActivated == 2)
            {
                if (RoundEvent == "NukeDisabled")
                {
                    Warhead.WarheadUnlocked = false;
                }
            }

            if (generatorsActivated == 1)
            {
                if (RoundEvent == "PowerBlackout")
                {
                    Facility.TurnOnAllLights();
                    Cassie.GlitchyMessage("Facility power is back online", 0.1f, 0.1f);
                }
            }
        }

        void ChangeToScienceTeam(Player player, RoleTypeId role)
        {
            // player.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.Escaped, RoleSpawnFlags.None);
            Config config = Plugin.Singleton.Config;
            player.ReferenceHub.inventory.UserInventory.Items.Clear();
            player.Role = PlayerRoles.RoleTypeId.Scientist;

            Timing.CallDelayed(0.3f, () =>
            {
                player.ReferenceHub.inventory.UserInventory.Items.Clear();
                sci.Add(player.PlayerId);

                // player.ClearBroadcasts();

                player.SendBroadcast("You have spawned as apart of the Science Team. Assist remaining MTF or Scientists.", 15);
                player.AddItem(ItemType.ArmorCombat);
                player.AddItem(ItemType.KeycardMTFOperative);
                player.AddItem(ItemType.Medkit);
                player.AddItem(ItemType.Painkillers);



                player.AddAmmo(ItemType.Ammo762x39, 120);
                AddOrDropFirearm(player, ItemType.GunAK, true);
                // credit to riptide for this code, i didnt feel like doing this system myself for the time being


                switch (UnityEngine.Random.Range(0, 9))
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
                    case 9: AddOrDropItem(player, ItemType.SCP1853); break;
                }
                //Player playertoTP = Player.Get(player.ReferenceHub);
                //  playertoTP.Position = new UnityEngine.Vector3(63.01f, 991.65f, -50.04f);

                // might add config for this in the future, dunno yet
                // fyi add +1000 to ur y coord if you wanna tp someone to somewhere on surface, learned that from axwabo. 
            });


        }


        Config cfg = Plugin.Singleton.Config;
        private IEnumerator<float> ShowDisplay()
        {

            while (!RoundSummary.singleton._roundEnded)
            {
                yield return Timing.WaitForSeconds(1f);
                try
                {
                    foreach (var player in Player.GetPlayers().Where(p => p != null && p.Room != null))// && p.CurrentItem == ItemType.SCP207 || p.CurrentItem == ItemType.AntiSCP207))
                    {

                        if (player.IsSCP)
                        {

                            if (RoundEvent == "Foggy")
                            {

                                if (player.Role != RoleTypeId.Overwatch && player.Role != RoleTypeId.Spectator)
                                {


                                    player.EffectsManager.ChangeState("FogControl", 255, 1.25f, false);



                                }

                            }

                            string text = $"<align=right>";

                          

                            foreach (var scp in Player.GetPlayers().Where(p => p?.Role.GetTeam() == Team.SCPs && cfg.DisplayStrings.ContainsKey(p.Role)))
                            {
                                text += (player == scp && true ? "<color=#D51D1D>You --></color>" + " " : "") + ProcessStringVariables(cfg.DisplayStrings[scp.Role], player, scp) + "\n";
                            }

                            text += $"<voffset={30}em> </voffset></align>";
                            player.ReceiveHint(text, 1.25f);
                        }



                        if (player.IsHuman && player.Role != RoleTypeId.Overwatch)
                        {



                            if (!ItemType.SCP207.Equals(player.ReferenceHub.inventory.NetworkCurItem.TypeId))
                            {


                                if (player.Room.name == "EZ_Smallrooms2" || player.Room.name == "LCZ_TCross (11)")
                                {
                                    player.ReceiveHint("You may be able to use SCP-294. (.scp294 (drink), run [.scp294 list] for a list of available drinks, some are hidden.)", 1.25f);
                                }

                                if (player.Room.name == "LCZ_372 (18)")
                                {
                                    player.ReceiveHint("You may be able to use SCP-1025. (.scp1025)", 1.25f);
                                }
                            }

                            if (player.Room.name == "LCZ_914 (14)" && ambience914.Contains(player.PlayerId))
                            {

                                // audioPlayerr.BroadcastTo.Add(player.PlayerId);

                                //   player.ReceiveHint("914", 1.25f);
                            }

                            if (player.Room.name != "LCZ_914 (14)" && !ambience914.Contains(player.PlayerId))
                            {
                                //   audioPlayerr.BroadcastTo.Remove(player.PlayerId);

                                //   ambience914.Remove(player.PlayerId);
                                //  player.ReceiveHint("914 is no more", 1.25f);

                            }

                            if (player.Room.name == "LCZ_914 (14)" && !ambience914.Contains(player.PlayerId))
                            {
                                //    audioPlayerr.BroadcastTo.Add(player.PlayerId);
                                // ambience914.Add(player.PlayerId);
                                //   player.ReceiveHint("914", 1.25f);
                            }

                            if (RoundEvent == "Foggy")
                            {

                                if (player.Role != RoleTypeId.Overwatch && player.Role != RoleTypeId.Spectator)
                                {


                                    player.EffectsManager.ChangeState("FogControl", 255, 1.25f, false);



                                }

                            }

                            if (ItemType.Lantern.Equals(player.ReferenceHub.inventory.NetworkCurItem.TypeId) && ghostLantern.Contains(player.CurrentItem.ItemSerial) && player.Room.name != "HCZ_079")
                            {
                                player.EffectsManager.ChangeState("Ghostly", 1, 1.25f, false);

                                if (RoundEvent != "Foggy")
                                {
                                    player.EffectsManager.ChangeState("FogControl", 255, 1.25f, false);
                                }

                                player.EffectsManager.ChangeState("Sinkhole", 1, 1.25f, false);
                                player.EffectsManager.ChangeState("Poisoned", 1, 1.25f, false);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }




        private string ProcessStringVariables(string raw, Player observer, Player target)
        {
            switch (target.ReferenceHub.roleManager.CurrentRole)
            {
                case Scp079Role scp079:
                    scp079.SubroutineModule.TryGetSubroutine(out Scp079AuxManager aux);
                    scp079.SubroutineModule.TryGetSubroutine(out Scp079TierManager tier);
                    raw = raw
                        .Replace("%079energy%", Math.Floor(aux.CurrentAux).ToString())
                        .Replace("%079level%", tier.AccessTierLevel.ToString())
                        .Replace("%079experience", tier.RelativeExp.ToString());
                    break;
                case Scp106Role scp106:
                  //  scp106.SubroutineModule.TryGetSubroutine(out Scp106Vigor vigor);
                    //raw = raw.Replace("%106vigor%", Math.Floor(vigor.VigorAmount * 100).ToString());
                    break;

            }

            return raw
                .Replace("%healthpercent%", Math.Floor(target.Health / target.MaxHealth * 100).ToString())
                .Replace("%health%", Math.Floor(target.Health).ToString())
                .Replace("%generators%", _generators.Count(gen => gen.Engaged).ToString())
            .Replace("%engaging%", _generators.Count(gen => gen.Activating) > 0 ? $" (+{_generators.Count(gen => gen.Activating)})" : "").Replace("%zombies%", Player.GetPlayers<Player>().Count(p => p.Role == RoleTypeId.Scp0492).ToString())
            .Replace("%distance%", target != observer ? Math.Floor(Vector3.Distance(observer.Position, target.Position)) + "m" : "");
    }

        [PluginEvent(ServerEventType.PlayerReceiveEffect)]
        void OnReceiveEffect(Player plr, StatusEffectBase effect, byte intensity, float duration)
        {

            if (effect.name == "Vitality" || effect.name == "MovementBoost" && intensity == 10 || effect.name == "RainbowTaste" && intensity == 1)
            {
                ReferenceHub TempDummy = AddDummy();

                
                if (effect.name == "Vitality")
            {
                
               

                PlayPlayerAudio096(plr, "windows.ogg", (byte)65f, TempDummy);

                Timing.CallDelayed(27.5f, () =>
                {
                    //  CheckPlaying(TempDummy);
                    RemoveDummy096(TempDummy);
                });
            }
            if (effect.name == "MovementBoost" && intensity == 10)
            {
                    //  ReferenceHub TempDummy = AddDummy();

               // CheckPlaying(TempDummy);
                PlayPlayerAudio096(plr, "yellowcandy.ogg", (byte)65f, TempDummy);

                Timing.CallDelayed(8.3f, () =>
                {
                    RemoveDummy096(TempDummy);
                });
            }
            if (effect.name == "RainbowTaste" && intensity == 1)
            {
                    //   ReferenceHub TempDummy = AddDummy();

                   // CheckPlaying(TempDummy);
                    PlayPlayerAudio096(plr, "rainbowcandy.ogg", (byte)75f, TempDummy);


                   

                    Timing.CallDelayed(29f, () =>
                    {

                    RemoveDummy096(TempDummy);

                });
            }
        }
       }


        [PluginEvent(ServerEventType.WaitingForPlayers)]
        public void OnWaitingForPlayers()
        {
            _generators = UnityEngine.Object.FindObjectsOfType<Scp079Generator>().ToList();
            randomNumber = Random.Range(1, 100);
            Log.Debug(randomNumber.ToString());
            RoundEvent = "";
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
                //int randn = Random.RandomRange(1, 4);
                //int randn = 2;
                // if (randn == 2 || randn == 3 || randn == 4 || randn == 1)
                //     {
                if (haveSerpentsSpawned == false)
                {
                    isSerpentSpawning = true;

                    if (config.ShouldSerpentsHandSpawnMore == false)
                    {
                        haveSerpentsSpawned = true;
                        isSerpentSpawning = false;
                    }
                    //  if (UnityEngine.Random.value < 0.10 && spawning_team == SpawnableTeamType.ChaosInsurgency)
                    //  {
                    if (config.ShouldCassie == true)
                    {
                        Cassie.Message(config.CassieMessage, true, config.CassieNoise, config.CassieText);
                    }

                    //  PlayAudio("SerpentsHand.ogg", (byte)45f, false);

                    Timing.CallDelayed(5f, () =>
                   {
                       isSerpentSpawning = false;
                   });

                    //       }
                    //    !player.TemporaryData.Contains("custom_class"))

                    //   }
                }
            

               
            }


            if (respawn_count >= 2 && spawning_team == SpawnableTeamType.NineTailedFox && config.ShouldScienceTeamSpawn == true && new System.Random().Next(7) == 1 && isSerpentSpawning == false)
            {

                if (havetheScienceTeamSpawned == false)
                {
                    isScienceTeamSpawning = true;

                    // if (config.Can == false)
                    //  {
                    //      haveSerpentsSpawned = true;
                    ////      isSerpentSpawning = false;
                    //  }
                    //  if (UnityEngine.Random.value < 0.10 && spawning_team == SpawnableTeamType.ChaosInsurgency)
                    //  {



                    Timing.CallDelayed(0.1f, () =>
                    {
                        Cassie.Clear();
                        if (config.ShouldCassie == true && spawning_team == SpawnableTeamType.NineTailedFox)
                        {
                            if (new System.Random().Next(2) == 1)
                            {
                                Cassie.Message("pitch_1 Attention All Personnel The Science Team have entered the facility . All remaining jam_056_4 personnel pitch_0.11 .g7 pitch_0.7 jam_056_4 .G3 .G2 .G3 .G3 pitch_1.2 NOSCPSLEFT .G4", true, true, false);
                            }
                            else
                            {
                                Cassie.Message("pitch_1 Attention All Personnel The Science Team have entered the facility . All remaining jam_056_4 personnel are to .G4 ", true, true, false);

                            }
                        }
                    });

                  

                    //  PlayAudio("SerpentsHand.ogg", (byte)45f, false);

                    Timing.CallDelayed(5f, () =>
                    {
                        isScienceTeamSpawning = false;
                    });

                    //       }
                    //    !player.TemporaryData.Contains("custom_class"))

                    //   }
                }
            }
        }


        [PluginEvent(ServerEventType.PlayerSpawn)]
        void OnPlayerSpawned(Player player, RoleTypeId role)
        {
            if (RoundEvent == "PowerBlackout")
            {
                Timing.CallDelayed(1.5f, () =>
                {
                    if (!player.IsInventoryFull && player.IsHuman == true)
                    {





                        switch (UnityEngine.Random.Range(0, 1))
                        {
                            case 0: AddOrDropItem(player, ItemType.Lantern); return;
                            case 1: AddOrDropItem(player, ItemType.Flashlight); return;
                        }

                    }
                });
            }


            if (respawn_count >= 2 && isSerpentSpawning == true)
            {
                if (spawning_team == SpawnableTeamType.ChaosInsurgency && role.GetTeam() == Team.ChaosInsurgency && !player.TemporaryData.Contains("custom_class"))
                {

                    // ewww formatting went bye bye, this is probably really inefficient but it seems to fix my original problem where players would infinitely be set to tutorial or have like thousands of each ammo type
                    Timing.CallDelayed(0.3f, () =>
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


            if (respawn_count >= 2 && isScienceTeamSpawning == true)
            {
                if (spawning_team == SpawnableTeamType.NineTailedFox && role.GetTeam() == Team.FoundationForces && role.GetTeam() != Team.Scientists && spawning_team != SpawnableTeamType.ChaosInsurgency)
                {

                    // ewww formatting went bye bye, this is probably really inefficient but it seems to fix my original problem where players would infinitely be set to tutorial or have like thousands of each ammo type
                    Timing.CallDelayed(0.5f, () =>
                    {
                        if (player.Role != RoleTypeId.Scientist)
                        {
                            ChangeToScienceTeam(player, role);
                        }



                        
                    });
                }
            }



            if (respawn_count >= 2 && isSerpentSpawning == true)
            {
                if (spawning_team == SpawnableTeamType.ChaosInsurgency && role.GetTeam() == Team.ChaosInsurgency && !player.TemporaryData.Contains("custom_class"))
                {

                    // ewww formatting went bye bye, this is probably really inefficient but it seems to fix my original problem where players would infinitely be set to tutorial or have like thousands of each ammo type
                    Timing.CallDelayed(0.3f, () =>
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




            if (role == RoleTypeId.Scp079)
            {
                Timing.CallDelayed(3f, () =>
                {
                   // player.ReceiveHint("ComputerBuff: You have bonus abilities availible. Open your console (press ~ to open it) and run the command .pcbuff for more info.", 10f);
                });
            }


        }



       


        // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) received effect &6{effect}&r with an intensity of &6{intensity}&r.");

        //  public bool doesSubclassMTFexist = false; 
        [PluginEvent(ServerEventType.PlayerChangeRole)]
        void PlayerChangeRole(Player player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
        {

          


            if (player != null && oldRole != null && oldRole.RoleTypeId == RoleTypeId.Scp173 && reason == RoleChangeReason.Died)
            {
                var item = player.ReferenceHub.inventory.CreateItemInstance(new ItemIdentifier(ItemType.GrenadeHE, ItemSerialGenerator.GenerateNext()), false) as ThrowableItem;
                TimeGrenade grenadeboom = (TimeGrenade)UnityEngine.Object.Instantiate(item.Projectile, player.Position, UnityEngine.Quaternion.identity);
                grenadeboom._fuseTime = 0f;
                grenadeboom.NetworkInfo = new PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
                grenadeboom.PreviousOwner = new Footprint(player != null ? player.ReferenceHub : ReferenceHub.HostHub);
                NetworkServer.Spawn(grenadeboom.gameObject);
                grenadeboom.ServerActivate();
            }


            if (player != null && newRole == RoleTypeId.NtfSergeant)
            {

                if (Random.Range(1, 6) == 4)
                {

                    Timing.CallDelayed(0.2f, () =>
                    {
                        player.SendBroadcast("You are a MTF Boom Boom Boy. You have access to EXPLOSIVES!", 10);
                        player.AddItem(ItemType.GrenadeHE);
                        player.AddItem(ItemType.GrenadeHE);
                        // player.CustomInfo = $"<color=#00B7EB>{player.DisplayNickname}</color>" + "\n<color=#00B7EB>Nine Tailed Fox Boom Boom Boy</color>";
                     //   player.CustomInfo = $"<color=#00B7EB>{player.DisplayNickname}\nNine Tailed Fox Boom Boom Boy</color>";
                       // player.PlayerInfo.IsRoleHidden = true;
                       // player.PlayerInfo.IsNicknameHidden = true;
                       // player.PlayerInfo.IsUnitNameHidden = true;
                       // player.PlayerInfo.IsRoleHidden = true;
                      //  player.ReceiveHint(player.CustomInfo, 10);
                    });


               }
            }





            if (player != null && newRole == RoleTypeId.ChaosRifleman && isSerpentSpawning == false)
            {

                if ((Random.Range(1, 10) == 4))
                {


                    Timing.CallDelayed(0.2f, () =>
                    {
                        player.SendBroadcast("You are a Chaos Specialist. You have access to ???.", 10);

                       // player.CustomInfo = $"<color=#228B22>{player.DisplayNickname}</color>" + "\n<color=#228B22>CHAOS SPECIALIST</color>";
                       // player.CustomInfo.Replace(player.CustomInfo, $"<color=#228B22>{player.DisplayNickname}</color>" + "\n<color=#228B22>CHAOS SPECIALIST</color>");
                        //player.CustomInfo = $"<color=#228B22>{player.DisplayNickname}\nChaos Specialist</color>";
                     //   player.PlayerInfo.IsNicknameHidden = true;
                      //  player.PlayerInfo.IsUnitNameHidden = true;
                     //   player.PlayerInfo.IsRoleHidden = true;
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

                      //  player.ReceiveHint(player.CustomInfo, 10);
                    });


                }
            }
           //
           // if (player != null && newRole == RoleTypeId.Scientist)
          //  {

           //     if ((Random.Range(1, 10) == 3))
            //    {
                    

              //      Timing.CallDelayed(0.1f, () =>
              //      {
               //        player.SendBroadcast("You are a Senior Researcher.", 10);
              //       //  player.CustomInfo = $"<color=#FAFF86>{player.DisplayNickname}\nSENIOR RESEARCHER</color>";
                      //  player.PlayerInfo.IsNicknameHidden = true;
                     //   player.PlayerInfo.IsUnitNameHidden = true;
                     //   player.PlayerInfo.IsRoleHidden = true;
              //          player.ClearInventory();
                //        player.AddItem(ItemType.KeycardResearchCoordinator);
                //        player.AddItem(ItemType.Radio);
                //        player.AddItem(ItemType.Medkit);
                 //   });


              //  }
           // }

            if (player != null && fbi.Contains(player.PlayerId) && player.PlayerId == captainplayer.PlayerId)
            {
                fbi.Remove(player.PlayerId);
                player.TemporaryData.Remove("custom_class");
                captainplayer = null;
            }

            if (player != null && fbi.Contains(player.PlayerId))
            {
                fbi.Remove(player.PlayerId);
                player.TemporaryData.Remove("custom_class");
            }

            if (player != null && sci.Contains(player.PlayerId))
            {
                sci.Remove(player.PlayerId);
               // player.TemporaryData.Remove("custom_class");
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


            if (player != null)
            {
                if (randomNumber > cfg.EventRarity)
                    return;

                if (reason != RoleChangeReason.RoundStart && reason != RoleChangeReason.LateJoin)
                    return;


                if (RoundEvent == "ChaosInvasion")
                {
                    Timing.CallDelayed(1.5f, () =>
                    {
                        if (newRole == RoleTypeId.FacilityGuard)
                        {
                            player.SetRole(RoleTypeId.ChaosRifleman);



                        }
                    });
                }

                if (RoundEvent == "ArmedDClass")
                {
                    Timing.CallDelayed(1.5f, () =>
                    {
                        if (newRole == RoleTypeId.ClassD)
                        {
                            AddOrDropFirearm(player, ItemType.GunCOM18, true);



                        }
                    });
                }

                if (RoundEvent == "SpecialOps")
                {
                    Timing.CallDelayed(1.5f, () =>
                    {
                        if (newRole == RoleTypeId.FacilityGuard)
                        {
                            player.SetRole(RoleTypeId.NtfPrivate);
                        }
                    });
                }

               
                if (RoundEvent == "PowerBlackout")
                {
                    Timing.CallDelayed(1.5f, () =>
                    {
                        if (!player.IsInventoryFull && player.IsHuman == true && newRole != RoleTypeId.Overwatch && newRole != RoleTypeId.Spectator)
                    {
                       
                            
                          


                            switch (UnityEngine.Random.Range(0, 1))
                            {
                                case 0: AddOrDropItem(player, ItemType.Lantern); return;
                                case 1: AddOrDropItem(player, ItemType.Flashlight); return;
                            }

                        }
                   });
                }
                /*
                 if (RoundEvent == "UnstablePower")
                 {
                     p.SendBroadcast("<color=#228B22>EVENT:</color> Unstable Facility Power. ", 13, Broadcast.BroadcastFlags.Normal, false);
                 }
                
                if (RoundEvent == "NukeDisabled")
                {
                    
                }
                */

                
               
            }

        }

        [PluginEvent(ServerEventType.PlayerDeath)]
        void OnPlayerDied(Player player, Player attacker, DamageHandlerBase damageHandler)
        {
            Config config = Plugin.Singleton.Config;
            if (player != null)
            {
                player.PlayerInfo.IsUnitNameHidden = false;
                player.PlayerInfo.IsNicknameHidden = false;
                player.PlayerInfo.IsRoleHidden = false;
                player.CustomInfo = string.Empty;
            }


          


            if (player != null)
            {
                if (sci.Contains(player.PlayerId))
                {
                    sci.Remove(player.PlayerId);
                }
                if (fbi.Contains(player.PlayerId) && player.PlayerId != captainplayer.PlayerId) 
                {
                    fbi.Remove(player.PlayerId);
                    player.TemporaryData.Remove("custom_class");
                }
                if (fbi.Contains(player.PlayerId) && player.PlayerId == captainplayer.PlayerId) 
                {
                    fbi.Remove(player.PlayerId);
                    player.TemporaryData.Remove("custom_class");
                    captainplayer = null;
                }



            }

            int alive_count = 0;
                Player target = null;
                alive_count = 0;
                foreach (var p in Player.GetPlayers())
                {
                    if (p.IsHuman && p.Role != RoleTypeId.Overwatch && p.Role != RoleTypeId.Tutorial && p.Role != RoleTypeId.Spectator)
                    {
                        alive_count++;
                        target = p;
                    }
                }
                if (alive_count == 1)
                {
                target.ReceiveHint(config.LastOneAliveHint, 10);
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
            if (player != null)
            {
                player.PlayerInfo.IsRoleHidden = false;
                player.PlayerInfo.IsNicknameHidden = false;
                player.PlayerInfo.IsRoleHidden = false;
                player.CustomInfo = string.Empty;
            }
            if (player != null && fbi.Contains(player.PlayerId) && player.PlayerId == captainplayer.PlayerId)
            {
                fbi.Remove(player.PlayerId);
                player.TemporaryData.Remove("custom_class");
                captainplayer = null;
            }
            if (player != null && fbi.Contains(player.PlayerId))
            {
                //  player.DisplayNickname = player.Nickname;
                fbi.Remove(player.PlayerId);
                player.TemporaryData.Remove("custom_class");
                player.DisplayNickname = null;
            }
            if (player != null && sci.Contains(player.PlayerId))
            {
                sci.Remove(player.PlayerId);
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


        [PluginEvent(ServerEventType.PlayerEscape)]
        bool OnPlayerEscaped(Player plr, RoleTypeId role)
        {
            if (sci.Contains(plr.PlayerId))
            {
                return false;
            }
            else return true;
        }

        [PluginEvent(ServerEventType.LczDecontaminationStart)]
        void OnLczDecontaminationStarts()
        {
            //Config config = Plugin.Singleton.Config;
          //  Log.Debug("Started LCZ decontamination.");
            if (new System.Random().Next(10) == 1)
            {
                if (new System.Random().Next(4) == 1)
                {
                    Timing.CallDelayed(9.5f, () =>
                    {
                        Cassie.Message("SCP 9 9 9 Lost in Decontamination Sequence .G3", true, true, true);
                    });
                }
                else {


                    if (new System.Random().Next(4) == 1)
                    {
                        Timing.CallDelayed(9.5f, () =>
                        {
                            Cassie.Message("SCP 9 6 6 Lost in Decontamination Sequence", true, true, true);
                        });
                    }
                    else
                    { 
                        Timing.CallDelayed(9.5f, () =>
                    {
                        Cassie.Message("SCP 1 0 4 8 Lost in Decontamination Sequence .G5", true, true, true);
                    });

                    }
                }
                
                
            }

        }


        [PluginEvent(ServerEventType.RoundEnd)]
        void OnRoundEnded(RoundSummary.LeadingTeam leadingTeam)
        {
            haveSerpentsSpawned = false;
            respawn_count = 0;
            fbi.Clear();
            captainplayer = null;
            sci.Clear();
        }




        [PluginEvent(ServerEventType.RoundRestart)]
        void OnRoundRestart()
        {
            haveSerpentsSpawned = false;
            respawn_count = 0;
            fbi.Clear();
            sci.Clear();
            captainplayer = null;
        }


        [PluginEvent]
        public void OnScp096AddTarget(Scp096AddingTargetEvent ev)
        {
           // Log.Info($"Player &6{ev.Target.Nickname}&r (&6{ev.Target.UserId}&r) {(ev.IsForLook ? "look" : "shoot")}  player &6{ev.Player.Nickname}&r (&6{ev.Player.UserId}&r) and was added to the SCP-096 target list");
            if (!fbi.Contains(ev.Target.PlayerId))
            {
               // ev.Target.TemporaryData.Add("chasemusic", this);
             //   ReferenceHub Dummy = AddDummy();
              //  PlayPlayerAudio096(ev.Target, "scp096chase.ogg", (byte)85f, Dummy);
            }
        }

        [PluginEvent]
        public void OnScp096Enrage(Scp096EnragingEvent ev)
        {
           // Log.Info($"Player &6{ev.Player.Nickname}&r (&6{ev.Player.UserId}&r) went into a state of rage for {ev.InitialDuration} seconds");
        }

        [PluginEvent]
        public void OnScp096CalmDown(Scp096ChangeStateEvent ev)
        {
           // Log.Info($"Player &6{ev.Player.Nickname}&r (&6{ev.Player.UserId}&r) changed its state to {ev.RageState} as SCP-096");
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


        // SCP-294 Drinks
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
        public static HashSet<ushort> colas_medusa = new HashSet<ushort>();
        public static HashSet<ushort> colas_windex = new HashSet<ushort>();
        public static HashSet<ushort> colas_crazy = new HashSet<ushort>();
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
        public static HashSet<ushort> greenjuice = new HashSet<ushort>();
        public static HashSet<ushort> choccymilk = new HashSet<ushort>();
        public static HashSet<ushort> lemonade = new HashSet<ushort>();
        public static HashSet<ushort> lava = new HashSet<ushort>();
        public static HashSet<ushort> balls = new HashSet<ushort>();



        // CUSTOM ITEMS
        public static HashSet<ushort> resurrection_pills = new HashSet<ushort>();
        public static HashSet<ushort> super_pills = new HashSet<ushort>();
        public static HashSet<ushort> scp500s = new HashSet<ushort>();
        public static HashSet<ushort> hats = new HashSet<ushort>();
        public static HashSet<ushort> scp1499 = new HashSet<ushort>();






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
                        player.SendConsoleMessage("List of SCP-294 Drinks: oxygen, speed, SCP-207, Coffee, Espresso, GoldenAtomKick, NuclearKick, godmode, nuclearkick, Invisibility, scp268, Me, Tea, Horror, PocketDimension, Borgor, Cheeserburger, Antimatter, Nuke, 049, Zombie, CherryAtomKick, HealthPotion, grenade, pinkcandy, Boom, SCP-173, Peanut, Saltwater, Ocean, Teleportation, Teleport, Escape, Windex, Medusa, SCP-330, Candy, SeveredHands, BEPIS, Small, Big, grow, LeafLover, Water, Slushy, Ghost, Cold, Ice, Death, Metal, Steel, RazorBlade, Oil, Freedom, Bose-Einstein, Condensate, QuantumGas, white, slime, scp1853, scp-1853, choccymilk, lava, lemonade");
                        player.SendConsoleMessage("DUPLICATE ENTRIES ARE INCLUDED. SOME MAY BE CASE-SENSITIVE; MAKE SURE TO DOUBLE CHECK CAPS / LOWERCASE.","white");
                    }
                    if (ItemType.Coin.Equals(player.ReferenceHub.inventory.NetworkCurItem.TypeId) && player.Room.name == "EZ_Smallrooms2" && arguments.First().ToLower() != "list" || player.Room.name == "LCZ_TCross (11)")
                    {



                            

                       
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
                        if (arguments.First().ToLower() == "oil" || arguments.First().ToLower() == "freedom")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a can of oil.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            colas_oil.Add(thiscola.ItemSerial);
                            ReferenceHub TempDummy = AddDummy();
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "choccymilk" || arguments.First().ToLower() == "milk")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";

                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a bottle of choccy milk.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            choccymilk.Add(thiscola.ItemSerial);
                            ReferenceHub TempDummy = AddDummy();
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "lemonade" || arguments.First().ToLower() == "pee")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";

                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a bottle of lemonade.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            lemonade.Add(thiscola.ItemSerial);
                            ReferenceHub TempDummy = AddDummy();
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "lava" || arguments.First().ToLower() == "magma")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";

                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a bottle of lava.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            lava.Add(thiscola.ItemSerial);
                            ReferenceHub TempDummy = AddDummy();
                            PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "balls" || arguments.First().ToLower() == "scp-018")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";

                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a bottle of balls.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            balls.Add(thiscola.ItemSerial);
                            ReferenceHub TempDummy = AddDummy();
                            PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "metal" || arguments.First().ToLower() == "steel" || arguments.First().ToLower() == "razorblade")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a cup of metal.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            colas_metal.Add(thiscola.ItemSerial);

                            PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f, TempDummy);

                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "cold" || arguments.First().ToLower() == "ice")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a cup of ice.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.AntiSCP207);
                            PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f, TempDummy);
                            colas_cold.Add(thiscola.ItemSerial);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First() == "Ghost" || arguments.First() == "ghost")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a cup of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            colas_ghost.Add(thiscola.ItemSerial);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "oxygen" || arguments.First() == "Oxygen")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a cup of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            colas_oxygen.Add(thiscola.ItemSerial);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "Speed" || arguments.First() == "speed")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise pitched up to high levels and dispensed you a cup of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            colas_speed.Add(thiscola.ItemSerial);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "Coffee" || arguments.First() == "Espresso" || arguments.First() == "coffee" || arguments.First() == "espresso")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a cup of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_coffee.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "GoldenAtomKick" || arguments.First() == "goldenatomkick" || arguments.First() == "goldatom" || arguments.First() == "goldenatomkick")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a glittering sound and dispensed you a can of Golden Atom Kick.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.AntiSCP207);
                            colas_atomkick.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First().ToLower() == "god" || arguments.First() == "NuclearKick" || arguments.First() == "godmode" || arguments.First() == "nuclearkick")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a small noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_nuclearkick.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "Invisibility" || arguments.First() == "invis" || arguments.First() == "scp268" || arguments.First() == "invisibility")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a noise of fabric being cut and dispensed you a cup of SCP-268.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_invis.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "Me" || arguments.First() == "Myself" || arguments.First() == "me" || arguments.First() == "I" || arguments.First().ToLower() == "blood")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud rumbling noise and dispensed you a cup of yourself.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_me.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "Tea" || arguments.First() == "tea" || arguments.First() == "teadrink" || arguments.First() == "t")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a cup of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_tea.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "Horror" || arguments.First() == "horror" || arguments.First() == "scp106" || arguments.First() == "PocketDimension")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a crunchy noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_horror.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "Borgor" || arguments.First() == "borgor" || arguments.First() == "Cheeseburger" || arguments.First() == "cheseburger")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_borgor.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "antimatter" || arguments.First() == "Antimatter" || arguments.First() == "Nuke" || arguments.First() == "nuke" || arguments.First().ToLower() == "death")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_antimatter.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "049" || arguments.First() == "049-2" || arguments.First() == "Zombie" || arguments.First() == "zombie" || arguments.First() == "LeafLover" || arguments.First() == "leaflover" || arguments.First() == "Leaflover")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_zombie.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "CherryAtomKick" || arguments.First() == "cherryatomkick" || arguments.First() == "CherryatomKick" || arguments.First() == "atomkickcherry" || arguments.First() == "HealthPotion" || arguments.First() == "healthpotion" || arguments.First() == "potion" || arguments.First() == "Potion")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_cherryatomkick.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "grenade" || arguments.First() == "Grenade" || arguments.First() == "boom" || arguments.First() == "Pinkcandy" || arguments.First() == "Boom" || arguments.First() == "pinkcandy" || arguments.First().ToLower() == "scp-330")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.AntiSCP207);
                            colas_explosion.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "SCP-173" || arguments.First() == "scp173" || arguments.First() == "Peanut" || arguments.First() == "peanut" || arguments.First() == "173")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a crunchy noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_peanut.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "Saltwater" || arguments.First() == "saltwater" || arguments.First() == "SaltWater" || arguments.First() == "salt" || arguments.First() == "Ocean" || arguments.First() == "ocean")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_saltwater.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "Gasoline" || arguments.First() == "gas" || arguments.First() == "Petrol" || arguments.First() == "pterol" || arguments.First() == "gasoline" || arguments.First() == "Gascan")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_gas.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First().ToLower() == "teleport" || arguments.First().ToLower() == "random" || arguments.First().ToLower() == "teleportation" || arguments.First().ToLower() == "tp" || arguments.First().ToLower() == "escape")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_teleportation.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First().ToLower() == "windex" || arguments.First().ToLower() == "wind" || arguments.First().ToLower() == "cleaningsupplies")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a glimmering noise and dispensed you a bottle of Windex.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_windex.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First().ToLower() == "medusa" || arguments.First().ToLower() == "rock" || arguments.First().ToLower() == "tank")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a glimmering noise and dispensed you a bottle of Medusa.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_medusa.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "Slushy" || arguments.First() == "slushy" || arguments.First() == "slush" || arguments.First() == "Slush")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a glimmering noise and dispensed you a bottle of Sour Patch Kids Slushy.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_sour_patch_kids_slushy.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "Crazy" || arguments.First() == "crazy" || arguments.First() == "Crazy?" || arguments.First() == "rubberroom" || arguments.First() == "arubberroom" || arguments.First() == "Iwascrazyonce")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine began to say Crazy? I was crazy once, they locked me in a room, a rubber room, a rubber room with rats, and rats make me crazy. In a robotic voice and dispensed you a drink.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_crazy.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "Small" || arguments.First() == "small" || arguments.First() == "smol" || arguments.First() == "Tiny" || arguments.First() == "tiny")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a high-pitched noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_small.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "Bepis" || arguments.First() == "bepis" || arguments.First() == "BEPIS")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a odd noise and dispensed you a can of Bepis.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_bepis.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "SCP-207" || arguments.First() == "scp207" || arguments.First() == "207" || arguments.First() == "cola" || arguments.First() == "Cola")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a high-pitched noise and dispensed you a bottle of cola.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_scp207.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });
                        }
                        if (arguments.First().ToLower() == "scp-1853" || arguments.First().ToLower() == "1853" || arguments.First().ToLower() == "slime")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a weird noise and dispensed a vile of SCP-1853.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP1853);
                            greenjuice.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "water" || arguments.First() == "Water" || arguments.First() == "h2o")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_water.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "flamingo" || arguments.First() == "Flamingo" || arguments.First() == "1507" || arguments.First() == "scp-1507" || arguments.First() == "SCP-1507")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a weird noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_flamingo.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "big" || arguments.First() == "Big" || arguments.First() == "large" || arguments.First() == "Large" || arguments.First() == "grow")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made a loud noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_big.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });
                        }
                        if (arguments.First().ToLower() == "quantamgas" || arguments.First().ToLower() == "bose-einstein" || arguments.First().ToLower() == "condensate")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with SCP-294, the machine made the sound of gas being released and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.AntiSCP207);
                            colas_quantam.Add(thiscola.ItemSerial);
                            PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                RemoveDummy096(TempDummy);
                            });
                        }



                    }
                }
                response = " If you were not holding a coin or did not enter a valid drink, and you did not get anything. You should run this command again. Please also make sure you are in the same room as the machine. You can get a list of valid drinks by running the command .scp294 list";
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
            else if (item.ItemTypeId == ItemType.SCP1853 && greenjuice.Contains(item.ItemSerial))
            {

                Timing.CallDelayed(3f, () =>
                {

                    
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You opened the green juice and spilt it. Oops!", 3);
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
             //       plr.ClearInventory();
          //          plr.ReferenceHub.inventory.CreateItemInstance(new ItemIdentifier(ItemType.GrenadeHE, ItemSerialGenerator.GenerateNext()), false);
                    if (InventoryItemLoader.AvailableItems.TryGetValue(ItemType.GrenadeHE, out ItemBase grenadeBase))
                    {
                        ThrowableItem grenadeThrowable = grenadeBase as ThrowableItem;
                        ThrownProjectile grenadeProjectile = UnityEngine.Object.Instantiate(grenadeThrowable.Projectile);
                        NetworkServer.Spawn(grenadeProjectile.gameObject);
                    }

                    //plr.EffectsManager.EnableEffect<Bleeding>(60, true);
                    // plr.Heal(50);
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);
                  
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
                        plr.EffectsManager.EnableEffect<Sinkhole>(15, true);
                        plr.EffectsManager.EnableEffect<Deafened>(15, true);
                        plr.EffectsManager.ChangeState("FogControl", 255, 15f, false);
                        //plr.EffectsManager.DisableEffect<Deafened>();
                        Timing.CallDelayed(15f, () =>
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
                   
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You drank the saltwater. Salty!", 3);
                    //  plr.EffectsManager.EnableEffect<Invigorated>(5, false);
                    //  plr.IsGodModeEnabled = true;
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            if (item.ItemTypeId == ItemType.SCP207 && colas_oil.Contains(item.ItemSerial))
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
            if (item.ItemTypeId == ItemType.AntiSCP207 && colas_cold.Contains(item.ItemSerial))
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
                  
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("Timeout for you!", 3);
                    UnityEngine.Vector3 plrpos = new UnityEngine.Vector3(40f, 1014f, -32.60f);
                    UnityEngine.Vector3 tppos = new UnityEngine.Vector3(40f, 1014f, -32.60f);
                    // plrpos = plr.Position;
                    plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    //  plr.ReceiveHint("You drank a cup of [REDACTED]. Your items magically disappeared!", 3);
                 
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
            if (item.ItemTypeId == ItemType.SCP207 && choccymilk.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                   
                    // plr.EffectsManager.EnableEffect<MovementBoost>(3, true);
                    //   plr.EffectsManager.ChangeState<MovementBoost>(255, 4, false);
                    // plr.EffectsManager.EnableEffect<Invisible>(10, true);
                    // plr.Heal(50);
                    //   plr.Damage(damageHandlerBase);
                   
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                   
                    plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                      plr.ReceiveHint("You drank a cup of Choccy Milk. You feel much better.", 3);

                    plr.Health = plr.MaxHealth;
                    

                    //  plr.EffectsManager.EnableEffect<Invigorated>(5, false);
                    //  plr.IsGodModeEnabled = true;
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            if (item.ItemTypeId == ItemType.SCP207 && lemonade.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {

                  
                    
                    plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                     plr.ReceiveHint("You drank a cup of lemonade. Refreshing!", 3);




                    //  plr.EffectsManager.EnableEffect<Invigorated>(5, false);
                    //  plr.IsGodModeEnabled = true;
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            if (item.ItemTypeId == ItemType.SCP207 && lava.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {



                    plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    plr.ReceiveHint("You drank a cup of lava. You begin to melt!", 3);




                      plr.EffectsManager.EnableEffect<Burned>(30, false);
                    Timing.CallDelayed(5f, () =>
                    {
                        plr.Kill("Melted.");
                    });
                    //  plr.IsGodModeEnabled = true;
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            if (item.ItemTypeId == ItemType.SCP207 && balls.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {



                    plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    plr.ReceiveHint("I hope you realize what you've done.", 3);



                    Cassie.Message("XMAS_BOUNCYBALLS XMAS_BOUNCYBALLS",true,true,false);
                   // plr.EffectsManager.EnableEffect<Burned>(30, false);
                    Timing.CallDelayed(5f, () =>
                    {
                        plr.Kill("so hold your head and pray.");
                    });
                    //  plr.IsGodModeEnabled = true;
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);

                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.AntiSCP207 && !colas_atomkick.Contains(item.ItemSerial) && !colas_cold.Contains(item.ItemSerial) && !colas_explosion.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

               // Timing.CallDelayed(3.4f, () =>
              //  {
              //     // plr.Health = plr.Health + 15;
              //  });

            }

            else if (item.ItemTypeId == ItemType.SCP1576 && THEButton.Contains(item.ItemSerial) && buttonused == false)
            {
                //  Log.Debug("SCP-268 was used.");
                buttonused = true;
                Timing.CallDelayed(4.1f, () =>
                {
                   // plr.RemoveItem(plr.CurrentItem);
                   // plr.CurrentItem = null;
                    List<Player> Playerss = Player.GetPlayers();

                        plr.ReceiveHint("You pressed <color=#C50000>THE BUTTON</color>. Your fate is being decided...");
                    if (new System.Random().Next(2) == 1)
                    {

                        foreach (var randplr in Playerss)
                        {
                            if (!randplr.IsSCP == true && randplr.Role != RoleTypeId.Scp079 && randplr.Role != RoleTypeId.Spectator && randplr.Role != RoleTypeId.Overwatch && randplr.PlayerId != plr.PlayerId)
                            {


                                foreach (var allplrs in Playerss)
                                {
                                    if (allplrs.Role != RoleTypeId.Overwatch)
                                    {
                                        allplrs.SendBroadcast("WARNING: The button is chosing a random player to have their fate sealed!", 3);
                                        Timing.CallDelayed(3f, () =>
                                        {
                                            allplrs.SendBroadcast("WARNING: The button is chosing a random player to have their fate sealed! Checking in 3...", 1);
                                            Timing.CallDelayed(1f, () =>
                                            {
                                                allplrs.SendBroadcast("WARNING: The button is chosing a random player to have their fate sealed! Checking in 2...", 1);
                                                Timing.CallDelayed(1f, () =>
                                                {
                                                    allplrs.SendBroadcast("WARNING: The button is chosing a random player to have their fate sealed! Checking in 1...", 1);
                                                    Timing.CallDelayed(1f, () =>
                                                    {
                                                        allplrs.SendBroadcast("WARNING: The button is chosing a random player to have their fate sealed! Checking in 0...", 1);
                                                        Timing.CallDelayed(1f, () =>
                                                        {
                                                            allplrs.SendBroadcast("WARNING: The button is chosing a random player to have their fate sealed! Checking in 0...", 1);
                                                            allplrs.SendBroadcast($"The player selected is... {randplr.Nickname}! Say goodbye!", 3);

                                                            randplr.Kill("Better luck next time!");
                                                            return;
                                                        });
                                                    });
                                                });
                                            });
                                        });
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        plr.Kill("Better luck next time!");
                        foreach (var randplr in Playerss)
                        {
                            if (randplr.Role != RoleTypeId.Overwatch)
                            {
                                randplr.SendBroadcast("NOTICE: A player attempted to use <color=#C50000>THE BUTTON</color> and was killed! Better luck next time!", 5);
                            }
                        }
                    }
                });

            }
            else if (item.ItemTypeId == ItemType.SCP500 && resurrection_pills.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(1.36f, () =>
                {
                    plr.ReceiveHint("You swallowed the Resurrection Pills.");
                    foreach (var plrr in Player.GetPlayers())
                    {
                        if (plrr.Role == RoleTypeId.Spectator)
                        {
                            if (plr.Role == RoleTypeId.Tutorial && fbi.Contains(plr.PlayerId))
                            {
                                ChangeToTutorial(plrr, plr.Role);
                                Timing.CallDelayed(1f, () =>
                                {
                                    plrr.Position = plr.Position;
                                    plrr.ReceiveHint($"{plr.DisplayNickname} has respawned you using the resurrection pills!");
                                });
                            }
                            else
                            {
                                plrr.Role = plr.Role;
                                plrr.Position = plr.Position;
                                plrr.ReceiveHint($"{plr.DisplayNickname} has respawned you using the resurrection pills!");
                            }
                        }
                    }
                });

            }
            else if (item.ItemTypeId == ItemType.SCP500 && super_pills.Contains(item.ItemSerial))
            {
              

                Timing.CallDelayed(1.36f, () =>
                {
                    plr.ReceiveHint("You swallowed the Super Pill.");
                    plr.EffectsManager.ChangeState("MovementBoost", 2, 15, true);
                    plr.EffectsManager.ChangeState("Invigorated", 1, 15, true);
                    plr.EffectsManager.ChangeState("DamageReduction", 1, 15, true);
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
                Timing.CallDelayed(3.4f, () =>
                {
                    plr.ClearBroadcasts();
                    plr.ReceiveHint("You drank a bottle of cola. You now feel faster...", 3);
                });
            }
            else if (item.ItemTypeId == ItemType.SCP330)
            {     
                foreach (ItemBase itemba in plr.Items)
                {
                    if ((itemba is Scp330Bag bag))
                    {

                        int candyCount = bag.Candies.Count - 1;
                        CandyKindID id = bag.Candies[candyCount];

                        if (id.ToString().ToLower() == "green")
                        {
                         //   ReferenceHub GCandyDummy = AddDummy();
                        //    PlayPlayerAudio096(plr, "windows.ogg", (byte)85f, GCandyDummy);
                         //   Timing.CallDelayed(28f, () =>
                         //   {
                         //       RemoveDummy096(GCandyDummy);
                        //    });
                        }
                    }
                }
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_teleportation.Contains(item.ItemSerial))
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
                    plr.ClearBroadcasts();
                    if (plr.IsAlive && plr.RoleBase is IFpcRole role)
                    {
                        plr.EffectsManager.EnableEffect<PocketCorroding>();
                        var position = Scp106PocketExitFinder.GetBestExitPosition(role);
                        plr.EffectsManager.DisableEffect<PocketCorroding>();
                        plr.EffectsManager.DisableEffect<Corroding>();
                        plr.Position = position;
                        

                    }

                    /*
                    List<Player> Playerss = Player.GetPlayers();

                    foreach (var randplr in Playerss)
                    {
                        if (randplr.IsSCP == true && randplr.Role != RoleTypeId.Scp079)
                        {
                            // plr.Position = randplr.Position;
                        }
                    }
                    */
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
                else if (!newItemBase == false && choccymilk.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of choccy milk.", 3);

                }
                else if (!newItemBase == false && lemonade.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of lemonade.", 3);

                }
                else if (!newItemBase == false && lava.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of lava.", 3);

                }
                else if (!newItemBase == false && balls.Contains(newItemBase.ItemSerial))
                {
                    plr.ClearBroadcasts();
                    //  plr.SendBroadcast("You equipped a cup of pure oxygen.", 5);
                    plr.ReceiveHint("You equipped a bottle of balls.", 3);

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


                else if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP268)
                {
                   

                     if (new System.Random().Next(5) == 1 && !scp1499.Contains(newItemBase.ItemSerial) && !hats.Contains(newItemBase.ItemSerial))
                       {
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

                else if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP1576)
                {


                    List<Player> Playerss = Player.GetPlayers();
                    Player randomPlr = Playerss.RandomItem();
                    if (THEButton.Contains(newItemBase.ItemSerial))
                    {
                        plr.ReceiveHint("You equipped <color=#C50000>THE BUTTON</color>. Use it for a suprise!", 3);

                       // List<Player> Playerss = Player.GetPlayers();

                        


                    }


                }

                else if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.Lantern && RoundEvent != "PowerBlackout")
                {
                   

                    if (new System.Random().Next(5) == 1 && !ghostLantern.Contains(newItemBase.ItemSerial) && !normalLantern.Contains(newItemBase.ItemSerial))
                    {
                        ghostLantern.Add(newItemBase.ItemSerial);
                        plr.ReceiveHint("You equipped the Ghastly Lantern. You can now phase through doors, at a cost...", 3);

                    }
                    else if (!normalLantern.Contains(newItemBase.ItemSerial) && !ghostLantern.Contains(newItemBase.ItemSerial))
                    {
                        hats.Add(newItemBase.ItemSerial);
                    }
                    else if (ghostLantern.Contains(newItemBase.ItemSerial))
                    {
                        plr.ReceiveHint("You equipped the Ghastly Lantern. You can now phase through doors, at a cost...", 3);
                    }


                }

                Int32 Random = new System.Random().Next(8);
              

                
                if ((Random == 6 || Random == 1) && !newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP500 && !super_pills.Contains(newItemBase.ItemSerial) && !scp500s.Contains(newItemBase.ItemSerial) && !resurrection_pills.Contains(newItemBase.ItemSerial) && newItemBase.ItemTypeId == ItemType.SCP500)
                {
                    plr.ClearBroadcasts();

                    if (new System.Random().Next(5) == 1 && !resurrection_pills.Contains(newItemBase.ItemSerial) && !scp500s.Contains(newItemBase.ItemSerial))
                    {
                        resurrection_pills.Add(newItemBase.ItemSerial);
                        plr.ReceiveHint("You equipped the Resurrection Pill.", 3);

                    }
                    else if (resurrection_pills.Contains(newItemBase.ItemSerial))
                    {
                        plr.ReceiveHint("You equipped the Resurrection Pill.", 3);
                    }
                }
                 
                if ((Random == 5 || Random == 3) && !newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP500 && !super_pills.Contains(newItemBase.ItemSerial) && !scp500s.Contains(newItemBase.ItemSerial) &&!resurrection_pills.Contains(newItemBase.ItemSerial) && newItemBase.ItemTypeId == ItemType.SCP500)
                {
                    plr.ClearBroadcasts();

                    if (new System.Random().Next(10) == 3 && !super_pills.Contains(newItemBase.ItemSerial) && !scp500s.Contains(newItemBase.ItemSerial))
                    {
                        super_pills.Add(newItemBase.ItemSerial);
                        plr.ReceiveHint("You equipped the Super Pill.", 3);

                    }
                  
                    else if (super_pills.Contains(newItemBase.ItemSerial))
                    {
                        plr.ReceiveHint("You equipped the Super Pill.", 3);
                    }


                }

                if ((Random == 4 || Random == 7 || Random == 2 || Random == 8) && !newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP500 && !super_pills.Contains(newItemBase.ItemSerial) && !scp500s.Contains(newItemBase.ItemSerial) && !resurrection_pills.Contains(newItemBase.ItemSerial) && newItemBase.ItemTypeId == ItemType.SCP500)
                {
                    scp500s.Add(newItemBase.ItemSerial);

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
        public class givethebutton : ICommand
        {
            public string Command { get; } = "givethebutton";

            public string[] Aliases { get; } = new string[] { };

            public string Description { get; } = "gives the button to you";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player;
                if (Player.TryGet(sender, out player))
                {
                    ItemBase adminscp1499item = player.AddItem(ItemType.SCP1576);
                    THEButton.Add(adminscp1499item.ItemSerial);
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

        static bool coolDowned2 = true;


        //        public List<ItemType> list = new List<ItemType>
        //if (ItemType.Coin.Equals(player.ReferenceHub.inventory.NetworkCurItem.TypeId) && player.Room.name == "EZ_Smallrooms2" || player.Room.name == "LCZ_TCross (11)")



        [CommandHandler(typeof(ClientCommandHandler))]
        public class scp1025 : ICommand
        {
            public string Command { get; } = "scp1025";

            public string[] Aliases { get; } = new string[] { "1025", "scp-1025", "book", "borgor" };

            public string Description { get; } = "scp-1025 command";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player = Player.Get(((CommandSender)sender).SenderId);


                if (Player.TryGet(sender, out player))
                {
                    if (player.Room.name == "LCZ_372 (18)")
                    {
                        //  public List<PlayerEff> list = new List<CustomPlayerEffects> { CustomPlayerEffects.AntiScp207, CustomPlayerEffects;. }

                        //  player.EffectsManager.EnableEffect<CustomPlayerEffects>
                        // StatusEffectBase Randomeffect = player.ReferenceHub.playerEffectsController.AllEffects.RandomItem();
                        // player.EffectsManager.EnableEffect<Randomeffect>()
                        StatusEffectBase effect = player.ReferenceHub.playerEffectsController.AllEffects.RandomItem();
                        effect.ServerSetState(1, 9999, false);
                        player.SendBroadcast($"You read the next page of SCP-1025 and got the effect {effect.name}.", 5);

                       // player.TemporaryData.Add("coolDown",player);


                        ReferenceHub TempDummy = AddDummy();
                        PlayPlayerAudio096(player, "page.ogg", (byte)85f, TempDummy);
                        Timing.CallDelayed(2f, () =>
                        {
                            RemoveDummy096(TempDummy);
                        });


                    }
                    response = "";
                    return true;
                }

                else
                {
                    response = "failed";
                    return false;
                }

            }
        }




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
                  //  player.SendConsoleMessage(".blackout - (LOCKED TO TIER 3+) Forces a facility power failure. BLACKS OUT THE ENTIRE FACILITY FOR 15-20 seconds.");
                    return true;
                }
                else
                {
                    response = "You can't run that.";
                    return false;
                }

            }
        }






        [CommandHandler(typeof(ClientCommandHandler))]
        public class infocommand : ICommand
        {
            public string Command { get; } = "info";

            public string[] Aliases { get; } = new string[] { "scpinfo" };

            public string Description { get; } = "Help command";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player = Player.Get(((CommandSender)sender).SenderId);


                if (Player.TryGet(sender, out player))
                {

                    if (arguments.First().ToLower() == "127" || arguments.First().ToLower() == "scp-127" || arguments.First().ToLower() == "scp127")
                    {
                        player.SendConsoleMessage("Description: SCP-127, upon first glance, appears to be a standard MP5K-PDW submachine gun. Tests have revealed that aside from the outer steel and polymer shell, the entirety of the firearm is organic and alive. The weapon's ammunition initially appeared to be human-like teeth. However, DNA testing of the \"bullets\" resulted in no match to any known species on Earth.","white");
                        player.SendConsoleMessage("Notes: Ingame, SCP-127 takes the form of a cut down MTF-E11 rifle. It regens ammo much faster than the one in the SCP universe does for gameplay purposes. It will exlusively spawn in Heavy Containment Zone's Armoury.","white");
                    }
                    else if (arguments.First().ToLower() == "1162" || arguments.First().ToLower() == "scp-1162" || arguments.First().ToLower() == "scp1162" || arguments.First().ToLower() == "hole" || arguments.First().ToLower() == "thehole")
                    {
                        player.SendConsoleMessage("Description: SCP-1162 is a hole in a cinderblock wall. Its anomalous properties are activated when a sentient being reaches into the hole to the depth that the girth of their arm allows. At this point their fingers touch a solid surface similar in feel to the current location of SCP-1162, and they discover an item small enough to fit through the hole below their fingertips.","white");
                        player.SendConsoleMessage("Notes: This SCP was removed from the scp wiki by the original author. Details are from the SCP:CB wiki. Found in SCP 173's OLD chamber in light containment zone.","white");
                    }
                    else if (arguments.First().ToLower() == "scp1499" || arguments.First().ToLower() == "1499" || arguments.First().ToLower() == "scp1499")
                    {
                        player.SendConsoleMessage("Description: SCP-1499 is a Soviet GP-5 gas mask. A seal test performed on the object suggests that the object retains its original functionality. The anomalous effects of SCP-1499 activate when a human places SCP-1499 on their head. Approximately one second after SCP-1499 is fully secured on the subject's head, the subject vanishes from view, and is no longer detectable. The subject reports no feeling of motion at this time.","white");
                        player.SendConsoleMessage("Notes: This SCP ingame takes the form of SCP-268 (The Hat) due to me not being able to replace models or textures. If the player is in an elevator, it will NOT teleport the user to the (dimension). It has a very very low chance to replace an SCP-268.","white");
                    }
                    else if (arguments.First().ToLower() == "scp294" || arguments.First().ToLower() == "294" || arguments.First().ToLower() == "scp-294")
                    {
                        player.SendConsoleMessage("Description: Item SCP-294 appears to be a standard coffee vending machine, the only noticeable difference being an entry touchpad with buttons corresponding to an English QWERTY keyboard. Upon depositing fifty cents US currency into the coin slot, the user is prompted to enter the name of any liquid using the touchpad.","white");
                        player.SendConsoleMessage("Notes: This SCP is still the favorite of many of you on this server. I orgininally wanted to make it to test my c# knowledge and also from my brain always thinking about whatever. Reminder that you can always get a list of drinks by running the command .scp294 list","white");
                    }
                    else if (arguments.First().ToLower() == "pills" || arguments.First().ToLower() == "scp-500" || arguments.First().ToLower() == "scp500" || arguments.First().ToLower() == "500")
                    {
                        player.SendConsoleMessage("Notes: no description here lol, however the server has 2 different alternative pills you can get, one being the resurrection pills and the super pills.","white");
                    }
                    else if (arguments.First().ToLower() == "scp-575" || arguments.First().ToLower() == "575" || arguments.First().ToLower() == "scp575" || arguments.First().ToLower() == "scp575-b" || arguments.First().ToLower() == "scp-575-b")
                    {
                        player.SendConsoleMessage("Description: SCP-575 appears to be an unknown form of matter, taking the form of a series of amorphous black shapes and structures. SCP-575 is difficult to observe, as it immediately dissipates when exposed to light","white");
                        player.SendConsoleMessage("Notes: SCP-575 in game is currently up to debate. I am still conflicted as to whether such an scp should be on our server. However, as of the session that you are reading this on. It is currently in the game. \n Gameplay: SCP-575-B will sometimes plunge the facility into complete* darkness. It will manifest on one player, and chase them until they shine a flashlight* on the entity, or until it disappears.","white");
                    }
                    else if (arguments.First().ToLower() == "serpentshand" || arguments.First().ToLower() == "serpents" || arguments.First().ToLower() == "theserpentshand")
                    {
                        player.SendConsoleMessage("Overview: The Serpent's Hand is a small but formidable organization responsible for several security breaches. At least three different individuals have been encountered, all of whom used possible or confirmed anomalous items for infiltration purposes (including SCP-268...","white");
                        player.SendConsoleMessage("Notes: Gameplay of the serpent's hand differs slightly from their lore. \n Ingame, they spawn as tutorial classes (because I can't do anything else), and spawn with less powerful weapons because they help the SCPs. They do however always spawn with an almost random* (due to obvious reasons) scp item. \n yo man, remember the time when they could friendly fire scps?... heh","white");
                    }
                    else if (arguments.First().ToLower() == "radiation" || arguments.First().ToLower() == "rad")
                    {
                        player.SendConsoleMessage("Notes: Radiation is an experimental gameplay mechanic that likely will need tuning/edits. Here's how it works: \n After 300 seconds (approx. 5 minutes) of nuke going off. (I understand it is a little long) radiation will begin to hit ALL players (excl. zombies, still needs testing). And is fairly* balanced.","white");
                    }
                    else if (arguments.First().ToLower() == "thekid" || arguments.First().ToLower() == "kid")
                    {
                        player.SendConsoleMessage("Notes: The Kid is a subrole of D-Class that gives the selected player candy, and makes them smaller. More benefits are being considered, such as giving them SCP-1853. For gameplay purposes, it would be removeable by said player if they pefer cola.","white");
                        player.SendConsoleMessage("Notes: If you are stuck at the kid's height. Run the command .fixmepls (works on any class) to fix your height. You might need to rejoin if it desyncs you.","red");
                    }
                    else if (arguments.First().ToLower() == "scp-966" || arguments.First().ToLower() == "scp966" || arguments.First().ToLower() == "966")
                    {
                        player.SendConsoleMessage("Notes: This guy is still under construction fam, come back later fam.","red");
                    }
                    else if (arguments.First().ToLower() == "scp-1040" || arguments.First().ToLower() == "scp1040" || arguments.First().ToLower() == "1040")
                    {
                        player.SendConsoleMessage("Description: SCP-1040 is an antique Tiffany floor lamp made of iron and stained glass. The item displays no unusual physical properties. When provided with a 100-watt light bulb and electricity, SCP-1040 functions as is typical for a floor lamp.","white");
                        player.SendConsoleMessage("Notes: This guy is still under construction fam, come back later fam.","red");
                    }
                    else if (arguments.First().ToLower() == "v-s-r")
                    {
                        player.SendConsoleMessage("(I can't use anything related to violation or vsr for some reason, it stops this cmd from working) Would we be able to (if I wanted to) become a public, verified server on the SL server list? No! infact, this command that you are using breaks one of the VSR rules! but don't worry, I'm having this server stay private just for you and everyone else.","white");
                    }
                    else if (arguments.First().ToLower() == "mtf")
                    {
                        player.SendConsoleMessage("Besides Epsilon-11, we also have a plugin that adds the MTF unit NU-7. I have plans for more in the future, but I am focusing on more items/scps.","white");
                    }
                    else if (arguments.First().ToLower() == "science")
                    {
                        player.SendConsoleMessage("NO! DONT SHOOT, IM WITH THE SCIENCE TEAM! (REAL!!)","white");
                    }
                    else if (arguments.First().ToLower() == "scp-1025" || arguments.First().ToLower() == "scp1025" || arguments.First().ToLower() == "1025")
                    {
                        player.SendConsoleMessage("Description: SCP-1025 is a hardcover book, approximately 1,500 pages long. The front cover and spine feature the title \"The Encyclopedia of Common Diseases.\"","white");
                        player.SendConsoleMessage("Notes: Readers of the book seem to exhibit symptoms of any disease they read about. Located in the glass room.","white");
                    }
                    else if (arguments.First().ToLower() == "button")
                    {
                        player.SendConsoleMessage("Description: The button is a (REDACTED) that has a 1 in 25 chance of spawning. When spun up and pressed it will (REDACTED) and (REDACTED).", "white");
                        player.SendConsoleMessage("Notes: None", "white");
                    }
                    else if (arguments.First().ToLower() == "ghastly")
                    {
                        player.SendConsoleMessage("Description: The Ghastly lantern might occasionally replace a normal lantern with itsself. It has no physical differences from the normal item.", "white");
                        player.SendConsoleMessage("Notes: Allows the user holding it to (REDACTED)", "white");
                    }
                    else if (arguments.First().ToLower() == "dictionary" || arguments.First().ToLower() == "list" || arguments.First().ToLower() == "help")
                    {
                        player.SendConsoleMessage("Every Entry to .info can be found below. (type the entry after the command, ex: .info scp-127) caps are handled automatically. \n- SCP-127 \n- SCP-1162 \n- SCP-1499 \n- SCP-294 \n- Pills \n- SCP-575 \n- SerpentsHand \n- Radiation \n- TheKid \n- SCP-966 \n- SCP-1040 \n- v-s-r \n- MTF","white");
                        player.SendConsoleMessage("\n- Science \n- SCP-1025 \n- button \n- ghastly", "white");
                    }


                    response = "Look above.";
                    return true;
                }
                else
                {
                    response = "You can't run that.";
                    return false;
                }

            }
        }






        // AUDIO API STUFF
        // CREDIT TO KoT0XleB 



        public static ReferenceHub AudioBot = new ReferenceHub();
        public static AudioPlayerBase PlayAudio(string audioFile, byte volume, bool loop)
        {
            if (AudioBot == null) AudioBot = AddDummy();

            StopAudio();

            var path = Path.Combine("", audioFile);

            AudioPlayerBase audioPlayer = AudioPlayerBase.Get(AudioBot);
            audioPlayer.Enqueue(path, -1);
            audioPlayer.LogDebug = false;
            audioPlayer.BroadcastChannel = VoiceChatChannel.Intercom;
            audioPlayer.Volume = 20f;
            audioPlayer.Loop = loop;
            audioPlayer.Play(0);
            return audioPlayer;
        }

        public static AudioPlayerBase PlayAudio64(string audioFile, byte volume, bool loop, ReferenceHub AudioBotT)
        {
         //   if (AudioBot == null) AudioBot = AddDummy();

            
            var path = Path.Combine("", audioFile);

            AudioPlayerBase audioPlayer = AudioPlayerBase.Get(AudioBotT);
            audioPlayer.Enqueue(path, -1);
            audioPlayer.LogDebug = false;
            audioPlayer.BroadcastChannel = VoiceChatChannel.Proximity;
            audioPlayer.BroadcastTo.Clear();
            audioPlayer.Volume = 10f;
            audioPlayer.Loop = loop;
            audioPlayer.Play(0);
            return audioPlayer;
        }





        public static AudioPlayerBase PlayPlayerAudio(Player player, string audioFile, byte volume)
        {
            if (AudioBot == null) AudioBot = AddDummy();

            StopAudio();

            var path = Path.Combine("", audioFile);

            AudioPlayerBase audioPlayer = AudioPlayerBase.Get(AudioBot);
            audioPlayer.Enqueue(path, -1);
            audioPlayer.LogDebug = false;
            audioPlayer.BroadcastChannel = VoiceChatChannel.Proximity;
            audioPlayer.BroadcastTo.Add(player.PlayerId);
            audioPlayer.Volume = 10f;
            audioPlayer.Loop = false;
            audioPlayer.Play(0);
            return audioPlayer;
        }


        public static AudioPlayerBase PlayPlayerAudio096(Player player, string audioFile, byte volume, ReferenceHub AudioBotT)
        {
            //if (AudioBot == null) AudioBot = AddDummy();

            StopAudio();

            var path = Path.Combine("", audioFile);

            AudioPlayerBase audioPlayer = AudioPlayerBase.Get(AudioBotT);
            audioPlayer.Enqueue(path, -1);
            audioPlayer.LogDebug = false;
            audioPlayer.BroadcastChannel = VoiceChatChannel.Proximity;
            audioPlayer.BroadcastTo.Add(player.PlayerId);
            audioPlayer.Volume = 10f;
            audioPlayer.Loop = false;
            audioPlayer.Play(0);
            return audioPlayer;
        }




        public static AudioPlayerBase AddListener(Player player, string audioFile, byte volume, ReferenceHub AudioBotT)
        {
            //if (AudioBot == null) AudioBot = AddDummy();

            //StopAudio();

            var path = Path.Combine("", audioFile);

            AudioPlayerBase audioPlayer = AudioPlayerBase.Get(AudioBotT);
            //audioPlayer.Enqueue(path, -1);
            audioPlayer.LogDebug = false;
            audioPlayer.BroadcastChannel = VoiceChatChannel.Proximity;
            audioPlayer.BroadcastTo.Add(player.PlayerId);
            audioPlayer.BroadcastChannel = VoiceChatChannel.Proximity;
            return audioPlayer;
        }

        public static AudioPlayerBase RemoveListener(Player player, string audioFile, byte volume, ReferenceHub AudioBotT)
        {
            //if (AudioBot == null) AudioBot = AddDummy();

            //StopAudio();

            var path = Path.Combine("", audioFile);

            AudioPlayerBase audioPlayer = AudioPlayerBase.Get(AudioBotT);
           // audioPlayer.Enqueue(path, -1);
            audioPlayer.LogDebug = false;
            audioPlayer.BroadcastChannel = VoiceChatChannel.Proximity;
            audioPlayer.BroadcastTo.Remove(player.PlayerId);
            audioPlayer.BroadcastChannel = VoiceChatChannel.Proximity;
            return audioPlayer;
        }



        public static void PlayPlayerAudio2(Player player, string audioFile, byte volume)
        {
            // if (AudioBot == null) AudioBot = AddDummy();

            //StopAudio();
          

            var path = Path.Combine("", audioFile);

            var audioPlayer = AudioPlayerBase.Get(player.ReferenceHub);
            audioPlayer.Enqueue(path, -1);
            audioPlayer.LogDebug = false;
            audioPlayer.BroadcastChannel = VoiceChatChannel.Proximity;
            audioPlayer.Volume = 85f;
            audioPlayer.Loop = false;
            audioPlayer.Play(0);
        }


        public static void PauseAudio()
        {
            if (AudioBot == null) return;

            var audioPlayer = AudioPlayerBase.Get(AudioBot);

            if (audioPlayer.CurrentPlay != null)
            {
                audioPlayer.ShouldPlay = false;
            }
        }


        public static void ResumeAudio()
        {
            if (AudioBot == null) return;

            var audioPlayer = AudioPlayerBase.Get(AudioBot);

            if (audioPlayer.CurrentPlay != null)
            {
                audioPlayer.ShouldPlay = true;
            }
        }

        public static void StopAudio()
        {
            if (AudioBot == null) return;

            var audioPlayer = AudioPlayerBase.Get(AudioBot);

            if (audioPlayer.CurrentPlay != null)
            {
                audioPlayer.Stoptrack(true);
                audioPlayer.OnDestroy();
            }
        }


        public static ReferenceHub AddDummy()
        {
            var newPlayer = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);
            var fakeConnection = new FakeConnection(0);
            var hubPlayer = newPlayer.GetComponent<ReferenceHub>();
            NetworkServer.AddPlayerForConnection(fakeConnection, newPlayer);
            hubPlayer.authManager.InstanceMode = CentralAuth.ClientInstanceMode.Unverified;
            // CharacterClassManager.instance
            hubPlayer.characterClassManager._godMode = true;
            hubPlayer.roleManager.ServerSetRole(RoleTypeId.Overwatch, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
           hubPlayer.transform.localScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
            try
            {
                if (isSerpentSpawning == true)
                {
                  //  hubPlayer.nicknameSync.SetNick("Serpents Hand");
                }
                else
                {
                 //   hubPlayer.nicknameSync.SetNick("SCP-294");
                }
                //    hubPlayer.nicknameSync.SetNick("SCP-294");
                hubPlayer.nicknameSync.SetNick(" ");
            }
            catch (Exception) { }

            Timing.CallDelayed(0.7f, () =>
            {
              
                hubPlayer.roleManager.ServerSetRole(RoleTypeId.Overwatch, RoleChangeReason.None, RoleSpawnFlags.None);
                hubPlayer.characterClassManager._godMode = true;
                hubPlayer.transform.localScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
               // foreach (var target in ReferenceHub.AllHubs.Where(x => x != ReferenceHub.HostHub))
                  //  NetworkServer.SendSpawnMessage(hub.networkIdentity, target.connectionToClient);
            });


            try
            {

                // hubPlayer.nicknameSync.SetNick("SCP-294");
                //hubPlayer.nicknameSync.SetNick(" ");
                if (isSerpentSpawning == true)
                {
                  //  hubPlayer.nicknameSync.SetNick("Serpents Hand");
                }
                else
                {
                    //hubPlayer.nicknameSync.SetNick("SCP-294");
                }
                hubPlayer.nicknameSync.SetNick(" ");
            }
            catch (Exception) { }

            Timing.CallDelayed(1f, () =>
            {

                    hubPlayer.TryOverridePosition(UnityEngine.Vector3.zero, UnityEngine.Vector3.zero);
            });


            return hubPlayer;
        }


        public static ReferenceHub AddDummy2()
        {
            var newPlayer = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);
            var fakeConnection = new FakeConnection(0);
            var hubPlayer = newPlayer.GetComponent<ReferenceHub>();
            NetworkServer.AddPlayerForConnection(fakeConnection, newPlayer);
            hubPlayer.authManager.InstanceMode = CentralAuth.ClientInstanceMode.Unverified;
          //  Positionn = RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == "LCZ_914 (14)").transform.position.x - 6, RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == "LCZ_914 (14)").transform.position.y + 1, RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == "LCZ_914 (14)").transform.position.z)
            // CharacterClassManager.instance
            hubPlayer.TryOverridePosition((Vector3)(RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == "LCZ_914 (14)").transform.position), Vector3.zero);
            //hubPlayer.TryOverridePosition((Vector3)RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == "LCZ_914 (14)").transform.position.x - 6,RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == "LCZ_914 (14)").transform.position.y + 1, RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == "LCZ_914 (14)").transform.position.z), Vector3.zero);
            hubPlayer.characterClassManager._godMode = true;
            foreach (StatusEffectBase effect in hubPlayer.playerEffectsController.AllEffects)
            {
                StatusEffectBase effectt = hubPlayer.playerEffectsController.AllEffects.FirstOrDefault(r => r.name == "Invisibility");
                effect.ServerSetState(1, 9999, false);
            }

            //hubPlayer.characterClassManager.isno = true;
            // hubPlayer.playerEffectsController.
            hubPlayer.roleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
            hubPlayer.transform.localScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
            try
            {
                if (isSerpentSpawning == true)
                {
                    //  hubPlayer.nicknameSync.SetNick("Serpents Hand");
                }
                else
                {
                    //   hubPlayer.nicknameSync.SetNick("SCP-294");
                }
                //    hubPlayer.nicknameSync.SetNick("SCP-294");
                hubPlayer.nicknameSync.SetNick(" ");
            }
            catch (Exception) { }

            Timing.CallDelayed(0.7f, () =>
            {

                hubPlayer.roleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.None, RoleSpawnFlags.None);
                hubPlayer.characterClassManager._godMode = true;
                hubPlayer.transform.localScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
                // foreach (var target in ReferenceHub.AllHubs.Where(x => x != ReferenceHub.HostHub))
                //  NetworkServer.SendSpawnMessage(hub.networkIdentity, target.connectionToClient);
            });


            try
            {

                // hubPlayer.nicknameSync.SetNick("SCP-294");
                //hubPlayer.nicknameSync.SetNick(" ");
                if (isSerpentSpawning == true)
                {
                    //  hubPlayer.nicknameSync.SetNick("Serpents Hand");
                }
                else
                {
                    //hubPlayer.nicknameSync.SetNick("SCP-294");
                }
                hubPlayer.nicknameSync.SetNick(" ");
            }
            catch (Exception) { }

            Timing.CallDelayed(1f, () =>
            {

                hubPlayer.TryOverridePosition((Vector3)(RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == "LCZ_914 (14)").transform.position), Vector3.zero);
            });


            return hubPlayer;
        }

    



        public static void RemoveDummy()
        {
            var audioPlayer = AudioPlayerBase.Get(AudioBot);

            if (audioPlayer.CurrentPlay != null)
            {
                audioPlayer.Stoptrack(true);
                audioPlayer.OnDestroy();
            }

            AudioBot.OnDestroy();
            CustomNetworkManager.TypedSingleton.OnServerDisconnect(AudioBot.connectionToClient);
            UnityEngine.Object.Destroy(AudioBot.gameObject);
        }


        public static void RemoveDummy096(ReferenceHub AudioBotT)
        {
            var audioPlayer = AudioPlayerBase.Get(AudioBotT);

            if (audioPlayer.CurrentPlay != null)
            {
                audioPlayer.Stoptrack(true);
                audioPlayer.OnDestroy();
            }

            AudioBotT.OnDestroy();
            CustomNetworkManager.TypedSingleton.OnServerDisconnect(AudioBotT.connectionToClient);
            UnityEngine.Object.Destroy(AudioBotT.gameObject);
        }



        public static void PauseAudio096(ReferenceHub AudioBotT)
        {
            if (AudioBotT == null) return;

            var audioPlayer = AudioPlayerBase.Get(AudioBotT);

            if (audioPlayer.CurrentPlay != null)
            {
                audioPlayer.ShouldPlay = false;
            }
        }


        public static void ResumeAudio096(ReferenceHub AudioBotT)
        {
            if (AudioBotT == null) return;

            var audioPlayer = AudioPlayerBase.Get(AudioBotT);

            if (audioPlayer.CurrentPlay != null)
            {
                audioPlayer.ShouldPlay = true;
            }
        }

        public static bool CheckPlaying(ReferenceHub AudioBotT)
        {
            if (AudioBotT == null) return false;


            var audioPlayer = AudioPlayerBase.Get(AudioBotT);

            if (audioPlayer.CurrentPlay != null)
            {
                audioPlayer.Stoptrack(true);
                audioPlayer.OnDestroy();
                return true;
                
            }
            else
            {
               // RemoveDummy096(AudioBotT);
                return false;
            }
        }

        public static void StopAudio096(ReferenceHub AudioBotT)
        {
            if (AudioBotT == null) return;

            var audioPlayer = AudioPlayerBase.Get(AudioBotT);

            if (audioPlayer.CurrentPlay != null)
            {
                audioPlayer.Stoptrack(true);
                audioPlayer.OnDestroy();
            }
        }




        public int CompareTo(object obj)
        {
            return Comparer<EventHandlers>.Default.Compare(this, obj as EventHandlers);
        }
    }
}
