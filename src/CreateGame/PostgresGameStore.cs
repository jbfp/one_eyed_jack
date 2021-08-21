using Dapper;
using Sequence.Postgres;

namespace Sequence.CreateGame
{
    public sealed class PostgresGameStore : IGameStore
    {
        private readonly NpgsqlConnectionFactory _connectionFactory;

        public PostgresGameStore(NpgsqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<GameId> PersistNewGameAsync(NewGame newGame, CancellationToken cancellationToken)
        {
            if (newGame == null)
            {
                throw new ArgumentNullException(nameof(newGame));
            }

            cancellationToken.ThrowIfCancellationRequested();

            GameId gameId;

            using (var connection = await _connectionFactory.CreateAndOpenAsync(cancellationToken))
            {
                using var transaction = connection.BeginTransaction();

                try
                {
                    int surrogateGameId = default;
                    int firstPlayerId = default;

                    {
                        var commandText = @"
                            INSERT INTO
                                game (board_type, num_sequences_to_win, seed, version)
                            VALUES
                                (@boardType, @numSequencesToWin, @seed, @version)
                            RETURNING id, game_id;";

                        var parameters = new
                        {
                            boardType = (int)newGame.BoardType,
                            numSequencesToWin = newGame.NumberOfSequencesToWin,
                            seed = newGame.Seed,
                            version = 1
                        };

                        var command = new CommandDefinition(
                            commandText,
                            parameters,
                            transaction,
                            cancellationToken: cancellationToken
                        );

                        var result = await connection.QuerySingleAsync<insert_into_game>(command);
                        surrogateGameId = result.id;
                        gameId = result.game_id;
                    }

                    for (int i = 0; i < newGame.PlayerList.Players.Count; i++)
                    {
                        var commandText = @"
                            INSERT INTO
                                game_player (game_id, player_id, player_type)
                            VALUES
                                (@gameId, @playerId, @playerType::player_type)
                            RETURNING id;";

                        var player = newGame.PlayerList.Players[i];

                        var parameters = new
                        {
                            gameId = surrogateGameId,
                            playerId = player.Handle,
                            playerType = player.Type.ToString().ToLowerInvariant(),
                        };

                        var command = new CommandDefinition(
                            commandText,
                            parameters,
                            transaction,
                            cancellationToken: cancellationToken
                        );

                        var result = await connection.QuerySingleAsync<int>(command);

                        if (newGame.FirstPlayerIndex == i)
                        {
                            firstPlayerId = result;
                        }
                    }

                    {
                        var commandText = @"
                            UPDATE game
                               SET first_player_id = @firstPlayerId
                             WHERE id = @gameId;";

                        var parameters = new { firstPlayerId, gameId = surrogateGameId };

                        var command = new CommandDefinition(
                            commandText,
                            parameters,
                            transaction,
                            cancellationToken: cancellationToken
                        );

                        await connection.ExecuteAsync(command);
                    }

                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }

            return gameId;
        }

#pragma warning disable CS0649, IDE1006
        private sealed record insert_into_game
        {
            public int id;
            public GameId game_id = null!;
        }
#pragma warning restore CS0649, IDE1006
    }
}
