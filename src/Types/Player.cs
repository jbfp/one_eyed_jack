namespace Sequence
{
    public sealed record Player(PlayerId Id, PlayerHandle Handle, PlayerType Type) : IEquatable<Player>;
}
