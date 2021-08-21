using Microsoft.AspNetCore.Mvc;
using Sequence.AspNetCore;

namespace Sequence.GetGameList
{
    public sealed class GetGameListController : SequenceControllerBase
    {
        private readonly GetGameListHandler _handler;

        public GetGameListController(GetGameListHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("/games")]
        public async Task<ActionResult<GameList>> Get(CancellationToken cancellationToken)
        {
            return await _handler.GetGamesForPlayerAsync(Player, cancellationToken);
        }
    }
}
