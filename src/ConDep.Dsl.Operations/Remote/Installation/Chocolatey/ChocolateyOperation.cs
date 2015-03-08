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
            server.Execute.PowerShell(string.Format(@"
function choco-package-installed($name, $version = $null) {{
    $result = choco version $($name.Trim()) -localonly

    if(!$?) {{ return $false }}    
    if(!$version) {{ return $true }}

	$resultArray = ($result -split '[\r\n]') |? {{$_}}
	$packages = @{{}}

    foreach($item in $resultArray) {{
    	$line = $item.Trim() -split ""\s+""
    	if($line.Count -eq 2) {{
    		$packages.Add($line[0].ToLower().Trim(), $line[1].ToLower().Trim())
    	}}
    }}

    $packageVersion = $packages[$($name.Trim().ToLower())]

    if($packageVersion -eq $null) {{ return $false }}

    return $packageVersion.ToLower().Trim() -eq $version.ToLower().Trim()
}}

$packageNames = ""{0}"" -split "" ""

foreach($package in $packageNames) {{
    if(!(choco-package-installed $package)) {{
        choco install $package {1}
    }}
    else {{
        write-host ""Package $package allready installed.""
    }}
}}
", string.Join(" ", _packageNames), options));
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