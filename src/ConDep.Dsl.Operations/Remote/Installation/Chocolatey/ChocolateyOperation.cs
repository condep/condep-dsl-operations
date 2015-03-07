using System.Collections.Generic;
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

            var opt = new List<string> {"--acceptlicense"};

            if (options != null)
            {
                if (options.ForceX86) opt.Add("--x86");
                if (options.NotSilent) opt.Add("--notsilent");
                if (options.PreRelease) opt.Add("--pre");
                if (!string.IsNullOrWhiteSpace(options.InstallerArgs)) opt.Add("--ia=\"" + options.InstallerArgs + "\"");
                if (!string.IsNullOrWhiteSpace(options.PackageParams)) opt.Add("--params=\"" + options.PackageParams + "\"");
                if (!string.IsNullOrWhiteSpace(options.Source)) opt.Add("--source=" + options.Source);
                if (options.TimeoutInSeconds > 0) opt.Add("--execution-timeout=" + options.TimeoutInSeconds);
                if (!string.IsNullOrWhiteSpace(options.Version)) opt.Add("--version=\"" + options.Version + "\"");
                if (!string.IsNullOrWhiteSpace(options.CacheLocation)) opt.Add("--cache=" + "\"" + options.CacheLocation + "\"");
            }

            return string.Format("{0} {1}", switches, string.Join(" ", opt));
        }
    }
}