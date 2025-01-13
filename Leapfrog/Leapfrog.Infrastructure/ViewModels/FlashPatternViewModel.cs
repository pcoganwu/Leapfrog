using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Entities;
using Leapfrog.Core.Enums.LightConfigEnums;
using Leapfrog.Interfaces.Services;
using System.Collections.ObjectModel;

namespace Leapfrog.Infrastructure.ViewModels
{
    public class FlashPatternViewModel
    {
        //private readonly IDelegateCommand _delegateCommand;
        private readonly IStateContainerService _stateContainerService;
        private readonly IFlashPatternService _flashPatternService;
        private readonly ILightConfigService _lightConfigService;
        private readonly IDelegateCommandService _delegateCommandService;

        public FlashPatternViewModel(IStateContainerService stateContainerService, 
            IFlashPatternService flashPatternService,
            ILightConfigService lightConfigService, 
            IDelegateCommandService delegateCommandService)
        {
            //_delegateCommand=delegateCommand;
            _stateContainerService=stateContainerService;
            _flashPatternService=flashPatternService;
            _lightConfigService=lightConfigService;
            _delegateCommandService=delegateCommandService;
            FlashPattern = _flashPatternService.Items;

            UploadAllSettingsCommand = _delegateCommandService.Create(() =>
            {
                // TODO
                return Task.CompletedTask;
            });

            DownloadAllSettingsCommand = _delegateCommandService.Create(() =>
            {
                // TODO
                return Task.CompletedTask;
            });

            UploadFlashPatternCommand = _delegateCommandService.Create(() =>
            {
                // TODO
                return Task.CompletedTask;
            });

            DownloadFlashPatternCommand = _delegateCommandService.Create(() =>
            {
                // TODO
                return Task.CompletedTask;
            });
        }

        public ObservableCollection<FlashPatternItem> FlashPattern { get; set; }

        private bool _flashPatternListVisible;
        public bool FlashPatternListVisible
        {
            get { return _flashPatternListVisible; }
            set
            {
                _flashPatternListVisible = value;
                _stateContainerService.NotifyStateChanged();
            }
        }

        public int FlashPatternIndex
        {
            get
            {
                return _lightConfigService.LightConfig.FlashPatternIndex;
            }
            set
            {
                // Manage custom pattern list visibility
                switch ((FlashPatternType)value)
                {
                    case FlashPatternType.FlashPatternCustom:
                        // Make sure the list isn't empty
                        if (_flashPatternService.Items.Count < 1)
                        {
                            _flashPatternService.LoadPattern(FlashPatternType.FlashPatternCustom);
                        }
                        FlashPatternListVisible = true;
                        break;

                    case FlashPatternType.FlashPatternTypeNone:
                    case FlashPatternType.FlashPatternMUTCD:
                    case FlashPatternType.FlashPatternWigWag:
                    default:
                        FlashPatternListVisible = false;
                        break;
                }

                _lightConfigService.LightConfig.FlashPatternIndex = value;
                _stateContainerService.NotifyStateChanged();
            }
        }

        public IDelegateCommand UploadAllSettingsCommand { get; set; }
        public IDelegateCommand DownloadAllSettingsCommand { get; set; }
        public IDelegateCommand UploadFlashPatternCommand { get; set; }
        public IDelegateCommand DownloadFlashPatternCommand { get; set; }

    }
}
