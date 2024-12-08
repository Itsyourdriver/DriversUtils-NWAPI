namespace DriversUtils
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.CustomItems;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Events.Commands.Reload;
    using HarmonyLib;
    using System;
    using System.IO;

    // using RueI;
    public class Plugin : Plugin<Config>
    {
        public override string Author { get; } = "Itsyourdriver";

        public override string Name { get; } = "DriversUtils";

        private static readonly Plugin Singleton = new();
        public static string PluginPath { get; private set; }
        public static string TextPath { get; private set; }
        private EventHandler EventHandler;
        
        public static Plugin Instance => Singleton;

        //public override PluginPriority Priority { get; } = PluginPriority.Last;

        public override void OnEnabled()
        {

            if (!Config.IsEnabled)
                return;

            
            /*
            Log.Warn($"I correctly read the string config, its value is: {Config.String}");
            Log.Warn($"I correctly read the int config, its value is: {Config.Int}");
            Log.Warn($"I correctly read the float config, its value is: {Config.Float}");
            */
            PluginPath = Path.Combine(Paths.Configs, "DriversUtils");
            TextPath = Path.Combine(PluginPath, "Hints.txt");
            
            if (!Directory.Exists(PluginPath))
            {
                Directory.CreateDirectory(PluginPath);
                if (!File.Exists(TextPath))
                {
                    File.WriteAllText(TextPath, "Test \n Test2");
                }
                //File.Create(Path.Combine(PluginPath, "Hints.txt"));
                
            }
            RegisterEvents();
            // RueIMain.EnsureInit();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            EventHandler = new EventHandler();

            Exiled.Events.Handlers.Server.WaitingForPlayers += EventHandler.OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += EventHandler.OnRoundStarted;
            Exiled.Events.Handlers.Player.FlippingCoin += EventHandler.OnFlippingCoin;
            Exiled.Events.Handlers.Player.Verified += EventHandler.OnVerified;
            Exiled.Events.Handlers.Player.Dying += EventHandler.OnDying;
            Exiled.Events.Handlers.Player.Left += EventHandler.OnLeft;
            Exiled.Events.Handlers.Player.ChangedItem += EventHandler.OnEquippedItem;
            Exiled.Events.Handlers.Scp914.UpgradingPickup += EventHandler.OnUpgradingPickup;
            Exiled.Events.Handlers.Scp914.UpgradingInventoryItem += EventHandler.OnUpgradingInventoryItem;
            Exiled.Events.Handlers.Player.ChangingRole += EventHandler.OnChangeRole;
            Exiled.Events.Handlers.Server.RoundEnded += EventHandler.OnRoundEnded;
            Exiled.Events.Handlers.Server.ChoosingStartTeamQueue += EventHandler.OnChoostingStartTeamQueue;

            CustomItem.RegisterItems();
            CustomRole.RegisterRoles(true,null);
            /*
            Exiled.Events.Handlers.Player.Destroying += playerHandler.OnDestroying;
            Exiled.Events.Handlers.Player.Spawning += playerHandler.OnSpawning;
            Exiled.Events.Handlers.Player.Escaping += playerHandler.OnEscaping;
            Exiled.Events.Handlers.Player.Hurting += playerHandler.OnHurting;
            Exiled.Events.Handlers.Player.Dying += playerHandler.OnDying;
            Exiled.Events.Handlers.Player.Died += playerHandler.OnDied;
            Exiled.Events.Handlers.Player.ChangingRole += playerHandler.OnChangingRole;
            Exiled.Events.Handlers.Player.ChangingItem += playerHandler.OnChangingItem;
            Exiled.Events.Handlers.Player.UsingItem += playerHandler.OnUsingItem;
            Exiled.Events.Handlers.Player.PickingUpItem += playerHandler.OnPickingUpItem;
            Exiled.Events.Handlers.Player.DroppingItem += playerHandler.OnDroppingItem;
            Exiled.Events.Handlers.Player.Verified += playerHandler.OnVerified;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension += playerHandler.OnFailingEscapePocketDimension;
            Exiled.Events.Handlers.Player.EscapingPocketDimension += playerHandler.OnEscapingPocketDimension;
            Exiled.Events.Handlers.Player.UnlockingGenerator += playerHandler.OnUnlockingGenerator;
            Exiled.Events.Handlers.Player.PreAuthenticating += playerHandler.OnPreAuthenticating;
            Exiled.Events.Handlers.Player.Shooting += playerHandler.OnShooting;
            Exiled.Events.Handlers.Player.ReloadingWeapon += playerHandler.OnReloading;
            Exiled.Events.Handlers.Player.ReceivingEffect += playerHandler.OnReceivingEffect;

            Exiled.Events.Handlers.Warhead.Stopping += warheadHandler.OnStopping;
            Exiled.Events.Handlers.Warhead.Starting += warheadHandler.OnStarting;

            Exiled.Events.Handlers.Scp106.Teleporting += playerHandler.OnTeleporting;

            Exiled.Events.Handlers.Scp914.Activating += playerHandler.OnActivating;
            Exiled.Events.Handlers.Scp914.ChangingKnobSetting += playerHandler.OnChangingKnobSetting;
            Exiled.Events.Handlers.Scp914.UpgradingPlayer += playerHandler.OnUpgradingPlayer;

            Exiled.Events.Handlers.Map.ExplodingGrenade += mapHandler.OnExplodingGrenade;
            Exiled.Events.Handlers.Map.GeneratorActivating += mapHandler.OnGeneratorActivated;

            Exiled.Events.Handlers.Item.ChangingAmmo += itemHandler.OnChangingAmmo;
            Exiled.Events.Handlers.Item.ChangingAttachments += itemHandler.OnChangingAttachments;
            Exiled.Events.Handlers.Item.ReceivingPreference += itemHandler.OnReceivingPreference;

            Exiled.Events.Handlers.Scp914.UpgradingPickup += scp914Handler.OnUpgradingItem;

            Exiled.Events.Handlers.Scp096.AddingTarget += scp096Handler.OnAddingTarget;
            */
        }

        private void UnregisterEvents()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= EventHandler.OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandler.OnRoundStarted;
            Exiled.Events.Handlers.Player.FlippingCoin -= EventHandler.OnFlippingCoin;
            Exiled.Events.Handlers.Player.Verified -= EventHandler.OnVerified;
            Exiled.Events.Handlers.Player.Dying -= EventHandler.OnDying;
            Exiled.Events.Handlers.Player.Left -= EventHandler.OnLeft;
            Exiled.Events.Handlers.Player.ChangedItem -= EventHandler.OnEquippedItem;
            Exiled.Events.Handlers.Scp914.UpgradingPickup -= EventHandler.OnUpgradingPickup;
            Exiled.Events.Handlers.Scp914.UpgradingInventoryItem -= EventHandler.OnUpgradingInventoryItem;
            Exiled.Events.Handlers.Player.ChangingRole -= EventHandler.OnChangeRole;
            Exiled.Events.Handlers.Server.RoundEnded -= EventHandler.OnRoundEnded;
            Exiled.Events.Handlers.Server.ChoosingStartTeamQueue -= EventHandler.OnChoostingStartTeamQueue;
            /*
            Exiled.Events.Handlers.Player.Destroying -= playerHandler.OnDestroying;
            Exiled.Events.Handlers.Player.Dying -= playerHandler.OnDying;
            Exiled.Events.Handlers.Player.Died -= playerHandler.OnDied;
            Exiled.Events.Handlers.Player.ChangingRole -= playerHandler.OnChangingRole;
            Exiled.Events.Handlers.Player.ChangingItem -= playerHandler.OnChangingItem;
            Exiled.Events.Handlers.Player.PickingUpItem += playerHandler.OnPickingUpItem;
            Exiled.Events.Handlers.Player.Verified -= playerHandler.OnVerified;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= playerHandler.OnFailingEscapePocketDimension;
            Exiled.Events.Handlers.Player.EscapingPocketDimension -= playerHandler.OnEscapingPocketDimension;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= playerHandler.OnUnlockingGenerator;
            Exiled.Events.Handlers.Player.PreAuthenticating -= playerHandler.OnPreAuthenticating;

            Exiled.Events.Handlers.Warhead.Stopping -= warheadHandler.OnStopping;
            Exiled.Events.Handlers.Warhead.Starting -= warheadHandler.OnStarting;

            Exiled.Events.Handlers.Scp106.Teleporting -= playerHandler.OnTeleporting;

            Exiled.Events.Handlers.Scp914.Activating -= playerHandler.OnActivating;
            Exiled.Events.Handlers.Scp914.ChangingKnobSetting -= playerHandler.OnChangingKnobSetting;

            Exiled.Events.Handlers.Map.ExplodingGrenade -= mapHandler.OnExplodingGrenade;
            Exiled.Events.Handlers.Map.GeneratorActivating -= mapHandler.OnGeneratorActivated;

            Exiled.Events.Handlers.Item.ChangingAmmo -= itemHandler.OnChangingAmmo;
            Exiled.Events.Handlers.Item.ChangingAttachments -= itemHandler.OnChangingAttachments;
            Exiled.Events.Handlers.Item.ReceivingPreference -= itemHandler.OnReceivingPreference;

            Exiled.Events.Handlers.Scp914.UpgradingPickup -= scp914Handler.OnUpgradingItem;

            Exiled.Events.Handlers.Scp096.AddingTarget -= scp096Handler.OnAddingTarget;

            serverHandler = null;
            playerHandler = null;
            warheadHandler = null;
            mapHandler = null;
            itemHandler = null;
            scp914Handler = null;
            scp096Handler = null;
            */
            EventHandler = null;
        }
    }
}