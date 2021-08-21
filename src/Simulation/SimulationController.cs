using Microsoft.AspNetCore.Mvc;
using Sequence.AspNetCore;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Sequence.Simulation
{
    [Route("/simulations")]
    public sealed class SimulationController : SequenceControllerBase
    {
        private readonly SimulationHandler _handler;

        public SimulationController(SimulationHandler handler)
        {
            _handler = handler;
        }

        [HttpGet]
        public async Task<ActionResult> Get(
            CancellationToken cancellationToken)
        {
            return Ok(await _handler.GetSimulationsAsync(Player, cancellationToken));
        }

        [HttpPost]
        public async Task<ActionResult> Post(
            [FromBody] SimulationForm form,
            CancellationToken cancellationToken)
        {
            var parameters = new SimulationParams
            (
                BoardType: form.BoardType!.Value,
                CreatedBy: Player,
                Players: form.Bots!.Select(type => new Bot(type)).ToImmutableList(),
                RandomFirstPlayer: form.RandomFirstPlayer!.Value,
                Seed: new Seed(form.Seed!.Value),
                WinCondition: form.NumSequencesToWin!.Value
            );

            GameId gameId;

            try
            {
                gameId = await _handler.CreateSimulationAsync(parameters, cancellationToken);
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest(new { error = "Number of sequences to win is invalid." });
            }

            return Created($"/simulations/{gameId}", new { gameId });
        }
    }

    public sealed class SimulationForm : IValidatableObject
    {
        [Required, Enum(typeof(BoardType))]
        public BoardType? BoardType { get; set; }

        [Required]
        public int? NumSequencesToWin { get; set; }

        [Required]
        public string[]? Bots { get; set; }

        [Required]
        public bool? RandomFirstPlayer { get; set; }

        [Required]
        public int? Seed { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validGameSizes = new[] { 2, 3, 4, 6 };

            if (validationContext.ObjectInstance is SimulationForm form)
            {
                if (!validGameSizes.Contains(form.Bots!.Length))
                {
                    var errorMessage = "Game size is not valid.";
                    var memberNames = new[] { nameof(Bots) };
                    yield return new ValidationResult(errorMessage, memberNames);
                }

                foreach (var bot in form.Bots)
                {
                    if (!BotProvider.BotTypes.ContainsKey(bot))
                    {
                        var errorMessage = $"The bot type '{bot}' does not exist.";
                        var memberNames = new[] { nameof(Bots) };
                        yield return new ValidationResult(errorMessage, memberNames);
                    }
                }
            }
        }
    }
}
