using System;
using System.Threading.Tasks;

namespace Cardgame.Blackjack {
    class MainRunner {
        static async Task Main(string[] args)
        {
            var CLI = new CommandLineInterface();
            CLI.Run();
            var Server = new APIServer(1441);
            //await Server.Listen();
        }
    }
}