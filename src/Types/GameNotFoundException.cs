namespace Sequence
{
    [Serializable]
    public sealed class GameNotFoundException : Exception
    {
        public GameNotFoundException()
        {
        }

        public GameNotFoundException(string message) : base(message)
        {
        }

        public GameNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
