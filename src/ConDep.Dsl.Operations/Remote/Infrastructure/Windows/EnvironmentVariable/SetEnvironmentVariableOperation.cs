using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Remote;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.EnvironmentVariable
{
    public class SetEnvironmentVariableOperation : ForEachServerOperation
    {
        private readonly string _name;
        private readonly string _value;
        private readonly string _target;

        public SetEnvironmentVariableOperation(string name, string value, string target)
        {
            _name = name;
            _value = value;
            _target = target;
        }

        public override void Execute(ServerConfig server, IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            var psExecutor = new PowerShellExecutor(server);
            psExecutor.Execute("[Environment]::SetEnvironmentVariable(\"" + _name + "\", \"" + _value + "\", \"" + _target + "\")");
        }

        public override string Name
        {
            get { return "Set Environment Variable, " + _name; }
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }
    }
}