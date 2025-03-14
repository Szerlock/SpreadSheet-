```
Author:     Andy Tran
Partner:    None
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  ATran-dev
Repo:       https://github.com/uofu-cs3500-20-fall2024/spreadsheet-ATran-dev
Date:       17-October-2024 23:10
Project:    Spreadsheet
Copyright:  CS 3500 and [Andy Tran] - This work may not be copied for use in Academic Coursework.
```

# Comments to Evaluators:
This assignment was a little tough, the way that JSON serializes was a bit strange, and everytime I had a + and serialized it, it would print
its unicode value instead. 

Something that I implemented was using "Exception" in my catch blocks for save and load in order to catch any other possible exceptions that would slip by. This was to allow
for complete testing coverage, and because any other exceptions would be thrown before it could reach that stage of load/save (ie. FormulaFormat, CircularException etc.) It was a "catch all"
case", and I was not sure if this is okay to have, or if I should be catching SPECIFIC exceptions. 

# Assignment Specific Topics
Nothing to in specific (based on the assignment instructions) to mention here

# Consulted Peers:
Nick Santos

# References:

	PS5:
	
	1. How Do I Get a List of Keys In a Dictionary - https://stackoverflow.com/questions/1276763/how-do-i-get-the-list-of-keys-in-a-dictionary

	PS6:

	1. Overloading of Indexers - https://www.geeksforgeeks.org/c-sharp-overloading-of-indexers/
	2. How to write .NET objects as JSON (serialize) - https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to
	3. How to write a Json into a file in c# - https://code-maze.com/csharp-write-json-into-a-file/
	4. How to write .Net Objects as JSON (deserialize) - https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/deserialization


