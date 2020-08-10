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
            var client = new DelveClient();
            TransactionResult res = client.TransactionAsync(new Transaction()).Result;
            Console.WriteLine(res);
        }
    }
}
