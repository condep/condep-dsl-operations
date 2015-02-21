using System;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Operations.Builders;
using ConDep.Dsl.Operations.Infrastructure;
using ConDep.Dsl.Operations.Infrastructure.IIS;
using ConDep.Dsl.Operations.Infrastructure.IIS.AppPool;
using ConDep.Dsl.Operations.Infrastructure.IIS.WebApp;
using ConDep.Dsl.Operations.Infrastructure.IIS.WebSite;
using ConDep.Dsl.Operations.Infrastructure.Windows;
using ConDep.Dsl.Operations.Remote.Infrastructure.Windows;
using ConDep.Dsl.Operations.Remote.Infrastructure.Windows.EnvironmentVariable;
using ConDep.Dsl.Operations.Remote.Infrastructure.Windows.FileStructure;
using ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry;
using ConDep.Dsl.Operations.Remote.Infrastructure.Windows.UserAdmin;

namespace ConDep.Dsl
{
    public static class InfrastructureExtensions
    {
        /// <summary>
        /// Installs and configures IIS with provided options
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IOfferRemoteConfiguration IIS(this IOfferRemoteConfiguration infra, Action<IisInfrastructureOptions> options)
        {
            var op = new IisInfrastructureOperation();
            options(new IisInfrastructureOptions(op));

            Configure.Operation(infra, op);
            return infra;
        }

        /// <summary>
        /// Installs IIS
        /// </summary>
        /// <returns></returns>
        public static IOfferRemoteConfiguration IIS(this IOfferRemoteConfiguration infra)
        {
            var op = new IisInfrastructureOperation();
            Configure.Operation(infra, op);
            return infra;
        }

        /// <summary>
        /// Offer common Windows operations
        /// </summary>
        /// <returns></returns>
        public static IOfferRemoteConfiguration Windows(this IOfferRemoteConfiguration infra, Action<WindowsInfrastructureOptions> options)
        {
            var op = new WindowsFeatureInfrastructureOperation();
            options(new WindowsInfrastructureOptions(op));
            Configure.Operation(infra, op);
            return infra;
        }

        /// <summary>
        /// Creates a new Web Site in IIS if not exist. If exist, will delete and then create new.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IOfferRemoteConfiguration IISWebSite(this IOfferRemoteConfiguration infra, string name, int id)
        {
            var op = new IisWebSiteOperation(name, id);
            Configure.Operation(infra, op);
            return infra;
        }

        /// <summary>
        /// Creates a new Web Site in IIS if not exist. If exist, will delete and then create new with provided options.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IOfferRemoteConfiguration IISWebSite(this IOfferRemoteConfiguration infra, string name, int id, Action<IOfferIisWebSiteOptions> options)
        {
            var opt = new IisWebSiteOptions();
            options(opt);
            var op = new IisWebSiteOperation(name, id, opt);
            Configure.Operation(infra, op);
            return infra;
        }

        /// <summary>
        /// Will create a new Application Pool in IIS.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IOfferRemoteConfiguration IISAppPool(this IOfferRemoteConfiguration infra, string name)
        {
            var op = new IisAppPoolOperation(name);
            Configure.Operation(infra, op);
            return infra;
        }

        /// <summary>
        /// Will create a new Application Pool in IIS with provided options.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IOfferRemoteConfiguration IISAppPool(this IOfferRemoteConfiguration infra, string name, Action<IOfferIisAppPoolOptions> options)
        {
            var opt = new IisAppPoolOptions();
            options(opt);
            var op = new IisAppPoolOperation(name, opt.Values);
            Configure.Operation(infra, op);
            return infra;
        }

        /// <summary>
        /// Will create a new Web Application in IIS under the given Web Site.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="webSite"></param>
        /// <returns></returns>
        public static IOfferRemoteConfiguration IISWebApp(this IOfferRemoteConfiguration infra, string name, string webSite)
        {
            var op = new IisWebAppOperation(name, webSite);
            Configure.Operation(infra, op);
            return infra;
        }

        /// <summary>
        /// Will create a new Web Application in IIS under the given Web Site, with the provided options.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="webSite"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IOfferRemoteConfiguration IISWebApp(this IOfferRemoteConfiguration infra, string name, string webSite, Action<IOfferIisWebAppOptions> options)
        {
            var op = new IisWebAppOperation(name, webSite);
            Configure.Operation(infra, op);
            return infra;
        }

        /// <summary>
        /// Provide operations for installing SSL certificates.
        /// </summary>
        public static IOfferSslInfrastructure SslCertificate(this IOfferRemoteConfiguration infra) { return new SslInfrastructureBuilder(infra); }

        /// <summary>
        /// Adds user to group
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="userName">Username</param>
        /// <param name="groupName">Group name</param>
        /// <returns></returns>
        public static IOfferRemoteConfiguration AddUserToGroup(this IOfferRemoteConfiguration configuration, string userName, string groupName)
        {
            var operation = new AddUserToGroupOperation(userName, groupName);
            Configure.Operation(configuration, operation);
            return configuration;
        }

        /// <summary>
        /// Creates a directory, if it not already exists.
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="path">Directory path</param>
        /// <returns></returns>
        public static IOfferRemoteOperations CreateDirectory(this IOfferRemoteOperations remote, string path)
        {
            var operation = new CreateDirectoryOperation(path);
            Configure.Operation(remote, operation);
            return remote;
        }

        /// <summary>
        /// Disables User Account Control. The operation is idempotent and will trigger a restart, but only if UAC not is already disabled. 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IOfferRemoteConfiguration DisableUserAccountControl(this IOfferRemoteConfiguration configuration)
        {
            var operation = new DisableUserAccountControlOperation();
            Configure.Operation(configuration, operation);
            return configuration;
        }

        /// <summary>
        /// Adds a registry key if the key doesn't exists. If the key already exist, this function will update the given key with the given value         
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="keyPath">Key path</param>
        /// <param name="keyName">Key name</param>
        /// <param name="keyValue">Key value</param>
        /// <param name="keyType">Key type</param>
        /// <returns></returns>
        public static IOfferRemoteConfiguration RegistryKey(this IOfferRemoteConfiguration configuration, string keyPath, string keyName, string keyValue, string keyType)
        {
            var operation = new SetRegistryKeyOperation(keyPath, keyName, keyValue, keyType);
            Configure.Operation(configuration, operation);
            return configuration;
        }

        /// <summary>
        /// Creates environment variable if not exists. Overwrites the variable if exists.
        /// </summary>
        /// <param name="configure"></param>
        /// <param name="name">Variable name </param>
        /// <param name="value">Variable value</param>
        /// <param name="target">Variable target</param>
        /// <returns></returns>
        public static IOfferRemoteConfiguration EnvironmentVariable(this IOfferRemoteConfiguration configure, string name, string value, string target)
        {
            var operation = new SetEnvironmentVariableOperation(name, value, target);
            Configure.Operation(configure, operation);
            return configure;
        }
    }
}