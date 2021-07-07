using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Qwirkle.Core.Entities;
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
        public BoardViewModel BoardViewModel { get; private set; }

        public bool GameInProgress { get => _gameInProgress; private set { _gameInProgress = value; NotifyPropertyChanged(); } }
        private bool _gameInProgress;
        public bool GameNotInProgress { get => !_gameInProgress; private set { _gameInProgress = !value; NotifyPropertyChanged(); } }

        public bool IsNewGameEnable { get => _isNewGameEnable; set { _isNewGameEnable = value; NotifyPropertyChanged(); } }
        private bool _isNewGameEnable;
        private List<Player> _players;
        private Player _currentPlayer;
        private readonly int _gameId;
        private readonly IConfigurationRoot _configuration;
        private readonly HttpClient _httpClient;

        public ICommand StartGameCommand => new RelayCommand(StartGame);

        public GameViewModel(IConfigurationRoot configuration, List<Player> players, HttpClient httpClient)
        {
            _configuration = configuration;
            _players = players;
            _gameId = GetGameId();
            _httpClient = httpClient;
            IsNewGameEnable = true;
            BoardViewModel = new BoardViewModel();
            GameInProgress = false;
        }

        public void StartGame(object _ = null)
        {
            var game = GetGame();
            _players = game.Players;
            _currentPlayer = _players.FirstOrDefault(p => p.IsTurn);
            Rack currentRack = new(_currentPlayer.Rack.Tiles);
            RackViewModel = new(currentRack, _configuration);
        }

        private Game GetGame()
        {
            var response = GetGameAsync().Result;
            if (!response.IsSuccessStatusCode) return null;
            var resultGetGame = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<Game>(resultGetGame);
        }

        private bool CanExecuteChangeTiles()
        {
            return RackViewModel != null && RackViewModel.SelectedCells.Count != 0;
        }

        public void ChangeTiles(object o)
        {
            if (RackViewModel.SelectedCells.Count == 0)
            {
                MessageBox.Show("aucune tuile à échanger");
                return;
            }
            List<int> tilesIds = new List<int>();
            foreach (var cell in RackViewModel.SelectedCells)
                tilesIds.Add(((TileOnPlayerViewModel)cell.Item).Tile.Id);

            //var rack = CoreUseCases.SwapTiles(1, tilesIds); //todo playerId
            //if (rack != null)
            //    RackViewModel = new RackViewModel(rack, _configuration);
            //else
            //    MessageBox.Show("aucune tuile ne peut être échangée");
        }

        public void Tips(object o)
        {
            MessageBox.Show("Fonctionnalité Tips en cours de dev...");
        }

        public void Play(object o)
        {
            MessageBox.Show("Fonctionnalité Play en cours de dev...");
        }

        private int GetGameId() => _players[0].GameId;
        private async Task<HttpResponseMessage> GetGameAsync() => await _httpClient.PostAsJsonAsync("Games/Get", new List<int> { _gameId }).ConfigureAwait(false);
    }
}