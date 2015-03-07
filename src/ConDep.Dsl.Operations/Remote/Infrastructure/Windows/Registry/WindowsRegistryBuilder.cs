namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry
{
    internal class WindowsRegistryBuilder : IOfferWindowsRegistryOperations
    {
        private readonly IOfferRemoteConfiguration _conf;

        public WindowsRegistryBuilder(IOfferRemoteConfiguration conf)
        {
            _conf = conf;
        }

        public IOfferRemoteConfiguration RemoteConfigurationBuilder { get { return _conf; } }
    }
}