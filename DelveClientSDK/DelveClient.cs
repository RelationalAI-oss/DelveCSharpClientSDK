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
        public const string JSON_CONTENT_TYPE = "application/json";
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
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

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
        private static MultiSetComparer<string> comp = new MultiSetComparer<string>();

        public RelKey()
        {
        }

        public RelKey(string name, List<string> keyTypes = null, List<string> valueTypes = null)
        {
            this.Name = name;
            this.Keys = keyTypes == null ? new List<string>() : keyTypes;
            this.Values = valueTypes == null ? new List<string>() : valueTypes;
        }

        public override bool Equals(object obj)
        {
            return obj is RelKey key &&
                   Name.Equals(key.Name) &&
                   comp.Equals(Keys, key.Keys) &&
                   comp.Equals(Values, key.Values);
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(Name, Keys, Values);
        }
    }

    partial class Relation
    {
        public Relation()
        {
        }

        public Relation(RelKey relKey, AnyValue[][] columns) : this(relKey, toCollection(columns))
        {
        }

        public Relation(RelKey relKey, ICollection<ICollection<AnyValue>> columns) : this()
        {
            this.Rel_key = relKey;
            this.Columns = columns;
        }

        public HashSet<HashSet<AnyValue>> columnsToHashSet(ICollection<ICollection<AnyValue>> columns)
        {
            return columns.Select(col => col.ToHashSet()).ToHashSet();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if ( obj is Relation relation ) {
                return EqualityComparer<RelKey>.Default.Equals(Rel_key, relation.Rel_key) &&
                       equalsToCollection(relation.Columns);
            } else if ( obj.GetType().IsArray ) {
                AnyValue[] arr = (AnyValue[]) obj;
                if( arr.Length == 0 ) {
                    return Columns.Count == 0 || Columns.First().Count == 0;
                } else if ( arr[0].GetType().IsArray ) {
                    return equalsToArr((dynamic) arr);
                }
            }
            return false;
        }

        public static ICollection<ICollection<AnyValue>> toCollection(AnyValue[][] arr)
        {
            return arr.Select(col => (ICollection<AnyValue>)(col.Cast<AnyValue>().ToHashSet())).ToHashSet();
        }

        private bool equalsToArr(AnyValue[][] arr) {
            return equalsToCollection(toCollection(arr));
        }

        private bool equalsToCollection(ICollection<ICollection<AnyValue>> col) {
            var x1 = columnsToHashSet(Columns);
            var x2 = columnsToHashSet(col);
            return x1.All(elem1 => x2.Any(elem2 => elem1.SetEquals(elem2))) && x2.All(elem2 => x1.Any(elem1 => elem2.SetEquals(elem1)));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Rel_key, columnsToHashSet(Columns));
        }
    }

    public partial class Source
    {
        public Source()
        {
        }
        public Source(string path) : this("", path, "")
        {
        }
        public Source(string name, string value) : this(name, name, value)
        {
        }
        public Source(string name, string path, string value) : this()
        {
            Name = name;
            Path = path;
            Value = value;
        }
    }

    public partial class CSVFileSchema : FileSchema
    {
        public CSVFileSchema(params string[] types)
        {
            this.Types = types.ToList();
        }
    }

    public partial class CSVFileSyntax : FileSyntax
    {
        public CSVFileSyntax(
            ICollection<string> header = null,
            int headerRow = -1,
            bool normalizeNames = false,
            int dataRow = -1,
            ICollection<string> missingStrings = null,
            string delim = ",",
            bool ignoreRepeated = false,
            string quoteChar = "\"",
            string escapeChar = "\\"
        )
        {
            if( missingStrings == null ) missingStrings = new List<string>();

            this.Header = header;
            this.Header_row = headerRow;
            this.Normalizenames = normalizeNames;
            this.Datarow = dataRow;
            this.Missingstrings = missingStrings;
            this.Delim = delim;
            this.Ignorerepeated = ignoreRepeated;
            this.Quotechar = quoteChar;
            this.Escapechar = escapeChar;
        }
    }

    public class DelveClient : GeneratedDelveClient
    {
        private static HttpClient createHttpClient(bool verifySSL) {
            if( verifySSL ) {
                return new HttpClient();
            } else {
                // If we don't want to verify SSL certificate (from the Server), we need to
                // specifically attach a `HttpClientHandler` to `HttpClient` for accepting
                // any certificate from the server. This is useful for testing purposes, but
                // should not be used in production.
                var handler = new HttpClientHandler()
                {
                    SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
                    ServerCertificateCustomValidationCallback = AcceptAllServerCertificate
                };
                return new HttpClient(handler);
            }
        }
        public static bool AcceptAllServerCertificate(
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
            if( "https".Equals(url.Scheme) && httpClientVerifySSL != verifySSL) {
                // we keep a single static HttpClient instance and keep reusing it instead
                // of creating an instance for each request. This is a proven best practice.
                // However, if we are going to handle a `https` request and suddenly
                // decide a value for `verifySSL` other than its default value (or the value
                // used in the previous requests), then this section disposes the existing
                // HttpClient instance and creates a new one.
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

        public bool installSource(String name, String srcStr)
        {
            return installSource(name, name, srcStr);
        }
        public bool installSource(String name, String path, String srcStr)
        {
            Source src = new Source();
            src.Name = name;
            src.Path = path;
            src.Value = srcStr;

            return installSource(src);
        }
        public bool installSource(Source src)
        {
            return installSource(new List<Source>() { src });
        }

        public bool installSource(ICollection<Source> srcList)
        {
            var action = new InstallAction();
            foreach(Source src in srcList) {
                _readFileFromPath(src);
            }
            action.Sources = srcList;

            return runAction(action) != null;
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

        public IDictionary<string, Source> listSource()
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

        private static ICollection<Pair_AnyValue_AnyValue_> convertCollection(ICollection<Tuple<AnyValue, AnyValue>> data)
        {
            var res = new List<Pair_AnyValue_AnyValue_>();
            foreach(var tpl in data) {
                var pair = new Pair_AnyValue_AnyValue_();
                pair.First = tpl.Item1;
                pair.Second = tpl.Item2;
                res.Add(pair);
            }
            return res;
        }

        public void updateEdb(
            RelKey rel,
            ICollection<Tuple<AnyValue, AnyValue>> updates = null,
            ICollection<Tuple<AnyValue, AnyValue>> delta = null
        )
        {
            var action = new UpdateAction();
            action.Rel = rel;
            if(updates != null) action.Updates = convertCollection(updates);
            if(delta != null)action.Delta = convertCollection(delta);

            runAction(action, isReadOnly: false);
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

            if(!loadData.Content_type.StartsWith(JSON_CONTENT_TYPE) && !loadData.Content_type.StartsWith(CSV_CONTENT_TYPE)) {
                throw new ArgumentException(string.Format("`ContentType`={0} is not supported.", loadData.Content_type));
            }
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
            FileSyntax syntax = null,
            FileSchema schema = null
        )
        {
            var loadData = new LoadData();
            loadData.Data = data;
            loadData.Content_type = JSON_CONTENT_TYPE;
            loadData.Key = key;
            loadData.File_syntax = syntax;
            loadData.File_schema = schema;

            return loadData;
        }

        public LoadData jsonFile(
            string filePath,
            AnyValue key = null,
            FileSyntax syntax = null,
            FileSchema schema = null
        )
        {
            var loadData = new LoadData();
            loadData.Path = filePath;
            loadData.Content_type = JSON_CONTENT_TYPE;
            loadData.Key = key;
            loadData.File_syntax = syntax;
            loadData.File_schema = schema;

            return loadData;
        }

        public LoadData csvString(
            string data,
            AnyValue key = null,
            FileSyntax syntax = null,
            FileSchema schema = null
        )
        {
            var loadData = new LoadData();
            loadData.Data = data;
            loadData.Content_type = CSV_CONTENT_TYPE;
            loadData.Key = key;
            loadData.File_syntax = syntax;
            loadData.File_schema = schema;

            return loadData;
        }

        public LoadData csvFile(
            string filePath,
            AnyValue key = null,
            FileSyntax syntax = null,
            FileSchema schema = null
        )
        {
            var loadData = new LoadData();
            loadData.Path = filePath;
            loadData.Content_type = CSV_CONTENT_TYPE;
            loadData.Key = key;
            loadData.File_syntax = syntax;
            loadData.File_schema = schema;

            return loadData;
        }

        public bool loadEdb(
            string rel,
            string contentType,
            string data = null,
            string path = null,
            AnyValue key = null,
            FileSyntax syntax = null,
            FileSchema schema = null
        )
        {
            if(key == null) key = new int[] {};

            var loadData = new LoadData();
            loadData.Content_type = contentType;
            loadData.Data = data;
            loadData.Path = path;
            loadData.Key = key;
            loadData.File_syntax = syntax;
            loadData.File_schema = schema;

            return loadEdb(rel, loadData);
        }
        public bool loadEdb(
            string rel,
            LoadData value
        )
        {
            _readFileFromPath(value);
            _handleNullFieldsForLoadData(value);
            var action = new LoadDataAction();
            action.Rel = rel;
            action.Value = value;

            return runAction(action, isReadOnly: false) != null;
        }

        private static string typeToString(Type tp)
        {
            var str = tp.ToString();
            return tp.Name;
        }

        public bool loadEdb(
            string relName, AnyValue[][] columns
        )
        {
            return loadEdb(relName, Relation.toCollection(columns));
        }

        public bool loadEdb(
            string relName, ICollection<ICollection<AnyValue>> columns
        )
        {
            var rel = new Relation();
            rel.Rel_key = new RelKey(relName);
            if( columns != null && columns.Count > 0 && columns.First().Count > 0) {
                Debug.Assert(columns.All(col => col.Count == columns.First().Count));
                foreach(var col in columns) {
                    rel.Rel_key.Keys.Add(typeToString(col.First().GetType()));
                }
            }

            rel.Columns = columns;
            return loadEdb(rel);
        }

        public bool loadEdb(
            RelKey relKey, ICollection<ICollection<AnyValue>> columns
        )
        {
            var rel = new Relation();
            rel.Rel_key = relKey;
            rel.Columns = columns;
            return loadEdb(rel);
        }
        public bool loadEdb(
            Relation value
        )
        {
            return loadEdb( new List<Relation>() { value } );
        }
        public bool loadEdb(
            ICollection<Relation> value
        )
        {
            var action = new ImportAction();
            action.Inputs = value;

            return runAction(action, isReadOnly: false) != null;
        }

        public bool loadCSV(
            string rel,
            string data = null,
            string path = null,
            AnyValue key = null,
            FileSyntax syntax = null,
            FileSchema schema = null
        )
        {
            return loadEdb(rel, CSV_CONTENT_TYPE, data, path, key, syntax, schema);
        }

        public bool loadJSON(
            string rel,
            string data = null,
            string path = null,
            AnyValue key = null
        )
        {
            return loadEdb(rel, JSON_CONTENT_TYPE, data, path, key, new JSONFileSyntax(), new JSONFileSchema());
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

        public bool enableLibrary(string srcName)
        {
            var action = new ModifyWorkspaceAction();
            action.Enable_library = srcName;
            return runAction(action) != null;
        }

        public ICollection<Relation> cardinality()
        {
            var action = new CardinalityAction();
            return ((CardinalityActionResult)runAction(action, isReadOnly: true)).Result;
        }

        public ICollection<Relation> cardinality(string relName)
        {
            var action = new CardinalityAction();
            action.Relname = relName;
            return ((CardinalityActionResult)runAction(action, isReadOnly: true)).Result;
        }

        public ICollection<AbstractProblem> collectProblems()
        {
            var action = new CollectProblemsAction();
            var actionRes = (CollectProblemsActionResult)runAction(action, isReadOnly: true);
            return actionRes.Problems;
        }

        public bool configure(
            bool? debug = null,
            bool? debugTrace = null,
            bool? broken = null,
            bool? silent = null,
            bool? abortOnError = null
        )
        {
            var action = new SetOptionsAction();
            action.Debug = debug;
            action.Debug_trace = debugTrace;
            action.Broken = broken;
            action.Silent = silent;
            action.Abort_on_error = abortOnError;
            return runAction(action, isReadOnly: false) != null;
        }
    }
}
