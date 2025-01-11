using System.Windows.Input;

namespace Leapfrog.Application.Interfaces
{
    public interface IDelegateCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
