using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Entities;
using Leapfrog.Core.Enums.RadioConfigEnums;
using System.Collections.ObjectModel;

namespace Leapfrog.Infrastructure.ViewModels
{
    public class RadioConfigViewModel
    {
        private readonly IRadioConfigService _radioConfigService;
        private readonly IDelegateCommandService _delegateCommandService;
        private readonly IStateContainerService _stateContainerService;
        private readonly ILoRaWANInterfaceService _loRaWANInterfaceService;

        public RadioConfigViewModel(IRadioConfigService radioConfigService, IDelegateCommandService delegateCommandService, 
            IStateContainerService stateContainerService, ILoRaWANInterfaceService loRaWANInterfaceService)
        {
            _radioConfigService=radioConfigService;
            _delegateCommandService=delegateCommandService;
            _stateContainerService=stateContainerService;
            _loRaWANInterfaceService=loRaWANInterfaceService;

            // Initialize our config
            _radioConfigService.Init();

            ChannelList = RadioConfigModel.Channels;
            BandwidthList = RadioConfigModel.Bandwidths;
            SpreadingFactorList = RadioConfigModel.SpreadingFactors;
            CodingList = RadioConfigModel.Codings;
            IqInversionList = RadioConfigModel.IqInversions;

            UploadAllSettingsCommand = _delegateCommandService.Create(() =>
            {
                // Copy value to the left
                _radioConfigService.SetRadioConfig();

                // Update the UI with the values from the device
                Refresh();
                return Task.CompletedTask;
            });

            AllLeftCommand = _delegateCommandService.Create(() =>
            {
                // Copy values to the left
                _radioConfigService.SetValue(RadioConfigSetting.CHANNEL);
                _radioConfigService.SetValue(RadioConfigSetting.FREQUENCY);
                _radioConfigService.SetValue(RadioConfigSetting.TX_POWER);
                _radioConfigService.SetValue(RadioConfigSetting.BANDWIDTH);
                _radioConfigService.SetValue(RadioConfigSetting.SPREADING_FACTOR);
                _radioConfigService.SetValue(RadioConfigSetting.CODING);
                _radioConfigService.SetValue(RadioConfigSetting.PREAMBLE_LENGTH);
                _radioConfigService.SetValue(RadioConfigSetting.IQ_INVERSION);
                _radioConfigService.SetValue(RadioConfigSetting.TX_TIMEOUT);

                // Update the UI with the values from the device
                Refresh();
                return Task.CompletedTask;
            });

            DownloadAllSettingsCommand = _delegateCommandService.Create(async () =>
            {
                // Copy value to the left
                await _loRaWANInterfaceService!.RefreshAllConfigurations();
                // Update the UI with the values from the device
                Refresh();
            });

            ChannelLeftCommand = _delegateCommandService.Create(() =>
            {
                // Copy value to the left
                _radioConfigService.SetValue(RadioConfigSetting.CHANNEL);

                // Update the UI with the values from the device
                Refresh();
                return Task.CompletedTask;

            });

            FreqLeftCommand = _delegateCommandService.Create(() =>
            {
                // Copy value to the left
                _radioConfigService.SetValue(RadioConfigSetting.FREQUENCY);

                // Update the UI with the values from the device
                Refresh();
                return Task.CompletedTask;

            });

            TxPowerLeftCommand = _delegateCommandService.Create(() =>
            {
                // Copy value to the left
                _radioConfigService.SetValue(RadioConfigSetting.TX_POWER);

                // Update the UI with the values from the device
                Refresh();
                return Task.CompletedTask;

            });

            BandwidthLeftCommand = _delegateCommandService.Create(() =>
            {
                // Copy value to the left
                _radioConfigService.SetValue(RadioConfigSetting.BANDWIDTH);

                // Update the UI with the values from the device
                Refresh();
                return Task.CompletedTask;

            });

            SpreadingFactorLeftCommand = _delegateCommandService.Create(() =>
            {
                // Copy value to the left
                _radioConfigService.SetValue(RadioConfigSetting.SPREADING_FACTOR);

                // Update the UI with the values from the device
                Refresh();
                return Task.CompletedTask;

            });

            CodingLeftCommand = _delegateCommandService.Create(() =>
            {
                // Copy value to the left
                _radioConfigService.SetValue(RadioConfigSetting.CODING);

                // Update the UI with the values from the device
                Refresh();
                return Task.CompletedTask;

            });

            PreambleLenLeftCommand = _delegateCommandService.Create(() =>
            {
                // Copy value to the left
                _radioConfigService.SetValue(RadioConfigSetting.PREAMBLE_LENGTH);

                // Update the UI with the values from the device
                Refresh();
                return Task.CompletedTask;

            });

            IqInversionLeftCommand = _delegateCommandService.Create(() =>
            {
                // Copy value to the left
                _radioConfigService.SetValue(RadioConfigSetting.IQ_INVERSION);

                // Update the UI with the values from the device
                Refresh();
                return Task.CompletedTask;

            });

            TxTimeoutLeftCommand = _delegateCommandService.Create(() =>
            {
                // Copy value to the left
                _radioConfigService.SetValue(RadioConfigSetting.TX_TIMEOUT);

                // Update the UI with the values from the device
                Refresh();
                return Task.CompletedTask;

            });
        }

        public RadioConfigModel RadioConfigModel { get; set; } = new();

        public IDelegateCommand UploadAllSettingsCommand { get; set; }
        public IDelegateCommand AllLeftCommand { get; set; }
        public IDelegateCommand DownloadAllSettingsCommand { get; set; }
        public IDelegateCommand ChannelLeftCommand { get; set; }
        public IDelegateCommand FreqLeftCommand { get; set; }
        public IDelegateCommand TxPowerLeftCommand { get; set; }
        public IDelegateCommand BandwidthLeftCommand { get; set; }
        public IDelegateCommand SpreadingFactorLeftCommand { get; set; }
        public IDelegateCommand CodingLeftCommand { get; set; }
        public IDelegateCommand PreambleLenLeftCommand { get; set; }
        public IDelegateCommand IqInversionLeftCommand { get; set; }
        public IDelegateCommand TxTimeoutLeftCommand { get; set; }

        public ObservableCollection<string> ChannelList { get; set; }

        public int ChannelIndex
        {
            get
            {
                return RadioConfigModel.ChannelIndex;
            }
            set
            {
                if (RadioConfigModel.ChannelIndex != value)
                {
                    //TrackUnsavedField(nameof(ChannelIndex), value.ToString(), "Channel");
                    RadioConfigModel.ChannelIndex = value;
                    Refresh();
                }
            }
        }

        public string ChannelDevice => RadioConfigModel.ChannelDevice;

        public string FrequencyTitle => $"Frequency ({RadioConfigModel.MinFreq} - {RadioConfigModel.MaxFreq}MHz)";

        public string Frequency
        {
            get
            {
                return RadioConfigModel.Freq;
            }
            set
            {
                if (RadioConfigModel.Freq != value)
                {
                    //TrackUnsavedField(nameof(Frequency), value.ToString(), "Frequency");
                    RadioConfigModel.Freq = value;
                    _stateContainerService.NotifyStateChanged();
                }
            }
        }

        public string FrequencyDevice => RadioConfigModel.FreqDeviceString;

        public string TxPower
        {
            get
            {
                return RadioConfigModel.TxPower;
            }
            set
            {
                if (RadioConfigModel.TxPower != value)
                {
                    //TrackUnsavedField(nameof(TxPower), value.ToString(), "TX Power");
                    RadioConfigModel.TxPower = value;
                    _stateContainerService.NotifyStateChanged();
                }
            }
        }

        public string TxPowerDevice => RadioConfigModel.TxPowerDeviceString;

        public ObservableCollection<string> BandwidthList { get; set; }

        public int BandwidthIndex
        {
            get
            {
                return RadioConfigModel.BandwidthIndex;
            }
            set
            {
                if (RadioConfigModel.BandwidthIndex != value)
                {
                    //TrackUnsavedField(nameof(BandwidthIndex), value.ToString(), "Bandwidth");
                    RadioConfigModel.BandwidthIndex = value;
                    Refresh();
                }
            }
        }

        public string BandwidthDevice => RadioConfigModel.BandwidthDeviceString;

        public ObservableCollection<string> SpreadingFactorList { get; set; }

        public int SpreadingFactorIndex
        {
            get
            {
                return RadioConfigModel.SpreadingFactorIndex;
            }
            set
            {
                if (RadioConfigModel.SpreadingFactorIndex != value)
                {
                    //TrackUnsavedField(nameof(SpreadingFactorIndex), value.ToString(), "Spreading Factor");
                    RadioConfigModel.SpreadingFactorIndex = value;
                    Refresh();
                }
            }
        }

        public string SpreadingFactorDevice => RadioConfigModel.SpreadingFactorDeviceString;

        public ObservableCollection<string> CodingList { get; set; }

        public int CodingIndex
        {
            get
            {
                return RadioConfigModel.CodingIndex;
            }
            set
            {
                if (RadioConfigModel.CodingIndex != value)
                {
                    //TrackUnsavedField(nameof(CodingIndex), value.ToString(), "Coding Rate");
                    RadioConfigModel.CodingIndex = value;
                    Refresh();
                }
            }
        }

        public string CodingDevice => RadioConfigModel.CodingDeviceString;

        public string PreambleLen
        {
            get
            {
                return RadioConfigModel.PreambleLen;
            }
            set
            {
                if (RadioConfigModel.PreambleLen != value)
                {
                    //TrackUnsavedField(nameof(PreambleLen), value.ToString(), "Preamble Length");
                    RadioConfigModel.PreambleLen = value;
                    _stateContainerService.NotifyStateChanged();
                }
            }
        }

        public string PreambleLenDevice => RadioConfigModel.PreambleLenDeviceString;

        public ObservableCollection<string> IqInversionList { get; set; }

        public int IqInversionIndex
        {
            get
            {
                return RadioConfigModel.IqInversionIndexValue;
            }
            set
            {
                if (RadioConfigModel.IqInversionIndexValue != value)
                {
                    //TrackUnsavedField(nameof(IqInversionIndex), value.ToString(), "IQ Inversion");
                    RadioConfigModel.IqInversionIndexValue = value;
                    Refresh();
                }
            }
        }

        public string IqInversionDevice => RadioConfigModel.IqInversionDeviceString;

        public string TxTimeout
        {
            get
            {
                return RadioConfigModel.TxTimeout;
            }
            set
            {
                if (RadioConfigModel.TxTimeout != value)
                {
                    //TrackUnsavedField(nameof(TxTimeout), value.ToString(), "TX Timeout");
                    RadioConfigModel.TxTimeout = value;
                    _stateContainerService.NotifyStateChanged();
                }
            }
        }

        public string TxTimeoutDevice => RadioConfigModel.TxTimeoutDeviceString;

        public void Refresh()
        {
            _stateContainerService.NotifyStateChanged();
        }

    }
}
