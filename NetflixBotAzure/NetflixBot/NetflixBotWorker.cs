using Microsoft.WindowsAzure.ServiceRuntime;
using NetflixRouletteSharp;
using RedditSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace NetflixBot
{
    public class NetflixBotWorker : RoleEntryPoint
    {
        //LIMITS
        public const int REQ_LIMIT = 25;
        public const int SPLIT_LIMIT = 10;
        public const string NETFLIX_PREFIX = "http://netflix.com/WiMovie/";
        public enum Request { Director, Actor, Movie }

        public NetflixBotWorker() { }

        public override void Run()
        {
            Trace.TraceInformation("NetflixBot entry point called");

            //Login to reddit
            var reddit = new Reddit();
            var user = reddit.LogIn("netflixbot", "B4254vdcpxZ2");

            //Create cache
            ArrayList cache = new ArrayList(200);
            HashSet<string> replies = new HashSet<string>();

            //Start running
            bool running = true;
            while (running)
            {
                var subreddit = reddit.GetSubreddit("/r/movies");
                foreach(var c in subreddit.Comments.Take(200))
                {
                    //Cached comment?
                    if(isCached(c, cache))
                        break;
                    else
                        cache.Add(c.Id);
                       
                    //should we reply?
                    if(botIsMentioned(c.Body))
                    {
                        //Did we already reply?
                        if (replies.Contains(c.Id) || alreadyReplied(c))
                            break;

                        //split replies
                        string body = c.Body.ToLower();
                        string[] split_body = Regex.Split(body, "\n\n");

                        if(split_body.Length <= SPLIT_LIMIT)
                        {
                            string text = "";
                            foreach(string split in split_body)
                            {
                                if(botIsMentioned(split))
                                {
                                    //Director response
                                    if(split.Contains("director:"))
                                    {
                                        string director = parse_name(split, "director:");
										text += build_person_reply(director, Request.Director);
                                    }
                                    //Actor response
                                    else if(split.Contains("actor:"))
                                    {
                                        string actor = parse_name(split, "actor:");
                                        text += build_person_reply(actor, Request.Actor);
                                    }
                                    //Movie List response
                                    else
                                    {
                                        List<string> movie_titles = parse_movies(split);
                                        text += build_movie_reply(movie_titles);
                                    }
                                }
                            }
                            text = add_signature(text);
                            replies = reply(c, text, replies);
                        }
                    }
                }
            }
        }

        public bool isCached(RedditSharp.Things.Comment c , ArrayList cache)
        {
            foreach(var id in cache)
            {
                if (c.Id == id)
                {
                    return true;
                }
            }
            return false;
        }

        public bool alreadyReplied(RedditSharp.Things.Comment c)
        {
            foreach(var reply in c.Comments)
            {
                if(reply.Author == "netflixbot")
                {
                    return true;
                }
            }
            return false;
        }

        public bool botIsMentioned(string body)
        {
            return body.ToLower().Contains("/u/netflix");
        }

        public string parse_name(string body, string phrase)
        {
            int begin = body.IndexOf("/u/netflixbot " + phrase) + ("/u/netflixbot " + phrase).Length + 1;
	        string text = body.Substring(begin);

	        //trim leading/trailing spaces
	        if(text[0].Equals(" "))
		        text = text.Substring(1);
	        if(text[text.Length - 1].Equals(" "))
		        text = text.Substring(0, text.Length);

	        return text;
        }

        public List<string> parse_movies(string body)
        {
            int begin = body.IndexOf("/u/netflixbot ") + ("/u/netflixbot ").Length + 1;
            string text = body.Substring(begin);

            //split by quotes first
            List<string> res = new List<string>();
            if(text.Contains(","))
            {
                string[] splitText = Regex.Split(text, "\"");
		        List<string> quotes = find_quoted_titles(text);

		        //Extract the quoted titles and split the others by ','
		        foreach(string item in splitText)
                {
			        //needs to be split
                    bool inQuotes = false;
                    foreach(string q in quotes)
                    {
                        if(item == q)
                        {
                            inQuotes = true;
                            break;
                        }
                    }
			        if(item != " " && item != "" && !inQuotes)
                    {
                    	var tmp = Regex.Split(item, ",");
				        foreach(string title in tmp)
                        {
					        if(title != " " && title !="")
                            {
						        res.Add(title);
                            }
                        }
                    }
			        //quoted title
			        else if(item != " " && item != "")
                    {
                        res.Add(item);
                    }
                }
            }
            else
                res = Regex.Split(text, ",").ToList<string>();

            //Remove leading and trailing spaces
            for(int i = 0; i < res.Count; ++i)
            {
		        string title = res[i];
		        if(title[0].Equals(' '))
                {
			        res[i] = title.Substring(1);
			        title = res[i];
                }
		        if(title[title.Length - 1].Equals(' '))
			        res[i] = title.Substring(0, title.Length - 1);
            }
            return res;
        }

        public List<string> find_quoted_titles(string text)
        {
            int start = -1;
	        int end = -1;
	        List<string> quotes = new List<string>();
	        for(int i = 0; i < text.Length; ++i)
            {
		        if(text[i] == '\"' && start == -1)
                {
			        start = i + 1;
                }
		        else if(text[i] == '"' && start != -1)
                {
			        end = i;
                }
		        if(start != -1 && end != -1)
                {
                    quotes.Add(text.Substring(start, end - start));
			        start = -1;
			        end = -1;
                }
            }
	        return quotes;
        }

        public string fix_caps(string text)
        {
            string ans = "" + Char.ToUpper(text[0]);
	        for(int i = 1; i < text.Length; ++i)
            {
		        if(text[i-1] == ' ')
			        ans += Char.ToUpper(text[i]);
		        else
			        ans += text[i];
            }
	        return ans;
        }

        public string build_person_reply(string input, Request type)
        {
            string text ="";

	        //Do an actor query
            string keyword = "";
	        if(type == Request.Actor)
            {
		        keyword = " starring ";
            }
            else if(type == Request.Director)
            {
                keyword = " directed by ";
            }

	       //check for newline if there is text after the call
		    bool newline = input.Contains("\n");
            string person;
		    if(newline)
            {
			    person = input.Substring(0, input.IndexOf('\n'));
            }
		    else
            {
			    person = input;
            }
		    person = fix_caps(person);

		    try
            {

                //Fetch data and create output string
                List<RouletteResponse> list = new List<RouletteResponse>();
                if (type == Request.Director)
                {
                    list = NetflixRoulette.DirectorRequest(person);
                }
                else
                {
                    list = NetflixRoulette.ActorRequest(person);
                }
                foreach(RouletteResponse r in list)
                {
                    text += ("* " + r.ShowTitle + " (" + r.ReleaseYear + ")" + keyword + person + " [is available on Netflix!](" +  NETFLIX_PREFIX + r.ShowId +
						    ") It has a " + r.Rating + " rating out of 5.\n");
                }
            }
		    catch(RouletteRequestException e)
            {
			    text += "* " + person + " has no movies streaming on Netflix :(\n";
            }

            text += "\n\n";
            for (int i = 0; i < 25; i++)
                text += "_";
            text += "\n";
            return text;
        }

        public string build_movie_reply(List<string> movie_titles)
        {
            string text = "";

            //check for newline if there is text after the call
            foreach (string movie in movie_titles)
            {
                bool newline = movie.Contains("\n");
                string person;
                if (newline)
                {
                    person = movie.Substring(0, movie.IndexOf('\n'));
                }

                else
                {
                    person = movie;
                }
                person = fix_caps(person);

                try
                {

                    //Fetch data and create output string
                    RouletteResponse r = NetflixRoulette.TitleRequest(person);
                    text += ("* " + r.ShowTitle + " (" + r.ReleaseYear + ") [is available on Netflix!](" + NETFLIX_PREFIX + r.ShowId +
                            ") It has a " + r.Rating + " rating out of 5.\n");
                }
                catch (RouletteRequestException e)
                {
                    text += "* " + person + " has no movies streaming on Netflix :(\n";
                }
            }

            text += "\n\n";
            for (int i = 0; i < 25; i++)
                text += "_";
            text += "\n";
            return text;
        }

        public string add_signature(string text)
        {
	        text += "[How to use NetflixBot.](https://github.com/alanwright/NetflixBot/blob/master/ReadMe.md)\n\n";
	        text += "*Note: Titles or names must match exactly, but capatilization does not matter.*\n\n";
            text += "PM for Feedback | [Source Code](https://github.com/alanwright/NetflixBot) | This bot uses the [NetflixRouletteAPI](http://netflixroulette.net/api/)"
                    + " and now runs on Windows Azure!";
	        return text;
        }

        public HashSet<string> reply(RedditSharp.Things.Comment c, string text, HashSet<string> done)
        {
	        DateTime now = DateTime.Now;
	        Trace.TraceInformation("ID:" +  c.Id + "Author:" + c.Author +  "r/" + c.Subreddit + "Title: " + c.LinkTitle);
	        Trace.TraceInformation(now.ToString());
	        c.Reply(text);
	        done.Add(c.Id);
            return done;
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
