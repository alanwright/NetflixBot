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
            Assert.AreEqual(true, n.BotIsMentioned(test));
        }

        [TestMethod]
        public void TestParseMovies()
        {
            NetflixBotWorker n = new NetflixBotWorker();
            string test = "/u/netflixbot test1, test with space2, test3";
            List<string> ans = new List<string>();
            ans.Add("test1");
            ans.Add("test with space2");
            ans.Add("test3");
            List<string> temp = n.parse_movies(test);
            CollectionAssert.AreEqual(ans, n.parse_movies(test));
        }

        [TestMethod]
        public void TestParseActorName()
        {
            NetflixBotWorker n = new NetflixBotWorker();
            string test = "/u/netflixbot Actor: testing";
            string ans = "testing";
            Assert.AreEqual(ans, n.ParseName(test, "Actor:"));
        }

        [TestMethod]
        public void TestParseDirector()
        {
            NetflixBotWorker n = new NetflixBotWorker();
            string test = "/u/netflixbot director: quentin tarantino";
            Assert.AreEqual("quentin tarantino", n.ParseName(test, "director:"));
        }

        [TestMethod]
        public void TestParseActor()
        {
            NetflixBotWorker n = new NetflixBotWorker();
            string test = "/u/netflixbot actor: quentin tarantino";
            Assert.AreEqual("quentin tarantino", n.ParseName(test, "actor:"));
        }

        [TestMethod]
        public void TestSingleQuoteFinder()
        {
            NetflixBotWorker n = new NetflixBotWorker();
            string test = "\"Lock, Stock and Two Smoking Barrels\"";
            List<string> ans = new List<string>();
            ans.Add("Lock, Stock and Two Smoking Barrels");
            CollectionAssert.AreEqual(ans, n.FindQuotedTitles(test));
        }

        [TestMethod]
        public void TestMultipleQuoteFinder()
        {
            NetflixBotWorker n = new NetflixBotWorker();
            string test = "\"Lock, Stock and Two Smoking Barrels\", hi, \"testing\"";
            List<string> ans = new List<string>();
            ans.Add("Lock, Stock and Two Smoking Barrels");
            ans.Add("testing");
            CollectionAssert.AreEqual(ans, n.FindQuotedTitles(test));
        }

        [TestMethod]
        public void TestEmptyQuoteFinder()
        {
            NetflixBotWorker n = new NetflixBotWorker();
            string test = "test";
            List<string> ans = new List<string>();
            CollectionAssert.AreEqual(ans, n.FindQuotedTitles(test));
        }

        [TestMethod]
        public void TestMovieListComment()
        {
            NetflixBotWorker n = new NetflixBotWorker();
            List<string> test = new List<string>();
            test.Add("Pulp Fiction");
            test.Add("Star Trek");
            string ans = "* Pulp Fiction (1994) [is available on Netflix!](http://netflix.com/WiMovie/880640) It has a 4.1 rating out of 5.\n* Star Trek (1966) [is available on Netflix!](http://netflix.com/WiMovie/70136140) It has a 3.9 rating out of 5.\n\n\n_________________________\n";
            Assert.AreEqual(ans, n.BuildMovieReply(test));
        }
    }
}
