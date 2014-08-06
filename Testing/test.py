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
from enum import Enum

NETFLIX_PREFIX = 'http://netflix.com/WiMovie/'
API_LIMIT = 25
SLEEP_TIME = 30
QUERY = Enum('QUERY', 'actor director movie')

def main():
	c = "/u/netflixbot Actor: David Bowie \n\n/u/netflixbot Actor: Tom Hiddleston\n\n/u/netflixbot Actor: Arnold Schwarzenegger "
	already_done = set()
	bodysplit = c.lower().split('\n\n')
	if len(bodysplit) <= 25:
		text = ''
		for line in bodysplit:
			if check_comment(line):
				if 'director:' in line.lower():
					director = parse_name(line, 'director:')
					text += build_reply(director, QUERY['director'])
				elif 'actor:' in line.lower():
					actor = parse_name(line, 'actor:')
					text += build_reply(actor, QUERY['actor'])
				else:
					movie_titles = parse_movies(line)
					text += build_reply(movie_titles, QUERY['movie'])

		text = add_signature(text)
		replyto(c, text, already_done)


#checks a comment for required text
def check_comment(comment):
		text = comment  
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

#Parses the actor or director name
def parse_name(comment, phrase):
	#get the movie list
	text = comment.lower()
	begin = text.find('/u/netflixbot '+ phrase) + len('/u/netflixbot ' + phrase)
	text = text[begin:]

	#trip leading/trailing spaces
	if text[0] == ' ':
		text = text[1:]
	if text[len(text)-1] == ' ':
		text = text[0:len(text) - 1]

	print text
	return text

#parses movie titles
def parse_movies(comment):
	#get the movie list
	text = comment.lower()
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

#builds reply text
def build_reply(input, type):
	text = ''
	flag = False

	#Do a movie list query
	if type == QUERY['movie']:
		movie_titles = input[0:API_LIMIT] #We can't spam API calls
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
				text += '* ' + fix_caps(movie) + ' is not available on Netflix :(\n'

	#Do an actor query
	elif type == QUERY['actor']:

		#check for newline if there is text after the call
		newline = input.find('\n')
		if newline != -1:
			actor = input[0:input.find('\n')]
		else:
			actor = input
		actor = fix_caps(actor)

		try:
			#Fetch data and create output string
			data = convert(get_all_data_actor(actor))
			for entry in data:
				text += ('* ' + entry['show_title'] + ' (' + str(entry['release_year']) + ') starring ' + actor + ' [is available on Netflix!](' +  NETFLIX_PREFIX + str(entry['show_id']) +
						') It has a ' + str(entry['rating']) + ' rating out of 5.\n')

		except urllib2.HTTPError, err:
			# 400 error code generally means invalid query structure
			if err.code == 400:
				print 'Something went wrong with the query:'
				print actor
			text += '* ' + actor + ' has no movies streaming on Netflix :(\n';

	#Do a Director query
	elif type == QUERY['director']:

		#check for newline
		newline = input.find('\n')
		if newline != -1:
			director = input[0:input.find('\n')]
		else:
			director = input
		director = fix_caps(director)
		
		try:
			#Fetch data and create output string
			data = convert(get_all_data_director(director))
			for entry in data:
				text += ('* ' + entry['show_title'] + ' (' + str(entry['release_year']) + ') directed by ' + director + ' [is available on Netflix!](' +  NETFLIX_PREFIX + str(entry['show_id']) +
						') It has a ' + str(entry['rating']) + ' rating out of 5.\n')

		except urllib2.HTTPError, err:
			# 400 error code generally means invalid query structure
			if err.code == 400:
				print 'Something went wrong with the query:'
				print director
			text += '* ' + director + ' has no movies streaming on Netflix :(\n';

	text += '\n\n' + ('_' * 25) + '\n'
	return text

#fix capitlization
def fix_caps(string):
	ans = string[0].upper()
	for i in range(1, len(string), 1):
		if string[i-1] == ' ':
			ans += string[i].upper()
		else:
			ans += string[i]
	return ans


def add_signature(text):
	text += '[How to use NetflixBot.](https://github.com/alanwright/NetflixBot/blob/master/ReadMe.md)\n\n'
	text += '*Note: Titles or names must match exactly, but capatilization does not matter.*\n\n'
	text += "PM for Feedback | [Source Code](https://github.com/alanwright/NetflixBot) | This bot uses the [NetflixRouletteAPI](http://netflixroulette.net/api/)"
	return text

#replies to given comment
def replyto(c, text, done):
	now = datetime.datetime.now()
	#print (len(done) + 1), 'ID:', c.id, 'Author:', c.author.name, 'r/' + str(c.subreddit.display_name), 'Title:', c.submission.title
	print now.strftime("%m-%d-%Y %H:%M"), '\n'
	print text
	#c.reply(text)
	done.add(c)
	
#call main function
main()