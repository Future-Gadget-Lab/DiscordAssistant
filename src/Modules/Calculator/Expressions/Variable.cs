namespace Assistant.Modules.Calculator.Expressions
{
    public class Variable : IExpression
    {
        public char Name { get; private set; }
        private Number _value;

        public Variable(char name, Number value)
        {
            _value = value;
            Name = name;
        }

        public double Evaluate() => _value.Evaluate();

        public void SetValue(Number value) => _value = value;
    }
}
