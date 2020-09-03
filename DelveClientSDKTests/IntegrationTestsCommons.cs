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

        private static AnyValue[][] toRelData(params AnyValue[] vals) {
            return new AnyValue[][] { vals };
        }

        private static void queryResEquals(IDictionary<RelKey, Relation> queryRes, AnyValue[][] expectedRes)
        {
            Assert.IsNotNull(queryRes);
            if ( expectedRes[0].Count() == 0 )
            {
                Assert.AreEqual(queryRes.Count, 0);
            }
            else
            {
                Assert.AreEqual(queryRes.Count, 1);
                Assert.AreEqual(queryRes.First().Value, expectedRes);
            }
        }

        public static void testInstallSource(DelveClient api, string name, bool installSourceRes)
        {
            Assert.True(installSourceRes);
            Assert.IsEmpty(api.collectProblems());
            Assert.True(api.listSource().ContainsKey(name));
        }
        private static void testInstallSource(DelveClient api, String name, String srcStr)
        {
            testInstallSource(api, name, api.installSource(name, srcStr));
        }
        private static void testInstallSource(DelveClient api, String name, String path, String srcStr)
        {
            testInstallSource(api, name, api.installSource(name, path, srcStr));
        }
        private static void testInstallSource(DelveClient api, Source src, string name=null)
        {
            testInstallSource(api, name == null ? src.Name : name, api.installSource(src));
        }

        public delegate void ConnFunc(out DelveClient api);
        public static void RunAllTests(ConnFunc connFunc)
        {
            DelveClient api;
            connFunc(out api);

            // create_database
            // =============================================================================
            Assert.IsTrue(api.createDatabase(true));
            Assert.IsFalse(api.createDatabase(false));

            // install_source
            // =============================================================================
            testInstallSource(api, "name", "def foo = 1");


            var src3 = new Source("name", "def foo = ");
            Assert.True(api.installSource(src3));
            Assert.AreEqual(api.collectProblems().Count, 2);

            testInstallSource(api, "name", "def foo = 1");

            try {
                System.IO.File.WriteAllText(@"test.delve", "def foo = 1");

                var src4 = new Source(@"test.delve");
                testInstallSource(api, src4, "test");


                var src5 = new Source(src4.Path);
                src5.Name = "not_" + src4.Name;
                testInstallSource(api, src5);
            } finally {
                System.IO.File.Delete(@"test.delve");
            }

            // delete_source
            // =============================================================================
            connFunc(out api);
            api.createDatabase();
            Assert.True(api.deleteSource("stdlib"));

            // list_source
            // =============================================================================
            connFunc(out api);
            api.createDatabase();
            Console.WriteLine("api.listSource().Keys: " + DictionaryToString(api.listSource()));
            Assert.True(new HashSet<string>() { "intrinsics", "stdlib", "ml" }.SetEquals(api.listSource().Keys));

            // query
            // =============================================================================
            connFunc(out api);
            api.createDatabase();

            queryResEquals(api.query(
                srcStr: "def bar = 2",
                output: "bar"
            ), toRelData( 2L ));

            queryResEquals(api.query(
                srcStr: "def p = {(1,); (2,); (3,)}",
                output: "p"
            ), toRelData( 1L, 2L, 3L ));

            queryResEquals(api.query(
                srcStr: "def p = {(1.1,); (2.2,); (3.4,)}",
                output: "p"
            ), toRelData( 1.1D, 2.2D, 3.4D ));

            queryResEquals(api.query(
                srcStr: "def p = {(parse_decimal[64, 2, \"1.1\"],); (parse_decimal[64, 2, \"2.2\"],); (parse_decimal[64, 2, \"3.4\"],)}",
                output: "p"
            ), toRelData( 1.1D, 2.2D, 3.4D ));

            queryResEquals(api.query(
                srcStr: "def p = {(1, 5); (2, 7); (3, 9)}",
                output: "p"
            ), new AnyValue[][] { new AnyValue[] { 1L, 2L, 3L }, new AnyValue[] { 5L, 7L, 9L } });

            // branch_database
            // =============================================================================

            DelveClient api2;
            connFunc(out api);
            connFunc(out api2);

            api.createDatabase();
            testInstallSource(api, "name", "def x = {(1,); (2,); (3,)}");
            queryResEquals(api.query(output: "x"), toRelData( 1L, 2L, 3L ));

            // Branch from conn to conn2
            api2.branchdatabase(api.dbname);
            queryResEquals(api2.query(output: "x"), toRelData( 1L, 2L, 3L ));

            testInstallSource(api, "name", "def x = {(1,); (2,); (3,); (4,)}");
            queryResEquals(api.query(output: "x"), toRelData( 1L, 2L, 3L, 4L ));
            queryResEquals(api2.query(output: "x"), toRelData( 1L, 2L, 3L ));

            testInstallSource(api2, "name", "def x = {(1,); (2,); (3,); (4,); (5,)}");
            queryResEquals(api2.query(output: "x"), toRelData( 1L, 2L, 3L, 4L, 5L ));
            queryResEquals(api.query(output: "x"), toRelData( 1L, 2L, 3L, 4L ));

            // update_edb
            // =============================================================================
            connFunc(out api);
            api.createDatabase();
            api.loadEdb("p", toRelData( 1L, 2L, 3L ));
            var pQuery = api.query(output: "p");
            queryResEquals(pQuery, toRelData( 1L, 2L, 3L ));

            var pRelKey = pQuery.First().Key;

            api.updateEdb(pRelKey, updates: new List<Tuple<AnyValue, AnyValue>>() {
                new Tuple<AnyValue, AnyValue>(new int[] { 1 } , +1),
                new Tuple<AnyValue, AnyValue>(new int[] { 3 } , -1),
                new Tuple<AnyValue, AnyValue>(new int[] { 8 } , -1),
                new Tuple<AnyValue, AnyValue>(new int[] { 4 } , +1),
                new Tuple<AnyValue, AnyValue>(new int[] { 4 } , -1),
                new Tuple<AnyValue, AnyValue>(new int[] { 0 } , -1),
                new Tuple<AnyValue, AnyValue>(new int[] { 5 } , +1),
            });

            queryResEquals(api.query(output: "p"), toRelData( 1L, 2L, 5L ));

            // load_csv
            // =============================================================================
            connFunc(out api);
            api.createDatabase();
            Assert.True(api.loadCSV("csv",
                schema: new CSVFileSchema("Int64", "Int64", "Int64"),
                data: @"
                    A,B,C
                    1,2,3
                    4,5,6
                "
            ));
            queryResEquals(api.query(
                srcStr: "def result = count[pos: csv[pos, :A]]",
                output: "result"
            ), toRelData( 2L ));

            Assert.True(api.loadCSV("bar",
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
            queryResEquals(api.query(
                srcStr: "def result = count[pos: bar[pos, :D]]",
                output: "result"
            ), toRelData( 4L ));

            // load_json
            // =============================================================================
            connFunc(out api);
            api.createDatabase();
            Assert.True(api.loadJSON("json",
                data: @"
                    { ""address"": { ""city"": ""Vancouver"", ""state"": ""BC"" } }
                "
            ));
            Assert.AreEqual(api.listEdb().Count, 2);
            queryResEquals(api.query(
                srcStr: @"
                    def cityRes(x) = exists(pos: json(:address, :city, x))
                ",
                output: "cityRes"
            ), toRelData( "Vancouver" ));

            Assert.True(api.loadJSON("json",
                data: @"
                    { ""name"": ""Martin"", ""height"": 185.5 }
                "
            ));
            Assert.AreEqual(api.listEdb().Count, 4);

            // list_edb
            // =============================================================================
            connFunc(out api);
            api.createDatabase();
            Assert.AreEqual(api.listEdb().Count, 0);// list_edb

            // enable_library
            // =============================================================================
            connFunc(out api);
            api.createDatabase();
            Assert.True(api.enableLibrary("stdlib"));

            // cardinality
            // =============================================================================
            connFunc(out api);
            api.createDatabase();
            queryResEquals(api.query(
                srcStr: "def p = {(1,); (2,); (3,)}",
                persist: new List<string>() { "p" }
            ), toRelData());
            var cardRels = api.cardinality("p");
            Assert.AreEqual(cardRels.Count, 1);
            var pCar = cardRels.ElementAt(0);
            Assert.AreEqual(
                pCar,
                new Relation(
                    new RelKey("p", new List<string>() { "Int64" }),
                    toRelData( 3L )
                )
            );
            var deleteRes = api.deleteEdb("p");
            Assert.AreEqual(deleteRes.Count, 1);
            Assert.AreEqual(deleteRes.ElementAt(0), new RelKey("p", new List<string>() { "Int64" }));

            // Collect problems
            // =============================================================================
            connFunc(out api);
            api.createDatabase();
            Assert.AreEqual(api.collectProblems().Count, 0);
            api.installSource(new Source("name", "", "def foo = "));
            Assert.AreEqual(api.collectProblems().Count, 2);

            // Set options
            // =============================================================================
            connFunc(out api);
            api.createDatabase();
            Assert.True(api.configure(debug: true));
            Assert.True(api.configure(debug: false));
        }
    }
}
