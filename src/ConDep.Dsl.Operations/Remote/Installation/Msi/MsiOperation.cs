using System;
using System.IO;
using System.Linq;
using System.Threading;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Remote.Installation.Msi
{
    public class MsiOperation : RemoteOperation
    {
        private readonly string _packageName;
        private readonly string _srcMsiFilePath;
        private readonly Uri _srcMsiUri;
        private readonly InstallOptions.InstallOptionsValues _installOptions;
        private readonly FileSourceType _srcType;

        public MsiOperation(string packageName, string srcMsiFilePath, InstallOptions.InstallOptionsValues installOptions = null)
        {
            _packageName = packageName;
            _srcMsiFilePath = srcMsiFilePath;
            _installOptions = installOptions;
            _srcType = FileSourceType.File;
        }

        public MsiOperation(string packageName, Uri srcMsiUri, InstallOptions.InstallOptionsValues installOptions = null)
        {
            _packageName = packageName;
            _srcMsiUri = srcMsiUri;
            _installOptions = installOptions;
            _srcType = FileSourceType.Url;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            switch (_srcType)
            {
                case FileSourceType.File:
                    return InstallMsiFromFile(remote, server.GetServerInfo(), _srcMsiFilePath);
                case FileSourceType.Url:
                    return InstallMsiFromUrl(remote, server.GetServerInfo(), _srcMsiUri);
                default:
                    throw new Exception("Source type unknown");
            }
        }

        public override string Name => $"Msi ({_packageName})";

        private Result InstallMsiFromUrl(IOfferRemoteOperations remote, ServerInfo serverInfo, Uri url)
        {
            var dstPath = $@"$env:temp\{Guid.NewGuid() + ".msi"}";
            if (InstallCondtion(serverInfo))
            {
                remote.Execute.PowerShell($"Install-ConDepMsiFromUri \"{url}\" \"{dstPath}\"", SetPowerShellOptions);
                return Result.SuccessChanged();
            }
            return Result.SuccessUnChanged();
        }

        private void SetPowerShellOptions(IOfferPowerShellOptions opt)
        {
            if (_installOptions != null && _installOptions.UseCredSSP)
            {
                opt.UseCredSSP(true);
            }
        }

        private bool InstallCondtion(ServerInfo condtion)
        {
            var installedPackages = condtion.OperatingSystem.InstalledSoftwarePackages.Where(x => x.DisplayName == _packageName);

            if (_installOptions != null && !string.IsNullOrWhiteSpace(_installOptions.Version))
            {
                installedPackages = installedPackages.Where(x => x.DisplayVersion == _installOptions.Version);
            }
            return !installedPackages.Any();
        }

        private Result InstallMsiFromFile(IOfferRemoteOperations remote, ServerInfo serverInfo, string src)
        {
            var dstPath = Path.Combine(@"%temp%\", Path.GetFileName(src));

            if (InstallCondtion(serverInfo))
            {
                remote.Deploy.File(src, dstPath);
                remote.Execute.PowerShell($"Install-ConDepMsiFromFile \"{dstPath}\"", SetPowerShellOptions);
                return Result.SuccessChanged();
            }
            return Result.SuccessUnChanged();
        }
    }
}