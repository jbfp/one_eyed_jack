namespace Sequence
{
    public interface IRandomFactory
    {
        Random Create();
    }

    public sealed class SystemRandomFactory : IRandomFactory
    {
        public Random Create() => new();
    }
}
