using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetflixBot;
using System.Collections.Generic;

namespace NetflixBotUnitTests
{
    [TestClass]
    public class HelperFunctionsUnitTest
    {
        [TestMethod]
        public void TestBotIsMentionedTrue()
        {
            NetflixBotWorker n = new NetflixBotWorker();
            string test = "/u/netflixbot test test test";
            Assert.AreEqual(true, n.botIsMentioned(test));
        }

        [TestMethod]
        public void TestParseDirector()
        {
            NetflixBotWorker n = new NetflixBotWorker();
            string test = "/u/netflixbot director: quentin tarantino";
            Assert.AreEqual("quentin tarantino", n.parse_name(test, "director:"));
        }

        [TestMethod]
        public void TestParseActor()
        {
            NetflixBotWorker n = new NetflixBotWorker();
            string test = "/u/netflixbot actor: quentin tarantino";
            Assert.AreEqual("quentin tarantino", n.parse_name(test, "actor:"));
        }

        [TestMethod]
        public void TestSingleQuoteFinder()
        {
            NetflixBotWorker n = new NetflixBotWorker();
            string test = "\"Lock, Stock and Two Smoking Barrels\"";
            List<string> ans = new List<string>();
            ans.Add("Lock, Stock and Two Smoking Barrels");
            CollectionAssert.AreEqual(ans, n.find_quoted_titles(test));
        }

        [TestMethod]
        public void TestMultipleQuoteFinder()
        {
            NetflixBotWorker n = new NetflixBotWorker();
            string test = "\"Lock, Stock and Two Smoking Barrels\", hi, \"testing\"";
            List<string> ans = new List<string>();
            ans.Add("Lock, Stock and Two Smoking Barrels");
            ans.Add("testing");
            CollectionAssert.AreEqual(ans, n.find_quoted_titles(test));
        }

        [TestMethod]
        public void TestEmptyQuoteFinder()
        {
            NetflixBotWorker n = new NetflixBotWorker();
            string test = "test";
            List<string> ans = new List<string>();
            CollectionAssert.AreEqual(ans, n.find_quoted_titles(test));
        }
    }
}
