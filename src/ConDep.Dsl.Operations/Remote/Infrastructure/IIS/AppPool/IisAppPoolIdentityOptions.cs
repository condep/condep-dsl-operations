namespace ConDep.Dsl.Operations.Infrastructure.IIS.AppPool
{
    public class IisAppPoolIdentityOptions : IOfferIisAppPoolIdentityOptions
    {
        private readonly IisAppPoolOptions _appPoolOptions;
        private readonly IisAppPoolOptions.IisAppPoolOptionsValues _values;

        public IisAppPoolIdentityOptions(IisAppPoolOptions appPoolOptions)
        {
            _appPoolOptions = appPoolOptions;
        }

        public IOfferIisAppPoolOptions NetworkService()
        {
            _appPoolOptions.Values.IdentityUsername = "NetworkService";
            return _appPoolOptions;
        }

        public IOfferIisAppPoolOptions LocalService()
        {
            _appPoolOptions.Values.IdentityUsername = "LocalService";
            return _appPoolOptions;
        }

        public IOfferIisAppPoolOptions LocalSystem()
        {
            _appPoolOptions.Values.IdentityUsername = "LocalSystem";
            return _appPoolOptions;
        }

        public IOfferIisAppPoolOptions ApplicationPoolIdentity()
        {
            _appPoolOptions.Values.IdentityUsername = "ApplicationPoolIdentity";
            return _appPoolOptions;
        }

        public IOfferIisAppPoolOptions SpecificUser(string username, string password)
        {
            _appPoolOptions.Values.IdentityUsername = username;
            _appPoolOptions.Values.IdentityPassword = password;
            return _appPoolOptions;
        }
    }
}