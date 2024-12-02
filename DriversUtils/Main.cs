namespace DriversUtils
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem.Items.Usables;
    using RueI;

    public class Plugin : Plugin<Config>
    {
        public override string Author { get; } = "Itsyourdriver";

        public override string Name { get; } = "DriversUtils";

        private static readonly Plugin Singleton = new();

        private EventHandler EventHandler;

        public static Plugin Instance => Singleton;

        public override PluginPriority Priority { get; } = PluginPriority.Last;

        public override void OnEnabled()
        {
            RegisterEvents();

            Log.Warn($"I correctly read the string config, its value is: {Config.String}");
            Log.Warn($"I correctly read the int config, its value is: {Config.Int}");
            Log.Warn($"I correctly read the float config, its value is: {Config.Float}");
            RueIMain.EnsureInit();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            base.OnDisabled();
        }

        /// <summary>
        /// Registers the plugin events.
        /// </summary>
        private void RegisterEvents()
        {
           // EventHandler = new EventHandler();

            Exiled.Events.Handlers.Server.WaitingForPlayers += EventHandler.OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += EventHandler.OnRoundStarted;
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

        /// <summary>
        /// Unregisters the plugin events.
        /// </summary>
        private void UnregisterEvents()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= EventHandler.OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandler.OnRoundStarted;
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