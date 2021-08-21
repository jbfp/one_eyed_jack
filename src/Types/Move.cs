namespace Sequence
{
    public sealed record Move(Card Card, Coord Coord) : IEquatable<Move>;
}
