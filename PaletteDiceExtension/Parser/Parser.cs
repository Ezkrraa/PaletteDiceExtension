using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Graphics.Printing.PrintSupport;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace PaletteDiceExtension.Parser;

public static class Parser
{
    private enum TokenType
    {
        Number,
        Operator,
        LeftParen,
        RightParen,
        // no functions or commas since those are out of scope
    }

    private static readonly char[] digits = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
    private static readonly char[] operators = ['D', '+', '-', '*', '/',];

    // tokenizes, translates and evaluates an expression.
    public static int? ParseExpression(string expression)
    {
        if (expression == null)
            return null;
        List<object>? tokenizedExpr = Tokenizer(expression);
        if (tokenizedExpr == null)
            return null;
        List<object>? RPNExpr = INToRPN(tokenizedExpr);
        if (RPNExpr == null)
            return null;
        return InterpretRPN(RPNExpr);
    }



    // turns a maths expression with infix notation into a bunch of tokens
    // 
    public static List<object>? Tokenizer(string expression)
    {
        if (expression.Length == 0) return null;

        Stack<object> output = new();
        TokenType? lastObject = null;
        for (int i = 0; i < expression.Length; i++)
        {
            TokenType? t = GetTokenType(expression[i]);
            if (t == null)
                continue;
            if (lastObject == TokenType.Number && t == TokenType.Number)
                output.Push($"{(string)output.Pop()}{expression[i]}");
            else if (t == TokenType.Number)
            {
                output.Push($"{expression[i]}");
            }
            else
            {
                output.Push(expression[i]);
            }
            lastObject = t;
        }
        List<object> outputList = output.ToList();
        for (int i = 0; i < outputList.Count; i++)
        {
            if (outputList[i] is string str)
            {
                if (!int.TryParse(str, out int result))
                {
                    Debug.Fail($"Failed to parse the number '{str}'");
                    return null;
                }
                outputList[i] = result;
            }
        }
        outputList.Reverse();
        return outputList;
    }


    // turns normal infix notation (e.g. '2 + 4 * 3', which should return 14) into postfix notation (e.g. '2 4 3 * +')
    // uses the Shunting yard algorithm (https://en.m.wikipedia.org/wiki/Shunting_yard_algorithm)
    public static List<object>? INToRPN(List<object> input)
    {
        Queue<object> tokens = new(input);
        List<object> output = [];
        Stack<char> operatorStack = [];
        while (tokens.Count > 0)
        {
            object token = tokens.Dequeue();
            TokenType type = GetTokenType(token);
            switch (type)
            {
                case TokenType.Number:
                    output.Add(token);
                    break;
                case TokenType.Operator:
                    while (operatorStack.Count > 0 && operatorStack.Peek() != '(' && (GetOperatorPrecedence(operatorStack.Peek()) >= GetOperatorPrecedence((char)token)))
                    {
                        output.Add(operatorStack.Pop());
                    }
                    operatorStack.Push((char)token);
                    break;
                case TokenType.LeftParen:
                    operatorStack.Push((char)token);
                    break;
                case TokenType.RightParen:
                    if (operatorStack.Count == 0)
                        return null;
                    while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
                    {
                        output.Add(operatorStack.Pop());
                    }
                    if (operatorStack.Count == 0 || operatorStack.Peek() != '(')
                        return null;
                    operatorStack.Pop();
                    break;
            }
        }
        while (operatorStack.Count > 0)
        {
            if (operatorStack.Peek() == '(')
                return null;
            else
            {
                output.Add(operatorStack.Pop());
            }
        }
        return output;
    }


    // takes RPN and returns an integer
    public static int? InterpretRPN(List<object> rpn)
    {
        try
        {

            // needs at least 3 items to work
            if (rpn.Count < 3) return null;
            int max = rpn.Count;
            for (int i = 2; i < max; i++)
            {
                if (GetTokenType(rpn[i]) == TokenType.Operator)
                {
                    int result = Evaluate((int)rpn[i - 2], (int)rpn[i - 1], (char)rpn[i]);
                    rpn.RemoveRange(i - 2, 3);
                    rpn.Insert(i - 2, result);
                    i -= 2;
                    max -= 2;
                }
            }
            if (max != 1)
            {
                Debug.Fail("Did not properly evaluate context");
                return null;
            }
            return (int)rpn[0];
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
    }

    private static int Evaluate(int ArgA, int ArgB, char Operator)
    {
        return Operator switch
        {
            'D' => new DiceResult(ArgB, ArgA).Result,
            '+' => ArgA + ArgB,
            '-' => ArgA - ArgB,
            '*' => ArgA * ArgB,
            '/' => ArgA / ArgB,
            _ => throw new ArgumentException($"'{Operator}' is not a valid operator")
        };
    }

    private static TokenType GetTokenType(object item)
    {
        if (item is int)
            return TokenType.Number;
        if (item is char itemChar)
        {
            if (itemChar == '(')
                return TokenType.LeftParen;
            else if (itemChar == ')')
                return TokenType.RightParen;
            else if (operators.Contains(itemChar))
                return TokenType.Operator;
        }
        else
            throw new ArgumentException($"Cannot find operator for token '{item}'");
        return TokenType.Number;
    }

    private static TokenType? GetTokenType(char item)
    {
        if (item == '(')
            return TokenType.LeftParen;
        else if (item == ')')
            return TokenType.RightParen;
        else if (operators.Contains(item))
            return TokenType.Operator;
        else if (digits.Contains(item))
            return TokenType.Number;
        return null;
    }

    private static int GetOperatorPrecedence(char item)
    {
        return item switch
        {
            '+' => 2,
            '-' => 2,
            'D' => 3,
            '*' => 3,
            '/' => 3,
            _ => throw new ArgumentException("Cannot evaluate token '" + item + "'."),
        };
    }
}
