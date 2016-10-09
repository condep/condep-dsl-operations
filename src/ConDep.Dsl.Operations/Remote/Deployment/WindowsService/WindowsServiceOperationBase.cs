using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Application.Deployment.WindowsService;

namespace ConDep.Dsl.Operations.Remote.Deployment.WindowsService
{
    public abstract class WindowsServiceOperationBase : RemoteOperation
    {
        protected readonly string _serviceName;
        protected readonly string _relativeExePath;
        protected readonly string _displayName;
        protected readonly WindowsServiceOptions.WindowsServiceOptionValues _values;

        protected WindowsServiceOperationBase(string serviceName, string displayName, string relativeExePath, WindowsServiceOptions.WindowsServiceOptionValues values)
        {
            _serviceName = serviceName;
            _relativeExePath = relativeExePath;
            _displayName = displayName;
            _values = values;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            ExecuteRemoveService(remote);
            ExecuteDeployment(remote);
            ExecuteInstallService(remote);
            ExecuteUserRights(remote);
            ExecuteServiceFailure(remote);
            ExecuteServiceConfig(remote);
            if(!_values.DoNotStart) ExecuteServiceStart(remote);

            return Result.SuccessChanged();
        }


        protected virtual void ExecuteDeployment(IOfferRemoteOperations remote)
        {
            
        }

        protected void ExecuteServiceStart(IOfferRemoteOperations remote)
        {
            if (!_values.DoNotStart)
            {
                var start = $"Start-ConDepWinService '{_serviceName}' {_values.TimeOutInSeconds} {"$" + _values.IgnoreFailureOnServiceStartStop}";
                remote.Execute.PowerShell(start,
                                                o => o.ContinueOnError(
                                                    _values.IgnoreFailureOnServiceStartStop));
            }
        }

        protected void ExecuteServiceConfig(IOfferRemoteOperations remote)
        {
            var serviceConfigCommand = _values.GetServiceConfigCommand(_serviceName);
            if (!string.IsNullOrWhiteSpace(serviceConfigCommand)) remote.Execute.DosCommand(serviceConfigCommand);
        }

        protected void ExecuteUserRights(IOfferRemoteOperations remote)
        {
            if (string.IsNullOrWhiteSpace(_values.UserName)) return;

            remote.Execute.PowerShell("$userName=\"" + _values.UserName + "\"; [ConDep.Dsl.Remote.Helpers.LsaWrapperCaller]::AddLogonAsAServiceRights($userName)", opt => opt.RequireRemoteLib());
        }

        protected void ExecuteServiceFailure(IOfferRemoteOperations remote)
        {
            var serviceFailureCommand = _values.GetServiceFailureCommand(_serviceName);
            if (!string.IsNullOrWhiteSpace(serviceFailureCommand)) remote.Execute.DosCommand(serviceFailureCommand);
        }

        protected void ExecuteRemoveService(IOfferRemoteOperations remote)
        {
            var remove = $"Remove-ConDepWinService '{_serviceName}' {_values.TimeOutInSeconds} {"$" + _values.IgnoreFailureOnServiceStartStop}";
            remote.Execute.PowerShell(remove,
                                            o =>
                                            o.ContinueOnError(_values.IgnoreFailureOnServiceStartStop));
        }

        protected virtual void ExecuteInstallService(IOfferRemoteOperations remote) { }
    }
}