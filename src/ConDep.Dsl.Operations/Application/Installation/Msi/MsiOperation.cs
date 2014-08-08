using System;
using System.IO;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Application.Installation.Msi
{
    public class MsiOperation : RemoteCompositeOperation
    {

        private readonly string _srcMsiFilePath;
        private readonly Uri _srcMsiUri;
        private readonly MsiOptions _msiOptions;
        private readonly FileSourceType _srcType;

        public MsiOperation(string srcMsiFilePath, MsiOptions msiOptions = null)
        {
            _srcMsiFilePath = srcMsiFilePath;
            _msiOptions = msiOptions;
            _srcType = FileSourceType.File;
        }

        public MsiOperation(Uri srcMsiUri, MsiOptions msiOptions = null)
        {
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
            server.ExecuteRemote.PowerShell(string.Format("Install-ConDepMsiFromUri \"{0}\" \"{1}\"", url, dstPath));
        }

        private void InstallMsiFromFile(IOfferRemoteComposition server, string src)
        {
            var dstPath = Path.Combine(@"%temp%\", Path.GetFileName(src));
            server.Deploy.File(src, dstPath);
            server.ExecuteRemote.PowerShell(string.Format("Install-ConDepMsiFromFile \"{0}\"", dstPath));
        }
    }
}