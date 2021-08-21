using System.Collections.Immutable;

namespace Sequence
{
    public interface IBot
    {
        Move? Decide(IImmutableList<Move> moves);
    }
}
