// <copyright file="Formula.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
// <authors>
//   Solution by: Joe Zachary, Daniel Kopta, Jim de St. Germain, Travis Martin
//   Implemented by: Andy Tran
// </authors>
// <date> September 19, 2024 </date>
// </summary>

namespace CS3500.Formula;

using System.Collections;
using System.ComponentModel.Design;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Data;

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules.  The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one ore more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
///   <para>
///     Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
///     a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
///     and "x 23" consists of a variable "x" and a number "23".  Otherwise, spaces are to be removed.
///   </para>
/// </summary>
public class Formula
{
    /// <summary>
    ///   All variables are letters followed by numbers.  This pattern
    ///   represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";

    private List<string> formulaAsList; // formula as a list, NOT normalized
    private string canocialFormula; // formula as one string but IS normalized

    private List<string> normalizedFormAsList; // formula as a list, IS normalized

    /// <summary>
    ///   Initializes a new instance of the <see cref="Formula"/> class.
    ///   <para>
    ///     Creates a Formula from a string that consists of an infix expression written as
    ///     described in the class comment.  If the expression is syntactically incorrect,
    ///     throws a FormulaFormatException with an explanatory Message.  See the assignment
    ///     specifications for the syntax rules you are to implement.
    ///   </para>
    ///   <para>
    ///     Non Exhaustive Example Errors:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///        Invalid variable name, e.g., x, x1x  (Note: x1 is valid, but would be normalized to X1)
    ///     </item>
    ///     <item>
    ///        Empty formula, e.g., string.Empty
    ///     </item>
    ///     <item>
    ///        Mismatched Parentheses, e.g., "(("
    ///     </item>
    ///     <item>
    ///        Invalid Following Rule, e.g., "2x+5"
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to be created.</param>
    public Formula(string formula)
    {
        // Check if there is at least one token
        if (formula.Length > 0)
        {
            List<string> formulaList = GetTokens(formula);

            int formulaLength = formulaList.Count();

            // Check the case to where there may be spaces in an empty string
            if (formulaLength == 0)
            {
                throw new FormulaFormatException("Cannot be empty");
            }

            // Check the first token for a parenthesis, variable, or number
            if (HasNoValidToken(0, formulaList, "("))
            {
                throw new FormulaFormatException("The formula must start with an opening parenthesis, variable, or number.");
            }

            // Check the last token for a parenthesis, variable, or number
            if (HasNoValidToken(formulaLength - 1, formulaList, ")"))
            {
                throw new FormulaFormatException("The formula must end with an closing parenthesis, variable, or number.");
            }

            // Keep track of parenthesis count
            int openingParenCount = 0;
            int closingParenCount = 0;

            // Loop through and check for the validity of each token
            foreach (string token in formulaList)
            {
                // Case to check for variable tokens
                if (IsVar(token))
                {
                    int currentIndex = formulaList.IndexOf(token);
                    if (currentIndex + 1 < formulaList.Count)
                    {
                        if (HasNoOperatorOrParen(currentIndex, formulaList, ")"))
                        {
                            throw new FormulaFormatException("This is not a valid variable token");
                        }
                    }
                }

                // Case to check operator tokens
                if (IsOperator(token))
                {
                    int currentIndex = formulaList.IndexOf(token);
                    if (currentIndex + 1 < formulaList.Count)
                    {
                        if (HasNoValidToken(currentIndex + 1, formulaList, "("))
                        {
                            throw new FormulaFormatException("There needs to be a number, variable or opening parenthesis following this operator");
                        }
                    }
                }

                // Case to check number tokens
                if (IsDouble(token))
                {
                    int currentIndex = formulaList.IndexOf(token);
                    if (currentIndex + 1 < formulaList.Count)
                    {
                        if (HasNoOperatorOrParen(currentIndex, formulaList, ")"))
                        {
                            throw new FormulaFormatException("There needs to be an operator or closing parenthesis after this number");
                        }
                    }
                }

                // Case to check opening parenthesis
                if (token == "(")
                {
                    openingParenCount++;
                    int currentIndex = formulaList.IndexOf(token);
                    if (currentIndex + 1 < formulaList.Count)
                    {
                        if (HasNoValidToken(currentIndex + 1, formulaList, "("))
                        {
                            throw new FormulaFormatException("There needs to be a number, variable or opening parenthesis following this operator");
                        }
                    }
                }

                // Case to check closing parenthesis
                if (token == ")")
                {
                    closingParenCount++;
                    int currentIndex = formulaList.IndexOf(token);
                    if (currentIndex + 1 < formulaList.Count)
                    {
                        if (HasNoOperatorOrParen(currentIndex, formulaList, ")"))
                        {
                            throw new FormulaFormatException("There needs to be a number, variable or closing parenthesis following this operator");
                        }
                    }
                }

                // Ensures that no closing parenthesis exceed opening parenthesis at all times
                if (closingParenCount > openingParenCount)
                {
                    throw new FormulaFormatException("There cannot be more closing parenthess seen than opening parentheses seen");
                }
            }

            // Ensures an equal of parenthesis is seen
            if (openingParenCount != closingParenCount)
            {
                throw new FormulaFormatException("There are not an equal amount of parentheses");
            }

            // intialize variables
            this.formulaAsList = formulaList;
            this.canocialFormula = string.Empty;
            this.normalizedFormAsList = [];

            // Normalizes and stores formula as string (used for ToString())
            foreach (string token in formulaList)
            {
                this.normalizedFormAsList.Add(token.ToUpper());
                if (IsDouble(token))
                {
                    double.TryParse(token, out double result);
                    this.canocialFormula += result.ToString();
                }
                else
                {
                    this.canocialFormula += token.ToUpper();
                }
            }
        }
        else
        {
            throw new FormulaFormatException("The formula cannot be empty");
        }
    }

    /// <summary>
    ///   <para>
    ///     Reports whether f1 == f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are the same.</returns>
    public static bool operator ==(Formula f1, Formula f2)
    {
        return f1.Equals(f2);
    }

    /// <summary>
    ///   <para>
    ///     Reports whether f1 != f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are not equal to each other.</returns>
    public static bool operator !=(Formula f1, Formula f2)
    {
        if (!(f1 == f2))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///   <para>
    ///     Returns a set of all the variables in the formula.
    ///   </para>
    ///   <remarks>
    ///     Important: no variable may appear more than once in the returned set, even
    ///     if it is used more than once in the Formula.
    ///   </remarks>
    ///   <para>
    ///     For example, if N is a method that converts all the letters in a string to upper case:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>new("x1+y1*z1").GetVariables() should enumerate "X1", "Y1", and "Z1".</item>
    ///     <item>new("x1+X1"   ).GetVariables() should enumerate "X1".</item>
    ///   </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables()
    {
        HashSet<string> result = new HashSet<string>();
        foreach (string s in this.formulaAsList)
        {
            if (IsVar(s))
            {
                result.Add(s.ToUpper());
            }
        }

        return result;
    }

    /// <summary>
    /// This will turn the formula into a string with no spaces. The values
    /// will be normalized The method will run in O(1) time.
    /// </summary>
    /// <returns>
    ///  A canonical version (string) of the formula. All "equal" formulas
    ///   should have the same value here.
    /// </returns>
    public override string ToString()
    {
        return this.canocialFormula;
    }

    /// <summary>
    ///   <para>
    ///     Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    ///     case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    ///     randomly-generated unequal Formulas have the same hash code should be extremely small.
    ///   </para>
    /// </summary>
    /// <returns> The hashcode for the object. </returns>
    public override int GetHashCode()
    {
        // Calls HashCode on the formula's ToString. It should be unique.
        return this.ToString().GetHashCode();
    }

    /// <summary>
    ///   <para>
    ///     Determines if two formula objects represent the same formula.
    ///   </para>
    ///   <para>
    ///     By definition, if the parameter is null or does not reference
    ///     a Formula Object then return false.
    ///   </para>
    ///   <para>
    ///     Two Formulas are considered equal if their canonical string representations
    ///     (as defined by ToString) are equal.
    ///   </para>
    /// </summary>
    /// <param name="obj"> The other object.</param>
    /// <returns>
    ///   True if the two objects represent the same formula.
    /// </returns>
    public override bool Equals(object? obj)
    {
        // Check if the object is null and if they have the same type
        if (ReferenceEquals(obj, null) || !ReferenceEquals(obj.GetType(), this.GetType()))
        {
            return false;
        }

        // Cast the Object into a Formula Object
        Formula formulaObj = (Formula)obj;

        string otherString = formulaObj.ToString();
        string thisString = this.ToString();

        // Check for string length first as a quick check
        if (this.ToString().Length != otherString.Length)
        {
            return false;
        }

        // Loop through and check each character and compare them
        for (int index = 0; index < thisString.Length; index++)
        {
            if (thisString[index] != otherString[index])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///   <para>
    ///     Evaluates this Formula, using the lookup delegate to determine the values of
    ///     variables.
    ///   </para>
    ///   <remarks>
    ///     When the lookup method is called, it will always be passed a Normalized (capitalized)
    ///     variable name.  The lookup method will throw an ArgumentException if there is
    ///     not a definition for that variable token.
    ///   </remarks>
    ///   <para>
    ///     If no undefined variables or divisions by zero are encountered when evaluating
    ///     this Formula, the numeric value of the formula is returned.  Otherwise, a
    ///     FormulaError is returned (with a meaningful explanation as the Reason property).
    ///   </para>
    ///   <para>
    ///     This method should never throw an exception.
    ///   </para>
    /// </summary>
    /// <param name="lookup">
    ///   <para>
    ///     Given a variable symbol as its parameter, lookup returns the variable's (double) value
    ///     (if it has one) or throws an ArgumentException (otherwise).  This method should expect
    ///     variable names to be capitalized.
    ///   </para>
    /// </param>
    /// <returns> Either a double or a formula error, based on evaluating the formula.</returns>
    public object Evaluate(Lookup lookup)
    {
        List<string> tokens = normalizedFormAsList;

        // Create Two Stacks: One for Operators, and the other for Values
        Stack<double> values = new Stack<double>();
        Stack<string> operators = new Stack<string>();

        // Iterate through the formula one token at a time to compute answer
        foreach (string token in normalizedFormAsList)
        {
            // Case for when the token is a number OR a variable
            if (double.TryParse(token, out double result) || IsVar(token))
            {
                // Process Variables into Strings, and returns an error when encountered
                if (IsVar(token))
                {
                    try
                    {
                        result = lookup(token);
                    }
                    catch (ArgumentException)
                    {
                        return new FormulaError("Variable Undefined");
                    }
                }

                // Check if the top operator is a * or / and do the operation on it if it exists
                if (operators.TryPeek(out string? output))
                {
                    if (output == "*")
                    {
                        var (topVal, topOp) = PopTwoFromStacks(values, operators);
                        values.Push(result * topVal);
                        continue;
                    }
                    else if (output == "/")
                    {
                        var (topVal, topOp) = PopTwoFromStacks(values, operators);
                        if (result == 0)
                        {
                            return new FormulaError("Cannot Divide by 0");
                        }

                        values.Push(topVal / result);
                        continue;
                    }
                }

                values.Push(result);
            }

            // Case when token is a + or -
            else if (token == "+" || token == "-")
            {
                // Check if the top operator is a + or a - and do the operation on it if it exists
                if (operators.TryPeek(out string? output))
                {
                    if (output == "+")
                    {
                        var (firstToken, secondToken, topOp) = PopThreeFromStacks(values, operators);
                        values.Push(firstToken + secondToken);
                        operators.Push(token);
                        continue;
                    }

                    if (output == "-")
                    {
                        var (firstToken, secondToken, topOp) = PopThreeFromStacks(values, operators);
                        values.Push(firstToken - secondToken);
                        operators.Push(token);
                        continue;
                    }
                }

                operators.Push(token);
            }

            // Case when token is a * or /
            else if (token == "*" || token == "/")
            {
                operators.Push(token);
            }

            // Case when token is a (
            else if (token == "(")
            {
                operators.Push(token);
            }

            // Case when token is a )
            else if (token == ")")
            {
                // Check For if the top operator is a + or - and do the operation if it exists
                if (operators.TryPeek(out string? output) && (output == "+" || output == "-"))
                {
                    var (firstVal, secondVal, topOp) = PopThreeFromStacks(values, operators);

                    if (topOp == "+")
                    {
                        values.Push(firstVal + secondVal);
                    }

                    if (topOp == "-")
                    {
                        values.Push(firstVal - secondVal);
                    }
                }

                // Take away the "("
                operators.Pop();

                // Check For if the top operator is a * or - and do the operation if it exists
                if (operators.TryPeek(out string? output2) && (output2 == "*" || output2 == "/"))
                {
                    var (firstVal, secondVal, topOp) = PopThreeFromStacks(values, operators);
                    if (topOp == "*")
                    {
                        values.Push(firstVal * secondVal);
                    }

                    if (topOp == "/")
                    {
                        if (secondVal == 0)
                        {
                            return new FormulaError("Cannot Divide By Zero");
                        }

                        values.Push(firstVal / secondVal);
                    }
                }
            }
        }

        // After all the tokens have been processed
        if (operators.Count == 0)
        {
            return values.Pop(); // If all operations are finished, return the value
        }
        else
        {
            // Only two values and one operator is left to compute
            var (firstVal, secondVal, topOp) = PopThreeFromStacks(values, operators);

            if (topOp == "+")
            {
                return firstVal + secondVal;
            }
            else
            {
                return firstVal - secondVal;
            }
        }
    }

    /// <summary>
    /// This is a helper method to pop off three items from two different stacks to later use for computations.
    /// </summary>
    /// <param name="doubleStack"> a stack that contains numbers. </param>
    /// <param name="opStack"> a stack that contains operators i.e +, -, *, /, (, ). </param>
    /// <returns> Three values: The first number, the second number, and the operator. </returns>
    private static (double, double, string) PopThreeFromStacks(Stack<double> doubleStack, Stack<string> opStack)
    {
        double secondVal = doubleStack.Pop();
        double firstVal = doubleStack.Pop();
        string topOp = opStack.Pop();

        return (firstVal, secondVal, topOp);
    }

    /// <summary>
    /// This is a helper method to pop off two items from two different stacks to later use for computations.
    /// </summary>
    /// <param name="doubleStack"> a stack that contains numbers. </param>
    /// <param name="opStack">  a stack that contains operators i.e +, -, *, /, (, ). </param>
    /// <returns> Two values: A number, and the operator. </returns>
    private static (double, string) PopTwoFromStacks(Stack<double> doubleStack, Stack<string> opStack)
    {
        double topVal = doubleStack.Pop();
        string topOp = opStack.Pop();

        return (topVal, topOp);
    }

    /// <summary>
    /// This method checks if the token is a valid number.
    /// </summary>
    /// <param name="token"> the token to be checked for if it is a number or not.</param>
    /// <returns> true if the value can be parsed as a double, false otherwise.</returns>
    private static bool IsDouble(string token)
    {
        return double.TryParse(token, out double result);
    }

    /// <summary>
    /// This method checks if the token is one of the four valid operators.
    /// </summary>
    /// <param name="token"> the token to check for if it is an operator or not.</param>
    /// <returns>true if it is a valid operator, false otherwise.</returns>
    private static bool IsOperator(string token)
    {
        return token == "+" || token == "-" || token == "*" || token == "/";
    }

    /// <summary>
    /// This method is used to check if the token at following index is a valid operator or either an open or closed
    /// parenthesis.
    /// </summary>
    /// <param name="index"> the location of the token to check.</param>
    /// <param name="tokenList"> the list of tokens to search in.</param>
    /// <param name="paren"> the type of parenthesis to check (open/closed).</param>
    /// <returns> true if the token is not an operator or parenthesis, false otherwise.</returns>
    private static bool HasNoOperatorOrParen(int index, List<string> tokenList, string paren)
    {
        return !IsOperator(tokenList[index + 1]) && !tokenList[index + 1].Equals(paren);
    }

    /// <summary>
    /// This method is used to check if the token at the given index is either a number, a variable, or either
    /// an open/closed parenthesis.
    /// </summary>
    /// <param name="index"> the location of the token to check.</param>
    /// <param name="tokenList"> A list that contains a collection of tokens.</param>
    /// <param name="paren"> either a an open or closed parenthesis to check for.</param>
    /// <returns> true if the token is not a number, variable or parenthesis, and false otherwise.</returns>
    private static bool HasNoValidToken(int index, List<string> tokenList, string paren)
    {
        return !IsDouble(tokenList[index]) && !IsVar(tokenList[index]) && tokenList[index] != paren;
    }

    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar(string token)
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch(token, standaloneVarPattern);
    }

    /// <summary>
    ///   <para>
    ///     Given an expression, enumerates the tokens that compose it.
    ///   </para>
    ///   <para>
    ///     Tokens returned are:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>left paren</item>
    ///     <item>right paren</item>
    ///     <item>one of the four operator symbols</item>
    ///     <item>a string consisting of one or more letters followed by one or more numbers</item>
    ///     <item>a double literal</item>
    ///     <item>and anything that doesn't match one of the above patterns</item>
    ///   </list>
    ///   <para>
    ///     There are no empty tokens; white space is ignored (except to separate other tokens).
    ///   </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens(string formula)
    {
        List<string> results =
        [];

        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format(
                                        "({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern,
                                        rpPattern,
                                        opPattern,
                                        VariableRegExPattern,
                                        doublePattern,
                                        spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                results.Add(s);
            }
        }

        return results;
    }
}

/// <summary>
///   Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaFormatException"/> class.
    ///   <para>
    ///      Constructs a FormulaFormatException containing the explanatory message.
    ///   </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occured.</param>
    public FormulaFormatException(string message)
        : base(message)
    {
        // All this does is call the base constructor. No extra code needed.
    }
}

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public class FormulaError
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaError"/> class.
    ///   <para>
    ///     Constructs a FormulaError containing the explanatory reason.
    ///   </para>
    /// </summary>
    /// <param name="message"> Contains a message for why the error occurred.</param>
    public FormulaError(string message)
    {
        Reason = message;
    }

    /// <summary>
    ///  Gets the reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}

/// <summary>
///   Any method meeting this type signature can be used for
///   looking up the value of a variable.  In general the expected behavior is that
///   the Lookup method will "know" about all variables in a formula
///   and return their appropriate value.
/// </summary>
/// <exception cref="ArgumentException">
///   If a variable name is provided that is not recognized by the implementing method,
///   then the method should throw an ArgumentException.
/// </exception>
/// <param name="variableName">
///   The name of the variable (e.g., "A1") to lookup.
/// </param>
/// <returns> The value of the given variable (if one exists). </returns>
public delegate double Lookup(string variableName);
