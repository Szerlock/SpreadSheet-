// <copyright file="SpreadsheetTests.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Andy Tran </authors>
// <date> September 25, 2024 </date>

namespace SpreadsheetTests;

using CS3500.Spreadsheet;
using CS3500.Formula;
using NuGet.Frameworks;
using System.Security.Cryptography;
using System.Reflection.Emit;
using System.Text.Json;
using System.Reflection;

/// <summary>
/// This is a a test class for our spreadsheet program. It checks to make sure all the current functionality of the spreadsheet
/// works as intended.
/// </summary>
[TestClass]
public class SpreadsheetTests
{
    // ---- PS5 MVC Tests (Updated for PS6) -----------------------------
    // ---- Cell Initialization and Content Retrieval --------------------

    /// <summary>
    /// This test checks that everything is initialized when a spreadsheet is created
    /// </summary>
    [TestMethod]
    public void EmptySpreadSheetTest()
    {
        Spreadsheet s = new Spreadsheet();
        Assert.IsTrue(s.GetNamesOfAllNonemptyCells().SetEquals([]));
    }

    /// <summary>
    /// This test checks when grabbing the contents from a non existant cell
    /// </summary>
    [TestMethod]
    public void GetCellName_NotFoundTest()
    {
        Spreadsheet s = new Spreadsheet();
        Assert.IsTrue(s.GetCellContents("B1").Equals(""));
    }

    /// <summary>
    /// This test checks that all non-empty cells are retrieved from a variety of different contents
    /// </summary>
    [TestMethod]
    public void GetAllCellsTest()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "start");
        s.SetContentsOfCell("A2", "1e-10");
        s.SetContentsOfCell("A3", ("=x1 * y1"));
        Assert.IsTrue(s.GetNamesOfAllNonemptyCells().SetEquals(["A1", "A2", "A3"]));
    }

    /// <summary>
    /// This test checks that empty cells are not retrieved and only cells containing content will
    /// </summary>
    [TestMethod]
    public void GetAllCellsTestWithEmptyCells()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "start");
        s.SetContentsOfCell("A2", "1e-10");
        s.SetContentsOfCell("A3", ("=x1 * y1"));
        s.SetContentsOfCell("A4", "");
        s.SetContentsOfCell("A2", "");
        Assert.IsTrue(s.GetNamesOfAllNonemptyCells().SetEquals(["A1", "A3"]));
    }

    /// <summary>
    /// This test checks the functionality of removing all cells of their contents, effecitively deleting them
    /// </summary>
    [TestMethod]
    public void TestRidAll_CellNames()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A3", "80");
        s.SetContentsOfCell("A1", "bye");
        s.SetContentsOfCell("A3", "");
        s.SetContentsOfCell("A1", "");
        Assert.IsTrue(s.GetNamesOfAllNonemptyCells().SetEquals([]));
    }

    /// <summary>
    /// This test checks that the same cell is accessed and is not case-sensitive
    /// </summary>
    [TestMethod]
    public void SameCellName()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "1.0");
        Assert.AreEqual(s.GetCellContents("a1"), 1.0);
    }

    /// <summary>
    /// This test checks if a cell can correctly store a number
    /// </summary>
    [TestMethod]
    public void TestGetCellContents_Number()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        Assert.AreEqual(s.GetCellContents("A1"), 5.0);
    }

    /// <summary>
    /// This test checks if a cell can correctly store a string
    /// </summary>
    [TestMethod]
    public void TestGetCellContents_String()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "hi");
        Assert.AreEqual(s.GetCellContents("A1"), "hi");
    }

    /// <summary>
    /// This test checks if a cell can correctly store a Formula
    /// </summary>
    [TestMethod]
    public void TestGetCellContents_Formula()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", ("=1*A2"));
        Formula f = new Formula("1*A2");
        Assert.AreEqual(s.GetCellContents("A1"), f);
    }

    // ----- Updating/Changing Cell Dependencies ----------------

    /// <summary>
    /// This test checks if a cell can correctly update when adding a new formula
    /// </summary>
    [TestMethod]
    public void TestUpdateCells_WithFormula()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5.0");
        s.SetContentsOfCell("A2", ("=A1+1"));
        s.SetContentsOfCell("A3", ("=A2+1"));
        Assert.IsTrue(s.SetContentsOfCell("A1", "2.0").SequenceEqual(["A1", "A2", "A3"]));
    }

    /// <summary>
    /// This test checks if a cell can correctly update when changing the number value
    /// </summary>
    [TestMethod]
    public void TestUpdateCells_WithNums()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("a1", "1");
        s.SetContentsOfCell("a2", "a1");
        Assert.IsTrue(s.SetContentsOfCell("a1", "2").SequenceEqual(["A1"]));
    }

    /// <summary>
    /// This test checks if a cell can correctly update when changing the string
    /// </summary>
    [TestMethod]
    public void TestSetCellUpdate_WithString()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "hi");
        s.SetContentsOfCell("A2", "yo");
        Assert.IsTrue(s.SetContentsOfCell("A3", "bye").SequenceEqual(["A3"]));
    }

    /// <summary>
    /// This test checks if a cell can correctly update when it affects numbers and formulas
    /// </summary>
    [TestMethod]
    public void TestSetCellsUpdate_WithFormulaAndNums()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", ("=A2 + 1"));
        s.SetContentsOfCell("A2", "5.0");
        Assert.IsTrue(s.SetContentsOfCell("A3", ("=A1 + 4")).SequenceEqual(["A3"]));

    }

    /// <summary>
    /// This test checks if a cell can correctly update in a larger chain of formula updates.
    /// </summary>
    [TestMethod]
    public void TestLongerSetSequence()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", ("=A3"));
        s.SetContentsOfCell("A2", ("=A1"));
        s.SetContentsOfCell("A3", "10");
        s.SetContentsOfCell("A4", ("=A2+A5"));
        Assert.IsTrue(s.SetContentsOfCell("A5", "5").SequenceEqual(["A5", "A4"]));
    }

    // ------ Exception Tests ----------------------

    /// <summary>
    /// This test checks that an exception should be thrown when there is an invalid cell name
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameThrown_SetCells()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("@@@", "0");
    }

    /// <summary>
    /// This test checks that an exception should be thrown when trying to grab an invalid cell name
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameThrown_GetCells()
    {
        Spreadsheet s = new Spreadsheet();
        s.GetCellContents("@@@");
    }

    /// <summary>
    /// This test checks that an exception should be thrown when there is an invalid cell name for formulas
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameThrown_SetFormulaCell()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("122", ("=x1 + y1"));
    }

    /// <summary>
    /// This test checks that an exception should be thrown when there is an invalid cell name for strings
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameThrown_SetStringCell()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("xyz", "abc");
    }

    /// <summary>
    /// This test checks that an exception should be thrown when there is an invalid cell name for numbers
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameThrown_SetNumCell()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("Q-Q", "0.001");
    }

    /// <summary>
    /// This test checks that an exception should be thrown when there is a loop of dependencies
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void TestThrowCircularException()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", ("=A3 + A2"));
        s.SetContentsOfCell("A2", ("=A1+1"));
        s.SetContentsOfCell("A3", ("=A2+A1"));
        Assert.IsTrue(s.SetContentsOfCell("A4", "A3 + 1").SequenceEqual([]));
    }

    // --- Overwriting/Replacing Cell Content -------------------------

    /// <summary>
    /// This test checks whether a formula can overwrite a pre-existing cell correctly
    /// </summary>
    [TestMethod]
    public void OverwriteContents_WithFormula()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "10");
        s.SetContentsOfCell("A1", ("=10"));
        Assert.AreEqual(s.GetCellContents("A1"), (new Formula("10")));
    }

    /// <summary>
    /// This test checks whether a number can overwrite a pre-existing cell correctly
    /// </summary>
    [TestMethod]
    public void OverwriteContents_WithNum()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "hi");
        s.SetContentsOfCell("A1", "10");
        Assert.AreEqual(s.GetCellContents("A1"), 10.0);
    }

    /// <summary>
    /// This test checks whether a string can overwrite a pre-existing cell correctly
    /// </summary>
    [TestMethod]
    public void OverwriteContents_WithString()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "10");
        s.SetContentsOfCell("A1", "bro");
        Assert.AreEqual(s.GetCellContents("A1"), "bro");
    }

    /// <summary>
    /// This test checks whether a number can overwrite a cell already containing a number
    /// </summary>
    [TestMethod]
    public void ReplaceNumTest()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "2.4");
        s.SetContentsOfCell("A1", "3.6");
        Assert.AreEqual(s.GetCellContents("A1"), 3.6);
    }

    /// <summary>
    /// This test checks whether a formula can overwrite a cell already containing a formula
    /// </summary>
    [TestMethod]
    public void ReplaceFormulaTest()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", ("=5 + 10"));
        s.SetContentsOfCell("A1", ("=100 * 100"));
        Assert.AreEqual(s.GetCellContents("A1"), (new Formula("100 * 100")));
    }

    /// <summary>
    /// This test checks whether a string can overwrite a cell already containing a string
    /// </summary>
    [TestMethod]
    public void ReplaceStringTest()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "open");
        s.SetContentsOfCell("A1", "closed");
        Assert.AreEqual(s.GetCellContents("A1"), "closed");
    }

    /// <summary>
    /// This test checks whether a string can be removed from a spreadsheet
    /// </summary>
    [TestMethod]
    public void ClearStringTest()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "open");
        s.SetContentsOfCell("A1", "");
        Assert.AreEqual(s.GetCellContents("A1"), string.Empty);
    }

    /// <summary>
    /// This test checks whether a number can be removed from a spreadsheet
    /// </summary>
    [TestMethod]
    public void ClearNumTest()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "10");
        s.SetContentsOfCell("A1", "");
        Assert.AreEqual(s.GetCellContents("A1"), string.Empty);
    }

    /// <summary>
    /// This test checks whether a formula can be removed from a spreadsheet
    /// </summary>
    [TestMethod]
    public void ClearFormulaTest()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", ("=0 + 0"));
        s.SetContentsOfCell("A1", "");
        Assert.AreEqual(s.GetCellContents("A1"), string.Empty);
    }

    // --- PS6 MVC Tests -------------------------

    // --- Cell Value Tests ---------------------
    /// <summary>
    /// This test checks as basic retrieval of a value from a simple formula
    /// </summary>
    [TestMethod]
    public void SimpleGet_FormulaValueTest()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1 + 1");
        Assert.AreEqual(2.0, s.GetCellValue("A1"));
    }

    /// <summary>
    /// This test checks for retrieval of a value based on two cells
    /// </summary>
    [TestMethod]
    public void AddCellsFor_FormulaValueTest()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5.0");
        s.SetContentsOfCell("A2", "3.0");
        s.SetContentsOfCell("A3", "=A1+A2");
        Assert.AreEqual(8.0, s.GetCellValue("A3"));
        Assert.AreEqual(new Formula("A1+A2"), s.GetCellContents("A3"));
    }

    /// <summary>
    /// This test checks for the retrieval of a cell value with other cell dependencies
    /// </summary>
    [TestMethod]
    public void MultipleOps_For_FormulaValueTest()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5.0");
        s.SetContentsOfCell("A2", "3.0");
        s.SetContentsOfCell("A3", "=A1*A2");
        s.SetContentsOfCell("A4", "=A1 + A2 + A3");
        Assert.AreEqual(23.0, s.GetCellValue("A4"));
    }

    /// <summary>
    ///  This test checks where value retrieval works with empty cells (will throw an error object).
    /// </summary>
    [TestMethod]
    public void OverrideFormulaValue_WithEmptyTest()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "3.1");
        s.SetContentsOfCell("A2", "=A1");
        s.SetContentsOfCell("A1", string.Empty);
        Assert.AreEqual(new FormulaError("A1 is Empty").ToString(), s.GetCellValue("A2").ToString());
    }

    /// <summary>
    /// This test checks that no value is returned when grabbing a cell that does not exist.
    /// </summary>
    [TestMethod]
    public void GetNonExistantCellValue()
    {
        Spreadsheet s = new Spreadsheet();
        Assert.AreEqual(string.Empty, s.GetCellValue("A1"));
    }

    // ---- Indexer & Constructor Tests ----------

    /// <summary>
    /// This test checks for if the indexer grabs the values correctly
    /// </summary>
    [TestMethod]
    public void TestIndexer()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "3");
        Assert.AreEqual(s["A1"], 3.0);
    }

    /// <summary>
    /// This test is a simple test to check that the constructor with a parameter creates a spreadsheet with no issues
    /// </summary>
    [TestMethod]
    public void TestOneParameterConstructor()
    {
        Spreadsheet s = new Spreadsheet("Sheet1");
        s.SetContentsOfCell("A1", "bye");
        Assert.AreEqual(s["A1"], "bye");
    }

    // ------ Save and Load File Tests ----------

    /// <summary>
    /// This test checks whether a spreadsheet is saved successfully into a text file. This file can be viewed 
    /// </summary>
    [TestMethod]
    public void SimpleSaveTest()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "1");
        s.SetContentsOfCell("A2", "=A1*A1-A1/A1+A1");
        s.SetContentsOfCell("A3", "bye");
        s.Save("myfile.txt");
    }

    /// <summary>
    /// This test checks for when a bad file name is encountered for saves using no filename
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveBadFileNameTest()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "1");
        s.SetContentsOfCell("A2", "=A1*A1-A1/A1+A1");
        s.SetContentsOfCell("A3", "bye");
        s.Save(string.Empty);
    }


    /// <summary>
    /// This test checks for when a bad file name is encountered for saves using just a dot
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveBadFileNameTest_Dot()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "1");
        s.Save(".");
    }

    /// <summary>
    /// This test checks for when a bad file name is encountered for saves using a nonsense path
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveBadFileNameTest_NonsensePath()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "1");
        s.Save("/random/lalala/cube/nonsensePath.txt");
    }

    /// <summary>
    /// This test checks for when a bad file name is encountered for loads using a dot as a path
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void LoadBadFileName_Dot()
    {
        string expectedOutput = @"
        {
          ""Cells"": {
            ""A1"": {
              ""StringForm"": ""1""
            },
          }
        }";

        File.WriteAllText("success.txt", expectedOutput);
        Spreadsheet s = new Spreadsheet();
        s.Load(".");
    }

    /// <summary>
    /// This test checks for when a bad file name is encountered for loads using a nonsense path
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void LoadBadFileName_Nonsense()
    {
        string expectedOutput = @"
        {
          ""Cells"": {
            ""A1"": {
              ""StringForm"": ""1""
            },
          }
        }";

        File.WriteAllText("success.txt", expectedOutput);
        Spreadsheet s = new Spreadsheet();
        s.Load("/path/pathy/nonsensepath.txt");
    }

    /// <summary>
    /// This test checks for when a bad file name is encountered for loads
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void LoadBadFileNameTest()
    {
        Spreadsheet s = new Spreadsheet();
        s.Load(string.Empty);
    }
    
    /// <summary>
    /// This test checks when for if a empty spreadsheet can be successfully loaded 
    /// </summary>
    [TestMethod]
    public void EmptyLoadTest()
    {
        string expectedOutput = "{ \"cells\": {} }";

        File.WriteAllText("known_values.txt", expectedOutput);

        Spreadsheet s = new Spreadsheet();
        s.Load("known_values.txt");
        Assert.IsTrue(s.GetNamesOfAllNonemptyCells().SetEquals([]));
    }

    /// <summary>
    /// This test checks for when a Json string file is loaded correctly into a new spreadsheet and the values are correct.
    /// </summary>
    [TestMethod]
    public void SimpleLoadTest_CheckValues()
    {
        string expectedOutput = @"
        {
          ""Cells"": {
            ""A1"": {
              ""StringForm"": ""1""
            },
            ""A2"": {
              ""StringForm"": ""=A1*A1-A1/A1+A1""
            },
            ""A3"": {
              ""StringForm"": ""bye""
            }
          }
        }";

        File.WriteAllText("save.txt", expectedOutput);

        Spreadsheet s = new Spreadsheet();
        s.Load("save.txt");
        Assert.AreEqual(s.GetCellValue("A2"), 1.0);
        Assert.AreEqual(s.GetCellValue("A3"), "bye");
        Assert.AreEqual(s.GetCellValue("A1"), 1.0);
    }

    /// <summary>
    /// This test checks for when a Json string file is loaded correctly into a new spreadsheet and the contents are corrent.
    /// </summary>
    [TestMethod]
    public void SimpleLoadTest_CheckContents()
    {
        string expectedOutput = @"
        {
          ""Cells"": {
            ""A1"": {
              ""StringForm"": ""1""
            },
            ""A2"": {
              ""StringForm"": ""=A1*A1-A1/A1+A1""
            },
            ""A3"": {
              ""StringForm"": ""bye""
            }
          }
        }";

        File.WriteAllText("save.txt", expectedOutput);

        Spreadsheet s = new Spreadsheet();
        s.Load("save.txt");
        Assert.AreEqual(s.GetCellContents("A2"), new Formula("A1*A1-A1/A1+A1"));
        Assert.AreEqual(s.GetCellContents("A3"), "bye");
        Assert.AreEqual(s.GetCellContents("A1"), 1.0);
    }
    
    // --- Stress Tests -----

    /// <summary>
    /// This stress test checks the speed of grabbing the contents of a cell in dependency chain
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    public void StressTest_SetContents()
    {
        Spreadsheet s = new();
        HashSet<string> cells = new();
        for (int i = 1; i < 200; i++)
        {
            cells.Add("A" + i);
            Assert.IsTrue(cells.SetEquals(s.SetContentsOfCell("A" + i, ("=A" + (i + 1)))));
        }
    }

    /// <summary>
    /// This stress test checks the speed of grabbing a cell value in a dependency chain
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    public void StressTest_GetCellValue()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "0");
        for (int i = 2; i < 1000; i++)
        {
            s.SetContentsOfCell($"A{i}", $"=A{i - 1}");
        }
        Assert.AreEqual(s.GetCellValue("A999"), 0.0);
    }

    /// <summary>
    /// This stress test checks the speed of grabbing a cell value in a very long dependency chain
    /// </summary>
    [TestMethod]
    [Timeout(4000)]
    public void StressTest_GetCellValue_WithLargerChain()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "5");
        for (int i = 2; i < 10000; i++)
        {
            s.SetContentsOfCell($"A{i}", $"=A{i - 1}");
        }
        Assert.AreEqual(s.GetCellValue("A9999"), 5.0);
    }

    /// <summary>
    /// This stress test checks the speed of updating a cell in a long dependency chain
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    public void StressTest_GetCellValue_ThenUpdate()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "0");
        for (int i = 2; i < 1000; i++)
        {
            s.SetContentsOfCell($"A{i}", $"=A{i - 1}");
        }
        s.SetContentsOfCell("A1", "1");
        Assert.AreEqual(s.GetCellValue("A999"), 1.0);
    }

    /// <summary>
    /// This stress test checks on the speed of updating a cell in a long chain
    /// </summary>
    [TestMethod]
    [Timeout(3000)]
    public void StressTest_GetCellValue_ThenUpdate_LargeChain()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "0");
        for (int i = 2; i < 5000; i++)
        {
            s.SetContentsOfCell($"A{i}", $"=A{i - 1}");
        }
        s.SetContentsOfCell("A1", "1");
        Assert.AreEqual(s.GetCellValue("A4999"), 1.0);
    }

    /// <summary>
    /// This stress test checks on the speed of more than one reference to another cell in a chain
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    public void StressTest_GetCellValue_LargerFormula()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "0");
        s.SetContentsOfCell("A2", "0");
        for (int i = 3; i < 1000; i++)
        {
            s.SetContentsOfCell($"A{i}", $"=A{i - 1}  + A{i - 2}");
        }
        Assert.AreEqual(s.GetCellValue("A999"), 0.0);
    }
}