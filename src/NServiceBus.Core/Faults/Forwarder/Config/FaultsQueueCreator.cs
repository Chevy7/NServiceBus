namespace NServiceBus.Faults.Forwarder.Config
{
    using Unicast.Queuing;

    class FaultsQueueCreator : IWantQueueCreated
    {
        public Address Address
        {
            get { return ConfigureFaultsForwarder.ErrorQueue; }
        }

        public bool ShouldCreateQueue(Configure config)
        {
            return ConfigureFaultsForwarder.ErrorQueue != null;
        }
    }
}
