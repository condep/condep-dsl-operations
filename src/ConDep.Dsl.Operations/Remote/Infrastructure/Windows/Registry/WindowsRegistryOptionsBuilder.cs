namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry
{
    internal class WindowsRegistryOptionsBuilder : IOfferWindowsRegistryOptions
    {
        private readonly IOfferWindowsRegistryValueOperations _valuesBuilder = new WindowsRegistryValueBuilder();
        private readonly IOfferWindowsRegistrySubKeyOperations _subKeyBuilder = new WindowsRegistrySubKeyBuilder();

        public IOfferWindowsRegistryValueOperations Values { get { return _valuesBuilder; } }
        public IOfferWindowsRegistrySubKeyOperations SubKeys { get { return _subKeyBuilder; } }
    }
}