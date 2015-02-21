using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Remote;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.FileStructure
{
    public class CreateDirectoryOperation : ForEachServerOperation
    {
        private readonly string _path;

        public CreateDirectoryOperation(string path)
        {
            _path = path;
        }

        public override void Execute(ServerConfig server, IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            var createFolderScript = string.Format(@"
if(!(Test-Path ""{0}""))
{{
    New-Item -ItemType directory -Path ""{0}""
}}
", _path);

            var psExecutor = new PowerShellExecutor(server);
            psExecutor.Execute(createFolderScript);
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Create Folder " + _path; }
        }
    }
}