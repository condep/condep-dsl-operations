using Microsoft.Win32;

namespace ConDep.Dsl
{
    public interface IOfferWindowsRegistryValueOperations
    {
        IOfferWindowsRegistryValueOperations Add(string valueName, string valueData, RegistryValueKind valueKind);
    }
}