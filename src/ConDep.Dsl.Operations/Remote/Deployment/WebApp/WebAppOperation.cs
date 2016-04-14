using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Remote.Node;

namespace ConDep.Dsl.Operations.Remote.Deployment.WebApp
{
    public class WebAppOperation : RemoteOperation
    {
        private readonly string _sourceDir;
        private readonly string _webAppName;
        private readonly string _destinationWebSiteName;
        private readonly string _destDir;
        private Api _api;

        public WebAppOperation(string sourceDir, string webAppName, string destinationWebSiteName, string destDir = null)
        {
            _sourceDir = sourceDir;
            _webAppName = webAppName;
            _destinationWebSiteName = destinationWebSiteName;
            _destDir = destDir;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            _api = new Api(new ConDepNodeUrl(server), server.DeploymentUser.UserName, server.DeploymentUser.Password, server.Node.TimeoutInSeconds.Value * 1000);
            var result = _api.SyncWebApp(_destinationWebSiteName, _webAppName, _sourceDir, _destDir);

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

        public override string Name => "Web Application";
    }
}