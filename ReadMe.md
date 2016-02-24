[NetflixBot](http://reddit.com/user/netflixbot)
===============================

##Description

NetflixBot is designed to be a bot that listens for mentions and when called upon (correctly) replies with whether a movie is currently streamable on Netflix (in the US). You can now also ask for a list of movies starring any actor or directed by any director on Netflix instant. 

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

## How to Clone and Run NetflixBot
NetflixBot was written to run in Python 2.7 and you will need the following packages:
* PRAW (Reddit Python API)
* NetflixRouletteAPI (For new features you will need [NetflixRoulettePython](http://github.com/alanwright/NetflixRoulettePython))
* enum34 for enum support

### Instructions:
* Install [Python 2.7](https://www.python.org/downloads/).
* Install [PIP](https://pip.pypa.io/en/stable/installing/).

Then run the following commands:
```
$ pip install praw

$ pip install NetflixRouletteAPI

$ pip install enum34
```

* Use the `.py' and `.pyc' files from my [extended NetflixRouletteApi Python wrapper](https://github.com/alanwright/NetflixRoulettePython).
* You will also need to edit the config.cfg file with your bots credentials. The code should now compile and run :smile:

```
$ python NetflixBot.py
```


##Additional Notes
*Why is it US only? Reddit is an international community!*

Right now the NetflixRouletteAPI only supports the US, and since I use this API, I can currently only support US accuracy. There are updates in the works to support this so stay tuned! :)

Alan Wright
