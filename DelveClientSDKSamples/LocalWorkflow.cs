using Newtonsoft.Json.Linq;
using System;
using Com.RelationalAI;

namespace DelveClientSDKSamples
{
    class LocalWorkflow
    {
        public void runLocalWorkflow()
        {
            string dbname = "localcsharpdatabase";
            Connection conn = new Connection(dbname);

            DelveClient client = new DelveClient();
            client.debugLevel = 1;

            /**
             * bool create_database(Connection conn, bool overwrite)
             */
            client.createDatabase(conn, true);

            /**
             * InstallActionResult installSource(Connection conn, String name, String path, String srcStr)
             */
            InstallActionResult sourceInstall = client.installSource(conn, "name", "name", "def foo = 1");
            Console.WriteLine("InstallActionResult: " + JObject.FromObject(sourceInstall).ToString());

            /**
             * QueryActionResult query(Connection conn, String name, String path, String srcStr, string output)
             */
            QueryActionResult queryRes = client.query(conn, srcStr: "def bar = 2", output: "bar");
            Console.WriteLine("QueryActionResult: " + JObject.FromObject(queryRes).ToString());
        }
    }
}
