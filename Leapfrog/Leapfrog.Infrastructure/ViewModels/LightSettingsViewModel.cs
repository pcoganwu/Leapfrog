using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Enums.LightConfigEnums;
using System.Collections.ObjectModel;

namespace Leapfrog.Infrastructure.ViewModels
{
    public class LightSettingsViewModel
    {
        private readonly IDelegateCommandService _delegateCommandService;
        private readonly IStateContainerService _stateContainerService;
        private readonly ILightConfigService _lightConfigService;

        public LightSettingsViewModel(IDelegateCommandService delegateCommandService,IStateContainerService stateContainerService, ILightConfigService lightConfigService)
        {
            _delegateCommandService=delegateCommandService;
            _stateContainerService=stateContainerService;
            _lightConfigService=lightConfigService;

            DimmerList = _lightConfigService.LightConfig.DimmerSettings;
            LightTypeList = _lightConfigService.LightConfig.LightTypeSettings;
            ButtonTypeList = _lightConfigService.LightConfig.ButtonTypeSettings;
            ButtonActionList = _lightConfigService.LightConfig.ButtonActionSettings;
            FlashPatternList = _lightConfigService.LightConfig.FlashPatternSettings;

            UploadAllSettingsCommand = _delegateCommandService.Create(UploadAllSettings);
            DownloadAllSettingsCommand = _delegateCommandService.Create(DownloadAllSettings);
            AllLeftCommand = _delegateCommandService.Create(AllLeft);
            DimmerLeftCommand = _delegateCommandService.Create(DimmerLeft);
            FlashLengthLeftCommand = _delegateCommandService.Create(FlashLengthLeft);
            LightTypeLeftCommand = _delegateCommandService.Create(LightTypeLeft);
            ButtonTypeLeftCommand = _delegateCommandService.Create(ButtonTypeLeft);
            ButtonActionLeftCommand = _delegateCommandService.Create(ButtonActionLeft);
            FlashDelayLeftCommand = _delegateCommandService.Create(FlashDelayLeft);
            FlashPatternLeftCommand = _delegateCommandService.Create(FlashPatternLeft);
        }

        public IDelegateCommand UploadAllSettingsCommand { get; set; }
        public IDelegateCommand DownloadAllSettingsCommand { get; set; }
        public IDelegateCommand AllLeftCommand { get; set; }
        public IDelegateCommand DimmerLeftCommand { get; set; }
        public IDelegateCommand FlashLengthLeftCommand { get; set; }
        public IDelegateCommand LightTypeLeftCommand { get; set; }
        public IDelegateCommand ButtonTypeLeftCommand { get; set; }
        public IDelegateCommand ButtonActionLeftCommand { get; set; }
        public IDelegateCommand FlashDelayLeftCommand { get; set; }
        public IDelegateCommand FlashPatternLeftCommand { get; set; }

        public ObservableCollection<string> DimmerList { get; set; }
        public ObservableCollection<string> LightTypeList { get; set; }
        public ObservableCollection<string> ButtonTypeList { get; set; }
        public ObservableCollection<string> ButtonActionList { get; set; }
        public ObservableCollection<string> FlashPatternList { get; set; }


        private async Task UploadAllSettings()
        {
            await _lightConfigService.UploadAll();
            _stateContainerService.NotifyStateChanged();
        }

        private async Task DownloadAllSettings()
        {
            //await _lightConfigService.GetDownloadCommand();
            _stateContainerService.NotifyStateChanged();
        }

        private Task AllLeft()
        {
            _lightConfigService.SetValue(LightConfigType.ConfigDimmer);
            _lightConfigService.SetValue(LightConfigType.ConfigFlashLength);
            _lightConfigService.SetValue(LightConfigType.ConfigLightType);
            _lightConfigService.SetValue(LightConfigType.ConfigButtonType);
            _lightConfigService.SetValue(LightConfigType.ConfigButtonAction);
            _lightConfigService.SetValue(LightConfigType.ConfigFlashDelay);
            _lightConfigService.SetValue(LightConfigType.ConfigFlashPattern);
            _stateContainerService.NotifyStateChanged();

            return Task.CompletedTask;
        }

        private Task DimmerLeft()
        {
            _lightConfigService.SetValue(LightConfigType.ConfigDimmer);
            _stateContainerService.NotifyStateChanged();

            return Task.CompletedTask;
        }

        private Task FlashLengthLeft()
        {
            _lightConfigService.SetValue(LightConfigType.ConfigFlashLength);
            _stateContainerService.NotifyStateChanged();

            return Task.CompletedTask;
        }

        private Task LightTypeLeft()
        {
            _lightConfigService.SetValue(LightConfigType.ConfigLightType);
            _stateContainerService.NotifyStateChanged();

            return Task.CompletedTask;
        }

        private Task ButtonTypeLeft()
        {
            _lightConfigService.SetValue(LightConfigType.ConfigButtonType);
            _stateContainerService.NotifyStateChanged();

            return Task.CompletedTask;
        }

        private Task ButtonActionLeft()
        {
            _lightConfigService.SetValue(LightConfigType.ConfigButtonAction);
            _stateContainerService.NotifyStateChanged();

            return Task.CompletedTask;
        }

        private Task FlashDelayLeft()
        {
            _lightConfigService.SetValue(LightConfigType.ConfigFlashDelay);
            _stateContainerService.NotifyStateChanged();

            return Task.CompletedTask;
        }

        private Task FlashPatternLeft()
        {
            _lightConfigService.SetValue(LightConfigType.ConfigFlashPattern);
            _stateContainerService.NotifyStateChanged();

            return Task.CompletedTask;
        }

        public void Refresh()
        {
            _stateContainerService.NotifyStateChanged();
        }

    }
}
