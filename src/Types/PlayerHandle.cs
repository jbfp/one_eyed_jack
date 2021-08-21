namespace Sequence
{
    public sealed record PlayerHandle(string Value) : IEquatable<PlayerHandle>
    {
        public override string ToString() => Value;
    }
}
