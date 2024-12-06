using Exiled.API.Extensions;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DriversUtils.Items
{
    namespace DriversUtils.Items
    {
        [CustomItem(ItemType.SCP500)]
        public class SCP500T : CustomItem
        {
            public override uint Id { get; set; } = 4;
            public override string Name { get; set; } = "SCP-500-T";
            public override string Description { get; set; } = "When consumed, you will be teleported to an entirely random room.";
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
                    ev.Player.Position = Facility.Rooms.GetRandomValue().Position + new Vector3(0f,1.5f,0f);
                });
            }
        }
    }
}
