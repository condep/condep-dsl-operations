using System.Threading;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Builders
{
    public class RemoteCertDeploymentBuilder : RemoteBuilder, IOfferRemoteCertDeployment
    {
        public RemoteCertDeploymentBuilder(IOfferRemoteDeployment remoteDeployment, ServerConfig server, ConDepSettings settings, CancellationToken token) : base(server, settings, token)
        {
            RemoteDeployment = remoteDeployment;
        }

        public IOfferRemoteDeployment RemoteDeployment { get; }
    }
}