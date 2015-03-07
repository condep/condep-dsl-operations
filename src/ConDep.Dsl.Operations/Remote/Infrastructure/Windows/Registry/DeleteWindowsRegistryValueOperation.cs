using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry
{
    internal class DeleteWindowsRegistryValueOperation : RemoteCompositeOperation
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

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Delete Windows Registry Value"; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            var fullPath = _root + @"\" + _key;
            server.Execute.PowerShell(string.Format(@"Remove-ItemProperty -Path ""Microsoft.PowerShell.Core\Registry::{0}"" -Name ""{1}"" -force", fullPath, _valueName));
        }
    }
}