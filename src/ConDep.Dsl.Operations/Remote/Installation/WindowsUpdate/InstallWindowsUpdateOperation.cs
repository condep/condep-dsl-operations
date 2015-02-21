using System;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Installation.WindowsUpdate
{
    public class InstallWindowsUpdateOperation : RemoteCompositeOperation
    {
        private readonly string _packageId;
        private readonly string _packageUrl;
        private readonly string _packageName;

        public InstallWindowsUpdateOperation(string packageId, string packageUrl, string packageName)
        {   
            _packageId = packageId;
            _packageUrl = packageUrl;
            _packageName = packageName;
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Installing " + _packageName; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            var notAlreadyInstalled = string.Format(@"
$progs = Get-WmiObject -Class Win32_Product | Select-Object -Property Name

	foreach($prog in $progs){{
		if($prog.Name -eq ""{0}""){{
            return $false
		}}
	}}
	return $true
", _packageId);

            server.OnlyIf(notAlreadyInstalled).Install.Msi(_packageName, new Uri(_packageUrl));
        }
    }
}