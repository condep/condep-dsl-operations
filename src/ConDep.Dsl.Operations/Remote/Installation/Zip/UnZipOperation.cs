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
            get { return "UnZip Operation"; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            var script = string.Format(@"
if(!(Test-Path ""{0}"")) {{
    throw [System.IO.FileNotFoundException] ""{0} not found.""
}}

$7z = ""$env:ProgramData\chocolatey\chocolateyinstall\tools\7za.exe""
cmd /c $7z x -o""{1}"" ""{0}"" > NUL
", _filePath, _destPath);

            server.Execute.PowerShell(script);
        }
    }
}