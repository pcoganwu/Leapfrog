using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Entities;
using Leapfrog.Interfaces.Services;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Leapfrog.Infrastructure.ViewModels
{
    public class MainViewModel
    {
        private readonly IDelegateCommandService _delegateCommandService;
        private readonly IFlashPatternService _flashPatternService;
        private readonly ILightConfigService _lightConfigService;
        private readonly ILoaderModelService _loaderModelService;
        private readonly ILoRaWANInterfaceService _loRaWANInterfaceService;
        private readonly IRadioConfigService _radioConfigService;
        private readonly ISecurityModelService _securityModelService;
        private readonly IStateContainerService _stateContainerService;
        private readonly IStatusService _statusService;
        private readonly IUtilsModelService _utilsModelService;

        public MainViewModel(IDelegateCommandService delegateCommandService, IFlashPatternService flashPatternService, 
            ILightConfigService lightConfigService, ILoaderModelService loaderModelService,
            ILoRaWANInterfaceService loRaWANInterfaceService, IRadioConfigService radioConfigService,
            ISecurityModelService securityModelService, IStateContainerService stateContainerService,
            IStatusService statusService, IUtilsModelService utilsModelService)
        {
            _delegateCommandService=delegateCommandService;
            _flashPatternService=flashPatternService;
            _lightConfigService=lightConfigService;
            _loaderModelService=loaderModelService;
            _loRaWANInterfaceService=loRaWANInterfaceService;
            _radioConfigService=radioConfigService;
            _securityModelService=securityModelService;
            _stateContainerService=stateContainerService;
            _statusService=statusService;
            _utilsModelService=utilsModelService;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            _versionNumber = version != null ? $"v{version.Major}.{version.Minor}.{version.Build}.{version.Revision}" : "Unknown Version";

            LightSettingsVM = new LightSettingsViewModel(_delegateCommandService, _stateContainerService, _lightConfigService);
            FlashPatternVM = new FlashPatternViewModel(_stateContainerService, _flashPatternService, _lightConfigService, _delegateCommandService);
            LoaderVM = new LoaderViewModel(_delegateCommandService, _stateContainerService, _loaderModelService);
            SecurityVM = new SecurityViewModel(_delegateCommandService, _stateContainerService, _securityModelService);
            RadioConfigVM = new RadioConfigViewModel(_radioConfigService,_delegateCommandService, _stateContainerService, _loRaWANInterfaceService);

            BlazorComponent = LightSettingsVM;

            _loRaWANInterfaceService.RefreshEndpoints();

            LightSettingsViewCommand = _delegateCommandService.Create(() =>
            {
                BlazorComponent = LightSettingsVM;
                return Task.CompletedTask;
            });

            FlashPatternViewCommand = _delegateCommandService.Create(() =>
            {
                BlazorComponent = FlashPatternVM;
                return Task.CompletedTask;
            });

            LoaderViewCommand = _delegateCommandService.Create(() =>
            {
                BlazorComponent = LoaderVM;
                return Task.CompletedTask;
            });

            SecurityViewCommand = _delegateCommandService.Create(() =>
            {
                BlazorComponent = SecurityVM;
                return Task.CompletedTask;
            });

            RadioConfigViewCommand = _delegateCommandService.Create(() =>
            {
                BlazorComponent = RadioConfigVM;
                return Task.CompletedTask;
            });

            RequestFlashCommand = _delegateCommandService.Create(() =>
            {
                _utilsModelService.FlashLights();

                return Task.CompletedTask;
            });

            RestoreDefaultsCommand = _delegateCommandService.Create(async () =>
            {
                bool result;

                result = await _lightConfigService.UploadDefault();
                if (result == false)
                {
                    return;
                }
                LightSettingsVM.Refresh();

                result = await _securityModelService.SetDefaualtSecurityInfo();
                if (result == false)
                {
                    return;
                }

                result = await _securityModelService.GenerateKey();
                if (result == false)
                {
                    return;
                }
                SecurityVM.Refresh();

                result = await _radioConfigService.SetDefaultRadioConfig();
                if (result == false)
                {
                    return;
                }
                RadioConfigVM.Refresh();
            });


        }

        //public IDelegateCommand RefreshPortsCommand { get; set; }
        public IDelegateCommand LightSettingsViewCommand { get; set; }
        public IDelegateCommand FlashPatternViewCommand { get; set; }
        public IDelegateCommand LoaderViewCommand { get; set; }
        public IDelegateCommand SecurityViewCommand { get; set; }
        public IDelegateCommand RadioConfigViewCommand { get; set; }
        public IDelegateCommand RequestFlashCommand { get; set; }
        public IDelegateCommand RestoreDefaultsCommand { get; set; }
        //public IDelegateCommand HideAdvancedSettingsCommand { get; set; }
        //public IDelegateCommand ShowAdvancedSettingsCommand { get; set; }

        public FlashPatternViewModel FlashPatternVM { get; set; }
        public LightSettingsViewModel LightSettingsVM { get; set; }
        public LoaderViewModel LoaderVM { get; set; }
        public SecurityViewModel SecurityVM { get; set; }
        public RadioConfigViewModel RadioConfigVM { get; set; }

        //public ObservableCollection<string> EndPointList { get; set; }

        public object? BlazorComponent
        {
            get { return _blazorComponent; }
            set
            {
                _blazorComponent = value;
                _stateContainerService.NotifyStateChanged();
            }
        }
        private object? _blazorComponent;

        private string _versionNumber;

        public string VersionNumber
        {
            get { return _versionNumber; }
        }


    }
}
