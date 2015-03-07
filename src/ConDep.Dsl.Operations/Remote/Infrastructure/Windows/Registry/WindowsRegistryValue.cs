using Microsoft.Win32;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry
{
    internal class WindowsRegistryValue
    {
        public string ValueName { get; set; }
        public string ValueData { get; set; }
        public RegistryValueKind ValueKind { get; set; }

        public WindowsRegistryValue(string valueName, string valueData, RegistryValueKind valueKind)
        {
            ValueName = valueName;
            ValueData = valueData;
            ValueKind = valueKind;
        }
    }
}