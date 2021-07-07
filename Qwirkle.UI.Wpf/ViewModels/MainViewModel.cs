using Microsoft.Extensions.Configuration;
using Newtomsoft.Configuration;
using System;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class MainViewModel : NotifyPropertyChangedBase
    {
        private IConfigurationRoot _configuration;
        private readonly Action<NotifyPropertyChangedBase> _changeViewModel;
        private readonly MainMenuViewModel _mainMenuViewModel;

        public NotifyPropertyChangedBase CurrentViewModel { get => _currentViewModel; set { _currentViewModel = value; NotifyPropertyChanged(); } }
        private NotifyPropertyChangedBase _currentViewModel;

        public MainViewModel()
        {
            _configuration = NewtomsoftConfiguration.GetConfiguration();
            _changeViewModel = viewModel => CurrentViewModel = viewModel;

            _mainMenuViewModel = new MainMenuViewModel(_changeViewModel);
            _changeViewModel(_mainMenuViewModel);
        }
    }
}
