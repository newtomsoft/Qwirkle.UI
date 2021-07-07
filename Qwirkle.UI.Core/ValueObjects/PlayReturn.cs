using Qwirkle.Core.Entities;
using Qwirkle.Core.Enums;
using System.Collections.Generic;

namespace Qwirkle.Core.ValueObjects
{
    public record PlayReturn
    {
        public PlayReturnCode Code { get; set; }
        public List<TileOnBoard> TilesPlayed { get; set; }
        public Rack NewRack { get; set; }
        public int Points { get; set; }

        public PlayReturn(PlayReturnCode code, List<TileOnBoard> tilesPlayed, Rack newRack, int points)
        {
            Code = code;
            TilesPlayed = tilesPlayed;
            NewRack = newRack;
            Points = points;
        }
    }
}
