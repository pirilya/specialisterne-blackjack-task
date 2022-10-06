using System;
using System.Linq;
using System.Collections.Generic;

namespace Cardgame.Blackjack {
    public class BasePosition {

        public Hand Hand { get; }
        public bool IsFinished { get; set; }
        public bool IsBust { get; set; }
        Base.Shoe Shoe { get; }
        public bool BlackjackDoesntCount = false;

        public BasePosition (Base.Shoe shoe) {
            Shoe = shoe;
            Hand = new Hand();
            IsFinished = false;
            IsBust = false;
        }
        public Base.Shoe GetShoe() {
            return Shoe;
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
        public Player Player { get; }

        public PlayerPosition (Player player, double bet, Base.Shoe shoe) : base (shoe) {
            Player = player;
            Bet = bet;
            DrawToTwo();
        }
        public PlayerPosition (Player player, double bet, Base.Shoe shoe, Base.Card card) : base (shoe) { // Constructor for when we're splitting, ie creating a new hand with one already-known card
            Hand.Add(card);
            Player = player;
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
        public void Double () {
            Bet *= 2.0;
            Hit();
            IsFinished = true;
        }
        public bool CanSplit() {
            return Hand.Count() == 2 && Hand.Cards[0].Value == Hand.Cards[1].Value;
        }
        public (PlayerPosition left, PlayerPosition right) Split () {
            if (!CanSplit()) {
                throw new InvalidOperationException("Can only split when you have exactly 2 cards which have the same value");
            }
            return (new PlayerPosition (Player, Bet, GetShoe(), Hand.Cards[0]), new PlayerPosition (Player, Bet, GetShoe(), Hand.Cards[1]));

        }
        public bool CanSurrender () {
            return Hand.Count() == 2;
        }
        public void Surrender () {
            if (!CanSurrender()) {
                throw new InvalidOperationException("Can only surrender on your first turn");
            }
            Player.Money += 0.5 * Bet;
            Bet = 0.0;
            IsFinished = true;
        }
        public double Winnings (Dealer dealer) {
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
}