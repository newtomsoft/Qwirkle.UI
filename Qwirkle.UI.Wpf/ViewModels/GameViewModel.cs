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
        public List<TileOnPlayer> TilesOnPlayerSelected { get => _tilesOnPlayerSelected; private set { _tilesOnPlayerSelected = value; NotifyPropertyChanged(); } }
        private List<TileOnPlayer> _tilesOnPlayerSelected;


        private List<int> _boardIndexSelected;
        private List<CoordinatesInGame> _boardCoordinatesSelected;


        public bool IsStartGameEnable { get => _isNewGameEnable; set { _isNewGameEnable = value; NotifyPropertyChanged(); } }
        private bool _isNewGameEnable;
        private Player _currentPlayer;
        private readonly int _gameId;
        private readonly IConfigurationRoot _configuration;
        private readonly HttpClient _httpClient;

        public ICommand StartGameCommand => new RelayCommand(StartGame);
        public ICommand PlayCommand => new RelayCommand(Play);
        public ICommand SwapTilesCommand => new RelayCommand(SwapTiles);

        public GameViewModel(IConfigurationRoot configuration, List<Player> players, HttpClient httpClient)
        {
            TilesOnPlayerSelected = new();
            _boardIndexSelected = new();
            _boardCoordinatesSelected = new();
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

        private void SelectTileOnPlayer(TileOnPlayer tileSelected) => TilesOnPlayerSelected.Add(tileSelected);

        private void SelectCellOnBoard(int index)
        {
            _boardIndexSelected.Add(index);
            CoordinatesInGame coordinate = IndexToCoordinates(index);
            _boardCoordinatesSelected.Add(coordinate);
        }

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

        private void Play(object _ = null)
        {
            var tilesOnPlayer = TilesOnPlayerSelected;
            List<int> tilesIds = GetIds(tilesOnPlayer);
            var response = PlayGameAsync(tilesIds, _boardCoordinatesSelected).Result;
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.ReasonPhrase);
                ResetSelectedElements();
                return;
            }

            var resultGetGame = response.Content.ReadAsStringAsync().Result;
            var playReturn = JsonConvert.DeserializeObject<PlayReturn>(resultGetGame);
            if (playReturn.Code != PlayReturnCode.Ok)
            {
                MessageBox.Show(playReturn.Code.ToString());
                ResetSelectedElements();
                return;
            }
            var tiles = new List<TileOnBoard>();
            for (var i = 0; i < tilesOnPlayer.Count; i++)
            {
                var tileOnPlayer = tilesOnPlayer[i];
                var coordinate = _boardCoordinatesSelected[i];
                var tileToPutOnBoard = new TileOnBoard(tileOnPlayer.Id, tileOnPlayer.Color, tileOnPlayer.Form, coordinate);
                tiles.Add(tileToPutOnBoard);
            }
            _currentPlayer.Rack = playReturn.NewRack;
            RackViewModel = new RackViewModel(playReturn.NewRack, SelectTileOnPlayer, _configuration);
            BoardViewModel.AddTiles(tiles);
            ResetSelectedElements();
        }

        private static List<int> GetIds(List<TileOnPlayer> tilesOnPlayer) => tilesOnPlayer.Select(t => t.Id).ToList();

        private void ResetSelectedElements()
        {
            TilesOnPlayerSelected = new();
            _boardIndexSelected = new();
            _boardCoordinatesSelected = new();
        }

        public bool CanExecuteSwapTiles() => TilesOnPlayerSelected.Count != 0;

        private void SwapTiles(object _ = null)
        {
            var tilesOnPlayer = TilesOnPlayerSelected;
            List<int> tilesIds = GetIds(tilesOnPlayer);
            var response = SwapTilesAsync(tilesIds).Result;
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.ReasonPhrase);
                ResetSelectedElements();
                return;
            }

            var resultGetGame = response.Content.ReadAsStringAsync().Result;
            var swapReturn = JsonConvert.DeserializeObject<SwapTilesReturn>(resultGetGame);
            if (swapReturn.Code != PlayReturnCode.Ok)
            {
                MessageBox.Show(swapReturn.Code.ToString());
                ResetSelectedElements();
                return;
            }
            _currentPlayer.Rack = swapReturn.NewRack;
            RackViewModel = new RackViewModel(swapReturn.NewRack, SelectTileOnPlayer, _configuration);
            ResetSelectedElements();
        }

        public void Tips(object o)
        {
            MessageBox.Show("Fonctionnalité Tips en cours de dev...");
        }



        private int GetGameId(List<Player> players) => players[0].GameId;

        private async Task<HttpResponseMessage> GetGameAsync() => await _httpClient.PostAsJsonAsync("Games/Get", new List<int> { _gameId }).ConfigureAwait(false);

        private async Task<HttpResponseMessage> PlayGameAsync(List<int> tileIds, List<CoordinatesInGame> coordinates)
        {
            var tiles = new List<TileToPlayModel>();
            for (var i = 0; i < tileIds.Count; i++)
            {
                var tile = new TileToPlayModel() { PlayerId = _currentPlayer.Id, TileId = tileIds[i], X = coordinates[i].X, Y = coordinates[i].Y };
                tiles.Add(tile);
            }
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