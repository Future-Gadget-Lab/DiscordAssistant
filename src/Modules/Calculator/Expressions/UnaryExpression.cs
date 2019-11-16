using System;

namespace Assistant.Modules.Calculator.Expressions
{
    public class UnaryExpression : IExpression
    {
        private readonly char _op;
        private IExpression _right;
        public UnaryExpression(char op, IExpression right)
        {
            _op = op;
            _right = right;
        }

        public UnaryExpression WithRight(IExpression right)
        {
            _right = right;
            return this;
        }

        public double Evaluate()
        {
            return _op switch
            {
                '-' => -_right.Evaluate(),
                _ => throw new NotImplementedException($"Invalid operator '{_op}'.")
            };
        }
    }
}
