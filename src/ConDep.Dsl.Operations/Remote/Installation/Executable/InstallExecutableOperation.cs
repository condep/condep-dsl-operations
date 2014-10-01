using System;
using System.IO;
using System.Linq;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Remote.Installation.Msi;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Installation.Executable
{
    public class InstallExecutableOperation : RemoteCompositeOperation
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

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Custom installer for " + _packageName; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            switch (_sourceType)
            {
                case FileSourceType.File:
                    InstallExecutableFromFile(server, _srcExecutableFilePath, _exeParams);
                    break;
                case FileSourceType.Url:
                    InstallExecutableFromUri(server, _srcExecutableUri, _exeParams);
                    break;
                default:
                    throw new ConDepInstallationFailureException("Invalid fine source type");
            }
        }

        private void InstallExecutableFromUri(IOfferRemoteComposition server, Uri srcExecutableUri, string exeParams)
        {
            var filename = Guid.NewGuid() + ".exe";
            var psDstPath = string.Format(@"$env:temp\{0}", filename);
            server.OnlyIf(InstallCondition)
                    .Execute
                        .PowerShell(string.Format("Get-ConDepRemoteFile \"{0}\" \"{1}\"", srcExecutableUri, psDstPath))
                        .PowerShell(string.Format("cd $env:temp; cmd /c \"{0} {1}\"", filename, exeParams), SetPowerShellOptions);
        }

        private void InstallExecutableFromFile(IOfferRemoteComposition server, string srcExecutableFilePath, string exeParams)
        {
            var filename = Path.GetFileName(srcExecutableFilePath);
            var dstPath = Path.Combine(@"%temp%\", filename);

            server.OnlyIf(InstallCondition)
                .Deploy.File(srcExecutableFilePath, dstPath);

            server.OnlyIf(InstallCondition)
                .Execute.PowerShell(string.Format("cd $env:temp; cmd /c \"{0} {1}\"", filename, exeParams), SetPowerShellOptions);
        }

        private bool InstallCondition(ServerInfo condition)
        {
            return !condition.OperatingSystem.InstalledSoftwarePackages.ToList().Contains(_packageName);
        }

        private void SetPowerShellOptions(IOfferPowerShellOptions opt)
        {
            if (_values != null && _values.UseCredSSP)
            {
                opt.UseCredSSP(true);
            }
        }

    }

    public class ConDepInstallationFailureException : Exception
    {
        public ConDepInstallationFailureException(string message) : base(message)
        {
            
        }
    }
}