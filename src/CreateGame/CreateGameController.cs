using Microsoft.AspNetCore.Mvc;
using Sequence.AspNetCore;
using System.ComponentModel.DataAnnotations;

namespace Sequence.CreateGame
{
    public sealed class CreateGameController : SequenceControllerBase
    {
        private readonly CreateGameHandler _handler;
        private readonly ILogger<CreateGameController> _logger;

        public CreateGameController(CreateGameHandler handler, ILogger<CreateGameController> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        [HttpGet("/bots")]
        public ActionResult<BotListResult> Get() => new BotListResult
        {
            BotTypes = BotProvider.BotTypes.Keys.ToArray()
        };

        [HttpPost("/games")]
        public async Task<ActionResult> Post(
            [FromBody] CreateGameForm form,
            CancellationToken cancellationToken)
        {
            var players = form.Opponents!
                .Select(opponent => new NewPlayer(new PlayerHandle(opponent.Name), opponent.Type!.Value))
                .Prepend(new NewPlayer(Player, PlayerType.User))
                .ToArray();

            PlayerList playerList;

            _logger.LogInformation("Attempting to create game for {Players}", players);

            try
            {
                playerList = new PlayerList(form.RandomFirstPlayer, players);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogWarning(ex, "Game size invalid for {Players}", players);
                return BadRequest(new { error = "Game size is invalid." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Duplicate players in {Players}", players);
                return BadRequest(new { error = "Duplicate players are not allowed." });
            }

            var boardType = form.BoardType!.Value;
            var numSequencesToWin = form.NumSequencesToWin;

            GameId gameId;

            try
            {
                gameId = await _handler.CreateGameAsync(playerList, boardType, numSequencesToWin!.Value, cancellationToken);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogWarning(ex, "Number of sequences to win {NumSequencesToWin} is invalid", numSequencesToWin);
                return BadRequest(new { error = "Number of sequences to win is invalid." });
            }

            _logger.LogInformation(
                "Successfully created game with ID {GameId} for {Players}",
                gameId, players);

            return Created($"/games/{gameId}", new { gameId });
        }
    }

    public sealed class BotListResult
    {
        public string[] BotTypes { get; set; } = Array.Empty<string>();
    }

    public sealed class CreateGameForm
    {
        [Required, Enum(typeof(BoardType))]
        public BoardType? BoardType { get; set; }

        [Required]
        public int? NumSequencesToWin { get; set; }

        [Required]
        public Opponent[]? Opponents { get; set; }

        [Required]
        public bool RandomFirstPlayer { get; set; }
    }

    public sealed class Opponent : IValidatableObject
    {
        [Required]
        public PlayerType? Type { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is Opponent opponent)
            {
                if (opponent.Type == PlayerType.Bot && !BotProvider.BotTypes.ContainsKey(opponent.Name))
                {
                    var errorMessage = $"The bot type '{opponent.Name}' does not exist.";
                    var memberNames = new[] { nameof(Name) };
                    yield return new ValidationResult(errorMessage, memberNames);
                }
            }
        }
    }
}
