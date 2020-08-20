using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;

namespace Com.RelationalAI
{
    public partial class GeneratedDelveClient
    {
        public Connection conn {get; set;}
        public int debugLevel = 0;

        partial void PrepareRequest(Transaction body, HttpClient client, HttpRequestMessage request, string url)
        {
            var uriBuilder = new UriBuilder(request.RequestUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["dbname"] = conn.dbname;
            query["open_mode"] = body.Mode.ToString();
            query["readonly"] = boolStr(body.Readonly);
            query["region"] = EnumString.GetDescription(conn.region);
            query["empty"] = boolStr(body.Actions == null || body.Actions.Count == 0);
            if(conn.computeName != null) {
                query["compute_name"] = conn.computeName;
            }
            uriBuilder.Query = query.ToString();
            request.RequestUri = uriBuilder.Uri;

            // populate headers
            request.Headers.Clear();
            request.Content.Headers.Clear();
            request.Headers.Host = request.RequestUri.Host;
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            // sign request here
            var raiRequest = new RAIRequest(request, conn);
            raiRequest.sign(debugLevel: debugLevel);
        }

        private string boolStr(bool val) {
            return val ? "true" : "false";
        }
    }

    public class DelveClient : GeneratedDelveClient
    {
        private static HttpClient createHttpClient(bool verifySSL) {
            if( verifySSL ) {
                return new HttpClient();
            } else {
                var handler = new HttpClientHandler()
                {
                    SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
                    ServerCertificateCustomValidationCallback = ValidateServerCertificate
                };
                return new HttpClient(handler);
            }
        }
        public static bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors
        )
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            else
            {
                Console.WriteLine("The server certificate is not valid.");
                return true;
            }
        }
        private static HttpClient httpClient = createHttpClient(httpClientVerifySSL);
        private static bool httpClientVerifySSL = Connection.DEFAULT_VERIFY_SSL;

        private static HttpClient getHttpClient(Uri url, bool verifySSL)
        {
            if( url.Scheme == "https" && httpClientVerifySSL != verifySSL) {
                httpClient.Dispose();
                httpClient = createHttpClient(verifySSL);
                httpClientVerifySSL = verifySSL;
            }
            return httpClient;
        }

        public DelveClient(
            string scheme = Connection.DEFAULT_SCHEME,
            string host = Connection.DEFAULT_HOST,
            int port = Connection.DEFAULT_PORT,
            bool verifySSL = Connection.DEFAULT_VERIFY_SSL
        ) : this(new UriBuilder(scheme, host, port).Uri, verifySSL)
        {
        }
        public DelveClient(string url, bool verifySSL = true) : this(new Uri(url), verifySSL)
        {
        }
        public DelveClient(Uri url, bool verifySSL = true) : base(getHttpClient(url, verifySSL))
        {
            this.BaseUrl = url.ToString();
        }
        public DelveClient(Connection conn) : this(conn.baseUrl, conn.verifySSL)
        {
            this.conn = conn;
        }

        public ActionResult runAction(Connection conn, String name, Action action)
        {
            this.conn = conn;

            Transaction xact = new Transaction();
            xact.Mode = TransactionMode.OPEN;
            xact.Dbname = conn.dbname;

            LabeledAction labeledAction = new LabeledAction();
            labeledAction.Name = name;
            labeledAction.Action = action;
            xact.Actions = new List<LabeledAction>();
            xact.Actions.Add(labeledAction);

            Task<TransactionResult> responseTask = this.TransactionAsync(xact);
            TransactionResult response = responseTask.Result;


            if (!response.Aborted && response.Problems.Count == 0)
            {
                foreach (LabeledActionResult act in response.Actions)
                {
                    if (name.Equals(act.Name))
                    {
                        ActionResult res = (ActionResult)act.Result;
                        return res;
                    }
                }
            }
            return null;
        }

        public bool createDatabase(Connection conn, bool overwrite)
        {
            this.conn = conn;

            Transaction xact = new Transaction();
            xact.Mode = overwrite ? TransactionMode.CREATE_OVERWRITE : TransactionMode.CREATE;
            xact.Dbname = conn.dbname;
            xact.Actions = new LinkedList<LabeledAction>();
            if(this.debugLevel > 0) {
                Console.WriteLine("Transaction: " + JObject.FromObject(xact).ToString());
            }
            Task<TransactionResult> responseTask = this.TransactionAsync(xact);
            TransactionResult response = responseTask.Result;

            if(this.debugLevel > 0) {
                Console.WriteLine("TransactionOutput: " + JObject.FromObject(response).ToString());
            }

            return !response.Aborted && response.Problems.Count == 0;
        }

        public InstallActionResult installSource(Connection conn, String name, String srcStr)
        {
            return installSource(conn, name, name, srcStr);
        }
        public InstallActionResult installSource(Connection conn, String name, String path, String srcStr)
        {
            Source src = new Source();
            src.Name = name;
            src.Path = path;
            src.Value = srcStr;

            InstallAction action = new InstallAction();
            action.Sources = new List<Source>();
            action.Sources.Add(src);

            return (InstallActionResult)runAction(conn, "single", action);
        }

        public QueryActionResult query(Connection conn, String name, String path, String srcStr, string output)
        {
            Source src = new Source();
            src.Name = name;
            src.Path = path;
            src.Value = srcStr;

            QueryAction action = new QueryAction();
            action.Source = src;
            action.Outputs = new List<string>();
            action.Outputs.Add(output);

            return (QueryActionResult)runAction(conn, "single", action);
        }
    }
}
