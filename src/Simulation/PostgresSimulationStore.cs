using Dapper;
using Sequence.Postgres;
using System.Collections.Immutable;

namespace Sequence.Simulation
{
    public sealed class PostgresSimulationStore : ISimulationStore
    {
        private readonly NpgsqlConnectionFactory _connectionFactory;

        public PostgresSimulationStore(NpgsqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IImmutableList<GameId>> GetSimulationsAsync(
            PlayerHandle player,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IEnumerable<GameId> gameIds;

            using (var connection = await _connectionFactory.CreateAndOpenAsync(cancellationToken))
            {
                var commandText = @"
                    SELECT g.game_id
                    FROM public.simulation AS s
                    INNER JOIN public.game AS g
                    ON g.id = s.game_id
                    WHERE s.created_by = @player;";

                var parameters = new { player };

                var command = new CommandDefinition(
                    commandText,
                    parameters,
                    cancellationToken: cancellationToken);

                gameIds = await connection.QueryAsync<GameId>(command);
            }

            return gameIds.ToImmutableList();
        }

        public async Task<GameId> SaveNewSimulationAsync(
            NewSimulation newSimulation,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            GameId gameId;

            using (var connection = await _connectionFactory.CreateAndOpenAsync(cancellationToken))
            using (var transaction = connection.BeginTransaction())
            {
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
                            boardType = (int)newSimulation.BoardType,
                            numSequencesToWin = newSimulation.WinCondition,
                            seed = newSimulation.Seed,
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

                    for (int i = 0; i < newSimulation.Players.Count; i++)
                    {
                        var commandText = @"
                            INSERT INTO
                                game_player (game_id, player_id, player_type)
                            VALUES
                                (@gameId, @playerId, @playerType::player_type)
                            RETURNING id;";

                        var player = newSimulation.Players[i];

                        var parameters = new
                        {
                            gameId = surrogateGameId,
                            playerId = player.BotType.ToString(),
                            playerType = PlayerType.Bot.ToString().ToLowerInvariant(),
                        };

                        var command = new CommandDefinition(
                            commandText,
                            parameters,
                            transaction,
                            cancellationToken: cancellationToken
                        );

                        var result = await connection.QuerySingleAsync<int>(command);

                        if (newSimulation.FirstPlayerIndex == i)
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

                    {
                        var commandText = @"
                            INSERT INTO
                                simulation (game_id, created_by)
                            VALUES
                                (@gameId, @createdBy);";

                        var parameters = new
                        {
                            gameId = surrogateGameId,
                            createdBy = newSimulation.CreatedBy
                        };

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
        private sealed class insert_into_game
        {
            public int id;
            public GameId game_id = null!;
        }
#pragma warning restore CS0649, IDE1006
    }
}
