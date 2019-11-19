using System;

namespace Assistant.Modules.Calculator.Expressions
{
    public class AbsoluteValue : IExpression
    {
        private readonly IExpression _expression;

        public AbsoluteValue(IExpression expression) => _expression = expression;

        public double Evaluate() => Math.Abs(_expression.Evaluate());
    }
}
