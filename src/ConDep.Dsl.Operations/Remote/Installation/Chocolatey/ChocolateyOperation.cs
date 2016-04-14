using System.Collections.Generic;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Installation.Chocolatey
{
    internal class ChocolateyOperation : RemoteOperation
    {
        private readonly ChocolateyOptionValues _options;
        private readonly string _packageName;

        public ChocolateyOperation(string packageName, ChocolateyOptionValues options)
        {
            _options = options;
            _packageName = packageName;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            var options = BuildOptions(_options);
            return remote.Execute.PowerShell(string.Format(@"
function ConDep-ChocoPackageExist($name, $version = $null) {{
    $name = $name.ToLower().Trim()
    $result = choco search $($name) --local-only
	$resultArray = ($result -split '[\r\n]') |? {{$_}}
	$packages = @{{}}

     foreach($item in $resultArray) {{
         $line = $item.Trim() -split ""\s+""
         if($line.Count -eq 2) {{
           $package = New-Object PSObject -Property @{{
               Name = $line[0].ToLower().Trim()
               Version = $line[1].substring(1)
           }}

           $packages.Add($line[0].ToLower().Trim(), $package)
         }}
    }}

    $foundPackage = $packages[$name]

    if(!$foundPackage) {{ return $false }}

    if($version) {{
        return ($version -eq $foundPackage.Version)
    }}

    return $false
}}

$package = ""{0}""

if((ConDep-ChocoPackageExist $package)) {{
    write-host ""Package $package allready installed.""
}}
else {{
    choco install $package {1}
}}
", _packageName, options)).Result;

        }

        public override string Name
        {
            get { return string.Format("Chocolatey ({0})", _packageName); }
        }

        private static string BuildOptions(ChocolateyOptionValues options)
        {
            var opt = new List<string> {"-y"};
            if (options != null)
            {
                if (options.Debug) opt.Add("-debug");
                if (options.Force) opt.Add("-force");

                if (options.ForceX86) opt.Add("--forcex86");
                if (options.PreRelease) opt.Add("--prerelease");
                if (options.OverrideArgs) opt.Add("--overridearguments");

                if (!string.IsNullOrWhiteSpace(options.InstallerArgs)) opt.Add("--installarguments=\"" + options.InstallerArgs + "\"");
                if (!string.IsNullOrWhiteSpace(options.PackageParams)) opt.Add("--package-parameters=\"" + options.PackageParams + "\"");
                if (!string.IsNullOrWhiteSpace(options.Source)) opt.Add("--source=" + options.Source);
                if (!string.IsNullOrWhiteSpace(options.Version)) opt.Add("--version=\"" + options.Version + "\"");
                if (!string.IsNullOrWhiteSpace(options.PackageParams)) opt.Add("--package-parameters=\"" + options.PackageParams + "\"");
            }

            return string.Join(" ", opt) + " " + (options != null ? options.OtherArgs : "");
        }
    }
}