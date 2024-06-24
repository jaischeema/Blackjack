using System.Collections.Generic;
using System.Linq;

namespace BlackJack.Models
{
    public class Game
    {
        private Deck m_Deck;
        private List<Card> m_DealerHand;
        private bool m_IsDealerHandOpen = false;
        private int m_CurrentPlayerIndex = -1;

        public readonly List<List<Card>> PlayerHands;
        public int CurrentPlayerIndex => m_CurrentPlayerIndex;
        public bool IsFinished => m_IsDealerHandOpen;

        public List<Card> VisibleDealerCards
        {
            get
            {
                if (m_IsDealerHandOpen) return m_DealerHand;

                if (m_DealerHand.Count <= 1) return new List<Card>();

                var topCard = m_DealerHand.Last();
                return new List<Card> { topCard };
            }
        }

        public Game(int playerCount)
        {
            m_Deck = new Deck();
            PlayerHands = Enumerable.Range(0, playerCount).Select(a => new List<Card>()).ToList();
            m_DealerHand = new List<Card>();
        }

        private void DealInitialCards()
        {
            m_DealerHand.AddRange(m_Deck.DealCards(1));
            PlayerHands.ForEach(a => { a.AddRange(m_Deck.DealCards(1)); });

            m_DealerHand.AddRange(m_Deck.DealCards(1));
            PlayerHands.ForEach(a => { a.AddRange(m_Deck.DealCards(1)); });
        }

        public void Start()
        {
            DealInitialCards();
            m_CurrentPlayerIndex = 0;
        }

        public void Update(PlayerAction action)
        {
            if(IsFinished) return;
            
            if (action == PlayerAction.Hit)
            {
                PlayerHands[CurrentPlayerIndex].AddRange(m_Deck.DealCards(1));
                if (SumOfValues(PlayerHands[CurrentPlayerIndex], true) >= 21)
                {
                    m_CurrentPlayerIndex += 1;
                }
            }
            else
            {
                m_CurrentPlayerIndex += 1;
            }

            if (m_CurrentPlayerIndex != PlayerHands.Count) return;

            m_IsDealerHandOpen = true;
            var shouldDealDealerCard = true;
            while(shouldDealDealerCard)
            {
                var leastDealerHand = SumOfValues(m_DealerHand, true);
                var highestDealerHand = SumOfValues(m_DealerHand, false);
                if (leastDealerHand is > 16 and <= 21 || highestDealerHand is > 16 and <= 21)
                {
                    shouldDealDealerCard = false;
                }
                else
                {
                    m_DealerHand.AddRange(m_Deck.DealCards(1));
                    if (SumOfValues(m_DealerHand, findLeast: true) > 21)
                    {
                        shouldDealDealerCard = false;
                    }
                }
            }
        }

        public int Reward(int playerIndex)
        {
            if (IsFinished) return 0;
            var playerValue = SumOfValues(PlayerHands[playerIndex], findLeast: false);
            var dealerValue = SumOfValues(m_DealerHand, findLeast: false);

            // Player gone over, saved only by dealer chance
            if (playerValue > 21 && dealerValue > 21) return 0;

            // Player lost by going over or value below dealer
            if (playerValue > 21 || playerValue < dealerValue) return -10;

            // Player won or tied by getting to 21 or won
            if (playerValue == 21 || playerValue > dealerValue) return 10;

            return 5;
        }

        public static int SumOfValues(List<Card> cards, bool findLeast)
        {
            return cards.Select(a => a.Value(findLeast)).Sum();
        }

        public List<PlayerAction> GetPossibleActions() => new() { PlayerAction.Hit, PlayerAction.Stand };

        public PlayerAction GetActionFromIndex(int index)
        {
            return GetPossibleActions()[index];
        }

        public enum PlayerAction
        {
            Hit,
            Stand
        }
    }
}
