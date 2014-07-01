[NetflixBot](http://reddit.com/user/netflixbot)
===============================

##NetflixBot is currently down!
I'm currently in the process of moving across the country (literally, from FL to WA) so NetflixBot will be down. Once I receive my Raspberry Pi at my temporary housing from the movers, I will have NetflixBot up and running again. I appreciate your patience!

##Description

NetflixBot is designed to be a bot that listens to specific movie subreddits and when called upon (correctly) replies with whether a movie is currently streamable on Netflix (in the US). You can now also ask for a list of movies starring any actor or directed by any director on Netflix instant. 

![Image of sample response](https://raw.githubusercontent.com/alanwright/NetflixBot/master/img/scrnsht.PNG)

## How to Call NetflixBot
Calling NetflixBot is simple, but if you enter an incorrect character (as of now) it may return inaccurate information, OR may not recognize a call. The proper call syntax is:

### Movies

For a single movie:

```
/u/netflixbot movie1
```

For a list of movies:

```
/u/netflixbot movie1,movie2,movie3
```

OR...

```
/u/netflixbot movie1, movie2, movie3
```

If a title has comma in it, you should specify the title in quotes:

```
/u/netflixbot Breaking Bad, "Lock, Stock and Two Smoking Barrels", Don Jon
```

### Directors

For a list of movies directed by a director:

```
/u/netflixbot Director: Quentin Tarantino
```

### Actors

For a list of movies starring an actor:

```
/u/netflixbot Actor: Bruce Willis
```

### Making Multiple Calls

To make multiple calls (of any type) in one comment:

```
/u/netflixbot Pulp Fiction, Breaking Bad

/u/netflixbot Director: David Fincher

/u/netflixbot Actor: Ben Stiller
```

### Important Notes
* It must be "/u/netflixbot" with both '/' on all call lines
* All calls must be on their own line
* Capitlization does not matter
* Title must match exactly; this causes problems for some things like "The Office" which is "The Office (U.S.)" on Netflix
* There can be spaces between items in a list
* You can put any amount of text before the line where you call /u/netflixbot or after (careful of bullet 3)

## Current Subreddits
Currently testing on small subreddits...

* [/r/movies](http://reddit.com/r/movies)
* [/r/movieclub](http://reddit.com/r/movieclub)
* [/r/askreddit](http://reddit.com/r/askreddit)
* [/r/bottesting](http://reddit.com/r/bottesting)
* [/r/botwatch](http://reddit.com/r/botwatch)

*If you would like me to add your subreddit, please reach out to me.*

## Reddit Gold

This is my plea for [Reddit Gold](http://www.reddit.com/gold/about). If you like NetflixBot and want to donate, buy me (NetflixBot) Reddit Gold. This way I can increase the scope of the bot on Reddit. Rather than specifying subreddits, the bot will be notified when mentioned by name, and will be much more efficient and will be able to answer any comment in any subreddit! If you donate Reddit Gold, please [email me](http://wrightlyso.com/contact) and I will put a thank you in all NetflixBot comments :)

## How to Clone and Run NetflixBot
NetflixBot was written to run in Python 2.7 and you will need the following packages:
* PRAW (Reddit Python API)
* NetflixRouletteAPI
* enum34 for enum support

To install these, first install Python 2.7, then install PIP. This is simple so do the necessary searching. Then run the following commands:
```
$ pip install praw

$ pip install NetflixRouletteAPI

$ pip install enum34
```

You will also need to edit the config.cfg file with your bots credentials. The code should now compile and run :)

## How Does the Code Work or How Do I Create a Bot?

Check out my Reddit PRAW tutorial [here](http://www.wrightlyso.com/blog/reddit-api-subreddit) and my Reddit bot tutorial [here](http://www.wrightlyso.com/blog/netflixbot-tutorial).

##Additional Notes
*Why is it US only? Reddit is an international community!*

Right now the NetflixRouletteAPI only supports the US, and since I use this API, I can currently only support US accuracy. If this bot becomes popular I would like to increase its scope. 

Alan Wright

[WrightlySo](www.wrightlyso.com)
