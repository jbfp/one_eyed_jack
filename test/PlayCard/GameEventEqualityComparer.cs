namespace Sequence.Test.PlayCard
{
    public sealed class GameEventEqualityComparer : EqualityComparer<GameEvent>
    {
        public override bool Equals(GameEvent? x, GameEvent? y)
        {
            if (x is null || y is null)
            {
                return false;
            }

            return x.ByPlayerId.Equals(y.ByPlayerId)
                && EqualityComparer<Card?>.Default.Equals(x.CardDrawn, y.CardDrawn)
                && x.CardUsed.Equals(y.CardUsed)
                && x.Chip.Equals(y.Chip)
                && x.Coord.Equals(y.Coord)
                && x.Index.Equals(y.Index)
                && (x.NextPlayerId?.Equals(y.NextPlayerId) ?? true)
                && x.Sequences.SequenceEqual(y.Sequences);
        }

        public override int GetHashCode(GameEvent obj)
        {
            throw new NotImplementedException();
        }
    }
}
