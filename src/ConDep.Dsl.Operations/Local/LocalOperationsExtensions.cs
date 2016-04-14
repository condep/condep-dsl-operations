using System.Management.Automation;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Operations.Local.PreCompile;
using ConDep.Dsl.Operations.Local.TransformConfig;
using ConDep.Dsl.Operations.Local.WebRequest;

namespace ConDep.Dsl
{
    public static class LocalOperationsExtensions
    {
        /// <summary>
        /// Transforms .NET configuration files (web and app config), in exactly the same way as msbuild and Visual Studio does.
        /// </summary>
        /// <param name="local"></param>
        /// <param name="configDirPath">Path to directory where the config you want to transform is located</param>
        /// <param name="configName">Name of the config file you want to transform</param>
        /// <param name="transformName">Name of the transform file you want to use for transformation</param>
        /// <returns>
        /// </returns>
        public static IOfferLocalOperations TransformConfigFile(this IOfferLocalOperations local, string configDirPath, string configName, string transformName)
        {
            var operation = new TransformConfigOperation(configDirPath, configName, transformName);
            OperationExecutor.Execute((LocalBuilder)local, operation);
            return local;
        }

        /// <summary>
        /// Pre-compile Web Applications to optimize startup time for the application. Even though this operation exist in ConDep, we recommend you to pre-compile web applications as part of your build process, and not the deployment process, using aspnet_compiler.exe.
        /// </summary>
        /// <param name="local"></param>
        /// <param name="webApplicationName">Name of the web application you want to pre-compile</param>
        /// <param name="webApplicationPhysicalPath">Location path to web application</param>
        /// <param name="preCompileOutputpath">Path to where you want the pre-compiled application to be copied</param>
        /// <returns></returns>
        public static IOfferLocalOperations PreCompile(this IOfferLocalOperations local, string webApplicationName, string webApplicationPhysicalPath, string preCompileOutputpath)
        {
            var operation = new PreCompileOperation(webApplicationName, webApplicationPhysicalPath,
                                                                          preCompileOutputpath);
            OperationExecutor.Execute((LocalBuilder)local, operation);
            return local;
        }

        /// <summary>
        /// Executes a simple HTTP GET to the specified url expecting a 200 (OK) in return. Will throw an exception if not 200.
        /// </summary>
        /// <param name="local"></param>
        /// <param name="url">The URL you want to HTTP GET</param>
        /// <returns></returns>
        public static IOfferLocalOperations HttpGet(this IOfferLocalOperations local, string url)
        {
            var operation = new HttpGetOperation(url);
            OperationExecutor.Execute((LocalBuilder)local, operation);
            return local;
        }
    }
}