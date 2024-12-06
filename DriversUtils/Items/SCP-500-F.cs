using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using HintServiceMeow.UI.Utilities;
using MEC;
using Exiled.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriversUtils.Items
{
    [CustomItem(ItemType.SCP500)]
    public class SCP500F : CustomItem
    {
        public override uint Id { get; set; } = 5;
        public override string Name { get; set; } = "SCP-500-F";
        public override string Description { get; set; } = "When consumed, you will be given a friend.";
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
                foreach (Player p in Player.List)
                {
                    if (p.Role.Type == PlayerRoles.RoleTypeId.Spectator)
                    {
                      p.RoleManager.ServerSetRole(ev.Player.Role.Type, PlayerRoles.RoleChangeReason.RemoteAdmin, PlayerRoles.RoleSpawnFlags.AssignInventory);
                      var ui = PlayerUI.Get(p);
                      ui.CommonHint.ShowOtherHint($"{ev.Player.Nickname} has respawned you using SCP-500-F.", 3f);
                    }
                    break;
                }
            });
        }
    }
}
