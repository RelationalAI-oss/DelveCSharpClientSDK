using System;
using NUnit.Framework;

namespace Com.RelationalAI
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var api = new DelveClient();
            //TransactionResult res = client.TransactionAsync(new Transaction()).Result;
            //Console.WriteLine(res);
            string dbname = "testjavaclient";
            Connection conn = new Connection(dbname);
            Assert.IsTrue(api.create_database(conn, true));
            Assert.IsFalse(api.create_database(conn, false));


            InstallActionResult source_install = api.install_source(conn, "name", "name", "def foo = 1");
            Assert.IsNotNull(source_install);
            Console.WriteLine("----------------");
            Console.WriteLine(source_install);
            Console.WriteLine("----------------");
            
            QueryActionResult query_res = api.query(conn, "name", "name", "def bar = 2", "bar");
            Console.WriteLine("----------------");
            Console.WriteLine(query_res);
            Console.WriteLine("----------------");
        }
    }
}
