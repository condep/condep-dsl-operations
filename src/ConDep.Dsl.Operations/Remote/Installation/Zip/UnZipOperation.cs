using System.IO;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Installation.Zip
{
    public class UnZipOperation : RemoteCompositeOperation
    {
        private readonly string _filePath;
        private readonly string _destPath;

        public UnZipOperation(string filePath, string destPath)
        {
            _filePath = filePath;
            _destPath = destPath;
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return string.Format("UnZip ({0})", Path.GetFileName(_filePath)); }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            server.Execute.DosCommand(string.Format(@"%ProgramData%\chocolatey\tools\7za.exe x -y -o""{1}"" ""{0}""", _filePath, _destPath));
        }
    }
}