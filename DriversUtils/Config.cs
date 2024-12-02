using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using DriversUtils.Util;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;

using InventorySystem.Items.Firearms.Attachments;

using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DriversUtils
{



    public sealed class Config : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc />
        public bool Debug { get; set; }

        /// <summary>
        /// Gets the string config.
        /// </summary>
        [Description("This is a string config")]
        public string String { get; private set; } = "I'm a string!";

        /// <summary>
        /// Gets the int config.
        /// </summary>
        [Description("This is an int config")]
        public int Int { get; private set; } = 1000;

        /// <summary>
        /// Gets the float config.
        /// </summary>
        [Description("This is a float config")]
        public float Float { get; private set; } = 28.2f;

        /// <summary>
        /// Gets the list of strings config.
        /// </summary>
        [Description("This is a list of strings config")]
        public List<string> StringsList { get; private set; } = new() { "First element", "Second element", "Third element" };

        /// <summary>
        /// Gets the list of ints config.
        /// </summary>
        [Description("This is a list of ints config")]
        public List<int> IntsList { get; private set; } = new() { 1, 2, 3 };

        /// <summary>
        /// Gets the dictionary of string as key and int as value config.
        /// </summary>
        
        

        /// <summary>
        /// Gets the Vector3 config.
        /// </summary>
        [Description("This is a Vector3 config, the same can be done by using a Vector2 or a Vector4")]
        public Vector3 Vector3 { get; private set; } = new(1.3f, -2.5f, 3);

        /// <summary>
        /// Gets the <see cref="List{T}"/> of <see cref="AttachmentName"/> config.
        /// </summary>
        [Description("This is a list of AttachmentNameTranslation config")]
        public List<AttachmentName> Attachments { get; private set; } = new()
        {
            AttachmentName.AmmoCounter,
            AttachmentName.DotSight,
            AttachmentName.RifleBody,
            AttachmentName.RecoilReducingStock,
            AttachmentName.StandardMagAP,
        };

        [Description("The distance that the player can be from SCP-294 before they can use it.")]
        public float UseDistance { get; private set; } = 3f;

        [Description("This is a dictionary of strings as key and int as value config")]
        public Dictionary<string, int> StringIntDictionary { get; private set; } = new()
        {
            { "First Key", 1 },
            { "Second Key", 2 },
            { "Third Key", 3 },
        };


        public static List<DrinkType> DefaultDrinks { get; private set; } = new List<DrinkType>() {
        new DrinkType()
        {
             DrinkNames = new List<string>(){
                    "cola",
                    "scp-207",
                    "207",
                    "pepsi",
                    "bepis"
                },
             ItemModel = ItemType.SCP207,
             DrinkEffects = new List<EffectType>(){
                 EffectType.Scp207
             },
             Duration = 0f,
             Intensity = 1,
             Explosive = false,
             SpawnTantrum = false,
             DrinkMessage = "",
             EquipMessage = "",
             PickupMessage = "",
        }
        };
        /*
        [Description("This is a dictionary of strings as key and Dictionary<string, int> as value config")]
        public Dictionary<string, Dictionary<string, int>> Scp294Drinks { get; private set; } = new()
        {
            {
                "First Key", new Dictionary<ItemType, EffectType, float, int>()
                {
                    { "First Key", 1 },
                    { "Second Key", 2 },
                    { "Third Key", 3 },
                }
            },
            {
                "Second Key", new Dictionary<string, int>()
                {
                    { "First Key", 4 },
                    { "Second Key", 5 },
                    { "Third Key", 6 },
                }
            },
            {
                "Third Key", new Dictionary<string, int>()
                {
                    { "First Key", 7 },
                    { "Second Key", 8 },
                    { "Third Key", 9 },
                }
            },
        };
        */

        [Description("The rooms & approximate positions (local offset) that SCP-294 can spawn at on round start.")]
        public Dictionary<RoomType, Vector3> SpawnPoints { get; set; } = new();

    }
}