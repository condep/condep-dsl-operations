using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Installation.Download
{
    public class DownloadOperation : RemoteCompositeOperation
    {
        private readonly string _url;
        private readonly DownloadOptions.DownloadOptionsValues _values;

        public DownloadOperation(string url, DownloadOptions.DownloadOptionsValues values)
        {
            _url = url;
            _values = values;
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Download Operation"; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            var dest = "$env:temp";

            if (_values != null && !string.IsNullOrWhiteSpace(_values.TargetDir))
            {
                dest = _values.TargetDir;
            }
            server.Execute.PowerShell(string.Format("Invoke-WebRequest -Uri {0} -OutFile {1}", _url, dest));
        }
    }
}