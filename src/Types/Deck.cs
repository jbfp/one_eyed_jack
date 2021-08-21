using System.Collections;
using System.Collections.Immutable;

namespace Sequence
{
    internal sealed class Deck : IEnumerable<Card>
    {
        private static readonly ImmutableArray<DeckNo> _deckNos = Enum
            .GetValues(typeof(DeckNo))
            .Cast<DeckNo>()
            .ToImmutableArray();

        private static readonly ImmutableArray<Suit> _suits = Enum
            .GetValues(typeof(Suit))
            .Cast<Suit>()
            .ToImmutableArray();

        private static readonly ImmutableArray<Rank> _ranks = Enum
            .GetValues(typeof(Rank))
            .Cast<Rank>()
            .ToImmutableArray();

        private static readonly ImmutableArray<Card> _cards;

        static Deck()
        {
            var numCards = _deckNos.Length * _suits.Length * _ranks.Length;
            var builder = ImmutableArray.CreateBuilder<Card>(numCards);

            foreach (var deckNo in _deckNos)
            {
                foreach (var suit in _suits)
                {
                    foreach (var rank in _ranks)
                    {
                        builder.Add(new Card(deckNo, suit, rank));
                    }
                }
            }

            _cards = builder.ToImmutable();
        }

        private readonly Random _rng;
        private readonly List<Card> _deck;
        private readonly int _numPlayers;

        public Deck(Seed seed, int numPlayers)
        {
            _rng = seed.ToRandom();
            _deck = _cards.ToList();
            _numPlayers = numPlayers;

            // Shuffle with Fisher-Yates algorithm.
            var n = _deck.Count;
            for (int i = 0; i < n; i++)
            {
                int r = i + _rng.Next(n - i);
                var t = _deck[r];
                _deck[r] = _deck[i];
                _deck[i] = t;
            }
        }

        public static IImmutableList<Card> Shuffle(IImmutableList<Card> cards, Seed seed)
        {
            if (cards.Count == 0)
            {
                return cards;
            }

            var deck = cards.ToList();
            var rng = seed.ToRandom();

            // Shuffle with Fisher-Yates algorithm.
            var n = deck.Count;

            for (int i = 0; i < n; i++)
            {
                int r = i + rng.Next(n - i);
                var t = deck[r];
                deck[r] = deck[i];
                deck[i] = t;
            }

            return deck.ToImmutableList();
        }

        public IImmutableList<IImmutableList<Card>> DealHands()
        {
            int GetNumCards() => _numPlayers switch
            {
                2 => 7,
                3 => 6,
                4 or 6 => 5,
                _ => throw new NotSupportedException(),
            };

            IImmutableList<Card> DealHand(int n)
            {
                var builder = ImmutableList.CreateBuilder<Card>();

                for (int i = 0; i < n; i++)
                {
                    var idx = _deck.Count - 1;
                    var card = _deck[idx];
                    builder.Add(card);
                    _deck.RemoveAt(idx);
                }

                return builder.ToImmutable();
            }

            int numCards = GetNumCards();

            return Enumerable
                .Range(0, _numPlayers)
                .Select(_ => DealHand(numCards))
                .ToImmutableList();
        }

        public IEnumerator<Card> GetEnumerator()
        {
            return ((IEnumerable<Card>)_deck).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Card>)_deck).GetEnumerator();
        }
    }
}
