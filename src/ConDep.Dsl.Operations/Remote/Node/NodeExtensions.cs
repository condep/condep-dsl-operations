using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Remote;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Node
{
    public static class NodeExtensions
    {
        public static IOfferRemoteOperations StartConDepNode(this IOfferRemoteOperations remote)
        {
            var op = new StartConDepNodeOperation();
            Configure.Operation(remote, op);
            return remote;
        }

        public static IOfferRemoteOperations StopConDepNode(this IOfferRemoteOperations remote)
        {
            var op = new StopConDepNodeOperation();
            Configure.Operation(remote, op);
            return remote;
        } 
    }

    internal class StartConDepNodeOperation : ForEachServerOperation
    {
        private readonly PowerShellExecutor _executor = new PowerShellExecutor();

        public override void Execute(ServerConfig server, IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            _executor.Execute(server, "Start-ConDepNode", mod =>
            {
                mod.LoadConDepNodeModule = true;
                mod.LoadConDepModule = false;
            }, logOutput: false);
        }

        public override string Name
        {
            get { return "Start ConDepNode"; }
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }
    }

    internal class StopConDepNodeOperation : ForEachServerOperation
    {
        private readonly PowerShellExecutor _executor = new PowerShellExecutor();

        public override void Execute(ServerConfig server, IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            _executor.Execute(server, "Stop-ConDepNode", mod =>
            {
                mod.LoadConDepNodeModule = true;
                mod.LoadConDepModule = false;
            }, logOutput: false);
        }

        public override string Name
        {
            get { return "Stop ConDepNode"; }
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }
    }
}