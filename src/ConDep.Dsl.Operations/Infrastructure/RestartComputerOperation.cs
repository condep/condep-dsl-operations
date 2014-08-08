using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations;
using ConDep.Dsl.Remote;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl
{
    public class RestartComputerOperation : RemoteOperation
    {
        public override void Execute(ServerConfig server, IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            var powershellExecutor = new PowerShellExecutor(server);
            powershellExecutor.Execute("Restart-Computer");

            //Wait for computer to restart
            //Ping
            //WinRM test
            //Restart completed
        }

        public override string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        public override bool IsValid(Notification notification)
        {
            throw new System.NotImplementedException();
        }
    }

    public static class RestartComputerExtensions
    {
        public static IOfferRemoteOperations Restart(this IOfferRemoteOperations remote)
        {
            var restartOperation = new RestartComputerOperation();
            Configure.Remote(remote, restartOperation);
            return remote;
        }
    }
}