using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Map;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using UnityEngine;
using Utils;
using Random = System.Random;
using CustomPlayerEffects;
using PlayerRoles.Spectating;
using HintServiceMeow.Core.Utilities;
using HintServiceMeow.Core.Enum;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;
using Exiled.Events.EventArgs.Player;
using HintServiceMeow.Core.Extension;
using HintServiceMeow.UI.Utilities;
using Exiled.API.Features.Toys;
using AdminToys;
using System.IO;
using Exiled.CustomItems.API.Features;
using Exiled.CustomItems;
using Exiled.Events.EventArgs.Scp914;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using Exiled.API.Features.Roles;

namespace DriversUtils
{

    internal sealed class EventHandler
    {
        private static MapEditorObject _scp294;
        private CoroutineHandle _mainCourtine;
        private CoroutineHandle _hintCourtine;
        private CoroutineHandle hintCourntine;
        private static Dictionary<Player, int> PlayerKills = new Dictionary<Player, int>();


        // UI/Spectator Menu Things

        // by Michal78900. I will make my own implementation in the future due to this plugin being purpose built for my own server
        string hintsPath = Path.Combine(Plugin.PluginPath, "Hints.txt");
        List<string> server_hints = new();
        public int HintIndex = 0;

        /// end

        // 914 Item List
        public List<ItemType> SafeItemList = new List<ItemType> { ItemType.GunE11SR, ItemType.GunCrossvec, ItemType.Adrenaline, ItemType.AntiSCP207, ItemType.SCP500, ItemType.SCP018, ItemType.SCP1576, ItemType.SCP1853, ItemType.SCP268, ItemType.ArmorCombat, ItemType.ArmorHeavy, ItemType.ArmorLight, ItemType.Flashlight, ItemType.KeycardContainmentEngineer, ItemType.GunCOM15, ItemType.GunCOM18, ItemType.GrenadeFlash, ItemType.KeycardChaosInsurgency, ItemType.KeycardJanitor, ItemType.KeycardScientist, ItemType.KeycardResearchCoordinator, ItemType.GunLogicer, ItemType.Medkit, ItemType.Painkillers, ItemType.GunFSP9, ItemType.SCP244b, ItemType.SCP244a, ItemType.KeycardZoneManager, ItemType.KeycardGuard, ItemType.Lantern, ItemType.Radio, ItemType.GrenadeHE, ItemType.SCP207, ItemType.Jailbird, ItemType.KeycardO5, ItemType.KeycardFacilityManager, ItemType.KeycardChaosInsurgency, ItemType.GunFRMG0, ItemType.SCP244a };

        // Scpswap
        public static bool CanSwap = true;
        public float SwapWindow = 90f;


        // OnStart stuff
        public void OnWaitingForPlayers()
        {
            if (File.Exists(hintsPath))
                server_hints.AddRange(File.ReadAllLines(hintsPath));

            /*
            Room room = Room.Get(GetRandomRoom());
            Vector3 spawnPoint = Plugin.Instance.Config.SpawnPoints[room.Type];


            _scp294 = ObjectSpawner.SpawnSchematic("scp294", room.WorldPosition(spawnPoint), Quaternion.identity, Vector3.one, data: null);
            */
                // MER Spawning is broken, random room function doesn't work either but I'd rather re-write that myself so that's what i'll do on a later day
        }
        
        public void OnRoundStarted()
        {
            Log.Info($"A round has started with {Player.Dictionary.Count} players!");

            foreach (Player p in Player.List)
            {
                if (PlayerKills.TryGetValue(p, out int test))
                {
                    PlayerKills[p] = 0;
               
                }
                else
                {
                    PlayerKills.Add(p, 0);
                }
            }

            if (_mainCourtine.IsRunning)
                Timing.KillCoroutines(_mainCourtine);

            _mainCourtine = Timing.RunCoroutine(MainLoop());

            if (_hintCourtine.IsRunning)
                Timing.KillCoroutines(_hintCourtine);

            _hintCourtine = Timing.RunCoroutine(UpdateHintsIndex());

            CanSwap = true;
            Timing.CallDelayed(SwapWindow, () =>
            {
                CanSwap = false;
            });
        }
        

        public void OnVerified(VerifiedEventArgs ev)
        {
            PlayerKills.Add(ev.Player, 0);
            /*
            var ui = PlayerUI.Get(ev.Player);
            PlayerDisplay playerDisplay = PlayerDisplay.Get(ev.Player);

            Hint Hint1 = new Hint
            {
                Text = $"<color={ev.Player.ReferenceHub.roleManager.CurrentRole.RoleColor.ToHex()}><align=left><b><size=75%>        🔪 | 0 </size></b></align></color>",
                YCoordinate = 1030,
                SyncSpeed = HintSyncSpeed.Fast,
            };

            Hint Hint2 = new Hint
            {
                Text = $"<color={ev.Player.ReferenceHub.roleManager.CurrentRole.RoleColor.ToHex()}><align=left><b><size=75%>                    👥 | {ev.Player.CurrentSpectatingPlayers.Count()} </size></b></align></color>",
                YCoordinate = 1030,
                SyncSpeed = HintSyncSpeed.Fast,
            };

            playerDisplay.AddHint(Hint1);
            playerDisplay.AddHint(Hint2);
            */
        }

        // Courntines + Misc Util Functions
        // temp, code by Nexus, will make my own handler for this in the future once plugin is done
        private RoomType GetRandomRoom()
        {

            Random random = new Random();

            List<RoomType> roomNames = Plugin.Instance.Config.SpawnPoints.Keys.ToList();

            int index = random.Next(roomNames.Count);
            return roomNames[index];
        }


        // by Michal78900. I will make my own implementation in the future due to this plugin being purpose built for my own server
        private void IncrementHintIndex()
        {
            HintIndex++;
            if (server_hints.Count == HintIndex)
                HintIndex = 0;

           
        }

        private IEnumerator<float> MainLoop()
        {

            while (!RoundSummary.singleton._roundEnded)
            {
                yield return Timing.WaitForSeconds(1f);
                try
                {
                    if (Round.IsStarted)
                    {
                        foreach (Player player in Player.List)
                        {
                            if (player.IsConnected)
                            {
                                var ui = PlayerUI.Get(player);
                                
                                PlayerDisplay playerDisplay = PlayerDisplay.Get(player);
                                
                                Hint Hint1 = new Hint
                                {
                                    Text = $"<color={player.ReferenceHub.roleManager.CurrentRole.RoleColor.ToHex()}><align=left><b><size=75%>        🔪 | {PlayerKills[player]} </size></b></align></color>",
                                    YCoordinate = 1030,
                                };

                                Hint Hint2 = new Hint
                                {
                                    Text = $"<color={player.ReferenceHub.roleManager.CurrentRole.RoleColor.ToHex()}><align=left><b><size=75%>                    👥 | {player.CurrentSpectatingPlayers.Count()} </size></b></align></color>",
                                    YCoordinate = 1030,
                                    SyncSpeed = HintSyncSpeed.Fast,
                                };

                                playerDisplay.AddHint(Hint1);
                                playerDisplay.AddHint(Hint2);
                                Hint1.HideAfter(1.01f);
                                Hint2.HideAfter(1.01f);
                                playerDisplay.RemoveAfter(Hint1, 1.02f);
                                playerDisplay.RemoveAfter(Hint2, 1.02f);


                                if (player.Role == RoleTypeId.Spectator)
                                {
                                    ui.CommonHint.ShowMapHint($"{server_hints[HintIndex]}", 1f);
                                    
                                }
                                if (_scp294 == null && Vector3.Distance(player.Position, _scp294.Position) <= Plugin.Instance.Config.UseDistance)
                                {

                                    //PlayerDisplayCore.SetElemTemp("You can use SCP-294. To use it, open your console (~) and type .vm (drink).\nYou can get a list of drinks by running the command .vm list", 300f, TimeSpan.FromSeconds(1), PlayerGlobalItemElem);
                                    ui.CommonHint.ShowItemHint($"You can use SCP-294. To use it, open your console (~) and type .vm (drink).\nYou can get a list of drinks by running the command .vm list", 1f);
                                }
                                
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (Plugin.Instance.Config.Debug == true)
                    {
                        Log.Debug($"Error: {ex}");
                    }
                }
            }
        }

        private IEnumerator<float> UpdateHintsIndex()
        {

            while (!RoundSummary.singleton._roundEnded)
            {
                yield return Timing.WaitForSeconds(10f);
                try
                {
                    if (Round.IsStarted)
                    {
                        IncrementHintIndex();
                    }
                }
                catch (Exception ex)
                {
                    if (Plugin.Instance.Config.Debug == true)
                    {
                        Log.Debug($"Error: {ex}");
                    }
                }
            }
        }




        // Item-related events

        public void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            RoleTypeId PlayerCurrentRole = ev.Player.Role.Type;
           
            Timing.CallDelayed(1.4f, () =>
            {
                var ui = PlayerUI.Get(ev.Player);
                if (ev.Player.Role == PlayerCurrentRole) {
                    if (ev.IsTails)
                    {

                        ui.CommonHint.ShowItemHint($"The coin landed on Tails!", 2f);
                    }
                    else
                    {
                        ui.CommonHint.ShowItemHint($"The coin landed on Heads!", 2f);
                    }
                }
            });
        }

        public void OnEquippedItem(ChangedItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.SCP500)
            {
                if (UnityEngine.Random.Range(1,15) >= 3)
                {
                  
                }
            }
        }

        public void OnUpgradingPickup(UpgradingPickupEventArgs ev)
        {
            if (ev.Pickup != null && ev.Pickup.Type == ItemType.Coin)
            {
                Pickup.CreateAndSpawn(SafeItemList.RandomItem(), ev.OutputPosition, ev.Pickup.Rotation);
                ev.Pickup.Destroy();
            }
        }

        public void OnUpgradingInventoryItem(UpgradingInventoryItemEventArgs ev)
        {
            if (ev.Item != null && ev.Item.Type == ItemType.Coin)
            { 
                ev.Player.AddItem(SafeItemList.RandomItem());
                ev.Player.RemoveItem(ev.Item,true);
            }
        }

        // Map related events


        // Player related events

        public void OnDying(DyingEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                var ui = PlayerUI.Get(ev.Attacker);
                PlayerKills[ev.Attacker]++;
                ui.CommonHint.ShowItemHint($"You killed {ev.Player.Nickname}. You now have {PlayerKills[ev.Attacker]} kills this round.", 3.5f);
            }
        }

        public void OnLeft(LeftEventArgs ev)
        {
            PlayerKills.Remove(ev.Player);
        }

        public void OnChangeRole(ChangingRoleEventArgs ev)
        {
            if (ev.NewRole.GetTeam() == Team.SCPs)
            {
                var ui = PlayerUI.Get(ev.Player);
                ui.CommonHint.ShowMapHint($"Reminder: To Swap Scp Classes, type .scpswap (scp nickname/number) in your (~) console.</color> \n<color=#FAFF86>You can get a list of classes to swap to by running the command .scpswap list</color></b>", 15f);
            }
        }

        // Scpswap

        [CommandHandler(typeof(ClientCommandHandler))]
        public class scpswap : ICommand
        {
            public string Command { get; } = "scpswap";

            public string[] Aliases { get; } = new string[] { "swapscp", "swap", "scp" };

            public string Description { get; } = "Swap the SCP you're currently playing";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player = Player.Get(sender);

                if (player != null)
                {
                    if (arguments.First().ToLower() == "list" || arguments.First().ToLower() == "help" || arguments.First().ToLower() == "roles" || arguments.First().ToLower() == "classes")
                    {
                        response = "List of scps: \n- doctor \n- computer \n- shyguy \n- larry \n- peanut \n- dog";
                        return true;
                    }
                }

                if (player != null && CanSwap == true && player.IsScp == true)
                {

                    if (arguments.First().ToLower() == "scp939" || arguments.First().ToLower() == "939" || arguments.First().ToLower() == "dog" || arguments.First().ToLower() == "scp-939")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.List)
                        {
                            if (p != player)
                            {
                                if (p.Role == RoleTypeId.Scp939)
                                {
                                    response = "Could not swap. Someone is already playing as SCP-939. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            player.RoleManager.ServerSetRole(RoleTypeId.Scp939, RoleChangeReason.RemoteAdmin);
                            response = "Swapped.";
                            return true;
                        }
                    }
                    else if (arguments.First().ToLower() == "049" || arguments.First().ToLower() == "scp049" || arguments.First().ToLower() == "doctor" || arguments.First().ToLower() == "scp-049" || arguments.First().ToLower() == "doc")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.List)
                        {
                            if (p != player)
                            {
                                if (p.Role == RoleTypeId.Scp049)
                                {
                                    response = "Could not swap. Someone is already playing as SCP-049. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            player.RoleManager.ServerSetRole(RoleTypeId.Scp049, RoleChangeReason.RemoteAdmin);
                            response = "Swapped.";
                            return true;
                        }

                    }
                    else if (arguments.First().ToLower() == "106" || arguments.First().ToLower() == "scp106" || arguments.First().ToLower() == "oldman" || arguments.First().ToLower() == "scp-106" || arguments.First().ToLower() == "larry")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.List)
                        {
                            if (p != player)
                            {
                                if (p.Role == RoleTypeId.Scp106)
                                {
                                    response = "Could not swap. Someone is already playing as SCP-106. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            player.RoleManager.ServerSetRole(RoleTypeId.Scp106, RoleChangeReason.RemoteAdmin);
                            response = "Swapped.";
                            return true;
                        }

                    }
                    else if (arguments.First().ToLower() == "3114" || arguments.First().ToLower() == "scp3114" || arguments.First().ToLower() == "skeleton" || arguments.First().ToLower() == "scp-3114" || arguments.First().ToLower() == "skele")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.List)
                        {
                            if (p != player)
                            {
                                if (p.Role == RoleTypeId.Scp3114)
                                {
                                    response = "Could not swap. Someone is already playing as SCP-3114. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            player.RoleManager.ServerSetRole(RoleTypeId.Scp3114, RoleChangeReason.RemoteAdmin);
                            response = "Swapped.";
                            return true;
                        }

                    }
                    else if (arguments.First() == "173" || arguments.First().ToLower() == "scp173" || arguments.First().ToLower() == "peanut" || arguments.First().ToLower() == "scp-173" || arguments.First().ToLower() == "matthew")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.List)
                        {
                            if (p != player)
                            {
                                if (p.Role == RoleTypeId.Scp173)
                                {
                                    response = "Could not swap. Someone is already playing as SCP-173. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            player.RoleManager.ServerSetRole(RoleTypeId.Scp173, RoleChangeReason.RemoteAdmin);
                            response = "Swapped.";
                            return true;
                        }

                    }
                    else if (arguments.First().ToLower() == "096" || arguments.First().ToLower() == "scp096" || arguments.First().ToLower() == "shyguy" || arguments.First().ToLower() == "scp-096" || arguments.First().ToLower() == "shy guy")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.List)
                        {
                            if (p != player)
                            {
                                if (p.Role == RoleTypeId.Scp096)
                                {
                                    response = "Could not swap. Someone is already playing as SCP-096. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            player.RoleManager.ServerSetRole(RoleTypeId.Scp096, RoleChangeReason.RemoteAdmin);
                            response = "Swapped.";
                            return true;
                        }

                    }
                    else if (arguments.First().ToLower() == "079" || arguments.First().ToLower() == "scp079" || arguments.First().ToLower() == "computer" || arguments.First().ToLower() == "scp-079" || arguments.First().ToLower() == "pc")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.List)
                        {
                            if (p != player)
                            {
                                if (p.Role == RoleTypeId.Scp079)
                                {
                                    response = "Could not swap. Someone is already playing as SCP-079. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            player.RoleManager.ServerSetRole(RoleTypeId.Scp079, RoleChangeReason.RemoteAdmin);
                            response = "Swapped.";
                            return true;
                        }

                    }
                }
                response = "<color=red>Command Execution Failed!</color>";
                return false;
            }
        }

    


    // Commands

    [CommandHandler(typeof(ClientCommandHandler))]
        public class Scp294 : ICommand
        {
            public string Command { get; } = "scp294";

            public string[] Aliases { get; } = new[] { "vm","294","drink" };

            public string Description { get; } = "Primary command for SCP-294.";
             
            /// <inheritdoc/>
            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player = Player.Get(sender);
                var ui = PlayerUI.Get(player);

                if (player != null)
                {
                    if (arguments.First().ToLower() == "list")
                    {

                    }
                    if (player.CurrentItem.Type != ItemType.Coin)
                    {
                        response = $"<color=red>You aren't holding a coin.</color>";
                        return false;
                    }
                    if (arguments.First().ToLower() == "cola" || arguments.First().ToLower() == "anticola")
                    {
                        player.RemoveItem(player.CurrentItem);
                        ui.CommonHint.ShowOtherHint($"You inserted a coin into SCP-294. The machine dispensed an {arguments.First()}", 3f);
                        player.AddItem(ItemType.SCP207);
                        response = $"<color=green>You are holding a coin. Continuing.</color>";
                        player.EnableEffect<SoundtrackMute>(255, 120, false);
                        return true;
                    }

                    if (_scp294 == null)
                    {
                        response = $"<color=red>Schematic not found! This is a bug, and should be reported to driver.</color>";
                        return false;
                    }

                    if (Vector3.Distance(player.Position, _scp294.Position) >= Plugin.Instance.Config.UseDistance)
                    {
                        response = $"<color=red>You are not close enough to use SCP-294.</color>";
                        return false;
                    }
                   
                }

                Log.Warn($"{player.Items.Count} -- {player.Inventory.UserInventory.Items.Count}");

                foreach (Player item in Player.List)
                    Log.Warn(item);

                foreach (Pickup pickup in Pickup.List)
                    Log.Warn($"{pickup.Type} ({pickup.Serial}) -- {pickup.Position}");

                foreach (PocketDimensionTeleport teleport in Map.PocketDimensionTeleports)
                    Log.Warn($"{teleport._type}");

                player.ClearInventory();
                response = $"{player.Nickname} sent the command!";

                // Return true if the command was executed successfully; otherwise, false.
                return true;
            }
        }


    }
}


[CommandHandler(typeof(ClientCommandHandler))]
public class NicknameCommand : ICommand
{
    public string Command { get; } = "nickname";

    public string[] Aliases { get; } = new string[] { "nick", "setnick", "setnickname", "setname" };

    public string Description { get; } = "Don't abuse this, thanks!";

    public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player player = Player.Get(sender);
        if (player != null && arguments.Count != 0)
        {
            if (arguments.First().ToLower() == "reset")
            {
                player.DisplayNickname = null;
                response = "Reset your nickname.";
                return true;
            }
            else
            {
                player.DisplayNickname = String.Join(" ", arguments);
                response = $"Set your nickname to: {String.Join(" ", arguments)}";
                return true;
            }
        }
        else
        {
            response = "<color=red>There was an error while running this command.</color>";
            return false;
        }
    }
}

[CommandHandler(typeof(ClientCommandHandler))]
public class KillCommand : ICommand
{
    public string Command { get; } = "kill";

    public string[] Aliases { get; } = new string[] { "die" };

    public string Description { get; } = "Don't abuse this, thanks!";

    public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player player = Player.Get(sender);
        if (player != null)
        {
           player.Kill("Suicide");
           response = $"<color=white>Goodbye!</color>";
           return true;
        }
        else
        {
            response = "<color=red>There was an error while running this command.</color>";
            return false;
        }
    }
}

[CommandHandler(typeof(ClientCommandHandler))]
public class RoundStatsCommand : ICommand
{
    public string Command { get; } = "roundstats";

    public string[] Aliases { get; } = new string[] { "rs","rstats" };

    public string Description { get; } = "Gives you some stats about the current round.";

    public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player player = Player.Get(sender);
        if (player != null)
        {
            
            response = $"<color=white>Round Stats:\nTime: {Round.ElapsedTime}\nStarted At: {Round.StartedTime}\nTPS: {Server.Tps}\nRound Count: {Round.UptimeRounds}\nVersion: {Server.Version}\nPlayers: {Server.PlayerCount}</color>";
            return true;
        }
        else
        {
            response = "<color=red>There was an error while running this command.</color>";
            return false;
        }
    }
}