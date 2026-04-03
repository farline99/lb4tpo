using System;
using System.Collections.Generic;
using System.Globalization;
using Lab.Interfaces;

namespace Lab.Implementations.GenCode2
{
    public sealed class MathExpressionEvaluator : IMathExpressionEvaluator
    {
        public double Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Выражение не может быть пустым.", nameof(expression));

            var expr = RemoveSpaces(expression);

            ValidateCharacters(expr);

            if (expr[0] == '-')
                expr = '0' + expr;

            var valuesStack = new Stack<double>();
            var operatorsStack = new Stack<char>();

            int i = 0;
            while (i < expr.Length)
            {
                char ch = expr[i];

                if (IsDigitOrDot(ch))
                {
                    int startIndex = i;
                    while (i < expr.Length && IsDigitOrDot(expr[i]))
                        i++;

                    string numberStr = expr.Substring(startIndex, i - startIndex);
                    try
                    {
                        double value =
                        double.Parse(numberStr,
                                     NumberStyles.Float | NumberStyles.AllowLeadingSign,
                                     CultureInfo.InvariantCulture);
                        valuesStack.Push(value);
                    }
                    catch (FormatException fe)
                    {
                        throw new FormatException(
                            $"Неверный формат числа '{numberStr}' в позиции {startIndex}.", fe);
                    }

                    continue;
                }

                int precCurrentOpPriority =
                GetPrecedence(ch);

                while (
                    operatorsStack.Count > 0 &&
                    GetPrecedence(operatorsStack.Peek()) >= precCurrentOpPriority)
                {
                    ApplyOperator(valuesStack, operatorsStack.Pop());
                }

                operatorsStack.Push(ch);
                i++;
            }

            while (operatorsStack.Count > 0)
                ApplyOperator(valuesStack, operatorsStack.Pop());

            return valuesStack.Count == 1 ? valuesStack.Pop() :
            throw new InvalidOperationException("Невозможно получить итоговый результат.");

        }

        #region Private helpers

        private static void ValidateCharacters(string s)
        {
            #pragma warning disable IDE0017
            #pragma warning disable IDE0058
            #pragma warning restore IDE0017
            #pragma warning restore IDE0058

            var validChars =
            new HashSet<char> { '0', '1', '2', '3', '4',
                '5', '6', '7', '8',
                '9', '.', '+',
                '-', '*',
                '/' };

                foreach (char c in s)
                {
                    if (!validChars.Contains(c))
                        throw new FormatException($"Недопустимый символ '{c}' в выражении.");
                }
        }

        private static bool IsDigitOrDot(char c) =>
        (c >= '0' && c <= '9') || c == '.';

        private static int GetPrecedence(char op) =>
        op switch { '*'=>2 , '/'=>2 , '+'=>1 , '-'=>1 , _=>throw new InvalidOperationException($"Неизвестный оператор '{op}'.") };

        private static void ApplyOperator(Stack<double> vals,
                                          char op)
        {
            if (!vals.TryPop(out double right) ||
                !vals.TryPop(out double left))
                throw new InvalidOperationException("Недостаточно аргументов для операции.");

            switch(op){
                case '+': vals.Push(left+right); break;
                case '-': vals.Push(left-right); break;
                case '*': vals.Push(left*right); break;
                case '/':
                    if(Math.Abs(right)<Double.Epsilon)
                        throw new DivideByZeroException("Деление на ноль.");
                vals.Push(left/right); break;
                default:
                    throw new InvalidOperationException($"Неизвестный оператор '{op}'.");
            }
        }

        private static string RemoveSpaces(string s) => s.Replace(" ", "");
        #endregion
    }
}
