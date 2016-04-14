using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry
{
    internal class DeleteWindowsRegistryValueOperation : RemoteOperation
    {
        private readonly WindowsRegistryRoot _root;
        private readonly string _key;
        private readonly string _valueName;

        public DeleteWindowsRegistryValueOperation(WindowsRegistryRoot root, string key, string valueName)
        {
            _root = root;
            _key = key;
            _valueName = valueName;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            var fullPath = _root + @"\" + _key;
            return remote.Execute.PowerShell(string.Format(@"Remove-ItemProperty -Path ""Microsoft.PowerShell.Core\Registry::{0}"" -Name ""{1}"" -force", fullPath, _valueName)).Result;
        }

        public override string Name
        {
            get { return "Delete Windows Registry Value"; }
        }
    }
}