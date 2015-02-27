using System;
using System.IO;
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
            try
            {
                var uri = new Uri(_url);
            }
            catch (Exception ex)
            {
                notification.AddError(new ValidationError(ex.Message));
                return false;
            }
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

            var uri = new Uri(_url);
            var fileName = Path.GetFileName(uri.AbsolutePath);
            var destFile = Path.Combine(dest, fileName);

            server.Execute.PowerShell(string.Format(@"
$path = $ExecutionContext.InvokeCommand.ExpandString(""{1}"")

if((Test-Path $path)) {{
    write-warning 'File allready exist. Not downloading again.'  
}}
else {{
    $client = new-object System.Net.WebClient
    $client.DownloadFile(""{0}"", $path )
}}", _url, destFile));
        }
    }
}