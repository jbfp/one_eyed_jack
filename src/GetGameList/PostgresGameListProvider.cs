using Dapper;
using Microsoft.Extensions.Options;
using Sequence.Postgres;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sequence.GetGameList
{
    public sealed class PostgresGameListProvider : IGameListProvider
    {
        private readonly NpgsqlConnectionFactory _connectionFactory;

        public PostgresGameListProvider(NpgsqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<GameList> GetGamesForPlayerAsync(PlayerHandle player, CancellationToken cancellationToken)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            cancellationToken.ThrowIfCancellationRequested();

            ImmutableList<GameListItem> gameListItems;

            using (var connection = await _connectionFactory.CreateAndOpenAsync(cancellationToken))
            {
                var command = new CommandDefinition(
                    commandText: "SELECT * FROM public.get_game_list_for_player(@player);",
                    parameters: new { player },
                    cancellationToken: cancellationToken
                );

                var rows = await connection.QueryAsync<get_game_list_for_player>(command);

                gameListItems = rows
                    .Select(get_game_list_for_player.ToGameListItem)
                    .ToImmutableList();
            }

            return new GameList(gameListItems);
        }

#pragma warning disable CS0649
        private sealed class get_game_list_for_player
        {
            public GameId game_id;
            public PlayerHandle next_player_id;
            public string[] opponents;
            public DateTimeOffset? last_move_at;

            public static GameListItem ToGameListItem(get_game_list_for_player row)
            {
                return new GameListItem(
                    row.game_id,
                    row.next_player_id,
                    row.opponents.Select(o => new PlayerHandle(o)).ToImmutableList(),
                    row.last_move_at
                );
            }
        }
#pragma warning restore CS0649
    }
}
