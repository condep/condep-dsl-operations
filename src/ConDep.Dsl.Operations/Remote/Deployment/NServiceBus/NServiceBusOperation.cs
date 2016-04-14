using System;
using System.IO;
using System.Threading;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Remote.Deployment.NServiceBus
{
    public class NServiceBusOperation : RemoteOperation
    {
        private string _serviceInstallerName = "NServiceBus.Host.exe";
        private readonly string _sourcePath;
        private readonly string _destPath;
        private readonly string _serviceName;
        private readonly string _profile;
        private readonly Action<IOfferWindowsServiceOptions> _options;

        public NServiceBusOperation(string path, string destDir, string serviceName, string profile, Action<IOfferWindowsServiceOptions> options)
        {
            _sourcePath = Path.GetFullPath(path);
            _destPath = destDir;
            _serviceName = serviceName;
            _profile = profile;
            _options = options;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            var installParams = string.Format("/install /serviceName:\"{0}\" /displayName:\"{0}\" {1}", _serviceName, _profile);
            return remote.Deploy.WindowsServiceWithInstaller(_serviceName, _serviceName, _sourcePath, _destPath, _serviceInstallerName,
                                                      installParams, _options).Result;
        }

        public override string Name => "NServiceBus";
    }
}