namespace ConDep.Dsl
{
    internal class ChocolateyOptionValues
    {
        public bool Force { get; set; }
        public string Source { get; set; }
        public string Version { get; set; }
        public bool ForceX86 { get; set; }
        public string InstallerArgs { get; set; }
        public string PackageParams { get; set; }
        public bool Verbose { get; set; }
        public bool Debug { get; set; }
        public bool PreRelease { get; set; }
        public bool OverrideArgs { get; set; }
        public string OtherArgs { get; set; }
    }
}