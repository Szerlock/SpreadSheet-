// <copyright file="EvaluationTests.cs" company="UofU-CS3500">
//   Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Andy Tran </authors>
// <date> September 19, 2024 </date>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS3500.Formula;

using CS3500.Formula;
using System.Diagnostics.Contracts;
using System.Xml;

/// <summary>
/// This is a test class to specifically test and ensure correctness of the Evaluate, Equals, Not Equals, and Hashcode Methods
/// within the Formula Class
/// </summary>
[TestClass]
public class EvaluationTests
{

    // ---- LookUp Functions ------

    /// <summary>
    /// This is a simple (throwaway) lookup function that an evaluate fuction is expecting.
    /// </summary>
    /// <param name="name"> the name of a variable. </param>
    /// <returns> returns 5 if the name is "A1" and throw and error otherwise. </returns>
    /// <exception cref="ArgumentException"> is thrown when the variable cannot be found. </exception>
    double MyVar(string name)
    {
        if (name == "A1")
        {
            return 5;
        }
        else
        {
            throw new ArgumentException("I don't know this variable");
        }

    }

    /// <summary>
    /// This is a simple lookup function that returns a number when given a string.
    /// </summary>
    /// <param name="name"> the name of the variable. </param>
    /// <returns> returns 5 if the name is "A1" and returns 10 otherwise. </returns>
    double FiveAsVal(string name)
    {
        if (name == "A1")
        {
            return 5;
        }
        else
        {
            return 10;
        }
    }

    // ----- Formula Evaluate Tests --------

    /// <summary>
    /// This is a simple test to check that the lookup function functions as intended.
    /// </summary>
    [TestMethod]
    public void SimpleLookUpTest()
    {
        Formula formula = new("A1 + 1");
        Assert.AreEqual(formula.Evaluate(MyVar), 6.0);
    }

    /// <summary>
    /// This test checks for if the evaluate method can do basic addition.
    /// </summary>
    [TestMethod]
    public void EvaluateSimpleAddition()
    {
        Formula formula = new("1+1");
        Assert.AreEqual(formula.Evaluate(MyVar), 2.0);
    }
    
    /// <summary>
    /// This test checks for if the evaluate method can add different numbers together.
    /// </summary>
    [TestMethod]
    public void AddDifferentNums()
    {
        Formula formula = new("3+2");
        Assert.AreEqual(formula.Evaluate(MyVar), 5.0);
    }

    /// <summary>
    /// This test checks for if the evaluate method can do basic subtraction.
    /// </summary>
    [TestMethod]
    public void EvaluateSimpleSubtraction()
    {
        Formula formula = new("1-1");
        Assert.AreEqual(formula.Evaluate(MyVar), 0.0);

    }

    /// <summary>
    /// This test checks for if the evaluate method can subtract different numbers in the correct order.
    /// </summary>
    [TestMethod]
    public void SubtractDifferentNums()
    {
        Formula formula = new("7-6");
        Assert.AreEqual(formula.Evaluate(MyVar), 1.0);
    }

    /// <summary>
    /// This test checks if the evaluate method can do basic multiplication.
    /// </summary>
    [TestMethod]
    public void EvaluateSimpleMultiplication()
    {
        Formula formula = new("1*1");
        Assert.AreEqual(formula.Evaluate(MyVar), 1.0);
    }

    /// <summary>
    /// This test checks if the evaluate method can multiply different numbers together.
    /// </summary>
    [TestMethod]
    public void MultiplyDifferentNums()
    {
        Formula formula = new("5*4");
        Assert.AreEqual(formula.Evaluate(MyVar), 20.0);
    }

    /// <summary>
    /// This test check if the evaluate method can perform basic division.
    /// </summary>
    [TestMethod]
    public void EvaluateSimpleDivision()
    {
        Formula formula = new("1/1");
        Assert.AreEqual(formula.Evaluate(MyVar), 1.0);
    }

    /// <summary>
    /// This test check if the evaluate method can divide different numbers in the correct order.
    /// </summary>
    [TestMethod]
    public void DivideDifferentNums()
    {
        Formula formula = new("10/2");
        Assert.AreEqual(formula.Evaluate(MyVar), 5.0);
    }

    /// <summary>
    /// This test check if the evaluate method correctly performs with parentheses for multiplication and division.
    /// </summary>
    [TestMethod]
    public void EvaluateWithParentheses_MultiplyAndDivide()
    {
        Formula formula = new("(1*1) / (1+1)");
        Assert.AreEqual(formula.Evaluate(MyVar), 0.5);
    }

    /// <summary>
    /// This test check if the evaluate method correctly performs with parenthesis for addition and subtraction.
    /// </summary>
    [TestMethod]
    public void EvaluateWithParentheses_AddAndSubtract()
    {
        Formula formula = new("(5+2) - (5+2)");
        Assert.AreEqual(formula.Evaluate(MyVar), 0.0);
    }

    /// <summary>
    /// This test check if the evaluate method correctly performs with parenthesis with mulitiplication as its final computation
    /// </summary>
    [TestMethod]
    public void EvaluateWithParentheses_MultiplicationAtEnd()
    {
        Formula formula = new("(5*5) * 2");
        Assert.AreEqual(formula.Evaluate(MyVar), 50.0);
    }

    /// <summary>
    /// This test check if the evaluate method correctly performs with parenthesis with division as its final computation
    /// </summary>
    [TestMethod]
    public void EvaluateWithParentheses_DivisionAtEnd()
    {
        Formula formula = new("(5*10) / 2");
        Assert.AreEqual(formula.Evaluate(MyVar), 25.0);
    }

    /// <summary>
    /// This test check if the evaluate method correctly performs with parenthesis with addition as its final computation
    /// </summary>
    [TestMethod]
    public void EvaluateWithParentheses_AdditionAtEnd()
    {
        Formula formula = new("(5*5) + 2");
        Assert.AreEqual(formula.Evaluate(MyVar), 27.0);
    }

    /// <summary>
    /// This test check if the evaluate method correctly performs with parenthesis with subtraction as its final computation
    /// </summary>
    [TestMethod]
    public void EvaluateWithParentheses_SubtractionAtEnd()
    {
        Formula formula = new("(5*5) - 2");
        Assert.AreEqual(formula.Evaluate(MyVar), 23.0);
    }

    /// <summary>
    /// This test check if the evaluate method correctly performs with parenthesis with mulitiplication as its beginning computation
    /// </summary>
    [TestMethod]
    public void EvaluateWithParentheses_MultipliationAtStart()
    {
        Formula formula = new("5 * (5*3)");
        Assert.AreEqual(formula.Evaluate(MyVar), 75.0);
    }

    /// <summary>
    /// This test check if the evaluate method correctly performs with parenthesis with division as its starting computation
    /// </summary>
    [TestMethod]
    public void EvaluateWithParentheses_DivisionAtStart()
    {
        Formula formula = new("100 / (10/2)");
        Assert.AreEqual(formula.Evaluate(MyVar), 20.0);
    }

    /// <summary>
    /// This test check if the evaluate method correctly performs with parenthesis with division as its starting computation, but it expects
    /// an error to return as an object
    /// </summary>
    [TestMethod]
    public void EvaluateWithParentheses_DivisionAtStart_ZeroError()
    {
        Formula formula = new("100 / (5 - 10/2)");
        Assert.ReferenceEquals(formula.Evaluate(MyVar), new FormulaError("Cannot Divide By Zero"));
    }

    /// <summary>
    /// This test check if the evaluate method can add multiple numbers together consecutively.
    /// </summary>
    [TestMethod]
    public void ConsecutiveAdds()
    {
        Formula formula = new("1+1+1");
        Assert.AreEqual(formula.Evaluate(MyVar), 3.0);
    }

    /// <summary>
    /// This test check if the evaluate method can divide multiple numbers together consecutively.
    /// </summary>
    [TestMethod]
    public void ConsecutiveDivides()
    {
        Formula formula = new("20/5/2");
        Assert.AreEqual(formula.Evaluate(MyVar), 2.0);
    }

    /// <summary>
    /// This test checks if the evaluate method can multiply multiple numbers together consecutively.
    /// </summary>
    [TestMethod]
    public void ConsecutiveMultiplies()
    {
        Formula formula = new("3*4*5");
        Assert.AreEqual(formula.Evaluate(MyVar), 60.0);
    }

    /// <summary>
    /// This test check if the evaluate method can subtract multiple numbers together consecutively.
    /// </summary>
    [TestMethod]
    public void ConsecutiveSubs()
    {
        Formula formula = new("2-1-1");
        Assert.AreEqual(formula.Evaluate(MyVar), 0.0);
    }

    /// <summary>
    /// This test checks if the evaluate method can catch division by zero errors.
    /// </summary>
    [TestMethod]
    public void EvaluateDivideByZero_Invalid()
    {
        Formula formula = new("1/0");
        Assert.ReferenceEquals(formula.Evaluate(MyVar), new FormulaError("Cannot Divide By Zero"));
    }

    /// <summary>
    /// This test checks if the evaluate method can catch when an unknown variable is processed, and returning an error in the process.
    /// </summary>
    [TestMethod]
    public void EvaluateWithUnknownVar_Invalid()
    {
        Formula formula = new("1 + X1");
        Assert.ReferenceEquals(formula.Evaluate(MyVar), new FormulaError("Unknown Variable"));
    }

    /// <summary>
    /// This test checks if the evaluate method can correctly compute an equation with one variable.
    /// </summary>
    [TestMethod]
    public void EvaluateWithOneVariable()
    {
        Formula formula = new("1+A1");
        Assert.AreEqual(formula.Evaluate(FiveAsVal), 6.0);
    }

    /// <summary>
    /// This test checks if the evaluate method can correctly compute an equation with more than one variable. 
    /// </summary>
    [TestMethod]
    public void EvaluateWithMultipleVariables()
    {
        Formula formula = new("A1 + B1");
        Assert.AreEqual(formula.Evaluate(FiveAsVal), 15.0);
    }

    // ----- Equality & HashCode Tests --------

    /// <summary>
    /// This test checks the behavior of the double equals method on different formulas. The result should be false.
    /// </summary>
    [TestMethod]
    public void DoubleEqualsOperator_False()
    {
        Formula f1 = new("1/1");
        Formula f2 = new("1*1");
        Assert.IsFalse(f1 == f2);
    }

    /// <summary>
    /// This test checks the behavior of the double equals method on the same formulas. The result should be true.
    /// </summary>
    [TestMethod]
    public void DoubleEqualsOperator_True()
    {
        Formula f1 = new("x1+x2");
        Formula f2 = new("X1+X2");
        Assert.IsTrue(f1 == f2);
    }

    /// <summary>
    /// This test checks the behavior of the not equals method on different formulas. The result should be true.
    /// </summary>
    [TestMethod]
    public void NotEqualsOperator_True()
    {
        Formula f1 = new("1-1");
        Formula f2 = new("1+1");
        Assert.IsTrue(f1 != f2);
    }

    /// <summary>
    /// This test checks the behavior of the not equals method on same formulas. The result should be false.
    /// </summary>
    [TestMethod]
    public void NotEqualsOperator_False()
    {
        Formula f1 = new("x3 - 2");
        Formula f2 = new("X3 - 2");
        Assert.IsFalse(f1 != f2);
    }

    /// <summary>
    /// This test checks the behavior of the equals method on the same formulas. The result should be true.
    /// </summary>
    [TestMethod]
    public void EqualsMethod()
    {
        Formula f1 = new("1 + 1");
        Formula f2 = new("1 + 1");
        Assert.IsTrue(f1.Equals(f2));
    }

    /// <summary>
    /// This test checks the behavior of the equals method on a formula and a null object. The result should be false.
    /// </summary>
    [TestMethod]
    public void EqualsToNull()
    {
        Formula f1 = new("1*2");
        Formula? f2 = null;
        Assert.IsFalse(f1.Equals(f2));
    }

    /// <summary>
    /// This test checks the behavior of the equals method on formulas with different lengths. The result should be false
    /// </summary>
    [TestMethod]
    public void EqualsOn_DifferentLengths()
    {
        Formula f1 = new("10/5");
        Formula f3 = new("(10/5)");
        Assert.IsFalse(f1.Equals(f3));
    }

    /// <summary>
    /// This test checks the behavior of the equals method on formulas with spaces. The result should be true.
    /// </summary>
    [TestMethod]
    public void EqualsWithSpaces_Valid()
    {
        Formula f1 = new("1 +            1");
        Formula f2 = new("1+1");
        Assert.IsTrue(f1.Equals(f2));
    }

    /// <summary>
    /// This test checks the behavior of the equals method on larger formulas. The result should be true.
    /// </summary>
    [TestMethod]
    public void EqualsOn_LargerOperation()
    {
        Formula f1 = new("(3e15 + 134) - (124 + 3.0)");
        Formula f2 = new("(3e15 + 134) - (124 + 3.0)");
        Assert.IsTrue(f1.Equals(f2));
    }

    /// <summary>
    /// This test checks the behavior of the equals method with variables. The result should be false.
    /// </summary>
    [TestMethod]
    public void EqualsMethodToFail()
    {
        Formula f1 = new("x3 + x2");
        Formula f2 = new("x1 + x2");
        Assert.IsFalse(f1.Equals(f2));
    }

    /// <summary>
    /// This test compares two different formulas based on their hashcodes to ensure they have different hashcodes.
    /// </summary>
    [TestMethod]
    public void RetrieveHashCode_DifferentFormulas()
    {
        Formula f1 = new("1+1");
        Formula f2 = new("1*1");
        Assert.IsTrue(f1.GetHashCode() != f2.GetHashCode());
    }

    /// <summary>
    /// This test compares two of the same formulas based on their hashcodes to ensure that they have the same hashcode.
    /// </summary>
    [TestMethod]
    public void RetrieveHashCode_SameFormulas()
    {
        Formula f1 = new("1+2");
        Formula f2 = new("1+2");
        Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
    }

    // -------------- Lambda Expression Tests -----------------

    /// <summary>
    /// This test uses a lambda expression and tests for the Divide by Zero error when a variable has zero as its value.
    /// </summary>
    [TestMethod]
    public void DivideByZero_Var()
    {
        Formula f1 = new("10/x1");
        Assert.ReferenceEquals(f1.Evaluate(name => 0), new FormulaError("Cannot Divide By Zero"));
    }

    /// <summary>
    /// This test checks for a Divide By Zero where the zero is followed by a closing parenthesis.
    /// </summary>
    [TestMethod]
    public void DivideByZero_ClosingParen()
    {
        Formula f1 = new("(10/0)");
        Assert.ReferenceEquals(f1.Evaluate(name => 0), new FormulaError("Cannot Divide By Zero"));
    }
    /// <summary>
    /// This test checks for a basic evaluation check using a lambda expression.
    /// </summary>
    [TestMethod]
    public void EvalulateWithLambda()
    {
        Formula formula = new("X1 * 2");
        Assert.AreEqual(formula.Evaluate((name) => 5), 10.0);
    }

    /// <summary>
    /// This test checks for a basic add operation with a lambda expression.
    /// </summary>
    [TestMethod]
    public void EvaluateAdd_TwoVar_WithLambda()
    {
        Formula formula = new("x1 + x2");
        Assert.AreEqual(formula.Evaluate((name) => 10), 20.0);
    }

    /// <summary>
    /// This test checks for multiplication with a lambda expression.
    /// </summary>
    [TestMethod]
    public void EvaluateMultiply_TwoVar_WithLambda()
    {
        Formula formula = new("x1 * x2");
        Assert.AreEqual(formula.Evaluate((name) => 10), 100.0);
    }

    /// <summary>
    /// This test checks for subtraction with a lambda expression.
    /// </summary>
    [TestMethod]
    public void EvaluateSubtract_TwoVar_WithLambda()
    {
        Formula formula = new("x1 - x2");
        Assert.AreEqual(formula.Evaluate((name) => 10), 0.0);
    }
    
    /// <summary>
    /// This test checks for a division operation with a lambda expression. 
    /// </summary>
    [TestMethod]
    public void EvaluateDivide_TwoVar_WithLambda()
    {
        Formula formula = new("x1 / x2");
        Assert.AreEqual(formula.Evaluate((name) => 10), 1.0);
    }

    /// <summary>
    /// This test checks for an simple evaluation with parenthesis.
    /// </summary>
    [TestMethod]
    public void Simple_ParenthesesEvaluation()
    {
        Formula formula = new("(1+1)");
        Assert.AreEqual(formula.Evaluate(MyVar), 2.0);
    }

    // The Equations Given Below were given and evaluated via ChatGPT. The Test itself and Code Written was NOT written with GPT, only the string (equation) and the result

    /// <summary>
    /// This test checks for the evaluation of a longer (more complex) formula with only numbers.
    /// </summary>
    [TestMethod]
    public void EvaluateComplexEquations_OnlyNumbers()
    {
        Formula formula = new("((9 * 12) / (2 + 3)) + (25 / 8 - 3.125)");
        Assert.AreEqual(formula.Evaluate(MyVar), 21.6);
    }

    /// <summary>
    /// This test checks for another evaluation of a formula with a different arrangement of numbers and operators.
    /// </summary>
    [TestMethod]
    public void EvaluateComplexEquation()
    {
        Formula formula = new("((12 + 8) * (5 - 3)) / 2 - (9 / 3) + 7 * (1 - 0.5)");
        Assert.AreEqual(formula.Evaluate(MyVar), 20.5);
    }

    /// <summary>
    /// This test checks for the evaluation of a longer (more complex) formula with numbers and variables.
    /// </summary>
    [TestMethod]
    public void EvaluateComplexEquation_WithVariables()
    {
        Formula formula = new("((x1 + 2 * y1) * (3 - z1)) / 4 - (5 * x1 - 2) + (y1 - z1)");
        Assert.AreEqual(formula.Evaluate((name) => 5), -30.5);
    }
}
