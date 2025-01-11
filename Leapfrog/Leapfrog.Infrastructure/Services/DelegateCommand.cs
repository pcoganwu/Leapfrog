using Leapfrog.Application.Interfaces;

namespace Leapfrog.Infrastructure.Services
{
    public class DelegateCommand(Func<Task> execute, Func<bool>? canExecute = null) : IDelegateCommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => canExecute == null || canExecute();

        public async void Execute(object? parameter) => await execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
