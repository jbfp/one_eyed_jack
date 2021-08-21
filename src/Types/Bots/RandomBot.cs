using System.Collections.Immutable;

namespace Sequence
{
    [Bot(Name)]
    public sealed class RandomBot : IBot
    {
        public const string Name = "Random Bot";

        private readonly Random _rng;

        public RandomBot() => _rng = new Random();
        public RandomBot(int seed) => _rng = new Random(seed);

        public Move? Decide(IImmutableList<Move> moves)
        {
            Thread.Sleep(2500);

            var numMoves = moves.Count;

            if (numMoves == 0)
            {
                return null;
            }

            return moves[_rng.Next(numMoves)];
        }
    }
}
