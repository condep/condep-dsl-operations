using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using ConDep.Dsl.Config;
using Microsoft.Win32;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.UserAdmin
{
    public class UserAccountControlOperation : RemoteOperation
    {
        private readonly bool _enabled;

        public UserAccountControlOperation(bool enabled)
        {
            _enabled = enabled;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            var result = Result.SuccessUnChanged();
            result.Data.CausedRestart = false;

            string uacEnabled = $@"
$regKey = Get-ItemProperty -Path hklm:software\microsoft\windows\currentversion\policies\system -Name ""EnableLUA""
return $regKey.EnableLUA -eq ${!_enabled}";

            const string restartNeeded = @"
$restartEnvVar = [Environment]::GetEnvironmentVariable(""CONDEP_RESTART_NEEDED"",""Machine"")
return $restartEnvVar -eq 'true'
";

            //Assume restart is not necessary.
            remote.Configure.EnvironmentVariable("CONDEP_RESTART_NEEDED", "false", EnvironmentVariableTarget.Machine);

            //Set uac if not set. Set env variable for restarting server necessary.
            var uacExecutionResult = ((Collection<PSObject>)remote.Execute.PowerShell(uacEnabled).Result.Data.PsResult).First().ToString().ToLowerInvariant();
            var uacResult = Convert.ToBoolean(uacExecutionResult);

            if (uacResult == true)
            {
                result.Changed = true;
                remote.Configure
                    .WindowsRegistry(reg =>
                        reg.SetValue(
                            WindowsRegistryRoot.HKEY_LOCAL_MACHINE,
                            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",
                            "EnableLUA",
                            _enabled ? "1" : "0",
                            RegistryValueKind.DWord
                        )
                    )
                    .EnvironmentVariable("CONDEP_RESTART_NEEDED", "true", EnvironmentVariableTarget.Machine);
            }

            //Restart server and set env variable for restart NOT necessary, since the machine rebooted.
            var restartExecutionResult = ((Collection<PSObject>)remote.Execute.PowerShell(restartNeeded).Result.Data.PsResult).First().ToString().ToLowerInvariant();
            var restartResult = Convert.ToBoolean(restartExecutionResult);
            
            if (restartResult == true)
            {
                result.Data.CausedRestart = true;
                remote
                    .Restart()
                    .Configure.EnvironmentVariable("CONDEP_RESTART_NEEDED", "false", EnvironmentVariableTarget.Machine);
            }
            return result;
        }

        public override string Name => "User Account Control";
    }
}