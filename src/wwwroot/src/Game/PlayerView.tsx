import React, { useCallback } from 'react';
import { Card, Team } from '../types';
import Hand from './Hand';

interface PlayerViewProps {
    deadCards: Map<string, Card>;
    hand: Card[];
    hasExchangedDeadCard: boolean;
    isCurrentPlayer: boolean;
    selectedCardKey: string | null;
    team: Team;
    onCardClick: (card: Card) => void;
    onExchangeDeadCardClick: () => void;
}

export default function PlayerView(props: PlayerViewProps) {
    const { deadCards, hand, hasExchangedDeadCard, selectedCardKey, team } = props;
    const { onCardClick, onExchangeDeadCardClick } = props;
    const isDead = selectedCardKey && deadCards.has(selectedCardKey);

    const handleExchangeDeadCardClick = useCallback((event: React.MouseEvent) => {
        event.preventDefault();
        onExchangeDeadCardClick();
    }, [onExchangeDeadCardClick]);

    const $exchangeDeadCard = isDead && !hasExchangedDeadCard ? (
        <button className="anchor exchange-dead-card-btn" type="button" onClick={handleExchangeDeadCardClick}>
            &gt;&nbsp;Exchange dead card&nbsp;&lt;
        </button>
    ) : null;

    return (
        <div className="player" data-team={team}>
            <Hand
                cards={hand}
                deadCards={deadCards}
                onCardClick={onCardClick}
                selectedCardKey={selectedCardKey}
            />

            {$exchangeDeadCard}
        </div >
    );
}
