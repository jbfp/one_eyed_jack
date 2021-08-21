using System.Collections.Immutable;

namespace Sequence
{
    public sealed class GameState
    {
        private static readonly Coord _deadCardExchangedCoord = new(-1, -1);

        private static readonly ImmutableDictionary<int, ImmutableArray<Team>> _teams =
            ImmutableDictionary<int, ImmutableArray<Team>>
                .Empty
                .Add(2, ImmutableArray.Create(Team.Red, Team.Green))
                .Add(3, ImmutableArray.Create(Team.Red, Team.Green, Team.Blue))
                .Add(4, ImmutableArray.Create(Team.Red, Team.Green,
                                              Team.Red, Team.Green))
                .Add(6, ImmutableArray.Create(Team.Red, Team.Green, Team.Blue,
                                              Team.Red, Team.Green, Team.Blue));

        private readonly GameInit _init;

        public GameState(GameInit init, params GameEvent[] gameEvents)
        {
            _init = init;

            var numPlayers = init.Players.Count;
            var deck = new Deck(init.Seed, numPlayers);
            var hands = deck.DealHands();
            var teams = _teams[numPlayers];

            for (int i = 0; i < numPlayers; i++)
            {
                Player player = init.Players[i];
                PlayerHandByIdx = PlayerHandByIdx.Add(hands[i]);
                PlayerHandleByIdx = PlayerHandleByIdx.Add(player.Handle);
                PlayerIdByIdx = PlayerIdByIdx.Add(player.Id);
                PlayerTeamByIdx = PlayerTeamByIdx.Add(teams[i]); // TODO: Get team from init.
                PlayerTypeByIdx = PlayerTypeByIdx.Add(player.Type);
            }

            BoardType = init.BoardType.Create()!;
            CurrentPlayerId = init.FirstPlayerId;
            Deck = deck.ToImmutableList();
            NumberOfPlayers = numPlayers;
            WinCondition = init.NumberOfSequencesToWin;

            foreach (var gameEvent in gameEvents)
            {
                var cardDrawn = gameEvent.CardDrawn;
                var cardUsed = gameEvent.CardUsed;
                var playerIdx = PlayerIdByIdx.IndexOf(gameEvent.ByPlayerId);
                var playerHand = PlayerHandByIdx[playerIdx].Remove(cardUsed);

                if (gameEvent.CardDrawn != null)
                {
                    Deck = Deck.Remove(gameEvent.CardDrawn);
                    playerHand = playerHand.Add(gameEvent.CardDrawn);
                }

                PlayerHandByIdx = PlayerHandByIdx.SetItem(playerIdx, playerHand);
                CurrentPlayerId = gameEvent.NextPlayerId!;
                Discards = Discards.Add(cardUsed);
                GameEvents = GameEvents.Add(gameEvent);
                Version = gameEvent.Index;

                if (Deck.Count == 0)
                {
                    Deck = Sequence.Deck.Shuffle(Discards, init.Seed);
                    Discards = ImmutableList<Card>.Empty;
                }

                if (gameEvent.Coord.Equals(_deadCardExchangedCoord))
                {
                    HasExchangedDeadCard = true;
                }
                else
                {
                    HasExchangedDeadCard = false;

                    if (gameEvent.Chip.HasValue)
                    {
                        Chips = Chips.Add(gameEvent.Coord, gameEvent.Chip.Value);
                    }
                    else
                    {
                        Chips = Chips.Remove(gameEvent.Coord);
                    }

                    Sequences = Sequences.AddRange(gameEvent.Sequences);

                    foreach (var coord in gameEvent.Sequences.SelectMany(seq => seq.Coords))
                    {
                        CoordsInSequence = CoordsInSequence.Add(coord);
                    }

                    Winner = gameEvent.Winner;
                }

                DeadCards = PlayerHandByIdx
                    .SelectMany(hand => hand)
                    .Where(IsCardDead)
                    .ToImmutableHashSet();
            }
        }

        public IBoardType BoardType { get; }
        public IImmutableDictionary<Coord, Team> Chips { get; } =
            ImmutableDictionary<Coord, Team>.Empty;
        public IImmutableSet<Coord> CoordsInSequence { get; } = ImmutableHashSet<Coord>.Empty;
        public PlayerId CurrentPlayerId { get; }
        public IImmutableSet<Card> DeadCards { get; } = ImmutableHashSet<Card>.Empty;
        public IImmutableList<Card> Deck { get; } = ImmutableList<Card>.Empty;
        public IImmutableList<Card> Discards { get; } = ImmutableList<Card>.Empty;
        public IImmutableList<GameEvent> GameEvents { get; } = ImmutableList<GameEvent>.Empty;
        public bool HasExchangedDeadCard { get; }
        public int NumberOfPlayers { get; }
        public IImmutableList<IImmutableList<Card>> PlayerHandByIdx { get; } =
            ImmutableList<IImmutableList<Card>>.Empty;
        public IImmutableList<PlayerHandle> PlayerHandleByIdx { get; } =
            ImmutableList<PlayerHandle>.Empty;
        public IImmutableList<PlayerId> PlayerIdByIdx { get; } =
            ImmutableList<PlayerId>.Empty;
        public IImmutableList<Team> PlayerTeamByIdx { get; } = ImmutableList<Team>.Empty;
        public IImmutableList<PlayerType> PlayerTypeByIdx { get; } =
            ImmutableList<PlayerType>.Empty;
        public IImmutableList<Seq> Sequences { get; } = ImmutableList<Seq>.Empty;
        public int Version { get; }
        public int WinCondition { get; }
        public Team? Winner { get; }

        public GameState Apply(GameEvent gameEvent)
        {
            return Apply(this, gameEvent);
        }

        public static GameState Apply(GameState state, GameEvent gameEvent)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (gameEvent == null)
            {
                throw new ArgumentNullException(nameof(gameEvent));
            }

            return new GameState(state._init, state.GameEvents.Add(gameEvent).ToArray());
        }

        private bool IsCardDead(Card card)
        {
            var tile = new Tile(card.Suit, card.Rank);

            return BoardType.CoordsByTile.TryGetValue(tile, out var coords)
                && Chips.ContainsKey(coords.Item1)
                && Chips.ContainsKey(coords.Item2);
        }
    }
}
