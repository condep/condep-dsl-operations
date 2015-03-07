using System.Collections.Generic;
using Microsoft.Win32;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry
{
    internal class WindowsRegistryValueBuilder : IOfferWindowsRegistryValueOperations
    {
        private readonly List<WindowsRegistryValue> _values = new List<WindowsRegistryValue>();

        public IOfferWindowsRegistryValueOperations Add(string valueName, string valueData, RegistryValueKind valueKind)
        {
            _values.Add(new WindowsRegistryValue(valueName, valueData, valueKind));
            return this;
        }

        public IEnumerable<WindowsRegistryValue> Values { get { return _values; } }
    }
}