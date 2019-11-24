using Assistant.Modules.Calculator.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant.Modules.Calculator
{
    public class ExpressionSolver
    {
        private readonly List<Variable> _variables;
        private readonly string _expression;
        private int _pos;

        public ExpressionSolver(string expression, bool allowVars = false)
        {
            if (allowVars)
                _variables = new List<Variable>();
            _expression = expression;
        }

        public static double SolveExpression(string expression) =>
            new ExpressionSolver(expression).Solve();

        public Variable SetVariable(char varName, double value) =>
            SetVariable(varName, new Number(value));

        public Variable SetVariable(char varName, Number value)
        {
            if (_variables == null)
                throw new InvalidOperationException("This instance does not support variables.");
            Variable variable = _variables.SingleOrDefault(v => v.Name == varName);
            if (variable == null)
            {
                variable = new Variable(varName, value);
                _variables.Add(variable);
            }
            else
            {
                variable.SetValue(value);
            }
            return variable;
        }

        public IExpression Parse() => Expression();

        public double Solve() => Expression().Evaluate();

        private IExpression Expression()
        {
            IExpression expr = Term();
            if (!MatchAny('+', '-'))
                return expr;

            BinaryExpression binaryExpr = new BinaryExpression(expr, ConsumeAny('+', '-'), Term());
            while (MatchAny('+', '-'))
                binaryExpr = new BinaryExpression(binaryExpr, ConsumeAny('+', '-'), Term());
            return binaryExpr;
        }

        private IExpression Term()
        {
            IExpression expr = Exponent();
            if (!MatchAny('*', '/'))
                return expr;

            BinaryExpression binaryExpr = new BinaryExpression(expr, ConsumeAny('*', '/'), Exponent());
            while (MatchAny('*', '/'))
                binaryExpr = new BinaryExpression(binaryExpr, ConsumeAny('*', '/'), Exponent());
            return binaryExpr;
        }

        private IExpression Exponent()
        {
            IExpression expr = Factor();
            if (!Match('^'))
                return expr;

            BinaryExpression binaryExpr = new BinaryExpression(expr, Consume(), Factor());
            while (Match('^'))
                binaryExpr = new BinaryExpression(binaryExpr, Consume(), Factor());
            return binaryExpr;
        }

        private IExpression Factor()
        {
            UnaryExpression unary = null;
            if (Consume('-'))
                unary = new UnaryExpression('-', null);

            IExpression expression;
            if (Consume('('))
            {
                expression = Expression();
                if (!Consume(')'))
                    throw new ParseException("Expected ')'.");
            }
            else if (Consume('|'))
            {
                expression = new AbsoluteValue(Expression());
                if (!Consume('|'))
                    throw new ParseException("Expected '|'.");
            }
            else if (char.IsDigit(Current()))
            {
                expression = Number();
            }
            else if (char.IsLetter(Current()) && _variables != null)
            {
                expression = Variable();
            }
            else
            {
                throw new ParseException($"Unexpected token '{Current()}'.");
            }

            if (unary != null)
                return unary.WithRight(expression);
            return expression;
        }

        private IExpression Variable() =>
            SetVariable(Consume(), null);

        private IExpression Number()
        {
            int start = _pos;
            bool hitDecimal = false;
            while (!IsAtEnd() && (char.IsDigit(_expression[_pos]) || (_expression[_pos] == '.' && !hitDecimal && char.IsDigit(Peek()))))
            {
                if (_expression[_pos] == '.')
                    hitDecimal = true;
                Advance();
            }
            ReadOnlySpan<char> digits = _expression.AsSpan(start, _pos - start);
            return new Number(double.Parse(digits));
        }

        private bool IsAtEnd() => _pos >= _expression.Length;

        private char Current()
        {
            if (IsAtEnd())
                throw new ParseException("Unexpected end of expression.");
            char current = _expression[_pos];
            while (!IsAtEnd() && char.IsWhiteSpace(current))
                current = _expression[++_pos];
            return current;
        }

        private char Peek()
        {
            if (_pos + 1 >= _expression.Length)
                throw new ParseException("Unexpected end of expression.");
            return _expression[_pos + 1];
        }

        private void Advance()
        {
            if (IsAtEnd())
                throw new ParseException("Unexpected end of expression.");
            _pos++;
        }

        private bool Match(char c) =>
            !IsAtEnd() && Current() == c;

        private bool MatchAny(params char[] chars) =>
            !IsAtEnd() && chars.Any(c => Match(c));

        private char Consume()
        {
            char c = Current();
            Advance();
            return c;
        }

        private bool Consume(char c)
        {
            if (Match(c))
            {
                Advance();
                return true;
            }
            return false;
        }

        private char ConsumeAny(params char[] chars)
        {
            foreach (char c in chars)
            {
                if (Match(c))
                {
                    Advance();
                    return c;
                }
            }
            throw new ParseException($"Unexpected token '{Current()}'.");
        }
    }
}
