using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Installation.Chocolatey
{
    public class InstallChocolateyOperation : RemoteCompositeOperation
    {
        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Install Chocolatey"; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            const string notAlreadyInstalled = @"
if(!(Test-Path C:/ProgramData/chocolatey/bin/choco.exe)){
    return $true
}
else {
    return $false
}
";
            server.OnlyIf(notAlreadyInstalled).Execute.PowerShell("iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))");
        }
    }
}