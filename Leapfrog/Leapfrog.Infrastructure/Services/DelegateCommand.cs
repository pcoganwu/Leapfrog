using Leapfrog.Application.Interfaces;

namespace Leapfrog.Infrastructure.Services
{
    public class DelegateCommand : IDelegateCommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public DelegateCommand(Func<Task> execute, Func<bool> canExecute = null!)
        {
            _execute=execute;
            _canExecute=canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();

        public async void Execute(object? parameter) => await _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
