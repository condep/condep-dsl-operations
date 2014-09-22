namespace ConDep.Dsl.Operations.Application.Installation.Msi
{
    public class MsiOptions : IOfferMsiOptions
    {
        private readonly MsiOptionsValues _values = new MsiOptionsValues();

        IOfferMsiOptions IOfferMsiOptions.UseCredSSP(bool value)
        {
            _values.UseCredSSP = value;
            return this;
        }

        public MsiOptionsValues Values { get { return _values; } }

        public class MsiOptionsValues
        {
            public bool UseCredSSP;
        }
    }
}