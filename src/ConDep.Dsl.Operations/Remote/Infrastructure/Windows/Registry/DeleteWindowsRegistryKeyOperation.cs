using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry
{
    internal class DeleteWindowsRegistryKeyOperation : RemoteCompositeOperation
    {
        private readonly WindowsRegistryRoot _root;
        private readonly string _key;

        public DeleteWindowsRegistryKeyOperation(WindowsRegistryRoot root, string key)
        {
            _root = root;
            _key = key;
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Delete Windows Registry Key"; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            var fullPath = _root + @"\" + _key;
            server.Execute.PowerShell(string.Format(@"Remove-Item -Path ""Microsoft.PowerShell.Core\Registry::{0}"" -recurse -force", fullPath));
        }
    }
}