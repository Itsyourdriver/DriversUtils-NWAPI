using HarmonyLib;
using PlayerStatsSystem;
using Plugin;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriversUtils;
using System.Diagnostics.Eventing.Reader;
using YamlDotNet.Core.Tokens;
// Code is from CustomSpawn
namespace DriversUtils.Patches
{
    [HarmonyPatch(typeof(HealthStat))]
    public static class HealthStatPatch
    {
        [HarmonyPatch(nameof(HealthStat.MaxValue), MethodType.Getter)] // thanks GBN#1862 for your help !
        public static bool Prefix(ref float __result, HealthStat __instance)
        {
            if (__instance?.Hub == null || Player.Get(__instance.Hub) == null || !EventHandlers.scp035s.Contains(Player.Get(__instance.Hub).PlayerId))
                return true;

            __result = 500;
            return false;
            /*
            if (__instance?.Hub == null || Player.Get(__instance.Hub) == null || !EventHandlers.scp035s.Contains(Player.Get(__instance.Hub).PlayerId) || !EventHandlers.thebosszombies.Contains(Player.Get(__instance.Hub).PlayerId))
                return true;

            if (EventHandlers.thebosszombies.Contains(Player.Get(__instance.Hub).PlayerId)) { __result = 1200f; return false; } else { __result = 500f; return false; }
           // __result = 500;
          //  return false;
            */
        }
    }
}
