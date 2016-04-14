using System.Collections.Generic;
using System.Text;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry
{
    internal class CreateWindowsRegistryKeyOperation : RemoteOperation
    {
        private readonly WindowsRegistryRoot _root;
        private readonly string _key;
        private readonly string _defaultValue;
        private readonly IEnumerable<WindowsRegistryValue> _values;
        private readonly IEnumerable<WindowsRegistrySubKey> _keys;

        public CreateWindowsRegistryKeyOperation(WindowsRegistryRoot root, string key, IEnumerable<WindowsRegistryValue> values, IEnumerable<WindowsRegistrySubKey> keys)
        {
            _root = root;
            _key = key;
            _values = values;
            _keys = keys;
        }

        public CreateWindowsRegistryKeyOperation(WindowsRegistryRoot root, string key, string defaultValue, IEnumerable<WindowsRegistryValue> values, IEnumerable<WindowsRegistrySubKey> keys)
        {
            _root = root;
            _key = key;
            _defaultValue = defaultValue;
            _values = values;
            _keys = keys;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            var builder = new StringBuilder();
            CreateKeyCmd(builder, _root.ToString(), new WindowsRegistrySubKey(_key, _defaultValue, _values, _keys));

            return remote.Execute.PowerShell(builder.ToString()).Result;
        }

        public override string Name
        {
            get { return "Windows Registry Key"; }
        }

        private void CreateKeyCmd(StringBuilder builder, string parentKey, WindowsRegistrySubKey key)
        {
            string keyPath = parentKey + @"\" + key.KeyName;
            builder.AppendLine(string.Format(@"New-Item -Path ""Microsoft.PowerShell.Core\Registry::{0}"" {1} -force", keyPath, string.IsNullOrWhiteSpace(key.DefaultValue) ? "" : "-Value " + "\"" + key.DefaultValue +  "\""));

            foreach (var value in key.Values)
            {
                CreateValueCmd(builder, keyPath, value);
            }

            foreach (var subKey in key.Keys)
            {
                CreateKeyCmd(builder, keyPath, subKey);
            }
        }

        private void CreateValueCmd(StringBuilder builder, string key, WindowsRegistryValue value)
        {
            builder.AppendLine(string.Format(@"New-ItemProperty -Path ""Microsoft.PowerShell.Core\Registry::{0}"" -Name ""{1}"" -PropertyType {2} -Value ""{3}"" -force", key, value.ValueName, value.ValueKind, value.ValueData));
        }
    }
}