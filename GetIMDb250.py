from BeautifulSoup import BeautifulSoup
import urllib2
import traceback
import ConfigParser
import re

#reverse replace: replaces the last x occurances of old with new in s
def rreplace(s, old, new, occurrence):
	li = s.rsplit(old, occurrence)
	return new.join(li)

#Fetch top 250
html = urllib2.urlopen('http://www.imdb.com/chart/top')
parsed_html = BeautifulSoup(html)
top_movies = parsed_html.body.findAll('td', 'titleColumn')

#write out
out = open('imdb250.txt', 'w');

for movie in top_movies:
	name = rreplace(rreplace(movie.text.replace('.', ' ', 1), '(', ' ', 1), ')', '', 1).encode('utf8')
	#print name
	out.write(name + '\n')