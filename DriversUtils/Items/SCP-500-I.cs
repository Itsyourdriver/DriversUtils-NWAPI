using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Footprinting;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using CustomPlayerEffects;

namespace DriversUtils.Items
{
    [CustomItem(ItemType.SCP500)]
    public class SCP500I : CustomItem
    {
        public override uint Id { get; set; } = 3;
        public override string Name { get; set; } = "SCP-500-I";
        public override string Description { get; set; } = "When consumed, you will be granted invisibility for 10 seconds.";
        public override float Weight { get; set; } = 1f;
        public override SpawnProperties? SpawnProperties { get; set; }
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;
            base.UnsubscribeEvents();
        }

        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            Timing.CallDelayed(1f, () =>
            {
                ev.Player.EnableEffect<Invisible>(10f, true);
            });
        }
    }
}