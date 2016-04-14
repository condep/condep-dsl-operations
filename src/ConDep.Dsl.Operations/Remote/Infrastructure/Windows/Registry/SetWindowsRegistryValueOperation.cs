using System.Threading;
using ConDep.Dsl.Config;
using Microsoft.Win32;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry
{
    internal class SetWindowsRegistryValueOperation : RemoteOperation
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

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            var fullPath = _root + @"\" + _key;
            return remote.Execute.PowerShell($@"New-ItemProperty -Path ""Microsoft.PowerShell.Core\Registry::{fullPath}"" -Name ""{_valueName}"" -PropertyType {_valueKind} -Value ""{_valueData}"" -force").Result;
        }

        public override string Name => "Windows Registry Value";
    }
}