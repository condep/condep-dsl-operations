using System.Collections.Generic;
using System.Diagnostics;
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
            var opt = new List<string> {"-yes"};
            if (options != null)
            {
                if (options.Debug) opt.Add("-debug");
                if (options.Force) opt.Add("-force");
                if (options.ForceX86) opt.Add("-forceX86");

                if (!string.IsNullOrWhiteSpace(options.InstallerArgs)) opt.Add("-installArguments \"" + options.InstallerArgs + "\"");
                if (!string.IsNullOrWhiteSpace(options.PackageParams)) opt.Add("-packageParameters \"" + options.PackageParams + "\"");
                if (!string.IsNullOrWhiteSpace(options.Source)) opt.Add("-source " + options.Source);
                if (!string.IsNullOrWhiteSpace(options.Version)) opt.Add("-version \"" + options.Version + "\"");
            }

            return string.Join(" ", opt);
        }
    }
}