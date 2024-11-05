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
    using System.Reflection.Emit;
    using UnityEngine.SceneManagement;
    using RueI;
    using RueI.Displays;
    using RueI.Extensions;
    using StringBuilder = RueI.Extensions.HintBuilding;
    using RueI.Elements;
    using RueI.Displays.Scheduling;
    using PlayerRoles.Spectating;
    using static System.Net.Mime.MediaTypeNames;
    using CentralAuth;
    using CommandSystem.Commands.Console;
    using PlayerRoles.PlayableScps.Scp096;
    using static global::Plugin.EventHandlers;
    using GameCore;
    using Log = PluginAPI.Core.Log;
    using System.Runtime.InteropServices;
    using System.Net;
    using InventorySystem.Items.Radio;
    using HarmonyLib;
    using System.Runtime.Remoting.Messaging;
    using slocLoader;
    using slocLoader.Objects;
    using AdminToys;
    using RemoteAdmin;
    using System.Reflection;

    // woo I love converting 6k lines of code over to new things (i'm gonna have to do it again when labapi drops :D)
    public class EventHandlers : IComparable
    {
        int respawn_count = 0;
        private GameObject game_object;
        HashSet<int> fbi = new HashSet<int>();
        HashSet<int> sci = new HashSet<int>();
        HashSet<int> chase096Music = new HashSet<int>();


        public static HashSet<int> scp035s = new HashSet<int>();
        public static HashSet<int> thebosszombies = new HashSet<int>();


        static UnityEngine.Vector3 offset = new UnityEngine.Vector3(-40.021f, -8.119f, -36.140f);
        SpawnableTeamType spawning_team = SpawnableTeamType.None;
        public static bool isSerpentSpawning = false;
        public static bool isScienceTeamSpawning = false;
        public static bool canswap = true;
        private List<Scp079Generator> _generators = new List<Scp079Generator>();
        int randomNumber;
        int generatorsActivated = 0;
        private HashSet<Player> _PlayersWithArmor = new HashSet<Player>();

        private CoroutineHandle _displayCoroutine;
        private CoroutineHandle _buttonCorountine;
        public static HashSet<ushort> ghostLantern = new HashSet<ushort>();
        public static HashSet<ushort> normalLantern = new HashSet<ushort>();


        public static bool PlayingEvents = false;


        public static Dictionary<Player, ReferenceHub> PlayerAudioBots = new Dictionary<Player, ReferenceHub>();


        public static Dictionary<Player, int> PlayerSpectators = new Dictionary<Player, int>();
        public static Dictionary<Player, int> PlayerKills = new Dictionary<Player, int>();

        public static Dictionary<String, bool> PlayerPreferenceEffectList = new Dictionary<String,bool>();

        // Custom Items that need to be accessed everwhere (should really make a custom enum or smthn for this lmao)
        public static HashSet<ushort> THEButton = new HashSet<ushort>();
        public static HashSet<ushort> doublenade = new HashSet<ushort>();
        public static HashSet<ushort> peanutnade = new HashSet<ushort>();
        public static HashSet<ushort> freezenade = new HashSet<ushort>();
        public static HashSet<ushort> grenades = new HashSet<ushort>();
        public static HashSet<ushort> Mask035List = new HashSet<ushort>();
        System.Random random = new System.Random();




        public bool Is035(Player player)
        {
            if (player != null && scp035s.Contains(player.PlayerId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsTheBoss(Player player)
        {
            if (player != null && thebosszombies.Contains(player.PlayerId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        


        public static string RoundEvent;
        string LastRoundEvent;
        bool buttonused = false;
        int PlrCount = 0;
        [PluginEvent(ServerEventType.RoundStart)]
        void RoundStarted()
        {
            buttonused = false;
            respawn_count = 0;
            fbi.Clear();
            chase096Music.Clear();
            spawning_team = SpawnableTeamType.None;
            isSerpentSpawning = false;
            // TempDummyy = AddDummy2();
          //  PlayerAudioBots.Clear();
            //  RoundEvent = "";
            generatorsActivated = 0;

            // PlayAudio64("ninefourteen.ogg", (byte)65F, true, TempDummyy);
            // audioPlayerr = AudioPlayerBase.Get(TempDummyy);
            foreach (var p in Player.GetPlayers())
            {
                if (!PlayerKills.TryGetValue(p, out int test))
                {
                    PlayerKills.Add(p, 0);
                    PlayerSpectators.Add(p, 0);
                }
                else
                {
                    PlayerKills[p] = 0;
                    PlayerSpectators[p] = 0;
                }
            }

            foreach (var p in Player.GetPlayers())
            {
                PlrCount++;
            }


            if (PlrCount == 8 || PlrCount == 9)
            {
                Timing.CallDelayed(0.2f, () =>
                {
                    foreach (var p in Player.GetPlayers())
                    {
                       if (p.IsSCP && p.Role != RoleTypeId.Scp0492)
                        {
                            switch (UnityEngine.Random.Range(0, 2))
                            {
                                case 0: p.SetRole(RoleTypeId.Scientist); break;
                                case 1: p.SetRole(RoleTypeId.ClassD); break;
                                case 2: p.SetRole(RoleTypeId.FacilityGuard); break;
                            }
                            break;
                        }
                    }
                });

            }



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
                /*
                PrimitiveObject divider = new PrimitiveObject(ObjectType.Cube);
                divider.ColliderMode = PrimitiveObject.ColliderCreationMode.ClientOnly;
                divider.Transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);
                divider.MaterialColor = Color.white;
                divider.SpawnObject(game_object);
                */
                LastRoundEvent = RoundEvent;


                Timing.CallDelayed(5f, () =>
                {




                    if (RoundEvent == "PowerBlackout")
                    {
                        Facility.TurnOffAllLights();
                        Cassie.GlitchyMessage("Facility power system failure", 1f, 1f);
                    }
                    else
                    {
                        Cassie.Message(". . . . . . . . . . . . . . .", false, true, false);
                    }
                    foreach (var p in Player.GetPlayers())
                    {
                        if (p.Role != RoleTypeId.Overwatch && p.Role != RoleTypeId.Tutorial && p.Role != RoleTypeId.Spectator)
                        {

                            
                            if (RoundEvent == "FriendlyFire")
                            {
                                Server.FriendlyFire = true;
                                p.SendBroadcast("<color=#228B22>EVENT:</color> Friendly fire is enabled.", 13, Broadcast.BroadcastFlags.Normal, false);
                            }
                            if (RoundEvent == "ClearDay")
                            {
                                p.SendBroadcast("<color=#228B22>EVENT:</color> The fog has subsided.", 13, Broadcast.BroadcastFlags.Normal, false);
                            }
                            if (RoundEvent == "TestingDay")
                            {
                                p.SendBroadcast("<color=#228B22>EVENT:</color> Testing day.", 13, Broadcast.BroadcastFlags.Normal, false);
                                foreach (var pl in Player.GetPlayers())
                                {
                                    if (pl.IsHuman == true)
                                    {
                                        pl.Position = new Vector3((float)(RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == "LCZ_372").transform.position.x), (float)RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == "LCZ_372").transform.position.y + 1.4f, (float)RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == "LCZ_372").transform.position.z);
                                    }
                                }
                            }

                            if (RoundEvent == "EveryoneIsSmall")
                            {
                                p.SendBroadcast("<color=#228B22>EVENT:</color> Everyone is small!", 13, Broadcast.BroadcastFlags.Normal, false);
                            }
                            if (RoundEvent == "ChaosInvasion")
                            {
                                p.SendBroadcast("<color=#228B22>EVENT:</color> Chaos Invasion.", 13, Broadcast.BroadcastFlags.Normal, false);
                            }
                            if (RoundEvent == "PowerBlackout")
                            {

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

                            }
                            if (RoundEvent == "Foggy")
                            {
                                p.SendBroadcast("<color=#228B22>EVENT:</color> THE FOG IS COMING THE FOG IS COMING THE FOG IS COMING THE FOG IS COMING THE FOG IS COMING", 13, Broadcast.BroadcastFlags.Normal, false);
                            }
                            if (RoundEvent == "SpecialOps")
                            {
                                p.SendBroadcast("<color=#228B22>EVENT:</color> Mobile Task Force Unit Epsilon-11 have been deployed to replace on-site security.", 13, Broadcast.BroadcastFlags.Normal, false);
                                
                            }
                            if (RoundEvent == "Nextbots")
                            {
                                p.SendBroadcast("<color=#228B22>EVENT:</color> The SCPs are now nextbots! (Note: 939 & 106 are not effected by this event.)", 13, Broadcast.BroadcastFlags.Normal, false);

                                
                                
                            }
                        }
                    }
                });

            }


            if (_displayCoroutine.IsRunning)
                Timing.KillCoroutines(_displayCoroutine);

            _displayCoroutine = Timing.RunCoroutine(ShowDisplay());

            if (_buttonCorountine.IsRunning)
                Timing.KillCoroutines(_buttonCorountine);




            //RoomIdentifier.AllRoomIdentifiers.TryGetValue(Random.Range(1, RoomIdentifier.AllRoomIdentifiers.Count));
            //HashSet<RoomIdentifier> AllRooms = RoomIdentifier.AllRoomIdentifiers;
            // int rndmNumber = Random.Next(AllRooms.size());






           
                _buttonCorountine = Timing.RunCoroutine(SpawnButton());

            


        //     _DoorLockCorountine = Timing.RunCoroutine(LockDoors());
    }


        static bool haveSerpentsSpawned = false;
        static bool havetheScienceTeamSpawned = false;


        static bool serpentsCaptain = false;
        private Player captainplayer = null;


        // Exiled
        // Reformatted by normalcat from the NWAPI Discord
        // Also broken!
        public static void SubtitledCassie(string message, string subtitles)
        {
            string finished = $"{subtitles.Replace(' ', ' ')}<size=0>{message}</size>";
            RespawnEffectsController.PlayCassieAnnouncement(finished, true, true, true);
        }

        public static void ChangeTo035(Player player, bool RoundStart)
        {
            if (!scp035s.Contains(player.PlayerId))
            {
                player.EffectsManager.EnableEffect<SeveredHands>(1f, false);
                Timing.CallDelayed(0.1f, () =>
                {

                    player.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                    player.EffectsManager.DisableAllEffects();

                    scp035s.Add(player.PlayerId);

                    player.SendBroadcast("You picked up an infected item and are now <color=#C50000>SCP-035</color>. Work with the SCPs, terminate any human classes.", 15, Broadcast.BroadcastFlags.Normal, true);
                    player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = $"<color=#C50000>{player.Nickname}</color>" + "\n<color=#C50000>SCP-035</color>";

                    player.PlayerInfo.IsNicknameHidden = true;
                    player.PlayerInfo.IsUnitNameHidden = true;
                    player.PlayerInfo.IsRoleHidden = true;

                    player.Health = 500f;

                   



                    if (player.ReferenceHub.playerStats.StatModules[1] is AhpStat ahpStat)
                    {
                        //AhpStat.AhpProcess ahpProcess = (250f, 250f, 0f, 10f, 1f, true);
                        ahpStat.ServerAddProcess(250f, 250f, 0f, 10f, 1f, true);
                    }

                    Timing.CallDelayed(0.2f, () =>
                    {
                        foreach (var items in player.Items)
                        {
                            if (Mask035List.Contains(items.ItemSerial))
                            {
                                player.ReferenceHub.inventory.ServerRemoveItem(items.ItemSerial, items.PickupDropModel);
                                //player.RemoveItem(items);
                            }
                        }
                    });

                    if (RoundStart == true)
                    {

                        String RoomName = "HCZ_079";


                        List<ItemType> Scp035Loadout = new List<ItemType> {ItemType.KeycardScientist, ItemType.Medkit};

                        foreach(ItemType item in Scp035Loadout)
                        {
                            player.AddItem(item);
                        }
                        player.Position = new Vector3((float)(RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == RoomName).transform.position.x), (float)RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == RoomName).transform.position.y + 1.4f, (float)RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == RoomName).transform.position.z);
                    }

                    LightSourceToy adminToy;
                    LightSourceToy light;
                    Dictionary<uint, GameObject>.ValueCollection.Enumerator Enumerator = NetworkClient.prefabs.Values.GetEnumerator();
                    ReferenceHub playerref = player.ReferenceHub;
                    while (Enumerator.MoveNext())
                    {
                        if (Enumerator.Current.TryGetComponent<LightSourceToy>(out adminToy))
                        {
                            light = UnityEngine.Object.Instantiate(adminToy, playerref.transform);
                            light.Position = playerref.transform.position;
                            light.LightColor = Color.red;
                            light.LightShadows = false;
                            light.LightRange = 5f;
                            light.LightIntensity = 1f;
                            light.OnSpawned(playerref, new ArraySegment<string>());
                        }
                    }
                });
            }
        }


        public static void ChangeToTheBoss(Player player, bool RoundStart)
        {
            if (!thebosszombies.Contains(player.PlayerId))
            {
               
                Timing.CallDelayed(0.1f, () =>
                {

                    player.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.Scp0492, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                    //player.EffectsManager.DisableAllEffects();

                    thebosszombies.Add(player.PlayerId);

                    player.SendBroadcast("You are now <color=#C50000>THE BOSS</color>. You are a beefed up zombie with a few small buffs.", 15, Broadcast.BroadcastFlags.Normal, true);
                    player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = $"<color=#C50000>{player.Nickname}</color>" + "\n<color=#C50000>THE BOSS</color>";

                    player.PlayerInfo.IsNicknameHidden = true;
                    player.PlayerInfo.IsUnitNameHidden = true;
                    player.PlayerInfo.IsRoleHidden = true;

                    player.Health = 1200f;

                    SetScale(player, 1.1f);

                    player.EffectsManager.EnableEffect<SilentWalk>(0, false);
                    player.EffectsManager.ChangeState<SilentWalk>(5, 0, false);


                    if (player.ReferenceHub.playerStats.StatModules[1] is AhpStat ahpStat)
                    {
                        //AhpStat.AhpProcess ahpProcess = (250f, 250f, 0f, 10f, 1f, true);
                       // ahpStat.ServerAddProcess(250f, 250f, 0f, 10f, 1f, true);
                    }


                    if (RoundStart == true)
                    {

                        String RoomNamee = "HCZ_HID";


                        player.Position = new Vector3((float)(RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == RoomNamee).transform.position.x), (float)RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == RoomNamee).transform.position.y + 1.4f, (float)RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == RoomNamee).transform.position.z);
                    }
                });
            }
        }





        void ChangeToTutorial(Player player, RoleTypeId role)
        {
            // player.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.Escaped, RoleSpawnFlags.None);
            Config config = Plugin.Singleton.Config;

            Timing.CallDelayed(0.1f, () =>
            {
            player.ReferenceHub.inventory.UserInventory.Items.Clear();
            player.Role = PlayerRoles.RoleTypeId.Tutorial;
            fbi.Add(player.PlayerId);

            player.ClearBroadcasts();
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
                player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = $"<color=#FF1493>{player.Nickname}</color>" + "\n<color=#FF1493>SERPENTS HAND CAPTAIN</color>";
                  //  player.CustomInfo = $"<color=#FF1493>{player.Nickname}</color>" + "\n<color=#FF1493>SERPENTS HAND CAPTAIN</color>";
                player.PlayerInfo.IsNicknameHidden = true;
                player.PlayerInfo.IsUnitNameHidden = true;
                player.PlayerInfo.IsRoleHidden = true;
                    

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

                AddOrDropFirearm(player, ItemType.GunCOM18, true);
                    player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = $"<color=#FF1493>{player.Nickname}</color>" + "\n<color=#FF1493>SERPENTS HAND AGENT</color>";
                    //player.CustomInfo = $"<color=#FF1493>{player.DisplayNickname}</color>" + "\n<color=#FF1493>SERPENTS HAND AGENT</color>";
                    player.PlayerInfo.IsNicknameHidden = true;
                  player.PlayerInfo.IsUnitNameHidden = true;
                  player.PlayerInfo.IsRoleHidden = true;
                    
                }


                Player playertoTP = Player.Get(player.ReferenceHub);
            playertoTP.Position = new UnityEngine.Vector3(0.06f, 1000.96f, 0.33f);
            });
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


        [PluginEvent]
        public bool OnScp049StartResurrectingBody(Scp049StartResurrectingBodyEvent ev)
        {
            //Log.Info($"Player &6{ev.Player.Nickname}&r (&6{ev.Player.UserId}&r) playing as SCP-049 tried resurrecting body of &6{ev.Target.Nickname}&r (&6{ev.Target.UserId}&r), ragdoll with class &2{ev.Body.Info.RoleType}&r but it {(ev.CanResurrct ? "failed" : "succeded")}!");
            if (thebosszombies.Contains(ev.Target.PlayerId) || scp035s.Contains(ev.Target.PlayerId))
            {
                return false;
            }
            else
            {
                return true;
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
                player.Health = 100;
                player.ReferenceHub.inventory.UserInventory.Items.Clear();
                sci.Add(player.PlayerId);

                // player.ClearBroadcasts();

                player.SendBroadcast("You have spawned as apart of the Science Team. Assist remaining MTF or Scientists.", 15);
                player.AddItem(ItemType.ArmorCombat);
                player.AddItem(ItemType.KeycardMTFOperative);
                player.AddItem(ItemType.Medkit);
                player.AddItem(ItemType.Painkillers);



                player.AddAmmo(ItemType.Ammo556x45, 80);
                player.AddAmmo(ItemType.Ammo9x19, 40);
                AddOrDropFirearm(player, ItemType.GunE11SR, true);
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
                Player playertoTP = Player.Get(player.ReferenceHub);
                playertoTP.Position = new UnityEngine.Vector3(63.01f, 991.65f, -50.04f);

                // might add config for this in the future, dunno yet
                // fyi add +1000 to ur y coord if you wanna tp someone to somewhere on surface, learned that from axwabo. 
                /*
                player.CustomInfo = $"<color=#FAFF86>{player.DisplayNickname}\nTHE SCIENCE TEAM</color>";
                player.PlayerInfo.IsNicknameHidden = true;
                player.PlayerInfo.IsUnitNameHidden = true;
                player.PlayerInfo.IsRoleHidden = true;
                */
            });


        }


        Config cfg = Plugin.Singleton.Config;


        private IEnumerator<float> SpawnButton()
        {

            yield return Timing.WaitForSeconds(Random.Range(300f, 600f));
            {
                try
                {
                    // Log.Debug(RandomRoom);
                    if (!RoundSummary.singleton._roundEnded && Round.IsRoundStarted)
                    {

                        List<String> RoomList = new List<String> { "EZ_Crossing (3)", "EZ_upstairs", "EZ_Smallrooms2", "EZ_PCs", "EZ_GateA", "EZ_GateB", "HCZ_049", "HCZ_Tesla", "LCZ_Cafe (15)", "LCZ_Plants", "LCZ_372 (18)" };
                        String RandomRoom = RoomList.RandomItem();


                        List<ItemType> ItemList = new List<ItemType> { ItemType.GunE11SR, ItemType.GunCrossvec, ItemType.Adrenaline, ItemType.AntiSCP207, ItemType.SCP500, ItemType.SCP018, ItemType.SCP1576, ItemType.SCP1853, ItemType.SCP268, ItemType.ArmorCombat, ItemType.ArmorHeavy, ItemType.ArmorLight, ItemType.Flashlight, ItemType.KeycardContainmentEngineer, ItemType.GunCOM15, ItemType.GunCOM18, ItemType.GrenadeFlash, ItemType.KeycardChaosInsurgency, ItemType.KeycardJanitor, ItemType.KeycardScientist, ItemType.KeycardResearchCoordinator, ItemType.GunLogicer, ItemType.Medkit, ItemType.Painkillers, ItemType.GunFSP9, ItemType.SCP244b, ItemType.SCP244a, ItemType.KeycardZoneManager, ItemType.KeycardGuard, ItemType.Lantern, ItemType.Radio, ItemType.GrenadeHE, ItemType.SCP207, ItemType.Jailbird, ItemType.KeycardO5, ItemType.KeycardFacilityManager, ItemType.KeycardChaosInsurgency };

                        ItemBase mask = ReferenceHub.HostHub.inventory.ServerAddItem(ItemList.RandomItem());

                        String RandomRoomMask = RoomList.RandomItem();
                        ItemPickupBase maskPickup = mask.ServerDropItem();
                        maskPickup.transform.position = new Vector3((float)(RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == RandomRoomMask).transform.position.x), (float)RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == RandomRoomMask).transform.position.y + 2, (float)RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == RandomRoomMask).transform.position.z);
                        maskPickup.transform.rotation = UnityEngine.Quaternion.Euler(0, 0, 0);
                        maskPickup.transform.localScale = new Vector3(1f, 1f, 1f);

                        Mask035List.Add(mask.ItemSerial);


                        if (new System.Random().Next(30) == 1)
                        {
                            ItemBase itemBase = ReferenceHub.HostHub.inventory.ServerAddItem(ItemType.SCP1576);

                            String RandomRoomButton = RoomList.RandomItem();
                            ItemPickupBase itemPickup = itemBase.ServerDropItem();
                            itemPickup.transform.position = new Vector3((float)(RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == RandomRoomButton).transform.position.x), (float)RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == RandomRoomButton).transform.position.y + 2, (float)RoomIdentifier.AllRoomIdentifiers?.FirstOrDefault(r => r.name == RandomRoomButton).transform.position.z);
                            itemPickup.transform.rotation = UnityEngine.Quaternion.Euler(0, 0, 0);
                            itemPickup.transform.localScale = new Vector3(1f, 1f, 1f);
                            THEButton.Add(itemBase.ItemSerial);
                            List<Player> Playersss = Player.GetPlayers();
                            //Log.Warning("The Button has spawned.");
                            foreach (var allplrs in Playersss)
                            {
                                if (allplrs.Role != RoleTypeId.Overwatch)
                                {
                                    allplrs.SendBroadcast("<color=#C50000>THE BUTTON</color> has spawned.", 3);


                                }
                            }
                            
                        }


                    
                    }
                }
                catch (Exception ex)
                {
                    if (cfg.Debug == true)
                    {
                        Log.Debug($"Error: {ex}");
                    }
                }


            

        }
        }
        
        private IEnumerator<float> ShowDisplay()
        {

            while (!RoundSummary.singleton._roundEnded)
            {
                yield return Timing.WaitForSeconds(1f);
                try
                {
                    if (Round.IsRoundStarted)
                    {


                        if (scp035s.Count != 0)
                        {
                            Round.IsLocked = true;
                        }


                        List<Player> players = Player.GetPlayers();
                        foreach (var player in players.Where(p => p != null))// && p.CurrentItem == ItemType.SCP207 || p.CurrentItem == ItemType.AntiSCP207))
                        {
                            DisplayCore core = DisplayCore.Get(player.ReferenceHub);


                            if (player.IsReady)
                            {

                                if (RoundEvent == "Foggy")
                                {

                                    // if (player.Role != RoleTypeId.Overwatch && player.Role != RoleTypeId.Spectator)
                                    //{


                                    player.EffectsManager.ChangeState("FogControl", 255, 1.25f, false);





                                    //}

                                }
                                if (RoundEvent == "ClearDay")
                                {
                                    player.EffectsManager.ChangeState("FogControl", 1, 1.25f, false);
                                }

                                int ScpCount = 0;
                                ScpCount = 0;

                                if (player.IsSCP || (player.Role == RoleTypeId.Tutorial && (fbi.Contains(player.PlayerId) || scp035s.Contains(player.PlayerId))))
                                {

                                    ScpCount++;

                                    if (ScpCount >= 1)
                                    {
                                        string text = $"<align=right>";



                                        foreach (var scp in Player.GetPlayers().Where(p => (p?.Role.GetTeam() == Team.SCPs || p.Role == RoleTypeId.Tutorial || thebosszombies.Contains(p.PlayerId)) && cfg.DisplayStrings.ContainsKey(p.Role)))
                                        {
                                            text += (player == scp && true ? "<color=#50C878>(You)</color>" + " " : "") + ProcessStringVariables(cfg.DisplayStrings[scp.Role], player, scp) + "\n";
                                        }

                                        text += $"<voffset={30}em> </voffset></align>";
                                        // player.ReceiveHint(text, 1.25f);
                                        //text += $"</align>";

                                        /*DisplayCore.Get(player.ReferenceHub)*/
                                        core.SetElemTemp(text, 850f, TimeSpan.FromSeconds(1.25), new TimedElemRef<SetElement>());
                                    }


                                }



                                int specCount = 0;

                                specCount = 0;


                                int TargetCount = 0;
                                TargetCount = 0;
                                //Player.GetPlayers().First(x => x.ReferenceHub.IsSpectatedBy(player.ReferenceHub));
                                PlayerSpectators[player] = 0;
                                foreach (var x in players)
                                {
                                    if (players.Count != 1)
                                    {
                                        if (x.IsHuman && x.Role != RoleTypeId.Tutorial)
                                        {
                                            TargetCount++;
                                        }

                                        if (x.Role == RoleTypeId.Spectator)
                                        {
                                            specCount++;
                                            if (specCount != 0)
                                            {
                                                if (player.ReferenceHub.IsSpectatedBy(x.ReferenceHub))
                                                {
                                                    PlayerSpectators[player]++;
                                                  //  DisplayCore.Get(x.ReferenceHub).SetElemTemp($"<color={player.ReferenceHub.roleManager.CurrentRole.RoleColor.ToHex()}><align=left><b><size=75%>        🔪 | {PlayerKills[player]} </size></b></align></color>", 15f, TimeSpan.FromSeconds(1.25), new TimedElemRef<SetElement>());
                                                    //DisplayCore.Get(x.ReferenceHub).SetElemTemp($"<color={player.ReferenceHub.roleManager.CurrentRole.RoleColor.ToHex()}><align=left><b><size=75%>                    👥 | {PlayerSpectators[player]} </size></b></align></color>", 15f, TimeSpan.FromSeconds(1.25), new TimedElemRef<SetElement>());
                                                }
                                            }


                                        }
                                    }

                                }







                                if (scp035s.Contains(player.PlayerId))
                                {
                                    core.SetElemTemp($"<align=right><pos=890><voffset=70><b><size=150%>👤{TargetCount}</size></b></voffset></align>", 890f, TimeSpan.FromSeconds(1.25), new TimedElemRef<SetElement>());
                                }

                                if (player.IsHuman || player.IsSCP || player.IsTutorial && player.Role != RoleTypeId.Scp079 && PlayingEvents == true)
                                {
                                    core.SetElemTemp($"<color={player.ReferenceHub.roleManager.CurrentRole.RoleColor.ToHex()}><align=left><b><size=75%>        🔪 | {PlayerKills[player]} </size></b></align></color>", 15f, TimeSpan.FromSeconds(1.25), new TimedElemRef<SetElement>());

                                    core.SetElemTemp($"<color={player.ReferenceHub.roleManager.CurrentRole.RoleColor.ToHex()}><align=left><b><size=75%>                    👥 | {PlayerSpectators[player]} </size></b></align></color>", 15f, TimeSpan.FromSeconds(1.25), new TimedElemRef<SetElement>());



                                    if (PlayerPreferenceEffectList.ContainsKey(player.UserId))
                                    {
                                        if (PlayerPreferenceEffectList[player.UserId] == true)
                                        {
                                            string text2 = $"<color={player.ReferenceHub.roleManager.CurrentRole.RoleColor.ToHex()}><align=left><b><size=75%>";
                                            text2 += "            ";
                                            text2 += "        ";
                                            text2 += "            ";
                                            text2 += "😎 | <size=40%>";
                                            foreach (StatusEffectBase effect in player.ReferenceHub.playerEffectsController.AllEffects)
                                            {
                                                byte intensity = effect.Intensity;
                                                float duration = effect.Duration;
                                                string name = effect.name;

                                                if (effect.IsEnabled)
                                                {
                                                    text2 += $"{name} {intensity},";
                                                }

                                            }
                                            text2 += "</size></b></align></color>";
                                            core.SetElemTemp(text2, 15f, TimeSpan.FromSeconds(1.25f), new TimedElemRef<SetElement>());
                                        }

                                    }

                                }





                                if (player.IsHuman || player.Role == RoleTypeId.Tutorial)
                                {



                                    if (ItemType.Lantern.Equals(player.ReferenceHub.inventory.NetworkCurItem.TypeId) && ghostLantern.Contains(player.CurrentItem.ItemSerial) && player.Room.name != "HCZ_079" && player.Room != null)
                                    {
                                        player.EffectsManager.ChangeState("Ghostly", 1, 1.25f, false);

                                        if (RoundEvent != "Foggy")
                                        {
                                            player.EffectsManager.ChangeState("FogControl", 255, 1.25f, false);
                                        }

                                        player.EffectsManager.ChangeState("Sinkhole", 1, 1.25f, false);
                                        player.EffectsManager.ChangeState("Poisoned", 1, 1.25f, false);
                                    }

                                    // ReferenceHub PlayersAudioBot;
                                    if (/*!ItemType.SCP207.Equals(player.ReferenceHub.inventory.NetworkCurItem.TypeId) &&*/ player.Room != null)
                                    {


                                        if (player.Room.name == "EZ_upstairs" || player.Room.name == "LCZ_TCross (11)")
                                        {
                                            //  player.ReceiveHint("You may be able to use <color=#C50000>SCP-294</color>. (.scp294 (drink), run [.scp294 list] for a list of available drinks, some are hidden.)", 1.25f);
                                            /*DisplayCore.Get(player.ReferenceHub)*/
                                            core.SetElemTemp("<b>You may be able to use <color=#C50000>SCP-294</color> (.vm (drink), run [.vm list] for a list of drinks.)</b>", 200f, TimeSpan.FromSeconds(1.25), new TimedElemRef<SetElement>());
                                        }

                                        if (player.Room.name == "LCZ_372 (18)")
                                        {
                                            //player.ReceiveHint("You may be able to use <color=#C50000>SCP-1025</color>. (.scp1025)", 1.25f);
                                            /*DisplayCore.Get(player.ReferenceHub)*/
                                            //  core.SetElemTemp("<b>You may be able to use <color=#C50000>SCP-1025</color> (.book) - This will give you a random effect.</b>", 200f, TimeSpan.FromSeconds(1.25), new TimedElemRef<SetElement>());
                                        }
                                    }

                                    /*
                                    if (player.Room.name == "LCZ_914 (14)" && !player.TemporaryData.Contains("scp914_ambience") || player.Room.name != "LCZ_914 (14)" && player.TemporaryData.Contains("scp914_ambience"))
                                    {
                                        PlayersAudioBot = AddDummy();
                                        if (player.Room.name == "LCZ_914 (14)" && !player.TemporaryData.Contains("scp914_ambience"))
                                        {

                                            player.TemporaryData.Add("scp914_ambience", this);

                                            //PlayersAudioBot = AddDummy();

                                            //     PlayersAudioBot = TemporaryBot;
                                            // PlayPlayerAudio096_Loop(player, "ninefourteen.ogg", (byte)65f, PlayersAudioBot);
                                           // player.EffectsManager.EnableEffect<SoundtrackMute>(0, false);
                                        }

                                        if (player.Room.name != "LCZ_914 (14)" && player.TemporaryData.Contains("scp914_ambience"))
                                        {

                                            player.TemporaryData.Remove("scp914_ambience");

                                            //player.EffectsManager.DisableEffect<SoundtrackMute>();
                                            Log.Debug($"{PlayersAudioBot.name}");
                                            //StopAudio096(PlayersAudioBot);
                                            StopAudio096(PlayersAudioBot);
                                            RemoveDummy096(PlayersAudioBot);
                                            Log.Debug($"{PlayersAudioBot.name}");
                                        }

                                    }
                                    */
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    if (cfg.Debug == true)
                    {
                        Log.Debug($"Error: {ex}");
                    }
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
                    scp106.SubroutineModule.TryGetSubroutine(out Scp106VigorAbilityBase vigor);
                    raw = raw.Replace("%106vigor%", Math.Floor(vigor.VigorAmount * 100).ToString() + "%");
                    break;

            }
            if (scp035s.Contains(target.PlayerId))
            {
                raw = raw.Replace("Serpent's Hand Agent", "SCP-035");
            }
            if (thebosszombies.Contains(target.PlayerId))
            {
                raw = raw.Replace("SCP-049-2", "THE BOSS");
            }

            return raw
                .Replace("%healthpercent%", "♥" + Math.Floor(target.Health / target.MaxHealth * 100).ToString())
                .Replace("%health%", "♥" + Math.Floor(target.Health).ToString() + "HP")
                .Replace("%generators%", _generators.Count(gen => gen.Engaged).ToString())
                .Replace("%engaging%", _generators.Count(gen => gen.Activating) > 0 ? $" (+{_generators.Count(gen => gen.Activating)})" : "").Replace("%zombies%", Player.GetPlayers<Player>().Count(p => p.Role == RoleTypeId.Scp0492 && !thebosszombies.Contains(p.PlayerId)).ToString())
          //  .Replace("%distance%", target != observer ? Math.Floor(Vector3.Distance(observer.Position, target.Position)) + "m" : "");
           .Replace("%distance%", "");
        }


        [PluginEvent(ServerEventType.PlayerJoined)]
        void OnPlayerJoin(Player player)
        {


            while (true) { 
                    if (player.IsReady)

                    
                    
                        if (!PlayerKills.TryGetValue(player, out int test))
                        {
                            PlayerKills.Add(player, 0);
                            PlayerSpectators.Add(player, 0);
                         //   Log.Debug(player.ReferenceHub.nicknameSync.Network_playerInfoToShow.ToString());

                       // player.ReferenceHub.nicknameSync.setpl = (PlayerInfoArea.Nickname, PlayerInfoArea.Badge, PlayerInfoArea.CustomInfo, PlayerInfoArea.Role, PlayerInfoArea.UnitName, PlayerInfoArea.PowerStatus)
                        }
                        

                    
                break;
            }

        }






        [PluginEvent(ServerEventType.PlayerReceiveEffect)]
        void OnReceiveEffect(Player plr, StatusEffectBase effect, byte intensity, float duration)
        {

           


            if (effect.name == "Vitality" || effect.name == "MovementBoost" && intensity == 10 || effect.name == "RainbowTaste" && intensity == 1 || effect.name == "DamageReduction" && intensity == 40)
            {
                // ReferenceHub TempDummy = AddDummy();

                
                if (effect.name == "Vitality")
            {


                // PlayPlayerAudio096(plr, "windows.ogg", (byte)65f, TempDummy);

                Timing.CallDelayed(27.5f, () =>
                {
                    //  CheckPlaying(TempDummy);
                    // RemoveDummy096(TempDummy);
                });
            }
            if (effect.name == "MovementBoost" && intensity == 10)
            {
                    //  // ReferenceHub TempDummy = AddDummy();

               // CheckPlaying(TempDummy);
                // PlayPlayerAudio096(plr, "yellowcandy.ogg", (byte)65f, TempDummy);

                Timing.CallDelayed(8.3f, () =>
                {
                    // RemoveDummy096(TempDummy);
                });
            }
            if (effect.name == "DamageReduction" && intensity == 40)
            {
                    //  // ReferenceHub TempDummy = AddDummy();

               // CheckPlaying(TempDummy);
                // PlayPlayerAudio096(plr, "graycandy.ogg", (byte)65f, TempDummy);

                Timing.CallDelayed(14.1f, () =>
                {
                    // RemoveDummy096(TempDummy);
                });
            }
            if (effect.name == "RainbowTaste" && intensity == 1)
            {
                    //   // ReferenceHub TempDummy = AddDummy();

                   // CheckPlaying(TempDummy);
                    // PlayPlayerAudio096(plr, "rainbowcandy.ogg", (byte)75f, TempDummy);


                   

                    Timing.CallDelayed(29f, () =>
                    {

                    // RemoveDummy096(TempDummy);

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

            
            /*
            foreach (var room in Facility.Rooms)
            {
                if (room.GameObject.name == "HCZ_Testroom") { 
                foreach (var component in room.GameObject.GetComponentsInChildren<Component>())
                    try
                    {
                        if (component.name.Contains("Generator"))
                        {
                                continue;
                        }
                        if (component.name.Contains("SCP-079") || component.name.Contains("CCTV"))
                        {
                           // Log.Debug(
                            //    $"Prevented from destroying: {component.name} {component.tag} {component.GetType().FullName}");
                            continue;
                        }

                        if (component.GetComponentsInParent<Component>()
                            .Any(c => c.name.Contains("SCP-079") || c.name.Contains("CCTV")))
                        {
                          //  Log.Debug(
                           //     $"Prevented from destroying: {component.name} {component.tag} {component.GetType().FullName}");
                            continue;
                        }

                             //Log.Debug($"Destroying component: {component.name} {component.tag} {component.GetType().FullName}");

                            Object.Destroy(component);
                        }
                    catch
                    {
                        // ignored
                    }
                }
            }
            */
            

        }

        public bool waswave1mtf = false;
        public bool waswave2mtf = false;

        [PluginEvent(ServerEventType.TeamRespawn)]
        bool OnRespawnWave(SpawnableTeamType team, List<Player> players, int max)
        {
            spawning_team = team;
            respawn_count++;
            Config config = Plugin.Singleton.Config;

            int SpecCount = 0;

            SpecCount = 0;
            foreach (var p in Player.GetPlayers())
            {
                if (p.Role == RoleTypeId.Spectator)
                {
                    SpecCount ++;
                }
            }


            if (config.ShouldSerpentsSpawn == true && isSerpentSpawning == false && new System.Random().Next(8) == 1 && haveSerpentsSpawned == false)
            {
                isSerpentSpawning = true;
               // Plugin.Instance.IsSerpentsSpawning = true;
            }


            if (config.ShouldScienceTeamSpawn == true && isScienceTeamSpawning == false && new System.Random().Next(6) == 1 && havetheScienceTeamSpawned == false)
            {
                isScienceTeamSpawning = true;
               // Plugin.Instance.isScienceTeamSpawning = true;
            }


            if (respawn_count >= 1 && spawning_team == SpawnableTeamType.ChaosInsurgency && config.ShouldSerpentsSpawn == true && players.Count >= 8)
            {
                if (haveSerpentsSpawned == false && isSerpentSpawning == true)
                {
                    isSerpentSpawning = false;
                   // Plugin.Instance.IsSerpentsSpawning = false;

                    if (config.ShouldSerpentsHandSpawnMore == false)
                    {
                        haveSerpentsSpawned = true;
                      //  isSerpentSpawning = false;
                    }

                    

                    if (config.ShouldCassie == true && SpecCount >= 1)
                    {
                        Cassie.Message(config.CassieMessage, true, config.CassieNoise, config.CassieText);
                    }

                    //  PlayAudio("SerpentsHand.ogg", (byte)45f, false);
                    /*
                    Timing.CallDelayed(5f, () =>
                   {
                       isSerpentSpawning = false;
                   });
                    */
                    
                    foreach (var p in Player.GetPlayers())
                    {
                        if (p.Role == RoleTypeId.Spectator)
                        {
                            ChangeToTutorial(p, RoleTypeId.Tutorial);
                            p.TemporaryData.Add("custom_class", this);
                        }
                    }

                    return false;
                }
                else
                {
                    return true;
                }

               
            }
            else if (respawn_count >= 1 && spawning_team == SpawnableTeamType.NineTailedFox && config.ShouldScienceTeamSpawn == true && isScienceTeamSpawning == true && isSerpentSpawning == false)
            {

                if (havetheScienceTeamSpawned == false)
                {
                    isScienceTeamSpawning = false;
                   // Plugin.Instance.isScienceTeamSpawning = true;


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


                    foreach (var p in Player.GetPlayers())
                    {
                        if (p.Role == RoleTypeId.Spectator)
                        {
                            ChangeToScienceTeam(p, RoleTypeId.Scientist);
                            p.TemporaryData.Add("custom_class", this);
                        }
                    }

                        Timing.CallDelayed(0.1f, () =>
                    {
                        Cassie.Clear();
                        if (config.ShouldCassie == true && spawning_team == SpawnableTeamType.NineTailedFox && SpecCount >= 1)
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


                    /*
                    //  PlayAudio("SerpentsHand.ogg", (byte)45f, false);
                    
                    Timing.CallDelayed(5f, () =>
                    {
                        isScienceTeamSpawning = false;
                    });
                    */
                    //       }
                    //    !player.TemporaryData.Contains("custom_class"))

                    //   }
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {

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

                return true;
            }
        }

   
        [PluginEvent(ServerEventType.PlayerSpawn)]
        void OnPlayerSpawned(Player player, RoleTypeId role)
        {
            if (player.CustomInfo != string.Empty)
            {
                player.PlayerInfo.IsUnitNameHidden = false;
                player.PlayerInfo.IsNicknameHidden = false;
                player.PlayerInfo.IsRoleHidden = false;
                player.CustomInfo = string.Empty;

            }

            if (RoundEvent == "PowerBlackout")
            {
                Timing.CallDelayed(1.5f, () =>
                {
                    if (!player.IsInventoryFull && player.IsHuman == true)
                    {





                        switch (UnityEngine.Random.Range(0, 1))
                        {
                            case 0: AddOrDropItem(player, ItemType.Lantern); break;
                            case 1: AddOrDropItem(player, ItemType.Flashlight); break;
                        }

                    }
                });
            }


            if (respawn_count >= 2 && isSerpentSpawning == true)
            {
                if (spawning_team == SpawnableTeamType.ChaosInsurgency && role.GetTeam() == Team.ChaosInsurgency && !player.TemporaryData.Contains("custom_class"))
                {

                    /*
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
                    */
                }
            }


            if (respawn_count >= 2 && isScienceTeamSpawning == true)
            {
                if (spawning_team == SpawnableTeamType.NineTailedFox && role.GetTeam() == Team.FoundationForces && role.GetTeam() != Team.Scientists && spawning_team != SpawnableTeamType.ChaosInsurgency)
                {
                    /*
                    // ewww formatting went bye bye, this is probably really inefficient but it seems to fix my original problem where players would infinitely be set to tutorial or have like thousands of each ammo type
                    Timing.CallDelayed(0.5f, () =>
                    {
                        if (player.Role != RoleTypeId.Scientist)
                        {
                            ChangeToScienceTeam(player, role);
                        }



                        
                    });
                    */
                }
            }



            if (respawn_count >= 2 && isSerpentSpawning == true)
            {
                if (spawning_team == SpawnableTeamType.ChaosInsurgency && role.GetTeam() == Team.ChaosInsurgency && !player.TemporaryData.Contains("custom_class"))
                {
                    /*
                    // ewww formatting went bye bye, this is probably really inefficient but it seems to fix my original problem where players would infinitely be set to tutorial or have like thousands of each ammo type
                    Timing.CallDelayed(0.3f, () =>
                    {
                        if (player.Role != RoleTypeId.Tutorial)
                        {
                           // ChangeToTutorial(player, role);
                        }



                        if (player.Role == RoleTypeId.Tutorial)
                        {

                           // player.TemporaryData.Add("custom_class", this);
                            // player.SendBroadcast("", 10);
                            //   player.TemporaryData.Add("custom_class", this);
                            //  AddOrDropItem(player, ItemType.KeycardFacilityManager);
                            //   AddOrDropFirearm(player, ItemType.GunCOM15, true);
                            //  }
                            // Log.Debug("Serpents hand spawned.");
                        }
                    });
                    */
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

        [PluginEvent(ServerEventType.GrenadeExploded)]
        void OnGrenadeExploded(Footprint owner, Vector3 position, ItemPickupBase item)
        {
            if (cfg.Debug == true)
            {
                Log.Info($"Grenade &6{item.NetworkInfo.ItemId}&r thrown by &6{item.PreviousOwner.Nickname}&r exploded at &6{position.ToString()}&r");
            }

            if (doublenade.Contains(item.NetworkInfo.Serial))
            {
                var nade = item.PreviousOwner.Hub.inventory.CreateItemInstance(new ItemIdentifier(ItemType.GrenadeHE, ItemSerialGenerator.GenerateNext()), false) as ThrowableItem;
                TimeGrenade grenadeboom = (TimeGrenade)UnityEngine.Object.Instantiate(nade.Projectile, position, UnityEngine.Quaternion.identity);
                grenadeboom._fuseTime = 2f;
                grenadeboom.NetworkInfo = new PickupSyncInfo(nade.ItemTypeId, nade.Weight, nade.ItemSerial);
                grenadeboom.PreviousOwner = new Footprint(item.PreviousOwner.Hub != null ? item.PreviousOwner.Hub : ReferenceHub.HostHub);
                NetworkServer.Spawn(grenadeboom.gameObject);
                grenadeboom.ServerActivate();
            }

            if (peanutnade.Contains(item.NetworkInfo.Serial))
            {
                var prefab = NetworkClient.prefabs[1306864341];
                var tantrumObj = UnityEngine.Object.Instantiate(prefab, position, UnityEngine.Quaternion.identity);
                var comp = tantrumObj.GetComponent<TantrumEnvironmentalHazard>();
                comp.SynchronizedPosition = new RelativePosition(position);
            }
        }



        //  public bool doesSubclassMTFexist = false; 
        [PluginEvent(ServerEventType.PlayerChangeRole)]
        void PlayerChangeRole(Player player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
        {
            if (player != null && newRole != RoleTypeId.Spectator || newRole != RoleTypeId.Overwatch || newRole != RoleTypeId.Filmmaker)
            {
                if (player.CustomInfo != string.Empty)
                {

                    player.PlayerInfo.IsRoleHidden = false;
                    player.PlayerInfo.IsNicknameHidden = false;
                    player.PlayerInfo.IsUnitNameHidden = false;

                    //player.CustomInfo = string.Empty;

                    player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = string.Empty;
                }
            }

            if (ScpSwap.CanSwap == true && newRole.GetTeam() == Team.SCPs)
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    player.SendBroadcast("<b><color=#00B7EB>Reminder: To Swap Scp Classes, type .scpswap (scp nickname/number) in your (~) console.</color>\n<color=#FAFF86>You can opt out of playing an SCP by running the command .human</color></b>", 10);
                });
            }


            if (player != null && scp035s.Contains(player.PlayerId))
            {
                scp035s.Remove(player.PlayerId);
                Round.IsLocked = false;
            }
            if (player != null && thebosszombies.Contains(player.PlayerId))
            {
                thebosszombies.Remove(player.PlayerId);
               
            }
            /*
            if (player != null && newRole == RoleTypeId.NtfSergeant)
            {

                 if (Random.Range(1, 6) == 4)
                   {

                Timing.CallDelayed(0.2f, () =>
                {
                    player.SendBroadcast("You are a <color=#00B7EB>Nine-Tailed Fox Demolitionist</color>. Check your inventory.", 10);
                    player.AddItem(ItemType.GrenadeHE);
                    player.AddItem(ItemType.GrenadeHE);

                    //   player.CustomInfo = $"<color=#00B7EB>{player.DisplayNickname}\nNine Tailed Fox Boom Boom Boy</color>";
                    
                    //   player.CustomInfo = $"<color=#00B7EB>{player.DisplayNickname}</color>" + "\n<color=#00B7EB>NINE-TAILED FOX DEMOLITIONIST</color>";
                    
                    player.PlayerInfo.IsRoleHidden = true;
                    player.PlayerInfo.IsNicknameHidden = true;
                    player.PlayerInfo.IsUnitNameHidden = true;
                    
                    


                    player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = $"<color=#00B7EB>{player.DisplayNickname}\nNine-Tailed Fox Boom Boom Boy</color>";
                   // Log.Debug(player.CustomInfo);
                });


                  }
            }
            */


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













            /*
            if (player != null && newRole == RoleTypeId.ChaosRifleman && isSerpentSpawning == false)
            {

                if ((Random.Range(1, 10) == 4))
                {


                    Timing.CallDelayed(0.2f, () =>
                    {

                        if (player.Role == RoleTypeId.ChaosRifleman)
                        {
                            player.SendBroadcast("You are a <color=#4B5320>Chaos Specialist</color>. You have access to ???.", 10);

                            //player.CustomInfo = $"<color=#228b22>{player.DisplayNickname}</color>" + "\n<color=#228b22>CHAOS SPECIALIST</color>";
                            player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = $"<color=#228b22>{player.DisplayNickname}</color>" + "\n<color=#228b22>CHAOS SPECIALIST</color>";

                            player.PlayerInfo.IsNicknameHidden = true;
                            player.PlayerInfo.IsUnitNameHidden = true;
                            player.PlayerInfo.IsRoleHidden = true;

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

                        }

                    });


                }
            }
            
            if (player != null && newRole == RoleTypeId.Scientist && isScienceTeamSpawning == false)
            {

                if ((Random.Range(1, 12) == 1))
                {


                    Timing.CallDelayed(0.3f, () =>
                    {
                        if (player.Role == RoleTypeId.Scientist)
                        {
                            player.SendBroadcast("You are a <color=#FAFF86>Research Supervisor</color>. Check your inventory.", 10);
                            player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = $"<color=#FAFF86>{player.DisplayNickname}\nRESEARCH SUPERVISOR</color>";
                            // player.CustomInfo = $"<color=#FAFF86>{player.DisplayNickname}\nSENIOR RESEARCHER</color>";
                            player.PlayerInfo.IsNicknameHidden = true;
                            player.PlayerInfo.IsUnitNameHidden = true;
                            player.PlayerInfo.IsRoleHidden = true;

                            RemoveItem(player, ItemType.KeycardScientist);
                            Timing.CallDelayed(0.1f, () =>
                            {

                                AddOrDropItem(player, ItemType.KeycardResearchCoordinator);
                            });
                                
                        }
                    });


                }
            }
            if (player != null && newRole == RoleTypeId.NtfPrivate)
            {

                if ((Random.Range(1, 10) == 2))
                {


                    Timing.CallDelayed(0.2f, () =>
                    {

                        if (player.Role == RoleTypeId.NtfPrivate)
                        {

                            player.SendBroadcast("You are a <color=#00B7EB>Nine-Tailed Fox Medic</color>. Check your inventory.", 10);

                            player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = $"<color=#00B7EB>{player.DisplayNickname}</color>" + "\n<color=#00B7EB>NINE-TAILED FOX MEDIC</color>";
                            //  player.CustomInfo = $"<color=#00B7EB>{player.DisplayNickname}</color>" + "\n<color=#00B7EB>NINE-TAILED FOX MEDIC</color>";
                            //   player.PlayerInfo.IsRoleHidden = true;
                            player.PlayerInfo.IsNicknameHidden = true;
                            player.PlayerInfo.IsUnitNameHidden = true;
                            player.PlayerInfo.IsRoleHidden = true;

                            player.AddItem(ItemType.Medkit);
                            player.AddItem(ItemType.Painkillers);
                        }
                       
                    });


                }
            }
            */

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

                   
                });
            }



            if (player != null)
            {
                

                if (RoundEvent == "Nextbots")
                {
                    Timing.CallDelayed(1.5f, () =>
                    {
                        if (player.IsSCP && player.Role != RoleTypeId.Scp079 && player.Role != RoleTypeId.Scp939 && player.Role != RoleTypeId.Scp106)
                        {


                            SetScale(player, new Vector3(1f, 1f, 0.1f));
                        }
                    });
                }

                if (RoundEvent == "EveryoneIsSmall")
                {
                    Timing.CallDelayed(0.3f, () =>
                    {
                        if (newRole != RoleTypeId.Spectator || newRole != RoleTypeId.Filmmaker && newRole != RoleTypeId.Overwatch)
                        {
                            SetScale(player, 0.5f);
                        }
                    });
                }


                if (randomNumber > cfg.EventRarity)
                    return;

                if (reason != RoleChangeReason.RoundStart && reason != RoleChangeReason.LateJoin)
                    return;

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
                if (RoundEvent == "EveryoneIsSmall")
                {
                    Timing.CallDelayed(1.5f, () =>
                    {
                        if (newRole != RoleTypeId.Spectator || newRole != RoleTypeId.Filmmaker && newRole != RoleTypeId.Overwatch)
                        {
                            SetScale(player, 0.5f);
                        }
                    });
                }
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
                if (RoundEvent == "SpecialOps")
                {
                    Timing.CallDelayed(1.5f, () =>
                    {
                        if (newRole == RoleTypeId.FacilityGuard)
                        {


                            switch (UnityEngine.Random.Range(0, 1))
                            {
                                case 0: player.SetRole(RoleTypeId.NtfPrivate); break;
                                case 1: player.SetRole(RoleTypeId.NtfSergeant); break;
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
        

            if (RoundEvent == "PowerBlackout")
            {
                Timing.CallDelayed(1.5f, () =>
                {
                    if (!player.IsInventoryFull && player.IsHuman == true && newRole != RoleTypeId.Overwatch && newRole != RoleTypeId.Spectator)
                    {




                        
                        switch (UnityEngine.Random.Range(0, 1))
                        {
                            case 0: AddOrDropItem(player, ItemType.Lantern); break;
                            case 1: AddOrDropItem(player, ItemType.Flashlight); break;
                        }
                        
                    }
                });
            }
            if (RoundEvent == "EveryoneIsSmall")
            {
                Timing.CallDelayed(0.3f, () =>
                {
                    if (newRole != RoleTypeId.Spectator || newRole != RoleTypeId.Filmmaker && newRole != RoleTypeId.Overwatch)
                    {
                        SetScale(player, 0.5f);
                    }
                });
            }


           

            

        }

        [PluginEvent(ServerEventType.PlayerDeath)]
        void OnPlayerDied(Player player, Player attacker, DamageHandlerBase damageHandler)
        {

           // Log.Debug(damageHandler.GetType().Name);
            Config config = Plugin.Singleton.Config;
            if (player != null)
            {
                if (player.CustomInfo != string.Empty)
                {
                    player.PlayerInfo.IsUnitNameHidden = false;
                    player.PlayerInfo.IsNicknameHidden = false;
                    player.PlayerInfo.IsRoleHidden = false;
                    player.CustomInfo = string.Empty;

                }




                if (scp035s.Contains(player.PlayerId))
                {
                    scp035s.Remove(player.PlayerId);
                    Round.IsLocked = false;
                    if (damageHandler != null)
                    {
                        if (damageHandler is WarheadDamageHandler wdh)
                        {
                            // SubtitledCassie("SCP 0 3 5 Successfully Terminated By Alpha Warhead", "SCP-035 successfully terminated by Alpha Warhead");
                            Cassie.Message("SCP 0 3 5 Successfully Terminated By Alpha Warhead", true, true, true);
                        }


                        /*
                        if (damageHandler is UniversalDamageHandler udh)
                        {
                            if (udh.TranslationId == DeathTranslations.Tesla.Id)
                            {
                                // SubtitledCassie("SCP 0 3 5 successfully terminated by Automatic Security System", "SCP-035 successfully terminated by Automatic Security System.");
                                Cassie.Message("SCP 0 3 5 successfully terminated by Automatic Security System", true, true, true);
                            }
                            else if (udh.TranslationId == DeathTranslations.Decontamination.Id)
                            {
                                //SubtitledCassie("SCP 0 3 5 lost in Decontamination Sequence", "SCP-035 lost in Decontamination Sequence.");
                                Cassie.Message("SCP 0 3 5 lost in Decontamination Sequence", true, true, true);
                            }
                            else
                            {
                                // SubtitledCassie("SCP 0 3 5 Successfully Terminated . Termination cause unspecified", "SCP-035 successfully terminated. Termination cause unspecified.");
                                Cassie.Message("SCP 0 3 5 Successfully Terminated . Termination cause unspecified", true, true, true);
                            }
                        }
                        */
                    }
                    else if (attacker != null)
                    {
                        if (attacker.Role == RoleTypeId.ClassD)
                        {
                            // SubtitledCassie("SCP 0 3 5 Contained Successfully By Class D Personnel", "SCP-035 contained successfully by Class D Personnel");
                            Cassie.Message("SCP 0 3 5 Contained Successfully By Class D Personnel", true, true, true);

                        }
                        if (attacker.IsChaos)
                        {
                            //  SubtitledCassie("SCP 0 3 5 Contained Successfully By Chaos Insurgency", "SCP-035 contained successfully by Chaos Insurgency");
                            Cassie.Message("SCP 0 3 5 Contained Successfully By Chaos Insurgency", true, true, true);
                        }
                        if (attacker.IsNTF)
                        {
                            // SubtitledCassie("SCP 0 3 5 Contained Successfully . Containment Unit .g4", "SCP-035 contained successfully. Containment unit Unknown");
                            Cassie.Message("SCP 0 3 5 Contained Successfully . Containment Unit Unknown", true, true, true);
                        }
                        if (attacker.Role == RoleTypeId.Scientist)
                        {
                            // SubtitledCassie("SCP 0 3 5 Contained Successfully By Science Personnel", "SCP-035 contained successfully. Containment unit Unknown");
                            Cassie.Message("SCP 0 3 5 Contained Successfully By Science Personnel", true, true, true);
                        }
                    }
                    else if (attacker != null && damageHandler != null)
                    {
                        Cassie.Message("SCP 0 3 5 Successfully Terminated . Termination cause unspecified", true, true, true);
                    }
                    Round.IsLocked = false;

                }

                if (thebosszombies.Contains(player.PlayerId))
                {
                    thebosszombies.Remove(player.PlayerId);
                    if (damageHandler != null)
                    {
                        if (damageHandler is WarheadDamageHandler wdh)
                        {
                            // SubtitledCassie("SCP 0 3 5 Successfully Terminated By Alpha Warhead", "SCP-035 successfully terminated by Alpha Warhead");
                            Cassie.Message("SCP 0 4 9 - B Successfully Terminated By Alpha Warhead", true, true, true);
                        }


                        /*
                        if (damageHandler is UniversalDamageHandler udh)
                        {
                            if (udh.TranslationId == DeathTranslations.Tesla.Id)
                            {
                                // SubtitledCassie("SCP 0 3 5 successfully terminated by Automatic Security System", "SCP-035 successfully terminated by Automatic Security System.");
                                Cassie.Message("SCP 0 3 5 successfully terminated by Automatic Security System", true, true, true);
                            }
                            else if (udh.TranslationId == DeathTranslations.Decontamination.Id)
                            {
                                //SubtitledCassie("SCP 0 3 5 lost in Decontamination Sequence", "SCP-035 lost in Decontamination Sequence.");
                                Cassie.Message("SCP 0 3 5 lost in Decontamination Sequence", true, true, true);
                            }
                            else
                            {
                                // SubtitledCassie("SCP 0 3 5 Successfully Terminated . Termination cause unspecified", "SCP-035 successfully terminated. Termination cause unspecified.");
                                Cassie.Message("SCP 0 3 5 Successfully Terminated . Termination cause unspecified", true, true, true);
                            }
                        }
                        */
                    }
                    else if (attacker != null)
                    {
                        if (attacker.Role == RoleTypeId.ClassD)
                        {
                            // SubtitledCassie("SCP 0 3 5 Contained Successfully By Class D Personnel", "SCP-035 contained successfully by Class D Personnel");
                            Cassie.Message("SCP 0 4 9 - B Contained Successfully By Class D Personnel", true, true, true);

                        }
                        if (attacker.IsChaos)
                        {
                            //  SubtitledCassie("SCP 0 3 5 Contained Successfully By Chaos Insurgency", "SCP-035 contained successfully by Chaos Insurgency");
                            Cassie.Message("SCP 0 4 9 - B Contained Successfully By Chaos Insurgency", true, true, true);
                        }
                        if (attacker.IsNTF)
                        {
                            // SubtitledCassie("SCP 0 3 5 Contained Successfully . Containment Unit .g4", "SCP-035 contained successfully. Containment unit Unknown");
                            Cassie.Message("SCP 0 4 9 - B Contained Successfully . Containment Unit Unknown", true, true, true);
                        }
                        if (attacker.Role == RoleTypeId.Scientist)
                        {
                            // SubtitledCassie("SCP 0 3 5 Contained Successfully By Science Personnel", "SCP-035 contained successfully. Containment unit Unknown");
                            Cassie.Message("SCP 0 4 9 - B Contained Successfully By Science Personnel", true, true, true);
                        }
                    }
                    else if (attacker != null && damageHandler != null)
                    {
                        Cassie.Message("SCP 0 4 9 - B Successfully Terminated . Termination cause unspecified", true, true, true);
                    }
                    

                }

                // player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = $"<color=#00B7EB>{player.DisplayNickname}</color>" + "\n<color=#00B7EB>NINE-TAILED FOX MEDIC</color>";

                if (chase096Music.Contains(player.PlayerId))
                {
                    chase096Music.Remove(player.PlayerId);
                    //  randplr.TemporaryData.Remove("scp096ambience");
                //    RemoveDummy096(PlayerAudioBots[player]);
                  //  PlayerAudioBots.Remove(player);
                }


                if (attacker != null && PlayerKills.TryGetValue(attacker, out int test) && attacker != player)
                {
                    PlayerKills[attacker]++;
                    attacker.SendBroadcast($"You killed <color=#C50000>{player.Nickname}</color>. You now have {PlayerKills[attacker]} kills this round.", 3, Broadcast.BroadcastFlags.Normal, false);
                    //DisplayCore.Get(attacker.ReferenceHub).SetElemTemp($"<b>{PlayerKills[attacker]} kills this round.</b>", 100f, TimeSpan.FromSeconds(1.25), new TimedElemRef<SetElement>());
                }
                else
                {
                  //  PlayerKills[attacker]++;
                    // NULL
                }
                

                if (attacker == null && damageHandler is UniversalDamageHandler udh)
                {
                    if (udh.TranslationId == DeathTranslations.PocketDecay.Id)
                    {
                        foreach (var scp106search in Player.GetPlayers())
                        {
                            if (scp106search.Role == RoleTypeId.Scp106)
                            {
                                PlayerKills[scp106search]++;
                                scp106search.SendBroadcast($"You killed <color=#C50000>{player.Nickname}</color>. You now have {PlayerKills[scp106search]} kills this round.", 3, Broadcast.BroadcastFlags.Normal, false);
                            }
                         }
                    }
                }

               

                if (player.Role == RoleTypeId.Scp096)
                {
                    List<Player> Playerss = Player.GetPlayers();
                    
                    foreach (var randplr in Playerss)
                    {

                        if (chase096Music.Contains(randplr.PlayerId))
                        {
                            chase096Music.Remove(randplr.PlayerId);
                            //  randplr.TemporaryData.Remove("scp096ambience");
                        //    RemoveDummy096(PlayerAudioBots[randplr]);
                          //  PlayerAudioBots.Remove(randplr);
                        }
                    }
                }
                

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
                //target.ReceiveHint(config.LastOneAliveHint, 10);
                    if (attacker.IsHuman == true && !attacker.IsTutorial)
                {
                    DisplayCore.Get(attacker.ReferenceHub).SetElemTemp(config.LastOneAliveHint, 200f, TimeSpan.FromSeconds(10), new TimedElemRef<SetElement>());
                }
                    else if (scp035s.Contains(attacker.PlayerId))
                {
                    Round.IsLocked = false;
                }
                   

            }
                    

        }
        [PluginEvent(ServerEventType.Scp173SnapPlayer)]
        public bool OnScp173SnapPlayer(Player player, Player target)
        {
            if (target != null && (fbi.Contains(target.PlayerId) || scp035s.Contains(target.PlayerId))) // && newRole.GetTeam() != Team.ChaosInsurgency
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
            /*
            if (player != null)
            {
                player.PlayerInfo.IsRoleHidden = false;
                player.PlayerInfo.IsNicknameHidden = false;
                player.PlayerInfo.IsRoleHidden = false;
                player.CustomInfo = string.Empty;
            }
            */
            /*
            if (player.IpAddress == "127.0.0.1" || player.IpAddress == "localhost")
            {
                return;

            }
            else
            {
            */
            if (player != null)
            {
                PlayerKills.Remove(player);
                PlayerSpectators.Remove(player);
            }
                
            //}

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
                //player.DisplayNickname = null;
            }
            if (player != null && sci.Contains(player.PlayerId))
            {
                sci.Remove(player.PlayerId);
            }
            if (player != null & scp035s.Contains(player.PlayerId))
            {
                scp035s.Remove(player.PlayerId);
            }
            if (player != null & thebosszombies.Contains(player.PlayerId))
            {
                thebosszombies.Remove(player.PlayerId);
            }
        }

        [PluginEvent(ServerEventType.PlayerCoinFlip)]
        void OnCoinFlip(Player player, bool isTails)
        {
            Timing.CallDelayed(1.4f, () =>
            {
                if (isTails)
                {
                   // player.ReceiveHint("The coin landed on <color=green>tails</color>.", 1.5f);
                    DisplayCore.Get(player.ReferenceHub).SetElemTemp("The coin landed on <color=green>tails</color>.", 400f, TimeSpan.FromSeconds(1.5), new TimedElemRef<SetElement>());
                }
                else
                {
                   // player.ReceiveHint("The coin landed on <color=green>heads</color>.", 1.5f);
                    DisplayCore.Get(player.ReferenceHub).SetElemTemp("The coin landed on <color=green>heads</color>.", 400f, TimeSpan.FromSeconds(1.5), new TimedElemRef<SetElement>());
                }
            });
        }

        [PluginEvent(ServerEventType.Scp096AddingTarget)]
        public bool New096Target(Scp096AddingTargetEvent args)
        {
            if (fbi.Contains(args.Target.PlayerId) || scp035s.Contains(args.Target.PlayerId))
            { 
                return false;
            }
            else
            {


                if (args.Player.RoleBase is Scp096Role scp096Role && scp096Role.StateController.RageState == Scp096RageState.Enraged && !chase096Music.Contains(args.Target.PlayerId))
                {
                   // ReferenceHub PlayersAudioBot = AddDummy();

                    //     PlayersAudioBot = TemporaryBot;
                 //   PlayPlayerAudio_096_HigherVolume(args.Target, "scp096chase.ogg", (byte)75f, PlayersAudioBot);
                    //    args.Target.TemporaryData.Add<args.Target>("scp096chase",args.Target);
                    // args.Target.TemporaryData.Add("scp096chase", args.Target);
                  //  PlayerAudioBots.Add(args.Target, PlayersAudioBot);
                    chase096Music.Add(args.Target.PlayerId);
                }
               
               
                DisplayCore.Get(args.Target.ReferenceHub).SetElemTemp(cfg.targetmessage, 400f, TimeSpan.FromSeconds(5), new TimedElemRef<SetElement>());
                return true;
            }
        }

        [PluginEvent]
        public void OnScp096CalmDown(Scp096ChangeStateEvent ev)
        {
            if (ev.RageState != Scp096RageState.Enraged && ev.RageState != Scp096RageState.Distressed)
            {

                Player player = ev.Player;
                List<Player> Playerss = Player.GetPlayers();


                foreach (var randplr in Playerss)
                {

                    if (chase096Music.Contains(randplr.PlayerId))
                    {
                        chase096Music.Remove(randplr.PlayerId);
                        //  randplr.TemporaryData.Remove("scp096ambience");
                      //  RemoveDummy096(PlayerAudioBots[randplr]);
                     //   PlayerAudioBots.Remove(randplr);
                    }
                }

               
            }
        }

        [PluginEvent]
        public void OnScp096Enrage(Scp096EnragingEvent ev)
        {

            Timing.CallDelayed(6.1f, () =>
            {
                if (ev.Player != null)
                {
                    if (ev.Player.Role == RoleTypeId.Scp096)
                    {
                       // ReferenceHub PlayersAudioBot = AddDummy();
                        // PlayPlayerAudio096(ev.Player, "scp096chase.ogg", (byte)75f, PlayersAudioBot);
                     //   PlayerAudioBots.Add(ev.Player, PlayersAudioBot);
                        chase096Music.Add(ev.Player.PlayerId);
                    }

                }
            });


            List<Player> Playerss = Player.GetPlayers();

            foreach (var randplr in Playerss)
            {

                    if (ev.Player.RoleBase is Scp096Role scp096Role && scp096Role.SubroutineModule.TryGetSubroutine<Scp096TargetsTracker>(out var player096targettrackerr)) //scp096Role.SubroutineModule.TryGetSubroutine(out Scp096RageManager player096RageManager))
                    {

                    //if (player096RageManager._targetsTracker.HasTarget(randplr.ReferenceHub)) {
                    

                        Timing.CallDelayed(6.1f, () =>
                        {
                        if (randplr != null && player096targettrackerr.Targets.Contains(randplr.ReferenceHub))
                        {
                            if (ev.Player.Role == RoleTypeId.Scp096)
                            {
                                Log.Debug("playing music now");
                             //   ReferenceHub PlayersAudioBot = AddDummy();
                               // PlayPlayerAudio_096_HigherVolume(randplr, "scp096chase.ogg", (byte)75f, PlayersAudioBot);
                               // PlayerAudioBots.Add(randplr, PlayersAudioBot);
                                chase096Music.Add(randplr.PlayerId);
                            }

                        }
                        });
                      
                        
                    
                        
                    }
                }
            
        }



        [PluginEvent(ServerEventType.Scp173NewObserver)]
        public bool New173Target(Scp173NewObserverEvent args)
        {
            if (fbi.Contains(args.Target.PlayerId) || scp035s.Contains(args.Target.PlayerId))
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
            else
            {
                if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                {
                    byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                    Timing.CallDelayed(0.2f, () =>
                    {
                        if (plr != null && plr.Role == role)
                        {
                            plr.EffectsManager.EnableEffect<CustomPlayerEffects.Scp207>(0, false);
                            plr.EffectsManager.ChangeState("Scp207", num, 0, false);
                        }
                    });
                }
                else if ((plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp1853 plrEffect) && plrEffect.IsEnabled))
                {
                    byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp1853>().Intensity;
                    Timing.CallDelayed(0.2f, () =>
                    {
                    if (plr != null && plr.Role == role)
                    {
                        plr.EffectsManager.EnableEffect<CustomPlayerEffects.Scp1853>(0, false);
                        plr.EffectsManager.ChangeState("Scp1853", num, 0, false);
                    }
                    });
                }
                else if ((plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.AntiScp207 antiCola) && antiCola.IsEnabled))
                {
                    byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.AntiScp207>().Intensity;
                    Timing.CallDelayed(0.2f, () =>
                    {
                        if (plr != null && plr.Role == role)
                        {
                            plr.EffectsManager.EnableEffect<CustomPlayerEffects.AntiScp207>(0, false);
                            plr.EffectsManager.ChangeState("AntiScp207", num, 0, false);
                        }
                    });
                }
                return true;
            } 
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
            PlayerKills.Clear();
            PlayerSpectators.Clear();
          //  PlayerAudioBots.Clear();
            Server.FriendlyFire = true;
            float restartTime = ConfigFile.ServerConfig.GetFloat("auto_round_restart_time");

            Timing.CallDelayed(restartTime - 0.5f, () =>
            {
                Server.FriendlyFire = false;
            });
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







        [PluginEvent(ServerEventType.PlayerDamage)]
        internal bool OnPlayerDamage(PlayerDamageEvent ev)
        {
            try
            {

               // Log.Info($"Player &6{target.Nickname}&r (&6{target.UserId}&r) received damage from &6{player.Nickname}&r (&6{player.UserId}&r), cause {damageHandler}.");
                if (ev.Target is null)
                    return true;
                if (ev.DamageHandler is null)
                    return true;
                if (ev.DamageHandler is FirearmDamageHandler fdh && ev.Target.IsSCP == true && (fbi.Contains(ev.Player.PlayerId) || scp035s.Contains(ev.Player.PlayerId)))
                    return false;
                if (ev.DamageHandler is JailbirdDamageHandler jdh && ev.Target.IsSCP == true && (fbi.Contains(ev.Player.PlayerId) || scp035s.Contains(ev.Player.PlayerId)))
                    return false;
                if (ev.DamageHandler is ExplosionDamageHandler gdh && ev.Target.IsSCP == true && (fbi.Contains(ev.Player.PlayerId) || scp035s.Contains(ev.Player.PlayerId)))
                    return false;
                if (ev.DamageHandler is DisruptorDamageHandler ddh && ev.Target.IsSCP == true && (fbi.Contains(ev.Player.PlayerId) || scp035s.Contains(ev.Player.PlayerId)))
                    return false;
                if (ev.DamageHandler is MicroHidDamageHandler mhd && ev.Target.IsSCP == true && (fbi.Contains(ev.Player.PlayerId) || scp035s.Contains(ev.Player.PlayerId)))
                    return false;
                if (ev.DamageHandler is Scp939DamageHandler sc939dh && ev.Player.IsSCP == true && (fbi.Contains(ev.Player.PlayerId) || scp035s.Contains(ev.Player.PlayerId)))
                    return false;
                if (ev.DamageHandler is Scp049DamageHandler sc049dh && ev.Player.IsSCP == true && (fbi.Contains(ev.Player.PlayerId) || scp035s.Contains(ev.Player.PlayerId)))
                    return false;
                if (ev.DamageHandler is Scp3114DamageHandler scp3114dh && ev.Player.IsSCP == true && (fbi.Contains(ev.Target.PlayerId) || scp035s.Contains(ev.Target.PlayerId)))
                    return false;
                if ((fbi.Contains(ev.Player.PlayerId) || scp035s.Contains(ev.Player.PlayerId)) && ev.Player.IsTutorial && ev.Target.IsSCP == true)
                    return false;
                if (ev.Player.IsSCP == true && ev.Target.IsTutorial == true && (fbi.Contains(ev.Target.PlayerId) || scp035s.Contains(ev.Target.PlayerId)))
                    return false;
                if (ev.Target.IsHuman == true && !fbi.Contains(ev.Player.PlayerId) && thebosszombies.Contains(ev.Player.PlayerId))
                {

                    return true;
                }
                if (ev.Target.IsHuman == true && !fbi.Contains(ev.Target.PlayerId) && ev.Player.Role == RoleTypeId.Scp106)
                {
                    /*
                    Timing.CallDelayed(0.1f, () =>
                    {
                        ev.Target.EffectsManager.DisableEffect<Traumatized>();
                        ev.Target.EffectsManager.DisableEffect<Corroding>();
                        ev.Target.EffectsManager.EnableEffect<PocketCorroding>(0, false);
                     //   ev.Target.EffectsManager.EnableEffect<Sinkhole>(0, false);

                    });
                    */
                }

                return true;
            }
            catch (Exception e)
            {
                if (cfg.Debug == true)
                {
                    Log.Debug($"Error: {e}");
                }
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
        public static HashSet<ushort> colas_beer = new HashSet<ushort>();
        public static HashSet<ushort> colas_nextbot = new HashSet<ushort>();


        // CUSTOM ITEMS


        // PILLS
        public static HashSet<ushort> resurrection_pills = new HashSet<ushort>();
        public static HashSet<ushort> super_pills = new HashSet<ushort>();
        public static HashSet<ushort> invis_pills = new HashSet<ushort>();
        public static HashSet<ushort> friend_pills = new HashSet<ushort>();
        public static HashSet<ushort> scale_pills = new HashSet<ushort>();
        public static HashSet<ushort> teleport_pills = new HashSet<ushort>();
        public static HashSet<ushort> seeing_adrenaline = new HashSet<ushort>();
        public static HashSet<ushort> scp500s = new HashSet<ushort>();

        // OTHER - ANYTHING ELSE IS AT THE TOP FOR EASY ACCESS FROM OTHER FUNCTIONS
        public static HashSet<ushort> hats = new HashSet<ushort>();
        public static HashSet<ushort> scp1499 = new HashSet<ushort>();






        [CommandHandler(typeof(ClientCommandHandler))]
        public sealed class scp294 : ICommand
        {
            public string Command { get; } = "scp294";

            public string[] Aliases { get; } = new string[] { "294", "coffeemachine", "vendingmachine", "vm" };
         
            public string Description { get; } = "scp 294 command real";


            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                // var list = new List<string> { "deeznuts", "oxygen", "water", "uranium235" };
                Player player = Player.Get(((CommandSender)sender).SenderId);

                if (arguments.Count != 0)
                {
                    if (arguments.Count != 0 && arguments.First().ToLower() == "list" || arguments.First().ToLower() == "help" || arguments.First().ToLower() == "drinks")
                    {
                        player.SendConsoleMessage("List of SCP-294 Drinks: cola, anticola, dietcola, coffee, supercola, invis, pinkcandy, peanut, teleport, ironskin, shrink, grow, timeout, ghost, scp1853", "white");
                      //  player.SendConsoleMessage("DUPLICATE ENTRIES ARE INCLUDED. SOME MAY BE CASE-SENSITIVE; MAKE SURE TO DOUBLE CHECK CAPS / LOWERCASE.","white");
                    }

                    if (arguments.Count != 0 && arguments.First().ToLower() == "oldlist")
                    {
                        player.SendConsoleMessage("Old List of SCP-294 Drinks: oxygen, speed, cola, anit-cola, Coffee, GoldenAtomKick, godmode, Invisibility, Me, Tea, Horror, Borgor, Cheeserburger, Antimatter, Zombie, CherryAtomKick, pinkcandy, Boom, Peanut, Saltwater, Teleport, Windex, Medusa, Candy, BEPIS, Small, grow, LeafLover, Water, Slushy, Ghost, Ice, Death, Steel, RazorBlade, Oil, Bose-Einstein, Beer, slime, scp1853, choccymilk, lava, lemonade, Balls, Crazy", "white");
                    }

                    if (player.Room != null && player.CurrentItem != null && player.CurrentItem.ItemTypeId == ItemType.Coin && arguments.First().ToLower() != "list" && arguments.First().ToLower() != "oldlist" && (player.Room.name == "EZ_upstairs"  || player.Room.name == "LCZ_TCross (11)"))
                    {

                        //player.ReferenceHub.inventory.NetworkCurItem.TypeId



                        if (arguments.First().ToLower() == "nextbot" || arguments.First().ToLower() == "2d" || arguments.First().ToLower() == "cutout" || arguments.First().ToLower() == "dietcola")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";

                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a loud noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            colas_nextbot.Add(thiscola.ItemSerial);

                            // ReferenceHub TempDummy = AddDummy();
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "anticola" || arguments.First().ToLower() == "a207" || arguments.First().ToLower() == "anti-scp-207")
                        {
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a loud noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.AntiSCP207);

                           // colas_nextbot.Add(thiscola.ItemSerial);

                            // ReferenceHub TempDummy = AddDummy();
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "beer" || arguments.First().ToLower() == "vodka" || arguments.First().ToLower() == "addiction")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a loud noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            colas_beer.Add(thiscola.ItemSerial);

                            // ReferenceHub TempDummy = AddDummy();
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "oil" || arguments.First().ToLower() == "freedom")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a loud noise and dispensed you a can of oil.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            colas_oil.Add(thiscola.ItemSerial);
                            // ReferenceHub TempDummy = AddDummy();
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "choccymilk" || arguments.First().ToLower() == "milk")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";

                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a loud noise and dispensed you a bottle of choccy milk.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            choccymilk.Add(thiscola.ItemSerial);
                            // ReferenceHub TempDummy = AddDummy();
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "lemonade" || arguments.First().ToLower() == "pee")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";

                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a loud noise and dispensed you a bottle of lemonade.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            lemonade.Add(thiscola.ItemSerial);
                            // ReferenceHub TempDummy = AddDummy();
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "lava" || arguments.First().ToLower() == "magma")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";

                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a loud noise and dispensed you a bottle of lava.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            lava.Add(thiscola.ItemSerial);
                            // ReferenceHub TempDummy = AddDummy();
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "balls" || arguments.First().ToLower() == "scp-018")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";

                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a loud noise and dispensed you a bottle of balls.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            balls.Add(thiscola.ItemSerial);
                            // ReferenceHub TempDummy = AddDummy();
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "metal" || arguments.First().ToLower() == "steel" || arguments.First().ToLower() == "razorblade")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a loud noise and dispensed you a cup of metal.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);

                            colas_metal.Add(thiscola.ItemSerial);

                            // PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f, TempDummy);

                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "cold" || arguments.First().ToLower() == "ice")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with<color=#C50000>SCP-294</color>, the machine made a loud noise and dispensed you a cup of ice.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.AntiSCP207);
                            // PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f, TempDummy);
                            colas_cold.Add(thiscola.ItemSerial);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        if (arguments.First().ToLower() == "Ghost" || arguments.First().ToLower() == "ghost")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a loud noise and dispensed you a Ghastly Brew.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            colas_ghost.Add(thiscola.ItemSerial);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First().ToLower() == "oxygen" || arguments.First().ToLower() == "Oxygen")
                        {
                            // Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a loud noise and dispensed you a cup of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            colas_oxygen.Add(thiscola.ItemSerial);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First().ToLower() == "Speed" || arguments.First().ToLower() == "speed")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a slight noise pitched up to high levels and dispensed you a cup of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            colas_speed.Add(thiscola.ItemSerial);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First().ToLower() == "coffee" || arguments.First() == "Espresso" || arguments.First() == "espresso")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a slight noise and dispensed you a cup of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_coffee.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "GoldenAtomKick" || arguments.First() == "goldenatomkick" || arguments.First().ToLower() == "supercola" || arguments.First() == "goldenatomkick")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a glittering sound and dispensed you a can of Super Cola!", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.AntiSCP207);
                            colas_atomkick.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First().ToLower() == "god" || arguments.First() == "NuclearKick" || arguments.First() == "godmode" || arguments.First() == "nuclearkick")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a small noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_nuclearkick.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First().ToLower() == "invisibility" || arguments.First().ToLower() == "invis" || arguments.First() == "scp268")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a noise of fabric being cut and dispensed you a cup of SCP-268.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_invis.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "Me" || arguments.First() == "Myself" || arguments.First() == "me" || arguments.First() == "I" || arguments.First().ToLower() == "blood")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a loud rumbling noise and dispensed you a cup of yourself.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_me.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "Tea" || arguments.First() == "tea" || arguments.First() == "teadrink" || arguments.First() == "t")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a slight noise and dispensed you a cup of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_tea.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "Horror" || arguments.First() == "horror" || arguments.First() == "scp106" || arguments.First() == "PocketDimension")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a crunchy noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_horror.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "Borgor" || arguments.First() == "borgor" || arguments.First() == "Cheeseburger" || arguments.First() == "cheseburger")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_borgor.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "antimatter" || arguments.First() == "Antimatter" || arguments.First() == "Nuke" || arguments.First() == "nuke" || arguments.First().ToLower() == "death")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_antimatter.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "049" || arguments.First() == "049-2" || arguments.First() == "Zombie" || arguments.First() == "zombie" || arguments.First() == "LeafLover" || arguments.First() == "leaflover" || arguments.First() == "Leaflover")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_zombie.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "CherryAtomKick" || arguments.First() == "cherryatomkick" || arguments.First() == "CherryatomKick" || arguments.First() == "atomkickcherry" || arguments.First() == "HealthPotion" || arguments.First() == "healthpotion" || arguments.First() == "potion" || arguments.First() == "Potion")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_cherryatomkick.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "grenade" || arguments.First() == "Grenade" || arguments.First().ToLower() == "boom" || arguments.First().ToLower() == "pinkcandy" || arguments.First() == "Boom" || arguments.First().ToLower() == "pink" || arguments.First().ToLower() == "scp-330")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.AntiSCP207);
                            colas_explosion.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "SCP-173" || arguments.First() == "scp173" || arguments.First() == "Peanut" || arguments.First() == "peanut" || arguments.First() == "173")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a crunchy noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_peanut.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "Saltwater" || arguments.First() == "saltwater" || arguments.First() == "SaltWater" || arguments.First() == "salt" || arguments.First() == "Ocean" || arguments.First() == "ocean")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_saltwater.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "Gasoline" || arguments.First() == "gas" || arguments.First() == "Petrol" || arguments.First() == "pterol" || arguments.First() == "gasoline" || arguments.First() == "Gascan")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_gas.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First().ToLower() == "teleport" || arguments.First().ToLower() == "random" || arguments.First().ToLower() == "teleportation" || arguments.First().ToLower() == "tp" || arguments.First().ToLower() == "escape")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_teleportation.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First().ToLower() == "windex" || arguments.First().ToLower() == "wind" || arguments.First().ToLower() == "cleaningsupplies")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a glimmering noise and dispensed you a bottle of Windex.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_windex.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First().ToLower() == "medusa" || arguments.First().ToLower() == "rock" || arguments.First().ToLower() == "tank")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a glimmering noise and dispensed you a bottle of Medusa.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_medusa.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First().ToLower() == "timeout" || arguments.First() == "slushy" || arguments.First() == "slush" || arguments.First() == "Slush")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a glimmering noise and dispensed you a Timeout Potion.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_sour_patch_kids_slushy.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });

                        }
                        else if (arguments.First() == "Crazy" || arguments.First() == "crazy" || arguments.First() == "Crazy?" || arguments.First() == "rubberroom" || arguments.First() == "arubberroom" || arguments.First() == "Iwascrazyonce")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine began to say Crazy? I was crazy once, they locked me in a room, a rubber room, a rubber room with rats, and rats make me crazy. In a robotic voice and dispensed you a drink.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_crazy.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "Small" || arguments.First().ToLower() == "shrink" || arguments.First() == "smol" || arguments.First() == "Tiny" || arguments.First() == "tiny")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a high-pitched noise and dispensed you a potion of Shrinking.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_small.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First().ToLower() == "ironskin")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a odd noise and dispensed you an Iron Skin Potion..", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_bepis.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "SCP-207" || arguments.First() == "scp207" || arguments.First() == "207" || arguments.First() == "cola" || arguments.First() == "Cola" || arguments.First().ToLower() == "coke" )
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a high-pitched noise and dispensed you a bottle of cola.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_scp207.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });
                        }
                        if (arguments.First().ToLower() == "scp-1853" || arguments.First().ToLower() == "1853" || arguments.First().ToLower() == "slime")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a weird noise and dispensed a vile of SCP-1853.", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP1853);
                           // greenjuice.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "water" || arguments.First() == "Water" || arguments.First() == "h2o")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a slight noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_water.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense1.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "flamingo" || arguments.First() == "Flamingo" || arguments.First() == "1507" || arguments.First() == "scp-1507" || arguments.First() == "SCP-1507")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a weird noise and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_flamingo.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });
                        }
                        else if (arguments.First() == "big" || arguments.First() == "Big" || arguments.First() == "large" || arguments.First() == "Large" || arguments.First() == "grow")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made a loud noise and dispensed you a Potion of Growing!", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.SCP207);
                            colas_big.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense2.ogg", (byte)85f,TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });
                        }
                        if (arguments.First().ToLower() == "quantamgas" || arguments.First().ToLower() == "bose-einstein" || arguments.First().ToLower() == "condensate")
                        {
                            //  Log.Debug("send help pls");
                            //response = $"You put a coin in SCP-294, the machine made a slight noise and dispensed you a cup of &6{arguments.First()}";
                            // ReferenceHub TempDummy = AddDummy();
                            player.RemoveItem(player.CurrentItem);
                            player.SendBroadcast($"You exchanged a coin with <color=#C50000>SCP-294</color>, the machine made the sound of gas being released and dispensed you a bottle of {arguments.First()}", 5);
                            //  player.AddItem(ItemType.SCP207); no longer need this lol
                            ItemBase thiscola = player.AddItem(ItemType.AntiSCP207);
                            colas_quantam.Add(thiscola.ItemSerial);
                            // PlayPlayerAudio096(player, "dispense3.ogg", (byte)85f, TempDummy);
                            Timing.CallDelayed(9f, () =>
                            {
                                // RemoveDummy096(TempDummy);
                            });
                        }



                    }
                }
                response = "";
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
                if (cfg.Debug == true)
                {
                    Log.Debug($"Error: {e}");
                }
            }
        }




        [PluginEvent(ServerEventType.PlayerUseItem)]
        void OnPlayerStartedUsingItem(Player plr, UsableItem item)
        {

            /*
            
                Timing.CallDelayed(0.1f, () =>
                {
                    
                    if (plr.ArtificialHealth == 30 && !plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.AntiScp207 sevHands) && !sevHands.IsEnabled && plr.CurrentItem.ItemTypeId != ItemType.Adrenaline) {
                        // ReferenceHub TempDummy = AddDummy();
                        // PlayPlayerAudio096(plr, "orangecandy.ogg", (byte)65f, TempDummy);

                        Timing.CallDelayed(28.2f, () =>
                        {
                            // RemoveDummy096(TempDummy);
                        });
                    }
                    
                });
        */

            /*
            if (item.ItemTypeId == ItemType.MicroHID)
            {
                List<Player> Playerss = Player.GetPlayers();
                foreach (var randplr in Playerss)
                {

                    if (randplr.Role == RoleTypeId.Scp096 && randplr.Room.name == plr.Room.name)
                    {


                        if (randplr.RoleBase is Scp096Role scp096Role && scp096Role.SubroutineModule.TryGetSubroutine<Scp096TargetsTracker>(out var tracker))
                        {
                            tracker.AddTarget(plr.ReferenceHub, true);

                        }

                    }
                }
            }
            */

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
            else if (item.ItemTypeId == ItemType.SCP207 && colas_nextbot.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {
                    if (plr.Role != RoleTypeId.Spectator)
                    {

                        Timing.CallDelayed(0.2f, () =>
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        });
                        if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                        {
                            byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                            plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                        }
                        else
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        }


                        


                        SetScale(plr, new Vector3(1f, 1f, 0.1f));
                        // plr.ReceiveHint("Why?", 3);
                        DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You drank the bottle of Diet Cola.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                    }
                   
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP1853 && greenjuice.Contains(item.ItemSerial))
            {

                Timing.CallDelayed(3f, () =>
                {

                    
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    plr.ReceiveHint("You opened the green juice and spilt it all over yourself. Oops!", 3);
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

                    if (plr.Role != RoleTypeId.Spectator)
                    {

                        Timing.CallDelayed(0.2f, () =>
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        });

                        if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                        {
                            byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                            plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                        }
                        else
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                           // plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        }
                        // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true);
                        plr.Heal(60);
                        plr.EffectsManager.EnableEffect<Invigorated>(28, true);

                        // ReferenceHub TempDummy = AddDummy();
                        // PlayPlayerAudio096(plr, "orangecandy.ogg", (byte)65f, TempDummy);

                        Timing.CallDelayed(28.2f, () =>
                        {
                            // RemoveDummy096(TempDummy);
                        });

                        // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                        //  plr.ReceiveHint("You drank a cup of coffee. It was refreshing.", 3);
                        DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You drank a cup of coffee. It was refreshing.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                    }
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.AntiSCP207 && colas_atomkick.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {
                if (plr.Role != RoleTypeId.Spectator)
                {

                    Timing.CallDelayed(0.2f, () =>
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                    });
                    if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                    {
                        byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                        plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                    }
                    else
                    {
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                    }
                    //plr.EffectsManager.DisableEffect<CustomPlayerEffects.AntiScp207>();
                    plr.EffectsManager.EnableEffect<MovementBoost>(10, true);
                    plr.EffectsManager.ChangeState<MovementBoost>(255, 10, false);
                    plr.EffectsManager.EnableEffect<CardiacArrest>(60, true);
                    plr.EffectsManager.EnableEffect<Bleeding>(60, true);
                    // plr.Heal(50);
                    //  plr.EffectsManager.EnableEffect<Invigorated>(30, true);
                   
                    // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                    //plr.ReceiveHint("You drank a can of golden atom kick. You feel amazing and think about the good times.", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You drank a bottle of super cola!!!!!!!!", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                   }
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
                    plr.EffectsManager.ChangeState<DamageReduction>(50, 5, false);
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
                    if (plr.Role != RoleTypeId.Spectator)
                    {

                        Timing.CallDelayed(0.2f, () =>
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        });
                        if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                        {
                            byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                            plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                        }
                        else
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
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
                    }
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_me.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {
                    if (plr.Role != RoleTypeId.Spectator)
                    {

                        Timing.CallDelayed(0.2f, () =>
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        });

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
                    }
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
                    if (plr.Role != RoleTypeId.Spectator)
                    {

                        Timing.CallDelayed(0.2f, () =>
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        });
                        if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                        {
                            byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                            plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                        }
                        else
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        }
                        // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                        plr.EffectsManager.EnableEffect<Invigorated>(28, true);
                        plr.EffectsManager.EnableEffect<BodyshotReduction>(28, true);
                        plr.EffectsManager.EnableEffect<DamageReduction>(28, true);
                        plr.EffectsManager.EnableEffect<Scp1853>(28, true);
                        // plr.Kill("I don't know what you expected.");


                        // ReferenceHub TempDummy = AddDummy();
                        // PlayPlayerAudio096(plr, "orangecandy.ogg", (byte)65f, TempDummy);

                        Timing.CallDelayed(28.2f, () =>
                        {
                            // RemoveDummy096(TempDummy);
                        });
                        //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);

                        // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                        plr.ReceiveHint("You took a sip of Cherry Atom Kick. It was perfectly refreshing.", 3);
                    }
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_bepis.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {
                    if (plr.Role != RoleTypeId.Spectator)
                    {

                        Timing.CallDelayed(0.2f, () =>
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        });
                        if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                        {
                            byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                            plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                        }
                        else
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        }
                        // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                        plr.EffectsManager.EnableEffect<Invigorated>(20, true);
                        plr.EffectsManager.EnableEffect<BodyshotReduction>(20, true);
                        plr.EffectsManager.ChangeState<BodyshotReduction>(3, 20, false);
                        plr.EffectsManager.EnableEffect<DamageReduction>(20, true);
                        plr.EffectsManager.ChangeState<DamageReduction>(15, 20, false);
                        plr.EffectsManager.EnableEffect<Scp1853>(20, true);

                        // plr.Kill("I don't know what you expected.");

                        //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);

                        // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                        DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You drank the Iron Skin potion.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                    }
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_small.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {
                    if (plr.Role != RoleTypeId.Spectator)
                    {

                        Timing.CallDelayed(0.2f, () =>
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        });
                        if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                        {
                            byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                            plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                        }
                        else
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        }
                        // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                        // plr.EffectsManager.EnableEffect<Invigorated>(20, true);
                        //  plr.EffectsManager.EnableEffect<BodyshotReduction>(20, true);
                        //  plr.EffectsManager.EnableEffect<DamageReduction>(15, true);
                        //  plr.EffectsManager.EnableEffect<Scp1853>(20, true);
                        // plr.Kill("I don't know what you expected.");
                        // SetScale(plr, 0.85f);
                        SetScale(plr, plr.GameObject.transform.localScale.y - 0.2f);
                        // player.GameObject.transform.localScale.y
                        // plr.SCal
                        //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);

                        // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                        DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You drank a potion of shrinking.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

                    }
                });
                // Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) started using item {item.ItemTypeId}");
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_big.Contains(item.ItemSerial))
            {
                //  Log.Debug("SCP-268 was used.");

                Timing.CallDelayed(3.4f, () =>
                {
                    if (plr.Role != RoleTypeId.Spectator)
                    {

                        Timing.CallDelayed(0.2f, () =>
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        });
                        if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                        {
                            byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                            plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                        }
                        else
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
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
                        DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You drank a potion of growing.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                    }
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
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
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
                    // plr.ReceiveHint("You drank the bottle of SCP-173. You suddenly feel the need to go to the bathroom, will you make it?", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("uhhhhhhh", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
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
                    // plr.SendBroadcast("You inhaled the box of gunpowder. BOOM!", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("BOOM!", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
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
                       
                    }
                   
                    
                    else
                    {
                        plrpos = plr.Position;
                        bool PlayerWasOnSurface = false;
                        plr.Position = dimension;
                        if (plr.Room.name == "Outside")
                        {
                            PlayerWasOnSurface = true;
                        }
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
                            if (PlayerWasOnSurface == false && Warhead.IsDetonated == true)
                            {
                                plr.Kill("Warhead Radiation.");
                            }
                            else
                            {
                                
                            }
                           

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
                    if (plr.Role != RoleTypeId.Spectator)
                    {

                        Timing.CallDelayed(0.2f, () =>
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        });
                        if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                        {
                            byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                            plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                        }
                        else
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        }
                        // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                        plr.EffectsManager.EnableEffect<Ghostly>(15, true);
                        plr.EffectsManager.EnableEffect<AmnesiaVision>(15, true);
                        //   plr.EffectsManager.EnableEffect<CustomPlayerEffects.Invisible>(41, true);
                        DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You drank the Ghastly Brew.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

                        // ReferenceHub TempDummy = AddDummy();


                        // PlayPlayerAudio096(plr, "whitecandy.ogg", (byte)65f, TempDummy);



                       



                        //plr.EffectsManager.EnableEffect<PocketCorroding>(120, true);

                        // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                        // plr.ReceiveHint("Borgor.", 3);
                    }
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
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                    }
                    // plr.EffectsManager.EnableEffect<CustomPlayerEffects.>(30, true
                    //plr.EffectsManager.EnableEffect<Flashed>(20, true);
                    //plr.Kill("I don't know what you expected.");

                    //  plr.ReceiveHint("You drank a cup of [REDACTED]. Your items magically disappeared!", 3);

                    //plr.SetRole(RoleTypeId.Scp0492);
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
                  //  RoleTypeId lastrole = RoleTypeId.None;

                    plrpos = plr.Position;
           //          lastrole = plr.Role;

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
                   // RoleTypeId lastrole = RoleTypeId.None;

                    plrpos = plr.Position;
                    //lastrole = plr.Role;
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
                    if (plr.Role != RoleTypeId.Spectator)
                    {

                        Timing.CallDelayed(0.2f, () =>
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        });

                        if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                        {
                            byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                            plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                        }
                        else
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        }
                        // plr.EffectsManager.EnableEffect<MovementBoost>(3, true);
                        //   plr.EffectsManager.ChangeState<MovementBoost>(255, 4, false);
                        // plr.EffectsManager.EnableEffect<Invisible>(10, true);
                        // plr.Heal(50);
                        //   plr.Damage(damageHandlerBase);
                        bool PlayerWasOnSurface = false;
                        plr.ClearBroadcasts();
                        if (plr.Room.name == "Outside")
                        {
                            PlayerWasOnSurface = true;
                        }
                        // plr.SendBroadcast("You drank pure oxygen... You didn't feel so good.", 5);
                        DisplayCore.Get(plr.ReferenceHub).SetElemTemp("Timeout for you!", 450f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                        UnityEngine.Vector3 plrpos = new UnityEngine.Vector3(40f, 1014f, -32.60f);
                        UnityEngine.Vector3 tppos = new UnityEngine.Vector3(40f, 1014f, -32.60f);
                        // plrpos = plr.Position;
                        plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                        //  plr.ReceiveHint("You drank a cup of [REDACTED]. Your items magically disappeared!", 3);
                        plr.ClearBroadcasts();
                        plrpos = plr.Position;
                        plr.Position = tppos;


                        Timing.CallDelayed(20f, () =>
                        {

                            if (!PlayerWasOnSurface && Warhead.IsDetonated)
                            {
                                plr.Kill("The timeout was not able to save you this time.");
                            }
                            else
                            {
                                plr.Position = plrpos;
                                //  plr.EffectsManager.DisableEffect<Invisible>();
                            }
                            //
                        });
                    }
                });
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


                Timing.CallDelayed(3.4f, () =>
                {
                    plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                     plr.ReceiveHint("You drank a cup of lemonade. Refreshing!", 3);
                });
            }
            if (item.ItemTypeId == ItemType.SCP207 && colas_beer.Contains(item.ItemSerial))
            {


                Timing.CallDelayed(3.4f, () =>
                {
                    plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                    plr.EffectsManager.EnableEffect<Burned>(0);
                    plr.ReceiveHint("You drank the entire bottle.", 3);
                });
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

                      //  plr.ReceiveHint("You pressed <color=#C50000>THE BUTTON</color>. Your fate is being decided...");
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You pressed <color=#C50000>THE BUTTON</color>. Your fate is being decided...", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

                    if (new System.Random().Next(2) == 1)
                    {

                        foreach (var randplr in Playerss)
                        {
                            if (!randplr.IsSCP == true && randplr.Role != RoleTypeId.Scp079 && randplr.Role != RoleTypeId.Spectator && randplr.Role != RoleTypeId.Overwatch)
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
                                                            
                                                        });
                                                    });
                                                });
                                            });
                                        });
                                        break;
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
//                    plr.ReceiveHint("You swallowed <color=#C50000>SCP-500-R</color> and summoned a wave of reinforcements.");

                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You swallowed <color=#C50000>SCP-500-R</color> and summoned a wave of reinforcements.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                    foreach (var plrr in Player.GetPlayers())
                    {
                        if (plrr.Role == RoleTypeId.Spectator && plrr != plr)
                        {
                            if (plr.Role == RoleTypeId.Tutorial && fbi.Contains(plr.PlayerId))
                            {
                                ChangeToTutorial(plrr, plr.Role);
                                Timing.CallDelayed(1f, () =>
                                {
                                    plrr.Position = plr.Position;
                                 //   plrr.ReceiveHint($"<color=#00FFFF>{plr.DisplayNickname}</color> has respawned you using the <color=#C50000>SCP-500-R</color>!");
                                    DisplayCore.Get(plrr.ReferenceHub).SetElemTemp("<color=#00FFFF>{plr.DisplayNickname}</color> has respawned you using the <color=#C50000>SCP-500-R</color>!", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

                                });
                            }
                            else
                            {
                                plrr.Role = plr.Role;
                                plrr.Position = plr.Position;
                               // plrr.ReceiveHint($"<color=#00FFFF>{plr.DisplayNickname}</color> has respawned you using the <color=#C50000>SCP-500-R</color>!");

                                DisplayCore.Get(plrr.ReferenceHub).SetElemTemp("<color=#00FFFF>{plr.DisplayNickname}</color> has respawned you using the <color=#C50000>SCP-500-R</color>!", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                            }
                        }
                    }
                });

            }
            else if (item.ItemTypeId == ItemType.SCP500 && super_pills.Contains(item.ItemSerial))
            {
              

                Timing.CallDelayed(1.36f, () =>
                {
                    // plr.ReceiveHint("You swallowed <color=#C50000>SCP-500-A</color>");
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You swallowed <color=#C50000>SCP-500-A</color>", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                    plr.EffectsManager.ChangeState("MovementBoost", 2, 15, true);
                    plr.EffectsManager.ChangeState("Invigorated", 1, 15, true);
                    plr.EffectsManager.ChangeState("DamageReduction", 1, 15, true);
                });

            }
            else if (item.ItemTypeId == ItemType.SCP500 && teleport_pills.Contains(item.ItemSerial))
            {


                Timing.CallDelayed(1.36f, () =>
                {
                    // plr.ReceiveHint("You swallowed <color=#C50000>SCP-500-A</color>");
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You swallowed <color=#C50000>SCP-500-T</color>", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                    if (plr.RoleBase is IFpcRole role)
                    {
                        plr.EffectsManager.EnableEffect<PocketCorroding>();
                        var position = Scp106PocketExitFinder.GetBestExitPosition(role);
                        plr.EffectsManager.DisableEffect<PocketCorroding>();
                        plr.EffectsManager.DisableEffect<Corroding>();
                        plr.Position = position;
                    }
                   
                });

            }
            else if (item.ItemTypeId == ItemType.SCP500 && invis_pills.Contains(item.ItemSerial))
            {


                Timing.CallDelayed(1.36f, () =>
                {
                    //  plr.ReceiveHint("You swallowed <color=#C50000>SCP-500-I</color> You are now invisible for the next 10s unless you use items or interact with doors.");

                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You swallowed <color=#C50000>SCP-500-I</color> You are now invisible for the next 10s unless you use items or interact with doors.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                    plr.EffectsManager.EnableEffect<Invisible>(10, false);
                });

            }
            else if (item.ItemTypeId == ItemType.SCP500 && scale_pills.Contains(item.ItemSerial))
            {


                Timing.CallDelayed(1.36f, () =>
                {
                    // plr.ReceiveHint("You swallowed <color=#C50000>SCP-500-S</color> You are now a (semi) random size.");
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You swallowed <color=#C50000>SCP-500-S</color> You are now a (semi) random size.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                    switch (UnityEngine.Random.Range(0, 7))
                    {
                        case 0: SetScale(plr,1.2f); break;
                        case 1: SetScale(plr,0.9f); break;
                        case 2: SetScale(plr, 0.8f); break;
                        case 3: SetScale(plr, 0.7f); break;
                        case 4: SetScale(plr, 0.75f); break;
                        case 5: SetScale(plr, -1.0f); break;
                        case 6: SetScale(plr, 0.85f); break;
                        case 7: SetScale(plr, 1.1f); break;
                    }
                });

            }
            else if (item.ItemTypeId == ItemType.SCP500 && friend_pills.Contains(item.ItemSerial))
            {


                Timing.CallDelayed(1.36f, () =>
                {
                    // plr.ReceiveHint("You swallowed <color=#C50000>SCP-500-F</color> You should have a friend with you at any second!.");
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You swallowed <color=#C50000>SCP-500-F</color> You should have a friend with you at any second!.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                    foreach (var plrr in Player.GetPlayers())
                    {
                        if (plrr.Role == RoleTypeId.Spectator && plrr != plr)
                        {
                            if (plr.Role == RoleTypeId.Tutorial && fbi.Contains(plr.PlayerId))
                            {
                                ChangeToTutorial(plrr, plr.Role);
                                Timing.CallDelayed(1f, () =>
                                {
                                    plrr.Position = plr.Position;
                                    //plrr.ReceiveHint($"<color=#00FFFF>{plr.DisplayNickname}</color> has respawned you using the <color=#C50000>SCP-500-F</color>!");
                                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp($"<color=#00FFFF>{plr.DisplayNickname}</color> has respawned you using the <color=#C50000>SCP-500-F</color>!", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                                });
                                return;
                            }
                            else
                            {
                                plrr.Role = plr.Role;
                                plrr.Position = plr.Position;
                                DisplayCore.Get(plr.ReferenceHub).SetElemTemp($"<color=#00FFFF>{plr.DisplayNickname}</color> has respawned you using the <color=#C50000>SCP-500-F</color>!", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                                return;
                            }
                        }
                    }

                });

            }
            else if (item.ItemTypeId == ItemType.Adrenaline && seeing_adrenaline.Contains(item.ItemSerial))
            {


                Timing.CallDelayed(1.36f, () =>
                {
                 //   plr.ReceiveHint("You injected the <color=#C50000>Prototype-32-X</color> You are able to see much farther for the next 15s.");
                    plr.EffectsManager.ChangeState("FogControl", 0, 15f, false);

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
                    plr.EffectsManager.EnableEffect<DamageReduction>(35, true);
                    plr.EffectsManager.ChangeState<DamageReduction>(40, 35, false);
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
                    //plr.ReceiveHint("You drank a bottle of cola. You now feel faster...", 3);
                    //DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You pressed <color=#C50000>THE BUTTON</color>. Your fate is being decided...", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                });
            }
            else if (item.ItemTypeId == ItemType.SCP330)
            {     
                foreach (ItemBase itemba in plr.Items)
                {
                    if ((itemba is Scp330Bag bag))
                    {

                        //int candyCount = bag.Candies.Count - 1;
                       // CandyKindID id = bag.Candies[candyCount];

                      //  if (id.ToString().ToLower() == "green")
                        //{
                         //   ReferenceHub GCandyDummy = AddDummy();
                        //    // PlayPlayerAudio096(plr, "windows.ogg", (byte)85f, GCandyDummy);
                         //   Timing.CallDelayed(28f, () =>
                         //   {
                         //       RemoveDummy096(GCandyDummy);
                        //    });
                       // }
                    }
                }
            }
            else if (item.ItemTypeId == ItemType.SCP207 && colas_teleportation.Contains(item.ItemSerial))
            {
                Timing.CallDelayed(3.4f, () =>
                {
                    if (plr.Role != RoleTypeId.Spectator)
                    {

                        Timing.CallDelayed(0.2f, () =>
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        });
                        if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.Scp207 sevHands) && sevHands.IsEnabled)
                        {
                            byte num = plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity;
                            plr.EffectsManager.GetEffect<CustomPlayerEffects.Scp207>().Intensity = (byte)(num - 1);
                        }
                        else
                        {
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Scp207>();
                            plr.EffectsManager.DisableEffect<CustomPlayerEffects.Poisoned>();
                        }
                        DisplayCore.Get(plr.ReferenceHub).SetElemTemp("Good luck!", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                        if (plr.IsAlive && plr.RoleBase is IFpcRole role)
                        {

                            if (plr.EffectsManager.TryGetEffect(out CustomPlayerEffects.PocketCorroding pcc) && pcc.IsEnabled)
                            {
                                plr.SendBroadcast("You were inside of the pocket dimension and your bottle was ineffective.", 5);
                            }
                            else
                            {
                                plr.EffectsManager.EnableEffect<PocketCorroding>();
                                var position = Scp106PocketExitFinder.GetBestExitPosition(role);
                                plr.EffectsManager.DisableEffect<PocketCorroding>();
                                plr.EffectsManager.DisableEffect<Corroding>();
                                plr.Position = position;
                            }
                            


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
                    }
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
                    plr.ReceiveHint("You equipped a cup of pure oxygen.", 3);
                }
                else if (!newItemBase == false && colas_quantam.Contains(newItemBase.ItemSerial))
                {
                    plr.ReceiveHint("You equipped a bottle of quantum gas. It's slightly pink... for some reason.", 3);
                }
                else if (!newItemBase == false && colas_alpha.Contains(newItemBase.ItemSerial))
                {
                    plr.ReceiveHint("You equipped a bottle of (VERY LOUD FLAMINGO BATTLE CRY). It seems very cool.", 3);
                }
                else if (!newItemBase == false && colas_metal.Contains(newItemBase.ItemSerial))
                {
                    plr.ReceiveHint("You equipped a bottle of metal. It is very heavy.", 3);
                }
                else if (!newItemBase == false && colas_beer.Contains(newItemBase.ItemSerial))
                {
                    plr.ReceiveHint("You equipped a bottle of beer.", 3);
                }
                else if (!newItemBase == false && colas_cold.Contains(newItemBase.ItemSerial))
                {
                    plr.ReceiveHint("You equipped a bottle of ice. Your hands begin to freeze.", 3);
                }
                else if (!newItemBase == false && colas_speed.Contains(newItemBase.ItemSerial))
                {
                    plr.ReceiveHint("You equipped a bottle of speed juice. The smell is terrible.", 3);
                }
                else if (!newItemBase == false && colas_water.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a bottle of water. It is just water.", 3);
                }
                else if (!newItemBase == false && colas_coffee.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a cup of coffee. You feel the sudden urge to drink it.", 3);
                }
                else if (!newItemBase == false && colas_atomkick.Contains(newItemBase.ItemSerial))
                {

                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped a bottle of Super Cola!!!!!!!!", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

                }
                else if (!newItemBase == false && colas_nuclearkick.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a bottle of Nuclear Kick!", 3);

                }
                else if (!newItemBase == false && colas_sour_patch_kids_slushy.Contains(newItemBase.ItemSerial))
                {
                    plr.ReceiveHint("You equipped a timeout potion.", 3);

                }
                else if (!newItemBase == false && colas_invis.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a bottle of invisibility.", 3);

                }
                else if (!newItemBase == false && colas_oil.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a can of oil. You feel that this would be better somewhere else and not in your stomach.", 3);

                }
                else if (!newItemBase == false && colas_me.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a bottle of yourself. Why are you doing this?", 3);

                }
                else if (!newItemBase == false && colas_tea.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a cup of tea.", 3);

                }
                else if (!newItemBase == false && colas_horror.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a bottle of horror.", 3);

                }
                else if (!newItemBase == false && colas_borgor.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a cup containing a borgor.", 3);

                }
                else if (!newItemBase == false && colas_antimatter.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a cup of antimatter.", 3);

                }
                else if (!newItemBase == false && colas_zombie.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a cup of [REDACTED].", 3);

                }
                else if (!newItemBase == false && colas_flamingo.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a cup of (FLAMINGO BATTLE CRY).", 3);

                }
                else if (!newItemBase == false && colas_cherryatomkick.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a bottle of Cherry Atom Kick.", 3);

                }
                else if (!newItemBase == false && colas_bepis.Contains(newItemBase.ItemSerial))
                {

                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped an Iron Skin Potion.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

                }
                else if (!newItemBase == false && colas_explosion.Contains(newItemBase.ItemSerial))
                {

                  //  plr.ReceiveHint("You equipped a bottle of gunpowd- WAIT NO", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped an explosive drink.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

                }
                else if (!newItemBase == false && colas_saltwater.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a bottle of saltwater.", 3);

                }
                else if (!newItemBase == false && choccymilk.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a bottle of choccy milk.", 3);

                }
                else if (!newItemBase == false && lemonade.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a bottle of lemonade.", 3);

                }
                else if (!newItemBase == false && lava.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a bottle of lava.", 3);

                }
                else if (!newItemBase == false && balls.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a bottle of balls.", 3);

                }
                else if (!newItemBase == false && colas_peanut.Contains(newItemBase.ItemSerial))
                {

                    //  plr.ReceiveHint("You equipped a bottle of SCP-173.", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped a bottle of SCP-173.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                }
                else if (!newItemBase == false && colas_gas.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a can of gasoline.", 3);

                }
                else if (!newItemBase == false && colas_ghost.Contains(newItemBase.ItemSerial))
                {

                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped a ghostly brew.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

                }
                else if (!newItemBase == false && colas_scp207.Contains(newItemBase.ItemSerial))
                {

                    //plr.ReceiveHint("COCA COLA ESPUMA", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("COCA COLA ESPUMA", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                }
                else if (!newItemBase == false && colas_big.Contains(newItemBase.ItemSerial))
                {

                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped a potion of growing.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

                }
                else if (!newItemBase == false && colas_beer.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a bottle of beer. Don't drink too much.", 3);

                }
                else if (!newItemBase == false && colas_teleportation.Contains(newItemBase.ItemSerial))
                {

                    //  plr.ReceiveHint("You equipped a teleportation potion.", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped a teleportation potion.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                }
                else if (!newItemBase == false && colas_windex.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a bottle of windex.", 3);

                }
                else if (!newItemBase == false && colas_medusa.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("You equipped a bottle of medusa.", 3);

                }
                else if (!newItemBase == false && colas_crazy.Contains(newItemBase.ItemSerial))
                {

                    plr.ReceiveHint("Crazy?", 3);

                }
                else if (!newItemBase == false && colas_small.Contains(newItemBase.ItemSerial))
                {

                 //   plr.ReceiveHint("You equipped a potion of smol.", 3);

                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped a potion of shrinking.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                }


                if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP268)
                {
                   
                     /*
                     if (new System.Random().Next(7) == 1 && !scp1499.Contains(newItemBase.ItemSerial) && !hats.Contains(newItemBase.ItemSerial))
                       {
                        scp1499.Add(newItemBase.ItemSerial);
                        //plr.ReceiveHint("You equipped <color=#C50000>SCP-1499</color> \nPutting it on will transport you to another dimension.", 3);
                        DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped <color=#C50000>SCP-1499</color> \nPutting it on will transport you to another dimension.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

                    }
                    else if (!hats.Contains(newItemBase.ItemSerial) && !scp1499.Contains(newItemBase.ItemSerial))
                        {
                          hats.Add(newItemBase.ItemSerial);
                      }
                      else if (scp1499.Contains(newItemBase.ItemSerial))
                    {
                     //  plr.ReceiveHint("You equipped <color=#C50000>SCP-1499</color> \nPutting it on will transport you to another dimension.", 3);
                        DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped <color=#C50000>SCP-1499</color> \nPutting it on will transport you to another dimension.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                    }
                     */

                }

                else if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.Adrenaline)
                {
                   
          
                }

                else if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.Lantern && !ghostLantern.Contains(newItemBase.ItemSerial) && !normalLantern.Contains(newItemBase.ItemSerial))
                {
                normalLantern.Add(newItemBase.ItemSerial);
                }


                else if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP1576)
                {


                    List<Player> Playerss = Player.GetPlayers();
                    Player randomPlr = Playerss.RandomItem();
                    if (THEButton.Contains(newItemBase.ItemSerial))
                    {
                        // plr.ReceiveHint("You equipped <color=#C50000>THE BUTTON</color>. Use it for a suprise!", 3);
                        DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped <color=#C50000>THE BUTTON</color>. Use it for a suprise!", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                        // List<Player> Playerss = Player.GetPlayers();




                    }


                }

                
                if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP500 && !scp500s.Contains(newItemBase.ItemSerial) && !scale_pills.Contains(newItemBase.ItemSerial) && !friend_pills.Contains(newItemBase.ItemSerial) && !invis_pills.Contains(newItemBase.ItemSerial) && !resurrection_pills.Contains(newItemBase.ItemSerial) && !super_pills.Contains(newItemBase.ItemSerial) && !teleport_pills.Contains(newItemBase.ItemSerial))
                {
                    if (UnityEngine.Random.Range(1, 12) <= 4)
                    {

                        switch (UnityEngine.Random.Range(0, 3))
                        {
                            //case 0: scale_pills.Add(newItemBase.ItemSerial); break;
                            case 0: invis_pills.Add(newItemBase.ItemSerial); break;
                            case 1: super_pills.Add(newItemBase.ItemSerial); break;
                            case 2: friend_pills.Add(newItemBase.ItemSerial); break;
                            case 3: teleport_pills.Add(newItemBase.ItemSerial); break;
                        }

                    }
                    else if (newItemBase.ItemTypeId == ItemType.SCP500 && !scale_pills.Contains(newItemBase.ItemSerial) && !friend_pills.Contains(newItemBase.ItemSerial) && !invis_pills.Contains(newItemBase.ItemSerial) && !resurrection_pills.Contains(newItemBase.ItemSerial) && !super_pills.Contains(newItemBase.ItemSerial) && !scp500s.Contains(newItemBase.ItemSerial) && !teleport_pills.Contains(newItemBase.ItemSerial))
                    {


                        scp500s.Add(newItemBase.ItemSerial);
                       // plr.ReceiveHint("You equipped a <color=#C50000>SCP-500</color>", 3);
                        DisplayCore.Get(plr.ReferenceHub).SetElemTempFunctional("You equipped a <color=#C50000>SCP-500</color>", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                        DisplayCore.Get(plr.ReferenceHub).Update();
                    }

                }




                if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP500 && resurrection_pills.Contains(newItemBase.ItemSerial))
                {
                    //plr.ReceiveHint("You equipped <color=#C50000>SCP-500-R</color> \nConsuming it will respawn any spectators at your position and as your class.", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped <color=#C50000>SCP-500-R</color> \nConsuming it will respawn any spectators at your position and as your class.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                }
                if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP500 && super_pills.Contains(newItemBase.ItemSerial))
                {
                   // plr.ReceiveHint("You equipped <color=#C50000>SCP-500-A</color> \nConsuming it will give you a variety of effects.", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped <color=#C50000>SCP-500-A</color> \nConsuming it will give you a variety of effects.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

                }
                if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP500 && teleport_pills.Contains(newItemBase.ItemSerial))
                {
                    // plr.ReceiveHint("You equipped <color=#C50000>SCP-500-A</color> \nConsuming it will give you a variety of effects.", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped <color=#C50000>SCP-500-T</color> \nConsuming it will teleport you to a random room in your current zone.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

                }
                if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP500 && invis_pills.Contains(newItemBase.ItemSerial))
                {

                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped <color=#C50000>SCP-500-I</color> \nConsuming it will make you invisible for 10s until it runs out or you use items or interact with objects.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                    //plr.ReceiveHint("You equipped <color=#C50000>SCP-500-I</color> \nConsuming it will make you invisible for 10s until it runs out or you use items or interact with objects.", 3);
                }
                if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP500 && scale_pills.Contains(newItemBase.ItemSerial))
                {
                    //plr.ReceiveHint("You equipped <color=#C50000>SCP-500-S</color> \nConsuming it will set you to a semi-random size.", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped <color=#C50000>SCP-500-S</color> \nConsuming it will set you to a semi-random size.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

                }
                if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP500 && friend_pills.Contains(newItemBase.ItemSerial))
                {
                  //  plr.ReceiveHint("You equipped <color=#C50000>SCP-500-F</color> \nConsuming it will bring you a friend! (if there is a spectator availible.)", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped <color=#C50000>SCP-500-F</color> \nConsuming it will bring you a friend! (if there is a spectator availible.)", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

                }
                if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.SCP500 && scp500s.Contains(newItemBase.ItemSerial))
                {
                    // plr.ReceiveHint("You equipped <color=#C50000>SCP-500</color>", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped <color=#C50000>SCP-500</color>", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                }
                if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.GrenadeHE && !doublenade.Contains(newItemBase.ItemSerial) && !peanutnade.Contains(newItemBase.ItemSerial) && !grenades.Contains(newItemBase.ItemSerial) && !freezenade.Contains(newItemBase.ItemSerial))
                {

                    Int32 GrenadeRandomizer = UnityEngine.Random.Range(1, 12);

                    if (GrenadeRandomizer == 2)
                    {
                        doublenade.Add(newItemBase.ItemSerial);
                        //plr.ReceiveHint("You equipped a <color=#C50000>Prototype Grenade</color> \nThis grenade will create a 2nd grenade that will explode 2s after the first explosion.", 3);
                        DisplayCore.Get(plr.ReferenceHub).SetElemTempFunctional("You equipped a <color=#C50000>Prototype Grenade</color> \nThis grenade will create a 2nd grenade that will explode 2s after the first explosion.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                    }
                    else if (GrenadeRandomizer == 3)
                    {
                        peanutnade.Add(newItemBase.ItemSerial);
                        DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped a <color=#C50000>Peanut-Infused Grenade</color> \nThis grenade will leave a nasty residue that slows people walking through it after exploding.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                        //plr.ReceiveHint("You equipped a <color=#C50000>Peanut-Infused Grenade</color> \nThis grenade will leave a nasty residue that slows people walking through it after exploding.", 3);
                    }
                    else
                    {
                        grenades.Add(newItemBase.ItemSerial);
                    }
                }
                else if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.GrenadeHE && doublenade.Contains(newItemBase.ItemSerial))
                {
                    //  plr.ReceiveHint("You equipped a <color=#C50000>Prototype Grenade</color> \nThis grenade will explode a 2nd time after the first one.", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped a <color=#C50000>Prototype Grenade</color> \nThis grenade will explode a 2nd time after the first one.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                }
                else if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.GrenadeHE && peanutnade.Contains(newItemBase.ItemSerial))
                {
                    //plr.ReceiveHint("You equipped a <color=#C50000>Peanut-Infused Grenade</color> \nThis grenade will leave a nasty residue that slows people walking through it after exploding.", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped a <color=#C50000>Peanut-Infused Grenade</color> \nThis grenade will leave a nasty residue that slows people walking through it after exploding.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                }
                else if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.GrenadeHE && freezenade.Contains(newItemBase.ItemSerial) && peanutnade.Contains(newItemBase.ItemSerial))
                {
                   //plr.ReceiveHint("You equipped a <color=#C50000>Ice-Infused Grenade</color> \nThis grenade will do very little damage however it will slow down <b>whoever</b> it hits.", 3);
                    DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped a <color=#C50000>Ice-Infused Grenade</color> \nThis grenade will do very little damage however it will slow down <b>whoever</b> it hits.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                }
                if (!newItemBase == false && newItemBase.ItemTypeId == ItemType.Lantern && RoundEvent != "PowerBlackout")
                {



                    if (ghostLantern.Contains(newItemBase.ItemSerial))
                    {
                        //plr.ReceiveHint("You equipped the <color=#4B5320>Ghastly Lantern</color>. \nYou can now phase through doors, at a cost...", 5);
                        DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You equipped the <color=#4B5320>Ghastly Lantern</color>. \nYou can now phase through doors, at a cost...", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                    }


                }

            }
        }


        [PluginEvent(ServerEventType.PlayerSearchedPickup)]
        void OnSearchedPickup(Player plr, ItemPickupBase pickup)
        {


          
            // Log.Debug(pickup.Info.ItemId.ToString());

            if (Mask035List.Contains(pickup.NetworkInfo.Serial))
            {
               
                
                ChangeTo035(plr, false);
            }
            

            
            if (cfg.Debug == true)
            {
                Log.Info($"Player &6{plr.Nickname}&r (&6{plr.UserId}&r) searched pickup &6{pickup.NetworkInfo.ItemId}&r");
            }





            // Pickup Randomization Logic :D


            /*
            if ((Random == 6)  && pickup.NetworkInfo.ItemId == ItemType.SCP500 && !super_pills.Contains(pickup.NetworkInfo.Serial) && !scp500s.Contains(pickup.NetworkInfo.Serial) && !scale_pills.Contains(pickup.NetworkInfo.Serial) && !friend_pills.Contains(pickup.NetworkInfo.Serial) && !invis_pills.Contains(pickup.NetworkInfo.Serial) && !resurrection_pills.Contains(pickup.NetworkInfo.Serial) && pickup.NetworkInfo.ItemId == ItemType.SCP500)
            {

                if (!resurrection_pills.Contains(pickup.NetworkInfo.Serial) && !scp500s.Contains(pickup.NetworkInfo.Serial))
                {
                    resurrection_pills.Add(pickup.NetworkInfo.Serial);


                }

            }
            else if ((Random == 9) && pickup.NetworkInfo.ItemId == ItemType.SCP500 && !super_pills.Contains(pickup.NetworkInfo.Serial) && !scp500s.Contains(pickup.NetworkInfo.Serial) && !scale_pills.Contains(pickup.NetworkInfo.Serial) && !friend_pills.Contains(pickup.NetworkInfo.Serial) && !invis_pills.Contains(pickup.NetworkInfo.Serial) && !resurrection_pills.Contains(pickup.NetworkInfo.Serial) && pickup.NetworkInfo.ItemId == ItemType.SCP500)
            {

                if (!scale_pills.Contains(pickup.NetworkInfo.Serial) && !scp500s.Contains(pickup.NetworkInfo.Serial))
                {
                    scale_pills.Add(pickup.NetworkInfo.Serial);


                }

            }
            else if ((Random == 11 || Random == 12) && pickup.NetworkInfo.ItemId == ItemType.SCP500 && !super_pills.Contains(pickup.NetworkInfo.Serial) && !scp500s.Contains(pickup.NetworkInfo.Serial) && !scale_pills.Contains(pickup.NetworkInfo.Serial) && !friend_pills.Contains(pickup.NetworkInfo.Serial) && !invis_pills.Contains(pickup.NetworkInfo.Serial) && !resurrection_pills.Contains(pickup.NetworkInfo.Serial) && pickup.NetworkInfo.ItemId == ItemType.SCP500)
            {

                if (!invis_pills.Contains(pickup.NetworkInfo.Serial) && !scp500s.Contains(pickup.NetworkInfo.Serial))
                {
                    invis_pills.Add(pickup.NetworkInfo.Serial);


                }

            }
            else if ((Random == 5 || Random == 3) && pickup.NetworkInfo.ItemId == ItemType.SCP500 && !super_pills.Contains(pickup.NetworkInfo.Serial) && !scp500s.Contains(pickup.NetworkInfo.Serial) && !resurrection_pills.Contains(pickup.NetworkInfo.Serial) && pickup.NetworkInfo.ItemId == ItemType.SCP500)
            {


                if (!super_pills.Contains(pickup.NetworkInfo.Serial) && !scp500s.Contains(pickup.NetworkInfo.Serial))
                {
                    super_pills.Add(pickup.NetworkInfo.Serial);


                }




            }
            else if ((Random == 5 || Random == 3) && pickup.NetworkInfo.ItemId == ItemType.SCP500 && !super_pills.Contains(pickup.NetworkInfo.Serial) && !scp500s.Contains(pickup.NetworkInfo.Serial) && !resurrection_pills.Contains(pickup.NetworkInfo.Serial) && pickup.NetworkInfo.ItemId == ItemType.SCP500)
            {


                if (!super_pills.Contains(pickup.NetworkInfo.Serial) && !scp500s.Contains(pickup.NetworkInfo.Serial))
                {
                    super_pills.Add(pickup.NetworkInfo.Serial);


                }




            }
            */

            //Int32 RandomN = new System.Random().Next(1, 12);

            /*
           
                if (UnityEngine.Random.Range(1, 12) <= 3 && pickup.Info.ItemId == ItemType.SCP500 && !scp500s.Contains(pickup.NetworkInfo.Serial) && !scale_pills.Contains(pickup.NetworkInfo.Serial) && !friend_pills.Contains(pickup.NetworkInfo.Serial) && !invis_pills.Contains(pickup.NetworkInfo.Serial) && !resurrection_pills.Contains(pickup.NetworkInfo.Serial) && !super_pills.Contains(pickup.NetworkInfo.Serial)) {

                switch (UnityEngine.Random.Range(0, 5))
                {
                    case 0: scale_pills.Add(pickup.NetworkInfo.Serial); break;
                    case 1: invis_pills.Add(pickup.NetworkInfo.Serial); break;
                    case 2: resurrection_pills.Add(pickup.NetworkInfo.Serial); break;
                    case 3: super_pills.Add(pickup.NetworkInfo.Serial); break;
                    case 4: friend_pills.Add(pickup.NetworkInfo.Serial); break;
                    case 5: scp500s.Add(pickup.NetworkInfo.Serial); break;
                }

            }
            else if(pickup.NetworkInfo.ItemId == ItemType.SCP500 && !scale_pills.Contains(pickup.NetworkInfo.Serial) && !friend_pills.Contains(pickup.NetworkInfo.Serial) && !invis_pills.Contains(pickup.NetworkInfo.Serial) && !resurrection_pills.Contains(pickup.NetworkInfo.Serial) && !super_pills.Contains(pickup.NetworkInfo.Serial) && !scp500s.Contains(pickup.NetworkInfo.Serial))
            {
                
                    
                        scp500s.Add(pickup.NetworkInfo.Serial);
                        plr.ReceiveHint("You picked up <color=#C50000>SCP-500</color>", 3);
            }
            */
            /*
                       */

            /*
            if (pickup.NetworkInfo.ItemId == ItemType.GrenadeHE && !doublenade.Contains(pickup.Info.Serial) && !grenades.Contains(pickup.Info.Serial))
            {
                if (UnityEngine.Random.Range(1, 12) == 2)
                {
                    doublenade.Add(pickup.Info.Serial);
                    plr.ReceiveHint("You picked up a <color=#C50000>Prototype Grenade</color> \nThis grenade will create a 2nd grenade that will explode 2s after the first explosion.", 3);
                }
                else
                {
                    grenades.Add(pickup.Info.Serial);
                }
            }
           
             */

            if (pickup.NetworkInfo.ItemId == ItemType.Lantern && RoundEvent != "PowerBlackout")
            {


                if (new System.Random().Next(5) == 1 && !ghostLantern.Contains(pickup.NetworkInfo.Serial) && !normalLantern.Contains(pickup.NetworkInfo.Serial))
                {
                 //   ghostLantern.Add(pickup.NetworkInfo.Serial);
                  //  plr.ReceiveHint("You equipped the <color=#4B5320> Ghastly Lantern</color>. You can now phase through doors, at a cost...", 3);

                }
                else if (!normalLantern.Contains(pickup.NetworkInfo.Serial) && !ghostLantern.Contains(pickup.NetworkInfo.Serial))
                {
                    normalLantern.Add(pickup.NetworkInfo.Serial);
                }
                else if (ghostLantern.Contains(pickup.NetworkInfo.Serial))
                {
                  //  plr.ReceiveHint("You equipped the <color=#4B5320> Ghastly Lantern</color>. You can now phase through doors, at a cost...", 3);
                }


            }

            // Pickup notifs for custom items

            if (pickup.NetworkInfo.ItemId == ItemType.SCP500 && super_pills.Contains(pickup.NetworkInfo.Serial))
            {
                //plr.ReceiveHint("You picked up <color=#C50000>SCP-500-A</color> \nConsuming it will give you a variety of effects.", 3);
                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up <color=#C50000>SCP-500-A</color> \nConsuming it will give you a variety of effects.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                /*
                if (!super_pills.Contains(pickup.NetworkInfo.Serial))
                {
                    scale_pills.Add(pickup.NetworkInfo.Serial); 
                }
                */
            }
            if (doublenade.Contains(pickup.NetworkInfo.Serial) && pickup.NetworkInfo.ItemId == ItemType.GrenadeHE && !peanutnade.Contains(pickup.NetworkInfo.Serial) && !grenades.Contains(pickup.NetworkInfo.Serial)) 
            {
                //plr.ReceiveHint("You picked up a <color=#4B5320>Prototype Grenade</color> \nThis grenade will explode a 2nd time after the first one.", 3);
                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up a <color=#4B5320>Prototype Grenade</color> \nThis grenade will explode a 2nd time after the first one.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
            }
            if (peanutnade.Contains(pickup.NetworkInfo.Serial) && pickup.NetworkInfo.ItemId == ItemType.GrenadeHE && !doublenade.Contains(pickup.NetworkInfo.Serial) && !grenades.Contains(pickup.NetworkInfo.Serial))
            {
               // plr.ReceiveHint("You picked up a <color=#4B5320>Peanut-Infused Grenade</color> \nThis grenade will leave a nasty residue that slows people walking through it after exploding.", 4);
                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up a <color=#4B5320>Peanut-Infused Grenade</color> \nThis grenade will leave a nasty residue that slows people walking through it after exploding.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
            }
            if (peanutnade.Contains(pickup.NetworkInfo.Serial) && pickup.NetworkInfo.ItemId == ItemType.GrenadeHE && !doublenade.Contains(pickup.NetworkInfo.Serial) && freezenade.Contains(pickup.NetworkInfo.Serial) && !grenades.Contains(pickup.NetworkInfo.Serial))
            {
                //plr.ReceiveHint("You picked up a <color=#C50000>Ice-Infused Grenade</color> \nThis grenade will do very little damage however it will slow down <b>whoever</b> it hits.", 3);
                //freezenade.Add(pickup.NetworkInfo.Serial);
            }
            if (pickup.NetworkInfo.ItemId == ItemType.SCP500 && resurrection_pills.Contains(pickup.NetworkInfo.Serial))
            {
                //plr.ReceiveHint("You picked up <color=#C50000>SCP-500-R</color> \nConsuming it will respawn any spectators at your position and as your class.", 3);
                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up <color=#C50000>SCP-500-R</color> \nConsuming it will respawn any spectators at your position and as your class.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

                /*
                if (!resurrection_pills.Contains(pickup.NetworkInfo.Serial))
                {
                    resurrection_pills.Add(pickup.NetworkInfo.Serial);
                }
                */
            }
            if (pickup.NetworkInfo.ItemId == ItemType.SCP500 && invis_pills.Contains(pickup.NetworkInfo.Serial)) //  && !scp500s.Contains(pickup.NetworkInfo.Serial)
            {
              //  plr.ReceiveHint("You picked up <color=#C50000>SCP-500-I</color> \nConsuming it will make you invisible for 10s until it runs out or you use items or interact with objects.", 3);
                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up <color=#C50000>SCP-500-I</color> \nConsuming it will make you invisible for 10s until it runs out or you use items or interact with objects.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                /*
                if (!invis_pills.Contains(pickup.NetworkInfo.Serial))
                {
                    invis_pills.Add(pickup.NetworkInfo.Serial);
                }
              */
            }
            if (pickup.NetworkInfo.ItemId == ItemType.SCP500 && scale_pills.Contains(pickup.NetworkInfo.Serial ))
            {
                // plr.ReceiveHint("You picked up <color=#C50000>SCP-500-S</color> \nConsuming it will set you to a semi-random size.", 3);

                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up <color=#C50000>SCP-500-S</color> \nConsuming it will set you to a semi-random size.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                /*
                if (!scale_pills.Contains(pickup.NetworkInfo.Serial))
                {
                     scale_pills.Add(pickup.NetworkInfo.Serial);
                    
                    }
                */
            }
            if (pickup.NetworkInfo.ItemId == ItemType.SCP500 && friend_pills.Contains(pickup.NetworkInfo.Serial))
            {
               // plr.ReceiveHint("You picked up <color=#C50000>SCP-500-F</color> \nConsuming it will bring you a friend! (if there is a spectator availible.)", 3);
                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up <color=#C50000>SCP-500-F</color> \nConsuming it will bring you a friend! (if there is a spectator availible.)", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
                /*
                if (friend_pills.Contains(pickup.NetworkInfo.Serial))
                {
                    friend_pills.Add(pickup.NetworkInfo.Serial);
                }
                */
            }
            if (pickup.NetworkInfo.ItemId == ItemType.Lantern && ghostLantern.Contains(pickup.NetworkInfo.Serial) && !normalLantern.Contains(pickup.NetworkInfo.Serial))
            {
        //        plr.ReceiveHint("You picked up the <color=#4B5320>Ghastly Lantern</color>. \nWhile held, it will let you phase through most doors.", 5);
                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up the <color=#4B5320>Ghastly Lantern</color>. \nWhile held, it will let you phase through most doors.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
            }
            if (pickup.NetworkInfo.ItemId == ItemType.SCP268 && scp1499.Contains(pickup.NetworkInfo.Serial) && !hats.Contains(pickup.NetworkInfo.Serial))
            {
                // plr.ReceiveHint("You picked up <color=#C50000>SCP-1499</color> \nPutting it on will transport you to another dimension.", 3);
                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up <color=#C50000>SCP-1499</color> \nPutting it on will transport you to another dimension.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
            }
            if (pickup.NetworkInfo.ItemId == ItemType.GrenadeHE && doublenade.Contains(pickup.NetworkInfo.Serial) && !grenades.Contains(pickup.NetworkInfo.Serial))
            {
               // plr.ReceiveHint("You picked up a <color=#C50000>Prototype Grenade</color> \nThis grenade will create a 2nd grenade that will explode 2s after the first explosion.", 3);
            }


            if (pickup.NetworkInfo.ItemId == ItemType.SCP500 && scp500s.Contains(pickup.NetworkInfo.Serial))
            {
               // plr.ReceiveHint("You picked up <color=#C50000>SCP-500</color>.", 3);
                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up <color=#C50000>SCP-500</color>.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
            }


            
            // Custom Items for drinks
            // not converting to ruei bc lazy :D
            if (colas_oxygen.Contains(pickup.NetworkInfo.Serial))
            {
                plr.ReceiveHint("You picked up a cup of pure oxygen.", 3);
            }
            if (colas_quantam.Contains(pickup.NetworkInfo.Serial))
            {
                plr.ReceiveHint("You picked up a bottle of quantum gas.", 3);
            }
            else if (colas_alpha.Contains(pickup.NetworkInfo.Serial))
            {
                plr.ReceiveHint("You picked up a bottle of (VERY LOUD FLAMINGO BATTLE CRY).", 3);
            }
            else if (colas_metal.Contains(pickup.NetworkInfo.Serial))
            {
                plr.ReceiveHint("You picked up a bottle of metal. It is very heavy.", 3);
            }
            else if (colas_cold.Contains(pickup.NetworkInfo.Serial))
            {
                plr.ReceiveHint("You picked up a bottle of ice.", 3);
            }
            else if (colas_speed.Contains(pickup.NetworkInfo.Serial))
            {
                plr.ReceiveHint("You picked up a bottle of speed juice.", 3);
            }
            else if (colas_water.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a bottle of water.", 3);
            }
            else if (colas_coffee.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a cup of coffee.", 3);
            }
            else if (colas_atomkick.Contains(pickup.NetworkInfo.Serial))
            {

                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up a bottle of super cola!!!!!!!!", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

            }
            else if (colas_nuclearkick.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a bottle of Nuclear Kick!", 3);

            }
            else if (colas_sour_patch_kids_slushy.Contains(pickup.NetworkInfo.Serial))
            {
                plr.ReceiveHint("You picked up a timeout potion.", 3);

            }
            else if (colas_invis.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a bottle of invisibility.", 3);

            }
            else if (colas_oil.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a can of oil.", 3);

            }
            else if (colas_me.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a bottle of yourself. Why are you doing this?", 3);

            }
            else if (colas_tea.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a cup of tea.", 3);

            }
            else if (colas_horror.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a bottle of horror.", 3);

            }
            else if (colas_borgor.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a cup containing a borgor.", 3);

            }
            else if (colas_antimatter.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a cup of antimatter.", 3);

            }
            else if (colas_zombie.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a cup of [REDACTED].", 3);

            }
            else if (colas_flamingo.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked a cup of (FLAMINGO BATTLE CRY).", 3);

            }
            else if (colas_cherryatomkick.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked a bottle of Cherry Atom Kick.", 3);

            }
            else if (colas_bepis.Contains(pickup.NetworkInfo.Serial))
            {

                //   plr.ReceiveHint("You picked a can of Bepis.", 3);
                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up a Iron Skin potion.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
            }
            else if (colas_explosion.Contains(pickup.NetworkInfo.Serial))
            {

               // plr.ReceiveHint("You picked up a bottle of gunpowd- WAIT NO", 3);
              DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up an explosive drink.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

            }
            else if (colas_saltwater.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a bottle of saltwater.", 3);

            }
            else if (choccymilk.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a bottle of choccy milk.", 3);

            }
            else if (lemonade.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a bottle of lemonade.", 3);

            }
            else if (lava.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a bottle of lava.", 3);

            }
            else if (balls.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a bottle of balls.", 3);

            }
            else if (colas_peanut.Contains(pickup.NetworkInfo.Serial))
            {

            //    plr.ReceiveHint("You picked up a bottle of SCP-173.", 3);
                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up a bottle of SCP-173.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

            }
            else if (colas_gas.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a can of gasoline.", 3);

            }
            else if (colas_ghost.Contains(pickup.NetworkInfo.Serial))
            {

                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up a ghostly brew.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

            }
            else if (colas_scp207.Contains(pickup.NetworkInfo.Serial))
            {

              //  plr.ReceiveHint("You picked up a bottle of cola.", 3);
                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up a bottle of cola.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

            }
            else if (colas_big.Contains(pickup.NetworkInfo.Serial))
            {

                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up a potion of shrinking.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

            }
            else if (colas_teleportation.Contains(pickup.NetworkInfo.Serial))
            {

                // plr.ReceiveHint("You picked up a teleportation potion.", 3);
                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up a teleportation potion.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
            }
            else if (colas_windex.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a bottle of windex.", 3);

            }
            else if (colas_medusa.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a bottle of medusa.", 3);

            }
            else if (colas_crazy.Contains(pickup.NetworkInfo.Serial))
            {

                plr.ReceiveHint("You picked up a bottle of <color=C50000>DO NOT DRINK</color>", 3);

            }
            else if (colas_small.Contains(pickup.NetworkInfo.Serial))
            {

                DisplayCore.Get(plr.ReferenceHub).SetElemTemp("You picked up a potion of shrinking.", 400f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());

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
        public class SetEvents : ICommand
        {
            public string Command { get; } = "setevents";

            public string[] Aliases { get; } = new string[] { };

            public string Description { get; } = "give scp-1499";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player;
                if (Player.TryGet(sender, out player))
                {
                    if (PlayingEvents == true)
                    {
                        PlayingEvents = true;

                    }
                    else
                    {
                        PlayingEvents = false;
                    }
                    response = "success";
                    return true;
                }
                response = "failed";
                return false;
            }
        }


        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class DestroyRoom : ICommand
        {
            public string Command { get; } = "destroyroom";

            public string[] Aliases { get; } = new string[] { };

            public string Description { get; } = "didn't know this was possible, lmaoo";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player;
                if (Player.TryGet(sender, out player))
                {
                    foreach (var component in player.Room.gameObject.GetComponentsInChildren<Component>())
                        try
                        {
                            if (component.name.Contains("SCP-079") || component.name.Contains("CCTV"))
                            {
                                Log.Debug(
                                    $"Prevent from destroying: {component.name} {component.tag} {component.GetType().FullName}");
                                continue;
                            }

                            if (component.GetComponentsInParent<Component>()
                                .Any(c => c.name.Contains("SCP-079") || c.name.Contains("CCTV")))
                            {
                                Log.Debug(
                                    $"Prevent from destroying: {component.name} {component.tag} {component.GetType().FullName}");
                                continue;
                            }

                            Log.Debug($"Destroying component: {component.name} {component.tag} {component.GetType().FullName}");

                            Object.Destroy(component);
                        }
                        catch
                        {
                            // ignored
                        }
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
                if (Player.TryGet(sender, out player) && RoundEvent != "EveryoneIsSmall")
                {
                    SetScale(player, 1.0f);
                    response = "success, your scale should be fixed. Have fun ^_^ \nNote: If desync issues persist, please rejoin the game.";
                    return true;
                }
                response = "failed. Either you are somehow set to null or the current event is EveryoneIsSmall";
                return false;
            }
        }



        [CommandHandler(typeof(ClientCommandHandler))]
        public class optineffectlist : ICommand
        {
            public string Command { get; } = "tgel";

            public string[] Aliases { get; } = new string[] { };

            public string Description { get; } = "Opt in to having an effect list on your screen.";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player;
                if (Player.TryGet(sender, out player))
                {
                    

                    if (PlayerPreferenceEffectList.ContainsKey(player.UserId))
                    {
                        PlayerPreferenceEffectList.Remove(player.UserId);
                        response = "Removed your effect list.";
                        return true;
                    }
                    else
                    {
                        PlayerPreferenceEffectList.Add(player.UserId, true);
                        response = "You should now be able to view your effects in the top left.";
                        return true;
                    }


                }
                response = "failed!";
                return false;
            }
        }




        [CommandHandler(typeof(ClientCommandHandler))]
        public class nickname : ICommand
        {
            public string Command { get; } = "nickname";

            public string[] Aliases { get; } = new string[] {"nick","setnick","setnickname","setname"};

            public string Description { get; } = "Don't abuse this, thanks!";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player;
                if (Player.TryGet(sender, out player) && arguments.Count != 0)
                {
                    if (arguments.First().ToLower() == "reset")
                    {
                        player.DisplayNickname = null;
                    }
                    else
                    {
                        player.DisplayNickname = String.Join(" ", arguments);
                    }
                   
                    response = "Success! If you want to reset it, run it like this: .nickname reset";
                    return true;
                }
                response = "Failed";
                return false;
            }
        }




        [CommandHandler(typeof(ClientCommandHandler))]
        public class kill : ICommand
        {
            public string Command { get; } = "kill";

            public string[] Aliases { get; } = new string[] { };

            public string Description { get; } = "There is nothing we can do.";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player;
                if (Player.TryGet(sender, out player) && player.Role != RoleTypeId.Spectator && !player.IsSCP)
                {
                    player.Kill("Suicide");
                    response = "Goodbye!";
                    return true;
                }
                response = "you can't do that, try it again later nerd.";
                return false;
            }
        }


        


        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class setscale : ICommand
        {
            public string Command { get; } = "setscale";

            public string[] Aliases { get; } = new string[] { };

            public string Description { get; } = "Set player scale (Syntax: setscale Scale PlayerName)";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {


                Player player;

                if (Player.TryGet(sender, out player))
                {
                    if (arguments.Count != 0)
                    {
                        foreach (var p in Player.GetPlayers())
                        {
                            if (p.Role != RoleTypeId.Spectator)
                            {
                                if (p.Nickname == arguments.At(1))
                                {
                                    SetScale(p, float.Parse(arguments.First()));
                                }
                               
                                
                            }
                        }
                        // response = " Success, you gave your coin for: ";
                        // problem if statement, wants me to stop comparing a string to a system.predicate string I'm probably stupid but yeah  if (arguments.First() == list.Find("deeznuts"))
                        //  {

                        //}    

                        
                        response = "set player scale";
                        return true;
                    }






                }
                response = "failed";
                return false;
            }
        }




        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class changeetoooo035 : ICommand
        {
            public string Command { get; } = "changeto035";

            public string[] Aliases { get; } = new string[] { };

            public string Description { get; } = "Sets you to scp-035 for testing.";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player;
                if (Player.TryGet(sender, out player))
                {
                        ChangeTo035(player,false);


                        response = "set player scale";
                        return true;
                }





                response = "failed";
                return false;
            }
            
        }





        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class RocketPlayer : ICommand
        {
            public string Command { get; } = "rocket";

            public string[] Aliases { get; } = new string[] { };

            public string Description { get; } = "Rocket someone! (Syntax: rocket PlayerName Speed)";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {


                Player player;

                if (Player.TryGet(sender, out player))
                {
                    if (arguments.Count != 0)
                    {
                        foreach (var p in Player.GetPlayers())
                        {
                            if (p.Role != RoleTypeId.Spectator)
                            {
                                if (p.Nickname.ToLower() == arguments.First().ToLower())
                                {
                                    if (float.TryParse(arguments.At(1), out float spd))
                                    {
                                        Timing.RunCoroutine(DoRocket(p, spd));
                                    }
                                    
                                }


                            }
                        }
                        // response = " Success, you gave your coin for: ";
                        // problem if statement, wants me to stop comparing a string to a system.predicate string I'm probably stupid but yeah  if (arguments.First() == list.Find("deeznuts"))
                        //  {

                        //}    


                        response = $"Rocketed {arguments.First()}";
                        return true;
                    }






                }
                response = "failed";
                return false;
            }
        }



        // Axwabo's NW Port. I do not take any credit for this code besides swapping out the grenade spawning system.
        public static IEnumerator<float> DoRocket(Player player, float speed)
        {
            const int maxAmount = 50;
            int current = 0;
            bool godMode = player.IsGodModeEnabled;
            while (player.GameObject != null && player.Role != RoleTypeId.Spectator)
            {
                player.Position += Vector3.up * speed;
                current++;
                if (current >= maxAmount)
                {
                    player.IsGodModeEnabled = false;
                    var item = player.ReferenceHub.inventory.CreateItemInstance(new ItemIdentifier(ItemType.GrenadeHE, ItemSerialGenerator.GenerateNext()), false) as ThrowableItem;
                    TimeGrenade grenadeboom = (TimeGrenade)UnityEngine.Object.Instantiate(item.Projectile, player.Position, UnityEngine.Quaternion.identity);
                    grenadeboom._fuseTime = 0f;
                    grenadeboom.NetworkInfo = new PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
                    grenadeboom.PreviousOwner = new Footprint(player != null ? player.ReferenceHub : ReferenceHub.HostHub);
                    NetworkServer.Spawn(grenadeboom.gameObject);
                    grenadeboom.ServerActivate();
                    player.Kill("Went on a trip in their favorite rocket ship.");
                    player.IsGodModeEnabled = godMode;
                    yield break;
                }

                yield return Timing.WaitForOneFrame;
            }
        }


       


        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class spawnBot : ICommand
        {
            public string Command { get; } = "spawnbot";

            public string[] Aliases { get; } = new string[] { };

            public string Description { get; } = "Spawn a bot for testing certain code";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {


                Player player;

                if (Player.TryGet(sender, out player))
                {
                    

                    var newPlayer = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);
                    var fakeConnection = new FakeConnection(0);
                    var hubPlayer = newPlayer.GetComponent<ReferenceHub>();
                    NetworkServer.AddPlayerForConnection(fakeConnection, newPlayer);
                    hubPlayer.authManager.InstanceMode = CentralAuth.ClientInstanceMode.Unverified;
                    hubPlayer.roleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                    hubPlayer.transform.localScale = new UnityEngine.Vector3(0.1f, 1f, 1f);

                    try
                    {
                      //  hubPlayer.nicknameSync.SetNick($"{player.Nickname}'s bot");
                        
                        hubPlayer.TryOverridePosition(player.Position, player.Rotation);
                    }
                    catch (Exception) { }

                    Timing.CallDelayed(0.7f, () =>
                    {
                        hubPlayer.roleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.None, RoleSpawnFlags.None);
                     
                    });


                    try
                    {
                        hubPlayer.nicknameSync.SetNick("This is a bot");

                    }
                    catch (Exception) { }

                    Timing.CallDelayed(1f, () =>
                    {
                        hubPlayer.TryOverridePosition(player.Position, player.Rotation);
                        // hubPlayer.nicknameSync.Network_playerInfoToShow = PlayerInfoArea.CustomInfo;
                       // hubPlayer.nicknameSync.CustomPlayerInfo = $"<color=#00B7EB>{hubPlayer.nicknameSync.Network_displayName} \nNine-Tailed Fox Boom Boom Boy</color>";
                        //hubPlayer.nicknameSync.CustomPlayerInfo = $"<color=#C50000>{hubPlayer.nicknameSync.CombinedName}</color>" + "\n<color=#C50000>SCP-035</color>";

                    });




                    response = "set player scale";
                    return true;
                }
                else { 




                
                response = "failed";
                return false;
                }
            }
        }


        

        static bool coolDowned2 = true;
        /*
        [CommandHandler(typeof(ClientCommandHandler))]
        public class scp1025 : ICommand
        {
            public string Command { get; } = "scp1025";

            public string[] Aliases { get; } = new string[] { "1025", "scp-1025", "book" };

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
                          List<String> list = new List<String> {"CardiacArrest", "Decontaminating", "Invigorated", "Poisoned", "Scp207", "MovementBoost", "Scp1853", "AntiScp207", "DamageReduction", "Decontaminating", "CardiacArrest", "CardiacArrest", "Poisoned", "Exhausted", "Disabled", "Bleeding", "Amnesia Vision", "Asphyxiated" };
                        //StatusEffectBase effect = player.ReferenceHub.playerEffectsController.AllEffects.RandomItem();
                        string effect = list.RandomItem();
                        player.ReferenceHub.playerEffectsController.ChangeState(effect, 1, 0, false);
                       // ef
                          player.SendBroadcast($"You opened <color=#C50000>SCP-1025</color> and read the page {effect}.", 5);

                        // player.TemporaryData.Add("coolDown",player);


                        // ReferenceHub TempDummy = AddDummy();
                        // PlayPlayerAudio096(player, "page.ogg", (byte)85f, TempDummy);
                        Timing.CallDelayed(2f, () =>
                        {
                            // RemoveDummy096(TempDummy);
                        });


                    }
                    response = ".";
                    return true;
                }

                else
                {
                    response = "failed";
                    return false;
                }

            }
        }

        */


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
        // CREDIT TO KoT0XleB / RisottoMan.


        private static Config PluginConfig = Plugin.Singleton.Config;
        public static ReferenceHub AudioBot = new ReferenceHub();
        /*
        public static AudioPlayerBase PlayAudio(string audioFile, byte volume, bool loop)
        {
            if (AudioBot == null) AudioBot = AddDummy();

            StopAudio();

            var path = Path.Combine(PluginConfig.AudioDirectory, audioFile);

            AudioPlayerBase audioPlayer = AudioPlayerBase.Get(AudioBot);
            audioPlayer.Enqueue(path, -1);
            audioPlayer.LogDebug = false;
            audioPlayer.BroadcastChannel = VoiceChatChannel.Intercom;
            audioPlayer.Volume = 20f;
            audioPlayer.Loop = loop;
            audioPlayer.Play(0);
            return audioPlayer;
        }
        */
        public static AudioPlayerBase PlayAudio64(string audioFile, byte volume, bool loop, ReferenceHub AudioBotT)
        {
            //   if (AudioBot == null) AudioBot = AddDummy();


            var path = Path.Combine(PluginConfig.AudioDirectory, audioFile);


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

            var path = Path.Combine(PluginConfig.AudioDirectory, audioFile);

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

            var path = Path.Combine(PluginConfig.AudioDirectory, audioFile);

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


        public static AudioPlayerBase PlayPlayerAudio_096_HigherVolume(Player player, string audioFile, byte volume, ReferenceHub AudioBotT)
        {
            //if (AudioBot == null) AudioBot = AddDummy();

            StopAudio();

            var path = Path.Combine(PluginConfig.AudioDirectory, audioFile);

            AudioPlayerBase audioPlayer = AudioPlayerBase.Get(AudioBotT);
            audioPlayer.Enqueue(path, -1);
            audioPlayer.LogDebug = false;
            audioPlayer.BroadcastChannel = VoiceChatChannel.Proximity;
            audioPlayer.BroadcastTo.Add(player.PlayerId);
            audioPlayer.Volume = 15f;
            audioPlayer.Loop = false;
            audioPlayer.Play(0);
            return audioPlayer;
        }


        public static AudioPlayerBase PlayPlayerAudio096_Loop(Player player, string audioFile, byte volume, ReferenceHub AudioBotT)
        {
            //if (AudioBot == null) AudioBot = AddDummy();

            StopAudio();

            var path = Path.Combine(PluginConfig.AudioDirectory, audioFile);

            AudioPlayerBase audioPlayer = AudioPlayerBase.Get(AudioBotT);
            audioPlayer.Enqueue(path, -1);
            audioPlayer.LogDebug = false;
            audioPlayer.BroadcastChannel = VoiceChatChannel.Proximity;
            audioPlayer.BroadcastTo.Add(player.PlayerId);
            audioPlayer.Volume = 25f;
            audioPlayer.Loop = false;
            audioPlayer.Play(0);
            return audioPlayer;
        }

   


        public static AudioPlayerBase AddListener(Player player, string audioFile, byte volume, ReferenceHub AudioBotT)
        {
            //if (AudioBot == null) AudioBot = AddDummy();

            //StopAudio();

            var path = Path.Combine(PluginConfig.AudioDirectory, audioFile);

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

            var path = Path.Combine(PluginConfig.AudioDirectory, audioFile);

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


            var path = Path.Combine(PluginConfig.AudioDirectory, audioFile);

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
          //  hubPlayer.playerStats.GetModule<AdminFlagsStat>().SetFlag(AdminFlags.Noclip, true);

            hubPlayer.roleManager.ServerSetRole(RoleTypeId.Overwatch, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
           // hubPlayer.transform.localScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
            //hubPlayer.authManager.NetworkSyncedUserId = null;
            //hubPlayer.Network_playerId = null;
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
             //  hubPlayer.transform.localScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
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
          //  hubPlayer.transform.localScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
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
             //   hubPlayer.transform.localScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
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
