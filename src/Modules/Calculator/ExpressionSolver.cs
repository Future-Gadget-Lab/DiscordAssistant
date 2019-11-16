using Assistant.Modules.Calculator.Expressions;
using System.Linq;
using System.Text;

namespace Assistant.Modules.Calculator
{
    // TODO: support exponents and absolute value
    public class ExpressionSolver
    {
        private readonly string _expression;
        private int _pos;

        public ExpressionSolver(string expression)
        {
            _expression = expression;
        }

        public static double SolveExpression(string expression) =>
            new ExpressionSolver(expression).Solve();

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
            IExpression expr = Factor();
            if (!MatchAny('*', '/'))
                return expr;

            BinaryExpression binaryExpr = new BinaryExpression(expr, ConsumeAny('*', '/'), Factor());
            while (MatchAny('*', '/'))
                binaryExpr = new BinaryExpression(binaryExpr, ConsumeAny('*', '/'), Factor());
            return binaryExpr;
        }

        private IExpression Factor()
        {
            UnaryExpression unary = null;
            if (Consume('-'))
                unary = new UnaryExpression('-', null);

            if (Consume('('))
            {
                IExpression expression = Expression();
                Consume(')');
                if (unary != null)
                    return unary.WithRight(expression);
                return expression;
            }
            else if (char.IsDigit(Current()))
            {
                IExpression number = Number();
                if (unary != null)
                    return unary.WithRight(number);
                return number;
            }
            throw new ParseException($"Unexpected token '{Current()}'.");
        }

        private IExpression Number()
        {
            StringBuilder digits = new StringBuilder();
            while (!IsAtEnd() && (char.IsDigit(_expression[_pos]) || _expression[_pos] == '.'))
            {
                digits.Append(Current());
                Advance();
            }
            return new Number(double.Parse(digits.ToString()));
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
