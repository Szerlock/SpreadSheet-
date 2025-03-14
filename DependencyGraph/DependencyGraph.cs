// <copyright file="DependencyGraph.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

namespace CS3500.DependencyGraph;

/// <summary>
///   <para>
///     (s1,t1) is an ordered pair of strings, meaning t1 depends on s1.
///     (in other words: s1 must be evaluated before t1.)
///   </para>
///   <para>
///     A DependencyGraph can be modeled as a set of ordered pairs of strings.
///     Two ordered pairs (s1,t1) and (s2,t2) are considered equal if and only
///     if s1 equals s2 and t1 equals t2.
///   </para>
///   <remarks>
///     Recall that sets never contain duplicates.
///     If an attempt is made to add an element to a set, and the element is already
///     in the set, the set remains unchanged.
///   </remarks>
///   <para>
///     Given a DependencyGraph DG:
///   </para>
///   <list type="number">
///     <item>
///       If s is a string, the set of all strings t such that (s,t) is in DG is called dependentsDict(s).
///       (The set of things that depend on s.)
///     </item>
///     <item>
///       If s is a string, the set of all strings t such that (t,s) is in DG is called dependeesDict(s).
///       (The set of things that s depends on.)
///     </item>
///   </list>
///   <para>
///      For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}.
///   </para>
///   <code>
///     dependentsDict("a") = {"b", "c"}
///     dependentsDict("b") = {"d"}
///     dependentsDict("c") = {}
///     dependentsDict("d") = {"d"}
///     dependeesDict("a")  = {}
///     dependeesDict("b")  = {"a"}
///     dependeesDict("c")  = {"a"}
///     dependeesDict("d")  = {"b", "d"}
///   </code>
/// </summary>
///
public class DependencyGraph
{
    private int dgSize;

    // Map of the dependeesDict in the form (dependeesDict, {set of dependents})
    private Dictionary<string, HashSet<string>> dependeesDict;

    // Map of dependentsDict in the form (Dependent, {set of dependees})'
    private Dictionary<string, HashSet<string>> dependentsDict;

    /// <summary>
    ///   Initializes a new instance of the <see cref="DependencyGraph"/> class.
    ///   The initial DependencyGraph is empty.
    /// </summary>
    public DependencyGraph()
    {
        dependeesDict = new Dictionary<string, HashSet<string>>();
        dependentsDict = new Dictionary<string, HashSet<string>>();
        dgSize = 0;
    }

    /// <summary>
    /// Gets the number of ordered pairs in the DependencyGraph.
    /// </summary>
    public int Size
    {
        get
        {
            return dgSize;
        }
    }

    /// <summary>
    ///   Reports whether the given node has dependentsDict (i.e., other nodes depend on it).
    /// </summary>
    /// <param name="nodeName"> The name of the node.</param>
    /// <returns> true if the node has dependentsDict. </returns>
    public bool HasDependents(string nodeName)
    {
        if (dependeesDict.ContainsKey(nodeName))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///   Reports whether the given node has dependeesDict (i.e., depends on one or more other nodes).
    /// </summary>
    /// <returns> true if the node has dependeesDict.</returns>
    /// <param name="nodeName">The name of the node.</param>
    public bool HasDependees(string nodeName)
    {
        if (dependentsDict.ContainsKey(nodeName))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///   <para>
    ///     Returns the dependentsDict of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependentsDict of nodeName. </returns>
    public IEnumerable<string> GetDependents(string nodeName)
    {
        if (dependeesDict.ContainsKey(nodeName))
        {
            // Extracts the set
            return new HashSet<string>(dependeesDict[nodeName]);
        }

        return new HashSet<string>();
    }

    /// <summary>
    ///   <para>
    ///     Returns the dependeesDict of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependeesDict of nodeName. </returns>
    public IEnumerable<string> GetDependees(string nodeName)
    {
        if (dependentsDict.ContainsKey(nodeName))
        {
            // Extracts the set
            return new HashSet<string>(dependentsDict[nodeName]);
        }

        return new HashSet<string>();
    }

    /// <summary>
    /// <para>
    ///   Adds the ordered pair (dependee, dependent), if it doesn't already exist (otherwise nothing happens).
    /// </para>
    /// <para>
    ///   This can be thought of as: dependee must be evaluated before dependent.
    /// </para>
    /// </summary>
    /// <param name="dependee"> The name of the node that must be evaluated first. </param>
    /// <param name="dependent"> The name of the node that cannot be evaluated until after the other node has been. </param>
    public void AddDependency(string dependee, string dependent)
    {
        // Check if the pair does not already exist
        if (!(dependentsDict.ContainsKey(dependent) && dependeesDict.ContainsKey(dependee)))
        {
            dgSize++;
        }

        // Adds the dependee to the dependent if the dependent exists, otherwise create a new set of dependees to the dependeesDict
        if (dependentsDict.ContainsKey(dependent))
        {
            dependentsDict[dependent].Add(dependee);
        }
        else
        {
            HashSet<string> dependeeSet = new HashSet<string>();
            dependeeSet.Add(dependee);
            dependentsDict.Add(dependent, dependeeSet);
        }

        // Adds the dependent to the dependee if the dependee exists, otherwise create a new set of dependents to the dependentsDict
        if (dependeesDict.ContainsKey(dependee))
        {
            dependeesDict[dependee].Add(dependent);
        }
        else
        {
            HashSet<string> dependentSet = new HashSet<string>();
            dependentSet.Add(dependent);
            dependeesDict.Add(dependee, dependentSet);
        }
    }

    /// <summary>
    ///   <para>
    ///     Removes the ordered pair (dependee, dependent), if it exists (otherwise nothing happens).
    ///   </para>
    /// </summary>
    /// <param name="dependee"> The name of the node that must be evaluated first. </param>
    /// <param name="dependent"> The name of the node that cannot be evaluated until the other node has been. </param>
    public void RemoveDependency(string dependee, string dependent)
    {
        // If the pair exists, it is removable
        if (dependentsDict.ContainsKey(dependent) && dependeesDict.ContainsKey(dependee))
        {
            dgSize--;
        }

        // Remove the dependee from the dependent
        if (dependentsDict.ContainsKey(dependent))
        {
            dependentsDict[dependent].Remove(dependee);

            // If the dependent has no more dependeesDict, remove it
            if (dependentsDict[dependent].Count == 0)
            {
                dependentsDict.Remove(dependent);
            }
        }

        // Remove the dependent from the dependee
        if (dependeesDict.ContainsKey(dependee))
        {
            dependeesDict[dependee].Remove(dependent);

            // If the dependee has no more dependentsDict, remove it
            if (dependeesDict[dependee].Count == 0)
            {
                dependeesDict.Remove(dependee);
            }
        }
    }

    /// <summary>
    ///   Removes all existing ordered pairs of the form (nodeName, *).  Then, for each
    ///   t in newDependents, adds the ordered pair (nodeName, t).
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependentsDict are being replaced. </param>
    /// <param name="newDependents"> The new dependentsDict for nodeName. </param>
    public void ReplaceDependents(string nodeName, IEnumerable<string> newDependents)
    {
        // Case to prevent the search for a Dependent that does not exist
        if (!dependeesDict.ContainsKey(nodeName))
        {
            foreach (string dependent in newDependents)
            {
                AddDependency(nodeName, dependent);
            }

            return;
        }

        // Hold a temporary set of the old dependentsDict
        IEnumerable<string> dependentSet = GetDependents(nodeName);

        // Remove all of the old dependentsDict
        foreach (string dependent in dependentSet)
        {
            RemoveDependency(nodeName, dependent);
        }

        // Add back all the new set of dependentsDict
        foreach (string dependent in newDependents)
        {
            AddDependency(nodeName, dependent);
        }
    }

    /// <summary>
    ///   <para>
    ///     Removes all existing ordered pairs of the form (*, nodeName).  Then, for each
    ///     t in newDependees, adds the ordered pair (t, nodeName).
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependeesDict are being replaced. </param>
    /// <param name="newDependees"> The new dependeesDict for nodeName. Could be empty.</param>
    public void ReplaceDependees(string nodeName, IEnumerable<string> newDependees)
    {
        // Case to prevent the search of a dependee that does not exist
        if (!dependentsDict.ContainsKey(nodeName))
        {
            foreach (string dependee in newDependees)
            {
                AddDependency(dependee, nodeName);
            }

            return;
        }

        // Hold a temporary set of the old dependeesDict
        IEnumerable<string> dependeeSet = GetDependees(nodeName);

        // Remove all of the old dependeesDict
        foreach (string dependee in dependeeSet)
        {
            RemoveDependency(dependee, nodeName);
        }

        // Add in all the new dependeesDict
        foreach (string dependee in newDependees)
        {
            AddDependency(dependee, nodeName);
        }
    }
}
