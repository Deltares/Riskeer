/*

Copyright (c) 2007 E. W. Bachtal, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial 
  portions of the Software.

The software is provided "as is", without warranty of any kind, express or implied, including but not 
limited to the warranties of merchantability, fitness for a particular purpose and noninfringement. In 
no event shall the authors or copyright holders be liable for any claim, damages or other liability, 
whether in an action of contract, tort or otherwise, arising from, out of or in connection with the 
software or the use or other dealings in the software. 

http://ewbi.blogs.com/develops/2007/03/excel_formula_p.html
http://ewbi.blogs.com/develops/2004/12/excel_formula_p.html

v1.0  Original
v1.1  Added support for in-formula scientific notation.

*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DelftTools.Utils.FormulaParser
{
    /// <summary>
    /// Parser for excel formula expressions
    /// </summary>
    public class ExcelFormula : IList<ExcelFormulaToken>
    {
        private readonly string formula;
        private List<ExcelFormulaToken> tokens;

        /// <summary>
        /// Initialize the parser with an excel formula of the form "=function(par1,par2,...)"
        /// </summary>
        /// <param name="formula"></param>
        public ExcelFormula(string formula)
        {
            if (formula == null) throw new ArgumentNullException("formula");
            this.formula = formula.Trim();
            tokens = new List<ExcelFormulaToken>();
            ParseToTokens();
        }

        /// <summary>
        /// Returns the excel formula supplied to the class on initialization
        /// </summary>
        public string Formula
        {
            get { return formula; }
        }

        public ExcelFormulaToken this[int index]
        {
            get { return tokens[index]; }
            set { throw new NotSupportedException(); }
        }

        public int IndexOf(ExcelFormulaToken item)
        {
            return tokens.IndexOf(item);
        }

        public void Insert(int index, ExcelFormulaToken item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public void Add(ExcelFormulaToken item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

  
        public bool Contains(ExcelFormulaToken item)
        {
            return tokens.Contains(item);
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(ExcelFormulaToken item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(ExcelFormulaToken[] array, int arrayIndex)
        {
            tokens.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return tokens.Count; }
        }

        public IEnumerator<ExcelFormulaToken> GetEnumerator()
        {
            return tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// No attempt is made to verify formulas; assumes formulas are derived from Excel, where 
        /// they can only exist if valid; stack overflows/underflows sunk as nulls without exceptions.
        /// </summary>
        private void ParseToTokens()
        {
            if ((formula.Length < 2) || (formula[0] != '=')) return;

            ExcelFormulaTokens tokens1 = new ExcelFormulaTokens();
            ExcelFormulaStack stack = new ExcelFormulaStack();

            const char QUOTE_DOUBLE = '"';
            const char QUOTE_SINGLE = '\'';
            const char BRACKET_CLOSE = ']';
            const char BRACKET_OPEN = '[';
            const char BRACE_OPEN = '{';
            const char BRACE_CLOSE = '}';
            const char PAREN_OPEN = '(';
            const char PAREN_CLOSE = ')';
            const char SEMICOLON = ';';
            const char WHITESPACE = ' ';
            const char DIGIT_GROUPING_SYMBOL = ',';
            const char ERROR_START = '#';

            const string OPERATORS_SN = "+-";
            const string OPERATORS_INFIX = "+-*/^&=><";
            const string OPERATORS_POSTFIX = "%";

            string[] ERRORS = new string[] {"#NULL!", "#DIV/0!", "#VALUE!", "#REF!", "#NAME?", "#NUM!", "#N/A"};

            string[] COMPARATORS_MULTI = new string[] {">=", "<=", "<>"};

            bool inString = false;
            bool inPath = false;
            bool inRange = false;
            bool inError = false;

            int index = 1;
            string value = "";

            while (index < formula.Length)
            {
                // state-dependent character evaluation (order is important)

                // double-quoted strings
                // embeds are doubled
                // end marks token

                if (inString)
                {
                    if (formula[index] == QUOTE_DOUBLE)
                    {
                        if (((index + 2) <= formula.Length) && (formula[index + 1] == QUOTE_DOUBLE))
                        {
                            value += QUOTE_DOUBLE;
                            index++;
                        }
                        else
                        {
                            inString = false;
                            tokens1.Add(
                                new ExcelFormulaToken(value, ExcelFormulaTokenType.Operand,
                                                      ExcelFormulaTokenSubtype.Text));
                            value = "";
                        }
                    }
                    else
                    {
                        value += formula[index];
                    }
                    index++;
                    continue;
                }

                // single-quoted strings (links)
                // embeds are double
                // end does not mark a token

                if (inPath)
                {
                    if (formula[index] == QUOTE_SINGLE)
                    {
                        if (((index + 2) <= formula.Length) && (formula[index + 1] == QUOTE_SINGLE))
                        {
                            value += QUOTE_SINGLE;
                            index++;
                        }
                        else
                        {
                            inPath = false;
                        }
                    }
                    else
                    {
                        value += formula[index];
                    }
                    index++;
                    continue;
                }

                // bracked strings (R1C1 range index or linked workbook name)
                // no embeds (changed to "()" by Excel)
                // end does not mark a token

                if (inRange)
                {
                    if (formula[index] == BRACKET_CLOSE)
                    {
                        inRange = false;
                    }
                    value += formula[index];
                    index++;
                    continue;
                }

                // error values
                // end marks a token, determined from absolute list of values

                if (inError)
                {
                    value += formula[index];
                    index++;
                    if (Array.IndexOf(ERRORS, value) != -1)
                    {
                        inError = false;
                        tokens1.Add(
                            new ExcelFormulaToken(value, ExcelFormulaTokenType.Operand, ExcelFormulaTokenSubtype.Error));
                        value = "";
                    }
                    continue;
                }

                // scientific notation check

                if ((OPERATORS_SN).IndexOf(formula[index]) != -1)
                {
                    if (value.Length > 1)
                    {
                        if (Regex.IsMatch(value, @"^[1-9]{1}(\.[0-9]+)?E{1}$"))
                        {
                            value += formula[index];
                            index++;
                            continue;
                        }
                    }
                }

                // independent character evaluation (order not important)

                // establish state-dependent character evaluations

                if (formula[index] == QUOTE_DOUBLE)
                {
                    if (value.Length > 0)
                    {
                        // unexpected
                        tokens1.Add(new ExcelFormulaToken(value, ExcelFormulaTokenType.Unknown));
                        value = "";
                    }
                    inString = true;
                    index++;
                    continue;
                }

                if (formula[index] == QUOTE_SINGLE)
                {
                    if (value.Length > 0)
                    {
                        // unexpected
                        tokens1.Add(new ExcelFormulaToken(value, ExcelFormulaTokenType.Unknown));
                        value = "";
                    }
                    inPath = true;
                    index++;
                    continue;
                }

                if (formula[index] == BRACKET_OPEN)
                {
                    inRange = true;
                    value += BRACKET_OPEN;
                    index++;
                    continue;
                }

                if (formula[index] == ERROR_START)
                {
                    if (value.Length > 0)
                    {
                        // unexpected
                        tokens1.Add(new ExcelFormulaToken(value, ExcelFormulaTokenType.Unknown));
                        value = "";
                    }
                    inError = true;
                    value += ERROR_START;
                    index++;
                    continue;
                }

                // mark start and end of arrays and array rows

                if (formula[index] == BRACE_OPEN)
                {
                    if (value.Length > 0)
                    {
                        // unexpected
                        tokens1.Add(new ExcelFormulaToken(value, ExcelFormulaTokenType.Unknown));
                        value = "";
                    }
                    stack.Push(
                        tokens1.Add(
                            new ExcelFormulaToken("ARRAY", ExcelFormulaTokenType.Function,
                                                  ExcelFormulaTokenSubtype.Start)));
                    stack.Push(
                        tokens1.Add(
                            new ExcelFormulaToken("ARRAYROW", ExcelFormulaTokenType.Function,
                                                  ExcelFormulaTokenSubtype.Start)));
                    index++;
                    continue;
                }

                if (formula[index] == SEMICOLON)
                {
                    if (value.Length > 0)
                    {
                        tokens1.Add(new ExcelFormulaToken(value, ExcelFormulaTokenType.Operand));
                        value = "";
                    }
                    tokens1.Add(stack.Pop());
                    tokens1.Add(new ExcelFormulaToken(",", ExcelFormulaTokenType.Argument));
                    stack.Push(
                        tokens1.Add(
                            new ExcelFormulaToken("ARRAYROW", ExcelFormulaTokenType.Function,
                                                  ExcelFormulaTokenSubtype.Start)));
                    index++;
                    continue;
                }

                if (formula[index] == BRACE_CLOSE)
                {
                    if (value.Length > 0)
                    {
                        tokens1.Add(new ExcelFormulaToken(value, ExcelFormulaTokenType.Operand));
                        value = "";
                    }
                    tokens1.Add(stack.Pop());
                    tokens1.Add(stack.Pop());
                    index++;
                    continue;
                }

                // trim white-space

                if (formula[index] == WHITESPACE)
                {
                    if (value.Length > 0)
                    {
                        tokens1.Add(new ExcelFormulaToken(value, ExcelFormulaTokenType.Operand));
                        value = "";
                    }
                    tokens1.Add(new ExcelFormulaToken("", ExcelFormulaTokenType.WhiteSpace));
                    index++;
                    while ((formula[index] == WHITESPACE) && (index < formula.Length))
                    {
                        index++;
                    }
                    continue;
                }

                // multi-character comparators

                if ((index + 2) <= formula.Length)
                {
                    if (Array.IndexOf(COMPARATORS_MULTI, formula.Substring(index, 2)) != -1)
                    {
                        if (value.Length > 0)
                        {
                            tokens1.Add(new ExcelFormulaToken(value, ExcelFormulaTokenType.Operand));
                            value = "";
                        }
                        tokens1.Add(
                            new ExcelFormulaToken(formula.Substring(index, 2), ExcelFormulaTokenType.OperatorInfix,
                                                  ExcelFormulaTokenSubtype.Logical));
                        index += 2;
                        continue;
                    }
                }

                // standard infix operators

                if ((OPERATORS_INFIX).IndexOf(formula[index]) != -1)
                {
                    if (value.Length > 0)
                    {
                        tokens1.Add(new ExcelFormulaToken(value, ExcelFormulaTokenType.Operand));
                        value = "";
                    }
                    tokens1.Add(new ExcelFormulaToken(formula[index].ToString(), ExcelFormulaTokenType.OperatorInfix));
                    index++;
                    continue;
                }

                // standard postfix operators (only one)

                if ((OPERATORS_POSTFIX).IndexOf(formula[index]) != -1)
                {
                    if (value.Length > 0)
                    {
                        tokens1.Add(new ExcelFormulaToken(value, ExcelFormulaTokenType.Operand));
                        value = "";
                    }
                    tokens1.Add(new ExcelFormulaToken(formula[index].ToString(), ExcelFormulaTokenType.OperatorPostfix));
                    index++;
                    continue;
                }

                // start subexpression or function

                if (formula[index] == PAREN_OPEN)
                {
                    if (value.Length > 0)
                    {
                        stack.Push(
                            tokens1.Add(
                                new ExcelFormulaToken(value, ExcelFormulaTokenType.Function,
                                                      ExcelFormulaTokenSubtype.Start)));
                        value = "";
                    }
                    else
                    {
                        stack.Push(
                            tokens1.Add(
                                new ExcelFormulaToken("", ExcelFormulaTokenType.Subexpression,
                                                      ExcelFormulaTokenSubtype.Start)));
                    }
                    index++;
                    continue;
                }

                // function, subexpression, or array parameters, or operand unions

                if (formula[index] == DIGIT_GROUPING_SYMBOL)
                {
                    if (value.Length > 0)
                    {
                        tokens1.Add(new ExcelFormulaToken(value, ExcelFormulaTokenType.Operand));
                        value = "";
                    }
                    if (stack.Current.Type != ExcelFormulaTokenType.Function)
                    {
                      
                        tokens1.Add(
                            new ExcelFormulaToken(DIGIT_GROUPING_SYMBOL.ToString(), ExcelFormulaTokenType.OperatorInfix,
                                                  ExcelFormulaTokenSubtype.Union));
                    }
                    else
                    {
                        tokens1.Add(new ExcelFormulaToken(DIGIT_GROUPING_SYMBOL.ToString(), ExcelFormulaTokenType.Argument));
                    }
                    index++;
                    continue;
                }

                // stop subexpression

                if (formula[index] == PAREN_CLOSE)
                {
                    if (value.Length > 0)
                    {
                        tokens1.Add(new ExcelFormulaToken(value, ExcelFormulaTokenType.Operand));
                        value = "";
                    }
                    tokens1.Add(stack.Pop());
                    index++;
                    continue;
                }

                // token accumulation

                value += formula[index];
                index++;
            }

            // dump remaining accumulation

            if (value.Length > 0)
            {
                tokens1.Add(new ExcelFormulaToken(value, ExcelFormulaTokenType.Operand));
            }

            // move tokenList to new set, excluding unnecessary white-space tokens and converting necessary ones to intersections

            ExcelFormulaTokens tokens2 = new ExcelFormulaTokens(tokens1.Count);

            while (tokens1.MoveNext())
            {
                ExcelFormulaToken token = tokens1.Current;

                if (token == null) continue;

                if (token.Type != ExcelFormulaTokenType.WhiteSpace)
                {
                    tokens2.Add(token);
                    continue;
                }

                if ((tokens1.BOF) || (tokens1.EOF)) continue;

                ExcelFormulaToken previous = tokens1.Previous;

                if (previous == null) continue;

                if (!(
                         ((previous.Type == ExcelFormulaTokenType.Function) &&
                          (previous.Subtype == ExcelFormulaTokenSubtype.Stop)) ||
                         ((previous.Type == ExcelFormulaTokenType.Subexpression) &&
                          (previous.Subtype == ExcelFormulaTokenSubtype.Stop)) ||
                         (previous.Type == ExcelFormulaTokenType.Operand)
                     )
                    ) continue;

                ExcelFormulaToken next = tokens1.Next;

                if (next == null) continue;

                if (!(
                         ((next.Type == ExcelFormulaTokenType.Function) &&
                          (next.Subtype == ExcelFormulaTokenSubtype.Start)) ||
                         ((next.Type == ExcelFormulaTokenType.Subexpression) &&
                          (next.Subtype == ExcelFormulaTokenSubtype.Start)) ||
                         (next.Type == ExcelFormulaTokenType.Operand)
                     )
                    ) continue;

                tokens2.Add(
                    new ExcelFormulaToken("", ExcelFormulaTokenType.OperatorInfix, ExcelFormulaTokenSubtype.Intersection));
            }

            // move tokens to final list, switching infix "-" operators to prefix when appropriate, switching infix "+" operators 
            // to noop when appropriate, identifying operand and infix-operator subtypes, and pulling "@" from function names

            tokens = new List<ExcelFormulaToken>(tokens2.Count);

            while (tokens2.MoveNext())
            {
                ExcelFormulaToken token = tokens2.Current;

                if (token == null) continue;

                ExcelFormulaToken previous = tokens2.Previous;
                //ExcelFormulaToken next = tokens2.Next;

                if ((token.Type == ExcelFormulaTokenType.OperatorInfix) && (token.Value == "-"))
                {
                    if (tokens2.BOF)
                        token.Type = ExcelFormulaTokenType.OperatorPrefix;
                    else if (
                        ((previous.Type == ExcelFormulaTokenType.Function) &&
                         (previous.Subtype == ExcelFormulaTokenSubtype.Stop)) ||
                        ((previous.Type == ExcelFormulaTokenType.Subexpression) &&
                         (previous.Subtype == ExcelFormulaTokenSubtype.Stop)) ||
                        (previous.Type == ExcelFormulaTokenType.OperatorPostfix) ||
                        (previous.Type == ExcelFormulaTokenType.Operand)
                        )
                        token.Subtype = ExcelFormulaTokenSubtype.Math;
                    else
                        token.Type = ExcelFormulaTokenType.OperatorPrefix;

                    tokens.Add(token);
                    continue;
                }

                if ((token.Type == ExcelFormulaTokenType.OperatorInfix) && (token.Value == "+"))
                {
                    if (tokens2.BOF)
                        continue;
                    else if (
                        ((previous.Type == ExcelFormulaTokenType.Function) &&
                         (previous.Subtype == ExcelFormulaTokenSubtype.Stop)) ||
                        ((previous.Type == ExcelFormulaTokenType.Subexpression) &&
                         (previous.Subtype == ExcelFormulaTokenSubtype.Stop)) ||
                        (previous.Type == ExcelFormulaTokenType.OperatorPostfix) ||
                        (previous.Type == ExcelFormulaTokenType.Operand)
                        )
                        token.Subtype = ExcelFormulaTokenSubtype.Math;
                    else
                        continue;

                    tokens.Add(token);
                    continue;
                }

                if ((token.Type == ExcelFormulaTokenType.OperatorInfix) &&
                    (token.Subtype == ExcelFormulaTokenSubtype.Nothing))
                {
                    if (("<>=").IndexOf(token.Value.Substring(0, 1)) != -1)
                        token.Subtype = ExcelFormulaTokenSubtype.Logical;
                    else if (token.Value == "&")
                        token.Subtype = ExcelFormulaTokenSubtype.Concatenation;
                    else
                        token.Subtype = ExcelFormulaTokenSubtype.Math;

                    tokens.Add(token);
                    continue;
                }

                if ((token.Type == ExcelFormulaTokenType.Operand) && (token.Subtype == ExcelFormulaTokenSubtype.Nothing))
                {
                    double d;
                    bool isNumber = double.TryParse(token.Value, NumberStyles.Any, CultureInfo.CurrentCulture, out d);
                    if (!isNumber)
                        if ((token.Value == "TRUE") || (token.Value == "FALSE"))
                            token.Subtype = ExcelFormulaTokenSubtype.Logical;
                        else
                            token.Subtype = ExcelFormulaTokenSubtype.Range;
                    else
                        token.Subtype = ExcelFormulaTokenSubtype.Number;

                    tokens.Add(token);
                    continue;
                }

                if (token.Type == ExcelFormulaTokenType.Function)
                {
                    if (token.Value.Length > 0)
                    {
                        if (token.Value.Substring(0, 1) == "@")
                        {
                            token.Value = token.Value.Substring(1);
                        }
                    }
                }

                tokens.Add(token);
            }
        }

        internal class ExcelFormulaTokens
        {
            private int index = -1;
            private readonly List<ExcelFormulaToken> tokens;

            public ExcelFormulaTokens() : this(4)
            {
            }

            public ExcelFormulaTokens(int capacity)
            {
                tokens = new List<ExcelFormulaToken>(capacity);
            }

            public int Count
            {
                get { return tokens.Count; }
            }

            public bool BOF
            {
                get { return (index <= 0); }
            }

            public bool EOF
            {
                get { return (index >= (tokens.Count - 1)); }
            }

            public ExcelFormulaToken Current
            {
                get
                {
                    if (index == -1) return null;
                    return tokens[index];
                }
            }

            public ExcelFormulaToken Next
            {
                get
                {
                    if (EOF) return null;
                    return tokens[index + 1];
                }
            }

            public ExcelFormulaToken Previous
            {
                get
                {
                    if (index < 1) return null;
                    return tokens[index - 1];
                }
            }

            public ExcelFormulaToken Add(ExcelFormulaToken token)
            {
                tokens.Add(token);
                return token;
            }

            public bool MoveNext()
            {
                if (EOF) return false;
                index++;
                return true;
            }

            public void Reset()
            {
                index = -1;
            }
        }

        internal class ExcelFormulaStack
        {
            private readonly Stack<ExcelFormulaToken> stack = new Stack<ExcelFormulaToken>();

            public void Push(ExcelFormulaToken token)
            {
                stack.Push(token);
            }

            public ExcelFormulaToken Pop()
            {
                if (stack.Count == 0) return null;
                return new ExcelFormulaToken("", stack.Pop().Type, ExcelFormulaTokenSubtype.Stop);
            }

            public ExcelFormulaToken Current
            {
                get { return (stack.Count > 0) ? stack.Peek() : null; }
            }
        }
    }
}