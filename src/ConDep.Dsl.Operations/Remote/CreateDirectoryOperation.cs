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
return ConvertTo-ConDepResult $false
", _path);

            var changed = remote.Execute.PowerShell(createFolderScript).Result.Data;
            return new Result(true, changed);
        }

        public override string Name => "Create Folder " + _path;
    }
}