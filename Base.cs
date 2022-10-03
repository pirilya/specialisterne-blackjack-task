using System;
using System.Collections.Generic;
using System.Linq;

namespace Cardgame.Base {
    public enum Suit {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }

    public struct Card {
        public int Value { get; }
        public Suit Suit { get; }
        public Card(int value, Suit suit) {
            if (value <= 0 || value > 13) {
                throw new ArgumentException(String.Format("{0} is not a valid value for a playing card", value));
            }
            Value = value;
            Suit = suit;
        }
        public override string ToString() {
            return String.Format("{0} of {1}", Value, Suit);
        }
    }
    public class Hand {
        public List<Card> Cards { get; }
        public Hand () {
            Cards = new List<Card>();
        }
        public virtual void Add (Card card) {
            Cards.Add(card);
        }
        public int Count () {
            return Cards.Count();
        }
        public override string ToString() {
            return String.Format("<{0}>", String.Join(", ", Cards));
        }
    }
    public class Shoe {
        public List<Card> Cards { get; }
        Random RNG;
        public Shoe (int numDecks = 1) {
            Cards = new List<Card>();
            RNG = new Random();
            for (var i = 0; i < numDecks; i++) {
                AddDeck();
            }
        }
        public void AddDeck () {
            for (var v = 1; v <= 13; v++) {
                for (var s = 0; s < 4; s++) {
                    Cards.Add(new Card(v, (Suit)s));
                }
            }
        }
        public void Shuffle () {
            for (var i = 0; i < Cards.Count(); i++) {
                var j = RNG.Next(i+1);
                var value = Cards[j];
                Cards[j] = Cards[i];
                Cards[i] = value;
            }
        }
        public Card Draw () {
            var n = Cards.Count() - 1; // removing list members is faster if you remove the one at the end, i think
            var drawn = Cards[n];
            Cards.RemoveAt(n);
            return drawn;
        }
    }
}