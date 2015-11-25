using System.Net;

namespace ConDep.Dsl.Operations.Remote.Installation.Download
{
    public class DownloadOptions : IOfferDownloadOptions
    {
        private readonly DownloadOptionsValues _values = new DownloadOptionsValues();

        public IOfferDownloadOptions TargetDir(string directory)
        {
            _values.TargetDir = directory;
            return this;
        }

        public IOfferDownloadOptions BasicAuthentication(string username, string password)
        {
            _values.BasicAuth = new DownloadOptionsBasicAuth(username, password);
            return this;
        }

        public DownloadOptionsValues Values { get { return _values; } }

        public class DownloadOptionsValues
        {
            public string TargetDir;
            public DownloadOptionsBasicAuth BasicAuth { get; set; }
        }

        public class DownloadOptionsBasicAuth
        {
            private readonly string _username;
            private readonly string _password;

            public DownloadOptionsBasicAuth(string username, string password)
            {
                _username = username;
                _password = password;
            }

            public string Username
            {
                get { return _username; }
            }

            public string Password
            {
                get { return _password; }
            }
        }
    }
}