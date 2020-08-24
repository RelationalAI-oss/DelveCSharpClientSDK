using NUnit.Framework;

namespace Com.RelationalAI
{
    public class IntegrationTestsCommons
    {
        public static void Test1(DelveClient api, Connection conn)
        {
            Assert.IsTrue(api.createDatabase(conn, true));
            Assert.IsFalse(api.createDatabase(conn, false));

            InstallActionResult sourceInstall = api.installSource(conn, "name", "name", "def foo = 1");
            Assert.IsNotNull(sourceInstall);

            QueryActionResult queryRes = api.query(
                conn,
                srcStr: "def bar = 2",
                output: "bar"
            );
            Assert.IsNotNull(queryRes);
        }
    }
}
