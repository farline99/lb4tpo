using System;
using System.Text.RegularExpressions;
using Lab.Interfaces;

namespace Lab.Implementations.GenCode3
{
    public class MathExpressionEvaluator : IMathExpressionEvaluator
    {
        public double Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentException("Выражение не может быть пустым", nameof(expression));
            }

            expression = expression.Replace(" ", string.Empty);

            var match = Regex.Match(expression, @"^[0-9\+\-\*\/\.]+$");
            if (!match.Success)
            {
                throw new ArgumentException("Выражение содержит недопустимые символы", nameof(expression));
            }

            try
            {
                var parts = expression.Split('+', '-', '*', '/');
                var operators = new char[parts.Length - 1];

                int partIndex = 0;
                for (int i = 0; i < expression.Length; i++)
                {
                    if (char.IsDigit(expression[i]) || expression[i] == '.')
                    {
                        continue;
                    }
                    else
                    {
                        operators[partIndex++] = expression[i];
                    }
                }

                double result = double.Parse(parts[0]);
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    switch (operators[i])
                    {
                        case '+':
                            result += double.Parse(parts[i + 1]);
                            break;
                        case '-':
                            result -= double.Parse(parts[i + 1]);
                            break;
                        case '*':
                            result *= double.Parse(parts[i + 1]);
                            break;
                        case '/':
                            if (double.Parse(parts[i + 1]) == 0)
                            {
                                throw new DivideByZeroException("Деление на ноль");
                            }
                            result /= double.Parse(parts[i + 1]);
                            break;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при вычислении выражения", ex);
            }
        }
    }
}
