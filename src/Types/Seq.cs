using System.Collections.Immutable;

namespace Sequence
{
    public sealed class Seq
    {
        public const int DefaultLength = 5;

        public Seq(Team team, IImmutableList<Coord> coords)
        {
            Team = team;

            if (coords.Count != DefaultLength)
            {
                throw new ArgumentException($"Must have {DefaultLength} coords.", nameof(coords));
            }

            Coords = coords ?? throw new ArgumentNullException(nameof(coords));
        }

        public Team Team { get; }
        public IImmutableList<Coord> Coords { get; }
    }
}
