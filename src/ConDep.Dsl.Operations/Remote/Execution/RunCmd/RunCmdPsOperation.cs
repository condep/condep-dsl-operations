using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Application.Execution.RunCmd
{
    public class RunCmdPsOperation : RemoteOperation
    {
        private readonly string _cmd;
        private readonly RunCmdOptions.RunCmdOptionValues _values;

        public RunCmdPsOperation(string cmd, RunCmdOptions.RunCmdOptionValues values = null)
        {
            _cmd = cmd;
            _values = values ?? new RunCmdOptions.RunCmdOptionValues();
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            return remote.Execute.PowerShell(string.Format(@"
$continueOnError = {0}
cmd /c {1}
if($lastexitcode -gt 0) {{
    if($continueOnError) {{
        Write-Warning ""Exit code $lastexitcode""
    }}
    else {{
        throw ""Exit code $lastexitcode""
    }}
}}", _values.ContinueOnError ? "$true" : "$false", _cmd)).Result;
        }

        public override string Name
        {
            get { return "Run Command (PS)"; }
        }
    }
}