// <copyright file="DependencyGraphTests.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Andy Tran </authors>
// <date> September 12, 2024 </date>

namespace CS3500.DependencyGraphTests;

using CS3500.DependencyGraph;

/// <summary>
///   This is a test class for DependencyGraphTest and is intended
///   to contain all DependencyGraphTest Unit Tests
/// </summary>
[TestClass]
public class DependencyGraphTests
{
    /// <summary>
    /// This is a test to check for the efficiency of dependency graph, given by the TAs and professors for this assignment.
    /// It stimulates my code by adding a large amount of dependency pairs. 
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]  
    public void StressTest( )
    {
        DependencyGraph dg = new();

        // A bunch of strings that we load into a list
        const int SIZE = 200;
        string[] letters = new string[SIZE];
        for ( int i = 0; i < SIZE; i++ )
        {
            letters[i] = string.Empty + ( (char) ( 'a' + i ) );
        }

        // The correct answers 
        HashSet<string>[] dependents = new HashSet<string>[SIZE]; 
        HashSet<string>[] dependees = new HashSet<string>[SIZE];
        for ( int i = 0; i < SIZE; i++ )
        {
            dependents[i] = [];
            dependees[i] = [];
        }

        // Add a bunch of dependencies
        for ( int i = 0; i < SIZE; i++ )
        {
            for ( int j = i + 1; j < SIZE; j++ )
            {
                dg.AddDependency( letters[i], letters[j] ); // dependencies are added to our graph
                dependents[i].Add( letters[j] ); // they are mirrored in each of the HashSets
                dependees[j].Add( letters[i] ); // to ensure correctness
            }
        }

        // Remove a bunch of dependencies
        for ( int i = 0; i < SIZE; i++ )
        {
            for ( int j = i + 4; j < SIZE; j += 4 )
            {
                dg.RemoveDependency( letters[i], letters[j] ); // removing dependencies at random in the case at gaps of 4
                dependents[i].Remove( letters[j] );
                dependees[j].Remove( letters[i] ); // the result should be a mirror of the action to maintain correctness unless something goes wrong
            }
        }

        // Same random add and remove behavior the same as above but in different gaps
        // Add some back
        for ( int i = 0; i < SIZE; i++ )
        {
            for ( int j = i + 1; j < SIZE; j += 2 )
            {
                dg.AddDependency( letters[i], letters[j] );
                dependents[i].Add( letters[j] );
                dependees[j].Add( letters[i] );
            }
        }

        // Remove some more
        for ( int i = 0; i < SIZE; i += 2 )
        {
            for ( int j = i + 3; j < SIZE; j += 3 )
            {
                dg.RemoveDependency( letters[i], letters[j] );
                dependents[i].Remove( letters[j] );
                dependees[j].Remove( letters[i] );
            }
        }

        // Make sure everything is right
        // Should always be correct unless something in the add and remove dependencies was incorrect
        for ( int i = 0; i < SIZE; i++ )
        {
            Assert.IsTrue( dependents[i].SetEquals( new HashSet<string>( dg.GetDependents( letters[i] ) ) ) );
            Assert.IsTrue( dependees[i].SetEquals( new HashSet<string>( dg.GetDependees( letters[i] ) ) ) );
        }
    }

    // --- Start of PRE-TESTS ---


    /// <summary>
    /// This test is to make sure that Dependency pairs are added to our graph and the size is growing accordingly.
    /// </summary>
    [TestMethod]
    public void CheckGraphSizeTest()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        dg.AddDependency("c", "b");
        dg.AddDependency("d", "b");
        dg.AddDependency("e", "b");
        Assert.AreEqual(4, dg.Size);
        
    }
    
    /// <summary>
    /// This test is to make sure that when a Dependency pair is removed, the size decreases
    /// </summary>
    [TestMethod]
    public void RemoveDependencyTest()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("b", "a");
        dg.AddDependency("c", "a");
        dg.AddDependency("d", "a");
        dg.RemoveDependency("c", "a");
        Assert.AreEqual(dg.Size, 2);
    }

    /// <summary>
    /// This test makes sure that the first item in a pair is a dependent when assigned a dependee
    /// </summary>
    [TestMethod]
    public void HasDependentsTest_True()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        dg.AddDependency("c", "b");
        dg.AddDependency("d", "b");
        Assert.IsTrue(dg.HasDependents("a"));
    }

    /// <summary>
    /// This test makes sure that the first item in a pair is a dependee when assigned a dependent
    /// </summary>
    [TestMethod]
    public void HasDependeesTest_True()
    {
        DependencyGraph dg  = new DependencyGraph();
        dg.AddDependency("a", "b");
        dg.AddDependency("c", "b");
        Assert.IsTrue(dg.HasDependees("b"));
    }

    /// <summary>
    /// This test makes sure that the correct set of dependents for a given dependee is retrieved
    /// </summary>
    [TestMethod]
    public void GetDependentsTest()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        dg.AddDependency("c", "b");
        HashSet<string> expected = new();
        Assert.IsTrue(expected.SetEquals(dg.GetDependents("b")));

    }

    /// <summary>
    /// This test makes sure that the correct set of dependees for a given dependent is retrieved 
    /// </summary>
    [TestMethod]
    public void GetDependeesTest()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        dg.AddDependency("c", "b");
        HashSet<string> expected = new();
        expected.Add("a");
        expected.Add("c");
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("b")));
    }

    /// <summary>
    /// This test for a simple case where a dependent is correctly replace with another dependent
    /// </summary>
    [TestMethod]
    public void Simple_ReplaceDependentTest()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        HashSet<string> NewDependents = ["c"];
        dg.ReplaceDependents("b", NewDependents);
        Assert.IsTrue(NewDependents.SetEquals(dg.GetDependents("b")));

    }

    /// <summary>
    /// This test for a simple case where a dependee is correctly replaced with another dependee
    /// </summary>
    [TestMethod]
    public void Simple_ReplaceDependeesTest()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        HashSet<string> NewDependees = ["c", "d"];
        dg.ReplaceDependees("a", NewDependees);
        Assert.IsTrue(NewDependees.SetEquals(dg.GetDependees("a")));

    }

    /// <summary>
    /// This test is to check for when we try to replace a dependee that does not exist in our graph
    /// </summary>
    [TestMethod]
    public void Replace_NonExistant_DependeesTest()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        HashSet<string> NewDependees = ["c", "d"];
        dg.ReplaceDependees("x", NewDependees);
        Assert.IsFalse(NewDependees.SetEquals(dg.GetDependees("a")));
        Assert.IsTrue(NewDependees.SetEquals(dg.GetDependees("x")));

    }

    /// <summary>
    /// This test is to check for when we try to replace a dependent that does not exist in our graph
    /// </summary>
    [TestMethod]
    public void Replace_NonExistantDependentsTest()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        HashSet<string> NewDependents = ["f"];
        dg.ReplaceDependents("g", NewDependents);
        Assert.IsFalse(NewDependents.SetEquals(dg.GetDependents("b")));
        Assert.IsTrue(NewDependents.SetEquals(dg.GetDependents("g")));

    }

    /// <summary>
    /// This test is to check for when we try to retrieve a dependent that does not exist
    /// </summary>
    [TestMethod]
    public void GetDependents_NonExistant()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        Assert.IsTrue(new HashSet<string>().SetEquals(dg.GetDependents("c")));
        

    }

    /// <summary>
    /// This is a simple check for when we try to retrieve a dependee that does not exist
    /// </summary>
    [TestMethod]
    public void GetDependees_NonExistant()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        Assert.IsTrue(new HashSet<string>().SetEquals(dg.GetDependees("g")));

    }

    // ------ POST-TESTS-------------

    /// <summary>
    /// This is a test that checks that our graph correctly adds a dependency pair
    /// </summary>
    [TestMethod]
    public void AddDependencyTest()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        Assert.IsTrue(dg.Size == 1);
    }

    /// <summary>
    /// This test makes sure that similar pairs are still added to our graph as long as they are not exactly the same
    /// </summary>
    [TestMethod]
    public void AddSimilarPairsTest()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "a");
        Assert.IsTrue(dg.Size == 2);
    }

    /// <summary>
    /// This test checks that nothing happens when a duplicate pair is added.
    /// </summary>
    [TestMethod]
    public void AddDuplicatePairTest()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "b");
        Assert.IsTrue(dg.Size == 1);
    }

    /// <summary>
    /// This test makes sure that we should not have any out-of-place dependents when not assigned any
    /// </summary>
    [TestMethod]
    public void HasDependentsTest_False()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        Assert.IsFalse(dg.HasDependents("b"));
    }

    /// <summary>
    /// This test makes sure that we should not have any out-of-place dependees when not assigned any
    /// </summary>
    [TestMethod]
    public void HasDependeesTest_False()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        Assert.IsFalse(dg.HasDependees("a"));
    }

    /// <summary>
    /// This test makes sure that a dependency is removed when it should be removed
    /// </summary>
    [TestMethod]
    public void TestRemoveDependency()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        dg.RemoveDependency("b", "c");
        Assert.IsTrue(dg.Size == 1);

    }

    /// <summary>
    /// This test makes sure that when removing a non-existant dependency, nothing happens
    /// </summary>
    [TestMethod]
    public void Remove_NonExistantDependency()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        dg.RemoveDependency("x", "y");
        Assert.IsTrue(dg.Size == 1);
    }

    /// <summary>
    /// This test checks that replacing the dependees on a larger DG, functions as intended
    /// </summary>
    [TestMethod]
    public void Larger_ReplaceDependeesTest()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        dg.AddDependency("b", "d");
        dg.AddDependency("d", "d");

        HashSet<string> NewDependees = ["e", "f"];
        dg.ReplaceDependees("b", NewDependees);
        HashSet<string> expected = ["e", "f"];
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("b")));
    }

    /// <summary>
    /// This test checks that replacing the dependents on a larger DG, functions as intended
    /// </summary>
    [TestMethod]
    public void Larger_ReplaceDependentsTest()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        dg.AddDependency("b", "d");
        dg.AddDependency("d", "d");

        HashSet<string> NewDependents = ["g", "x"];
        dg.ReplaceDependents("b", NewDependents);
        HashSet<string> expected = ["g", "x"];
        Assert.IsTrue(expected.SetEquals(dg.GetDependents("b")));
    }



    // The following tests are sure to check my understanding of the concept using the example table provided in the PS3 instructions
    // It ensures that my code follows the same reasoning as expected (These tests should most likely show that my code is on the right track)

    /// <summary>
    /// This test checks that the correct dependees are retrieved given a defined table of results (in instructions)
    /// </summary>
    [TestMethod]
    public void InstructionsExample_GetDependeesTest()
    {
        DependencyGraph dg = new();
        dg.AddDependency("A3", "A1");
        dg.AddDependency("A4", "A2");
        dg.AddDependency("A3", "A2");
        dg.AddDependency("A2", "A1");
        
        HashSet<string> expected = new();
        expected.Add("A2");
        expected.Add("A3");
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("A1")));

    }

    /// <summary>
    /// This test checks that the correct dependents are retrieved given a defined table of results (in instructions)
    /// </summary>
    [TestMethod]
    public void InstructionsExample_GetDependentsTest()
    {
        DependencyGraph dg = new();
        dg.AddDependency("A3", "A1");
        dg.AddDependency("A4", "A2");
        dg.AddDependency("A3", "A2");
        dg.AddDependency("A2", "A1");

        HashSet<string> expected = new();
        expected.Add("A1");
        expected.Add("A2");
        Assert.IsTrue(expected.SetEquals(dg.GetDependents("A3")));

    }

    /// <summary>
    /// This test checks that no dependees are retrieved given a defined table of results (in instructions)
    /// </summary>
    [TestMethod]
    public void InstructionsExample_HasNoDependeesTest()
    {
        DependencyGraph dg = new();
        dg.AddDependency("A3", "A1");
        dg.AddDependency("A4", "A2");
        dg.AddDependency("A3", "A2");
        dg.AddDependency("A2", "A1");
        Assert.IsTrue(dg.HasDependees("A1"));

    }

    /// <summary>
    /// This test checks that no dependents are retrieved given a defined table of results (in instructions)
    /// </summary>
    [TestMethod]
    public void InstructionsExample_HasNoDependentsTest()
    {
        DependencyGraph dg = new();
        dg.AddDependency("A3", "A1");
        dg.AddDependency("A4", "A2");
        dg.AddDependency("A3", "A2");
        dg.AddDependency("A2", "A1");
        Assert.IsTrue(dg.HasDependents("A3"));

    }

    /// <summary>
    /// This is a stress test based heavily on the stress test code given. This test simplifies the operations,
    /// but drastically increases the size of the number of pairs in the graph.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    public void Simple_LargeSize_StressTest()
    {
        DependencyGraph dg = new DependencyGraph();

        // Define a large size for the stress test
        const int SIZE = 2000;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = string.Empty + ((char)('a' + i));
        }

        // Initialize the dependencies and dependees
        HashSet<string>[] dependents = new HashSet<string>[SIZE];
        HashSet<string>[] dependees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dependents[i] = new HashSet<string>();
            dependees[i] = new HashSet<string>();
        }

        // Add in dependency pairs
        for (int i = 0; i < SIZE - 1; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }
        
        // Stress test the Get Methods
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dependents[i].SetEquals(new HashSet<string>(dg.GetDependents(letters[i]))));
            Assert.IsTrue(dependees[i].SetEquals(new HashSet<string>(dg.GetDependees(letters[i]))));
        }
    }

    /// <summary>
    /// This is a stress test based heavily on the stress test code given. This test DOES NOT test on correctness, 
    /// rather it tests more on the TIME it takes a function to execute.
    /// <remark>
    /// This code was commented out due the AutoGrader giving me a notice that I had not enough Coverage, although it 
    /// says I have 100% coverage for both
    /// </remark>
    /// </summary>
    [TestMethod]
    [Timeout(5000)]
    public void ReplaceTime_StressTest()
    {
        DependencyGraph dg = new DependencyGraph();

        // Define a large size for the stress test
        const int SIZE = 2500;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = string.Empty + ((char)('a' + i));
        }

        // Initialize the dependencies and dependees
        HashSet<string>[] dependents = new HashSet<string>[SIZE];
        HashSet<string>[] dependees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dependents[i] = new HashSet<string>();
            dependees[i] = new HashSet<string>();
        }

        // Add in dependency pairs
        for (int i = 0; i < SIZE - 1; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        // Stress test the replace methods
        HashSet<string> emptySet = new(); // replacing it with nothing
        for (int i = 0; i < SIZE - 1; i++)
        {
            dg.ReplaceDependees(letters[i], emptySet);
        }

        for (int i = 0; i < SIZE - 1; i++)
        {
            dg.ReplaceDependees(letters[i], emptySet);
        }

        Assert.IsTrue(dg.Size != 0);
    }
}