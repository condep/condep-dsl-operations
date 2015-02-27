using System;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Remote;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.EnvironmentVariable
{
    public class EnvironmentVariableOperation : ForEachServerOperation
    {
        private readonly string _name;
        private readonly string _value;
        private readonly EnvironmentVariableTarget _target;

        public EnvironmentVariableOperation(string name, string value, EnvironmentVariableTarget target)
        {
            _name = name;
            _value = value;
            _target = target;
        }

        public override void Execute(ServerConfig server, IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            var psExecutor = new PowerShellExecutor(server);
            psExecutor.Execute(string.Format("[Environment]::SetEnvironmentVariable(\"{0}\", \"{1}\", \"{2}\")", _name, _value, _target));
        }

        public override string Name
        {
            get { return "Set Environment Variable " + _name; }
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }
    }
}