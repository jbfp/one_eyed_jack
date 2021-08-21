namespace Sequence
{
    public interface IGameEvent
    {
    }

    public sealed record CardDiscarded(PlayerId ByPlayerId, Card Card) : IGameEvent;

    public sealed record ChipAdded(Coord Coord, Team Team) : IGameEvent;

    public sealed record ChipRemoved(Coord Coord) : IGameEvent;

    public sealed record CardDrawn(PlayerId ByPlayerId, Card? Card) : IGameEvent;

    public sealed record DeckShuffled(int NumCardsInDeck) : IGameEvent;

    public sealed record CardDied(Card Card) : IGameEvent;

    public sealed record CardRevived(Card Card) : IGameEvent;

    public sealed record SequenceCreated(Seq Sequence) : IGameEvent;

    public sealed record TurnEnded(PlayerId NextPlayerId) : IGameEvent;

    public sealed record GameEnded(Team WinnerTeam) : IGameEvent;
}
