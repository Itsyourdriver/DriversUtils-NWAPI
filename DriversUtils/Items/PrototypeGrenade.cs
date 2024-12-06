using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Footprinting;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items;
using MapEditorReborn.Commands.ModifyingCommands.Position;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriversUtils.Items
{
    [CustomItem(ItemType.GrenadeHE)]
    public class PrototypeGrenade : CustomGrenade
    {
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 3f;
        public override uint Id { get; set; } = 1;
        public override string Name { get; set; } = "2x Prototype Grenade";
        public override string Description { get; set; } = "A grenade that explodes twice.";
        public override float Weight { get; set; } = 1.15f;
        public override SpawnProperties? SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
        {
            new()
            {
                Chance = 50,
                Location = SpawnLocationType.InsideHczArmory,
            },
        },
        };

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            // old method but works i think
            var nade = ev.Player.ReferenceHub.inventory.CreateItemInstance(new ItemIdentifier(ItemType.GrenadeHE, ItemSerialGenerator.GenerateNext()), false) as ThrowableItem;
            TimeGrenade grenadeboom = (TimeGrenade)UnityEngine.Object.Instantiate(nade.Projectile, ev.Position, UnityEngine.Quaternion.identity);
            grenadeboom._fuseTime = 1f;
            grenadeboom.NetworkInfo = new PickupSyncInfo(nade.ItemTypeId, nade.Weight, nade.ItemSerial);
            grenadeboom.PreviousOwner = new Footprint(ev.Player.ReferenceHub ?? ReferenceHub.HostHub);
            NetworkServer.Spawn(grenadeboom.gameObject);
            grenadeboom.ServerActivate();
            ev.IsAllowed = true;
        }


    }
}
