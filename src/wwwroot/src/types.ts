export enum BoardType {
    OneEyedJack = 'oneEyedJack',
    Sequence = 'sequence',
}

export type Board = Tile[][];

export type GameId = string;

export interface Tile {
    suit: Suit;
    rank: Rank;
}

export interface Coord {
    column: number;
    row: number;
}

export enum DeckNo {
    One = 'one',
    Two = 'two',
}

export enum Suit {
    Hearts = 'hearts',
    Spades = 'spades',
    Diamonds = 'diamonds',
    Clubs = 'clubs',
}

export enum Rank {
    Ace = 'ace',
    Two = 'two',
    Three = 'three',
    Four = 'four',
    Five = 'five',
    Six = 'six',
    Seven = 'seven',
    Eight = 'eight',
    Nine = 'nine',
    Ten = 'ten',
    Jack = 'jack',
    Queen = 'queen',
    King = 'king',
}

export interface Card {
    deckNo: DeckNo;
    suit: Suit;
    rank: Rank;
}

export enum Team {
    Red = 'red',
    Green = 'green',
    Blue = 'blue',
}

export interface Chip {
    coord: Coord;
    isLocked: boolean;
    team: Team;
}

export type PlayerId = number;
export type PlayerHandle = string;

export enum PlayerType {
    User = 'user',
    Bot = 'bot',
}

export interface Player {
    id: PlayerId;
    handle: PlayerHandle;
    numberOfCards: number;
    team: Team;
    type: PlayerType;
}

export interface Sequence {
    coords: Coord[];
    team: Team;
}

export interface CardPlayed {
    updates: GameUpdated[];
}

export interface CardPlayedError {
    error: string;
}

export enum LoadGameResponseKind {
    Ok, NotFound, Error,
}

export interface OkLoadGameResponse {
    kind: LoadGameResponseKind.Ok;
    init: GameStarted;
    board: Board;
    updates: GameUpdated[];
}

export interface NotFoundLoadGameResponse {
    kind: LoadGameResponseKind.NotFound;
}

export interface ErrorLoadGameResponse {
    kind: LoadGameResponseKind.Error;
    error: Error;
}

export type LoadGameResponse =
    | OkLoadGameResponse
    | NotFoundLoadGameResponse
    | ErrorLoadGameResponse;

export interface GameStarted {
    boardType: BoardType;
    firstPlayerId: PlayerId;
    hand: Card[] | null;
    numCardsInDeck: number;
    playerHandle: PlayerHandle | null;
    playerId: PlayerId | null;
    players: Player[];
    team: Team | null;
    winCondition: number;
}

export enum GameEventKind {
    CardDiscarded = 'card-discarded',
    ChipAdded = 'chip-added',
    ChipRemoved = 'chip-removed',
    CardDrawn = 'card-drawn',
    DeckShuffled = 'deck-shuffled',
    CardDied = 'card-died',
    CardRevived = 'card-revived',
    SequenceCreated = 'sequence-created',
    TurnEnded = 'turn-ended',
    GameEnded = 'game-ended',
}

export interface CardDiscarded {
    kind: GameEventKind.CardDiscarded;
    byPlayerId: PlayerId;
    card: Card;
}

export interface ChipAdded {
    kind: GameEventKind.ChipAdded;
    coord: Coord;
    team: Team;
}

export interface ChipRemoved {
    kind: GameEventKind.ChipRemoved;
    coord: Coord;
}

export interface CardDrawn {
    kind: GameEventKind.CardDrawn;
    byPlayerId: PlayerId;
    card: Card | null;
}

export interface DeckShuffled {
    kind: GameEventKind.DeckShuffled;
    numCardsInDeck: number;
}

export interface CardDied {
    kind: GameEventKind.CardDied;
    card: Card;
}

export interface CardRevived {
    kind: GameEventKind.CardRevived;
    card: Card;
}

export interface SequenceCreated {
    kind: GameEventKind.SequenceCreated;
    sequence: Sequence;
}

export interface TurnEnded {
    kind: GameEventKind.TurnEnded;
    nextPlayerId: PlayerId | null;
}

export interface GameEnded {
    kind: GameEventKind.GameEnded;
    winnerTeam: Team;
}

export type GameEvents =
    | CardDiscarded
    | ChipAdded
    | ChipRemoved
    | CardDrawn
    | DeckShuffled
    | CardDied
    | CardRevived
    | SequenceCreated
    | TurnEnded
    | GameEnded;

export interface GameUpdated {
    gameEvents: GameEvents[];
    version: number;
}
