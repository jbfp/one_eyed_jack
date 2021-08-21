using Microsoft.AspNetCore.Mvc;
using Sequence.AspNetCore;

namespace Sequence.GetGame
{
    public sealed class GetGameController : SequenceControllerBase
    {
        private const int BoardCacheTime = 10519200; // seconds.

        private readonly IGameProvider _provider;

        public GetGameController(IGameProvider provider)
        {
            _provider = provider;
        }

        [HttpGet("/boards")]
        public ActionResult<string[]> Get()
        {
            var enumType = typeof(BoardType);
            var boardTypes = Enum.GetNames(enumType);
            return boardTypes;
        }

        [HttpGet("/boards/{boardType}")]
        [ResponseCache(Duration = BoardCacheTime)]
        public ActionResult Get([Enum(typeof(BoardType))] BoardType boardType)
        {
            IBoardType? boardTypeInstance;

            try
            {
                boardTypeInstance = boardType.Create();
            }
            catch (ArgumentOutOfRangeException)
            {
                return ValidationProblem(ModelState);
            }

            return Ok(boardTypeInstance!.Board);
        }


        [HttpGet("/games/{id:guid}")]
        public async Task<ActionResult<object>> Get(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var gameId = new GameId(id);
            var game = await _provider.GetGameByIdAsync(gameId, cancellationToken);

            if (game == null)
            {
                return NotFound();
            }

            var init = game.Init(Player);

            if (init == null)
            {
                return NotFound();
            }

            return new { init, updates = game.GenerateForPlayer(Player) };
        }
    }
}
