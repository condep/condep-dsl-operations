using System.Threading;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry
{
    internal class DeleteWindowsRegistryKeyOperation : RemoteOperation
    {
        private readonly WindowsRegistryRoot _root;
        private readonly string _key;

        public DeleteWindowsRegistryKeyOperation(WindowsRegistryRoot root, string key)
        {
            _root = root;
            _key = key;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            var fullPath = _root + @"\" + _key;
            return remote.Execute.PowerShell(string.Format(@"Remove-Item -Path ""Microsoft.PowerShell.Core\Registry::{0}"" -recurse -force", fullPath)).Result;
        }

        public override string Name
        {
            get { return "Delete Windows Registry Key"; }
        }
    }
}