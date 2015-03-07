using System;
using ConDep.Dsl.Validation;
using Microsoft.Win32;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.UserAdmin
{
    public class UserAccountControlOperation : RemoteCompositeOperation
    {
        private readonly bool _enabled;

        public UserAccountControlOperation(bool enabled)
        {
            _enabled = enabled;
        }

        public override string Name
        {
            get { return "User Account Control"; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            string uacEnabled = string.Format(@"
$regKey = Get-ItemProperty -Path hklm:software\microsoft\windows\currentversion\policies\system -Name ""EnableLUA""
return $regKey.EnableLUA -eq ${0}", !_enabled);

            const string restartNeeded = @"
$restartEnvVar = [Environment]::GetEnvironmentVariable(""CONDEP_RESTART_NEEDED"",""Machine"")
return $restartEnvVar -eq 'true'
";

            //Assume restart is not necessary.
            server.Configure.EnvironmentVariable("CONDEP_RESTART_NEEDED", "false", EnvironmentVariableTarget.Machine);

            //Set uac if not set. Set env variable for restarting server necessary.
            server
                .OnlyIf(uacEnabled)
                    .Configure
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

            //Restart server and set env variable for restart NOT necessary, since the machine rebooted.
            server
                .OnlyIf(restartNeeded)
                    .Restart()
                    .Configure.EnvironmentVariable("CONDEP_RESTART_NEEDED", "false", EnvironmentVariableTarget.Machine);
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }
    }
}