using Leapfrog.Application.Interfaces;

namespace Leapfrog.Infrastructure.ViewModels
{
    public class SecurityViewModel
    {
        private readonly IDelegateCommandService _delegateCommandService;
        private readonly IStateContainerService _stateContainerService;
        private readonly ISecurityModelService _securityModelService;

        public SecurityViewModel(IDelegateCommandService delegateCommandService, IStateContainerService stateContainerService, ISecurityModelService securityModelService)
        {
            _delegateCommandService=delegateCommandService;
            _stateContainerService=stateContainerService;
            _securityModelService=securityModelService;

            // Initialize the security model
            _securityModelService.Init();

            EnableLibraryCommand = _delegateCommandService.Create(EnableLibrary);
            UploadAllSettingsCommand = _delegateCommandService.Create(UploadAllSettings);
            DownloadAllSettingsCommand = _delegateCommandService.Create(DownloadAllSettings);

        }

        public IDelegateCommand EnableLibraryCommand { get; set; }
        public IDelegateCommand UploadAllSettingsCommand { get; set; }
        public IDelegateCommand DownloadAllSettingsCommand { get; set; }

        public string DeviceID => _securityModelService.SecurityModel.DeviceID;

        public string Enabled => _securityModelService.SecurityModel.Enabled;

        public string Valid => _securityModelService.SecurityModel.ValidString;

        public string Password
        {
            get => _securityModelService.SecurityModel.Password;
            set
            {
                if (_securityModelService.SecurityModel.Password != value)
                {
                    TrackUnsavedField(nameof(Password), value, "Password");
                    _securityModelService.SecurityModel.Password = value;
                    _stateContainerService.NotifyStateChanged();
                }
            }
        }

        private async Task EnableLibrary()
        {
            // Set the new values
            await _securityModelService.GenerateKey();

            // Update to the values stored on the device
            await _securityModelService.GetSecurityInfo();

            // Update the UI with the values from the device
            Refresh();
        }


        private async Task UploadAllSettings()
        {
            // Set the new values
            await _securityModelService.SetSecurityInfo();

            // Update to the values stored on the device
            await _securityModelService.GetSecurityInfo();

            // Update the UI with the values from the device
            Refresh();
        }

        private async Task DownloadAllSettings()
        {
            // Update to the values stored on the device
            await _securityModelService.GetSecurityInfo();

            // Update the UI with the values from the device
            Refresh();
        }

        public void Refresh()
        {
            _stateContainerService.NotifyStateChanged();
        }

        private void TrackUnsavedField(string fieldName, string fieldValue, string labelName)
        {
            // Implement tracking of unsaved fields if needed
        }


    }
}
