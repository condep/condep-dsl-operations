using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Infrastructure.IIS.WebApp;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.IIS.WebApp
{
    public class IisWebAppOperation : RemoteOperation
    {
        private readonly string _webAppName;
        private readonly string _webSiteName;
        private readonly IisWebAppOptions.IisWebAppOptionsValues _options;

        public IisWebAppOperation(string webAppName, string webSiteName)
        {
            _webAppName = webAppName;
            _webSiteName = webSiteName;
        }

        public IisWebAppOperation(string webAppName, string webSiteName, IisWebAppOptions.IisWebAppOptionsValues options)
        {
            _webAppName = webAppName;
            _webSiteName = webSiteName;
            _options = options;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            return remote.Execute.PowerShell(string.Format(@"New-ConDepWebApp '{0}' '{1}' {2} {3};"
                , _webAppName
                , _webSiteName
                , (_options == null || string.IsNullOrWhiteSpace(_options.PhysicalPath)) ? "$null" : "'" + _options.PhysicalPath + "'"
                , (_options == null || string.IsNullOrWhiteSpace(_options.AppPool)) ? "$null" : "'" + _options.AppPool + "'")).Result;
        }

        public override string Name
        {
            get { return "Web Application - " + _webSiteName + " - " + _webAppName; }
        }
    }
}