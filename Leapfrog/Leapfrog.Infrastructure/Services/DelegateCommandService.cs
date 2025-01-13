using Leapfrog.Application.Interfaces;

namespace Leapfrog.Infrastructure.Services
{
    public class DelegateCommandService : IDelegateCommandService
    {
        public IDelegateCommand Create(Func<Task> execute, Func<bool> canExecute = null!)
        {
            return new DelegateCommand(execute, canExecute);
        }
    }
}
