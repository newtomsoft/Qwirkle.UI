﻿using Microsoft.Extensions.Configuration;
using Qwirkle.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace Qwirkle.UI.Wpf.ViewModels
{

    public class RackViewModel : NotifyPropertyChangedBase
    {
        private IList<DataGridCellInfo> _selectedCells;
        public IList<DataGridCellInfo> SelectedCells { get => _selectedCells; set { _selectedCells = value; NotifyPropertyChanged(); } }

        public List<TileOnPlayerViewModel> TilesViewModel { get; set; }

        public TileOnPlayerViewModel SelectedTileViewModel { get; set; }

        private readonly IConfiguration _configuration;

        public RackViewModel(Rack rack, IConfiguration configuration)
        {
            _configuration = configuration;
            SelectedCells = new List<DataGridCellInfo>();
            TilesViewModel = TilesViewModelFromRack(rack);
        }

        private List<TileOnPlayerViewModel> TilesViewModelFromRack(Rack rack)
        {
            var tilesViewModel = new List<TileOnPlayerViewModel>();
            foreach (var tile in rack.Tiles)
                tilesViewModel.Add(new TileOnPlayerViewModel(tile, GetFullNameImage(tile)));
            tilesViewModel = tilesViewModel.OrderBy(t => t.Tile.RackPosition).ToList();
            return tilesViewModel;
        }

        private string GetFullNameImage(TileOnPlayer tile) => Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection("ImagesPath:Tiles").Value, tile.GetNameImage());
    }
}
