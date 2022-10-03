using System;

using Cardgame.Base;

namespace Cardgame.Blackjack {
    class MainRunner {
        static void Main(string[] args)
        {
            var q = new Blackjack.Hand();
            q.Add( new Base.Card(1, Base.Suit.Hearts) );
            q.Add( new Base.Card(12, Base.Suit.Hearts) );
            Console.WriteLine(q.IsBlackjack());
        }
    }
}