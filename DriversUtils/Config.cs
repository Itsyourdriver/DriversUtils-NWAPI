using MapGeneration;
using PlayerRoles;
using PluginAPI.Core.Zones.Heavy.Rooms;
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

        [Description("Whether or not CASSIE should announce when serpents hand spawn.")]
        public bool ShouldCassie { get; set; } = true;

        [Description("Cassie announcement when serpents hand spawn. Only valid cassie words will work")]
        public string CassieMessage { get; set; } = "Attention All Personnel The Serpents .G5 Hand have been detected inside the facility . All .G2 remaining personnel are to pitch_0.7 jam_056_4 .G1 .G2 .G4 pitch_1.2 NOSCPSLEFT .G6";

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

        [Description("Should Serpent's Hand spawn more than once?")]
        public bool ShouldSerpentsHandSpawnMore { get; set; } = false;

        [Description("Broadcast text to all new serpents hand agents upon spawn")]
        public string SerpentsHandText { get; set; } = "You have been spawned as a <color=#FF96DE>Serpent's Hand</color> agent. Follow Orders. Work with the SCPs. Terminate other classes.";

        [Description("Broadcast text to all new serpents hand agents upon spawn")]
        public string SerpentsHandCaptainText { get; set; } = "You have been spawned as a <color=#FF96DE>Serpent's Hand</color> Captain. Command your unit. Work with the SCPs. Terminate other classes.";

        [Description("Should a player be picked as a guard captain upon spawn?")]
        public bool ShouldGuardCaptainsSpawn { get; set; } = true;

        [Description("Broadcast text to the selected guard captain.")]
        public string GuardText { get; set; } = "You are the <color=#50C878>Guard Captain.</color> Command Your Unit. Check your inventory.";

        [Description("Broadcast text to the selected kid class")]
        public string KidText { get; set; } = "You are the kid. You can take 3 candy instead of 2.";

        [Description("Should we load item commands? Not implemented yet.")]
        public bool CommandsEnabled { get; set; } = true;




    }
}
