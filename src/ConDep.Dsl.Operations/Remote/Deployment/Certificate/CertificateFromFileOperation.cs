using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Infrastructure.IIS.WebSite;

namespace ConDep.Dsl.Operations.Remote.Deployment.Certificate
{
    public class CertificateFromFileOperation : RemoteOperation
    {
        private readonly string _path;
        private readonly string _password;
        private readonly CertificateOptions _certOptions;

        public CertificateFromFileOperation(string path, string password, CertificateOptions certOptions = null)
        {
            _path = path;
            _password = password;
            _certOptions = certOptions;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            var result = Result.SuccessChanged();
            result.Data.HasPrivateKey = false;
            result.Data.PrivateKeyPermissionsSet = false;

            var path = Path.GetFullPath(_path);
            var cert = string.IsNullOrWhiteSpace(_password) ? new X509Certificate2(path) : new X509Certificate2(path, _password);

            if (cert.HasPrivateKey)
            {
                result.Data.HasPrivateKey = true;

                string psUserArray = "@()";
                if (_certOptions != null && _certOptions.Values.PrivateKeyPermissions.Count > 0)
                {
                    var formattedUserArray = _certOptions.Values.PrivateKeyPermissions.Select(user => "'" + user + "'").ToList();
                    var users = string.Join(",", formattedUserArray);
                    psUserArray = $"@({users})";
                    result.Data.PrivateKeyPermissionsSet = true;
                }

                var destPath = $@"{"%windir%"}\temp\{Guid.NewGuid()}.pfx";
                remote.Deploy.File(path, destPath);
                remote.Execute.PowerShell("$path=\"" + destPath + "\"; $password='" + _password + "'; $privateKeyUsers = " + psUserArray + "; [ConDep.Dsl.Remote.Helpers.CertificateInstaller]::InstallPfx($path, $password, $privateKeyUsers);", opt => opt.RequireRemoteLib());
            }
            else
            {
                var base64Cert = Convert.ToBase64String(cert.RawData);
                remote.Execute.PowerShell($"[ConDep.Dsl.Remote.Helpers.CertificateInstaller]::InstallCertFromBase64('{base64Cert}');", opt => opt.RequireRemoteLib());
            }
            return result;
        }

        public override string Name => "CertificateFromFile";
    }
}