using System.Threading;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Builders
{
    public class SslInfrastructureBuilder : RemoteBuilder, IOfferSslInfrastructure
    {
        public SslInfrastructureBuilder(IOfferRemoteConfiguration remoteConf, ServerConfig server, ConDepSettings settings, CancellationToken token) : base(server, settings, token)
        {
            InfrastructureBuilder = remoteConf;
        }

        public IOfferRemoteConfiguration InfrastructureBuilder { get; }
    }
}