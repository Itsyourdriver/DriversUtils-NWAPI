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
    public class Heisenberg : CustomRole
    {
        public int Chance { get; set; } = 15;
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;
        public Team Team { get; set; } = Team.Scientists;
        public override uint Id { get; set; } = 4;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "Heisenberg";
        public override string Description { get; set; } = "You have more medical supplies than your allies.";
        public override string CustomInfo { get; set; } = "Heisenberg";
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
                Role = RoleTypeId.Scientist,
                Chance = 15,
            },
        },
        };
        public override List<string> Inventory { get; set; } = new()
        {
            $"{ItemType.SCP500}",
            $"{ItemType.Painkillers}",
            $"{ItemType.Medkit}",
            $"{ItemType.KeycardScientist}",
        };

    }
}
