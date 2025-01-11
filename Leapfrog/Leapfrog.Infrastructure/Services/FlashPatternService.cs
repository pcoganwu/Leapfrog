using Leapfrog.Core.Entities;
using Leapfrog.Core.Enums.FlashPatternEnums;
using Leapfrog.Core.Enums.LightConfigEnums;
using Leapfrog.Interfaces.Services;
using System.Collections.ObjectModel;

namespace Leapfrog.Infrastructure.Services
{
    public class FlashPatternService : IFlashPatternService
    {
        private const int MaxItems = 32;
        public ObservableCollection<FlashPatternItem> Items { get; set; } = new ObservableCollection<FlashPatternItem>();

        public void AddFlashPatternItem(int index)
        {
            if (Items.Count >= MaxItems) return;

            Items.Insert(index, new FlashPatternItem((int)FlashMode.FlashModeBoth, "100"));
        }

        public void AddFlashPatternItem(int index, FlashPatternItem item)
        {
            if (Items.Count >= MaxItems) return;

            Items.Insert(index, item);
        }

        public void DeleteFlashPatternItem(int index)
        {
            if (Items.Count <= 1) return;

            Items.RemoveAt(index);
        }

        public void LoadPattern(FlashPatternType type)
        {
            Items.Clear();
            switch (type)
            {
                case FlashPatternType.FlashPatternMUTCD:
                    AddMUTCDPattern();
                    break;

                case FlashPatternType.FlashPatternWigWag:
                    AddWigWagPattern();
                    break;

                case FlashPatternType.FlashPatternCustom:
                    AddCustomPattern();
                    break;

                case FlashPatternType.FlashPatternTypeNone:
                default:
                    break;
            }
        }

        private void AddMUTCDPattern()
        {
            var pattern = new[]
            {
                new FlashPatternItem((int)FlashMode.FlashModeLeft, "50"),
                new FlashPatternItem((int)FlashMode.FlashModeNone, "50"),
                new FlashPatternItem((int)FlashMode.FlashModeRight, "50"),
                new FlashPatternItem((int)FlashMode.FlashModeNone, "50"),
                new FlashPatternItem((int)FlashMode.FlashModeLeft, "50"),
                new FlashPatternItem((int)FlashMode.FlashModeNone, "50"),
                new FlashPatternItem((int)FlashMode.FlashModeRight, "50"),
                new FlashPatternItem((int)FlashMode.FlashModeNone, "50"),
                new FlashPatternItem((int)FlashMode.FlashModeBoth, "50"),
                new FlashPatternItem((int)FlashMode.FlashModeNone, "50"),
                new FlashPatternItem((int)FlashMode.FlashModeBoth, "50"),
                new FlashPatternItem((int)FlashMode.FlashModeNone, "250")
            };
            foreach (var item in pattern)
            {
                Items.Add(item);
            }
        }

        private void AddWigWagPattern()
        {
            Items.Add(new FlashPatternItem((int)FlashMode.FlashModeLeft, "500"));
            Items.Add(new FlashPatternItem((int)FlashMode.FlashModeRight, "500"));
        }

        private void AddCustomPattern()
        {
            Items.Add(new FlashPatternItem((int)FlashMode.FlashModeBoth, "500"));
            Items.Add(new FlashPatternItem((int)FlashMode.FlashModeNone, "500"));
        }
    }
}
