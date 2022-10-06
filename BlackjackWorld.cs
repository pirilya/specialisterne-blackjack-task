using System;
using System.Linq;
using System.Collections.Generic;

namespace Cardgame.Blackjack {
    public class Player {
        public double Money { get; set; }
        public string Name { get; set; }
    }
    public class World {
        Dictionary<string, Game> Games { get; }
        Dictionary<string, Player> Players { get; }
        public World () {
            Games = new Dictionary<string, Game>();
            Players = new Dictionary<string, Player>();
        }
        public Game AddGame (string gameid) {
            if (Games.ContainsKey(gameid)) {
                throw new InvalidOperationException("A game by that name already exists!");
            }
            Games[gameid] = new Game();
            return Games[gameid];
        }
        public Game GetGame (string gameid) {
            // will error if a game by that name doesn't exist
            return Games[gameid];
        }
        public Player AddPlayer (string name, double money) {
            if (Players.ContainsKey(name)) {
                throw new InvalidOperationException("A player by that name already exists!");
            }
            var p = new Player();
            p.Money = money;
            p.Name = name;
            Players[name] = p;
            return p;
        }
        public Player GetPlayer (string name) {
            // will error if a player by that name doesn't exist
            return Players[name];
        }
    }
}