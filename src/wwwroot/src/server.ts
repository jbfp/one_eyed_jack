import { Game } from './Games/types';
import { CanCreateGame, CreateGameForm } from './NewGame/types';
import { BotType, CanGetBotTypes } from './NewGame/types';
import * as t from './types';

interface CreateGameResponseBody {
  gameId: t.GameId;
}

interface GetBotsResponseBody {
  botTypes: BotType[];
}

interface GetGamesResponseBody {
  games: Game[];
}

interface GetGameByIdResponseBody {
  init: t.GameStarted;
  updates: t.GameUpdated[];
}

interface CreateGameResponseError {
  error: string;
}

class Server implements CanCreateGame, CanGetBotTypes {
  constructor(readonly endpoint: URL, readonly userName: string) { }

  public async createGameAsync(form: CreateGameForm): Promise<t.GameId> {
    const url = this.buildUrl('games');

    const response = await fetch(url, {
      body: JSON.stringify(form),
      headers: {
        'Accept': 'application/json',
        'Authorization': this.userName,
        'Content-Type': 'application/json',
      },
      method: 'POST',
    });

    const responseBody: CreateGameResponseBody | CreateGameResponseError = await response.json();

    if (response.ok) {
      return (responseBody as CreateGameResponseBody).gameId;
    } else {
      throw new Error((responseBody as CreateGameResponseError).error);
    }
  }

  public async createSimulationAsync(form: any): Promise<t.GameId> {
    const url = this.buildUrl('simulations');

    const response = await fetch(url, {
      body: JSON.stringify(form),
      headers: {
        'Accept': 'application/json',
        'Authorization': this.userName,
        'Content-Type': 'application/json',
      },
      method: 'POST',
    });

    const responseBody: CreateGameResponseBody | CreateGameResponseError = await response.json();

    if (response.ok) {
      return (responseBody as CreateGameResponseBody).gameId;
    } else {
      throw new Error((responseBody as CreateGameResponseError).error);
    }
  }

  public async exchangeDeadCardAsync(
    id: t.GameId, deadCard: t.Card,
  ): Promise<t.CardPlayed | t.CardPlayedError> {
    const url = this.buildUrl('games', id, 'dead-card');

    const response = await fetch(url, {
      body: JSON.stringify({ deadCard }),
      headers: {
        'Accept': 'application/json',
        'Authorization': this.userName,
        'Content-Type': 'application/json',
      },
      method: 'POST',
    });

    return await response.json();
  }

  public async getBoardAsync(boardType: t.BoardType): Promise<t.Board> {
    const url = this.buildUrl('boards', boardType.toString());

    const response = await fetch(url, {
      headers: {
        Accept: 'application/json',
        Authorization: this.userName,
      },
      method: 'GET',
    });

    return await response.json();
  }

  public async getBotsAsync(): Promise<BotType[]> {
    const url = this.buildUrl('bots');

    const response = await fetch(url, {
      headers: {
        Accept: 'application/json',
        Authorization: this.userName,
      },
      method: 'GET',
    });

    const body: GetBotsResponseBody = await response.json();

    return body.botTypes;
  }

  public async getGamesAsync() {
    const url = this.buildUrl('games');

    const response = await fetch(url, {
      headers: {
        Accept: 'application/json',
        Authorization: this.userName,
      },
      method: 'GET',
    });

    const body: GetGamesResponseBody = await response.json();

    return body.games;
  }

  public async getGameByIdAsync(id: t.GameId): Promise<t.LoadGameResponse> {
    const url = `${this.buildUrl('games', id)}`;

    try {
      const response = await fetch(url, {
        headers: {
          Accept: 'application/json',
          Authorization: this.userName,
        },
        method: 'GET',
      });

      if (response.status === 404) {
        return { kind: t.LoadGameResponseKind.NotFound };
      }

      const body: GetGameByIdResponseBody = await response.json();
      const init = body.init;
      const board = await this.getBoardAsync(init.boardType);
      const updates = body.updates;
      return { kind: t.LoadGameResponseKind.Ok, init, board, updates };
    } catch (error) {
      return { kind: t.LoadGameResponseKind.Error, error };
    }
  }

  public async getSimulationsAsync() {
    const url = this.buildUrl('simulations');

    const response = await fetch(url, {
      headers: {
        Accept: 'application/json',
        Authorization: this.userName,
      },
      method: 'GET',
    });

    const body = await response.json();
    const gameIds = body as t.GameId[];
    return gameIds;
  }

  public async playCardAsync(
    id: t.GameId, card: t.Card, coord: t.Coord,
  ): Promise<t.CardPlayed | t.CardPlayedError> {
    const url = this.buildUrl('games', id);

    const response = await fetch(url, {
      body: JSON.stringify({
        card, ...coord,
      }),
      headers: {
        'Accept': 'application/json',
        'Authorization': this.userName,
        'Content-Type': 'application/json',
      },
      method: 'POST',
    });

    return await response.json();
  }

  private buildUrl(...components: string[]): string {
    return [this.endpoint, ...components].join('/');
  }
}

export default Server;
