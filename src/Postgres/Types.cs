namespace Sequence.Postgres
{
#pragma warning disable CS0649
    internal sealed class GameEventRow
    {
        public GameId surrogate_game_id = null!;
        public int game_id;
        public int idx;
        public CardComposite? card_drawn;
        public CardComposite card_used = null!;
        public Team? chip;
        public CoordComposite coord = null!;
        public int id;
        public DateTimeOffset timestamp;
        public SequenceComposite[] sequences = null!;
        public PlayerId by_player_id = null!;
        public PlayerId? next_player_id;
        public Team? winner;

        public GameEvent ToGameEvent() => ToGameEvent(this);

        public static GameEvent ToGameEvent(GameEventRow row)
        {
            return new GameEvent
            {
                ByPlayerId = row.by_player_id,
                CardDrawn = row.card_drawn?.ToCard(),
                CardUsed = row.card_used.ToCard(),
                Chip = row.chip,
                Coord = row.coord.ToCoord(),
                Index = row.idx,
                NextPlayerId = row.next_player_id,
                Sequences = row.sequences.Select(SequenceComposite.ToSequence).ToArray(),
                Winner = row.winner
            };
        }
    }
#pragma warning restore CS0649
}
