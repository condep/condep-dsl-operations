namespace ConDep.Dsl
{
    public interface IOfferWindowsRegistryOptions
    {
        IOfferWindowsRegistryValueOperations Values { get; }
        IOfferWindowsRegistrySubKeyOperations SubKeys { get; }
    }
}