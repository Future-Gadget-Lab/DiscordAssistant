using System;

namespace Assistant.Modules.Calculator.Expressions
{
    public class BinaryExpression : IExpression
    {
        private readonly IExpression _left;
        private readonly IExpression _right;
        private readonly char _op;

        public BinaryExpression(IExpression left, char op, IExpression right)
        {
            _left = left;
            _op = op;
            _right = right;
        }

        public double Evaluate()
        {
            double leftValue = _left.Evaluate();
            double rightValue = _right.Evaluate();
            return _op switch
            {
                '+' => leftValue + rightValue,
                '-' => leftValue - rightValue,
                '*' => leftValue * rightValue,
                '/' => leftValue / rightValue,
                '^' => Math.Pow(leftValue, rightValue),
                _ => throw new NotImplementedException($"Invalid operator '{_op}'.")
            };
        }
    }
}
