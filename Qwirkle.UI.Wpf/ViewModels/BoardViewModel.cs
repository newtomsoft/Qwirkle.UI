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
            ColumnsNumber = 6;
            RowsNumber = 6;
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
                RaiseTileAdded(tileModel);
                TilesOnBoardModel.Add(tileModel);
            }
        }

        public void AddTile(TileOnBoard tile) => AddTiles(new List<TileOnBoard>() { tile });

        private string GetFullNameImage(Tile tile) => Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection("ImagesPath:Tiles").Value, tile.GetNameImage());
    }
}
