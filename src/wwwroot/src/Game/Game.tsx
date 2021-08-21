import * as SignalR from '@microsoft/signalr';
import React, { useCallback, useContext, useEffect, useRef, useState } from 'react';
import { ServerContext } from '../contexts';
import * as t from '../types';
import GameView from './GameView';
import { GameState, reducer } from './reducer';

interface GameProps {
    id: t.GameId;
    init: GameState;
    onRequestReload: () => Promise<void>;
}

const HUB_URL = '/game-hub';

const CONNECTION_OPTIONS: SignalR.IHttpConnectionOptions = {
    logMessageContent: true,
    skipNegotiation: true,
    transport: SignalR.HttpTransportType.WebSockets,
};

async function startAsync(connection: SignalR.HubConnection, callback: () => Promise<void>) {
    console.log('Trying to connect...');

    try {
        await connection.start();

        try {
            await callback();
        } catch (err) {
            console.error('Error executing SignalR connection callback', err);
        }
    } catch (err) {
        setTimeout(async () => await startAsync(connection, callback), 5000);
    }
}

export default function Game(props: GameProps) {
    const { init } = props;
    const gameId = props.id;
    const playerId = init.playerId;
    const [updates, setUpdates] = useState<t.GameUpdated[]>([]);
    const [state, setState] = useState<GameState>(init);
    const [selectedCard, setSelectedCard] = useState<t.Card | null>(null);
    const connection = useRef<SignalR.HubConnection>();
    const showNotifications = useRef<boolean | null>(null);
    const server = useContext(ServerContext);

    const handleUpdatesAsync = useCallback((update: t.GameUpdated) => {
        setUpdates((currentUpdates) => [...currentUpdates, update]);
    }, []);

    useEffect(() => {
        const newUpdates = updates.filter((update) => update.version >= init.version);
        const finalState = newUpdates.reduce(reducer, init);
        setState(finalState);
    }, [init, updates]);

    useEffect(() => {
        const conn = new SignalR.HubConnectionBuilder()
            .withUrl(HUB_URL, { ...CONNECTION_OPTIONS })
            .configureLogging(SignalR.LogLevel.Information)
            .build();

        conn.on('UpdateGame', (msg) => handleUpdatesAsync(msg));

        async function handleConnectionStarted() {
            if (conn.state === SignalR.HubConnectionState.Connected) {
                if (playerId) {
                    await conn.invoke('Identify', playerId);
                } else {
                    await conn.invoke('Subscribe', gameId);
                }
            }

            setTimeout(handleConnectionStarted, 5000);
        }

        conn.onclose(async (error) => {
            if (error) {
                await startAsync(conn, handleConnectionStarted);
            } else {
                // Connection closed gracefully.
            }
        });

        startAsync(conn, handleConnectionStarted);

        connection.current = conn;

        return () => {
            conn.stop();
        };
    }, [gameId, handleUpdatesAsync, playerId]);

    useEffect(() => {
        if (showNotifications.current === null && typeof Notification !== 'undefined') {
            const handlePermissionCallback = (result: NotificationPermission) => {
                showNotifications.current = result === 'granted';
            };

            const promise = Notification.requestPermission();

            if (typeof promise === 'undefined') {
                // Safari on MacOS only supports the old requestPermission function.
                Notification.requestPermission(handlePermissionCallback);
            } else {
                promise.then(handlePermissionCallback);
            }
        }
    }, []);

    const handleCardClick = useCallback((card: t.Card) => {
        setSelectedCard((selectedCard) => {
            if (selectedCard === card) {
                return null;
            } else {
                return card;
            }
        });
    }, []);

    const handleCoordClickAsync = useCallback(async (coord: t.Coord) => {
        if (selectedCard) {
            try {
                const result = await server.playCardAsync(gameId, selectedCard, coord);

                if ((result as t.CardPlayedError).error) {
                    console.warn((result as t.CardPlayedError).error);
                } else {
                    const updates = (result as t.CardPlayed).updates;
                    console.assert(updates.length === 1);
                    setSelectedCard(null);
                    await handleUpdatesAsync(updates[0]);
                }
            } catch (err) {
                alert(err.toString());
            }
        }
    }, [gameId, handleUpdatesAsync, selectedCard, server]);

    const handleExchangeDeadCardClick = useCallback(async () => {
        if (selectedCard) {
            try {
                const result = await server.exchangeDeadCardAsync(gameId, selectedCard);

                if ((result as t.CardPlayedError).error) {
                    console.warn((result as t.CardPlayedError).error);
                } else {
                    const updates = (result as t.CardPlayed).updates;
                    console.assert(updates.length === 1);
                    setSelectedCard(null);
                    handleUpdatesAsync(updates[0]);
                }
            } catch (err) {
                alert(err.toString());
            }
        }
    }, [gameId, handleUpdatesAsync, selectedCard, server]);

    return (
        <GameView
            game={state}
            selectedCard={selectedCard}
            onCardClick={handleCardClick}
            onCoordClick={handleCoordClickAsync}
            onExchangeDeadCardClick={handleExchangeDeadCardClick}
        />
    );
}
