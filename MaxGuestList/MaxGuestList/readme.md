# Maximum list of guests

Given a list of guests represented by integer numbers, for example `{0,1,2,3,4}` and pairs of guests collisions (guests that don’t like each other), for example `(0,1) (0,2) (2,3) (1,4)`, find the maximum list of guests that can attend the meeting. There can’t be any guests that do not like any other guest in the meeting and the list of guests should be as long as possible.
In the example given, maximum list of guests is `{0,3,4}`

There can be more than one such list, e.g.:  
Guests: `{0,1,2}`  
GuestsCollisions: `(0,1)`  
Maximum guests lists are: `{0,2}` and `{1,2}`


The represented solution uses dynamic programming to make smaller subproblems - by recursively removing one guest. If there are no guest collisions in any of subproblems then the  current guest list is maximal.
The problem can be reduced to finding max cliques in a graph.
The algorithm gives precise results but gets slow for more than 22 guests
