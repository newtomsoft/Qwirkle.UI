using System.Collections.Generic;

namespace Qwirkle.Core.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public Board Board { get; set; }
        public List<Player> Players { get; set; }
        public Bag Bag { get; set; }

        public Game(int id, List<TileOnBoard> tiles, List<Player> players, Bag bag = null) //todo board
        {
            Id = id;
            Board = new Board(tiles);
            Players = players;
            Bag = bag;
        }
    }
}
