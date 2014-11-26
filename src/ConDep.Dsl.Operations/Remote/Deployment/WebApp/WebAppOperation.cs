using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Remote.Node;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Application.Deployment.WebApp
{
    public class WebAppOperation : ForEachServerOperation
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

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override void Execute(ServerConfig server, IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            _api = new Api(new ConDepNodeUrl(server, settings), server.DeploymentUser.UserName, server.DeploymentUser.Password, server.Node.TimeoutInSeconds.Value * 1000);
            var result = _api.SyncWebApp(_destinationWebSiteName, _webAppName, _sourceDir, _destDir);

            if (result == null) return;

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
            }
        }

        public override string Name { get { return "Web Application"; } }
        public void DryRun()
        {
            Logger.WithLogSection(Name, () => { });
        }
    }
}