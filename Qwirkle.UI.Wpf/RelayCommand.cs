using System;
using System.Windows.Input;

namespace Qwirkle.UI.Wpf
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _command;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> command) : this(command, (_) => true) { }

        public RelayCommand(Action<object> command, Predicate<object> canExecute)
        {
            _command = command;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute(parameter);
        public void Execute(object parameter) => _command(parameter);
    }
}