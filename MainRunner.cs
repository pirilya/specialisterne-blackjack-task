using System;

using Cardgame.Base;

namespace Cardgame.Blackjack {
    class MainRunner {
        static void Main(string[] args)
        {
            var q = new Blackjack.Game();
            q.AddPlayerPos(100.0);
            Console.WriteLine(q.Dealer.Hand);
            Console.WriteLine(q.PlayerPositions[0].Hand);
            q.Dealer.Run();
            Console.WriteLine("{0} {1}", q.Dealer.Hand, q.Dealer.Hand.Score());
        }
    }
}