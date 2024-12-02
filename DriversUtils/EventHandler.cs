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
using RueI.Displays.Scheduling;
using RueI.Displays;
using RueI.Elements;
using UnityEngine;
using Utils;
using Random = System.Random;
using RueI.Extensions;
using CustomPlayerEffects;
using PlayerRoles.Spectating;


namespace DriversUtils
{

    internal sealed class EventHandler
    {
        private static MapEditorObject _scp294;
        private CoroutineHandle _mainCourtine;
        public void OnWaitingForPlayers()
        { 
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
            if (_mainCourtine.IsRunning)
                Timing.KillCoroutines(_mainCourtine);

            _mainCourtine = Timing.RunCoroutine(MainLoop());
        }



        private IEnumerator<float> MainLoop()
        {
            TimedElemRef<SetElement> PlayerGlobalItemElem = new TimedElemRef<SetElement>();
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
                                DisplayCore PlayerDisplayCore = DisplayCore.Get(player.ReferenceHub);

                                PlayerDisplayCore.SetElemTemp($"<color={player.ReferenceHub.roleManager.CurrentRole.RoleColor.ToHex()}><align=left><b><size=75%>        🔪 | null </size></b></align></color>", 15f, TimeSpan.FromSeconds(1), new TimedElemRef<SetElement>()); //{PlayerKills[player]}
                                PlayerDisplayCore.SetElemTemp($"<color={player.ReferenceHub.roleManager.CurrentRole.RoleColor.ToHex()}><align=left><b><size=75%>                    👥 | {player.CurrentSpectatingPlayers.Count()} </size></b></align></color>", 15f, TimeSpan.FromSeconds(1), new TimedElemRef<SetElement>());//{PlayerSpectators[player]}
                                if (_scp294 == null && Vector3.Distance(player.Position, _scp294.Position) <= Plugin.Instance.Config.UseDistance)
                                {
                                   
                                    PlayerDisplayCore.SetElemTemp("You can use SCP-294. To use it, open your console (~) and type .vm (drink).\nYou can get a list of drinks by running the command .vm list", 300f, TimeSpan.FromSeconds(1), PlayerGlobalItemElem);
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




        // temp, code by Nexus, will make my own handler for this in the future once plugin is done
        private RoomType GetRandomRoom()
        {

            Random random = new Random();

            List<RoomType> roomNames = Plugin.Instance.Config.SpawnPoints.Keys.ToList();

            int index = random.Next(roomNames.Count);
            return roomNames[index];
        }

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

                    
                    if (arguments.First().ToLower() == "cola" || arguments.First().ToLower() == "anticola")
                    {
                        player.RemoveItem(player.CurrentItem);
                        DisplayCore.Get(player.ReferenceHub).SetElemTemp("You inserted a coin into SCP-294 and got out an", 350f, TimeSpan.FromSeconds(3), new TimedElemRef<SetElement>());
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

