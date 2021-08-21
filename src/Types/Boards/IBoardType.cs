using System.Collections.Immutable;

namespace Sequence
{
    public interface IBoardType
    {
        ImmutableArray<ImmutableArray<Tile?>> Board { get; }
        IImmutableDictionary<Tile, (Coord, Coord)> CoordsByTile { get; }
    }
}
