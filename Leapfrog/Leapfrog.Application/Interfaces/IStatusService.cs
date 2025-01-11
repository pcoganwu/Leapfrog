using Leapfrog.Core.Enums.StatusEnums;

namespace Leapfrog.Application.Interfaces
{
    public interface IStatusService
    {
        string Text { get; }
        string Type { get; }
        void SetStatus(StatusType type, string text);
        void SetStatus(string text);
    }
}
