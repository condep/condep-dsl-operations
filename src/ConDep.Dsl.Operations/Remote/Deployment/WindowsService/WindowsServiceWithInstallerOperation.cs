using System.IO;
using ConDep.Dsl.Operations.Application.Deployment.WindowsService;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Deployment.WindowsService
{
    public class WindowsServiceDeployWithInstallerOperation : WindowsServiceOperationBase
    {
        private readonly string _sourceDir;
        private readonly string _destDir;
        private readonly string _installerParams;

        public WindowsServiceDeployWithInstallerOperation(string serviceName, string displayName, string sourceDir, string destDir, string relativeExePath, string installerParams, WindowsServiceOptions.WindowsServiceOptionValues options)
            : base(serviceName, displayName, relativeExePath, options)
        {
            _sourceDir = sourceDir;
            _destDir = destDir;
            _installerParams = installerParams;
        }

        public override string Name
        {
            get { return "Windows Service With Installer"; }
        }

        protected override void ExecuteInstallService(IOfferRemoteOperations remote)
        {
            var installCmd = string.Format("{0} {1}", Path.Combine(_destDir, _relativeExePath), _installerParams);
            remote.Execute.DosCommand(installCmd);
        }

        protected override void ExecuteDeployment(IOfferRemoteOperations remote)
        {
            remote.Deploy.Directory(_sourceDir, _destDir);
        }
    }
}