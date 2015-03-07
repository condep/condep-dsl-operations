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
        /// Override the default execution timeout in the configuration of 2700 seconds.
        /// </summary>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        IOfferChocolateyOptions Timeout(int timeoutInSeconds);

        /// <summary>
        /// Limit the output to essential information
        /// </summary>
        /// <param name="limitOutputToEssentialInfo"></param>
        /// <returns></returns>
        IOfferChocolateyOptions LimitOutput(bool limitOutputToEssentialInfo);

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
        /// Include Prereleases? Defaults to false.
        /// </summary>
        /// <param name="preRelease"></param>
        /// <returns></returns>
        IOfferChocolateyOptions PreRelese(bool preRelease);

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
        /// Should install arguments be used exclusively without appending to current package passed 
        /// arguments? Defaults to false.
        /// </summary>
        /// <param name="overrideArgs"></param>
        /// <returns></returns>
        IOfferChocolateyOptions OverrideArgs(bool overrideArgs);

        /// <summary>
        /// Do not install this silently. Defaults to false.
        /// </summary>
        /// <param name="notSilent"></param>
        /// <returns></returns>
        IOfferChocolateyOptions NotSilent(bool notSilent);

        /// <summary>
        /// Parameters to pass to the package. Defaults to unspecified.
        /// </summary>
        /// <param name="packageParams"></param>
        /// <returns></returns>
        IOfferChocolateyOptions PackageParams(string packageParams);

        /// <summary>
        /// Should multiple versions of a package be installed? Defaults to false.
        /// </summary>
        /// <param name="multipleVersions"></param>
        /// <returns></returns>
        IOfferChocolateyOptions AllowMultipleVersions(bool multipleVersions);

        /// <summary>
        /// Ignore dependencies when upgrading package(s). Defaults to false.
        /// </summary>
        /// <param name="ignoreDependencies"></param>
        /// <returns></returns>
        IOfferChocolateyOptions IgnoreDependecies(bool ignoreDependencies);

        /// <summary>
        /// Force dependencies to be reinstalled when force installing package(s). Must be used in 
        /// conjunction with --force. Defaults to false.
        /// </summary>
        /// <param name="forceDependencies"></param>
        /// <returns></returns>
        IOfferChocolateyOptions ForceDependencies(bool forceDependencies);

        /// <summary>
        /// Do not run chocolateyInstall.ps1. Defaults to false.
        /// </summary>
        /// <param name="skipPowerShell"></param>
        /// <returns></returns>
        IOfferChocolateyOptions SkipPowerShell(bool skipPowerShell);
    }
}