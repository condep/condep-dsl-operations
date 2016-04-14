using System.IO;
using System.ServiceProcess;
using ConDep.Dsl.Operations.Application.Deployment.WindowsService;

namespace ConDep.Dsl.Operations.Remote.Deployment.WindowsService
{
    public class WindowsServiceDeployOperation : WindowsServiceOperationBase
    {
        private readonly string _sourceDir;
        private readonly string _destDir;

        public WindowsServiceDeployOperation(string serviceName, string displayName, string sourceDir, string destDir, string relativeExePath, WindowsServiceOptions.WindowsServiceOptionValues values)
            : base(serviceName, displayName, relativeExePath, values)
        {
            _sourceDir = sourceDir;
            _destDir = destDir;
        }

        public override string Name
        {
            get { return "Windows Service"; }
        }

        protected override void ExecuteInstallService(IOfferRemoteOperations remote)
        {
            var installCmd = string.Format("New-ConDepWinService '{0}' '{1}' {2} {3} {4}",
                                           _serviceName,
                                           Path.Combine(_destDir, _relativeExePath) + " " + _values.ExeParams,
                                           string.IsNullOrWhiteSpace(_displayName) ? "$null" : ("'" + _displayName + "'"),
                                           string.IsNullOrWhiteSpace(_values.Description)
                                               ? "$null"
                                               : ("'" + _values.Description + "'"),
                                           _values.StartupType.HasValue ? "'" + _values.StartupType + "'" : "'" + ServiceStartMode.Manual + "'"
                );

            remote.Execute.PowerShell(installCmd);
        }

        protected override void ExecuteDeployment(IOfferRemoteOperations remote)
        {
            remote.Deploy.Directory(_sourceDir, _destDir);
        }
    }
}