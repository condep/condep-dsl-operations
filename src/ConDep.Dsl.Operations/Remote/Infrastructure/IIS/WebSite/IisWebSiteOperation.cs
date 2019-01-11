using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Infrastructure.IIS.WebSite;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.IIS.WebSite
{
    public class IisWebSiteOperation : RemoteOperation
    {
        private readonly string _webSiteName;
        private readonly int _id;
        private readonly IisWebSiteOptions _options;

        public IisWebSiteOperation(string webSiteName, int id)
        {
            _webSiteName = webSiteName;
            _id = id;
            _options = new IisWebSiteOptions();
        }

        public IisWebSiteOperation(string webSiteName, int id, IisWebSiteOptions options)
        {
            _webSiteName = webSiteName;
            _id = id;
            _options = options;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            var bindings = _options.Values.HttpBindings.Select(httpBinding => $"@{{protocol='http';bindingInformation='{httpBinding.Ip}:{httpBinding.Port}:{httpBinding.HostName}';sslFlags=0}}").ToList();

            foreach (var httpsBinding in _options.Values.HttpsBindings)
            {
                if (httpsBinding.FindType == X509FindType.FindByThumbprint)
                {
                    httpsBinding.FindName = httpsBinding.FindName.Replace(" ", "");
                }
                var type = httpsBinding.FindType.GetType();
                var sslFlags = httpsBinding.BindingOptions.RequireSNI ? "1" : "0";
                bindings.Add($"@{{protocol='https';bindingInformation='{httpsBinding.BindingOptions.Ip}:{httpsBinding.BindingOptions.Port}:{httpsBinding.BindingOptions.HostName}';sslFlags={sslFlags};findType=[{type.FullName}]::{httpsBinding.FindType};findValue='{httpsBinding.FindName}'}}");
            }

            remote.Execute.PowerShell($@"New-ConDepIisWebSite '{_webSiteName}' {_id} {"@(" + string.Join(",", bindings) + ")"} {
                    (string.IsNullOrWhiteSpace(_options.Values.PhysicalPath)
                        ? "$null"
                        : "'" + _options.Values.PhysicalPath + "'")} '{_options.Values.AppPool}' '{
                    _options.Values.LogDirectory}';");

            foreach (var webApp in _options.Values.WebApps)
            {
                remote.Configure.IISWebApp(webApp.Item1, _webSiteName, webApp.Item2);
            }
            return Result.SuccessChanged();
        }

        public override string Name => "IIS Web Site - " + _webSiteName;
    }
}