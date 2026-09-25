using System.Windows.Input;

namespace SBS.Core.EInvoiceUtilsDemo
{
    internal class RelayCommand: ICommand
    {
        readonly Action<object?> _execute;
        readonly Predicate<object?> _canExecute;
        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action<object?> execute, Predicate<object?> canExecute)
        {
            ArgumentNullException.ThrowIfNull(execute, nameof(execute));
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (_canExecute == null)
                return true;
            return _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
