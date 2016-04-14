using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Remote;

namespace ConDep.Dsl.Operations.Remote.Node
{
    internal class StopConDepNodeOperation : RemoteOperation
    {
        private readonly PowerShellExecutor _executor = new PowerShellExecutor();

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            _executor.Execute(server, "Stop-ConDepNode", mod =>
            {
                mod.LoadConDepNodeModule = true;
                mod.LoadConDepModule = false;
            }, logOutput: false);

            return Result.SuccessChanged();
        }

        public override string Name => "Stop ConDepNode";
    }
}