namespace Sequence.PlayCard
{
    public sealed class PlayCardFailedException : Exception
    {
        public PlayCardFailedException(PlayCardError error) : base(error.ToString())
        {
            Error = error;
        }

        public PlayCardError Error { get; }
    }

    public sealed class ExchangeDeadCardFailedException : Exception
    {
        public ExchangeDeadCardFailedException(ExchangeDeadCardError error) : base(error.ToString())
        {
            Error = error;
        }

        public ExchangeDeadCardError Error { get; }
    }
}
