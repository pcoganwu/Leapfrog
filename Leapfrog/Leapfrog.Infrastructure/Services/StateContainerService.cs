using Leapfrog.Application.Interfaces;

namespace Leapfrog.Infrastructure.Services
{
    public class StateContainerService : IStateContainerService
    {
        public event Action? OnChanged;

        public void NotifyStateChanged() => OnChanged?.Invoke();
    }
}
