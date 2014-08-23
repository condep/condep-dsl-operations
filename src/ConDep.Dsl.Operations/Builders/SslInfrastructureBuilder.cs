namespace ConDep.Dsl.Operations.Builders
{
    public class SslInfrastructureBuilder : IOfferSslInfrastructure
    {
        private readonly IOfferRemoteConfiguration _infraBuilder;

        public SslInfrastructureBuilder(IOfferRemoteConfiguration infraBuilder)
        {
            _infraBuilder = infraBuilder;
        }

        public IOfferRemoteConfiguration InfrastructureBuilder
        {
            get { return _infraBuilder; }
        }
    }
}