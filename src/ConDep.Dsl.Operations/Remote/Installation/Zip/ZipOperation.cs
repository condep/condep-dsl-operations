using System.IO;
using System.Threading;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Remote.Installation.Zip
{
    public class ZipOperation : RemoteOperation
    {
        private readonly string _pathToCompress;
        private readonly string _destZipFile;

        public ZipOperation(string pathToCompress, string destZipFile)
        {
            _pathToCompress = pathToCompress;
            _destZipFile = destZipFile;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            return remote.Execute.DosCommand(string.Format(@"%ProgramData%\chocolatey\tools\7za.exe a -y ""{0}"" ""{1}""", _destZipFile, _pathToCompress)).Result;
        }

        public override string Name
        {
            get { return string.Format("Zip ({0})", Path.GetFileName(_destZipFile)); }
        }
    }
}