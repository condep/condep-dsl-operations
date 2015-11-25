namespace ConDep.Dsl
{
    public interface IOfferDownloadOptions
    {
        IOfferDownloadOptions TargetDir(string directory);
        IOfferDownloadOptions BasicAuthentication(string username, string password);
    }
}