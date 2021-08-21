using System.Collections.Immutable;

namespace Sequence
{
    internal sealed class SequenceBoard : IBoardType
    {
        public ImmutableArray<ImmutableArray<Tile?>> Board => _board;
        public IImmutableDictionary<Tile, (Coord, Coord)> CoordsByTile => _coordsByTile;

        private static readonly ImmutableArray<ImmutableArray<Tile?>> _board;
        private static readonly ImmutableDictionary<Tile, (Coord, Coord)> _coordsByTile;

        static SequenceBoard()
        {
            var b = new[]
            {
                new Tile?[]
                {
                    null,
                    new Tile(Suit.Diamonds, Rank.Six),
                    new Tile(Suit.Diamonds, Rank.Seven),
                    new Tile(Suit.Diamonds, Rank.Eight),
                    new Tile(Suit.Diamonds, Rank.Nine),
                    new Tile(Suit.Diamonds, Rank.Ten),
                    new Tile(Suit.Diamonds, Rank.Queen),
                    new Tile(Suit.Diamonds, Rank.King),
                    new Tile(Suit.Diamonds, Rank.Ace),
                    null,
                },
                new Tile[]
                {
                    new Tile(Suit.Diamonds, Rank.Five),
                    new Tile(Suit.Hearts, Rank.Three),
                    new Tile(Suit.Hearts, Rank.Two),
                    new Tile(Suit.Spades, Rank.Two),
                    new Tile(Suit.Spades, Rank.Three),
                    new Tile(Suit.Spades, Rank.Four),
                    new Tile(Suit.Spades, Rank.Five),
                    new Tile(Suit.Spades, Rank.Six),
                    new Tile(Suit.Spades, Rank.Seven),
                    new Tile(Suit.Clubs, Rank.Ace),
                },
                new Tile[]
                {
                    new Tile(Suit.Diamonds, Rank.Four),
                    new Tile(Suit.Hearts, Rank.Four),
                    new Tile(Suit.Diamonds, Rank.King),
                    new Tile(Suit.Diamonds, Rank.Ace),
                    new Tile(Suit.Clubs, Rank.Ace),
                    new Tile(Suit.Clubs, Rank.King),
                    new Tile(Suit.Clubs, Rank.Queen),
                    new Tile(Suit.Clubs, Rank.Ten),
                    new Tile(Suit.Spades, Rank.Eight),
                    new Tile(Suit.Clubs, Rank.King),
                },
                new Tile[]
                {
                    new Tile(Suit.Diamonds, Rank.Three),
                    new Tile(Suit.Hearts, Rank.Five),
                    new Tile(Suit.Diamonds, Rank.Queen),
                    new Tile(Suit.Hearts, Rank.Queen),
                    new Tile(Suit.Hearts, Rank.Ten),
                    new Tile(Suit.Hearts, Rank.Nine),
                    new Tile(Suit.Hearts, Rank.Eight),
                    new Tile(Suit.Clubs, Rank.Nine),
                    new Tile(Suit.Spades, Rank.Nine),
                    new Tile(Suit.Clubs, Rank.Queen),
                },
                new Tile[]
                {
                    new Tile(Suit.Diamonds, Rank.Two),
                    new Tile(Suit.Hearts, Rank.Six),
                    new Tile(Suit.Diamonds, Rank.Ten),
                    new Tile(Suit.Hearts, Rank.King),
                    new Tile(Suit.Hearts, Rank.Three),
                    new Tile(Suit.Hearts, Rank.Two),
                    new Tile(Suit.Hearts, Rank.Seven),
                    new Tile(Suit.Clubs, Rank.Eight),
                    new Tile(Suit.Spades, Rank.Ten),
                    new Tile(Suit.Clubs, Rank.Ten),
                },
                new Tile[]
                {
                    new Tile(Suit.Spades, Rank.Ace),
                    new Tile(Suit.Hearts, Rank.Seven),
                    new Tile(Suit.Diamonds, Rank.Nine),
                    new Tile(Suit.Hearts, Rank.Ace),
                    new Tile(Suit.Hearts, Rank.Four),
                    new Tile(Suit.Hearts, Rank.Five),
                    new Tile(Suit.Hearts, Rank.Six),
                    new Tile(Suit.Clubs, Rank.Seven),
                    new Tile(Suit.Spades, Rank.Queen),
                    new Tile(Suit.Clubs, Rank.Nine),
                },
                new Tile[]
                {
                    new Tile(Suit.Spades, Rank.King),
                    new Tile(Suit.Hearts, Rank.Eight),
                    new Tile(Suit.Diamonds, Rank.Eight),
                    new Tile(Suit.Clubs, Rank.Two),
                    new Tile(Suit.Clubs, Rank.Three),
                    new Tile(Suit.Clubs, Rank.Four),
                    new Tile(Suit.Clubs, Rank.Five),
                    new Tile(Suit.Clubs, Rank.Six),
                    new Tile(Suit.Spades, Rank.King),
                    new Tile(Suit.Clubs, Rank.Eight),
                },
                new Tile[]
                {
                    new Tile(Suit.Spades, Rank.Queen),
                    new Tile(Suit.Hearts, Rank.Nine),
                    new Tile(Suit.Diamonds, Rank.Seven),
                    new Tile(Suit.Diamonds, Rank.Six),
                    new Tile(Suit.Diamonds, Rank.Five),
                    new Tile(Suit.Diamonds, Rank.Four),
                    new Tile(Suit.Diamonds, Rank.Three),
                    new Tile(Suit.Diamonds, Rank.Two),
                    new Tile(Suit.Spades, Rank.Ace),
                    new Tile(Suit.Clubs, Rank.Seven),
                },
                new Tile[]
                {
                    new Tile(Suit.Spades, Rank.Ten),
                    new Tile(Suit.Hearts, Rank.Ten),
                    new Tile(Suit.Hearts, Rank.Queen),
                    new Tile(Suit.Hearts, Rank.King),
                    new Tile(Suit.Hearts, Rank.Ace),
                    new Tile(Suit.Clubs, Rank.Two),
                    new Tile(Suit.Clubs, Rank.Three),
                    new Tile(Suit.Clubs, Rank.Four),
                    new Tile(Suit.Clubs, Rank.Five),
                    new Tile(Suit.Clubs, Rank.Six),
                },
                new Tile?[]
                {
                    null,
                    new Tile(Suit.Spades, Rank.Nine),
                    new Tile(Suit.Spades, Rank.Eight),
                    new Tile(Suit.Spades, Rank.Seven),
                    new Tile(Suit.Spades, Rank.Six),
                    new Tile(Suit.Spades, Rank.Five),
                    new Tile(Suit.Spades, Rank.Four),
                    new Tile(Suit.Spades, Rank.Three),
                    new Tile(Suit.Spades, Rank.Two),
                    null,
                },
            };

            _board = b
                .Select(r => r.ToImmutableArray())
                .ToImmutableArray();

            var l = new Dictionary<Tile, (Coord, Coord)>
            {
                // Hearts:
                [new Tile(Suit.Hearts, Rank.Ace)] = (new Coord(3, 5), new Coord(4, 8)),
                [new Tile(Suit.Hearts, Rank.Two)] = (new Coord(2, 1), new Coord(5, 4)),
                [new Tile(Suit.Hearts, Rank.Three)] = (new Coord(1, 1), new Coord(4, 4)),
                [new Tile(Suit.Hearts, Rank.Four)] = (new Coord(1, 2), new Coord(4, 5)),
                [new Tile(Suit.Hearts, Rank.Five)] = (new Coord(1, 3), new Coord(5, 5)),
                [new Tile(Suit.Hearts, Rank.Six)] = (new Coord(1, 4), new Coord(6, 5)),
                [new Tile(Suit.Hearts, Rank.Seven)] = (new Coord(1, 5), new Coord(6, 4)),
                [new Tile(Suit.Hearts, Rank.Eight)] = (new Coord(1, 6), new Coord(6, 3)),
                [new Tile(Suit.Hearts, Rank.Nine)] = (new Coord(1, 7), new Coord(5, 3)),
                [new Tile(Suit.Hearts, Rank.Ten)] = (new Coord(1, 8), new Coord(4, 3)),
                [new Tile(Suit.Hearts, Rank.Queen)] = (new Coord(2, 8), new Coord(3, 3)),
                [new Tile(Suit.Hearts, Rank.King)] = (new Coord(3, 8), new Coord(3, 4)),

                // Spades:
                [new Tile(Suit.Spades, Rank.Ace)] = (new Coord(0, 5), new Coord(8, 7)),
                [new Tile(Suit.Spades, Rank.Two)] = (new Coord(8, 9), new Coord(3, 1)),
                [new Tile(Suit.Spades, Rank.Three)] = (new Coord(7, 9), new Coord(4, 1)),
                [new Tile(Suit.Spades, Rank.Four)] = (new Coord(6, 9), new Coord(5, 1)),
                [new Tile(Suit.Spades, Rank.Five)] = (new Coord(5, 9), new Coord(6, 1)),
                [new Tile(Suit.Spades, Rank.Six)] = (new Coord(4, 9), new Coord(7, 1)),
                [new Tile(Suit.Spades, Rank.Seven)] = (new Coord(3, 9), new Coord(8, 1)),
                [new Tile(Suit.Spades, Rank.Eight)] = (new Coord(2, 9), new Coord(8, 2)),
                [new Tile(Suit.Spades, Rank.Nine)] = (new Coord(1, 9), new Coord(8, 3)),
                [new Tile(Suit.Spades, Rank.Ten)] = (new Coord(0, 8), new Coord(8, 4)),
                [new Tile(Suit.Spades, Rank.Queen)] = (new Coord(0, 7), new Coord(8, 5)),
                [new Tile(Suit.Spades, Rank.King)] = (new Coord(0, 6), new Coord(8, 6)),

                // Diamonds:
                [new Tile(Suit.Diamonds, Rank.Ace)] = (new Coord(3, 2), new Coord(8, 0)),
                [new Tile(Suit.Diamonds, Rank.Two)] = (new Coord(7, 7), new Coord(0, 4)),
                [new Tile(Suit.Diamonds, Rank.Three)] = (new Coord(6, 7), new Coord(0, 3)),
                [new Tile(Suit.Diamonds, Rank.Four)] = (new Coord(5, 7), new Coord(0, 2)),
                [new Tile(Suit.Diamonds, Rank.Five)] = (new Coord(4, 7), new Coord(0, 1)),
                [new Tile(Suit.Diamonds, Rank.Six)] = (new Coord(3, 7), new Coord(1, 0)),
                [new Tile(Suit.Diamonds, Rank.Seven)] = (new Coord(2, 7), new Coord(2, 0)),
                [new Tile(Suit.Diamonds, Rank.Eight)] = (new Coord(2, 6), new Coord(3, 0)),
                [new Tile(Suit.Diamonds, Rank.Nine)] = (new Coord(2, 5), new Coord(4, 0)),
                [new Tile(Suit.Diamonds, Rank.Ten)] = (new Coord(2, 4), new Coord(5, 0)),
                [new Tile(Suit.Diamonds, Rank.Queen)] = (new Coord(2, 3), new Coord(6, 0)),
                [new Tile(Suit.Diamonds, Rank.King)] = (new Coord(2, 2), new Coord(7, 0)),

                // Clubs:
                [new Tile(Suit.Clubs, Rank.Ace)] = (new Coord(4, 2), new Coord(9, 1)),
                [new Tile(Suit.Clubs, Rank.Two)] = (new Coord(3, 6), new Coord(5, 8)),
                [new Tile(Suit.Clubs, Rank.Three)] = (new Coord(4, 6), new Coord(6, 8)),
                [new Tile(Suit.Clubs, Rank.Four)] = (new Coord(5, 6), new Coord(7, 8)),
                [new Tile(Suit.Clubs, Rank.Five)] = (new Coord(6, 6), new Coord(8, 8)),
                [new Tile(Suit.Clubs, Rank.Six)] = (new Coord(7, 6), new Coord(9, 8)),
                [new Tile(Suit.Clubs, Rank.Seven)] = (new Coord(7, 5), new Coord(9, 7)),
                [new Tile(Suit.Clubs, Rank.Eight)] = (new Coord(7, 4), new Coord(9, 6)),
                [new Tile(Suit.Clubs, Rank.Nine)] = (new Coord(7, 3), new Coord(9, 5)),
                [new Tile(Suit.Clubs, Rank.Ten)] = (new Coord(7, 2), new Coord(9, 4)),
                [new Tile(Suit.Clubs, Rank.Queen)] = (new Coord(6, 2), new Coord(9, 3)),
                [new Tile(Suit.Clubs, Rank.King)] = (new Coord(5, 2), new Coord(9, 2)),
            };

            _coordsByTile = l.ToImmutableDictionary();
        }
    }
}
