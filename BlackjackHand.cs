using System;
using System.Linq;
using System.Collections.Generic;

namespace Cardgame.Blackjack {
    public class Hand : Base.Hand {

        int SimpleScore { get; set; }
        bool HasAce { get; set; }

        public Hand () : base () {
            SimpleScore = 0;
            HasAce = false;
        }
        public bool IsSoft () {
            return (SimpleScore <= 11 && HasAce);
        }
        public int Score () {
            return IsSoft() ? SimpleScore + 10 : SimpleScore;
        }
        public bool IsBlackjack () {
            return Score() == 21 && Cards.Count() == 2;
        }
        public override void Add (Base.Card card) {
            base.Add(card);
            SimpleScore += card.Value > 10 ? 10 : card.Value;
            if (card.Value == 1) {
                HasAce = true;
            }
        }
    }
}