﻿using Qwirkle.Core.Enums;
using Qwirkle.Core.ValueObjects;

namespace Qwirkle.Core.Entities
{
    public class TileOnBoard : Tile
    {
        public CoordinatesInGame Coordinates { get; }

        public TileOnBoard(int id, TileColor color, TileForm form, CoordinatesInGame coordinates) : base(id, color, form)
        {
            Coordinates = coordinates;
        }

        public TileOnBoard(TileColor color, TileForm form, CoordinatesInGame coordinates) : this(0, color, form, coordinates)
        { }
    }
}
