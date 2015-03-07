using System;

namespace ConDep.Dsl
{
    public interface IOfferWindowsRegistrySubKeyOperations
    {
        IOfferWindowsRegistrySubKeyOperations Add(string keyName, Action<IOfferWindowsRegistryOptions> options = null);
        IOfferWindowsRegistrySubKeyOperations Add(string keyName, string defaultValue, Action<IOfferWindowsRegistryOptions> options = null);
    }
}