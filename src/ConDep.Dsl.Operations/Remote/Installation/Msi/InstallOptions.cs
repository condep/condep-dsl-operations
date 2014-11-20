namespace ConDep.Dsl.Operations.Remote.Installation.Msi
{
    public class InstallOptions : IOfferInstallOptions
    {
        private readonly InstallOptionsValues _values = new InstallOptionsValues();

        IOfferInstallOptions IOfferInstallOptions.UseCredSSP(bool value)
        {
            _values.UseCredSSP = value;
            return this;
        }

        IOfferInstallOptions IOfferInstallOptions.Version(string value)
        {
            _values.Version = value;
            return this;
        }

        public InstallOptionsValues Values { get { return _values; } }

        public class InstallOptionsValues
        {
            public bool UseCredSSP;
            public string Version;
        }
    }
}