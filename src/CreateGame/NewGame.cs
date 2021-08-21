namespace Sequence.CreateGame
{
    public sealed class NewGame
    {
        public const int MaxNumberOfSequencesToWin = 4;

        public NewGame(
            PlayerList players,
            int firstPlayerIndex,
            Seed seed,
            BoardType boardType,
            int numSequencesToWin)
        {
            PlayerList = players;
            FirstPlayerIndex = firstPlayerIndex;
            Seed = seed;

            if (Enum.IsDefined(typeof(BoardType), boardType))
            {
                BoardType = boardType;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(boardType), boardType, null);
            }

            if (numSequencesToWin > 0 && numSequencesToWin <= MaxNumberOfSequencesToWin)
            {
                NumberOfSequencesToWin = numSequencesToWin;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(numSequencesToWin), numSequencesToWin, null);
            }
        }

        public PlayerList PlayerList { get; }
        public int FirstPlayerIndex { get; }
        public Seed Seed { get; }
        public BoardType BoardType { get; }
        public int NumberOfSequencesToWin { get; }
    }
}
