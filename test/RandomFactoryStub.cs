namespace Sequence.Test
{
    internal sealed class RandomFactoryStub : IRandomFactory
    {
        public RandomFactoryStub(int value) => Value = value;

        public int Value { get; set; }

        public Random Create() => new RandomStub(Value);
    }

    internal sealed class RandomStub : Random
    {
        private readonly int _value;

        public RandomStub(int value) => _value = value;

        protected override double Sample() => _value;
    }
}
