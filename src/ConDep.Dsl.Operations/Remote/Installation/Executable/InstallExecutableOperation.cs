using System;
using System.IO;
using System.Linq;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Operations.Remote.Installation.Msi;

namespace ConDep.Dsl.Operations.Remote.Installation.Executable
{
    public class InstallExecutableOperation : RemoteOperation
    {
        private readonly Uri _srcExecutableUri;
        private readonly FileSourceType _sourceType;
        private readonly string _packageName;
        private readonly string _srcExecutableFilePath;
        private readonly string _exeParams;
        private readonly InstallOptions.InstallOptionsValues _values;

        public InstallExecutableOperation(string packageName, string srcExecutableFilePath, string exeParams, InstallOptions.InstallOptionsValues values)
        {
            _packageName = packageName;
            _srcExecutableFilePath = srcExecutableFilePath;
            _exeParams = exeParams;
            _values = values;
            _sourceType = FileSourceType.File;
        }

        public InstallExecutableOperation(string packageName, Uri srcExecutableUri, string exeParams, InstallOptions.InstallOptionsValues values)
        {
            _packageName = packageName;
            _srcExecutableUri = srcExecutableUri;
            _exeParams = exeParams;
            _values = values;
            _sourceType = FileSourceType.Url;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            switch (_sourceType)
            {
                case FileSourceType.File:
                    return InstallExecutableFromFile(remote, server.GetServerInfo(), _srcExecutableFilePath, _exeParams);
                case FileSourceType.Url:
                    return InstallExecutableFromUri(remote, server.GetServerInfo(), _srcExecutableUri, _exeParams);
                default:
                    Logger.Error("Invalid find source type");
                    return Result.Failed();
            }
        }

        public override string Name => "Custom installer for " + _packageName;

        private Result InstallExecutableFromUri(IOfferRemoteOperations remote, ServerInfo serverInfo, Uri srcExecutableUri, string exeParams)
        {
            var filename = Guid.NewGuid() + ".exe";
            var psDstPath = $@"$env:temp\{filename}";

            if (InstallCondition(serverInfo))
            {
                remote.Execute
                    .PowerShell($"Get-ConDepRemoteFile \"{srcExecutableUri}\" \"{psDstPath}\"")
                    .PowerShell($"cd $env:temp; cmd /c \"{filename} {exeParams}\"", SetPowerShellOptions);

                return Result.SuccessChanged();
            }
            return Result.SuccessUnChanged();
        }

        private Result InstallExecutableFromFile(IOfferRemoteOperations remote, ServerInfo serverInfo, string srcExecutableFilePath, string exeParams)
        {
            var filename = Path.GetFileName(srcExecutableFilePath);
            var dstPath = Path.Combine(@"%temp%\", filename);

            if (InstallCondition(serverInfo))
            {
                remote.Deploy.File(srcExecutableFilePath, dstPath);
                remote.Execute.PowerShell($"cd $env:temp; cmd /c \"{filename} {exeParams}\"", SetPowerShellOptions);

                return Result.SuccessChanged();
            }
            return Result.SuccessUnChanged();
        }

        private bool InstallCondition(ServerInfo condition)
        {
            var installedPackages = condition.OperatingSystem.InstalledSoftwarePackages.Where(x => x.DisplayName == _packageName);

            if (_values != null && !string.IsNullOrWhiteSpace(_values.Version))
            {
                installedPackages = installedPackages.Where(x => x.DisplayVersion == _values.Version);
            }
            return !installedPackages.Any();
        }

        private void SetPowerShellOptions(IOfferPowerShellOptions opt)
        {
            if (_values != null && _values.UseCredSSP)
            {
                opt.UseCredSSP(true);
            }
        }
    }
}