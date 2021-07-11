using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Qwirkle.Core.Entities;
using Qwirkle.Core.Enums;
using Qwirkle.Core.ValueObjects;
using Qwirkle.UI.Wpf.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class GameViewModel : NotifyPropertyChangedBase
    {
        public RackViewModel RackViewModel { get => _rackViewModel; private set { _rackViewModel = value; NotifyPropertyChanged(); } }
        private RackViewModel _rackViewModel;
        public BoardViewModel BoardViewModel { get => _boardViewModel; private set { _boardViewModel = value; NotifyPropertyChanged(); } }
        private BoardViewModel _boardViewModel;

        private Game _game;

        public int MovePoints { get => _movePoints; private set { _movePoints = value; NotifyPropertyChanged(); } }
        private int _movePoints;

        public int PlayerPoints { get => _playerPoints; private set { _playerPoints = value; NotifyPropertyChanged(); } }
        private int _playerPoints;

        public int TilesPlayedNumber { get => _tilesPlayedNumber; private set { _tilesPlayedNumber = value; NotifyPropertyChanged(); } }
        private int _tilesPlayedNumber;

        public int TilesInBagNumber { get => _tilesInBagNumber; private set { _tilesInBagNumber = value; NotifyPropertyChanged(); } }
        private int _tilesInBagNumber;

        public bool IsStartGameEnable { get => _isNewGameEnable; set { _isNewGameEnable = value; NotifyPropertyChanged(); } }
        private bool _isNewGameEnable;
        private Player _currentPlayer;
        private readonly int _gameId;
        private readonly IConfigurationRoot _configuration;
        private readonly HttpClient _httpClient;

        public ICommand StartGameCommand => new RelayCommand(StartGame);
        public ICommand PlayMoveCommand => new RelayCommand(PlayMove);
        public ICommand SwapTilesCommand => new RelayCommand(SwapTiles);

        private List<TileOnPlayer> _tilesOnPlayerSelected;
        private Dictionary<CoordinatesInGame, TileOnPlayer> _tilesOnPlayerToPlaySelected;

        public GameViewModel(IConfigurationRoot configuration, List<Player> players, HttpClient httpClient)
        {
            _tilesOnPlayerSelected = new();
            _tilesOnPlayerToPlaySelected = new Dictionary<CoordinatesInGame, TileOnPlayer>();
            TilesInBagNumber = 108;
            _configuration = configuration;
            _gameId = GetGameId(players);
            _httpClient = httpClient;
            IsStartGameEnable = true;
        }

        public void StartGame(object _ = null)
        {
            IsStartGameEnable = false;
            _game = GetGame();
            _currentPlayer = _game.Players.FirstOrDefault(p => p.IsTurn);
            BoardViewModel = new BoardViewModel(SelectCellOnBoard, _configuration);
            RackViewModel = new(new Rack(_currentPlayer.Rack.Tiles), SelectTileOnPlayer, _configuration);
        }

        private void SelectTileOnPlayer(TileOnPlayer tileSelected) => _tilesOnPlayerSelected.Add(tileSelected);

        private void SelectCellOnBoard(int index) => _tilesOnPlayerToPlaySelected.Add(IndexToCoordinates(index), _tilesOnPlayerSelected[^1]);

        private CoordinatesInGame IndexToCoordinates(int index)
        {
            var x = index % BoardViewModel.ColumnsNumber;
            var y = index / BoardViewModel.ColumnsNumber;
            return new CoordinatesInGame((sbyte)x, (sbyte)y);
        }

        private Game GetGame()
        {
            var response = GetGameAsync().Result;
            if (!response.IsSuccessStatusCode) return null;
            var resultGetGame = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<Game>(resultGetGame);
        }

        private void PlayMove(object _ = null)
        {
            List<int> tilesSelectedIds = GetIds(_tilesOnPlayerToPlaySelected.Values);
            var response = PlayGameAsync().Result;
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.ReasonPhrase);
                ResetSelectedTiles();
                return;
            }
            var resultGetGame = response.Content.ReadAsStringAsync().Result;
            var playReturn = JsonConvert.DeserializeObject<PlayReturn>(resultGetGame);
            if (playReturn.Code != PlayReturnCode.Ok)
            {
                MessageBox.Show(playReturn.Code.ToString());
                ResetSelectedTiles();
                return;
            }
            var tiles = new List<TileOnBoard>();
            foreach (var tileToPlay in _tilesOnPlayerToPlaySelected)
            {
                var tileOnPlayer = tileToPlay.Value;
                var coordinate = tileToPlay.Key;
                var tileToPutOnBoard = new TileOnBoard(tileOnPlayer.Id, tileOnPlayer.Color, tileOnPlayer.Form, coordinate);
                tiles.Add(tileToPutOnBoard);
            }

            _currentPlayer.Rack = playReturn.NewRack;
            MovePoints = playReturn.Points;
            PlayerPoints += MovePoints;
            TilesPlayedNumber += _tilesOnPlayerToPlaySelected.Count;
            TilesInBagNumber -= _tilesOnPlayerToPlaySelected.Count;

            RackViewModel = new RackViewModel(playReturn.NewRack, SelectTileOnPlayer, _configuration);
            BoardViewModel.AddTiles(tiles);
            ResetSelectedTiles();
        }

        private static List<int> GetIds(ICollection<TileOnPlayer> tilesOnPlayer) => tilesOnPlayer.Select(t => t.Id).ToList();

        private void ResetSelectedTiles()
        {
            _tilesOnPlayerSelected = new();
            _tilesOnPlayerToPlaySelected = new();
        }

        public bool CanExecuteSwapTiles() => _tilesOnPlayerToPlaySelected.Count != 0;

        private void SwapTiles(object _ = null)
        {
            List<int> tilesIds = GetIds(_tilesOnPlayerSelected);
            var response = SwapTilesAsync(tilesIds).Result;
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.ReasonPhrase);
                ResetSelectedTiles();
                return;
            }

            var resultGetGame = response.Content.ReadAsStringAsync().Result;
            var swapReturn = JsonConvert.DeserializeObject<SwapTilesReturn>(resultGetGame);
            if (swapReturn.Code != PlayReturnCode.Ok)
            {
                MessageBox.Show(swapReturn.Code.ToString());
                ResetSelectedTiles();
                return;
            }
            _currentPlayer.Rack = swapReturn.NewRack;
            RackViewModel = new RackViewModel(swapReturn.NewRack, SelectTileOnPlayer, _configuration);
            ResetSelectedTiles();
        }

        public void Tips(object o)
        {
            MessageBox.Show("Fonctionnalité Tips en cours de dev...");
        }



        private int GetGameId(List<Player> players) => players[0].GameId;

        private async Task<HttpResponseMessage> GetGameAsync() => await _httpClient.PostAsJsonAsync("Games/Get", new List<int> { _gameId }).ConfigureAwait(false);

        private async Task<HttpResponseMessage> PlayGameAsync()
        {
            var tiles = new List<TileToPlayModel>();
            foreach (var tileToPlay in _tilesOnPlayerToPlaySelected)
                tiles.Add(new TileToPlayModel() { PlayerId = _currentPlayer.Id, TileId = tileToPlay.Value.Id, X = tileToPlay.Key.X, Y = tileToPlay.Key.Y });

            return await _httpClient.PostAsJsonAsync("Games/PlayTiles", tiles).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> SwapTilesAsync(List<int> tileIds)
        {
            var tiles = new List<TileToDrawModel>();
            for (var i = 0; i < tileIds.Count; i++)
            {
                var tile = new TileToDrawModel() { PlayerId = _currentPlayer.Id, TileId = tileIds[i] };
                tiles.Add(tile);
            }
            return await _httpClient.PostAsJsonAsync("Games/SwapTiles", tiles).ConfigureAwait(false);
        }
    }
}