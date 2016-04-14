using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Remote;

namespace ConDep.Dsl.Operations.Remote.Node
{
    internal class StartConDepNodeOperation : RemoteOperation
    {
        private readonly PowerShellExecutor _executor = new PowerShellExecutor();

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            _executor.Execute(server, "Start-ConDepNode", mod =>
            {
                mod.LoadConDepNodeModule = true;
                mod.LoadConDepModule = false;
            }, logOutput: false);

            return Result.SuccessChanged();
        }

        public override string Name => "Start ConDepNode";
    }
}