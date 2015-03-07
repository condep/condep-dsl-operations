namespace ConDep.Dsl
{
    internal class ChocolateyOptionValues
    {
        public bool Force { get; set; }
        public string Source { get; set; }
        public string Version { get; set; }
        public bool PreRelease { get; set; }
        public bool ForceX86 { get; set; }
        public string InstallerArgs { get; set; }
        public bool OverrideArgs { get; set; }
        public bool NotSilent { get; set; }
        public string PackageParams { get; set; }
        public bool AllowMultipleVersions { get; set; }
        public bool IgnoreDependencies { get; set; }
        public bool ForceDependencies { get; set; }
        public bool SkipPowerShell { get; set; }
        public bool Verbose { get; set; }
        public int TimeoutInSeconds { get; set; }
        public bool LimitOutput { get; set; }
        public string CacheLocation { get; set; }
    }
}