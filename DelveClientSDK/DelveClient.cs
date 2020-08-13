using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Com.RelationalAI
{
    public partial class GeneratedDelveClient
    {
        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
        {
            //sign request here
        }
    }

    public class DelveClient : GeneratedDelveClient
    {
        public DelveClient(InterceptedHttpClient httpClient) : base(httpClient)
        {
        }

        public DelveClient() : this(new InterceptedHttpClient())
        {
        }

        public ActionResult run_action(Connection conn, String name, Action action)
        {
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

        public bool create_database(Connection conn, bool overwrite)
        {
            Transaction xact = new Transaction();
            xact.Mode = overwrite ? TransactionMode.CREATE_OVERWRITE : TransactionMode.CREATE;
            xact.Dbname = conn.dbname;
            Task<TransactionResult> responseTask = this.TransactionAsync(xact);
            TransactionResult response = responseTask.Result;

            return !response.Aborted && response.Problems.Count == 0;
        }

        public InstallActionResult install_source(Connection conn, String name, String path, String src_str)
        {
            Source src = new Source();
            src.Name = name;
            src.Path = path;
            src.Value = src_str;

            InstallAction action = new InstallAction();
            action.Sources = new List<Source>();
            action.Sources.Add(src);

            return (InstallActionResult)run_action(conn, "single", action);
        }

        public QueryActionResult query(Connection conn, String name, String path, String src_str, string output)
        {
            Source src = new Source();
            src.Name = name;
            src.Path = path;
            src.Value = src_str;

            QueryAction action = new QueryAction();
            action.Source = src;
            action.Outputs = new List<string>();
            action.Outputs.Add(output);

            return (QueryActionResult)run_action(conn, "single", action);
        }
    }
}
