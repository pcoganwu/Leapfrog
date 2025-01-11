using Leapfrog.Core.Entities;
using Leapfrog.Core.Enums.LightConfigEnums;
using System.Collections.ObjectModel;

namespace Leapfrog.Interfaces.Services
{
    public interface IFlashPatternService
    {
        ObservableCollection<FlashPatternItem> Items { get; set; }

        void AddFlashPatternItem(int index);
        void AddFlashPatternItem(int index, FlashPatternItem item);
        void DeleteFlashPatternItem(int index);
        void LoadPattern(FlashPatternType type);
    }
}