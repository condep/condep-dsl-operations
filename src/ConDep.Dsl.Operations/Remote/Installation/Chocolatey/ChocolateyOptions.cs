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

        public IOfferChocolateyOptions Timeout(int timeoutInSeconds)
        {
            _values.TimeoutInSeconds = timeoutInSeconds;
            return this;
        }

        public IOfferChocolateyOptions LimitOutput(bool limitOutputToEssentialInfo)
        {
            _values.LimitOutput = limitOutputToEssentialInfo;
            return this;
        }

        public IOfferChocolateyOptions CacheLocation(string cacheLocation)
        {
            _values.CacheLocation = cacheLocation;
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

        public IOfferChocolateyOptions PreRelese(bool preRelease)
        {
            _values.PreRelease = preRelease;
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

        public IOfferChocolateyOptions OverrideArgs(bool overrideArgs)
        {
            _values.OverrideArgs = overrideArgs;
            return this;
        }

        public IOfferChocolateyOptions NotSilent(bool notSilent)
        {
            _values.NotSilent = notSilent;
            return this;
        }

        public IOfferChocolateyOptions PackageParams(string packageParams)
        {
            _values.PackageParams = packageParams;
            return this;
        }

        public IOfferChocolateyOptions AllowMultipleVersions(bool multipleVersions)
        {
            _values.AllowMultipleVersions = multipleVersions;
            return this;
        }

        public IOfferChocolateyOptions IgnoreDependecies(bool ignoreDependencies)
        {
            _values.IgnoreDependencies = ignoreDependencies;
            return this;
        }

        public IOfferChocolateyOptions ForceDependencies(bool forceDependencies)
        {
            _values.ForceDependencies = forceDependencies;
            return this;
        }

        public IOfferChocolateyOptions SkipPowerShell(bool skipPowerShell)
        {
            _values.SkipPowerShell = skipPowerShell;
            return this;
        }

        public ChocolateyOptionValues Values { get { return _values; } }
    }
}