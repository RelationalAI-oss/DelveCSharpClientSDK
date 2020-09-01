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

        private static void queryResEquals(IDictionary<RelKey, Relation> queryRes, object[][] expectedRes)
        {
            Assert.IsNotNull(queryRes);
            Assert.AreEqual(queryRes.Count, 1);
            Assert.AreEqual(queryRes.First().Value, expectedRes);
        }

        public static void testInstallSource(DelveClient api, string name, InstallActionResult installSourceRes)
        {
            Assert.IsNotNull(installSourceRes);
            Assert.IsEmpty(api.collectProblems());
            Assert.True(api.list_source().ContainsKey(name));
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


            var src3 = new Source();
            src3.Name = "name";
            src3.Value = "def foo = ";
            InstallActionResult sourceInstall3 = api.installSource(src3);
            Assert.IsNotNull(sourceInstall3);
            Assert.AreEqual(api.collectProblems().Count, 2);

            testInstallSource(api, "name", "def foo = 1");

            try {
                System.IO.File.WriteAllText(@"test.delve", "def foo = 1");

                var src4 = new Source();
                src4.Path = @"test.delve";
                testInstallSource(api, src4, "test");


                var src5 = new Source();
                src5.Path = src4.Path;
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
            Console.WriteLine("api.list_source().Keys: " + DictionaryToString(api.list_source()));
            Assert.True(new HashSet<string>() { "intrinsics", "stdlib", "ml" }.SetEquals(api.list_source().Keys));

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
            api.loadEDB("p", toRelData( 1L, 2L, 3L ));
            queryResEquals(api.query(
                output: "p"
            ), toRelData( 1L, 2L, 3L ));
        }
    }
}
