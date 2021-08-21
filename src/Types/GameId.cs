namespace Sequence
{
    public sealed record GameId(Guid Value) : IEquatable<GameId>
    {
        public override string ToString() => Value.ToString();
    }
}
