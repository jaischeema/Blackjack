using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace BlackJack.Models
{
    public class Deck
    {
        public readonly List<Card> Cards;

        public Deck()
        {
            var list = Enumerable.Range(0, 52).ToList();
            var n = 52;
            while (n > 1)
            {
                n--;
                var k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }

            Cards = list.Select(value => new Card((byte)value)).ToList();
        }

        public List<Card> DealCards(int count)
        {
            Cards.Reverse();
            var cardsDealt = Cards.PopRange(0, count);
            Cards.Reverse();
            return cardsDealt;
        }
    }

    internal static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random s_Local;
        public static Random ThisThreadsRandom =>
            s_Local ??= new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId));
    }

    public struct Card
    {
        private static Rank GetRank(byte value)
        {
            return (Rank)(value % 13 + 1);
        }

        public static Suite GetSuite(byte value)
        {
            return (Suite)(value / 13);
        }

        public readonly byte globalIndex;

        public Card(byte globalIndex) => this.globalIndex = globalIndex;

        public Card(Suite suite, Rank rank)
        {
            globalIndex = (byte)((int)suite * 13 + (int)rank - 1);
        }

        public Rank Rank => GetRank(globalIndex);
        public Suite Suite => GetSuite(globalIndex);

        public int Value(bool useLeast)
        {
            return Rank switch
            {
                Rank.Ace => useLeast ? 1 : 10,
                Rank.Two => 2,
                Rank.Three => 3,
                Rank.Four => 4,
                Rank.Five => 5,
                Rank.Six => 6,
                Rank.Seven => 7,
                Rank.Eight => 8,
                Rank.Nine => 9,
                Rank.Ten or Rank.Jack or Rank.Queen or Rank.King => 10,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override bool Equals(object obj)
        {
            return obj is Card card && card.globalIndex == globalIndex;
        }

        public override int GetHashCode()
        {
            return globalIndex.GetHashCode();
        }

        public override string ToString()
        {
            return $"Card: {Suite}-{Rank.GetDescription()}";
        }

        public string GetSpriteName()
        {
            var suiteName = Suite switch
            {
                Suite.Spades => "spade",
                Suite.Clubs => "club",
                Suite.Diamonds => "diamond",
                Suite.Hearts => "heart",
                _ => throw new ArgumentOutOfRangeException()
            };

            var rankIntValue = (int)Rank;

            return $"{rankIntValue}_{suiteName}";
        }
    }

    public enum Suite
    {
        Spades = 0,
        Clubs = 1,
        Diamonds = 2,
        Hearts = 3,
    }

    public enum Rank
    {
        [Description("Ace")] Ace = 1,
        [Description("2")] Two = 2,
        [Description("3")] Three = 3,
        [Description("4")] Four = 4,
        [Description("5")] Five = 5,
        [Description("6")] Six = 6,
        [Description("7")] Seven = 7,
        [Description("8")] Eight = 8,
        [Description("9")] Nine = 9,
        [Description("10")] Ten = 10,
        [Description("Jack")] Jack = 11,
        [Description("Queen")] Queen = 12,
        [Description("King")] King = 13,
    }

    internal static class CardExtensions
    {
        public static string GetDescription(this Rank rank)
        {
            var fieldInfo = rank.GetType().GetField(rank.ToString());
            var attributes =
                fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return attributes[0].Description;
        }

        public static List<T> PopRange<T>(this List<T> list, int index, int count)
        {
            var items = list.GetRange(index, count);
            list.RemoveRange(index, count);
            return items;
        }
    }
}
