using ConDep.Dsl.Validation;
using Microsoft.Win32;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry
{
    public class SetRegistryKeyOperation : RemoteCompositeOperation
    {
        private readonly string _path;
        private readonly string _keyName;
        private readonly string _keyValue;
        private readonly RegistryValueKind _keyType;

        public SetRegistryKeyOperation(string path, string keyName, string keyValue, RegistryValueKind keyType)
        {
            _path = path;
            _keyName = keyName;
            _keyValue = keyValue;
            _keyType = keyType;
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Setting Registry Key"; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            server.Execute.PowerShell(string.Format(@"Set-ItemProperty -Path ""Microsoft.PowerShell.Core\Registry::{0}"" -Name {1} -Value {2} -Type {3}", _path, _keyName, _keyValue, _keyType));
        }
    }
}