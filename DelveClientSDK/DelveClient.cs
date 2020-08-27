using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;

namespace Com.RelationalAI
{
    using AnyValue = System.Object;
    public partial class GeneratedDelveClient
    {
        public const string JSON_CONTENT_TYPE = "application/json; charset=utf-8";
        public const string CSV_CONTENT_TYPE = "text/csv";

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
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(JSON_CONTENT_TYPE);

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
        public static HttpClient createHttpClient(bool verifySSL) {
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
        private static bool httpClientVerifySSL = Connection.DEFAULT_VERIFY_SSL;
        private static HttpClient httpClient = createHttpClient(httpClientVerifySSL);

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

        public TransactionResult runTransaction(Transaction xact)
        {
            if(this.debugLevel > 0) {
                Console.WriteLine("Transaction: " + JObject.FromObject(xact).ToString());
            }
            Task<TransactionResult> responseTask = this.TransactionAsync(xact);
            TransactionResult response = responseTask.Result;

            if(this.debugLevel > 0) {
                Console.WriteLine("TransactionOutput: " + JObject.FromObject(response).ToString());
            }
            return response;
        }

        public ActionResult runAction(Connection conn, Action action, bool isReadOnly = false, TransactionMode mode=TransactionMode.OPEN)
        {
            return runAction(conn, "single", action, isReadOnly, mode);
        }
        public ActionResult runAction(Connection conn, String name, Action action, bool isReadOnly = false, TransactionMode mode=TransactionMode.OPEN)
        {
            this.conn = conn;

            var xact = new Transaction();
            xact.Mode = mode;
            xact.Dbname = conn.dbname;

            var labeledAction = new LabeledAction();
            labeledAction.Name = name;
            labeledAction.Action = action;
            xact.Actions = new List<LabeledAction>();
            xact.Actions.Add(labeledAction);
            xact.Readonly = isReadOnly;

            TransactionResult response = runTransaction(xact);


            if (is_success(response))
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
        private bool is_success(TransactionResult response) {
            return !response.Aborted && response.Problems.Count == 0;
        }

        public bool branchdatabase(Connection conn, string sourceDbname)
        {
            var xact = new Transaction();
            xact.Mode = TransactionMode.BRANCH;
            xact.Dbname = conn.dbname;
            xact.Actions = new LinkedList<LabeledAction>();
            xact.Source_dbname = sourceDbname;
            xact.Readonly = false;
            TransactionResult response = runTransaction(xact);

            return is_success(response);
        }

        public bool createDatabase(Connection conn, bool overwrite)
        {
            this.conn = conn;

            var xact = new Transaction();
            xact.Mode = overwrite ? TransactionMode.CREATE_OVERWRITE : TransactionMode.CREATE;
            xact.Dbname = conn.dbname;
            xact.Actions = new LinkedList<LabeledAction>();
            xact.Readonly = false;

            TransactionResult response = null;
            try {
                response = runTransaction(xact);
            } catch (Exception e) {
                string fullError = ExceptionUtils.flattenException(e);
                Console.WriteLine("Error while creating the database (" + conn.dbname + "): " + fullError);
                return false;
            }

            return is_success(response);
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

            return installSource(conn, src);
        }
        public InstallActionResult installSource(Connection conn, Source src)
        {
            return installSource(conn, new List<Source>() { src });
        }

        public InstallActionResult installSource(Connection conn, ICollection<Source> srcList)
        {
            var action = new InstallAction();
            action.Sources = srcList;

            return (InstallActionResult)runAction(conn, action);
        }

        public ModifyWorkspaceActionResult deleteSource(Connection conn, ICollection<string> srcNameList)
        {
            var action = new ModifyWorkspaceAction();
            action.Delete_source = srcNameList;

            return (ModifyWorkspaceActionResult)runAction(conn, action);
        }

        public ModifyWorkspaceActionResult deleteSource(Connection conn, string srcName)
        {
            return deleteSource(conn, new List<string>() { srcName });
        }

        public IDictionary<string, Source> list_source(Connection conn)
        {
            var action = new ListSourceAction();
            var actionRes = (ListSourceActionResult)runAction(conn, action, isReadOnly: true);

            var resultDict = new Dictionary<string, Source>();
            foreach(Source src in actionRes.Sources) {
                resultDict[src.Name] = src;
            }

            return resultDict;
        }

        public QueryActionResult query(
            Connection conn,
            string output,
            string name = "query",
            string path = null,
            string srcStr = "",
            ICollection<Relation> inputs = null,
            ICollection<string> persist = null,
            bool? is_readonly = null,
            TransactionMode? mode = null
        )
        {
            return query(conn, name, path, srcStr, inputs, new List<string>() { output }, persist, is_readonly, mode);
        }

        public QueryActionResult query(
            Connection conn,
            string name = "query",
            string path = null,
            string srcStr = "",
            ICollection<Relation> inputs = null,
            ICollection<string> outputs = null,
            ICollection<string> persist = null,
            bool? is_readonly = null,
            TransactionMode? mode = null
        )
        {
            if(path == null) path = name;

            var src = new Source();
            src.Name = name;
            src.Path = path;
            src.Value = srcStr;

            return query(conn, src, inputs, outputs, persist, is_readonly, mode);
        }

        public QueryActionResult query(
            Connection conn,
            Source src = null,
            ICollection<Relation> inputs = null,
            ICollection<string> outputs = null,
            ICollection<string> persist = null,
            bool? isReadOnly = null,
            TransactionMode? mode = null
        )
        {
            if(src == null) {
                src = new Source();
                src.Name = "";
                src.Path = "";
                src.Value = "";
            }
            if(inputs == null) inputs = new List<Relation>();
            if(outputs == null) outputs = new List<string>();
            if(persist == null) persist = new List<string>();


            var action = new QueryAction();
            action.Inputs = inputs;
            action.Source = src;
            action.Outputs = outputs;
            action.Persist = persist;

            return (QueryActionResult)runAction(
                conn,
                action,
                isReadOnly.GetValueOrDefault(persist.Count == 0),
                mode.GetValueOrDefault(conn.defaultOpenMode)
            );
        }

        public UpdateActionResult updateEDB(
            Connection conn,
            RelKey rel,
            ICollection<Pair_AnyValue_AnyValue_> updates = null,
            ICollection<Pair_AnyValue_AnyValue_> delta = null
        )
        {
            if(updates == null) updates = new List<Pair_AnyValue_AnyValue_>();
            if(delta == null) delta = new List<Pair_AnyValue_AnyValue_>();

            var action = new UpdateAction();
            action.Rel = rel;
            action.Updates = updates;
            action.Delta = delta;

            return (UpdateActionResult)runAction(conn, action, isReadOnly: true);
        }

        private void _handleNullFieldsForLoadData(LoadData loadData)
        {
            if(!isEmpty(loadData.Path) && !isEmpty(loadData.Data)) {
                var message = string.Format(
                    "Either `Path` or `Data` should be specified for `LoadData`." +
                    "You have provided both: `Path`={0} and `Data`={1}",
                    loadData.Path,
                    loadData.Data
                );
                throw new ArgumentException(message);
            }

            if(isEmpty(loadData.Path) && isEmpty(loadData.Data)) {
                var message = "Either `Path` or `Data` is required.";
                throw new ArgumentException(message);
            }

            if(isEmpty(loadData.Content_type)) {
                throw new ArgumentException("`ContentType` is required.");
            }

            if(!loadData.Content_type.Equals(JSON_CONTENT_TYPE) && !loadData.Content_type.Equals(CSV_CONTENT_TYPE)) {
                throw new ArgumentException(string.Format("`ContentType`={0} is not supported.", loadData.Content_type));
            }

            // if(loadData.Key == null) loadData.Key = new List<string>();

            // if(loadData.File_syntax == null) {
            //     if(loadData.Content_type.Equals(JSON_CONTENT_TYPE)) {
            //         loadData.File_syntax = new JSONFileSyntax();
            //     } else if(loadData.Content_type.Equals(CSV_CONTENT_TYPE)) {
            //         loadData.File_syntax = new CSVFileSyntax();
            //     } else {
            //         throw new InvalidOperationException();
            //     }
            // }

            // if(loadData.File_schema == null) {
            //     if(loadData.Content_type.Equals(JSON_CONTENT_TYPE)) {
            //         loadData.File_schema = new JSONFileSchema();
            //     } else if(loadData.Content_type.Equals(CSV_CONTENT_TYPE)) {
            //         loadData.File_schema = new CSVFileSchema();
            //     } else {
            //         throw new InvalidOperationException();
            //     }
            // }
        }
        private bool isEmpty(string str)
        {
            return str == null || str.Length == 0;
        }
        private void _readFileFromPath(LoadData loadData)
        {
            if(!isEmpty(loadData.Path) && isEmpty(loadData.Data)) {
                if(!File.Exists(loadData.Path)) {
                    throw new FileLoadException(string.Format("Could not load file from {0}", loadData.Path));
                }
                loadData.Data = File.ReadAllText(loadData.Path);
                loadData.Path = null;
            }
        }

        public LoadData jsonString(
            string data,
            AnyValue key = null,
            FileSyntax fileSyntax = null,
            FileSchema fileSchema = null
        )
        {
            var loadData = new LoadData();
            loadData.Data = data;
            loadData.Content_type = JSON_CONTENT_TYPE;
            loadData.Key = key;
            loadData.File_syntax = fileSyntax;
            loadData.File_schema = fileSchema;

            return loadData;
        }

        public LoadData jsonFile(
            string filePath,
            AnyValue key = null,
            FileSyntax fileSyntax = null,
            FileSchema fileSchema = null
        )
        {
            var loadData = new LoadData();
            loadData.Path = filePath;
            loadData.Content_type = JSON_CONTENT_TYPE;
            loadData.Key = key;
            loadData.File_syntax = fileSyntax;
            loadData.File_schema = fileSchema;

            return loadData;
        }

        public LoadData csvString(
            string data,
            AnyValue key = null,
            FileSyntax fileSyntax = null,
            FileSchema fileSchema = null
        )
        {
            var loadData = new LoadData();
            loadData.Data = data;
            loadData.Content_type = CSV_CONTENT_TYPE;
            loadData.Key = key;
            loadData.File_syntax = fileSyntax;
            loadData.File_schema = fileSchema;

            return loadData;
        }

        public LoadData csvFile(
            string filePath,
            AnyValue key = null,
            FileSyntax fileSyntax = null,
            FileSchema fileSchema = null
        )
        {
            var loadData = new LoadData();
            loadData.Path = filePath;
            loadData.Content_type = CSV_CONTENT_TYPE;
            loadData.Key = key;
            loadData.File_syntax = fileSyntax;
            loadData.File_schema = fileSchema;

            return loadData;
        }

        public LoadDataActionResult loadEDB(
            Connection conn,
            string rel,
            string contentType,
            string data = null,
            string path = null,
            AnyValue key = null,
            FileSyntax fileSyntax = null,
            FileSchema fileSchema = null
        )
        {
            var loadData = new LoadData();
            loadData.Content_type = contentType;
            loadData.Data = data;
            loadData.Path = path;
            loadData.Key = key;
            loadData.File_syntax = fileSyntax;
            loadData.File_schema = fileSchema;

            return loadEDB(conn, rel, loadData);
        }
        public LoadDataActionResult loadEDB(
            Connection conn,
            string rel,
            LoadData value
        )
        {
            _readFileFromPath(value);
            _handleNullFieldsForLoadData(value);
            var action = new LoadDataAction();
            action.Rel = rel;
            action.Value = value;

            return (LoadDataActionResult)runAction(conn, action, isReadOnly: true);
        }

        public LoadDataActionResult loadCSV(
            Connection conn,
            string rel,
            string data = null,
            string path = null,
            AnyValue key = null,
            FileSyntax fileSyntax = null,
            FileSchema fileSchema = null
        )
        {
            return loadEDB(conn, rel, CSV_CONTENT_TYPE, data, path, key, fileSyntax, fileSchema);
        }

        public LoadDataActionResult loadJSON(
            Connection conn,
            string rel,
            string data = null,
            string path = null,
            AnyValue key = null
        )
        {
            return loadEDB(conn, rel, JSON_CONTENT_TYPE, data, path, key, new JSONFileSyntax(), new JSONFileSchema());
        }

        public ICollection<RelKey> listEdb(Connection conn)
        {
            var action = new ListEdbAction();
            var actionRes = (ListEdbActionResult)runAction(conn, action, isReadOnly: true);
            return actionRes.Rels;
        }

        public ICollection<RelKey> listEdb(Connection conn, string relName)
        {
            var action = new ListEdbAction();
            action.Relname = relName;
            var actionRes = (ListEdbActionResult)runAction(conn, action, isReadOnly: true);
            return actionRes.Rels;
        }

        public ICollection<RelKey> deleteEdb(Connection conn, string relName)
        {
            var action = new ModifyWorkspaceAction();
            action.Delete_edb = relName;
            var actionRes = (ModifyWorkspaceActionResult)runAction(conn, action);
            return actionRes.Delete_edb_result;
        }

        public ModifyWorkspaceActionResult enableLibrary(Connection conn, string srcName)
        {
            var action = new ModifyWorkspaceAction();
            action.Enable_library = srcName;
            return (ModifyWorkspaceActionResult)runAction(conn, action);
        }

        public CardinalityActionResult cardinality(Connection conn)
        {
            var action = new CardinalityAction();
            return (CardinalityActionResult)runAction(conn, action, isReadOnly: true);
        }

        public CardinalityActionResult cardinality(Connection conn, string relName)
        {
            var action = new CardinalityAction();
            action.Relname = relName;
            return (CardinalityActionResult)runAction(conn, action, isReadOnly: true);
        }

        public ICollection<AbstractProblem> collectProblems(Connection conn)
        {
            var action = new CollectProblemsAction();
            var actionRes = (CollectProblemsActionResult)runAction(conn, action, isReadOnly: true);
            return actionRes.Problems;
        }

        public SetOptionsActionResult configure(
            Connection conn,
            bool? debug,
            bool? debugTrace,
            bool? broken,
            bool? silent,
            bool? abortOnError
        )
        {
            var action = new SetOptionsAction();
            action.Debug = debug;
            action.Debug_trace = debugTrace;
            action.Broken = broken;
            action.Silent = silent;
            action.Abort_on_error = abortOnError;
            return (SetOptionsActionResult)runAction(conn, action, isReadOnly: true);
        }
    }
}
