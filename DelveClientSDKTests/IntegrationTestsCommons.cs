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
            InstallActionResult sourceInstall1 = api.installSource("name", "name", "def foo = 1");
            Assert.IsNotNull(sourceInstall1);
            Assert.IsEmpty(api.collectProblems());


            var src3 = new Source();
            src3.Name = "name";
            src3.Value = "def foo = ";
            InstallActionResult sourceInstall3 = api.installSource(src3);
            Assert.IsNotNull(sourceInstall3);
            Assert.AreEqual(api.collectProblems().Count, 2);


            var src2 = new Source();
            src2.Name = "name";
            src2.Value = "def foo = 1";
            InstallActionResult sourceInstall2 = api.installSource(src2);
            Assert.IsNotNull(sourceInstall2);
            Assert.IsEmpty(api.collectProblems());

            try {
                System.IO.File.WriteAllText(@"test.delve", "def foo = 1");

                var src4 = new Source();
                src4.Path = @"test.delve";
                InstallActionResult sourceInstall4 = api.installSource(src4);
                Assert.IsNotNull(sourceInstall4);
                Assert.IsEmpty(api.collectProblems());
                Assert.True(api.list_source().ContainsKey("test"));


                var src5 = new Source();
                src5.Path = src4.Path;
                src5.Name = "not_" + src4.Name;
                InstallActionResult sourceInstall5 = api.installSource(src5);
                Assert.IsNotNull(sourceInstall5);
                Assert.IsEmpty(api.collectProblems());
                Assert.True(api.list_source().ContainsKey("not_" + src4.Name));
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
