using System;
using System.Security.Cryptography.X509Certificates;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Operations.Builders;
using ConDep.Dsl.Operations.Infrastructure.IIS.WebSite;
using ConDep.Dsl.Operations.Remote.Deployment.Certificate;

namespace ConDep.Dsl
{
    public static class RemoteCertDeploymentExtensions
    {
        /// <summary>
        /// Will deploy certificate found by X509FindType and value from the local certificate store, to remote certificate store on server.
        /// </summary>
        /// <param name="remoteCert"></param>
        /// <param name="findType">The X509FindType to use when searching for certificate in the Certificate Store</param>
        /// <param name="findValue">The value to search for in the Certificate Store</param>
        /// <returns></returns>
        public static IOfferRemoteDeployment FromStore(this IOfferRemoteCertDeployment remoteCert, X509FindType findType, string findValue)
        {
            return FromStore(remoteCert, findType, findValue, null);
        }

        /// <summary>
        /// Will deploy certificate found by find type and find value from the local certificate store, to remote certificate store on server with provided options.
        /// </summary>
        /// <param name="remoteCert"></param>
        /// <param name="findType"></param>
        /// <param name="findValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IOfferRemoteDeployment FromStore(this IOfferRemoteCertDeployment remoteCert, X509FindType findType, string findValue, Action<IOfferCertificateOptions> options)
        {
            var certOptions = new CertificateOptions();
            if (options != null)
            {
                options(certOptions);
            }

            var certOp = new CertificateFromStoreOperation(findType, findValue, certOptions);
            OperationExecutor.Execute((RemoteBuilder)remoteCert, certOp);
            return ((RemoteCertDeploymentBuilder)remoteCert).RemoteDeployment;
        }

        /// <summary>
        /// Will deploy certificate from local file path given correct password for private key, and deploy to certificate store on remote server.
        /// </summary>
        /// <param name="remoteCert"></param>
        /// <param name="path"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static IOfferRemoteDeployment FromFile(this IOfferRemoteCertDeployment remoteCert, string path, string password)
        {
            return FromFile(remoteCert, path, password, null);
        }

        /// <summary>
        /// Will deploy certificate from local file path given correct password for private key, and deploy to certificate store on remote server with provided options.
        /// </summary>
        /// <param name="remoteCert"></param>
        /// <param name="path"></param>
        /// <param name="password"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IOfferRemoteDeployment FromFile(this IOfferRemoteCertDeployment remoteCert, string path, string password, Action<IOfferCertificateOptions> options)
        {
            var certOptions = new CertificateOptions();
            if (options != null)
            {
                options(certOptions);
            }

            var certOp = new CertificateFromFileOperation(path, password, certOptions);
            OperationExecutor.Execute((RemoteBuilder)remoteCert, certOp);
            return ((RemoteCertDeploymentBuilder)remoteCert).RemoteDeployment;
        }
    }
}