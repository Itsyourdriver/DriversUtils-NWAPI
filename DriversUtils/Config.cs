using MapGeneration;
using PlayerRoles;
using PluginAPI.Core.Zones.Heavy.Rooms;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Plugin
{
    public class Config
    {
        [Description("If you get YAML errors use a YAML validator; Will the plugin be enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Debug (parts for now) of the plugin incase certain events arent firing.")]
        public bool Debug { get; set; } = false;

        [Description("Display strings. Format: Role, display string.")]
        public Dictionary<RoleTypeId, string> DisplayStrings { get; set; } = new Dictionary<RoleTypeId, string>()
        {
            { RoleTypeId.Scp106, "<color=#D51D1D>SCP-106 [%healthpercent%%] Vigor: %106vigor% %distance%</color>" },
            { RoleTypeId.Scp049, "<color=#D51D1D>SCP-049 [%healthpercent%% Zombies: %zombies%] %distance%</color>" },
            { RoleTypeId.Scp079, "<color=#D51D1D>SCP-079 [%generators%%engaging%/3] Lvl: %079level% Exp: %079experience% Energy: %079energy%</color>" },
            { RoleTypeId.Scp096, "<color=#D51D1D>SCP-096 [%healthpercent%%] %distance%</color>" },
            { RoleTypeId.Scp173, "<color=#D51D1D>SCP-173 [%healthpercent%%] %distance%</color>" },
            { RoleTypeId.Scp939, "<color=#D51D1D>SCP-939 [%healthpercent%%] %distance%</color>" },
            { RoleTypeId.Scp0492, "<color=#D51D1D>SCP-049-2 [%healthpercent%%] %distance%</color>" },
            { RoleTypeId.Scp3114, "<color=#D51D1D>SCP-3114 [%healthpercent%%] %distance%</color>" },
            { RoleTypeId.Tutorial, "<color=#D51D1D>Serpent's Hand Agent [%health%%] %distance%</color>" },
        };
        public List<string> Events { get; set; } =  new List<String> { "ChaosInvasion", "Foggy", "SpecialOps", "ArmedDClass", "EveryoneIsSmall", "Nextbots", "FriendlyFire", "ClearDay", "TestingDay" };

        [Description("Whether or not CASSIE should announce when serpents hand spawn.")]
        public bool ShouldCassie { get; set; } = true;

        [Description("Cassie announcement when serpents hand spawn. Only valid cassie words will work")]
        public string CassieMessage { get; set; } = "Attention All Personnel The Serpents .G5 Hand have entered the facility . All .G2 remaining personnel are to pitch_0.7 jam_056_4 .G1 .G2 .G4 pitch_1.2 NOSCPSLEFT .G6";

        [Description("Cassie announcement when flamingos spawn. Only valid cassie words will work. ")]
        public string FlamingoCassieMessage { get; set; } = "Attention All Personnel a new group of SCP 1 5 0 7 .G5 has been detected . All remaining .g6 . are to report to jam_45_3 .g4 .g5 .g2 . pitch_1.2 NOSCPSLEFT .G6";

        [Description("DO NOT ENABLE, COMPLETELY BROKEN")]
        public bool CanFlamingosSpawn { get; set; } = false;

        [Description("Whether or not to play the cassie bell start and bell end / cassie noise")]
        public bool CassieNoise { get; set; } = true;

        [Description("Whether or not CASSIE should display the text of the announcement. This is set to false because the default announcement uses NOSCPLEFT which looks weird in text.")]
        public bool CassieText { get; set; } = false;

        [Description("Should Serpent's Hand spawn?")]
        public bool ShouldSerpentsSpawn { get; set; } = true;

        [Description("Should The Science Team's spawn?")]
        public bool ShouldScienceTeamSpawn { get; set; } = true;

        [Description("Should Serpent's Hand spawn more than once?")]
        public bool ShouldSerpentsHandSpawnMore { get; set; } = false;

        [Description("Broadcast text to all new serpents hand agents upon spawn")]
        public string SerpentsHandText { get; set; } = "You have been spawned as a <color=#FF96DE>Serpent's Hand</color> agent. Work with the SCPs. Terminate other classes.";

        [Description("Broadcast text to all new serpents hand agents upon spawn")]
        public string SerpentsHandCaptainText { get; set; } = "You have been spawned as a <color=#FF96DE>Serpent's Hand</color> Captain. Command your unit. Work with the SCPs. Terminate other classes.";

        [Description("Should a player be picked as a guard captain upon spawn?")]
        public bool ShouldGuardCaptainsSpawn { get; set; } = true;

        [Description("CAN MTF Nu-7 Spawn in instead of epsillon 11?")]
        public bool CanNu7Spawn { get; set; } = true;

        [Description("Broadcast text to the selected guard captain.")]
        public string GuardText { get; set; } = "<color=#50C878>Guard Captain.</color> Command Your Unit. Check your inventory.";

        [Description("Broadcast text to the selected kid class")]
        public string KidText { get; set; } = "You are the kid. You start with candy and are shorter than everyone else!";

        [Description("Text sent to players who find themselves being the last alive.")]
        public string LastOneAliveHint { get; set; } = "<b><color=red>You are the last one alive!</color></b>";

        [Description("Should we load item commands? Not implemented yet.")]
        public bool CommandsEnabled { get; set; } = true;

        [Description("Invalue - Rarity of events out of 100.")]
        public int EventRarity { get; set; } = 100;

        [Description("UI Hint Testing (Dev-Only)")]
        public string KillsHint { get; set; } = "<b><align=left><pos=150>hi</pos></align></b>";

        [Description("Hint that's shown when you see scp-096")]
        public string targetmessage { get; set; } = "<b><color=red>You have become a target for SCP-096!</color></b>";

        [Description("Directory For Audio Files")]
        public string AudioDirectory { get; set; } = "";



    }
}
