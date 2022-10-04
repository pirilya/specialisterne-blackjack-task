using System;

namespace Cardgame.Blackjack {
    class MainRunner {
        static void Main(string[] args)
        {
            var CLI = new CommandLineInterface();
            CLI.Run();
            //var Server = new APIServer(8000);
            // Note: This is a hacky way to call an async function, 
            // consider looking up what the approved non-hacky way is
            //Server.Listen().GetAwaiter().GetResult(); 
        }
    }
}