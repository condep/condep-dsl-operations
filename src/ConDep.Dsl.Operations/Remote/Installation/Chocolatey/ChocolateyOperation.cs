using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Installation.Chocolatey
{
    internal class ChocolateyOperation : RemoteCompositeOperation
    {
        private readonly ChocolateyOptionValues _options;
        private readonly string[] _packageNames;

        public ChocolateyOperation(string[] packageName, ChocolateyOptionValues options)
        {
            _options = options;
            _packageNames = packageName;
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return string.Format("Chocolatey ({0})", string.Join(", ", _packageNames)); }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            var options = BuildOptions(_options);
            server.Execute.PowerShell(string.Format("choco install {0} {1}", string.Join(" ", _packageNames), options));
        }

        private static string BuildOptions(ChocolateyOptionValues options)
        {
            var switches = "-y";
            if (options != null)
            {
                if (options.AllowMultipleVersions) switches += "m";
                if (options.Force) switches += "f";
                if (options.ForceDependencies) switches += "x";
                if (options.IgnoreDependencies) switches += "i";
                if (options.LimitOutput) switches += "r";
                if (options.OverrideArgs) switches += "o";
                if (options.SkipPowerShell) switches += "n";
            }

            var opt = " --acceptlicense";
            if (options != null)
            {
                if (options.ForceX86) opt += " --x86";
                if (options.NotSilent) opt += " --notsilent";
                if (options.PreRelease) opt += " --pre";
                if (!string.IsNullOrWhiteSpace(options.InstallerArgs)) opt += "--ia=\"" + options.InstallerArgs + "\"";
                if (!string.IsNullOrWhiteSpace(options.PackageParams)) opt += "--params=\"" + options.PackageParams + "\"";
                if (!string.IsNullOrWhiteSpace(options.Source)) opt += "--source=" + options.Source;
                if (options.TimeoutInSeconds > 0) opt += "--execution-timeout=" + options.TimeoutInSeconds;
                if (!string.IsNullOrWhiteSpace(options.Version)) opt += "--version=\"" + options.Version + "\"";
                if (!string.IsNullOrWhiteSpace(options.CacheLocation)) opt += "--cache=" + "\"" + options.CacheLocation + "\"";
            }

            return switches + opt;
        }
    }
}