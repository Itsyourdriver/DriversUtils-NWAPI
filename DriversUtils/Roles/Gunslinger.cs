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
    [CustomRole(RoleTypeId.NtfPrivate)]
    public class Gunslinger : CustomRole
    {
        public int Chance { get; set; } = 25;
        public override RoleTypeId Role { get; set; } = RoleTypeId.NtfPrivate;
        public Team Team { get; set; } = Team.FoundationForces;
        public override uint Id { get; set; } = 3;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "Gunslinger";
        public override string Description { get; set; } = "You are equipped with a revolver.";
        public override string CustomInfo { get; set; } = "Gunslinger";
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
                Role = RoleTypeId.NtfPrivate,
                Chance = 25,
            },
        },
        };
        public override List<string> Inventory { get; set; } = new()
        {
            $"{ItemType.ArmorCombat}",
            $"{ItemType.GrenadeFlash}",
            $"{ItemType.Medkit}",
            $"{ItemType.GunRevolver}",
            $"{ItemType.GunFSP9}",
            $"{ItemType.Radio}",
            $"{ItemType.KeycardMTFOperative}",
            $"{ItemType.Ammo9x19}",
            $"{ItemType.Ammo9x19}",
            $"{ItemType.Ammo9x19}",
            $"{ItemType.Ammo9x19}",
            $"{ItemType.Ammo9x19}",
            $"{ItemType.Ammo44cal}",
            $"{ItemType.Ammo44cal}",
            $"{ItemType.Ammo44cal}",
        };

    }
}
