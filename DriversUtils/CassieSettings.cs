using MapGeneration;
using PlayerRoles;
using PluginAPI.Core.Zones.Heavy.Rooms;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Plugin
{
    public class CassieSettings
    {
        [Description("Cassie messages that have a chance to play after decontamination.")]
        public List<string> AfterDeconMessages { get; set; } = new List<String> { "ChaosInvasion", "PowerBlackout", "Foggy", "SpecialOps", "ArmedDClass", "EveryoneIsSmall", "Nextbots" };

        [Description("Invalue 1-100 - Rarity for a message above that is randomly selected to be played.")]
        public int EventRarity { get; set; } = 10;

        [Description("Whether or not CASSIE should announce when serpents hand/science team spawn.")]
        public bool ShouldCassie { get; set; } = true;

        [Description("Cassie announcement for serpent's hand.")]
        public string SerpentsMessage { get; set; } = "Attention All Personnel The Serpents .G5 Hand have entered the facility . All .G2 remaining personnel are to pitch_0.7 jam_056_4 .G1 .G2 .G4 pitch_1.2 NOSCPSLEFT .G6";

        [Description("Cassie announcement for science team.")]
        public string ScienceMessage { get; set; } = "Attention All Personnel The Serpents .G5 Hand have entered the facility . All .G2 remaining personnel are to pitch_0.7 jam_056_4 .G1 .G2 .G4 pitch_1.2 NOSCPSLEFT .G6";

        [Description("Whether or not to play the cassie bell start and bell end / cassie noise")]
        public bool CassieNoise { get; set; } = true;

        [Description("Whether or not CASSIE should display the text of the announcement. This is set to false because the default announcement uses NOSCPLEFT which looks weird in text.")]
        public bool CassieText { get; set; } = false;

    }
}