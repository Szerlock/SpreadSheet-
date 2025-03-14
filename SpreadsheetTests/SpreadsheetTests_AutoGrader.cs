//// <copyright file="GradingTestsPS5.cs" company="UofU-CS3500">
//// Copyright (c) 2024 UofU-CS3500. All rights reserved.
//// </copyright>

//namespace CS3500.GradingTestsPS5;

//using CS3500.Spreadsheet;
//using CS3500.Formula;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

///// <summary>
///// Authors:   Joe Zachary
/////            Daniel Kopta
/////            Jim de St. Germain
///// Date:      Updated Spring 2020
///// Course:    CS 3500, University of Utah, School of Computing
///// Copyright: CS 3500 - This work may not be copied for use
/////                      in Academic Coursework.  See below.
/////
///// File Contents:
/////
/////   This file contains proprietary grading tests for CS 3500.  These tests cases
/////   are for individual student use only and MAY NOT BE SHARED.  Do not back them up
/////   nor place them in any online repository.  Improper use of these test cases
/////   can result in removal from the course and an academic misconduct sanction.
/////
/////   These tests are for your private use, this semester, only to improve the quality of the
/////   rest of your assignments.
///// </summary>
//[TestClass]
//public class SpreadsheetTest
//{
//    /// <summary>
//    ///   Test that the cell naming convention is honored.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("2")]
//    [ExpectedException(typeof(InvalidNameException))]
//    public void GetCellContents_InvalidCellName_Throws()
//    {
//        Spreadsheet s = new();
//        s.GetCellContents("1AA");
//    }

//    /// <summary>
//    ///   Test that an unassigned cell has the default value of an empty string.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("3")]
//    public void GetCellContents_DefaultCellValue_Empty()
//    {
//        Spreadsheet s = new();
//        Assert.AreEqual(string.Empty, s.GetCellContents("A2"));
//    }

//    /// <summary>
//    ///   Try setting an invalid cell to a double.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("5")]
//    [ExpectedException(typeof(InvalidNameException))]
//    public void SetCellContents_InvalidCellName_Throws()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("1A1A", 1.5);
//    }

//    /// <summary>
//    ///   Set a cell to a number and get the number back out.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("6")]
//    public void SetGetCellContents_SetTheNumber_RetrieveTheNumber()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("Z7", 1.5);
//        Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
//    }

//    // SETTING CELL TO A STRING

//    /// <summary>
//    ///   Try to assign a string to an invalid cell.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("9")]
//    [ExpectedException(typeof(InvalidNameException))]
//    public void SetCellContentsString_InvalidCellName_Throw()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("1AZ", "hello");
//    }

//    /// <summary>
//    ///   Simple test of assigning a string to a cell and getting
//    ///   it back out.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("10")]
//    public void SetAndGetCellContents_SetTheString_RetrieveTheString()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("Z7", "hello");
//        Assert.AreEqual("hello", s.GetCellContents("Z7"));
//    }

//    // SETTING CELL TO A FORMULA

//    /// <summary>
//    ///   Test that when assigning a formula, an invalid cell name
//    ///   throws an exception.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("13")]
//    [ExpectedException(typeof(InvalidNameException))]
//    public void SetCellContents_InvalidCellNameForFormula_Throws()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("1AZ", new Formula("2"));
//    }

//    /// <summary>
//    ///   Set a formula, retrieve the formula.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("14")]
//    public void SetGetCellContents_SetAFormula_RetrieveTheFormula()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("Z7", new Formula("3"));
//        Formula f = (Formula)s.GetCellContents("Z7");
//        Assert.AreEqual(new Formula("3"), f);
//        Assert.AreNotEqual(new Formula("2"), f);
//    }

//    // CIRCULAR FORMULA DETECTION

//    /// <summary>
//    ///   Two cell circular dependency check.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("15")]
//    [ExpectedException(typeof(CircularException))]
//    public void SetCellContents_CircularDependency_Throws()
//    {
//        Spreadsheet s = new();

//        s.SetCellContents("A1", new Formula("A2"));
//        s.SetCellContents("A2", new Formula("A1"));
//    }

//    /// <summary>
//    ///    A four cell circular dependency test.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("16")]
//    [ExpectedException(typeof(CircularException))]
//    public void SetCellContents_CircularDependencyMultipleCells_Throws()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("A1", new Formula("A2+A3"));
//        s.SetCellContents("A3", new Formula("A4+A5"));
//        s.SetCellContents("A5", new Formula("A6+A7"));
//        s.SetCellContents("A7", new Formula("A1+A1"));
//    }

//    /// <summary>
//    ///  Trying to add a circular dependency should leave the
//    ///  spreadsheet unmodified.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("17")]
//    [ExpectedException(typeof(CircularException))]
//    public void SetCellContents_TestUndoCircular_OriginalSheetRemains()
//    {
//        Spreadsheet s = new();
//        try
//        {
//            s.SetCellContents("A1", new Formula("A2+A3"));
//            s.SetCellContents("A2", 15);
//            s.SetCellContents("A3", 30);
//            s.SetCellContents("A2", new Formula("A3*A1"));
//        }
//        catch (CircularException)
//        {
//            Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
//            throw;
//        }
//    }

//    /// <summary>
//    ///   After adding the simplest circular dependency, the first cell
//    ///   should still contain the original value, but the second one removed.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("17b")]
//    [ExpectedException(typeof(CircularException))]
//    public void SetCellContents_SimpleCircularUndo_OriginalSheetRemains()
//    {
//        Spreadsheet s = new();
//        try
//        {
//            s.SetCellContents("A1", new Formula("A2"));
//            s.SetCellContents("A2", new Formula("A1"));
//        }
//        catch (CircularException)
//        {
//            Assert.AreEqual(string.Empty, s.GetCellContents("A2"));
//            Assert.IsTrue(new HashSet<string> { "A1" }.SetEquals(s.GetNamesOfAllNonemptyCells()));
//            throw;
//        }
//    }

//    // NONEMPTY CELLS

//    /// <summary>
//    ///   An empty spreadsheet should have no non-empty cells.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("18")]
//    public void GetNamesOfAllNonEmptyCells_EmptySpreadsheet_EmptyEnumerator()
//    {
//        Spreadsheet s = new();
//        Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
//    }

//    /// <summary>
//    ///   Assigning an empty string into a cell should not create a non-empty cell.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("19")]
//    public void SetCellContents_SetEmptyCell_CellIsEmpty()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("B1", string.Empty);
//        Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
//    }

//    /// <summary>
//    ///   Assigning a string into a cell produces a spreadsheet with one non-empty cell.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("20")]
//    public void GetNamesOfAllNonEmptyCells_AddStringToCell_ThatCellIsNotEmpty()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("B1", "hello");
//        Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(["B1"]));
//    }

//    /// <summary>
//    ///   Assigning a double into a cell produces a spreadsheet with one non-empty cell.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("21")]
//    public void GetNamesOfAllNonEmptyCells_AddDoubleToCell_ThatCellIsNotEmpty()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("B1", 52.25);
//        Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Matches(["B1"]));
//        Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(["B1"]));
//    }

//    /// <summary>
//    ///   Assigning a Formula into a cell produces a spreadsheet with one non-empty cell.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("22")]
//    public void GetNamesOfAllNonEmptyCells_AddFormulaToCell_ThatCellIsNotEmpty()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("B1", new Formula("3.5"));
//        Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(["B1"]));
//    }

//    /// <summary>
//    ///   Assign a double, string, and formula into the sheet and make sure
//    ///   they each have their own cell.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("23")]
//    public void SetCellContents_AssignDoubleStringAndFormula_ThreeCellsExist()
//    {
//        Spreadsheet s = new();

//        s.SetCellContents("A1", 17.2);
//        s.SetCellContents("C1", "hello");
//        s.SetCellContents("B1", new Formula("3.5"));

//        Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Matches(["A1", "B1", "C1"]));
//        Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(["A1", "B1", "C1"]));
//    }

//    // RETURN VALUE OF SET CELL CONTENTS

//    /// <summary>
//    ///   When a cell that has no cells depending on it is changed, then only
//    ///   that cell needs to be reevaluated. (Testing for Double content cells.)
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("24")]
//    public void SetCellContents_SettingIndependentCellToDouble_ReturnsOnlyThatCell()
//    {
//        Spreadsheet s = new();

//        s.SetCellContents("B1", "hello");
//        s.SetCellContents("C1", new Formula("5"));
//        var toReevaluate = s.SetCellContents("A1", 17.2);
//        Assert.IsTrue(toReevaluate.Matches(["A1"])); // Note: Matches is not order dependent
//    }

//    /// <summary>
//    ///   When a cell that has no cells depending on it is changed, then only
//    ///   that cell needs to be reevaluated. (Testing for Formula content cells.)
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("25")]
//    public void SetCellContents_SettingIndependentCellToString_ReturnsOnlyThatCell()
//    {
//        Spreadsheet s = new();

//        s.SetCellContents("A1", 17.2);
//        s.SetCellContents("C1", new Formula("5"));

//        var toReevaluated = s.SetCellContents("B1", "hello");
//        Assert.IsTrue(toReevaluated.Matches(["B1"]));
//    }

//    /// <summary>
//    ///   When a cell that has no cells depending on it is changed, then only
//    ///   that cell needs to be reevaluated. (Testing for Formula content cells.)
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("26")]
//    public void SetCellContents_SettingIndependentCellToFormula_ReturnsOnlyThatCell()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("A1", 17.2);
//        s.SetCellContents("B1", "hello");
//        var changed = s.SetCellContents("C1", new Formula("5"));
//        Assert.IsTrue(changed.Matches(["C1"]));
//    }

//    /// <summary>
//    ///   A chain of 5 cells is created.  When the first cell in the chain
//    ///   is modified, then all the cells have to be recomputed.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("27")]
//    public void SetCellContents_CreateChainModifyFirst_AllAreInNeedOfUpdate()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("A1", new Formula("A2+A3"));
//        s.SetCellContents("A2", 6);
//        s.SetCellContents("A3", new Formula("A2+A4"));
//        s.SetCellContents("A4", new Formula("A2+A5"));

//        var changed = s.SetCellContents("A5", 82.5);

//        Assert.IsTrue(changed.SequenceEqual(["A5", "A4", "A3", "A1"]));
//    }

//    // CHANGING CELLS

//    /// <summary>
//    ///   Test that replacing the contents of a cell (Formula --> double) works.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("28")]
//    public void SetCellContents_ReplaceFormulaWithDouble_CellValueCorrect()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("A1", new Formula("A2+A3"));
//        s.SetCellContents("A1", 2.5);
//        Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
//    }

//    /// <summary>
//    ///   Test that replacing a formula in a cell with a string works.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("29")]
//    public void SetCellContents_ReplaceFormulaWithString_CellValueCorrect()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("A1", new Formula("A2+A3"));
//        s.SetCellContents("A1", "Hello");
//        Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
//    }

//    /// <summary>
//    ///   Test that replacing a cell containing a string with a new formula works.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("30")]
//    public void SetCellContents_ReplaceStringWithFormula_CellValueCorrect()
//    {
//        Spreadsheet s = new();
//        s.SetCellContents("A1", "Hello");
//        s.SetCellContents("A1", new Formula("23"));
//        Assert.AreEqual(new Formula("23"), (Formula)s.GetCellContents("A1"));
//        Assert.AreNotEqual(new Formula("24"), (Formula)s.GetCellContents("A1"));
//    }

//    // STRESS TESTS

//    /// <summary>
//    ///   Create a sheet with 15 cells containing formulas.  Make sure that modifying
//    ///   the end of the chain results in all the cells having to be recomputed.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("31")]
//    public void SetCellContents_LongChainModifyEnd_AllCellsNeedToBeReEvaluated()
//    {
//        Spreadsheet s = new();

//        s.SetCellContents("A1", new Formula("B1+B2"));
//        s.SetCellContents("B1", new Formula("C1-C2"));
//        s.SetCellContents("B2", new Formula("C3*C4"));
//        s.SetCellContents("C1", new Formula("D1*D2"));
//        s.SetCellContents("C2", new Formula("D3*D4"));
//        s.SetCellContents("C3", new Formula("D5*D6"));
//        s.SetCellContents("C4", new Formula("D7*D8"));
//        s.SetCellContents("D1", new Formula("E1"));
//        s.SetCellContents("D2", new Formula("E1"));
//        s.SetCellContents("D3", new Formula("E1"));
//        s.SetCellContents("D4", new Formula("E1"));
//        s.SetCellContents("D5", new Formula("E1"));
//        s.SetCellContents("D6", new Formula("E1"));
//        s.SetCellContents("D7", new Formula("E1"));
//        s.SetCellContents("D8", new Formula("E1"));

//        var cells = s.SetCellContents("E1", 0);
//        Assert.IsTrue(cells.Matches(["A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1"]));
//    }

//    /// <summary>
//    ///    Repeat the stress test for more weight in grading process.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("32")]
//    public void IncreaseGradingWeight1()
//    {
//        SetCellContents_LongChainModifyEnd_AllCellsNeedToBeReEvaluated();
//    }

//    /// <summary>
//    ///    Repeat the stress test for more weight in grading process.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("33")]
//    public void IncreaseGradingWeight2()
//    {
//        SetCellContents_LongChainModifyEnd_AllCellsNeedToBeReEvaluated();
//    }

//    /// <summary>
//    ///    Repeat the stress test for more weight in grading process.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("34")]
//    public void IncreaseGradingWeight3()
//    {
//        SetCellContents_LongChainModifyEnd_AllCellsNeedToBeReEvaluated();
//    }

//    /// <summary>
//    ///   Programmatically create a chain of cells.  Each time we add
//    ///   another element to the chain, we check that the whole chain
//    ///   needs to be reevaluated.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("35")]
//    public void SetCellContents_TwoHundredLongChain_EachTimeReturnsRestOfChain()
//    {
//        Spreadsheet s = new();
//        ISet<string> expectedAnswers = new HashSet<string>();
//        for (int i = 1; i < 200; i++)
//        {
//            string currentCell = "A" + i;
//            expectedAnswers.Add(currentCell);

//            var changed = s.SetCellContents(currentCell, new Formula("A" + (i + 1)));

//            Assert.IsTrue(changed.Matches([.. expectedAnswers]));
//        }
//    }

//    /// <summary>
//    ///   Add weight to the grading by repeating the above test.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("36")]
//    public void IncreaseGradingWeight4()
//    {
//        SetCellContents_TwoHundredLongChain_EachTimeReturnsRestOfChain();
//    }

//    /// <summary>
//    ///   Add weight to the grading by repeating the above test.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("37")]
//    public void IncreaseGradingWeight5()
//    {
//        SetCellContents_TwoHundredLongChain_EachTimeReturnsRestOfChain();
//    }

//    /// <summary>
//    ///   Add weight to the grading by repeating the above test.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("38")]
//    public void IncreaseGradingWeight6()
//    {
//        SetCellContents_TwoHundredLongChain_EachTimeReturnsRestOfChain();
//    }

//    /// <summary>
//    ///   Build a long chain of cells.  Set one of the cells in the middle
//    ///   of the chain to a circular dependency.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("39")]
//    [ExpectedException(typeof(CircularException))]
//    public void SetCellContents_LongChainAddCircularInMiddle_Throws()
//    {
//        Spreadsheet s = new();

//        for (int i = 1; i < 200; i++)
//        {
//            string currentCell = "A" + i;
//            string nextCell = "A" + (i + 1);
//            s.SetCellContents(nextCell, 0);
//            s.SetCellContents(currentCell, new Formula(nextCell));
//        }

//        s.SetCellContents("A150", new Formula("A50"));
//    }

//    /// <summary>
//    ///   Add weight to the grading by repeating the above test.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("40")]
//    [ExpectedException(typeof(CircularException))]
//    public void IncreaseGradingWeight7()
//    {
//        SetCellContents_LongChainAddCircularInMiddle_Throws();
//    }

//    /// <summary>
//    ///   Add weight to the grading by repeating the above test.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("41")]
//    [ExpectedException(typeof(CircularException))]
//    public void IncreaseGradingWeight8()
//    {
//        SetCellContents_LongChainAddCircularInMiddle_Throws();
//    }

//    /// <summary>
//    ///   Add weight to the grading by repeating the above test.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("42")]
//    [ExpectedException(typeof(CircularException))]
//    public void IncreaseGradingWeight9()
//    {
//        SetCellContents_LongChainAddCircularInMiddle_Throws();
//    }

//    /// <summary>
//    ///   <para>
//    ///     This is a stress test with lots of cells "linked" together.
//    ///   </para>
//    ///   <para>
//    ///     Create 500 cells that are in a chain from A10 to A1499.
//    ///     Then break the chain in the middle by setting A1249 to
//    ///     a number.
//    ///   </para>
//    ///   <para>
//    ///     Then check that there are two separate chains of cells.
//    ///   </para>
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("43")]
//    public void SetCellContents_BreakALongChain_TwoIndependentChains()
//    {
//        Spreadsheet s = new();

//        for (int i = 0; i < 500; i++)
//        {
//            string currentCell = "A1" + i;
//            string nextCell = "A1" + (i + 1);
//            s.SetCellContents(nextCell, 0);
//            s.SetCellContents(currentCell, new Formula(nextCell));
//        }

//        List<string> firstCells = [];
//        List<string> lastCells = [];

//        for (int i = 0; i < 250; i++)
//        {
//            string firstHalfCell = "A1" + i;
//            string secondHalfCell = "A1" + (i + 250);
//            firstCells.Add(firstHalfCell);
//            lastCells.Add(secondHalfCell);
//        }

//        firstCells.Reverse();
//        lastCells.Reverse();

//        var firstHalfNeedReevaluate = s.SetCellContents("A1249", 25.0);
//        var secondHalfNeedReevaluate = s.SetCellContents("A1499", 0);

//        Assert.IsTrue(firstHalfNeedReevaluate.SequenceEqual(firstCells));
//        Assert.IsTrue(secondHalfNeedReevaluate.SequenceEqual(lastCells));
//    }

//    /// <summary>
//    ///   Add weight to the grading by repeating the above test.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("44")]
//    public void IncreaseGradingWeight10()
//    {
//        SetCellContents_BreakALongChain_TwoIndependentChains();
//    }

//    /// <summary>
//    ///   Add weight to the grading by repeating the above test.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("45")]
//    public void IncreaseGradingWeight11()
//    {
//        SetCellContents_BreakALongChain_TwoIndependentChains();
//    }

//    /// <summary>
//    ///   Add weight to the grading by repeating the above test.
//    /// </summary>
//    [TestMethod]
//    [Timeout(2000)]
//    [TestCategory("46")]
//    public void IncreaseGradingWeight12()
//    {
//        SetCellContents_BreakALongChain_TwoIndependentChains();
//    }

//    /// <summary>
//    ///   Add weight to the grading by repeating the given test.
//    /// </summary>
//    [TestMethod]
//    [Timeout(4000)]
//    [TestCategory("47")]
//    public void IncreaseGradingWeight13()
//    {
//        SetCellContents_1000RandomCells_MatchesPrecomputedSizeValue(47, 2514);
//    }

//    /// <summary>
//    ///   Add weight to the grading by repeating the given test.
//    /// </summary>
//    [TestMethod]
//    [Timeout(4000)]
//    [TestCategory("48")]
//    public void IncreaseGradingWeight14()
//    {
//        SetCellContents_1000RandomCells_MatchesPrecomputedSizeValue(48, 2519);
//    }

//    /// <summary>
//    ///   Add weight to the grading by repeating the given test.
//    /// </summary>
//    [TestMethod]
//    [Timeout(4000)]
//    [TestCategory("49")]
//    public void IncreaseGradingWeight15()
//    {
//        SetCellContents_1000RandomCells_MatchesPrecomputedSizeValue(49, 2502);
//    }

//    /// <summary>
//    ///   Add weight to the grading by repeating the given test.
//    /// </summary>
//    [TestMethod]
//    [Timeout(4000)]
//    [TestCategory("50")]
//    public void IncreaseGradingWeight16()
//    {
//        SetCellContents_1000RandomCells_MatchesPrecomputedSizeValue(50, 2515);
//    }

//    /// <summary>
//    ///   Generates a random cell name with a capital letter
//    ///   and number between 1 - 99.
//    /// </summary>
//    /// <param name="rand"> A random number generator. </param>
//    /// <returns> A random cell name like A10, or B50, .... </returns>
//    private static string GenerateRandomCellName(Random rand)
//    {
//        return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
//    }

//    /// <summary>
//    ///   Sets random cells to random contents (strings, doubles, formulas)
//    ///   10000 times.  The number of "repeated" cells in the random group
//    ///   has been predetermined based on the given random seed.
//    /// </summary>
//    /// <param name="seed">Random seed.</param>
//    /// <param name="size">
//    ///   The precomputed/known size of the resulting spreadsheet.
//    ///   This size was determined by pre-running the test with the given seed.
//    /// </param>
//    private static void SetCellContents_1000RandomCells_MatchesPrecomputedSizeValue(int seed, int size)
//    {
//        Spreadsheet s = new();
//        Random rand = new(seed);
//        for (int i = 0; i < 10000; i++)
//        {
//            try
//            {
//                string cellName = GenerateRandomCellName(rand);
//                switch (rand.Next(3))
//                {
//                    case 0:
//                        s.SetCellContents(cellName, 3.14);
//                        break;
//                    case 1:
//                        s.SetCellContents(cellName, "hello");
//                        break;
//                    case 2:
//                        s.SetCellContents(cellName, new Formula(GenerateRandomFormula(rand)));
//                        break;
//                }
//            }
//            catch (CircularException)
//            {
//            }
//        }

//        ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
//        Assert.AreEqual(size, set.Count);
//    }

//    /// <summary>
//    ///   <para>
//    ///     Generates a random Formula.
//    ///   </para>
//    ///   <para>
//    ///     This helper method is used in the randomize test.
//    ///   </para>
//    /// </summary>
//    /// <param name="rand"> A random number generator.</param>
//    /// <returns> A formula referencing random cells in a spreadsheet. </returns>
//    private static string GenerateRandomFormula(Random rand)
//    {
//        string formula = GenerateRandomCellName(rand);
//        for (int i = 0; i < 10; i++)
//        {
//            switch (rand.Next(4))
//            {
//                case 0:
//                    formula += "+";
//                    break;
//                case 1:
//                    formula += "-";
//                    break;
//                case 2:
//                    formula += "*";
//                    break;
//                case 3:
//                    formula += "/";
//                    break;
//            }

//            switch (rand.Next(2))
//            {
//                case 0:
//                    formula += 7.2;
//                    break;
//                case 1:
//                    formula += GenerateRandomCellName(rand);
//                    break;
//            }
//        }

//        return formula;
//    }
//}

///// <summary>
/////   Helper methods for the tests above.
///// </summary>
//public static class IEnumerableExtensions
//{
//    /// <summary>
//    ///   Check to see if the two "sets" (source and items) match, i.e.,
//    ///   contain exactly the same values. Note: we do not check for sequencing.
//    /// </summary>
//    /// <param name="source"> original container.</param>
//    /// <param name="items"> elements to match against.</param>
//    /// <returns> true if every element in source is in items and vice versa. They are the "same set".</returns>
//    public static bool Matches(this IEnumerable<string> source, params string[] items)
//    {
//        return (source.Count() == items.Length) && items.All(item => source.Contains(item));
//    }
//}