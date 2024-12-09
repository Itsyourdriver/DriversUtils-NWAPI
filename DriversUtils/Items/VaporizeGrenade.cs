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
using Utils;
using Exiled.API.Features;
using CustomPlayerEffects;

namespace DriversUtils.Items
{
    [CustomItem(ItemType.GrenadeHE)]
    public class VaporizeGrenade : CustomGrenade
    {
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 3f;
        public override uint Id { get; set; } = 6;
        public override string Name { get; set; } = "Particle Grenade";
        public override string Description { get; set; } = "A grenade that vaporizes targets.";
        public override float Weight { get; set; } = 2f;
        public override SpawnProperties? SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
        {
            new()
            {
                Chance = 50,
                Location = SpawnLocationType.Inside049Armory,
            },
        },
        };

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
            //ExplosionUtils.ServerExplode(ev.Player.ReferenceHub, ExplosionType.Grenade); -- keeping this for something else lol
            foreach (Player plr in ev.TargetsToAffect)
            {
                if (!plr.IsScp && !plr.IsGodModeEnabled && plr.ReferenceHub.playerEffectsController.TryGetEffect<AntiScp207>(out var effect))
                {
                    plr.Vaporize();
                }
            }

        }


    }
}
