using System.IO;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Installation.Zip
{
    public class ZipOperation : RemoteCompositeOperation
    {
        private readonly string _pathToCompress;
        private readonly string _destZipFile;

        public ZipOperation(string pathToCompress, string destZipFile)
        {
            _pathToCompress = pathToCompress;
            _destZipFile = destZipFile;
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Zip Operation"; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            server.Execute.DosCommand(string.Format(@"%ProgramData%\chocolatey\chocolateyinstall\tools\7za.exe a -y ""{0}"" ""{1}""", _destZipFile, _pathToCompress));
        }
    }
}