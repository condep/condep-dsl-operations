using System;
using ConDep.Dsl.Operations.Application.Installation.Executable;
using ConDep.Dsl.Operations.Application.Installation.Msi;

namespace ConDep.Dsl
{
    public static class InstallationExtensions
    {
        public static IOfferRemoteInstallation Msi(this IOfferRemoteInstallation install, string srcMsiFilePath)
        {
            var msiOperation = new MsiOperation(srcMsiFilePath);
            Configure.Installation(install, msiOperation);
            return install;
        }

        public static IOfferRemoteInstallation Msi(this IOfferRemoteInstallation install, string srcMsiFilePath, Action<IOfferMsiOptions> options)
        {
            var msiOptions = new MsiOptions();
            options(msiOptions);
            var msiOperation = new MsiOperation(srcMsiFilePath, msiOptions);
            Configure.Installation(install, msiOperation);
            return install;
        }

        public static IOfferRemoteInstallation Msi(this IOfferRemoteInstallation install, Uri srcMsiUri, Action<IOfferMsiOptions> options)
        {
            var msiOptions = new MsiOptions();
            options(msiOptions);
            var msiOperation = new MsiOperation(srcMsiUri, msiOptions);
            Configure.Installation(install, msiOperation);
            return install;
        }

        public static IOfferRemoteInstallation Msi(this IOfferRemoteInstallation install, Uri srcMsiUri)
        {
            var msiOperation = new MsiOperation(srcMsiUri);
            Configure.Installation(install, msiOperation);
            return install;
        }

        public static IOfferRemoteInstallation Executable(this IOfferRemoteInstallation install, string srcExecutableFilePath, string exeParams)
        {
            var exeOperation = new InstallExecutableOperation(srcExecutableFilePath, exeParams);
            Configure.Installation(install, exeOperation);
            return install;
        }

        public static IOfferRemoteInstallation Executable(this IOfferRemoteInstallation install, Uri srcExecutableUri, string exeParams)
        {
            var exeOperation = new InstallExecutableOperation(srcExecutableUri, exeParams);
            Configure.Installation(install, exeOperation);
            return install;
        }
    }
}