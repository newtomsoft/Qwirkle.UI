using Qwirkle.Core.Entities;
using Qwirkle.UI.Wpf.Models;
using Qwirkle.UI.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Qwirkle.UI.Wpf.Views
{
    public partial class BoardView : UserControl
    {
        public BoardView()
        {
            InitializeComponent();
            Loaded += (_, __) => CreateBoard();
            DataContextChanged += OnDataContextChange;
        }

        private void OnDataContextChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is BoardViewModel oldValue) oldValue.RefreshBoard -= RefreshBoard;
            if (e.NewValue is BoardViewModel newValue) newValue.RefreshBoard += RefreshBoard;
        }

        private void RefreshBoard(object _, TileOnBoardModel tile)
        {
            var board = (BoardViewModel)DataContext;
            var xIndex = tile.Tile.Coordinates.X;
            var yIndex = tile.Tile.Coordinates.Y;
            var index = yIndex * board.RowsNumber + xIndex;
            var image = new Image { Source = tile.Image };
            ((Button)Board.Children[index]).Content = image;
        }


        private void CreateBoard()
        {
            var board = (BoardViewModel)DataContext;
            if (board is null) return;
            for (int tileIndex = 0; tileIndex < board.RowsNumber * board.ColumnsNumber; tileIndex++)
            {
                Button element = new Button();
                element.Content = tileIndex.ToString();
                element.Command = board.TryMoveTileCommand;
                element.CommandParameter = tileIndex;
                Board.Children.Add(element);
            }
        }

    }
}
