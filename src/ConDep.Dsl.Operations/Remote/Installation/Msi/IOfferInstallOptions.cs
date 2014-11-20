namespace ConDep.Dsl
{
    public interface IOfferInstallOptions
    {
        IOfferInstallOptions UseCredSSP(bool value);
        IOfferInstallOptions Version(string value);
    }
}