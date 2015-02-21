using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.UserAdmin
{
    public class DisableUserAccountControlOperation : RemoteCompositeOperation
    {
        public override string Name
        {
            get { return "Disable User Account Control"; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            const string uacEnabled = @"
$val = Get-ItemProperty -Path hklm:software\microsoft\windows\currentversion\policies\system -Name ""EnableLUA""
if ($val.EnableLUA -eq 1){
    return $true
}
else {
    return $false
}
";

            const string restartNeeded = @"
$val = [Environment]::GetEnvironmentVariable(""CONDEP_RESTART_NEEDED"",""Machine"")

if($val -eq 'true'){
    return $true
}
else {
    return $false
}
";

            //Assume restart is not necessary.
            server.Configure.EnvironmentVariable("CONDEP_RESTART_NEEDED", "false", "Machine");

            //Set uac if not set. Set env variable for restarting server necessary.
            server
                .OnlyIf(uacEnabled)
                    .Configure
                        .RegistryKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "EnableLUA", "0", "DWord")
                        .EnvironmentVariable("CONDEP_RESTART_NEEDED", "true", "Machine");

            //Restart server and set env variable for restart NOT necessary, since the machine rebooted.
            server
                .OnlyIf(restartNeeded)
                    .Restart()
                    .Configure.EnvironmentVariable("CONDEP_RESTART_NEEDED", "false", "Machine");
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }
    }
}