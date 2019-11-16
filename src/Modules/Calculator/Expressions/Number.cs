namespace Assistant.Modules.Calculator.Expressions
{
    public class Number : IExpression
    {
        private readonly double _value;

        public Number(double value) => _value = value;

        public double Evaluate() => _value;
    }
}
