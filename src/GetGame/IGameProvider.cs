namespace Sequence.GetGame
{
    public interface IGameProvider
    {
        Task<Game?> GetGameByIdAsync(
            GameId gameId,
            CancellationToken cancellationToken);
    }
}
