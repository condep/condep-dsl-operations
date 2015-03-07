using ConDep.Dsl.Validation;
using Microsoft.Win32;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry
{
    internal class SetWindowsRegistryValueOperation : RemoteCompositeOperation
    {
        private readonly WindowsRegistryRoot _root;
        private readonly string _key;
        private readonly string _valueName;
        private readonly string _valueData;
        private readonly RegistryValueKind _valueKind;

        public SetWindowsRegistryValueOperation(WindowsRegistryRoot root, string key, string valueName, string valueData, RegistryValueKind valueKind)
        {
            _root = root;
            _key = key;
            _valueName = valueName;
            _valueData = valueData;
            _valueKind = valueKind;
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Windows Registry Value"; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            var fullPath = _root + @"\" + _key;
            server.Execute.PowerShell(string.Format(@"New-ItemProperty -Path ""Microsoft.PowerShell.Core\Registry::{0}"" -Name ""{1}"" -PropertyType {2} -Value ""{3}"" -force", fullPath, _valueName, _valueKind, _valueData));
        }
    }
}