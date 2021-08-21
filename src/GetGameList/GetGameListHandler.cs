namespace Sequence.GetGameList
{
    public sealed class GetGameListHandler
    {
        private readonly IGameListProvider _provider;

        public GetGameListHandler(IGameListProvider provider)
        {
            _provider = provider;
        }

        public async Task<GameList> GetGamesForPlayerAsync(
            PlayerHandle player,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _provider.GetGamesForPlayerAsync(player, cancellationToken);
        }
    }
}
