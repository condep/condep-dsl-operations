using System;
using System.Threading;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Remote.Installation.WindowsUpdate
{
    public class InstallWindowsUpdateOperation : RemoteOperation
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

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            var notAlreadyInstalled = $@"
$progs = Get-WmiObject -Class Win32_Product | Select-Object -Property Name

	foreach($prog in $progs){{
		if($prog.Name -eq ""{_packageId}""){{
            return ConvertTo-ConDepResult $false
		}}
	}}
	return ConvertTo-ConDepResult $true
";

            var notInstalledResult = remote.Execute.PowerShell(notAlreadyInstalled).Result;

            if (notInstalledResult.Data.PsResult == true)
            {
                remote.Install.Msi(_packageName, new Uri(_packageUrl));
                return Result.SuccessChanged();
            }
            return Result.SuccessUnChanged();
        }

        public override string Name => $"Windows Update ({_packageId})";
    }
}