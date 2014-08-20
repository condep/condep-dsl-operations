namespace ConDep.Dsl.Operations.Builders
{
    public class BootstrapOperationsBuilder : IOfferBootstrapOperations
    {
        private readonly IOfferLocalOperations _local;

        public BootstrapOperationsBuilder(IOfferLocalOperations local)
        {
            _local = local;
        }

        public IOfferLocalOperations LocalOperations
        {
            get { return _local; }
        }
    }
}