# NetflixBot
# Created by: Alan Wright
# wrightlyso.com
# github.com/alanwright

import praw
import ConfigParser
import os
import urllib2
import datetime
from NetflixRoulette import *
from os.path import sys
from collections import deque 
from time import sleep

NETFLIX_PREFIX = 'http://netflix.com/WiMovie/'
API_LIMIT = 25
SLEEP_TIME = 30

def main():
	print('NetflixBot v1.1 by u/yoalan')
	
	#Check for config file
	if not os.path.exists('config.cfg'):
		print('No config file.')
		sys.exit()

	#Fetch config settings
	config = ConfigParser.ConfigParser()
	config.read('config.cfg')
	USERNAME  = config.get('Configuration', 'Username')
	PASSWORD  = config.get('Configuration', 'Password')
	USERAGENT = config.get('Configuration', 'Useragent')
	SUBREDDITS = config.get('Configuration', 'Subreddit').split(',')
	
	if str(SUBREDDITS[0]) != 'all':
		combined_subs = ('%s') % '+'.join(SUBREDDITS)
		print('Looking at the following subreddits: "' + combined_subs + '".')
	else:
		comments = praw.helpers.comment_stream(r, 'all', limit=None)
		print('Looking at r/all.')

	#login to reddit
	r = praw.Reddit(USERAGENT)
	r.login(USERNAME,PASSWORD)

	#Set up our cache and completed work set
	cache = deque(maxlen=200) # double-ended queue
	already_done = set()

	running = True
	while running:    
		try:
			if str(SUBREDDITS[0]) != 'all':
				subs = r.get_subreddit(combined_subs)
				comments = subs.get_comments()

			#Check comments 
			for c in comments:
				
				#Did we recently check it? If so fetch new comments
				if c.id in cache:
					break
				
				#Add this to our cache
				cache.append(c.id)
				
				#Check if we need to reply
				if check_comment(c):
					
					#Check if we already replied
					for reply in c.replies:
						if reply.author.name == USERNAME:
							already_done.add(c.id)
					
					if c.id not in already_done:
						#comment
						movie_titles = parse_movies(c)
						replyto(c, movie_titles, already_done)

		except KeyboardInterrupt:
			running = False
		except Exception as e:
			now = datetime.datetime.now()
			print now.strftime("%m-%d-%Y %H:%M")
			print 'ERROR:', e
			print 'Going to sleep for 30 seconds...\n'
			sleep(SLEEP_TIME)
			continue

#checks a comment for required text
def check_comment(comment):
		text = comment.body    
		if '/u/netflixbot' in text.lower():
			return True
		return False

#returns a list of movie titles that were in quotes in the text string
def find_quoted_titles(text):
	start = -1
	end = -1
	quotes = []
	for i in range(0, len(text), 1):
		if text[i] == '"' and start == -1:
			start = i + 1
		elif text[i] == '"' and start != -1:
			end = i
		if start != -1 and end != -1:
			quotes.append(text[start:end])
			start = -1
			end = -1
	return quotes

#parses movie titles
def parse_movies(comment):
	#get the movie list
	text = comment.body.lower()
	begin = text.find('/u/netflixbot ') + len('/u/netflixbot ')
	text = text[begin:]

	if '"' in text:
		splitText = text.split('"')
		quotes = find_quoted_titles(text)

		#Extract the quoted titles and split the others by ','
		res = []
		for item in splitText:

			#needs to be split
			if item != ' ' and item != '' and not(item in quotes):
				tmp = item.split(',')
				for title in tmp:
					if title != ' ' and title != '':
						res.append(title)
			#quoted title
			elif item != ' ' and item != '':
				res.append(item)
	else:
		res = text.split(',')

	#Remove leading and trailing spaces
	for i in range(0, len(res), 1):
		title = res[i]
		if title[0] == ' ':
			res[i] = title[1:]
			title = res[i]
		if title[len(title) - 1] == ' ':
			res[i] = title[0:len(title) - 1]

	return res

#converts the unicode dictionary from JSON							
def convert(input):
    if isinstance(input, dict):
        return {convert(key): convert(value) for key, value in input.iteritems()}
    elif isinstance(input, list):
        return [convert(element) for element in input]
    elif isinstance(input, unicode):
        return input.encode('utf-8')
    else:
        return input

#replies to given comment
def replyto(c, movie_titles, done):
	
	text = ''
	flag = False
	movie_titles = movie_titles[0:API_LIMIT] #We can't spam API calls
	for movie in movie_titles:
		
		try:
			#Check for the last movie in a list
			if '\n' in movie:
				flag = True
				movie = movie[0:movie.find('\n')]
			
			#Fetch data and create output string
			data = convert(get_all_data(movie))
			text += ('* ' + data['show_title'] + ' (' + str(data['release_year']) + ') [is available on Netflix!](' +  NETFLIX_PREFIX + str(data['show_id']) +
					') It has a ' + str(data['rating']) + ' rating out of 5.\n')

			if flag:
				break

		except urllib2.HTTPError, err:
			# 400 error code generally means invalid query structure
			if err.code == 400:
				print 'Something went wrong with the query:'
				print movie
				print data
			text += '* ' + movie + ' is not available on Netflix :(\n'

	text += '\n\n[How to use NetflixBot.](https://github.com/alanwright/NetflixBot/ReadMe.md)\n\n'
	text += '*Note: Titles must match exactly.*\n\n'
	text += "PM for Feedback | [Source Code](https://github.com/alanwright/NetflixBot) | This bot uses the [NetflixRouletteAPI](http://netflixroulette.net/api/)"
	
	now = datetime.datetime.now()
	print (len(done) + 1), 'ID:', c.id, 'Author:', c.author.name, 'r/' + str(c.subreddit.display_name), 'Title:', c.submission.title
	print now.strftime("%m-%d-%Y %H:%M"), '\n'
	c.reply(text)
	done.add(c.id)
	
#call main function
main()