using System.Net;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;

namespace ConDep.Dsl.Operations.Local.WebRequest
{
    public class HttpGetOperation : LocalOperation
    {
        private readonly string _url;

        public HttpGetOperation(string url)
        {
            _url = url;
        }

        public override Result Execute(ConDepSettings settings, CancellationToken token)
        {
            var result = Result.SuccessChanged();
            Thread.Sleep(1000);
            var webRequest = System.Net.WebRequest.Create(_url);
            webRequest.Method = "GET";
            webRequest.ContentLength = 0;
            webRequest.ContentType = "application/x-www-form-urlencoded";

            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

            HttpStatusCode statusCode = ((HttpWebResponse)webRequest.GetResponse()).StatusCode;
            result.Data.StatusCode = statusCode.ToString();
            result.Data.Url = _url;

            if (statusCode == HttpStatusCode.OK)
            {
                Logger.Info("HTTP {0} Succeeded: {1}", "GET", _url);
            }
            else
            {
                Logger.Error("GET request did not return with 200 (OK), but {0} ({1})", (int)statusCode, statusCode);
                result.Success = false;
            }
            return result;
        }

        public override string Name => "Http Get";
    }
}