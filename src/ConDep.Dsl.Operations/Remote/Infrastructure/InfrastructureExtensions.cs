using System;
using System.Diagnostics;
using System.Web.Configuration;
using System.Web.Routing;
using ConDep.Dsl.Operations.Builders;
using ConDep.Dsl.Operations.Infrastructure.IIS;
using ConDep.Dsl.Operations.Infrastructure.IIS.AppPool;
using ConDep.Dsl.Operations.Infrastructure.IIS.WebApp;
using ConDep.Dsl.Operations.Infrastructure.IIS.WebSite;
using ConDep.Dsl.Operations.Infrastructure.Windows;
using ConDep.Dsl.Operations.Remote.Infrastructure.IIS.MachineKey;
using ConDep.Dsl.Operations.Remote.Infrastructure.Windows.EnvironmentVariable;
using ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Registry;
using ConDep.Dsl.Operations.Remote.Infrastructure.Windows.UserAdmin;
using Microsoft.Win32;

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
            var builder = new IisWebAppOptions(name);
            options(builder);

            var op = new IisWebAppOperation(name, webSite, builder.Values);
            Configure.Operation(infra, op);
            return infra;
        }

        /// <summary>
        /// Provide operations for installing SSL certificates.
        /// </summary>
        public static IOfferSslInfrastructure SslCertificate(this IOfferRemoteConfiguration infra) { return new SslInfrastructureBuilder(infra); }

        /// <summary>
        /// Disables User Account Control. The operation is idempotent and will trigger a restart, but only if UAC not is already disabled. 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="enabled">Specify if you want UAC enabled or not. E.g. setting this to false will disable UAC.</param>
        /// <returns></returns>
        public static IOfferRemoteConfiguration UserAccountControl(this IOfferRemoteConfiguration configuration, bool enabled)
        {
            var operation = new UserAccountControlOperation(enabled);
            Configure.Operation(configuration, operation);
            return configuration;
        }

        /// <summary>
        /// Gives access to Windows Registry operations 
        /// </summary>
        /// <param name="conf"></param>
        /// <param name="reg">Windows Registry operations</param>
        /// <returns></returns>
        public static IOfferRemoteConfiguration WindowsRegistry(this IOfferRemoteConfiguration conf, Action<IOfferWindowsRegistryOperations> reg)
        {
            var builder = new WindowsRegistryBuilder(conf);
            reg(builder);
            return conf;
        }

        /// <summary>
        /// Creates a Windows Registry key with default value and optional values and sub keys.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="root">The Windows Registry hive to use. See <see cref="WindowsRegistryRoot"/> for available options. Example: WindowsRegistryRoot.HKEY_LOCAL_MACHINE</param>
        /// <param name="key">Name of the key to create. Example: SOFTWARE\ConDep</param>
        /// <param name="defaultValue">The default value of the key</param>
        /// <param name="options">Additional options for setting Windows Registry values and sub keys.</param>
        /// <returns></returns>
        public static IOfferWindowsRegistryOperations CreateKey(this IOfferWindowsRegistryOperations reg, WindowsRegistryRoot root, string key, string defaultValue, Action<IOfferWindowsRegistryOptions> options = null)
        {
            var optBuilder = new WindowsRegistryOptionsBuilder();

            if (options != null)
            {
                options(optBuilder);
            }

            var valuesBuilder = optBuilder.Values as WindowsRegistryValueBuilder;
            var keysBuilder = optBuilder.SubKeys as WindowsRegistrySubKeyBuilder;

            var op = new CreateWindowsRegistryKeyOperation(root, key, defaultValue, valuesBuilder.Values, keysBuilder.Keys);
            var regBuilder = reg as WindowsRegistryBuilder;
            Configure.Operation(regBuilder.RemoteConfigurationBuilder, op);
            return reg;
        }

        /// <summary>
        /// Creates a Windows Registry key and optional values and sub keys.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="root">The Windows Registry hive to use. See <see cref="WindowsRegistryRoot"/> for available options. Example: WindowsRegistryRoot.HKEY_LOCAL_MACHINE</param>
        /// <param name="key">Name of the key to create. Example: SOFTWARE\ConDep</param>
        /// <param name="options">Additional options for setting Windows Registry values and sub keys.</param>
        /// <returns></returns>
        public static IOfferWindowsRegistryOperations CreateKey(this IOfferWindowsRegistryOperations reg, WindowsRegistryRoot root, string key, Action<IOfferWindowsRegistryOptions> options = null)
        {
            return CreateKey(reg, root, key, "", options);
        }

        /// <summary>
        /// Creates or updates a Windows Registry value.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="root">The Windows Registry hive to use. See <see cref="WindowsRegistryRoot"/> for available options. Example: WindowsRegistryRoot.HKEY_LOCAL_MACHINE</param>
        /// <param name="key">Name of the key containing the value you want to create or update. Example: SOFTWARE\ConDep</param>
        /// <param name="valueName">Name of the registry value</param>
        /// <param name="valueData">The data value you want to set</param>
        /// <param name="valueKind">The data type to use when storing values in the registry</param>
        /// <returns></returns>
        public static IOfferWindowsRegistryOperations SetValue(this IOfferWindowsRegistryOperations reg, WindowsRegistryRoot root, string key, string valueName, string valueData, RegistryValueKind valueKind)
        {
            var op = new SetWindowsRegistryValueOperation(root, key, valueName, valueData, valueKind);
            var regBuilder = reg as WindowsRegistryBuilder;
            Configure.Operation(regBuilder.RemoteConfigurationBuilder, op);
            return reg;
        }

        /// <summary>
        /// Deletes a Windows Registry key.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="root">The Windows Registry hive to use. See <see cref="WindowsRegistryRoot"/> for available options. Example: WindowsRegistryRoot.HKEY_LOCAL_MACHINE</param>
        /// <param name="key">Name of the key you want to delete. Example: SOFTWARE\ConDep</param>
        /// <returns></returns>
        public static IOfferWindowsRegistryOperations DeleteKey(this IOfferWindowsRegistryOperations reg, WindowsRegistryRoot root, string key)
        {
            var op = new DeleteWindowsRegistryKeyOperation(root, key);
            var regBuilder = reg as WindowsRegistryBuilder;
            Configure.Operation(regBuilder.RemoteConfigurationBuilder, op);
            return reg;
        }

        /// <summary>
        /// Deletes a value in Windows Registry.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="root">The Windows Registry hive to use. See <see cref="WindowsRegistryRoot"/> for available options. Example: WindowsRegistryRoot.HKEY_LOCAL_MACHINE</param>
        /// <param name="key">Name of the key where the value you want to delete exists. Example: SOFTWARE\ConDep</param>
        /// <param name="valueName">Name of the value you want to delete.</param>
        /// <returns></returns>
        public static IOfferWindowsRegistryOperations DeleteValue(this IOfferWindowsRegistryOperations reg, WindowsRegistryRoot root, string key, string valueName)
        {
            var op = new DeleteWindowsRegistryValueOperation(root, key, valueName);
            var regBuilder = reg as WindowsRegistryBuilder;
            Configure.Operation(regBuilder.RemoteConfigurationBuilder, op);
            return reg;
        }

        /// <summary>
        /// Creates environment variable if not exists. Overwrites the variable if exists.
        /// </summary>
        /// <param name="configure"></param>
        /// <param name="name">Variable name </param>
        /// <param name="value">Variable value</param>
        /// <param name="target">Variable target</param>
        /// <returns></returns>
        public static IOfferRemoteConfiguration EnvironmentVariable(this IOfferRemoteConfiguration configure, string name, string value, EnvironmentVariableTarget target)
        {
            var operation = new EnvironmentVariableOperation(name, value, target);
            Configure.Operation(configure, operation);
            return configure;
        }

        /// <summary>
        /// Sets the IIS machine key. Configures algorithms and keys to use for encryption, 
        /// decryption, and validation of forms-authentication data and view-state data, and 
        /// for out-of-process session state identification.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="validationKey">Specifies the key used to validate encrypted data</param>
        /// <param name="decryptionKey">Specifies the key that is used to encrypt and decrypt data or the process by which the key is generated</param>
        /// <param name="validation">Specifies the type of encryption that is used to validate data</param>
        /// <returns></returns>
        public static IOfferRemoteConfiguration IisMachineKey(this IOfferRemoteConfiguration configuration, string validationKey, string decryptionKey, MachineKeyValidation validation)
        {
            var operation = new SetIisMachineKeyOperation(validationKey, decryptionKey, validation);
            Configure.Operation(configuration, operation);
            return configuration;
        }
    }
}