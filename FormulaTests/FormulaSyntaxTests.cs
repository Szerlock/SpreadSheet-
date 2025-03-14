// <copyright file="FormulaSyntaxTests.cs" company="UofU-CS3500">
//   Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Andy Tran </authors>
// <date> September 5, 2024 </date>

namespace CS3500.Formula;

using CS3500.Formula;

/// <summary>
///   <para>
///     The following class tests the formula constructor of PS2,
///   </para>
/// </summary>
[TestClass]
public class FormulaSyntaxTests
{
    // --- Tests for One Token Rule ---

    /// <summary>
    ///   <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with no tokens.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNoTokens_Invalid()
    {
        _ = new Formula(string.Empty);
    }


    // --- Tests for Valid Token Rule ---

    /// <summary>
    ///   <para>
    ///     This test makes sure that basic integers are valid tokens for our formula (This will not throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestIntegers_Valid()
    {
        _ = new Formula("11 + 200");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that negative numbers are not valid tokens for our formula (This will throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNegInts_Invalid()
    {
        _ = new Formula("-5 * -17");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that exponential expressions are valid tokens (This will not
    ///     throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestExponentialNums_Valid()
    {
        _ = new Formula("3e6 * 17e-5");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that floating point numbers are valid tokens (This will not throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFloatingPoints_Valid()
    {
        _ = new Formula("3.14 / 194.58132");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that only valid operators ( (, ), +, -, *, /) are accepted (This will
    ///     throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperators_Invalid()
    {
        _ = new Formula("5 ^ 8 & 14 @ 1");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that invalid tokens will not be accepted by the constructor (This will
    ///     throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariables_Invalid()
    {
        _ = new Formula("a1a * 1a");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that variables (1 or 2 more letters followed by 1 or 2 numbers) are valid tokens (This will not
    ///     throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestVariables_Valid()
    {
        _ = new Formula("a12 * AjBe9");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that parentheses are valid tokens, assuming integers are already valid (This will not
    ///     throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestParenthesisAsTokens_Valid()
    {
        _ = new Formula("(6)");
    }

    // --- Tests for Closing Parenthesis Rule --

    /// <summary>
    ///   <para>
    ///     This test makes sure that there are no more closing parenthesis than there are opening parenthesis seen so far (This will
    ///     throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestGreaterClosingParenthesis_Invalid()
    {
        _ = new Formula("))10((");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that there are no more closing parenthesis than there are opening parenthesis seen so far (This will
    ///     throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestParenthesis_InWrongOrder()
    {
        _ = new Formula("(1* 6)) * (4 + (8)");
    }

    /// <summary>
    ///   <para>
    ///     This test is to show a valid case where there are no more closing parenthesis than opening parenthesis seen (This will not
    ///     throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestValidParenthesisOrder()
    {
        _ = new Formula("(((1 * 3) + 5) + 6)");
    }

    // --- Tests for Balanced Parentheses Rule -- 

    /// <summary>
    ///   <para>
    ///     This test makes sure that there are an equal amount of closing and parenthesis present (This will
    ///     throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestEqualParenthesis_Valid()
    {
        _ = new Formula("(1 + (6*7))");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that multiple sets of balanced parenthesis will not invalidate a formula (This will not
    ///     throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestManyParenthesis_Valid()
    {
        _ = new Formula("(((((10)))))");
    }

    /// <summary>
    ///   <para>
    ///     This test is to verify that multiple parenthesis with no closure will cause for an exception (This will
    ///     throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]

    public void FormulaConstructor_TestOpeningParenthesis_Invalid()
    {
        _ = new Formula("((6");
    }

    // --- Tests for First Token Rule --

    /// <summary>
    ///   <para>
    ///     Make sure a simple well formed formula is accepted by the constructor (the constructor
    ///     should not throw an exception).
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
    ///     In other words, the formula "1+1" is a valid formula which should not cause any errors.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenNumber_Valid()
    {
        _ = new Formula("1+1");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that an opening parenthesis as a first token is accepted by the constructor. (This will not
    ///     throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestParenthesis_Valid()
    {
        _ = new Formula("(1)+1");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that a valid variable as a first token is accepted by the constructor. (This will will not throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenVariable_Valid()
    {
        _ = new Formula("abc1 + 8");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that an operator as the first token is not accepted by the constructor. (This will will throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstTokenOperator_Valid()
    {
        _ = new Formula("* 7 + 5");
    }

    // --- Tests for  Last Token Rule ---

    /// <summary>
    ///   <para>
    ///     This test makes sure that an number as a last token is accepted by the constructor. (This will not
    ///     throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestLastTokenNumber_Valid()
    {
        _ = new Formula("5/5 + 10");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that a valid variable as a last token is accepted by the constructor. (This will not
    ///     throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestLastTokenVariable_Valid()
    {
        _ = new Formula("aab12 - ss2");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that an closing parenthesis as a last token is accepted by the constructor. (This will not
    ///     throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestLastTokenParenthesis_Valid()
    {
        _ = new Formula("((abc1*1) + 0)");
    }

    /// <summary>
    ///   <para>
    ///     This is a test that makes sure that an invalid token as a last token will not be accepted by the constructor
    ///     (This will thrown an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastToken_Invalid()
    {
        _ = new Formula("123 * a");
    }

    /// <summary>
    ///   <para>
    ///     This is a test that makes sure that an operator as a last token will not be accepted by the constructor
    ///     (This will thrown an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenOperator_Invalid()
    {
        _ = new Formula("ab53 -");
    }

    // --- Tests for Parentheses/Operator Following Rule ---

    /// <summary>
    ///   <para>
    ///     This is a test that makes sure that operators cannot follow parenthesis (This will thrown an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorFollowingParenthesis_Invalid()
    {
        _ = new Formula("(+)");
    }

    /// <summary>
    ///   <para>
    ///     This is a test that makes sure that operators cannot follow operators (This will thrown an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorFollowingOp_Invalid()
    {
        _ = new Formula("5--8");
    }

    /// <summary>
    ///   <para>
    ///     This is a test that makes sure that valid tokens must follow operators (This will thrown an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestTokenFollowingOp_Invalid()
    {
        _ = new Formula("1/ ");
    }

    /// <summary>
    ///   <para>
    ///     This is a test that makes sure that valid tokens must follow opening parenthesis (This will thrown an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestTokenFollowingParenthesis_Invalid()
    {
        _ = new Formula("( +6)");
    }

    /// <summary>
    ///   <para>
    ///     This is a test that makes sure that opening parenthesis can follow opening parenthesis (This will not thrown an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestParenFollowingParen_Valid()
    {
        _ = new Formula("((5 * 6) + 6)");
    }

    // --- Tests for Extra Following Rule ---

    /// <summary>
    ///   <para>
    ///     This test makes sure that any valid token following a valid token (number) must be an operator or closing parenthesis
    ///     (This will throw an exception).
    ///     </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNoOperator_Invalid()
    {
        _ = new Formula("5 5");
    }


    /// <summary>
    ///   <para>
    ///     This is a test that makes sure that a opening parenthesis or an operator must follow after
    ///     a closing parenthesis (This will thrown an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesis_Invalid()
    {
        _ = new Formula("(a67+23)abc");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that any valid token following a closing parenthesis can be another closing parenthesis
    ///     (This will not throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestClosingFollowingClosingParen_Valid()
    {
        _ = new Formula("(16 + (5 * 5))");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that any token following a variable must be an operator or closing parenthesis
    ///     (This will throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestTokenFollowVariable_Invalid()
    {
        _ = new Formula("(yo1 aba56");
    }

    /// --- New Syntax Tests for PS2 --- 

    /// <summary>
    ///   <para>
    ///     This test makes sure that only parenthesis will not pass the constructor
    ///     (This will throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_OnlyParenthesis()
    {
        _ = new Formula("()");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure that there should be an operator in between parenthesis
    ///     (This will throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_NoOperator()
    {
        _ = new Formula("(5)(8)");
    }

    /// --- Tests for New Formula Class ---

    /// <summary>
    ///   <para>
    ///     This test checks the ToStringMethod on a simple formula
    ///     (This will not throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_SimpleToStringTest()
    {
        Formula formula = new Formula("ab1 + 5");
        String expected = formula.ToString();
        Assert.AreEqual(expected, "AB1+5");
    }

    /// <summary>
    ///   <para>
    ///     This test checks the ToStringMethod on a longer formula
    ///     (This will not throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_LongerToStringTest()
    {
        Formula formula = new Formula("((sq3 * 5.301) + 35)");
        String expected = formula.ToString();
        Assert.AreEqual(expected, "((SQ3*5.301)+35)");
    }

    /// <summary>
    ///   <para>
    ///     This test checks the ToStringMethod to make sure it normalizes numbers other than integers
    ///     (This will not throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_NormalizeNums_ToStringTest()
    {
        Formula formula = new Formula("5.3000 + 1e3");
        String expected = formula.ToString();
        Assert.AreEqual(expected, "5.3+1000");
    }

    /// <summary>
    ///   <para>
    ///     This test checks the GetVariables method on one variable
    ///     (This will not throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_SingleGetVariablesTest()
    {
        Formula formula = new Formula("ab1 + 5");

        HashSet<string> set = new HashSet<string>();
        set.Add("AB1");

        Assert.IsTrue(set.SetEquals(formula.GetVariables()));
    }


    /// <summary>
    ///   <para>
    ///     This test checks the GetVariables method on multiple variables
    ///     (This will not throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_MultipleGetVariablesTest()
    {
        Formula formula = new Formula("(ab14 / asag3 * sgdsgn456)");

        HashSet<string> set = new HashSet<string>();
        set.Add("AB14");
        set.Add("ASAG3");
        set.Add("SGDSGN456");

        Assert.IsTrue(set.SetEquals(formula.GetVariables()));
    }

    /// <summary>
    ///   <para>
    ///     This test checks the GetVariables method on duplicate variables
    ///     (This will not throw an exception).
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_DuplicateVar_GetVariablesTest()
    {
        Formula formula = new Formula("xyz9 + XYZ9");

        HashSet<string> set = new HashSet<string>();
        set.Add("XYZ9");

        Assert.IsTrue(set.SetEquals(formula.GetVariables()));
    }
}
   