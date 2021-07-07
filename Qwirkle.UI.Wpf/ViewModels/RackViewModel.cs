using Microsoft.Extensions.Configuration;
using Qwirkle.Core.Entities;
using Qwirkle.UI.Wpf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Qwirkle.UI.Wpf.ViewModels
{

    public class RackViewModel : NotifyPropertyChangedBase
    {
        public List<TileOnPlayerModel> TilesOnPlayerModel { get; set; }
        public TileOnPlayer TileOnPlayerSelected { get; private set; }

        private Action<TileOnPlayer> _selectTileOnPlayer;
        private readonly IConfiguration _configuration;

        public ICommand TileMoveCommand => new RelayCommand(MoveTile);
        public ICommand SelectTileCommand => new RelayCommand(SelectTile);


        public RackViewModel(Rack rack, Action<TileOnPlayer> selectTileOnPlayer, IConfiguration configuration)
        {
            _configuration = configuration;
            TilesOnPlayerModel = RackToTilesOnPlayerModel(rack);
            _selectTileOnPlayer = selectTileOnPlayer;
        }

        private void SelectTile(object tileSelected)
        {
            TileOnPlayerSelected = ((TileOnPlayerModel)tileSelected).Tile;
            _selectTileOnPlayer(TileOnPlayerSelected);
        }

        private void MoveTile(object obj)
        {
            throw new NotImplementedException();
        }

        private List<TileOnPlayerModel> RackToTilesOnPlayerModel(Rack rack)
        {
            var tilesModel = new List<TileOnPlayerModel>();
            foreach (var tile in rack.Tiles) tilesModel.Add(new TileOnPlayerModel(tile, GetFullNameImage(tile)));
            tilesModel = tilesModel.OrderBy(t => t.Tile.RackPosition).ToList();
            return tilesModel;
        }

        private string GetFullNameImage(Tile tile) => Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection("ImagesPath:Tiles").Value, tile.GetNameImage());
    }
}
