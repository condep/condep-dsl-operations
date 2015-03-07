using System;
using System.Collections.Generic;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry
{
    internal class WindowsRegistrySubKeyBuilder : IOfferWindowsRegistrySubKeyOperations
    {
        private readonly List<WindowsRegistrySubKey> _subKeys = new List<WindowsRegistrySubKey>();
 
        public IOfferWindowsRegistrySubKeyOperations Add(string keyName, string defaultValue = "", Action<IOfferWindowsRegistryValueOperations> values = null, Action<IOfferWindowsRegistrySubKeyOperations> subKeys = null)
        {
            var valuesBuilder = new WindowsRegistryValueBuilder();
            var subKeyBuilder = new WindowsRegistrySubKeyBuilder();

            if (values != null)
            {
                values(valuesBuilder);
            }
            if (subKeys != null)
            {
                subKeys(subKeyBuilder);
            }
            _subKeys.Add(new WindowsRegistrySubKey(keyName, defaultValue, valuesBuilder.Values, subKeyBuilder.Keys));
            return this;
        }

        public IEnumerable<WindowsRegistrySubKey> Keys { get { return _subKeys; } }
        public IOfferWindowsRegistrySubKeyOperations Add(string keyName, Action<IOfferWindowsRegistryOptions> options = null)
        {
            return Add(keyName, "", options);
        }

        public IOfferWindowsRegistrySubKeyOperations Add(string keyName, string defaultValue, Action<IOfferWindowsRegistryOptions> options = null)
        {
            var optBuilder = new WindowsRegistryOptionsBuilder();

            if (options != null)
            {
                options(optBuilder);
            }

            var valuesBuilder = optBuilder.Values as WindowsRegistryValueBuilder;
            var keysBuilder = optBuilder.SubKeys as WindowsRegistrySubKeyBuilder;

            _subKeys.Add(new WindowsRegistrySubKey(keyName, "", valuesBuilder.Values, keysBuilder.Keys));
            return this;
        }
    }
}