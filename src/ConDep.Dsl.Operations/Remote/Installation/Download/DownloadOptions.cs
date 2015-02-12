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

        public DownloadOptionsValues Values { get { return _values; } }

        public class DownloadOptionsValues
        {
            public string TargetDir;
        }
    }
}