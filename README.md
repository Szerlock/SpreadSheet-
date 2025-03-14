```
Author:     Andy Tran
Partner:    Ryan Cubas
Start Date: 29-August-2024
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  ATran-dev
Repo:       https://github.com/uofu-cs3500-20-fall2024/spreadsheet-ATran-dev
Commit Date: 29-October-2024 17:00
Solution:   Spreadsheet
Copyright:  CS 3500 and [Andy Tran, Ryan Cubas] - This work may not be copied for use in Academic Coursework.
```

# Overview of the Spreadsheet functionality

The Spreadsheet program is finished, which is displayed on a GUI.

# Time Expenditures:

    1. Assignment One:   Predicted Hours:          5        Actual Hours:   6  
    2. Assignment Two:   Predicted Hours:          12       Actual Hours:   8.5
    3. Assignment Three: Predicted Hours:          11       Actual Hours:   12
    4. Assignment Four:  Predicted Hours:          10       Actual Hours:   12.5
    5. Assignment Five:  Predicted Hours:          13       Actual Hours:   9.75
    6. Assignment Six:   Predicted Hours:          12       Actual Hours:   15
    7. Assignment Seven: Predicted Hours:          10       Actual Hours:   20
       - 12 total hours as a pair.
            These hours were spent writing the majority of the code to make the basic application run. Then the rest of the time was spent working on implementin the extra additional features.
       - Separately on the same code: Andy's hours: 3, Ryan's Hours: 7
            Andy's hours were spent on adding small tweaks and bug fixes in the code. He spend his time also adding the documentation for the code, played a part in
            cleaning up some of the code, and adding comments where seemed necessary.

            Ryan's hours were spent mainly on debugging chunks of broken code, and asking for help during professor office hours, and in CADE lab for TA hours. He spent his
            time making sure that the code works where intended and what code should belong where.

            Most of the time was spent debugging, and working on implementing the new features. We got our basic spreadsheet up and running early.


# Examples of Good Software Practice (GSP)

    PS5 --------------------------------

    1. Testing
        - An example of some good software practice that I have been performing is with my testing classes. Take for example my most recent one, SpreadSheetTests. In this testing class
        I made sure that all of my tests were well written, and easy to understand from a reader's standpoint. I made sure to test for edge cases for the three possible content types. Something
        else I do well is structure and format my tests. I section off my tests based on what the tests are trying to achieve. I have a section for replacing the content in cells, a section for catching
        exceptions, etc. This can actually be seen across all my tester classes. I also keep the documentation brief, but to the point.

    2. Helper Methods
        - I often employ the use of helper methods when I can, to prevent myself from being to repetiive and reusing the same code over and over. This can be seen especially prevalent in my 
        Formula class, where I like to use methods such as IsDouble, IsVar, IsOperator, etc. This not only makes it easier to look at and understand, it also saves me time to write all the code
        up. These helper methods help break down the complexity of my code and as a grader or reader, it should be relatively easier to understand had I not used helper methods. It 
        also just looks nice. 

    3. GitHub & Versioning
        - Frequently, I publish to Github whenever I make large changes, or when I get a chunk of code working. This is a feature that I use heavily. I tend to note exactly what changed, and what I did, whilst
        not going too deep into detail. Keeping it short and concise. At the start of new assignments, I push tags to the versions that I had finished. 


    PS6  --------------------------------

    4. Code Reuse
        - I tried to not reuse code wherever I could, and followed DRY. For example, when implementing SetContentsOfCell method, I reused the old SetCellContents methods, so 
        that I would not have to write all the computations again. I also only checked for a valid name once in said method, instead of checking for it every time I called for SetCellContents.
        Basically, I would not have to check for a valid name in three different places. Another small thing I did was I made checking a valid file name into a helper method (for saves and loads).

    5. Names
        - In terms of naming, I kept my names concise and to the point, so that when someone reads them, they could understand what it is. My helper methods are quite self explanatory in how I named them,
        so they required less comments to explain what they mean. 

    6. Commenting
        - I like to use in-line(or above line?) comments where I see fit. this not only helps break up larger code chunks, but into sections, where someone could read and understand the high level logic
        of that certain code block. This makes it easier on the eyes to read, and helps me understand what is going on as I write my code, and after I write my code, so that I understand exactly what
        each line is doing in my code.
    
    Other GSPs

    - All files are formatted
    - All files contain a header
    - Warnings and Errors are eliminated
    - All methods are documented
    - Code chunks are commented to explain their high level behavior
    - Utilizes C# libraries (HashSet, List, Dictionaries, etc.), and '
    - ReadMe are constantly up to date, in addition to the hours tracked.
    - Code Coverage is ALWAYS 100% (excluding tester classes due to unusual cases i.e "}")

# Time Management Skills

From these past assignments, I have found out that my time management and estimations have not been lining up to what I had expected. Some of my assignements that 
I assumed would be easy, turned out to take more time than I originally thought. Coincidently enough, the opposite also happens with harder assignments. I think that my 
estimations are reasonable given the assignment, but I think that how I spend my time has been a bit poor. Nonetheless, I think I have been slowly trying to get more on track
to get code done. However, I have been doing well in keeping up on tracking my work hours.

    PS7 -----------------------------------

    I believe that our time estimations are getting better. I think that the time spent to get the main GUI up and running has been pretty on point or even faster than we expected. The
    real challange was adding the extra optional features. These were the things that took longer than expected to implement. We expected around 1-2 hours on each function, but it took us perhaps 3-4 hours trying to
    get it to work. I think that this shows that we are slowly getting better when it comes to the basic requirements, but overall there is still a lot that we have to work on. 