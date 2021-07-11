using Qwirkle.UI.Wpf.ViewModels;
using Qwirkle.UI.Wpf.Views;
using System.Windows;

namespace Qwirkle.UI.Wpf
{
    public partial class App : Application
    {
        private MainViewModel _mainViewModel;

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainView = new MainView();
            _mainViewModel = new MainViewModel();
            mainView.DataContext = _mainViewModel;
            mainView.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //_mainViewModel.Dispose();
            base.OnExit(e);
        }
    }
}
