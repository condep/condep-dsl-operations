namespace ConDep.Dsl
{
    public interface IOfferChocolateyOptions
    {
        /// <summary>
        /// Forces the package to be installed, even if already installed. Defaults to False.
        /// </summary>
        /// <param name="force">Set to True in order to force installation of package, even if already installed.</param>
        /// <returns></returns>
        IOfferChocolateyOptions Force(bool force);

        /// <summary>
        /// See verbose messaging
        /// </summary>
        /// <param name="verbose"></param>
        /// <returns></returns>
        IOfferChocolateyOptions Verbose(bool verbose);

        /// <summary>
        /// Run in Debug Mode.
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        IOfferChocolateyOptions Debug(bool debug);

        /// <summary>
        /// The source to find the package(s) to install. Special sources include: ruby, webpi, cygwin, windowsfeatures, 
        /// and python. Defaults to default feeds.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        IOfferChocolateyOptions Source(string source);

        /// <summary>
        /// A specific version to install. Defaults to unspecified.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        IOfferChocolateyOptions Version(string version);

        /// <summary>
        /// Force x86 (32bit) installation on 64 bit systems. Defaults to false.
        /// </summary>
        /// <param name="force32BitInstallOn64BitSystem"></param>
        /// <returns></returns>
        IOfferChocolateyOptions ForceX86(bool force32BitInstallOn64BitSystem);

        /// <summary>
        /// Install Arguments to pass to the native installer in the package. Defaults to unspecified.
        /// </summary>
        /// <param name="argumentsToNativeInstaller"></param>
        /// <returns></returns>
        IOfferChocolateyOptions InstallerArgs(string argumentsToNativeInstaller);

        /// <summary>
        /// Parameters to pass to the package. Defaults to unspecified.
        /// </summary>
        /// <param name="packageParams"></param>
        /// <returns></returns>
        IOfferChocolateyOptions PackageParams(string packageParams);

        /// <summary>
        /// Should install arguments be used exclusively
        /// without appending to current package passed arguments? Defaults to
        /// false.
        /// </summary>
        /// <param name="overrideArgs"></param>
        /// <returns></returns>
        IOfferChocolateyOptions OverideArgs(bool overrideArgs);

        /// <summary>
        /// Include Prereleases? Defaults to false.
        /// </summary>
        /// <param name="preRelease"></param>
        /// <returns></returns>
        IOfferChocolateyOptions PreRelease(bool preRelease);

        /// <summary>
        /// Other arguments to pass to Chocolatey. For available arguments 
        /// see Chocolatey documentation at https://github.com/chocolatey/choco/wiki/CommandsInstall
        /// </summary>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        IOfferChocolateyOptions OtherArgs(string otherArgs);
    }
}