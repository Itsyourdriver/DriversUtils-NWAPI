using Exiled.API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriversUtils.Util
{
    public class DrinkType
    {
        /// <summary>
        /// List of aliases / names for the drink.
        /// </summary>
        public List<string> DrinkNames { get; set; } = new List<string>(){
            ""
        };
        /// <summary>
        /// ItemBase. Use SCP207 or AntiSCP207
        /// </summary>
        public ItemType ItemModel { get; set; } = ItemType.SCP207;
        /// <summary>
        /// What effect to give the player.
        /// </summary>
        public List<EffectType> DrinkEffects { get; set; } = new List<EffectType>(){
           
        };
        /// <summary>
        /// Duration of the effect.
        /// </summary>
        public float Duration { get; set; } = 15f;
        /// <summary>
        /// Intensity of the effect.
        /// </summary>
        public int Intensity { get; set; } = 10;
        /// <summary>
        /// Explodes the player after consumption.
        /// </summary>
        public bool Explosive { get; set; } = false;
        /// <summary>
        /// Spawns peanut tantrum upon being consumed.
        /// </summary>
        public bool SpawnTantrum { get; set; } = false;
        /// <summary>
        /// Hint to show to the player when they drink it.
        /// </summary>
        public string DrinkMessage { get; set; } = "You drank the drink.";
        /// <summary>
        /// Hint to show to the player when they equip it.
        /// </summary>
        public string EquipMessage { get; set; } = "You equipped the drink.";
        /// <summary>
        /// Hint to show to the player when they pick it up.
        /// </summary>
        public string PickupMessage { get; set; } = "You picked up the drink.";

    }
}
