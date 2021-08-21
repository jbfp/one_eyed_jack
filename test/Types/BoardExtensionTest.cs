using System.Collections.Immutable;
using Xunit;

namespace Sequence.Test
{
    public sealed class BoardExtensionsTest
    {
        private static readonly Team _team = Team.Red;

        public static IEnumerable<object[]> Data => new List<object[]>
        {
            new object[] { (0, 0), (1, 0), (2, 0), (3, 0), (4, 0) },
            new object[] { (6, 0), (5, 1), (4, 2), (3, 3), (2, 4) },
            new object[] { (3, 3), (3, 4), (3, 5), (3, 6), (3, 7) },
            new object[] { (2, 2), (3, 3), (4, 4), (5, 5), (6, 6) },
            new object[] { (1, 1), (2, 2), (3, 3), (4, 4) },
            new object[] { (1, 0), (2, 0), (3, 0), (4, 0) },
            new object[] { (5, 0), (6, 0), (7, 0), (8, 0) },
            new object[] { (5, 5), (6, 6), (7, 7), (8, 8) },
            new object[] { (0, 8), (0, 7), (0, 6), (0, 5) },
        };

        [Theory]
        [MemberData(nameof(Data))]
        public void GetSequence_Test1(params (int, int)[] coords)
        {
            var coordsInSequence = ImmutableHashSet<Coord>.Empty;
            var boardSize = 10;
            var boardBuilder = ImmutableArray.CreateBuilder<ImmutableArray<Tile?>>();

            for (int i = 0; i < boardSize; i++)
            {
                var row = ImmutableArray.CreateRange(
                    Enumerable
                        .Range(0, 10)
                        .Select(_ => (Tile?)null));

                boardBuilder.Add(row);
            }

            var board = boardBuilder.ToImmutable();
            var builder = ImmutableDictionary.CreateBuilder<Coord, Team>();

            foreach (var coord in coords)
            {
                builder.Add(new Coord(coord.Item1, coord.Item2), _team);
            }

            var chips = builder.ToImmutable();

            foreach (var coord in chips.Keys)
            {
                var seq = board.GetSequence(chips, coordsInSequence, coord, _team);
                Assert.NotNull(seq);
            }
        }

        [Fact]
        public void TwoSequencesCanShareOneCoord()
        {
            var boardSize = 10;
            var boardBuilder = ImmutableArray.CreateBuilder<ImmutableArray<Tile?>>();

            for (int i = 0; i < boardSize; i++)
            {
                var row = ImmutableArray.CreateRange<Tile?>(
                    Enumerable
                        .Range(0, 10)
                        .Select(_ => new Tile(Suit.Spades, Rank.Ace)));

                boardBuilder.Add(row);
            }

            var board = boardBuilder.ToImmutable();

            var chips = ImmutableDictionary<Coord, Team>.Empty
                .Add(new Coord(1, 0), _team)
                .Add(new Coord(1, 1), _team)
                .Add(new Coord(1, 2), _team)
                .Add(new Coord(1, 3), _team)
                .Add(new Coord(1, 4), _team)
                .Add(new Coord(1, 5), _team)
                .Add(new Coord(1, 6), _team)
                .Add(new Coord(1, 7), _team)
                .Add(new Coord(1, 8), _team);

            var coordsInSequence = ImmutableHashSet.Create(
                new Coord(1, 4),
                new Coord(1, 5),
                new Coord(1, 6),
                new Coord(1, 7),
                new Coord(1, 8)
            );

            var coord = new Coord(1, 0);

            var seq = board.GetSequence(chips, coordsInSequence, coord, _team);

            Assert.NotNull(seq);
        }

        [Fact]
        public void TwoInOneSequence()
        {
            var boardSize = 10;
            var boardBuilder = ImmutableArray.CreateBuilder<ImmutableArray<Tile?>>();

            for (int i = 0; i < boardSize; i++)
            {
                var row = ImmutableArray.CreateRange<Tile?>(
                    Enumerable
                        .Range(0, 10)
                        .Select(_ => new Tile(Suit.Spades, Rank.Ace)));

                boardBuilder.Add(row);
            }

            var board = boardBuilder.ToImmutable();
            var coordsInSequence = ImmutableHashSet<Coord>.Empty;
            var sharedCoord = new Coord(4, 3);
            var chips = ImmutableDictionary<Coord, Team>.Empty
                .Add(new Coord(0, 3), _team)
                .Add(new Coord(1, 3), _team)
                .Add(new Coord(2, 3), _team)
                .Add(new Coord(3, 3), _team)
                .Add(sharedCoord, _team)
                .Add(new Coord(5, 3), _team)
                .Add(new Coord(6, 3), _team)
                .Add(new Coord(7, 3), _team)
                .Add(new Coord(8, 3), _team);


            var actual = board.GetSequences(chips, coordsInSequence, sharedCoord, _team);

            var expected = new[]
            {
                new Seq(_team, ImmutableList.Create(
                    new Coord(0, 3),
                    new Coord(1, 3),
                    new Coord(2, 3),
                    new Coord(3, 3),
                    sharedCoord)),

                new Seq(_team, ImmutableList.Create(
                    sharedCoord,
                    new Coord(5, 3),
                    new Coord(6, 3),
                    new Coord(7, 3),
                    new Coord(8, 3)))
            };

            Assert.Equal(expected, actual, new SeqEqualityComparer());
        }

        private sealed class SeqEqualityComparer : EqualityComparer<Seq>
        {
            public override bool Equals(Seq? x, Seq? y)
            {
                return x!.Team.Equals(y!.Team) && x!.Coords.SequenceEqual(y!.Coords);
            }

            public override int GetHashCode(Seq obj)
            {
                return HashCode.Combine(obj.Team, obj.Coords);
            }
        }
    }
}
