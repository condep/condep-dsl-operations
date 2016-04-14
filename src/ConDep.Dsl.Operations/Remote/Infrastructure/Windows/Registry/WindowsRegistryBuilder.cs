using System.Threading;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry
{
    internal class WindowsRegistryBuilder : RemoteBuilder, IOfferWindowsRegistryOperations
    {
        public WindowsRegistryBuilder(IOfferRemoteConfiguration remoteConfig, ServerConfig server, ConDepSettings settings, CancellationToken token) : base(server, settings, token)
        {
            RemoteConfigurationBuilder = remoteConfig;
        }

        public IOfferRemoteConfiguration RemoteConfigurationBuilder { get; }
    }
}