using System;
using System.IO;
using System.Linq;
using ConDep.Dsl.Config;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Application.Installation.Msi
{
    public class MsiOperation : RemoteCompositeOperation
    {
        private readonly string _packageName;
        private readonly string _srcMsiFilePath;
        private readonly Uri _srcMsiUri;
        private readonly MsiOptions _msiOptions;
        private readonly FileSourceType _srcType;

        public MsiOperation(string packageName, string srcMsiFilePath, MsiOptions msiOptions = null)
        {
            _packageName = packageName;
            _srcMsiFilePath = srcMsiFilePath;
            _msiOptions = msiOptions;
            _srcType = FileSourceType.File;
        }

        public MsiOperation(string packageName, Uri srcMsiUri, MsiOptions msiOptions = null)
        {
            _packageName = packageName;
            _srcMsiUri = srcMsiUri;
            _msiOptions = msiOptions;
            _srcType = FileSourceType.Url;
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Msi"; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            switch (_srcType)
            {
                case FileSourceType.File:
                    InstallMsiFromFile(server, _srcMsiFilePath);
                    break;
                case FileSourceType.Url:
                    InstallMsiFromUrl(server, _srcMsiUri);
                    break;
                default:
                    throw new Exception("Source type unknown");
            }
        }

        private void InstallMsiFromUrl(IOfferRemoteComposition server, Uri url)
        {
            var dstPath = string.Format(@"$env:temp\{0}", Guid.NewGuid() + ".msi");
            server.OnlyIf(InstallCondtion)
                .ExecuteRemote.PowerShell(string.Format("Install-ConDepMsiFromUri \"{0}\" \"{1}\"", url, dstPath));
        }

        private bool InstallCondtion(ServerInfo condtion)
        {
            return !condtion.OperatingSystem.InstalledSoftwarePackages.Contains(_packageName);
        }

        private void InstallMsiFromFile(IOfferRemoteComposition server, string src)
        {
            var dstPath = Path.Combine(@"%temp%\", Path.GetFileName(src));

            server.OnlyIf(InstallCondtion)
                .Deploy.File(src, dstPath);

            server.OnlyIf(InstallCondtion)
                .ExecuteRemote.PowerShell(string.Format("Install-ConDepMsiFromFile \"{0}\"", dstPath));
        }
    }
}