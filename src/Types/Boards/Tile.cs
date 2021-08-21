namespace Sequence
{
    public sealed class Tile : IEquatable<Tile>, IEquatable<Card>
    {
        public Tile(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public Suit Suit { get; }
        public Rank Rank { get; }

        public void Deconstruct(out Suit suit, out Rank rank)
        {
            suit = Suit;
            rank = Rank;
        }

        public bool Equals(Tile? other) =>
            other != null &&
            Suit.Equals(other.Suit) &&
            Rank.Equals(other.Rank);

        public bool Equals(Card? other) =>
            other != null &&
            Suit.Equals(other.Suit) &&
            Rank.Equals(other.Rank);

        public override bool Equals(object? obj) => Equals(obj as Tile) || Equals(obj as Card);

        public override int GetHashCode() => HashCode.Combine(Suit, Rank);

        public override string ToString() => $"({Suit}, {Rank})";
    }
}
