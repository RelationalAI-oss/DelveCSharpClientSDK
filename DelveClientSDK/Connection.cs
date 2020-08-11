using System;
namespace Com.RelationalAI
{

    public class Connection
    {
        public string dbname { get; }

        public Connection(string dbname)
        {
            this.dbname = dbname;
        }
    }
}
