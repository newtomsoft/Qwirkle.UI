﻿using Qwirkle.Core.Entities;
using System;
using System.Windows.Media.Imaging;

namespace Qwirkle.UI.Wpf.Models
{
    public class TileOnBoardModel
    {
        public TileOnBoard Tile { get; }
        public string FullNameImage { get; }
        public BitmapImage Image { get; }

        public TileOnBoardModel(TileOnBoard tile, string fullNameImage)
        {
            Tile = tile;
            FullNameImage = fullNameImage;
            Image = new BitmapImage(new Uri(FullNameImage));
        }
    }
}
