using System.Collections.Immutable;

namespace Sequence.GetGame
{
    public sealed record InitialGameState(
        BoardType BoardType, PlayerId FirstPlayerId, IImmutableList<Card>? Hand,
        int NumCardsInDeck, PlayerHandle? PlayerHandle, PlayerId? PlayerId,
        IImmutableList<Player> Players, Team? Team, int WinCondition);

    public sealed record Player(PlayerHandle Handle, PlayerId Id, int NumberOfCards, Team Team, PlayerType Type);
}
