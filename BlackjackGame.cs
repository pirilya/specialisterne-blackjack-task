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
        Base.Shoe Shoe { get; }
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
        public void AddPlayerPos (Player player, double bet) {
            if (Phase != GamePhase.Setup) {
                throw new InvalidOperationException("Can only add players during the setup phase");
            }
            player.Money -= bet;
            PlayerPositions.Add(new PlayerPosition(player, bet, Shoe));
        }
        public GamePhase GetPhase () {
            return Phase;
        }
        public void StartGame () {
            Phase = GamePhase.Play;
            CurrentPlayerIndex = 0;
        }
        public PlayerPosition GetCurrentPosition () {
            if (Phase != GamePhase.Play) {
                throw new InvalidOperationException("Can only get the current position during the play phase");
            }
            return PlayerPositions[CurrentPlayerIndex];
        }
        public void EndCurrentTurn () {
            if (Phase != GamePhase.Play) {
                throw new InvalidOperationException("Can't end the player's turn if it isn't currently anyone's turn");
            }
            CurrentPlayerIndex++;
            if (CurrentPlayerIndex >= PlayerPositions.Count()) {
                Phase = GamePhase.Ended;
                Dealer.Run();
                foreach (var p in PlayerPositions) {
                    p.Player.Money += p.Winnings(Dealer);
                }
            }
        }
    }
}