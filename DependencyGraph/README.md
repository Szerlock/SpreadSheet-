```
Author:     Andy Tran
Partner:    None
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  ATran-dev
Repo:       https://github.com/uofu-cs3500-20-fall2024/spreadsheet-ATran-dev
Date:       13-September-2024 18:20
Project:    DependencyGraph
Copyright:  CS 3500 and [Andy Tran] - This work may not be copied for use in Academic Coursework.
```

# Comments to Evaluators

This class was not too bad to implement, it just required a bit of thought to understand the Dependee and Dependent relationship during my implementation. In terms of 
comments, I kept it to a mimimum; short enough to be concise and not clutter the code, and long enough so that someone reading it could understand the code.
Essentially, the code was not too complicated, so not many in-line comments were required.

NOTE: 

It was a bit of a struggle to wrap my mind around Dependent and Dependee. My code/its logic could be completely backwards, but this is how I wrote the code
based on MY understand of the Dependency relationship.

Given (a,b), we know that A precedes B, or A -> B.
Therefore, A is a dependee of B, and B is a dependent of A.
B will go into my Dependents Map, with A as its dependee value
A will go into the Dependees Map, with B as it dependent


# Assignment Specific Topics
Nothing to in specific (based on the assignment instructions) to mention here

# Consulted Peers:
Dinali Assylbek (In different CS3500 class)
Nick Santos (In different CS3500 class)
Cameron (In Lab)

# References:

PS3 -

    1. HashSet inside a HashMap - https://bukkit.org/threads/hashset-inside-a-hashmap.109957/
    2. How to update the value stored in Dictionary in C# - https://stackoverflow.com/questions/1243717/how-to-update-the-value-stored-in-dictionary-in-c
    3. HashSet<T> Class - https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1?view=net-8.0
    4. C# Dictionary with Examples - https://www.geeksforgeeks.org/c-sharp-dictionary-with-examples/
