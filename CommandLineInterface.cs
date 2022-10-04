using System;
using System.Linq;

namespace Cardgame.Blackjack {
    public class CommandLineInterface {
        Game Game { get; }
        public CommandLineInterface () {
            Game = new Game();
            Console.WriteLine("Welcome to Blackjack!");
        }
        public void Run () {
            Setup();
        }
        void Setup () {
            Console.WriteLine("Your game has {0} player positions.", Game.PlayerPositions.Count());
            Console.WriteLine("What do you want to do?");
            var command = Console.ReadLine();
            if (command == "quit") {
                // do nothing
            } else if (command.StartsWith("add") && command.Contains(" ")) {
                Game.AddPlayerPos(Double.Parse(command.Split(" ")[1]));
                Setup();
            } else if (command == "start") {
                Play();
            }
            else {
                Console.WriteLine("I didn't understand that. (Accepted commands at this stage are \"quit\", \"add {bet size}\", and \"start\".)");
                Setup();
            }
        }
        void Play () {
            var player = Game.GetCurrentPlayer();
            if (player == null) {
                Finish();
                return;
            }
            PrintGame();
            Console.WriteLine("What does the current player want to do?");
            var command = Console.ReadLine();
            if (command == "quit") {
                return;
            } else if (command == "hit") {
                player.Hit();
                if (player.IsFinished) {
                    var cards = player.Hand.Cards;
                    Console.WriteLine("You drew a {0} and bust!", cards[cards.Count() - 1]);
                }
            } else if (command == "stand") {
                player.Stand();
            } else if (command == "double") {
                player.Double();
                PrintGame();
            } else if (command == "split" && player.CanSplit()) {
                player.Split();
            } else {
                Console.WriteLine("I didn't recognize that command. (Accepted commands at this stage are \"quit\", \"hit\", \"stand\", \"double\", and \"split\" if splitting is possible.)");
            }

            if (player.IsFinished) {
                Game.NextPlayer();
            }
            Play();
        }
        void Finish () {
            Console.WriteLine("Everyone has played! Now it is the dealer's turn!");
            Game.Dealer.Run();
            PrintGame();
            Console.WriteLine("Therefore, the final results are:");
            foreach (var p in Game.PlayerPositions) {
                Console.WriteLine("Player: {0} (won {1})", p.Hand, p.Winnings(Game.Dealer));
            }
        }
        static string FormattedScore(BasePosition pos) {
            if (pos.IsBust) {
                return "bust";
            } else if (pos.Hand.IsBlackjack() && !pos.BlackjackDoesntCount) {
                return "blackjack";
            } else {
                return String.Format("score {0}", pos.Hand.Score());
            }
        }
        void PrintGame () {
            Console.WriteLine("Dealer: {0} ({1})", Game.Dealer.Hand, FormattedScore(Game.Dealer));
            foreach (var p in Game.PlayerPositions) {
                var descriptor = p == Game.GetCurrentPlayer() ? "Current player" : "Player";
                Console.WriteLine("{0}: {1} ({2}, bet {3})", descriptor, p.Hand, FormattedScore(p), p.Bet);
            }
        }
    }
}