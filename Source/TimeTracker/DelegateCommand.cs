using System;

namespace TimeTracker
{
    public class DelegateCommand : BaseCommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        public event EventHandler? CanExecuteChanged;

        public DelegateCommand(Predicate<object> canExecute, Action<object> execute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public DelegateCommand(Action<object> execute) : this(null, execute)
        {
        }
        public void RaiseCanExecuteChanged() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public override void Execute(object? parameter) => _execute?.Invoke(parameter);

    }
}
