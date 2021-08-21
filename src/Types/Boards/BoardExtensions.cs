using System.Collections.Immutable;

namespace Sequence
{
    using Board = ImmutableArray<ImmutableArray<Tile?>>;

    public static class BoardExtensions
    {
        public static bool Matches(this Board board, Coord coord, Card card)
        {
            var row = coord.Row;
            var column = coord.Column;

            if (row < 0 || row >= board.Length)
            {
                return false;
            }

            var theRow = board[row];

            if (column < 0 || column >= theRow.Length)
            {
                return false;
            }

            if (card.IsTwoEyedJack())
            {
                return true;
            }

            var match = theRow[column];

            if (match == null)
            {
                return false;
            }

            return match.Equals(card);
        }

        public static Seq? GetSequence(
            this Board board,
            IImmutableDictionary<Coord, Team> chips,
            IImmutableSet<Coord> coordsInSequences,
            Coord coord,
            Team team)
        {
            var row = coord.Row;
            var col = coord.Column;

            bool IsSequence(IEnumerable<Coord> cs)
            {
                int shared = 0;

                foreach (var c in cs)
                {
                    var isCorner =
                        (c.Row >= 0 && c.Row < board.Length) &&
                        (c.Column >= 0 && c.Column < board[c.Row].Length) &&
                        board[c.Row][c.Column] == null;

                    var teamOwnsCoord = chips.TryGetValue(c, out var t) && team == t;

                    if (isCorner || teamOwnsCoord)
                    {
                        var isPartOfAnotherSequence =
                            coordsInSequences.Contains(c) &&
                            chips.ContainsKey(c) && // 'chips' won't contain c is it's a corner.
                            chips[c] == team;

                        if (isPartOfAnotherSequence)
                        {
                            shared++;
                        }
                    }
                    else
                    {
                        // This is not a valid sequence. Exit early.
                        return false;
                    }
                }

                // This is a valid sequence if all coords of 'cs' are owned by 'team' and only 1
                // other coord is part of a sequence.
                return shared < 2;
            }

            bool TrySequence(IEnumerable<Coord> cs, out IImmutableList<Coord> seq)
            {
                int i = 0;

                while (true)
                {
                    seq = cs
                        .Skip(i++)
                        .Take(Seq.DefaultLength)
                        .ToImmutableList();

                    if (seq.Count < Seq.DefaultLength)
                    {
                        break;
                    }

                    if (IsSequence(seq))
                    {
                        return true;
                    }
                }

                return false;
            }

            var range = Enumerable.Range(-5, 11);
            var vertical = range.Select(d => new Coord(col, row + d));
            var horizontal = range.Select(d => new Coord(col + d, row));
            var diagonal1 = range.Select(d => new Coord(col + d, row + d));
            var diagonal2 = range.Select(d => new Coord(col + d, row - d));

#pragma warning disable IDE0018
            IImmutableList<Coord> coords;

            if (TrySequence(vertical, out coords) ||
                TrySequence(horizontal, out coords) ||
                TrySequence(diagonal1, out coords) ||
                TrySequence(diagonal2, out coords))
            {
                return new Seq(team, coords);
            }
#pragma warning restore IDE0018

            return null;
        }

        public static IImmutableList<Seq> GetSequences(
            this Board board,
            IImmutableDictionary<Coord, Team> chips,
            IImmutableSet<Coord> coordsInSequences,
            Coord coord,
            Team team)
        {
            var row = coord.Row;
            var col = coord.Column;

            bool IsSequence(IEnumerable<Coord> cs)
            {
                int shared = 0;

                foreach (var c in cs)
                {
                    var isCorner =
                        (c.Row >= 0 && c.Row < board.Length) &&
                        (c.Column >= 0 && c.Column < board[c.Row].Length) &&
                        board[c.Row][c.Column] == null;

                    var teamOwnsCoord = chips.TryGetValue(c, out var t) && team == t;

                    if (isCorner || teamOwnsCoord)
                    {
                        var isPartOfAnotherSequence =
                            coordsInSequences.Contains(c) &&
                            chips.ContainsKey(c) && // 'chips' won't contain c is it's a corner.
                            chips[c] == team;

                        if (isPartOfAnotherSequence)
                        {
                            shared++;
                        }
                    }
                    else
                    {
                        // This is not a valid sequence. Exit early.
                        return false;
                    }
                }

                // This is a valid sequence if all coords of 'cs' are owned by 'team' and only 1
                // other coord is part of a sequence.
                return shared < 2;
            }

            IEnumerable<Seq> TrySequence(IEnumerable<Coord> cs)
            {
                int i = 0;

                while (true)
                {
                    var seq = cs
                        .Skip(i++)
                        .Take(Seq.DefaultLength)
                        .ToImmutableList();

                    if (seq.Count < Seq.DefaultLength)
                    {
                        // Not enough coordinates to form a sequence, stop.
                        yield break;
                    }

                    if (IsSequence(seq))
                    {
                        yield return new Seq(team, seq);

                        // One coordinate can be shared so we jump forward to that one in the next
                        // iteration. Since 'i' has already been incremented once, we add only
                        // N - 2, not N - 1.
                        i += Seq.DefaultLength - 2;
                    }
                }
            }

            var range = Enumerable.Range(-5, 11);

            return new[]
            {
                range.Select(d => new Coord(col, row + d)),     // Vertical
                range.Select(d => new Coord(col + d, row)),     // Horizontal
                range.Select(d => new Coord(col + d, row + d)), // (0, 0) -> (9, 9) diagonal
                range.Select(d => new Coord(col + d, row - d)), // (0, 9) -> (9, 0) diagonal
            }.AsParallel()
             .SelectMany(TrySequence)
             .ToImmutableList();
        }
    }
}
