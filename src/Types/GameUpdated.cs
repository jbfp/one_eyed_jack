namespace Sequence
{
    public sealed record GameUpdated(IGameEvent[] GameEvents, int Version);
}
