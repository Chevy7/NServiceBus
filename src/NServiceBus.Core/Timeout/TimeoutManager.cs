﻿namespace NServiceBus.Features
{
    using Config;
    using Timeout.Core;
    using Timeout.Hosting.Windows;
    using Transports;

    /// <summary>
    /// This feature provides message deferral based on a external timeout manager.
    /// </summary>
    public class TimeoutManager : Feature
    {
        public override bool IsEnabledByDefault
        {
            get
            {
                return true;
            }
        }

        public override bool ShouldBeEnabled(Configure config)
        {
            //has the user already specified a custom deferral method
            if (config.Configurer.HasComponent<IDeferMessages>())
            {
                return false;
            }

            //if we have a master node configured we should use the Master Node timeout manager instead
            if (config.Settings.GetOrDefault<bool>("Distributor.Enabled"))
            {
                return false;
            }

            var unicastConfig = config.GetConfigSection<UnicastBusConfig>();

            //if the user has specified another TM we don't need to run our own
            if (unicastConfig != null && !string.IsNullOrWhiteSpace(unicastConfig.TimeoutManagerAddress))
            {
                return false;
            }
            
            return true;
        }

        public override void Initialize(Configure config)
        {
            DispatcherAddress = Address.Parse(Configure.EndpointName).SubScope("TimeoutsDispatcher");
            InputAddress = Address.Parse(Configure.EndpointName).SubScope("Timeouts");

            config.Configurer.ConfigureComponent<TimeoutPersisterReceiver>(DependencyLifecycle.SingleInstance);
            config.Configurer.ConfigureComponent<DefaultTimeoutManager>(DependencyLifecycle.SingleInstance);
        }

        public static Address InputAddress { get; private set; }
        public static Address DispatcherAddress { get; private set; }
    }
}