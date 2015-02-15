using System.IO;
using ConDep.Dsl.Logging;
using Ionic.Zip;

namespace ConDep.Dsl.Operations.Remote.Installation.Zip
{
    public class UnZipOperation : RemoteServerOperation
    {
        private readonly string _filePath;
        private readonly string _destPath;

        public UnZipOperation(string filePath, string destPath) : base(filePath, destPath)
        {
            _filePath = filePath;
            _destPath = destPath;
        }

        public override void Execute(ILogForConDep logger)
        {
            if (!File.Exists(_filePath)) throw new FileNotFoundException(_filePath);

            if (!Directory.Exists(_destPath))
            {
                Directory.CreateDirectory(_destPath);
            }

            using (var archive = new ZipFile(_filePath))
            {
                archive.ExtractAll(_destPath, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        public override string Name
        {
            get { return "UnZip Operation"; }
        }
    }
}