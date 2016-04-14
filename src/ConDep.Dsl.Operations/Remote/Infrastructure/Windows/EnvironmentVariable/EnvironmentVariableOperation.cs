using System;
using System.Threading;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.EnvironmentVariable
{
    public class EnvironmentVariableOperation : RemoteOperation
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

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            return remote.Execute.PowerShell($"[Environment]::SetEnvironmentVariable(\"{_name}\", \"{_value}\", \"{_target}\")").Result;
        }

        public override string Name => "Set Environment Variable " + _name;
    }
}