namespace Leapfrog.Application.Interfaces
{
    public interface IDelegateCommandService
    {
        IDelegateCommand Create(Func<Task> execute, Func<bool> canExecute = null!);
    }
}
