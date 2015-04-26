#!/usr/bin/python
import operator
fd = open( "D:\codewordcounts.txt" )
content = fd.readline()
runner=0
while(content != ""):
    splitval=float(content.split(" ")[0])
    runner=runner+splitval
    content=fd.readline()


print "Total Line Count:" + str(runner)


def Countlines(filename):
