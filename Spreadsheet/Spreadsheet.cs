// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// <summary>
// Author:    Andy Tran
// Date:      10-17-24
// Course:    CS 3500, University of Utah, School of Computing
// Copyright: CS 3500 and Andy Tran - This work may not
//            be copied for use in Academic Coursework.
//
// I, Andy Tran, certify that I wrote this code from scratch and
// did not copy it in part or whole from another source.  All
// references used in the completion of the assignments are cited
// in my README file.
//
// File Contents
// This file contains the underlying structure of our overall spreadsheet. The current functionality of this spreadsheet
// is to add and map dependent cells together. There is no implementation of recalculation yet.
// </summary>

// Written by Joe Zachary for CS 3500, September 2013
// Update by Profs Kopta and de St. Germain
//     - Updated return types
//     - Updated documentation
namespace CS3500.Spreadsheet;

using CS3500.DependencyGraph;
using CS3500.Formula;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

/// <summary>
///   <para>
///     Thrown to indicate that a change to a cell will cause a circular dependency.
///   </para>
/// </summary>
public class CircularException : Exception
{
}

/// <summary>
///   <para>
///     Thrown to indicate that a name parameter was invalid.
///   </para>
/// </summary>
public class InvalidNameException : Exception
{
}

/// <summary>
/// <para>
///   Thrown to indicate that a read or write attempt has failed with
///   an expected error message informing the user of what went wrong.
/// </para>
/// </summary>
public class SpreadsheetReadWriteException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpreadsheetReadWriteException"/> class.
    ///   <para>
    ///     Creates the exception with a message defining what went wrong.
    ///   </para>
    /// </summary>
    /// <param name="msg"> An informative message to the user. </param>
    public SpreadsheetReadWriteException(string msg)
        : base(msg)
    {
    }
}

/// <summary>
///   <para>
///     An Spreadsheet object represents the state of a simple spreadsheet.  A
///     spreadsheet represents an infinite number of named cells.
///   </para>
/// <para>
///     Valid Cell Names: A string is a valid cell name if and only if it is one or
///     more letters followed by one or more numbers, e.g., A5, BC27.
/// </para>
/// <para>
///    Cell names are case insensitive, so "x1" and "X1" are the same cell name.
///    Your code should normalize (uppercased) any stored name but accept either.
/// </para>
/// <para>
///     A spreadsheet represents a cell corresponding to every possible cell name.  (This
///     means that a spreadsheet contains an infinite number of cells.)  In addition to
///     a name, each cell has a contents and a value.  The distinction is important.
/// </para>
/// <para>
///     The <b>contents</b> of a cell can be (1) a string, (2) a double, or (3) a Formula.
///     If the contents of a cell is set to the empty string, the cell is considered empty.
/// </para>
/// <para>
///     By analogy, the contents of a cell in Excel is what is displayed on
///     the editing line when the cell is selected.
/// </para>
/// <para>
///     In a new spreadsheet, the contents of every cell is the empty string. Note:
///     this is by definition (it is IMPLIED, not stored).
/// </para>
/// <para>
///     The <b>value</b> of a cell can be (1) a string, (2) a double, or (3) a FormulaError.
///     (By analogy, the value of an Excel cell is what is displayed in that cell's position
///     in the grid.)
/// </para>
/// <list type="number">
///   <item>If a cell's contents is a string, its value is that string.</item>
///   <item>If a cell's contents is a double, its value is that double.</item>
///   <item>
///     <para>
///       If a cell's contents is a Formula, its value is either a double or a FormulaError,
///       as reported by the Evaluate method of the Formula class.  For this assignment,
///       you are not dealing with values yet.
///     </para>
///   </item>
/// </list>
/// <para>
///     Spreadsheets are never allowed to contain a combination of Formulas that establish
///     a circular dependency.  A circular dependency exists when a cell depends on itself.
///     For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
///     A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
///     dependency.
/// </para>
/// </summary>
public class Spreadsheet
{
    // instantiate depedency graph and spreadsheet that holds the cells
    private DependencyGraph dg;

    [JsonInclude]
    [JsonPropertyName("Cells")]
    private Dictionary<string, Cell> cells;

    // Name of spreadsheet
    private string ssName;

    /// <summary>
    /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
    /// </summary>
    public Spreadsheet()
    {
        dg = new DependencyGraph();
        cells = new Dictionary<string, Cell>();
        ssName = "default";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Spreadsheet"/> class with a dedicated name.
    /// </summary>
    /// <param name="name"> the name of the spreadsheet. </param>
    public Spreadsheet(string name)
    {
        dg = new DependencyGraph();
        cells = new Dictionary<string, Cell>();
        ssName = name;
    }

    /// <summary>
    /// Gets a value indicating whether the spreadsheet has been changed or not.
    /// </summary>
    [JsonIgnore]
    public bool Changed { get; private set; }

    /// <summary>
    ///   <para>
    ///     Shortcut syntax to for getting the value of the cell
    ///     using the [] operator.
    ///   </para>
    ///   <para>
    ///     See: <see cref="GetCellValue(string)"/>.
    ///   </para>
    ///   <para>
    ///     Example Usage:
    ///   </para>
    ///   <code>
    ///      sheet.SetContentsOfCell( "A1", "=5+5" );
    ///
    ///      sheet["A1"] == 10;
    ///      // vs.
    ///      sheet.GetCellValue("A1") == 10;
    ///   </code>
    /// </summary>
    /// <param name="cellName"> Any valid cell name. </param>
    /// <returns>
    ///   Returns the value of a cell.  Note: If the cell is a formula, the value should
    ///   already have been computed.
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If the name parameter is invalid, throw an InvalidNameException.
    /// </exception>
    public object this[string cellName]
    {
        get
        {
            return GetCellValue(cellName);
        }
    }

    /// <summary>
    ///   Provides a copy of the names of all of the cells in the spreadsheet
    ///   that contain information (i.e., not empty cells).
    /// </summary>
    /// <returns>
    ///   A set of the names of all the non-empty cells in the spreadsheet.
    /// </returns>
    public ISet<string> GetNamesOfAllNonemptyCells()
    {
        // return all the cell names into a hashset
        return cells.Keys.ToHashSet();
    }

    /// <summary>
    ///   Returns the contents (as opposed to the value) of the named cell.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   Thrown if the name is invalid.
    /// </exception>
    ///
    /// <param name="name">The name of the spreadsheet cell to query. </param>
    /// <returns>
    ///   The contents as either a string, a double, or a Formula.
    ///   See the class header summary.
    /// </returns>
    public object GetCellContents(string name)
    {
        name = NormalizeAndCheckName(name);

        if (cells.TryGetValue(name, out var output))
        {
            return output.Content;
        }

        // case when the cell cannot be found
        return string.Empty;
    }

    /// <summary>
    ///   <para>
    ///     Writes the contents of this spreadsheet to the named file using a JSON format.
    ///     If the file already exists, overwrite it.
    ///   </para>
    ///   <para>
    ///     The output JSON should look like the following.
    ///   </para>
    ///   <para>
    ///     For example, consider a spreadsheet that contains a cell "A1"
    ///     with contents being the double 5.0, and a cell "B3" with contents
    ///     being the Formula("A1+2"), and a cell "C4" with the contents "hello".
    ///   </para>
    ///   <para>
    ///      This method would produce the following JSON string:
    ///   </para>
    ///   <code>
    ///   {
    ///     "Cells": {
    ///       "A1": {
    ///         "StringForm": "5"
    ///       },
    ///       "B3": {
    ///         "StringForm": "=A1+2"
    ///       },
    ///       "C4": {
    ///         "StringForm": "hello"
    ///       }
    ///     }
    ///   }
    ///   </code>
    ///   <para>
    ///     You can achieve this by making sure your data structure is a dictionary
    ///     and that the contained objects (Cells) have property named "StringForm"
    ///     (if this name does not match your existing code, use the JsonPropertyName
    ///     attribute).
    ///   </para>
    ///   <para>
    ///     There can be 0 cells in the dictionary, resulting in { "Cells" : {} }.
    ///   </para>
    ///   <para>
    ///     Further, when writing the value of each cell...
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       If the contents is a string, the value of StringForm is that string
    ///     </item>
    ///     <item>
    ///       If the contents is a double d, the value of StringForm is d.ToString()
    ///     </item>
    ///     <item>
    ///       If the contents is a Formula f, the value of StringForm is "=" + f.ToString()
    ///     </item>
    ///   </list>
    ///   <para>
    ///     After saving the file, the spreadsheet is no longer "Changed".
    ///   </para>
    /// </summary>
    /// <param name="filename"> The name (with path) of the file to save to.</param>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   If there are any problems opening, writing, or closing the file,
    ///   the method should throw a SpreadsheetReadWriteException with an
    ///   explanatory message.
    /// </exception>
    public void Save(string filename)
    {
        // check for invalid filename
        IsValidFileName(filename);

        try
        {
        // serialize spreadsheet and save to file
        string filePath = filename;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        string jsonString = JsonSerializer.Serialize(this, options);
        File.WriteAllText(filePath, jsonString);
        }
        catch (Exception)
        {
            throw new SpreadsheetReadWriteException("Error occured when trying to save your file");
        }

        Changed = false;
    }

    /// <summary>
    /// This method computes the Json string of the spreadsheet.
    /// </summary>
    /// <returns>Returns the Json representation of the spreadsheet.</returns>
    public string GetJSON()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        string jsonString = JsonSerializer.Serialize(this, options);
        return jsonString;
    }

    /// <summary>
    /// Instantiates the current spreadsheet from the given JSON string.
    /// </summary>
    /// <param name="json">Json string containing spreadsheet contents.</param>
    /// <exception cref="SpreadsheetReadWriteException">Throws Exception if the Json string is not valid.</exception>
    public void InstantiateFromJSON(string json)
    {
        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            Spreadsheet ss = JsonSerializer.Deserialize<Spreadsheet>(json, options) ?? throw new Exception("deserialization error");
            cells.Clear();

            // add all the cells in the temporary spread sheet into our spreadsheet
            foreach (var keypair in ss.cells)
            {
                SetContentsOfCell(keypair.Key, keypair.Value.StringForm);
            }
        }
        catch (Exception)
        {
            throw new SpreadsheetReadWriteException("Error occured when trying to load your file");
        }

        Changed = false;
    }

    /// <summary>
    ///   <para>
    ///     Read the data (JSON) from the file and instantiate the current
    ///     spreadsheet.  See <see cref="Save(string)"/> for expected format.
    ///   </para>
    ///   <para>
    ///     Note: First deletes any current data in the spreadsheet.
    ///   </para>
    ///   <para>
    ///     Loading a spreadsheet should set Changed to false.  External
    ///     programs should alert the user before loading over a Changed sheet.
    ///   </para>
    /// </summary>
    /// <param name="filename"> The saved file name including the path. </param>
    /// <exception cref="SpreadsheetReadWriteException"> When the file cannot be opened or the json is bad.</exception>
    public void Load(string filename)
    {
        // check for invalid filename
        IsValidFileName(filename);

        try
        {
            // read file, and deserialize it
            string jsonString = File.ReadAllText(filename);
            Spreadsheet ss = JsonSerializer.Deserialize<Spreadsheet>(jsonString)!;
            cells.Clear();

            // add all the cells in the temporary spread sheet into our spreadsheet
            foreach(var keypair in ss.cells)
            {
                SetContentsOfCell(keypair.Key, keypair.Value.StringForm);
            }
        }
        catch (Exception)
        {
            throw new SpreadsheetReadWriteException("Error occured when trying to load your file");
        }

        Changed = false;
    }

    /// <summary>
    ///   <para>
    ///     Return the value of the named cell.
    ///   </para>
    /// </summary>
    /// <param name="cellName"> The cell in question. </param>
    /// <returns>
    ///   Returns the value (as opposed to the contents) of the named cell.  The return
    ///   value's type should be either a string, a double, or a CS3500.Formula.FormulaError.
    ///   If the cell contents are a formula, the value should have already been computed
    ///   at this point.
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object GetCellValue(string cellName)
    {
        cellName = NormalizeAndCheckName(cellName);

        if (cells.TryGetValue(cellName, out var cell))
        {
            return cell.Value;
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    ///   <para>
    ///       Sets the contents of the named cell to the appropriate object
    ///       based on the string in <paramref name="content"/>.
    ///   </para>
    ///   <para>
    ///       First, if the <paramref name="content"/> parses as a double, the contents of the named
    ///       cell becomes that double.
    ///   </para>
    ///   <para>
    ///       Otherwise, if the <paramref name="content"/> begins with the character '=', an attempt is made
    ///       to parse the remainder of content into a Formula.
    ///   </para>
    ///   <para>
    ///       There are then three possible outcomes when a formula is detected:
    ///   </para>
    ///
    ///   <list type="number">
    ///     <item>
    ///       If the remainder of content cannot be parsed into a Formula, a
    ///       FormulaFormatException is thrown.
    ///     </item>
    ///     <item>
    ///       If changing the contents of the named cell to be f
    ///       would cause a circular dependency, a CircularException is thrown,
    ///       and no change is made to the spreadsheet.
    ///     </item>
    ///     <item>
    ///       Otherwise, the contents of the named cell becomes f.
    ///     </item>
    ///   </list>
    ///   <para>
    ///     Finally, if the content is a string that is not a double and does not
    ///     begin with an "=" (equal sign), save the content as a string.
    ///   </para>
    ///   <para>
    ///     On successfully changing the contents of a cell, the spreadsheet will be <see cref="Changed"/>.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell name that is being Changed.</param>
    /// <param name="content"> The new content of the cell.</param>
    /// <returns>
    ///   <para>
    ///     This method returns a list consisting of the passed in cell name,
    ///     followed by the names of all other cells whose value depends, directly
    ///     or indirectly, on the named cell. The order of the list MUST BE any
    ///     order such that if cells are re-evaluated in that order, their dependencies
    ///     are satisfied by the time they are evaluated.
    ///   </para>
    ///   <para>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list {A1, B1, C1} is returned.  If the cells are then evaluate din the orde
    ///     A1, then B1, then C1, the integrity of the Spreadsheet is maintained.
    ///   </para>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the name parameter is invalid, throw an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///   If changing the contents of the named cell to be the formula would
    ///   cause a circular dependency, throw a CircularException.
    ///   (NOTE: No change is made to the spreadsheet.)
    /// </exception>
    public IList<string> SetContentsOfCell(string name, string content)
    {
        name = NormalizeAndCheckName(name);

        // The list of outputs
        List<string> dependentsList;

        // Check and create the cell based on its contents
        if (double.TryParse(content, out var result))
        {
            dependentsList = new List<string>(SetCellContents(name, result));
        }
        else if (content.StartsWith("="))
        {
            string formulaAsString = content.Substring(1);
            Formula formula = new Formula(formulaAsString);
            dependentsList = new List<string>(SetCellContents(name, formula));
        }
        else
        {
            dependentsList = new List<string>(SetCellContents(name, content));
        }

        Changed = true;

        // look through and update each value everytime a new cell is created. Only formulas will update.
        foreach (string dependent in dependentsList)
        {
            if (cells.TryGetValue(dependent, out var cellVal))
            {
                cellVal.ReEvaluate(LookUpVal);
            }
        }

        return dependentsList;
    }

    private void IsValidFileName(string filename)
    {
        if (filename.Equals(string.Empty) || ReferenceEquals(filename, null))
        {
            throw new SpreadsheetReadWriteException("filename cannot be empty.");
        }
    }

    /// <summary>
    ///  Set the contents of the named cell to the given number.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    ///
    /// <param name="name"> The name of the cell. </param>
    /// <param name="number"> The new content of the cell. </param>
    /// <returns>
    ///   <para>
    ///     This method returns an ordered list consisting of the passed in name
    ///     followed by the names of all other cells whose value depends, directly
    ///     or indirectly, on the named cell.
    ///   </para>
    ///   <para>
    ///     The order must correspond to a valid dependency ordering for recomputing
    ///     all of the cells, i.e., if you re-evaluate each cell in the order of the list,
    ///     the overall spreadsheet will be correctly updated.
    ///   </para>
    ///   <para>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list [A1, B1, C1] is returned, i.e., A1 was Changed, so then A1 must be
    ///     evaluated, followed by B1 re-evaluated, followed by C1 re-evaluated.
    ///   </para>
    /// </returns>
    private IList<string> SetCellContents(string name, double number)
    {
        // replaces the cell if it already exists, or create a new one otherwise
        if (cells.ContainsKey(name))
        {
            cells[name] = new Cell(number);
        }
        else
        {
            cells.Add(name, new Cell(number));
        }

        return UpdateDependees(name);
    }

    /// <summary>
    ///   The contents of the named cell becomes the given text.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="text"> The new content of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, string text)
    {
        // replaces the cell if it already exists, or create a new one otherwise
        if (cells.ContainsKey(name))
        {
            cells[name] = new Cell(text);
        }
        else
        {
            cells.Add(name, new Cell(text));
        }

        // if the cell becomes empty, discard the cell
        if (cells[name].Content.Equals(string.Empty))
        {
            cells.Remove(name);
        }

        return UpdateDependees(name);
    }

    /// <summary>
    ///   Set the contents of the named cell to the given formula.
    /// </summary>
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///   <para>
    ///     If changing the contents of the named cell to be the formula would
    ///     cause a circular dependency, throw a CircularException.
    ///   </para>
    ///   <para>
    ///     No change is made to the spreadsheet.
    ///   </para>
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="formula"> The new content of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, Formula formula)
    {
        // store the current dependees
        IEnumerable<string> currentDependees = dg.GetDependees(name);

        // replace the dependees with all the formula's
        dg.ReplaceDependees(name, formula.GetVariables());

        try
        {
            // recalculate the dependencies
            List<string> newDependees = new List<string>(GetCellsToRecalculate(name));

            // replaces the cell if it already exists, or create a new one otherwise
            if (cells.ContainsKey(name))
            {
                cells[name] = new Cell(formula);
            }
            else
            {
                cells.Add(name, new Cell(formula));
            }

            return newDependees;
        }
        catch (CircularException)
        {
            // case to restore the dependees in case an exception happens
            dg.ReplaceDependees(name, currentDependees);
            throw new CircularException();
        }
    }

    /// <summary>
    /// This method updates all the dependees of the cell that it affects.
    /// </summary>
    /// <param name="name"> the name of the cell that is to be updated. </param>
    /// <returns> a list of dependees that need to be updated/reevaluated. </returns>
    private List<string> UpdateDependees(string name)
    {
        dg.ReplaceDependees(name, new HashSet<string>());
        List<string> dependees = new List<string>(GetCellsToRecalculate(name));
        return dependees;
    }

    /// <summary>
    /// This method normalizes the cell name to not be case-sensitive and checks for any invalid names.
    /// </summary>
    /// <param name="name"> the name to check. </param>
    /// <returns> the normalized name if it is valid. </returns>
    /// <exception cref="InvalidNameException"> If the cell name is not a variable(s) followed by a number(s). </exception>
    private string NormalizeAndCheckName(string name)
    {
        string normalizedName = name.ToUpper();
        if (!IsValidName(name))
        {
            throw new InvalidNameException();
        }

        return normalizedName;
    }

    /// <summary>
    /// This method checks whether the input string has valid variable syntax or not.
    /// </summary>
    /// <param name="name"> the name of the cell. </param>
    /// <returns> true if the cell name is valid, false otherwise. </returns>
    private bool IsValidName(string name)
    {
        // code is the same as seen in IsVar method in the formula class
        string standaloneVarPattern = $"^{@"[a-zA-Z]+\d+"}$";
        return Regex.IsMatch(name, standaloneVarPattern);
    }

    /// <summary>
    ///   Returns an enumeration, without duplicates, of the names of all cells whose
    ///   values depend directly on the value of the named cell.
    /// </summary>
    /// <param name="name"> This <b>MUST</b> be a valid name.  </param>
    /// <returns>
    ///   <para>
    ///     Returns an enumeration, without duplicates, of the names of all cells
    ///     that contain formulas containing name.
    ///   </para>
    ///   <para>For example, suppose that: </para>
    ///   <list type="bullet">
    ///      <item>A1 contains 3</item>
    ///      <item>B1 contains the formula A1 * A1</item>
    ///      <item>C1 contains the formula B1 + A1</item>
    ///      <item>D1 contains the formula B1 - C1</item>
    ///   </list>
    ///   <para> The direct dependents of A1 are B1 and C1. </para>
    /// </returns>
    private IEnumerable<string> GetDirectDependents(string name)
    {
        return dg.GetDependents(name);
    }

    /// <summary>
    ///   <para>
    ///     This method is implemented for you, but makes use of your GetDirectDependents.
    ///   </para>
    ///   <para>
    ///     Returns an enumeration of the names of all cells whose values must
    ///     be recalculated, assuming that the contents of the cell referred
    ///     to by name has Changed.  The cell names are enumerated in an order
    ///     in which the calculations should be done.
    ///   </para>
    ///   <exception cref="CircularException">
    ///     If the cell referred to by name is involved in a circular dependency,
    ///     throws a CircularException.
    ///   </exception>
    ///   <para>
    ///     For example, suppose that:
    ///   </para>
    ///   <list type="number">
    ///     <item>
    ///       A1 contains 5
    ///     </item>
    ///     <item>
    ///       B1 contains the formula A1 + 2.
    ///     </item>
    ///     <item>
    ///       C1 contains the formula A1 + B1.
    ///     </item>
    ///     <item>
    ///       D1 contains the formula A1 * 7.
    ///     </item>
    ///     <item>
    ///       E1 contains 15
    ///     </item>
    ///   </list>
    ///   <para>
    ///     If A1 has Changed, then A1, B1, C1, and D1 must be recalculated,
    ///     and they must be recalculated in an order which has A1 first, and B1 before C1
    ///     (there are multiple such valid orders).
    ///     The method will produce one of those enumerations.
    ///   </para>
    ///   <para>
    ///      PLEASE NOTE THAT THIS METHOD DEPENDS ON THE METHOD GetDirectDependents.
    ///      IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
    ///   </para>
    /// </summary>
    /// <param name="name"> The name of the cell.  Requires that name be a valid cell name.</param>
    /// <returns>
    ///    Returns an enumeration of the names of all cells whose values must
    ///    be recalculated.
    /// </returns>
    private IEnumerable<string> GetCellsToRecalculate(string name)
    {
        // keep track of the cells that Changed
        LinkedList<string> changed = new();

        // keep track of those visited
        HashSet<string> visited = [];

        // recursive call
        Visit(name, name, visited, changed);
        return changed;
    }

    /// <summary>
    ///   A helper for the GetCellsToRecalculate method.
    /// </summary>
    private void Visit(string start, string name, ISet<string> visited, LinkedList<string> changed)
    {
        // cell as visited
        visited.Add(name);

        // look at each dependent
        foreach (string dependent in GetDirectDependents(name))
        {
            // if the dependent is back where we started, throw an exception
            if (dependent.Equals(start))
            {
                throw new CircularException();
            }

            // if there are more dependents and we havent visited, go deeper
            else if (!visited.Contains(dependent))
            {
                Visit(start, dependent, visited, changed);
            }
        }

        // at the lowest level with no further dependents, add it to the list of Changed
        changed.AddFirst(name);
    }

    /// <summary>
    /// This method is responsible for looking up and finding the value of a certain cell.
    /// </summary>
    /// <param name="name"> the name of the cell. </param>
    /// <returns> the value contained in that cell. </returns>
    /// <exception cref="ArgumentException"> if there is anything else besides a number contained in the cell used for calculations. </exception>
    private double LookUpVal(string name)
    {
        if (cells.TryGetValue(name, out var cell))
        {
            if (cell.Value is double)
            {
                return (double)cell.Value;
            }
            else
            {
                throw new ArgumentException("value is not a number");
            }
        }
        else
        {
            throw new ArgumentException("value is not a number");
        }
    }

    /// <summary>
    /// This is a class that defines a cell object in a spreadsheet. It holds and contains the contents and value of a cell.
    /// </summary>
    private class Cell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class. This instance will never be accessed besides for deserialization purposes.
        /// </summary>
        public Cell()
        {
            // Should be empty, but initializing is necessary in order to avoid warnings by compiler
            Content = string.Empty;
            Value = Content;
            StringForm = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class for strings.
        /// </summary>
        /// <param name="name"> the string that will be stored in the cell. </param>
        public Cell(string name)
        {
            Content = name;
            Value = Content;
            StringForm = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class for numbers.
        /// </summary>
        /// <param name="number"> the number that will be stored in the cell. </param>
        public Cell(double number)
        {
            Content = number;
            Value = Content;
            StringForm = number.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class for formulas.
        /// </summary>
        /// <param name="formula"> the formula that will be stored in the cell. </param>
        public Cell(Formula formula)
        {
            Content = formula;
            Value = Content;
            StringForm = "=" + formula.ToString();
        }

        // the actual object inside the cell
        [JsonIgnore]
        public object Content { get; private set; }

        [JsonIgnore]
        public object Value { get; private set; }

        // The contents of the cell in the formatting of a string
        public string StringForm { get; set; }

        /// <summary>
        /// This method revaluates the formula based on values that have been updated.
        /// </summary>
        /// <param name="lookup"> the lookup used to determine the value of a cell. </param>
        public void ReEvaluate(Lookup lookup)
        {
            if (Content is Formula)
            {
                // cast and evaluate the formula once more
                Formula f = (Formula)Content;
                Value = f.Evaluate(lookup);
            }
        }
    }
}
