using System;
using ConDep.Dsl.Operations.Application.Installation.Executable;
using ConDep.Dsl.Operations.Application.Installation.Msi;

namespace ConDep.Dsl
{
    /// <summary>
    /// HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Uninstall
    /// HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall
    /// HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall
    /// </summary>
    public static class InstallationExtensions
    {
        /// <summary>
        /// Installs MSI package on remote server using a MSI package found on local 
        /// file path (not on target server). ConDep will first copy the MSI package to the server 
        /// and then install the package.
        /// </summary>
        /// <param name="install"></param>
        /// <param name="packageName">A uniqe package name (DisplayName in Windows Registry) to make this 
        /// operation idempotent. If this package name is not correct, ConDep will install this package 
        /// on every execution. ConDep looks in these three registry keys for installed software packages: 
        /// HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Uninstall
        /// HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall
        /// HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall</param>
        /// <param name="srcMsiFilePath">A local file path to the MSI package (not a path on target server).</param>
        /// <returns></returns>
        public static IOfferRemoteInstallation Msi(this IOfferRemoteInstallation install, string packageName, string srcMsiFilePath)
        {
            var msiOperation = new MsiOperation(packageName, srcMsiFilePath);
            Configure.Installation(install, msiOperation);
            return install;
        }

        /// <summary>
        /// Installs MSI package on remote server using a MSI package found on local 
        /// file path (not on target server). ConDep will first copy the MSI package to the server 
        /// and then install the package.
        /// </summary>
        /// <param name="install"></param>
        /// <param name="packageName">A uniqe package name (DisplayName in Windows Registry) to make this 
        /// operation idempotent. If this package name is not correct, ConDep will install this package 
        /// on every execution. ConDep looks in these three registry keys for installed software packages: 
        /// HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Uninstall
        /// HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall
        /// HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall</param>
        /// <param name="srcMsiFilePath">A local file path to the MSI package (not a path on target server).</param>
        /// <param name="options">Additional options to pass in to msiexec.</param>
        /// <returns></returns>
        public static IOfferRemoteInstallation Msi(this IOfferRemoteInstallation install, string packageName, string srcMsiFilePath, Action<IOfferMsiOptions> options)
        {
            var msiOptions = new MsiOptions();
            options(msiOptions);
            var msiOperation = new MsiOperation(packageName, srcMsiFilePath, msiOptions);
            Configure.Installation(install, msiOperation);
            return install;
        }

        /// <summary>
        /// Installs MSI package on remote server using the URI to the MSI package. 
        /// ConDep will first download the MSI package to the server and then install the package.
        /// </summary>
        /// <param name="install"></param>
        /// <param name="packageName">A uniqe package name (DisplayName in Windows Registry) to make this 
        /// operation idempotent. If this package name is not correct, ConDep will install this package 
        /// on every execution. ConDep looks in these three registry keys for installed software packages: 
        /// HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Uninstall
        /// HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall
        /// HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall</param>
        /// <param name="srcMsiUri">A URI to the MSI package</param>
        /// <param name="options">Additional options to pass in to msiexec.</param>
        /// <returns></returns>
        public static IOfferRemoteInstallation Msi(this IOfferRemoteInstallation install, string packageName, Uri srcMsiUri, Action<IOfferMsiOptions> options)
        {
            var msiOptions = new MsiOptions();
            options(msiOptions);
            var msiOperation = new MsiOperation(packageName, srcMsiUri, msiOptions);
            Configure.Installation(install, msiOperation);
            return install;
        }

        /// <summary>
        /// Installs MSI package on remote server using the URI to the MSI package. 
        /// ConDep will first download the MSI package to the server and then install the package.
        /// </summary>
        /// <param name="install"></param>
        /// <param name="packageName">A uniqe package name (DisplayName in Windows Registry) to make this 
        /// operation idempotent. If this package name is not correct, ConDep will install this package 
        /// on every execution. ConDep looks in these three registry keys for installed software packages: 
        /// HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Uninstall
        /// HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall
        /// HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall</param>
        /// <param name="srcMsiUri">A URI to the MSI package</param>
        /// <returns></returns>
        public static IOfferRemoteInstallation Msi(this IOfferRemoteInstallation install, string packageName, Uri srcMsiUri)
        {
            var msiOperation = new MsiOperation(packageName, srcMsiUri);
            Configure.Installation(install, msiOperation);
            return install;
        }

        /// <summary>
        /// Use this for installing packages that are not an MSI (.msi extension). Typically an 
        /// executable (.exe) followed by a set of parameters to make the installation 
        /// non-interactive (silent).
        /// </summary>
        /// <param name="install"></param>
        /// <param name="packageName">A uniqe package name (DisplayName in Windows Registry) to make this 
        /// operation idempotent. If this package name is not correct, ConDep will install this package 
        /// on every execution. ConDep looks in these three registry keys for installed software packages: 
        /// HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Uninstall
        /// HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall
        /// HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall</param>
        /// <param name="srcExecutableFilePath">A local file path to the custom package (not a path on target server).</param>
        /// <param name="exeParams">Parameters needed by the package to install silently, logging etc.</param>
        /// <returns></returns>
        public static IOfferRemoteInstallation Custom(this IOfferRemoteInstallation install, string packageName, string srcExecutableFilePath, string exeParams)
        {
            var exeOperation = new InstallExecutableOperation(packageName, srcExecutableFilePath, exeParams);
            Configure.Installation(install, exeOperation);
            return install;
        }

        /// <summary>
        /// Use this for installing packages that are not an MSI (.msi extension). Typically an 
        /// executable (.exe) followed by a set of parameters to make the installation 
        /// non-interactive (silent).
        /// </summary>
        /// <param name="install"></param>
        /// <param name="packageName">A uniqe package name (DisplayName in Windows Registry) to make this 
        /// operation idempotent. If this package name is not correct, ConDep will install this package 
        /// on every execution. ConDep looks in these three registry keys for installed software packages: 
        /// HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Uninstall
        /// HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall
        /// HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall</param>
        /// <param name="srcExecutableUri">A URI to the custom package</param>
        /// <param name="exeParams">Parameters needed by the package to install silently, logging etc.</param>
        /// <returns></returns>
        public static IOfferRemoteInstallation Custom(this IOfferRemoteInstallation install, string packageName, Uri srcExecutableUri, string exeParams)
        {
            var exeOperation = new InstallExecutableOperation(packageName, srcExecutableUri, exeParams);
            Configure.Installation(install, exeOperation);
            return install;
        }
    }
}