using System;
using System.Runtime.InteropServices;
using System.Web.UI;
using ConDep.Dsl.Operations.Remote.Installation.Download;
using ConDep.Dsl.Operations.Remote.Installation.Executable;
using ConDep.Dsl.Operations.Remote.Installation.Msi;
using ConDep.Dsl.Operations.Remote.Installation.WindowsUpdate;
using ConDep.Dsl.Operations.Remote.Installation.Zip;

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
            Configure.Operation(install, msiOperation);
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
        public static IOfferRemoteInstallation Msi(this IOfferRemoteInstallation install, string packageName, string srcMsiFilePath, Action<IOfferInstallOptions> options)
        {
            var msiOptions = new InstallOptions();
            options(msiOptions);
            var msiOperation = new MsiOperation(packageName, srcMsiFilePath, msiOptions.Values);
            Configure.Operation(install, msiOperation);
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
        public static IOfferRemoteInstallation Msi(this IOfferRemoteInstallation install, string packageName, Uri srcMsiUri, Action<IOfferInstallOptions> options)
        {
            var msiOptions = new InstallOptions();
            options(msiOptions);
            var msiOperation = new MsiOperation(packageName, srcMsiUri, msiOptions.Values);
            Configure.Operation(install, msiOperation);
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
            Configure.Operation(install, msiOperation);
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
        public static IOfferRemoteInstallation Custom(this IOfferRemoteInstallation install, string packageName, string srcExecutableFilePath, string exeParams, Action<IOfferInstallOptions> options = null)
        {
            var installOptions = new InstallOptions();
            if (options != null)
            {
                options(installOptions);
            }
            var exeOperation = new InstallExecutableOperation(packageName, srcExecutableFilePath, exeParams, installOptions.Values);
            Configure.Operation(install, exeOperation);
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
        public static IOfferRemoteInstallation Custom(this IOfferRemoteInstallation install, string packageName, Uri srcExecutableUri, string exeParams, Action<IOfferInstallOptions> options = null)
        {
            var installOptions = new InstallOptions();
            if (options != null)
            {
                options(installOptions);
            }
            var exeOperation = new InstallExecutableOperation(packageName, srcExecutableUri, exeParams, installOptions.Values);
            Configure.Operation(install, exeOperation);
            return install;
        }

        /// <summary>
        /// Download files using PowerShell's Invoke-WebRequest. If not destination is specified in options, Windows temp folder will be used.
        /// </summary>
        /// <param name="install"></param>
        /// <param name="url">The url for the file to download</param>
        /// <param name="options">Additional download options</param>
        /// <returns></returns>
        public static IOfferRemoteInstallation Download(this IOfferRemoteInstallation install, string url, Action<IOfferDownloadOptions> options = null)
        {
            var downloadOptions = new DownloadOptions();

            if (options != null)
            {
                options(downloadOptions);
            }

            var downloadOperation = new DownloadOperation(url, downloadOptions.Values);
            Configure.Operation(install, downloadOperation);
            return install;
        }

        /// <summary>
        /// Will unzip archive to specified destination. This uses the DotNetZip library under the hood. For more information go to http://dotnetzip.codeplex.com/
        /// </summary>
        /// <param name="install"></param>
        /// <param name="filePath">Path to zip file</param>
        /// <param name="destPath">Path to where archive content should be extracted</param>
        /// <returns></returns>
        public static IOfferRemoteInstallation UnZip(this IOfferRemoteInstallation install, string filePath, string destPath)
        {
            var zipOperation = new UnZipOperation(filePath, destPath);
            Configure.Operation(install, zipOperation);
            return install;
        }

        /// <summary>
        /// Will take a folder or file and zip into specified zip-file.
        /// </summary>
        /// <param name="install"></param>
        /// <param name="pathToCompress">Path to file or directory to compress</param>
        /// <param name="destZipFile">Name of zip-file to add compressed content to</param>
        /// <returns></returns>
        public static IOfferRemoteInstallation Zip(this IOfferRemoteInstallation install, string pathToCompress, string destZipFile)
        {
            var zipOperation = new ZipOperation(pathToCompress, destZipFile);
            Configure.Operation(install, zipOperation);
            return install;
        }

        /// <summary>
        /// Will check if Windows Update package is installed, and install package if not already installed. 
        /// </summary>
        /// <param name="install"></param>
        /// <param name="packageId">Windows package KB-id. Example: KB1234567</param>
        /// <param name="packageUrl">The URL for the msi install file</param>
        /// <param name="packageName">A uniqe package name (DisplayName in Windows Registry) to make this 
        /// operation idempotent. If this package name is not correct, ConDep will install this package 
        /// on every execution. ConDep looks in these three registry keys for installed software packages: 
        /// HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Uninstall
        /// HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall
        /// HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall</param>
        /// <returns></returns>
        public static IOfferRemoteInstallation WindowsUpdate(this IOfferRemoteInstallation install, string packageId, string packageUrl, string packageName)
        {
            var winUpdateOperation = new InstallWindowsUpdateOperation(packageId, packageUrl, packageName);
            Configure.Operation(install, winUpdateOperation);
            return install;
        }
    }
}