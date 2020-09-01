using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

    partial class RelKey
    {
        public override bool Equals(object obj)
        {
            return obj is RelKey key &&
                   Name == key.Name &&
                   EqualityComparer<ICollection<string>>.Default.Equals(Keys, key.Keys) &&
                   EqualityComparer<ICollection<string>>.Default.Equals(Values, key.Values);
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(Name, Keys, Values);
        }
    }

    partial class Relation
    {
        public HashSet<HashSet<AnyValue>> columnsToHashSet(ICollection<ICollection<AnyValue>> columns)
        {
            return columns.Select(col => col.ToHashSet()).ToHashSet();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if ( obj is Relation relation ) {
                return EqualityComparer<RelKey>.Default.Equals(Rel_key, relation.Rel_key) &&
                       columnsToHashSet(Columns).SetEquals(columnsToHashSet(relation.Columns));
            } else if ( obj.GetType().IsArray ) {
                object[] arr = (object[]) obj;
                if( arr.Length == 0 ) {
                    return Columns.Count == 0 || Columns.First().Count == 0;
                } else if ( arr[0].GetType().IsArray ) {
                    return equalsToArr((dynamic) arr);
                }
            }
            return false;
        }

        public static ICollection<ICollection<object>> toCollection(AnyValue[][] arr)
        {
            return arr.Select(col => (ICollection<object>)(col.Cast<object>().ToHashSet())).ToHashSet();
        }

        private bool equalsToArr(AnyValue[][] arr) {
            var x1 = columnsToHashSet(Columns);
            var x2 = columnsToHashSet(toCollection(arr));
            return x1.All(elem1 => x2.Any(elem2 => elem1.SetEquals(elem2))) && x2.All(elem2 => x1.Any(elem1 => elem2.SetEquals(elem1)));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Rel_key, columnsToHashSet(Columns));
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
        private static bool httpClientVerifySSL = Connection.DEFAULT_VERIFY_SSL;
        private static HttpClient httpClient = createHttpClient(httpClientVerifySSL);

        public string dbname { get { return conn.dbname; } }

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

        public ActionResult runAction(Action action, out bool success, bool isReadOnly = false, TransactionMode mode=TransactionMode.OPEN) {
            return runAction("single", action, out success, isReadOnly, mode);
        }

        public ActionResult runAction(Action action, bool isReadOnly = false, TransactionMode mode=TransactionMode.OPEN)
        {
            bool success;
            return runAction("single", action, out success, isReadOnly, mode);
        }
        public ActionResult runAction(String name, Action action, out bool success, bool isReadOnly = false, TransactionMode mode=TransactionMode.OPEN)
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

            success = is_success(response);
            foreach (LabeledActionResult act in response.Actions)
            {
                if (name.Equals(act.Name))
                {
                    ActionResult res = (ActionResult)act.Result;
                    return res;
                }
            }
            return null;
        }
        private bool is_success(TransactionResult response) {
            return !response.Aborted && response.Problems.Count == 0;
        }

        public bool branchdatabase(string sourceDbname)
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

        public bool createDatabase(bool overwrite = false)
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

        public InstallActionResult installSource(String name, String srcStr)
        {
            return installSource(name, name, srcStr);
        }
        public InstallActionResult installSource(String name, String path, String srcStr)
        {
            Source src = new Source();
            src.Name = name;
            src.Path = path;
            src.Value = srcStr;

            return installSource(src);
        }
        public InstallActionResult installSource(Source src)
        {
            return installSource(new List<Source>() { src });
        }

        public InstallActionResult installSource(ICollection<Source> srcList)
        {
            var action = new InstallAction();
            foreach(Source src in srcList) {
                _readFileFromPath(src);
            }
            action.Sources = srcList;

            return (InstallActionResult)runAction(action);
        }

        private void _readFileFromPath(Source src)
        {
            if(!isEmpty(src.Path)) {
                if(isEmpty(src.Value)) {
                    if(!File.Exists(src.Path)) {
                        src.Value = File.ReadAllText(src.Path);
                    }
                }
                if(isEmpty(src.Name)) {
                    src.Name = Path.GetFileNameWithoutExtension(src.Path);
                }
            }
        }

        public bool deleteSource(ICollection<string> srcNameList)
        {
            var action = new ModifyWorkspaceAction();
            action.Delete_source = srcNameList;

            bool success;
            var actionRes = (ModifyWorkspaceActionResult)runAction(action, out success);
            return success && actionRes != null;
        }

        public bool deleteSource(string srcName)
        {
            return deleteSource(new List<string>() { srcName });
        }

        public IDictionary<string, Source> list_source()
        {
            var action = new ListSourceAction();
            var actionRes = (ListSourceActionResult)runAction(action, isReadOnly: true);

            var resultDict = new Dictionary<string, Source>();
            foreach(Source src in actionRes.Sources) {
                resultDict[src.Name] = src;
            }

            return resultDict;
        }

        public IDictionary<RelKey, Relation> query(
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
            return query(name, path, srcStr, inputs, new List<string>() { output }, persist, is_readonly, mode);
        }

        public IDictionary<RelKey, Relation> query(
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

            return filterDictionary(query(src, inputs, outputs, persist, is_readonly, mode), outputs);
        }

        private IDictionary<RelKey, Relation> filterDictionary(IDictionary<RelKey, Relation> dict, ICollection<string> outputs)
        {
            if(dict.All(kvp => outputs.Contains(kvp.Key.Name))) return dict;
            return dict.Where(kvp => outputs.Contains(kvp.Key.Name))
                       .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public IDictionary<RelKey, Relation> query(
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

            var actionRes = (QueryActionResult)runAction(
                action,
                isReadOnly.GetValueOrDefault(persist.Count == 0),
                mode.GetValueOrDefault(conn.defaultOpenMode)
            );

            return actionRes == null ? null : convertToDictionary(actionRes.Output);
        }

        private IDictionary<RelKey, Relation> convertToDictionary(ICollection<Relation> output)
        {
            var outDict = new Dictionary<RelKey, Relation>();
            if(output != null) {
                foreach(Relation rel in output) {
                    outDict[rel.Rel_key] = rel;
                }
            }
            return outDict;
        }

        public UpdateActionResult updateEDB(
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

            return (UpdateActionResult)runAction(action, isReadOnly: false);
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
                if(File.Exists(loadData.Path)) {
                    loadData.Data = File.ReadAllText(loadData.Path);
                }
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

            return loadEDB(rel, loadData);
        }
        public LoadDataActionResult loadEDB(
            string rel,
            LoadData value
        )
        {
            _readFileFromPath(value);
            _handleNullFieldsForLoadData(value);
            var action = new LoadDataAction();
            action.Rel = rel;
            action.Value = value;

            return (LoadDataActionResult)runAction(action, isReadOnly: false);
        }

        private static string typeToString(Type tp)
        {
            var str = tp.ToString();
            return tp.Name;
        }

        public ImportActionResult loadEDB(
            string relName, AnyValue[][] columns
        )
        {
            return loadEDB(relName, Relation.toCollection(columns));
        }

        public ImportActionResult loadEDB(
            string relName, ICollection<ICollection<AnyValue>> columns
        )
        {
            var rel = new Relation();
            rel.Rel_key = new RelKey();
            rel.Rel_key.Name = relName;
            rel.Rel_key.Keys = new List<string>();
            rel.Rel_key.Values = new List<string>();
            if( columns != null && columns.Count > 0 && columns.First().Count > 0) {
                Debug.Assert(columns.All(col => col.Count == columns.First().Count));
                foreach(var col in columns) {
                    rel.Rel_key.Keys.Add(typeToString(col.First().GetType()));
                }
            }

            rel.Columns = columns;
            return loadEDB(rel);
        }

        public ImportActionResult loadEDB(
            RelKey relKey, ICollection<ICollection<AnyValue>> columns
        )
        {
            var rel = new Relation();
            rel.Rel_key = relKey;
            rel.Columns = columns;
            return loadEDB(rel);
        }
        public ImportActionResult loadEDB(
            Relation value
        )
        {
            return loadEDB( new List<Relation>() { value } );
        }
        public ImportActionResult loadEDB(
            ICollection<Relation> value
        )
        {
            var action = new ImportAction();
            action.Inputs = value;

            return (ImportActionResult)runAction(action, isReadOnly: false);
        }

        public LoadDataActionResult loadCSV(
            string rel,
            string data = null,
            string path = null,
            AnyValue key = null,
            FileSyntax fileSyntax = null,
            FileSchema fileSchema = null
        )
        {
            return loadEDB(rel, CSV_CONTENT_TYPE, data, path, key, fileSyntax, fileSchema);
        }

        public LoadDataActionResult loadJSON(
            string rel,
            string data = null,
            string path = null,
            AnyValue key = null
        )
        {
            return loadEDB(rel, JSON_CONTENT_TYPE, data, path, key, new JSONFileSyntax(), new JSONFileSchema());
        }

        public ICollection<RelKey> listEdb()
        {
            var action = new ListEdbAction();
            var actionRes = (ListEdbActionResult)runAction(action, isReadOnly: true);
            return actionRes.Rels;
        }

        public ICollection<RelKey> listEdb(string relName)
        {
            var action = new ListEdbAction();
            action.Relname = relName;
            var actionRes = (ListEdbActionResult)runAction(action, isReadOnly: true);
            return actionRes.Rels;
        }

        public ICollection<RelKey> deleteEdb(string relName)
        {
            var action = new ModifyWorkspaceAction();
            action.Delete_edb = relName;
            var actionRes = (ModifyWorkspaceActionResult)runAction(action);
            return actionRes.Delete_edb_result;
        }

        public ModifyWorkspaceActionResult enableLibrary(string srcName)
        {
            var action = new ModifyWorkspaceAction();
            action.Enable_library = srcName;
            return (ModifyWorkspaceActionResult)runAction(action);
        }

        public CardinalityActionResult cardinality()
        {
            var action = new CardinalityAction();
            return (CardinalityActionResult)runAction(action, isReadOnly: true);
        }

        public CardinalityActionResult cardinality(string relName)
        {
            var action = new CardinalityAction();
            action.Relname = relName;
            return (CardinalityActionResult)runAction(action, isReadOnly: true);
        }

        public ICollection<AbstractProblem> collectProblems()
        {
            var action = new CollectProblemsAction();
            var actionRes = (CollectProblemsActionResult)runAction(action, isReadOnly: true);
            return actionRes.Problems;
        }

        public SetOptionsActionResult configure(
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
            return (SetOptionsActionResult)runAction(action, isReadOnly: false);
        }
    }
}
