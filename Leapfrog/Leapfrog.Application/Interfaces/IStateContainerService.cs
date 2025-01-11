namespace Leapfrog.Application.Interfaces
{
    public interface IStateContainerService
    {
        event Action? OnChanged;
        void NotifyStateChanged();
    }
}
