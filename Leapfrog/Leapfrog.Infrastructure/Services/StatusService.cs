using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Enums.StatusEnums;

namespace Leapfrog.Infrastructure.Services
{
    public class StatusService(IStateContainerService stateContainerService) : IStatusService
    {
        private string _text = "Welcome to Leapfrog";
        public string Text => _text;

        private StatusType _type;

        public string Type
        {
            get
            {
                return _type switch
                {
                    StatusType.NORMAL => "white",
                    StatusType.WARNING => "yellow",
                    StatusType.ERROR => "red",
                    _ => "white",
                };
            }
        }

        public void SetStatus(StatusType type, string text)
        {
            _text = text;
            _type = type;
            stateContainerService.NotifyStateChanged();
        }

        public void SetStatus(string text)
        {
            _text = text;
            stateContainerService.NotifyStateChanged();
        }
    }
}
