using Exiled.API.Features.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerRoles;
using Exiled.CustomRoles.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Scp330;
using InventorySystem;
using Exiled.Events.EventArgs.Server;
using CustomPlayerEffects;
using Interactables.Interobjects;
using MEC;
using Exiled.API.Features;
using UnityEngine;

namespace DriversUtils.Roles
{
    [CustomRole(RoleTypeId.ClassD)]
    public class TheKid : CustomRole
    {
        public int Chance { get; set; } = 15;
        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;
        public Team Team { get; set; } = Team.ClassD;
        public override uint Id { get; set; } = 1;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "The Kid";
        public override string Description { get; set; } = "You are slightly smaller and can take 3 candies.";
        public override string CustomInfo { get; set; } = "The Kid";
        public override bool KeepPositionOnSpawn { get; set; } = true;
        public override bool KeepInventoryOnSpawn { get; set; } = true;
        public override bool KeepRoleOnDeath { get; set; } = false;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
        {
            new()
            {
                Role = RoleTypeId.ClassD,
                Chance = 100,
            },
        },
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Scp330.InteractingScp330 += OnInteractingScp330;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Scp330.InteractingScp330 -= OnInteractingScp330;
            base.UnsubscribeEvents();
        }

        protected override void RoleAdded(Player player)
        {
            //Timing.CallDelayed(0.1f, () => player.Scale = new Vector3(0.75f, 0.75f, 0.75f));
        }

        protected override void RoleRemoved(Player player)
        {
            //player.Scale = Vector3.one;
        }

        private void OnInteractingScp330(InteractingScp330EventArgs ev)
        {
            if (ev.UsageCount == 2 && Check(ev.Player))
            {
                ev.ShouldPlaySound = true;
                ev.Player.Inventory.ServerAddItem(ItemType.SCP330, InventorySystem.Items.ItemAddReason.PickedUp);
                ev.Player.DisableEffect<SeveredHands>();
                ev.IsAllowed = false;
                //ev.Player.DisableEffect<SeveredHands>();
            }
            else
            {
                //return;
            }
        }
    }
}
