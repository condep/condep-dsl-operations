using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Remote
{
    public class CreateDirectoryOperation : RemoteOperation
    {
        private readonly string _path;

        public CreateDirectoryOperation(string path)
        {
            _path = path;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            var createFolderScript = string.Format(@"
if(!(Test-Path ""{0}""))
{{
    New-Item -ItemType directory -Path ""{0}""
    return $true
}}
return $false
", _path);

            var executionResult = ((Collection<PSObject>)remote.Execute.PowerShell(createFolderScript).Result.Data.PsResult).First().ToString().ToLowerInvariant();

            if (executionResult == _path)
                return new Result(true, true);

            return new Result(true, false);
        }

        public override string Name => "Create Folder " + _path;
    }
}