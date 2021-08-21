using Microsoft.AspNetCore.Mvc;
using Sequence.AspNetCore;
using System.ComponentModel.DataAnnotations;

namespace Sequence.PlayCard
{
    public sealed class PlayCardController : SequenceControllerBase
    {
        private readonly PlayCardHandler _handler;

        public PlayCardController(PlayCardHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("/games/{id:guid}/moves")]
        public async Task<ActionResult> Get(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var gameId = new GameId(id);
            var moves = await _handler.GetMovesForPlayerAsync(gameId, Player, cancellationToken);
            return Ok(moves);
        }

        [HttpPost("/games/{id:guid}")]
        [PlayCardFailedExceptionFilter]
        public async Task<ActionResult> Post(
            [FromRoute] Guid id,
            [FromBody] PlayCardForm form,
            CancellationToken cancellationToken)
        {
            var gameId = new GameId(id);
            var coord = new Coord(form.Column!.Value, form.Row!.Value);
            var updates = await _handler.PlayCardAsync(gameId, Player, form.Card!, coord, cancellationToken);
            return Ok(new { updates });
        }

        [HttpPost("/games/{id:guid}/dead-card")]
        [ExchangeDeadCardFailedExceptionFilter]
        public async Task<ActionResult> Post(
            [FromRoute] Guid id,
            [FromBody] ExchangeDeadCardForm form,
            CancellationToken cancellationToken)
        {
            var gameId = new GameId(id);
            var deadCard = form.DeadCard!;
            var updates = await _handler.ExchangeDeadCardAsync(gameId, Player, deadCard, cancellationToken);
            return Ok(new { updates });
        }
    }

    public sealed class PlayCardForm
    {
        [Required]
        public Card? Card { get; set; }

        [Required]
        public int? Column { get; set; }

        [Required]
        public int? Row { get; set; }
    }

    public sealed class ExchangeDeadCardForm
    {
        [Required]
        public Card? DeadCard { get; set; }
    }
}
