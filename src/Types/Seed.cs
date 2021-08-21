namespace Sequence
{
    public struct Seed
    {
        private readonly int _value;

        public Seed(int value)
        {
            _value = value;
        }

        public Random ToRandom() => new(_value);

        public int ToInt32() => _value;
    }
}
