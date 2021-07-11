using Microsoft.Extensions.Configuration;
using Qwirkle.Core.Entities;
using Qwirkle.UI.Wpf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class BoardViewModel : NotifyPropertyChangedBase
    {
        public List<TileOnBoardModel> TilesOnBoardModel { get; set; }

        public int ColumnsNumber { get => _columnsNumber; private set { _columnsNumber = value; NotifyPropertyChanged(); } }
        private int _columnsNumber;
        public int RowsNumber { get => _rowNumber; private set { _rowNumber = value; NotifyPropertyChanged(); } }
        private int _rowNumber;

        private readonly IConfiguration _configuration;

        public delegate void RefreshBoardEventHandler(object sender, TileOnBoardModel tile);
        public event RefreshBoardEventHandler RefreshBoard;
        private void RaiseTileAdded(TileOnBoardModel tile) => RefreshBoard?.Invoke(this, tile);

        private Action<int> _selectCellOnBoard { get; }

        public ICommand TryMoveTileCommand => new RelayCommand(TryMoveTile);

        public BoardViewModel(Action<int> selectCellOnBoard, IConfiguration configuration)
        {
            const int minColumnsNumber = 20;
            const int minRowsNumber = 20;
            ColumnsNumber = minColumnsNumber;
            RowsNumber = minRowsNumber;
            _selectCellOnBoard = selectCellOnBoard;
            _configuration = configuration;
            TilesOnBoardModel = new();
        }

        private TileOnBoardModel TileOnBoardToTileOnBoardModel(TileOnBoard tile) => new TileOnBoardModel(tile, GetFullNameImage(tile));

        private void TryMoveTile(object tileIndex)
        {
            int tileIdexInBoard = (int)tileIndex;
            _selectCellOnBoard(tileIdexInBoard);
        }

        public void AddTiles(List<TileOnBoard> tiles)
        {
            foreach (var tile in tiles)
            {
                TileOnBoardModel tileModel = TileOnBoardToTileOnBoardModel(tile);
                TilesOnBoardModel.Add(tileModel);
                //var xMin = TilesOnBoardModel.Select(t => t.Tile.Coordinates.X).Min();
                //var xMax = TilesOnBoardModel.Select(t => t.Tile.Coordinates.X).Max();
                //var yMin = TilesOnBoardModel.Select(t => t.Tile.Coordinates.Y).Min();
                //var yMax = TilesOnBoardModel.Select(t => t.Tile.Coordinates.Y).Max();
                //ColumnsNumber = xMax - xMin + 1 + 2 * 5;
                //RowsNumber = yMax - yMin + 1 + 2 * 5;
                RaiseTileAdded(tileModel);
            }
        }

        public void AddTile(TileOnBoard tile) => AddTiles(new List<TileOnBoard>() { tile });

        private string GetFullNameImage(Tile tile) => Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection("ImagesPath:Tiles").Value, tile.GetNameImage());
    }
}
