using System;
using System.Linq;

namespace Cardgame.Blackjack {
    public class CommandLineInterface {
        World World { get; }
        // this interface is simple: we only have one player playing in one game
        // so might as well have those as accessible variables
        Game Game { get; set; }
        Player Player { get; set; }
        public CommandLineInterface () {
            World = new World();
            Console.WriteLine("Welcome to Blackjack!");
        }
        public void Run () {
            WorldSetup();
        }
        void WorldSetup () {
            Console.WriteLine("What's your name?");
            var playerName = Console.ReadLine();
            Console.WriteLine("How much money do you have?");
            var success = false;
            double playerMoney = 0.0;
            while (!success) {
                Console.WriteLine("Please write a number.");
                success = Double.TryParse(Console.ReadLine(), out playerMoney);
            }
            Player = World.AddPlayer(playerName, playerMoney);
            Game = World.AddGame("cli-game");
            PrintWorld();
            GameSetup();
        }
        void GameSetup () {
            Console.WriteLine("Your game has {0} player positions.", Game.PlayerPositions.Count());
            Console.WriteLine("What do you want to do?");
            var command = Console.ReadLine();
            if (command == "quit") {
                // do nothing
            } else if (command.StartsWith("bet") && command.Contains(" ")) {
                Game.AddPlayerPos(Player, Double.Parse(command.Split(" ")[1]));
                GameSetup();
            } else if (command == "start") {
                Game.StartGame();
                Play();
            }
            else {
                Console.WriteLine("I didn't understand that. (Accepted commands at this stage are \"quit\", \"bet {bet size}\", and \"start\".)");
                GameSetup();
            }
        }
        void Play () {
            if (Game.GetPhase() != GamePhase.Play) {
                Finish();
                return;
            }
            var pos = Game.GetCurrentPosition();
            PrintGame();
            Console.WriteLine("What does the current player position want to do?");
            var command = Console.ReadLine();
            if (command == "quit") {
                return;
            } else if (command == "hit") {
                pos.Hit();
            } else if (command == "stand") {
                pos.Stand();
            } else if (command == "double") {
                pos.Double();
                PrintGame();
            } else if (command == "split" && pos.CanSplit()) {
                pos.Split();
            } else if (command == "surrender" && pos.CanSurrender()) {
                pos.Surrender();
            } else {
                Console.WriteLine("I didn't recognize that command. (Accepted commands at this stage are \"quit\", \"hit\", \"stand\", \"double\", and \"split\" and \"surrender\" if those are possible.)");
            }

            if (pos.IsFinished) {
                Game.EndCurrentTurn();
            }
            if (pos.IsBust) {
                var cards = pos.Hand.Cards;
                Console.WriteLine("You drew a {0} and bust!", cards[cards.Count() - 1]);
            }
            Play();
        }
        void Finish () {
            Console.WriteLine("Everyone has played! Now it is the dealer's turn!");
            PrintGame();
            Console.WriteLine();
            Console.WriteLine("Therefore, the final results are:");
            foreach (var p in Game.PlayerPositions) {
                Console.WriteLine("{0} {1} (won {2})", p.Player.Name, p.Hand, p.Winnings(Game.Dealer));
            }
            PrintWorld();
            Console.WriteLine("Do you wish to play another game?");
            if (YesNo()) {
                GameSetup();
            } else {
                Console.WriteLine("Understandable! Have a nice day!");
            }
        }
        bool YesNo() {
            var answer = Console.ReadLine();
            if (answer.Length > 0){
                var c = Char.ToLower(answer[0]);
                if (c == 'y') {
                    return true;
                } else if (c == 'n') {
                    return false;
                }
            }
            Console.WriteLine("Please answer yes or no.");
            return YesNo();
        }
        void PrintWorld () {
            Console.WriteLine("The state of everyone's financials is:");
            Console.WriteLine("{0}: {1}", Player.Name, Player.Money);
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
                string beginning = "";
                if (Game.GetPhase() == GamePhase.Play && p == Game.GetCurrentPosition()) {
                    beginning = "Currently playing: ";
                }
                Console.WriteLine("{0}{1} {2} ({3}, bet {4})", beginning, p.Player.Name, p.Hand, FormattedScore(p), p.Bet);
            }
        }
    }
}