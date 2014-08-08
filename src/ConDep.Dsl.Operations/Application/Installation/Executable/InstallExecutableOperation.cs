using System;
using System.IO;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Application.Installation.Executable
{
    public class InstallExecutableOperation : RemoteCompositeOperation
    {
        private readonly Uri _srcExecutableUri;
        private readonly FileSourceType _sourceType;
        private readonly string _srcExecutableFilePath;
        private readonly string _exeParams;

        public InstallExecutableOperation(string srcExecutableFilePath, string exeParams)
        {
            _srcExecutableFilePath = srcExecutableFilePath;
            _exeParams = exeParams;
            _sourceType = FileSourceType.File;
        }

        public InstallExecutableOperation(Uri srcExecutableUri, string exeParams)
        {
            _srcExecutableUri = srcExecutableUri;
            _exeParams = exeParams;
            _sourceType = FileSourceType.Url;
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Executable installer"; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            switch (_sourceType)
            {
                case FileSourceType.File:
                    InstallExecutableFromFile(server, _srcExecutableFilePath, _exeParams);
                    break;
                case FileSourceType.Url:
                    InstallExecutableFromUri(server, _srcExecutableUri, _exeParams);
                    break;
                default:
                    throw new Exception("Source type unknown.");
            }
        }

        private void InstallExecutableFromUri(IOfferRemoteComposition server, Uri srcExecutableUri, string exeParams)
        {
            var filename = Guid.NewGuid() + ".exe";
            var dosDstPath = string.Format(@"%temp%\{0}", filename);
            var psDstPath = string.Format(@"$env:temp\{0}", filename);

            server.ExecuteRemote
                .PowerShell(string.Format("Get-ConDepRemoteFile \"{0}\" \"{1}\"", srcExecutableUri, psDstPath))
                .DosCommand(string.Format("{0} {1}", dosDstPath, exeParams));
        }

        private void InstallExecutableFromFile(IOfferRemoteComposition server, string srcExecutableFilePath, string exeParams)
        {
            var dstPath = Path.Combine(@"%temp%\", Path.GetFileName(srcExecutableFilePath));
            server.Deploy.File(srcExecutableFilePath, dstPath);
            server.ExecuteRemote.PowerShell(string.Format("Install-ConDepExecutableFromFile \"{0}\" \"{1}\"", dstPath, exeParams));
        }
    }
}