using System.Collections.Generic;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry
{
    internal class WindowsRegistrySubKey
    {
        public string KeyName { get; set; }
        public string DefaultValue { get; set; }
        public IEnumerable<WindowsRegistryValue> Values { get; set; }
        public IEnumerable<WindowsRegistrySubKey> Keys { get; set; }

        public WindowsRegistrySubKey(string keyName, string defaultValue, IEnumerable<WindowsRegistryValue> values, IEnumerable<WindowsRegistrySubKey> keys)
        {
            KeyName = keyName;
            DefaultValue = defaultValue;
            Values = values;
            Keys = keys;
        }
    }
}