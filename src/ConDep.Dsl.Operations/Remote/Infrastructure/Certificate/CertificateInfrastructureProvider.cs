using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Infrastructure.Certificate;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Certificate
{
    public class CertificateInfrastructureProvider : RemoteOperation
    {
        private readonly string _searchString;
        private readonly string _certFriendlyName;
        private readonly X509FindType _findType;
        private readonly string _certFile;
        private readonly bool _copyCertFromFile;

        public CertificateInfrastructureProvider(string searchString, X509FindType findType) 
        {
            _searchString = searchString;
            _findType = findType;
            _copyCertFromFile = false;
        }

        public CertificateInfrastructureProvider(string searchString, string certFriendlyName) 
        {
            _searchString = searchString;
            _certFriendlyName = certFriendlyName;
            _copyCertFromFile = false;
        }

        public CertificateInfrastructureProvider(string certFile) 
        {
            _certFile = certFile;
            _copyCertFromFile = true;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            if (_copyCertFromFile)
            {
                var cert = new X509Certificate2(_certFile);
                return ConfigureCertInstall(remote, cert);
            }

            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            try
            {
                var certs = new X509Certificate2Collection();

                if (_certFriendlyName != null)
                {
                    certs.AddRange(store.Certificates.Cast<X509Certificate2>().Where(cert => cert.FriendlyName == _certFriendlyName).ToArray());
                }
                else
                {
                    certs.AddRange(store.Certificates.Find(_findType, _searchString, true));
                }

                if (certs.Count != 1)
                {
                    if (certs.Count < 1)
                        throw new ConDepCertificateNotFoundException("Certificate not found");

                    throw new ConDepCertificateDuplicationException("More than one certificate found in search");
                }

                return ConfigureCertInstall(remote, certs[0]);
            }
            finally
            {
                store.Close();
            }
        }

        public override string Name
        {
            get { return "Certificate"; }
        }

        private Result ConfigureCertInstall(IOfferRemoteOperations remote, X509Certificate2 cert)
        {
            var certScript = string.Format("[byte[]]$byteArray = {0}; $myCert = new-object System.Security.Cryptography.X509Certificates.X509Certificate2(,$byteArray); ", string.Join(",", cert.GetRawCertData()));
            certScript += string.Format("$store = new-object System.Security.Cryptography.X509Certificates.X509Store('{0}', '{1}'); $store.open(“MaxAllowed”); $store.add($myCert); $store.close();", StoreName.My, StoreLocation.LocalMachine);
            return remote.Execute.PowerShell(certScript).Result;
        }   
    }
}