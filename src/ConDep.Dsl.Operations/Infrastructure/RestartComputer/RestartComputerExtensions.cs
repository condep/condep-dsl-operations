using ConDep.Dsl.Operations.Infrastructure.RestartComputer;

namespace ConDep.Dsl
{
    public static class RestartComputerExtensions
    {
        public static IOfferRemoteOperations Restart(this IOfferRemoteOperations remote, int delayInSeconds = 10)
        {
            var restartOperation = new RestartComputerOperation(delayInSeconds);
            Configure.Remote(remote, restartOperation);
            return remote;
        }
    }
}