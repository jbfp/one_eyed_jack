namespace Sequence
{
    public enum BoardType
    {
        OneEyedJack,
        Sequence,
    }

    public static class BoardTypeExtensions
    {
        public static Type ToType(this BoardType boardType) => boardType switch
        {
            BoardType.OneEyedJack => typeof(OneEyedJackBoard),
            BoardType.Sequence => typeof(SequenceBoard),
            _ => throw new ArgumentOutOfRangeException(nameof(boardType), boardType, null),
        };

        public static IBoardType? Create(this BoardType boardType) => Activator.CreateInstance(boardType.ToType()) as IBoardType;
    }
}
