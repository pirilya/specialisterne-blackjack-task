using System;
using System.Linq;
using System.Collections.Generic;

namespace Cardgame.Blackjack {
    public enum GamePhase {
        Setup,
        Play,
        Ended
    }
    public class Game {
        public Base.Shoe Shoe { get; }
        public Dealer Dealer { get; }
        public List<PlayerPosition> PlayerPositions { get; }
        int CurrentPlayerIndex { get; set; }
        GamePhase Phase { get; set; }
        public Game () {
            Shoe = new Base.Shoe(6);
            Dealer = new Dealer(Shoe);
            PlayerPositions = new List<PlayerPosition>();
            Phase = GamePhase.Setup;
            CurrentPlayerIndex = 0;
        }
        public void AddPlayerPos (double bet) {
            if (Phase != GamePhase.Setup) {
                throw new InvalidOperationException("Can only add players during the setup phase");
            }
            PlayerPositions.Add(new PlayerPosition(bet, Shoe));
        }
        public GamePhase GetPhase () {
            return Phase;
        }
        public void StartGame () {
            Phase = GamePhase.Play;
            CurrentPlayerIndex = 0;
        }
        public PlayerPosition? GetCurrentPlayer () {
            if (Phase == GamePhase.Play) {
                return PlayerPositions[CurrentPlayerIndex];
            }
            return null;
        }
        public void EndCurrentTurn () {
            if (Phase != GamePhase.Play) {
                throw new InvalidOperationException("Can't end the player's turn if it isn't currently anyone's turn");
            }
            CurrentPlayerIndex++;
            if (CurrentPlayerIndex >= PlayerPositions.Count()) {
                Phase = GamePhase.Ended;
                Dealer.Run();
            }
        }
    }
}