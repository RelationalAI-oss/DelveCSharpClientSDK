using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Com.RelationalAI
{
    using AnyValue = Object;
    public class IntegrationTestsCommons
    {
        public static string DictionaryToString<K,V>(IDictionary < K, V > dictionary) {
            string dictionaryString = "{";
            foreach(KeyValuePair < K, V > keyValues in dictionary) {
                dictionaryString += keyValues.Key + " : " + keyValues.Value + ", ";
            }
            return dictionaryString.TrimEnd(',', ' ') + "}";
        }

        public static Random rnd = new Random();
        public static string genDbname(string prefix="") {
            return string.Format("{0}-{1}", prefix, Math.Abs(rnd.Next()));
        }

        private static AnyValue[][] ToRelData(params AnyValue[] vals) {
            return Relation.ToRelData(vals);
        }

        private static void queryResEquals(IDictionary<RelKey, Relation> queryRes, AnyValue[][] expectedRes)
        {
            Assert.IsNotNull(queryRes);
            if ( expectedRes[0].Count() == 0 )
            {
                Assert.AreEqual(0, queryRes.Count);
            }
            else
            {
                Assert.AreEqual(1, queryRes.Count);
                Assert.AreEqual(queryRes.First().Value, expectedRes);
            }
        }

        public static void testInstallSource(LocalConnection conn, string name, bool installSourceRes)
        {
            Assert.True(installSourceRes);
            Assert.IsEmpty(conn.CollectProblems());
            Assert.True(conn.ListSource().ContainsKey(name));
        }
        private static void testInstallSource(LocalConnection conn, String name, String srcStr)
        {
            testInstallSource(conn, name, conn.InstallSource(name, srcStr));
        }
        private static void testInstallSource(LocalConnection conn, String name, String path, String srcStr)
        {
            testInstallSource(conn, name, conn.InstallSource(name, path, srcStr));
        }
        private static void testInstallSource(LocalConnection conn, Source src, string name=null)
        {
            testInstallSource(conn, name == null ? src.Name : name, conn.InstallSource(src));
        }

        public delegate LocalConnection ConnFunc();
        public static void RunAllTests(ConnFunc connFunc)
        {
            var conn = connFunc();

            // create_database
            // =============================================================================
            Assert.IsTrue(conn.CreateDatabase(overwrite: true));
            Assert.Throws<AggregateException>(() => conn.CreateDatabase());

            // install_source
            // =============================================================================
            testInstallSource(conn, "name", "def foo = 1");


            var src3 = new Source("name", "def foo = ");
            Assert.True(conn.InstallSource(src3));
            Assert.AreEqual(conn.CollectProblems().Count, 2);

            testInstallSource(conn, "name", "def foo = 1");

            try {
                System.IO.File.WriteAllText(@"test.delve", "def foo = 1");

                var src4 = new Source(@"test.delve");
                testInstallSource(conn, src4, "test");


                var src5 = new Source(src4.Path);
                src5.Name = "not_" + src4.Name;
                testInstallSource(conn, src5);
            } finally {
                System.IO.File.Delete(@"test.delve");
            }

            // delete_source
            // =============================================================================
            conn = connFunc();
            conn.CreateDatabase(overwrite: true);
            Assert.True(conn.DeleteSource("stdlib"));

            // list_source
            // =============================================================================
            conn = connFunc();
            conn.CreateDatabase(overwrite: true);
            Assert.True(new HashSet<string>() { "intrinsics", "stdlib", "ml" }.SetEquals(conn.ListSource().Keys));

            // query
            // =============================================================================
            conn = connFunc();
            conn.CreateDatabase(overwrite: true);

            queryResEquals(conn.Query(
                srcStr: "def bar = 2",
                output: "bar"
            ), ToRelData( 2L ));

            queryResEquals(conn.Query(
                srcStr: "def p = {(1,); (2,); (3,)}",
                output: "p"
            ), ToRelData( 1L, 2L, 3L ));

            queryResEquals(conn.Query(
                srcStr: "def p = {(1.1,); (2.2,); (3.4,)}",
                output: "p"
            ), ToRelData( 1.1D, 2.2D, 3.4D ));

            queryResEquals(conn.Query(
                srcStr: "def p = {(parse_decimal[64, 2, \"1.1\"],); (parse_decimal[64, 2, \"2.2\"],); (parse_decimal[64, 2, \"3.4\"],)}",
                output: "p"
            ), ToRelData( 1.1D, 2.2D, 3.4D ));

            queryResEquals(conn.Query(
                srcStr: "def p = {(1, 5); (2, 7); (3, 9)}",
                output: "p"
            ), new AnyValue[][] { new AnyValue[] { 1L, 2L, 3L }, new AnyValue[] { 5L, 7L, 9L } });

            // branch_database
            // =============================================================================

            conn = connFunc();
            var conn2 = connFunc();

            conn.CreateDatabase(overwrite: true);
            testInstallSource(conn, "name", "def x = {(1,); (2,); (3,)}");
            queryResEquals(conn.Query(output: "x"), ToRelData( 1L, 2L, 3L ));

            // Branch from conn to conn2
            if(!(conn2 is CloudConnection))
            {
                //Not supported by CloudConnection, yet
                conn2.BranchDatabase(conn.DbName);
                queryResEquals(conn2.Query(output: "x"), ToRelData( 1L, 2L, 3L ));

                testInstallSource(conn, "name", "def x = {(1,); (2,); (3,); (4,)}");
                queryResEquals(conn.Query(output: "x"), ToRelData( 1L, 2L, 3L, 4L ));
                queryResEquals(conn2.Query(output: "x"), ToRelData( 1L, 2L, 3L ));

                testInstallSource(conn2, "name", "def x = {(1,); (2,); (3,); (4,); (5,)}");
                queryResEquals(conn2.Query(output: "x"), ToRelData( 1L, 2L, 3L, 4L, 5L ));
                queryResEquals(conn.Query(output: "x"), ToRelData( 1L, 2L, 3L, 4L ));
            }

            // update_edb
            // =============================================================================
            conn = connFunc();
            conn.CreateDatabase(overwrite: true);
            conn.LoadEdb("p", ToRelData( 1L, 2L, 3L ));
            var pQuery = conn.Query(output: "p");
            queryResEquals(pQuery, ToRelData( 1L, 2L, 3L ));

            var pRelKey = pQuery.First().Key;

            conn.UpdateEdb(pRelKey, updates: new List<Tuple<AnyValue, AnyValue>>() {
                new Tuple<AnyValue, AnyValue>(new int[] { 1 } , +1),
                new Tuple<AnyValue, AnyValue>(new int[] { 3 } , -1),
                new Tuple<AnyValue, AnyValue>(new int[] { 8 } , -1),
                new Tuple<AnyValue, AnyValue>(new int[] { 4 } , +1),
                new Tuple<AnyValue, AnyValue>(new int[] { 4 } , -1),
                new Tuple<AnyValue, AnyValue>(new int[] { 0 } , -1),
                new Tuple<AnyValue, AnyValue>(new int[] { 5 } , +1),
            });

            queryResEquals(conn.Query(output: "p"), ToRelData( 1L, 2L, 5L ));

            // load_csv
            // =============================================================================
            conn = connFunc();
            conn.CreateDatabase(overwrite: true);
            Assert.True(conn.LoadCSV("csv",
                schema: new CSVFileSchema("Int64", "Int64", "Int64"),
                data: @"
                    A,B,C
                    1,2,3
                    4,5,6
                "
            ));
            queryResEquals(conn.Query(
                srcStr: "def result = count[pos: csv[pos, :A]]",
                output: "result"
            ), ToRelData( 2L ));

            Assert.True(conn.LoadCSV("bar",
                syntax: new CSVFileSyntax(delim: "|"),
                schema: new CSVFileSchema("Int64", "Int64", "Int64"),
                data: @"
                    D|E|F
                    1|2|3
                    1|2|3
                    1|2|3
                    1|2|3
                "
            ));
            queryResEquals(conn.Query(
                srcStr: "def result = count[pos: bar[pos, :D]]",
                output: "result"
            ), ToRelData( 4L ));

            // load_json
            // =============================================================================
            conn = connFunc();
            conn.CreateDatabase(overwrite: true);
            Assert.True(conn.LoadJSON("json",
                data: @"
                    { ""address"": { ""city"": ""Vancouver"", ""state"": ""BC"" } }
                "
            ));
            Assert.AreEqual(conn.ListEdb().Count, 2);
            queryResEquals(conn.Query(
                srcStr: @"
                    def cityRes(x) = exists(pos: json(:address, :city, x))
                ",
                output: "cityRes"
            ), ToRelData( "Vancouver" ));

            Assert.True(conn.LoadJSON("json",
                data: @"
                    { ""name"": ""Martin"", ""height"": 185.5 }
                "
            ));
            Assert.AreEqual(conn.ListEdb().Count, 4);

            // list_edb
            // =============================================================================
            conn = connFunc();
            conn.CreateDatabase(overwrite: true);
            Assert.AreEqual(conn.ListEdb().Count, 0);// list_edb

            // enable_library
            // =============================================================================
            conn = connFunc();
            conn.CreateDatabase(overwrite: true);
            Assert.True(conn.EnableLibrary("stdlib"));

            // cardinality
            // =============================================================================
            conn = connFunc();
            conn.CreateDatabase(overwrite: true);
            queryResEquals(conn.Query(
                srcStr: "def p = {(1,); (2,); (3,)}",
                persist: new List<string>() { "p" }
            ), ToRelData());
            var cardRels = conn.Cardinality("p");
            Assert.AreEqual(cardRels.Count, 1);
            var pCar = cardRels.ElementAt(0);
            Assert.AreEqual(
                pCar,
                new Relation(
                    new RelKey("p", new List<string>() { "Int64" }),
                    ToRelData( 3L )
                )
            );
            var deleteRes = conn.DeleteEdb("p");
            Assert.AreEqual(deleteRes.Count, 1);
            Assert.AreEqual(deleteRes.ElementAt(0), new RelKey("p", new List<string>() { "Int64" }));

            // Collect problems
            // =============================================================================
            conn = connFunc();
            conn.CreateDatabase(overwrite: true);
            Assert.AreEqual(conn.CollectProblems().Count, 0);
            conn.InstallSource(new Source("name", "", "def foo = "));
            Assert.AreEqual(conn.CollectProblems().Count, 2);

            // Set options
            // =============================================================================
            conn = connFunc();
            conn.CreateDatabase(overwrite: true);
            Assert.True(conn.Configure(debug: true));
            Assert.True(conn.Configure(debug: false));
        }
    }
}
