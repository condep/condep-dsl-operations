using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Remote.Node;

namespace ConDep.Dsl.Operations.Remote.Deployment.CopyFile
{
    public class CopyFileOperation : RemoteOperation
    {
        private readonly string _srcFile;
        private readonly string _dstFile;
        private Api _api;

        public CopyFileOperation(string srcFile, string dstFile)
        {
            _srcFile = srcFile;
            _dstFile = dstFile;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            _api = new Api(new ConDepNodeUrl(server), server.DeploymentUser.UserName, server.DeploymentUser.Password, server.Node.TimeoutInSeconds.Value * 1000);
            var result = _api.SyncFile(_srcFile, _dstFile);

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

        public override string Name => "Copy File";
    }
}