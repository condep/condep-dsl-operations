using System.IO;
using ConDep.Dsl.Logging;
using Ionic.Zip;

namespace ConDep.Dsl.Operations.Remote.Installation.Zip
{
    public class ZipOperation : RemoteServerOperation
    {
        private readonly string _pathToCompress;
        private readonly string _destZipFile;

        public ZipOperation(string pathToCompress, string destZipFile)
            : base(pathToCompress, destZipFile)
        {
            _pathToCompress = pathToCompress;
            _destZipFile = destZipFile;
        }

        public override void Execute(ILogForConDep logger)
        {
            using (var archive = new ZipFile(_destZipFile))
            {
                var attr = File.GetAttributes(_pathToCompress);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    archive.AddDirectory(_pathToCompress);
                }
                else
                {
                    archive.AddFile(_pathToCompress);
                }
            }
        }

        public override string Name
        {
            get { return "Zip Operation"; }
        }
    }
}