using InventorySystem.Items.Pickups;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using Scp914;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InventorySystem.Items.Coin;
using InventorySystem.Items.Usables;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Radio;
using Mirror;
using PluginAPI.Core;
using InventorySystem.Items;
using PluginAPI.Core.Items;
using InventorySystem;

namespace Plugin
{
    public class Coin914
    {


        public List<ItemType> list = new List<ItemType> {ItemType.Snowball, ItemType.Adrenaline, ItemType.AntiSCP207, ItemType.SCP500, ItemType.SCP018, ItemType.SCP1576, ItemType.SCP1853, ItemType.SCP268, ItemType.SpecialCoal, ItemType.Coal, ItemType.ArmorCombat, ItemType.ArmorHeavy, ItemType.ArmorLight, ItemType.Flashlight, ItemType.KeycardContainmentEngineer, ItemType.GunCOM15, ItemType.GunCOM18, ItemType.GrenadeFlash, ItemType.KeycardChaosInsurgency, ItemType.KeycardJanitor, ItemType.KeycardScientist, ItemType.KeycardResearchCoordinator, ItemType.GunLogicer, ItemType.Medkit, ItemType.Painkillers, ItemType.GunFSP9, ItemType.Tape, ItemType.SCP244b, ItemType.SCP244a, ItemType.KeycardZoneManager, ItemType.KeycardGuard, ItemType.Lantern, ItemType.Radio, ItemType.GrenadeHE, ItemType.SCP207};
        System.Random random = new System.Random();
        [PluginEvent(ServerEventType.Scp914UpgradeInventory)]
        public void OnScp914UpgradeInventory(Player player, ItemBase item, Scp914KnobSetting setting)
        {
            if (item.ItemTypeId == ItemType.Coin)
            {
                ItemType tempitem2 = list.RandomItem();
                switch (setting)
                {
                    case Scp914KnobSetting.Rough:
                    //    player.RemoveItem(new Item(item));
                    //    player.AddItem(tempitem2);
                   //     player.ReceiveHint("Your coin was upgraded into a random item...", 3);
                   //     Log.Info("upgraded.");
                        return;
                    case Scp914KnobSetting.Coarse:
                    //    player.RemoveItem(new Item(item));
                    //    player.AddItem(tempitem2);
                     //   player.ReceiveHint("Your coin was upgraded into a random item...", 3);
                       // Log.Info("upgraded.");
                        return;
                    case Scp914KnobSetting.OneToOne:
                        return;
                    case Scp914KnobSetting.Fine:
               //                 player.RemoveItem(new Item(item));
             //           player.AddItem(tempitem2);
             //           player.ReceiveHint("Your coin was upgraded into a random item...", 3);
              //                   Log.Info("upgraded.");
                         return;
                    case Scp914KnobSetting.VeryFine:
                        player.RemoveItem(new Item(item));
                        player.AddItem(tempitem2);
                        player.ReceiveHint("Your coin was upgraded into a random item...", 3);
                        Log.Info("upgraded.");
                        return;
                }
            }
        }

        [PluginEvent(ServerEventType.Scp914UpgradePickup)]
        public void OnScp914UpgradePickup(ItemPickupBase item, Vector3 position, Scp914KnobSetting setting)
        {
            if (item.Info.ItemId == ItemType.Coin)
            {
                switch (setting)
                {
                    case Scp914KnobSetting.Rough:
                    case Scp914KnobSetting.Coarse:
                    case Scp914KnobSetting.OneToOne:
                        return;
                    case Scp914KnobSetting.Fine:
                        return;
                    case Scp914KnobSetting.VeryFine:

                        Quaternion rot = item.transform.rotation;
                        ItemType tempitem1 = list.RandomItem();
                        if (InventoryItemLoader.TryGetItem(tempitem1, out ItemBase items)) {
                            item.DestroySelf();
                            InventoryExtensions.ServerCreatePickup(items, new PickupSyncInfo(tempitem1, 1.0f), position, rot);
                        }
                return;
            }
            }
        }
    }
}