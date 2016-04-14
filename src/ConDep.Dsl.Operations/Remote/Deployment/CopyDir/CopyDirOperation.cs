using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Remote.Node;

namespace ConDep.Dsl.Operations.Remote.Deployment.CopyDir
{
    public class CopyDirOperation : RemoteOperation
    {
        private readonly string _srcDir;
        private readonly string _dstDir;
        private Api _api;

        public CopyDirOperation(string srcDir, string dstDir)
        {
            _srcDir = srcDir;
            _dstDir = dstDir;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            _api = new Api(new ConDepNodeUrl(server), server.DeploymentUser.UserName, server.DeploymentUser.Password, server.Node.TimeoutInSeconds.Value * 1000);
            var result = _api.SyncDir(_srcDir, _dstDir);

            if (result == null) return Result.SuccessUnChanged();

            if (result.Log.Count > 0)
            {
                foreach (var entry in result.Log)
                {
                    Logger.Info(entry);
                }
            }
            else
            {
                Logger.Info("Nothing to deploy. Everything is in sync.");
                return Result.SuccessUnChanged();
            }

            return Result.SuccessChanged();
        }

        public override string Name => "Copy Dir";
    }
}