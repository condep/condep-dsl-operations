namespace ConDep.Dsl.Operations.Builders
{
    public class TerminateOperationsBuilder : IOfferTerminateOperations
    {
        private readonly IOfferLocalOperations _local;

        public TerminateOperationsBuilder(IOfferLocalOperations local)
        {
            _local = local;
        }

        public IOfferLocalOperations LocalOperations
        {
            get { return _local; }
        }
    }
}