namespace ConDep.Dsl.Operations.Remote.Installation.Chocolatey
{
    internal class ChocolateyOptions : IOfferChocolateyOptions
    {
        private readonly ChocolateyOptionValues _values = new ChocolateyOptionValues();

        public IOfferChocolateyOptions Force(bool force)
        {
            _values.Force = force;
            return this;
        }

        public IOfferChocolateyOptions Verbose(bool verbose)
        {
            _values.Verbose = verbose;
            return this;
        }

        public IOfferChocolateyOptions Debug(bool debug)
        {
            _values.Debug = debug;
            return this;
        }

        public IOfferChocolateyOptions Source(string source)
        {
            _values.Source = source;
            return this;
        }

        public IOfferChocolateyOptions Version(string version)
        {
            _values.Version = version;
            return this;
        }

        public IOfferChocolateyOptions ForceX86(bool force32BitInstallOn64BitSystem)
        {
            _values.ForceX86 = force32BitInstallOn64BitSystem;
            return this;
        }

        public IOfferChocolateyOptions InstallerArgs(string argumentsToNativeInstaller)
        {
            _values.InstallerArgs = argumentsToNativeInstaller;
            return this;
        }
    
        public IOfferChocolateyOptions PackageParams(string packageParams)
        {
            _values.PackageParams = packageParams;
            return this;
        }

        public ChocolateyOptionValues Values { get { return _values; } }
    }
}