namespace Sequence.GetGameList
{
    public interface IGameListProvider
    {
        Task<GameList> GetGamesForPlayerAsync(
            PlayerHandle player,
            CancellationToken cancellationToken);
    }
}
