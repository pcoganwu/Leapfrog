using Leapfrog.Application.Interfaces;
using System.Collections.ObjectModel;

namespace Leapfrog.Infrastructure.ViewModels
{
    public class LoaderViewModel
    {
        private readonly IDelegateCommandService _delegateCommandService;
        private readonly IStateContainerService _stateContainerService;
        private readonly ILoaderModelService _loaderModelService;

        public LoaderViewModel(IDelegateCommandService delegateCommandService, IStateContainerService stateContainerService, ILoaderModelService loaderModelService)
        {
            _delegateCommandService=delegateCommandService;
            _stateContainerService=stateContainerService;
            _loaderModelService=loaderModelService;

            _loaderModelService.LoadFirmwareList();
            FirmwareUpgrade = _loaderModelService.LoaderModel.FirmwareUpgrade;

            UploadFirmwareCommand = _delegateCommandService.Create(UploadFirmware);
            GenerateFirmwareCommand = _delegateCommandService.Create(GenerateFirmware);
            GetVersionCommand = _delegateCommandService.Create(GetVersion);

        }

        public IDelegateCommand UploadFirmwareCommand { get; set; }
        public IDelegateCommand GenerateFirmwareCommand { get; set; }
        public IDelegateCommand GetVersionCommand { get; set; }

        public ObservableCollection<string> FirmwareUpgrade { get; set; }

        public int FirmwareUpgradeIndex
        {
            get => _loaderModelService.LoaderModel.FirmwareUpgradeIndex;
            set
            {
                _loaderModelService.LoaderModel.FirmwareUpgradeIndex = value;
                _stateContainerService.NotifyStateChanged();
            }
        }

        public int VersionMaj
        {
            get => _loaderModelService.LoaderModel.FirmwareVersionMaj;
            set
            {
                _loaderModelService.LoaderModel.FirmwareVersionMaj = value;
                _stateContainerService.NotifyStateChanged();
            }
        }

        public int VersionMin
        {
            get => _loaderModelService.LoaderModel.FirmwareVersionMin;
            set
            {
                _loaderModelService.LoaderModel.FirmwareVersionMin = value;
                _stateContainerService.NotifyStateChanged();
            }
        }

        public int VersionRev
        {
            get => _loaderModelService.LoaderModel.FirmwareVersionRev;
            set
            {
                _loaderModelService.LoaderModel.FirmwareVersionRev = value;
                _stateContainerService.NotifyStateChanged();
            }
        }

        public string FirmwareVersion => _loaderModelService.LoaderModel.DeviceFirmwareVersion;

        private bool _advancedSettingsVisible = false;
        public bool AdvancedSettingsVisible
        {
            get => _advancedSettingsVisible;
            set
            {
                _advancedSettingsVisible = value;
                _stateContainerService.NotifyStateChanged();
            }
        }

        public void ShowAdvancedSettings()
        {
            AdvancedSettingsVisible = true;
        }

        public void HideAdvancedSettings()
        {
            AdvancedSettingsVisible = false;
        }

        private async Task UploadFirmware()
        {
            await _loaderModelService.UploadFirmware();
            _stateContainerService.NotifyStateChanged();
        }

        private async Task GenerateFirmware()
        {
            await _loaderModelService.GenerateFirmware();
            _stateContainerService.NotifyStateChanged();
        }

        private async Task GetVersion()
        {
            //await _loaderModelService.GetDeviceVersion();
            //await _loaderModelService.RefreshAllConfigurations();
            _stateContainerService.NotifyStateChanged();
        }


    }
}
