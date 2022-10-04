using System;

namespace Cardgame.Blackjack {
    class MainRunner {
        static void Main(string[] args)
        {
            var CLI = new CommandLineInterface();
            CLI.Run();
        }
    }
}