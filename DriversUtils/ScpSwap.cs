using CommandSystem;
using MEC;
using PlayerRoles;
using Plugin;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using RueI.Displays;
using RueI.Displays.Scheduling;
using RueI.Elements;
using RueI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace DriversUtils
{
    internal class ScpSwap
    {
        public static bool CanSwap = true;
        public float SwapWindow = 90f;


        [PluginEvent(ServerEventType.RoundStart)]
        void RoundStarted()
        {
            CanSwap = true;
            Timing.CallDelayed(SwapWindow, () =>
            {
                CanSwap = false;
            });
        }

        [PluginEvent(ServerEventType.PlayerChangeRole)]
        void PlayerChangeRolee(Player player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
        {
            if (CanSwap == true && newRole.GetTeam() == Team.SCPs)
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    player.SendBroadcast("<b><color=#00B7EB>Reminder: To Swap Scp Classes, type .scpswap (scp nickname/number) in your (~) console.</color> \n<color=#FAFF86>You can get a list of classes to swap to by running the command .scpswap list</color></b>", 10);
                });
            }
        }


        [CommandHandler(typeof(ClientCommandHandler))]
        public class scpswap : ICommand
        {
            public string Command { get; } = "scpswap";

            public string[] Aliases { get; } = new string[] {"swapscp", "swap" };

            public string Description { get; } = "Swap the SCP you're currently playing";

            public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player;
                if (Player.TryGet(sender, out player) && CanSwap == true  && (player.IsSCP || EventHandlers.scp035s.Contains(player.PlayerId)))
                {

                    if (arguments.First().ToLower() == "scp939" || arguments.First().ToLower() == "939" || arguments.First().ToLower() == "dog" || arguments.First().ToLower() == "scp-939")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.GetPlayers())
                        {
                            if (p != player)
                            {
                                if (p.Role == PlayerRoles.RoleTypeId.Scp939)
                                {
                                    response = "Could not swap. Someone is already playing as SCP-939. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            player.Role = PlayerRoles.RoleTypeId.Scp939;
                            response = "Swapped.";
                            return true;
                        }
                    }
                    else if (arguments.First().ToLower() == "049" || arguments.First().ToLower() == "scp049" || arguments.First().ToLower() == "doctor" || arguments.First().ToLower() == "scp-049" || arguments.First().ToLower() == "doc")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.GetPlayers())
                        {
                            if (p != player)
                            {
                                if (p.Role == PlayerRoles.RoleTypeId.Scp049)
                                {
                                    response = "Could not swap. Someone is already playing as SCP-049. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            player.Role = PlayerRoles.RoleTypeId.Scp049;
                            response = "Swapped.";
                            return true;
                        }

                    }
                    else if (arguments.First().ToLower() == "106" || arguments.First().ToLower() == "scp106" || arguments.First().ToLower() == "oldman" || arguments.First().ToLower() == "scp-106" || arguments.First().ToLower() == "larry")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.GetPlayers())
                        {
                            if (p != player)
                            {
                                if (p.Role == PlayerRoles.RoleTypeId.Scp106)
                                {
                                    response = "Could not swap. Someone is already playing as SCP-106. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            player.Role = PlayerRoles.RoleTypeId.Scp106;
                            response = "Swapped.";
                            return true;
                        }

                    }
                    else if (arguments.First().ToLower() == "3114" || arguments.First().ToLower() == "scp3114" || arguments.First().ToLower() == "skeleton" || arguments.First().ToLower() == "scp-3114" || arguments.First().ToLower() == "skele")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.GetPlayers())
                        {
                            if (p != player)
                            {
                                if (p.Role == PlayerRoles.RoleTypeId.Scp3114)
                                {
                                    response = "Could not swap. Someone is already playing as SCP-3114. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            player.Role = PlayerRoles.RoleTypeId.Scp3114;
                            response = "Swapped.";
                            return true;
                        }

                    }
                    else if (arguments.First() == "173" || arguments.First().ToLower() == "scp173" || arguments.First().ToLower() == "peanut" || arguments.First().ToLower() == "scp-173" || arguments.First().ToLower() == "matthew")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.GetPlayers())
                        {
                            if (p != player)
                            {
                                if (p.Role == PlayerRoles.RoleTypeId.Scp173)
                                {
                                    response = "Could not swap. Someone is already playing as SCP-173. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            player.Role = PlayerRoles.RoleTypeId.Scp173;
                            response = "Swapped.";
                            return true;
                        }

                    }
                    else if (arguments.First().ToLower() == "096" || arguments.First().ToLower() == "scp096" || arguments.First().ToLower() == "shyguy" || arguments.First().ToLower() == "scp-096" || arguments.First().ToLower() == "shy guy")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.GetPlayers())
                        {
                            if (p != player)
                            {
                                if (p.Role == PlayerRoles.RoleTypeId.Scp096)
                                {
                                    response = "Could not swap. Someone is already playing as SCP-096. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            player.Role = PlayerRoles.RoleTypeId.Scp096;
                            response = "Swapped.";
                            return true;
                        }

                    }
                    else if (arguments.First().ToLower() == "079" || arguments.First().ToLower() == "scp079" || arguments.First().ToLower() == "computer" || arguments.First().ToLower() == "scp-079" || arguments.First().ToLower() == "pc")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.GetPlayers())
                        {
                            if (p != player)
                            {
                                if (p.Role == PlayerRoles.RoleTypeId.Scp079)
                                {
                                    response = "Could not swap. Someone is already playing as SCP-079. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            player.Role = PlayerRoles.RoleTypeId.Scp079;
                            response = "Swapped.";
                            return true;
                        }

                    }
                    else if (arguments.First().ToLower() == "scp-049-3" || arguments.First().ToLower() == "boss" || arguments.First().ToLower() == "theboss" || arguments.First().ToLower() == "bosszombie")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.GetPlayers())
                        {
                            if (p != player)
                            {
                                if (p.Role == PlayerRoles.RoleTypeId.Scp0492 && EventHandlers.thebosszombies.Contains(p.PlayerId))
                                {
                                    response = "Could not swap. Someone is already playing as SCP-035. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            EventHandlers.ChangeToTheBoss(player, true);
                            response = "Swapped.";
                            return true;
                        }

                    }
                    /*
                    else if (arguments.First().ToLower() == "035" || arguments.First().ToLower() == "scp035" || arguments.First().ToLower() == "mask" || arguments.First().ToLower() == "scp-035")
                    {
                        bool ExistingPlayer = false;
                        foreach (var p in Player.GetPlayers())
                        {
                            if (p != player)
                            {
                                if (p.Role == PlayerRoles.RoleTypeId.Tutorial && EventHandlers.scp035s.Contains(p.PlayerId))
                                {
                                    response = "Could not swap. Someone is already playing as SCP-035. Ask them to swap to a different SCP.";
                                    ExistingPlayer = true;
                                    return false;

                                }

                            }
                        }
                        if (ExistingPlayer == false)
                        {
                            EventHandlers.ChangeTo035(player, true);
                            response = "Swapped.";
                            return true;
                        }

                    }

                    */

                    else if (arguments.First().ToLower() == "list" || arguments.First().ToLower() == "help" || arguments.First().ToLower() == "roles" || arguments.First().ToLower() == "classes")
                    {

                        response = "List of scps: \n- doctor \n- computer \n- shyguy \n- larry \n- peanut \n- dog \n\nDO NOT SWAP TO: \n- TheBoss";
                        return true;



                    }
                }
                response = "Command Execution Failed!";
                return false;
            }
        }

    }
}
