using System;
using NUnit.Framework;

namespace Com.RelationalAI
{
    public class IntegrationTestsCommons
    {
        public static Random rnd = new Random();
        public static string genDbname(string prefix="") {
            return string.Format("{0}-{1}", prefix, Math.Abs(rnd.Next()));
        }

        public delegate void ConnFunc(out DelveClient api);
        public static void Test1(ConnFunc connFunc)
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
            // connFunc(out api);
            // api.createDatabase();
            // ModifyWorkspaceActionResult deleteSrcRes = api.deleteSource("stdlib");

            // query
            // =============================================================================
            QueryActionResult queryRes = api.query(
                srcStr: "def bar = 2",
                output: "bar"
            );
            Assert.IsNotNull(queryRes);

        }
    }
}
