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
            get { return "Downloading " + Path.GetFileName(new Uri(_url).AbsolutePath); }
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

            string basicAuth = "";
            if (_values.BasicAuth != null)
            {
                basicAuth = string.Format(@"
    $client.Credentials = new-object system.net.networkcredential(""{0}"", ""{1}"")", _values.BasicAuth.Username, _values.BasicAuth.Password);
            }

            server.Execute.PowerShell(string.Format(@"
$path = $ExecutionContext.InvokeCommand.ExpandString(""{1}"")

if((Test-Path $path)) {{
    write-host 'File allready exist. Skipping.'  
}}
else {{
    $client = new-object System.Net.WebClient
{2}
    $client.DownloadFile(""{0}"", $path )
}}", _url, destFile, basicAuth));
        }
    }
}