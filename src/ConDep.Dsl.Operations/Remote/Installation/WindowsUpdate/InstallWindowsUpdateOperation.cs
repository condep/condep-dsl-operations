using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
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
            return $false
		}}
	}}
	return $true
";

            var ExecutionResult = ((Collection<PSObject>)remote.Execute.PowerShell(notAlreadyInstalled).Result.Data.PsResult).First().ToString().ToLowerInvariant();
            var notInstalledResult = Convert.ToBoolean(ExecutionResult);

            if (notInstalledResult == true)
            {
                remote.Install.Msi(_packageName, new Uri(_packageUrl));
                return Result.SuccessChanged();
            }
            return Result.SuccessUnChanged();
        }

        public override string Name => $"Windows Update ({_packageId})";
    }
}