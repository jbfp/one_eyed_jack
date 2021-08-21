namespace Sequence
{
    public sealed class GameEvent
    {
        public PlayerId ByPlayerId { get; set; } = null!;
        public Card? CardDrawn { get; set; }
        public Card CardUsed { get; set; } = null!;
        public Team? Chip { get; set; }
        public Coord Coord { get; set; }
        public int Index { get; set; }
        public PlayerId? NextPlayerId { get; set; }
        public Seq[] Sequences { get; set; } = Array.Empty<Seq>();
        public Team? Winner { get; set; }
    }
}
