using System;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Remote.Node;
using ConDep.Dsl.Remote.Node.Model;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Application.Installation.Executable
{
    public class InstallExecutableOperation : ForEachServerOperation
    {
        private readonly Uri _srcExecutableUri;
        private readonly FileSourceType _sourceType;
        private readonly string _packageName;
        private readonly string _srcExecutableFilePath;
        private readonly string _exeParams;

        public InstallExecutableOperation(string packageName, string srcExecutableFilePath, string exeParams)
        {
            _packageName = packageName;
            _srcExecutableFilePath = srcExecutableFilePath;
            _exeParams = exeParams;
            _sourceType = FileSourceType.File;
        }

        public InstallExecutableOperation(string packageName, Uri srcExecutableUri, string exeParams)
        {
            _packageName = packageName;
            _srcExecutableUri = srcExecutableUri;
            _exeParams = exeParams;
            _sourceType = FileSourceType.Url;
        }

        public override void Execute(ServerConfig server, IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            var api = new Api(string.Format("http://{0}/ConDepNode/", server.Name), server.DeploymentUser.UserName, server.DeploymentUser.Password, settings.Options.ApiTimout);
            InstallationResult result;
            switch (_sourceType)
            {
                case FileSourceType.File:
                    result = api.InstallCustom(_packageName, _srcExecutableFilePath, _exeParams);
                    break;
                case FileSourceType.Url:
                    result = api.InstallCustom(_packageName, _srcExecutableUri, _exeParams);
                    break;
                default:
                    throw new ConDepInstallationFailureException("Invalid fine source type");
            }
            if (result == null)
            {
                throw new ConDepInstallationFailureException("Installation failed with no result for unknown reason");
            }

            if(result.Log != null) Logger.Verbose(result.Log);

            if (!result.Success)
            {
                throw new ConDepInstallationFailureException(string.Format("Installation failed with exit code {0}",
                    result.ExitCode));
            }
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Custom installer for " + _packageName; }
        }

        //private void InstallExecutableFromUri(IOfferRemoteComposition server, Uri srcExecutableUri, string exeParams)
        //{
        //    var filename = Guid.NewGuid() + ".exe";
        //    var psDstPath = string.Format(@"$env:temp\{0}", filename);
        //    server.OnlyIf(InstallCondition)
        //            .Execute
        //                .PowerShell(string.Format("Get-ConDepRemoteFile \"{0}\" \"{1}\"", srcExecutableUri, psDstPath))
        //                .PowerShell(string.Format("cd $env:temp; cmd /c \"{0} {1}\"", filename, exeParams));
        //}

        //private void InstallExecutableFromFile(IOfferRemoteComposition server, string srcExecutableFilePath, string exeParams)
        //{
        //    var filename = Path.GetFileName(srcExecutableFilePath);
        //    var dstPath = Path.Combine(@"%temp%\", filename);

        //    server.OnlyIf(InstallCondition)
        //        .Deploy.File(srcExecutableFilePath, dstPath);

        //    server.OnlyIf(InstallCondition)
        //        .Execute.PowerShell(string.Format("cd $env:temp; cmd /c \"{0} {1}\"", filename, exeParams));
        //}

        //private bool InstallCondition(ServerInfo condition)
        //{
        //    return !condition.OperatingSystem.InstalledSoftwarePackages.Contains(_packageName);
        //}
    }

    public class ConDepInstallationFailureException : Exception
    {
        public ConDepInstallationFailureException(string message) : base(message)
        {
            
        }
    }
}