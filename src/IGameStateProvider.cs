namespace Sequence
{
    public interface IGameStateProvider
    {
        Task<GameState?> GetGameByIdAsync(GameId gameId, CancellationToken cancellationToken);
    }
}
