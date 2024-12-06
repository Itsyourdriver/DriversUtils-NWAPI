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
using Exiled.API.Enums;


namespace DriversUtils.Roles
{
    [CustomRole(RoleTypeId.Scientist)]
    public class Demoknight : CustomRole
    {
        public int Chance { get; set; } = 1;
        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;
        public Team Team { get; set; } = Team.ClassD;
        public override uint Id { get; set; } = 4;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "Demoknight";
        public override string Description { get; set; } = "Check your inventory.";
        public override string CustomInfo { get; set; } = "Demoknight";
        public override bool KeepPositionOnSpawn { get; set; } = true;
        //public override bool KeepInventoryOnSpawn { get; set; } = false;
        public override bool KeepRoleOnDeath { get; set; } = false;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
        {
            new()
            {
                Role = RoleTypeId.ClassD,
                Chance = 1,
            },
        },
        };
        public override List<string> Inventory { get; set; } = new()
        {
            $"{ItemType.Jailbird}",
            $"{ItemType.ArmorHeavy}",
            $"{ItemType.Painkillers}",
        };

    }
}
