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
    public class BasePosition {

        public Hand Hand { get; }
        public bool IsFinished { get; set; }
        public bool IsBust { get; set; }
        public Base.Shoe Shoe { get; }

        public BasePosition (Base.Shoe shoe) {
            Shoe = shoe;
            Hand = new Hand();
            IsFinished = false;
            IsBust = false;
        }
        public void Draw () {
            Hand.Add(Shoe.Draw());
        }
        public virtual void CheckIfBust () {
            if (Hand.Score() > 21) {
                IsFinished = true;
                IsBust = true;
            }
        }
        public void Stand () {
            IsFinished = true;
        }
        public void Hit () {
            Draw();
            CheckIfBust();
        }
    }
    public class PlayerPosition : BasePosition {

        public double Bet { get; set; }
        public bool BlackjackDoesntCount = false;

        public PlayerPosition (double bet, Base.Shoe shoe) : base (shoe) {
            Bet = bet;
            DrawToTwo();
        }
        public PlayerPosition (double bet, Base.Shoe shoe, Base.Card card) : base (shoe) { // Constructor for when we're splitting, ie creating a new hand with one already-known card
            Hand.Add(card);
            Bet = bet;
            DrawToTwo();
            if (card.Value == 1) {
                BlackjackDoesntCount = true;
            }
        }
        void DrawToTwo () {
            while (Hand.Count() < 2) {
                Draw();
            }
            if (Hand.IsBlackjack()) {
                IsFinished = true;
            }
        }
        public override void CheckIfBust () {
            base.CheckIfBust();
            if (IsBust) {
                Bet = 0;
            }
        }
        void Double () {
            Bet *= 2.0;
            Hit();
            IsFinished = true;
        }
        bool CanSplit() {
            return Hand.Count() == 2 && Hand.Cards[0].Value == Hand.Cards[1].Value;
        }
        (PlayerPosition left, PlayerPosition right) Split () {
            if (!CanSplit()) {
                throw new InvalidOperationException("Can only split when you have exactly 2 cards which have the same value");
            }
            return (new PlayerPosition (Bet, Shoe, Hand.Cards[0]), new PlayerPosition (Bet, Shoe, Hand.Cards[1]));

        }
        double Winnings (Dealer dealer) {
            // A blackjack counts as a higher score than a non-blackjack 21, so for this comparison let's count a blackjack as 22
            var dealerScore = dealer.Hand.IsBlackjack() ? 22 : dealer.Hand.Score();
            var myScore = Hand.IsBlackjack() && !BlackjackDoesntCount ? 22 : Hand.Score();
            if (dealer.IsBust || dealerScore < Hand.Score()) { // if the player is bust, the bet is 0, so we don't have to check for that case
                if (myScore == 22) {
                    return Bet + 1.5 * Bet;
                } else {
                    return Bet + Bet;
                }
            } else {
                return 0.0;
            }
        }
    }
    public class Dealer : BasePosition {
        public Dealer (Base.Shoe shoe) : base (shoe) {
            Draw();
        }
        public void Run () {
            while (Hand.Score() < 17) { // if the score is exactly 17, and the hand is soft, what happens in that case would ideally be configurable
                Hit();
            }
            Stand();
        }
    }
    public class Game {
        public Base.Shoe Shoe { get; }
        public Dealer Dealer { get; }
        public List<PlayerPosition> PlayerPositions { get; }
        public Game () {
            Shoe = new Base.Shoe(6);
            Dealer = new Dealer(Shoe);
            PlayerPositions = new List<PlayerPosition>();
        }
        public void AddPlayerPos (double bet) {
            PlayerPositions.Add(new PlayerPosition(bet, Shoe));
        }
    }
}