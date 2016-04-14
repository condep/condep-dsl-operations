using ConDep.Dsl.Builders;
using ConDep.Dsl.Operations.Remote.Node;

namespace ConDep.Dsl
{
    public static class NodeExtensions
    {
        public static IOfferRemoteOperations StartConDepNode(this IOfferRemoteOperations remote)
        {
            var op = new StartConDepNodeOperation();
            OperationExecutor.Execute((RemoteBuilder)remote, op);
            return remote;
        }

        public static IOfferRemoteOperations StopConDepNode(this IOfferRemoteOperations remote)
        {
            var op = new StopConDepNodeOperation();
            OperationExecutor.Execute((RemoteBuilder)remote, op);
            return remote;
        }
    }
}