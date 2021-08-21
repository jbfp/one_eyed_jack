namespace Sequence
{
    public sealed class Card : IEquatable<Card>
    {
        public Card(DeckNo deckNo, Suit suit, Rank rank)
        {
            if (!Enum.IsDefined(typeof(DeckNo), deckNo))
            {
                throw new ArgumentOutOfRangeException(nameof(deckNo));
            }

            if (!Enum.IsDefined(typeof(Suit), suit))
            {
                throw new ArgumentOutOfRangeException(nameof(suit));
            }

            if (!Enum.IsDefined(typeof(Rank), rank))
            {
                throw new ArgumentOutOfRangeException(nameof(rank));
            }

            DeckNo = deckNo;
            Suit = suit;
            Rank = rank;
        }

        public DeckNo DeckNo { get; }
        public Suit Suit { get; }
        public Rank Rank { get; }

        public bool Equals(Card? other)
        {
            return other != null
                && DeckNo.Equals(other.DeckNo)
                && Suit.Equals(other.Suit)
                && Rank.Equals(other.Rank);
        }

        public override bool Equals(object? obj) => Equals(obj as Card);

        public override int GetHashCode() => HashCode.Combine(DeckNo, Suit, Rank);

        internal bool IsOneEyedJack() => Rank == Rank.Jack && (Suit == Suit.Hearts || Suit == Suit.Spades);
        internal bool IsTwoEyedJack() => Rank == Rank.Jack && (Suit == Suit.Diamonds || Suit == Suit.Clubs);


        public override string ToString() => $"{Rank} of {Suit} ({DeckNo:D})";
    }

    public enum DeckNo
    {
        One = 1, Two = 2,
    }

    public enum Suit
    {
        Hearts, Spades, Diamonds, Clubs,
    }

    public enum Rank
    {
        Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King,
    }
}
