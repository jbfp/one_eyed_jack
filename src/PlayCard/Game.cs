using System.Collections.Immutable;

namespace Sequence.PlayCard
{
    public static class Game
    {
        public static GameEvent PlayCard(GameState state, PlayerHandle player, Card card, Coord coord)
        {
            return PlayCard(state, state.PlayerHandleByIdx.IndexOf(player), card, coord);
        }

        public static GameEvent PlayCard(GameState state, PlayerId player, Card card, Coord coord)
        {
            return PlayCard(state, state.PlayerIdByIdx.IndexOf(player), card, coord);
        }

        private static GameEvent PlayCard(GameState state, int playerIdx, Card card, Coord coord)
        {
            if (playerIdx == -1)
            {
                throw new PlayCardFailedException(PlayCardError.PlayerIsNotInGame);
            }

            var playerId = state.PlayerIdByIdx[playerIdx];

            if (!playerId.Equals(state.CurrentPlayerId))
            {
                throw new PlayCardFailedException(PlayCardError.PlayerIsNotCurrentPlayer);
            }

            if (!state.PlayerHandByIdx[playerIdx].Contains(card))
            {
                throw new PlayCardFailedException(PlayCardError.PlayerDoesNotHaveCard);
            }

            var chips = state.Chips;
            var deck = ImmutableStack.CreateRange(state.Deck);
            var team = state.PlayerTeamByIdx[playerIdx];

            if (card.IsOneEyedJack())
            {
                if (!chips.ContainsKey(coord))
                {
                    throw new PlayCardFailedException(PlayCardError.CoordIsEmpty);
                }

                if (chips.TryGetValue(coord, out var chip) && chip == team)
                {
                    throw new PlayCardFailedException(PlayCardError.ChipBelongsToPlayerTeam);
                }

                if (state.CoordsInSequence.Contains(coord))
                {
                    throw new PlayCardFailedException(PlayCardError.ChipIsPartOfSequence);
                }

                return new GameEvent
                {
                    ByPlayerId = playerId,
                    CardDrawn = deck.Peek(),
                    CardUsed = card,
                    Coord = coord,
                    Index = state.Version + 1,
                    NextPlayerId = state.PlayerIdByIdx[(playerIdx + 1) % state.NumberOfPlayers],
                    Sequences = Array.Empty<Seq>(),
                };
            }
            else
            {
                if (chips.ContainsKey(coord))
                {
                    throw new PlayCardFailedException(PlayCardError.CoordIsOccupied);
                }

                var board = state.BoardType.Board;

                if (!board.Matches(coord, card))
                {
                    throw new PlayCardFailedException(PlayCardError.CardDoesNotMatchCoord);
                }

                var sequences = board.GetSequences(
                    chips: chips.Add(coord, team),
                    state.CoordsInSequence,
                    coord, team);

                Team? winnerTeam = null;

                if (sequences.Count > 0)
                {
                    // Test for win condition:
                    var numSequencesForTeam = state.Sequences
                        .AddRange(sequences)
                        .Count(seq => seq.Team == team);

                    if (numSequencesForTeam >= state.WinCondition)
                    {
                        winnerTeam = team;
                    }
                }

                var nextPlayerId = winnerTeam == null
                    ? state.PlayerIdByIdx[(playerIdx + 1) % state.NumberOfPlayers]
                    : null;

                return new GameEvent
                {
                    ByPlayerId = playerId,
                    CardDrawn = deck.Peek(),
                    CardUsed = card,
                    Chip = team,
                    Coord = coord,
                    Index = state.Version + 1,
                    NextPlayerId = nextPlayerId,
                    Sequences = sequences.ToArray(),
                    Winner = winnerTeam
                };
            }
        }

        public static GameEvent ExchangeDeadCard(GameState state, PlayerHandle player, Card deadCard)
        {
            return ExchangeDeadCard(state, state.PlayerHandleByIdx.IndexOf(player), deadCard);
        }

        public static GameEvent ExchangeDeadCard(GameState state, PlayerId player, Card deadCard)
        {
            return ExchangeDeadCard(state, state.PlayerIdByIdx.IndexOf(player), deadCard);
        }

        private static GameEvent ExchangeDeadCard(GameState state, int playerIdx, Card deadCard)
        {
            if (playerIdx == -1)
            {
                throw new ExchangeDeadCardFailedException(ExchangeDeadCardError.PlayerIsNotInGame);
            }

            var playerId = state.PlayerIdByIdx[playerIdx];

            if (!playerId.Equals(state.CurrentPlayerId))
            {
                throw new ExchangeDeadCardFailedException(ExchangeDeadCardError.PlayerIsNotCurrentPlayer);
            }

            if (!state.PlayerHandByIdx[playerIdx].Contains(deadCard))
            {
                throw new ExchangeDeadCardFailedException(ExchangeDeadCardError.PlayerDoesNotHaveCard);
            }

            if (!state.DeadCards.Contains(deadCard))
            {
                throw new ExchangeDeadCardFailedException(ExchangeDeadCardError.CardIsNotDead);
            }

            if (state.HasExchangedDeadCard)
            {
                throw new ExchangeDeadCardFailedException(ExchangeDeadCardError.PlayerHasAlreadyExchangedDeadCard);
            }

            var deck = ImmutableStack.CreateRange(state.Deck);

            return new GameEvent
            {
                ByPlayerId = playerId,
                CardDrawn = deck.Peek(),
                CardUsed = deadCard,
                Coord = new Coord(-1, -1),
                Index = state.Version + 1,
                NextPlayerId = playerId,
                Sequences = Array.Empty<Seq>(),
            };
        }
    }
}
