using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Operations.Infrastructure.IIS.WebSite;

namespace ConDep.Dsl.Operations.Remote.Deployment.Certificate
{
    public class CertificateFromStoreOperation : RemoteOperation
    {
        private readonly X509FindType _findType;
        private readonly string _findValue;
        private readonly CertificateOptions _certOptions;

        public CertificateFromStoreOperation(X509FindType findType, string findValue, CertificateOptions certOptions = null)
        {
            _findType = findType;
            _findValue = findValue;
            _certOptions = certOptions;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            var result = Result.SuccessChanged();
            result.Data.HasPrivateKey = false;
            result.Data.PrivateKeyPermissionsSet = false;

            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                var certs = store.Certificates;

                var findResult = certs.Find(_findType, _findValue, false);
                if (findResult.Count == 0)
                {
                    Logger.Error($"Certificate with find type [{_findType}] and term [{_findValue}] not found.");
                    result.Success = false;
                    return result;
                }

                if (findResult.Count > 1)
                {
                    Logger.Error($"Certificate with find type [{_findType}] and term [{_findValue}] returned {findResult.Count} certificates. Please narrow your search to only return one certificate.");
                    result.Success = false;
                    return result;
                }

                var cert = findResult[0];

                if (cert.HasPrivateKey)
                {
                    result.Data.HasPrivateKey = true;
                    var guid = Guid.NewGuid();
                    var destPath = $@"%windir%\temp\{guid}.pfx";
                    var sourcePath = Path.Combine(Path.GetTempPath(), guid + ".pfx");

                    const string password = "%se65#1s)=3";
                    var exportedCert = cert.Export(X509ContentType.Pkcs12, password);
                    File.WriteAllBytes(sourcePath, exportedCert);

                    remote.Deploy.File(sourcePath, destPath);

                    if(_certOptions.Values.PrivateKeyPermissions.Any()) result.Data.PrivateKeyPermissionsSet = true;

                    var formattedUserArray = _certOptions.Values.PrivateKeyPermissions.Select(user => "'" + user + "'").ToList();
                    var users = string.Join(",", formattedUserArray);
                    var psUserArray = $"@({users})";

                    remote.Execute.PowerShell("$path='" + destPath + "'; $password='" + password + "'; $privateKeyUsers = " + psUserArray + "; [ConDep.Dsl.Remote.Helpers.CertificateInstaller]::InstallPfx($path, $password, $privateKeyUsers);", opt => opt.RequireRemoteLib());
                }
                else
                {
                    var base64Cert = Convert.ToBase64String(cert.RawData);
                    remote.Execute.PowerShell($"[ConDep.Dsl.Remote.Helpers.CertificateInstaller]::InstallCertFromBase64('{base64Cert}');", opt => opt.RequireRemoteLib());
                }
            }
            finally
            {
                store.Close();
            }
            return result;
        }

        public override string Name => "CertificateFromStore";
    }
}