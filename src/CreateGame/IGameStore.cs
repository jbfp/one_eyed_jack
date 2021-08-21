namespace Sequence.CreateGame
{
    public interface IGameStore
    {
        Task<GameId> PersistNewGameAsync(NewGame newGame, CancellationToken cancellationToken);
    }
}
