using System.Collections.Immutable;

namespace Sequence.Postgres
{
#pragma warning disable CS0649
    internal sealed class CardComposite
    {
        public DeckNo deckno;
        public Suit suit;
        public Rank rank;

        public static CardComposite? FromCard(Card? card)
        {
            if (card == null)
            {
                return null;
            }

            return new CardComposite
            {
                deckno = card.DeckNo,
                rank = card.Rank,
                suit = card.Suit,
            };
        }

        public Card ToCard() => new(deckno, suit, rank);
    }

    internal sealed class CoordComposite
    {
        public short col;
        public short row;

        public static CoordComposite FromCoord(Coord coord) => new()
        {
            col = (short)coord.Column,
            row = (short)coord.Row,
        };

        public Coord ToCoord() => new(col, row);
    }

    internal sealed class SequenceComposite
    {
        public Team team;
        public CoordComposite[] coords = null!;

        public static SequenceComposite FromSequence(Seq sequence) => new()
        {
            team = sequence.Team,
            coords = sequence.Coords.Select(CoordComposite.FromCoord).ToArray(),
        };

        public static Seq ToSequence(SequenceComposite seq) => new(
            seq.team,
            seq.coords.Select(c => c.ToCoord()).ToImmutableList());
    }
#pragma warning restore CS0649
}
