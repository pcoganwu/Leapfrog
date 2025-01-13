using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Constants;
using Leapfrog.Core.Entities;
using Leapfrog.Core.Enums.RadioConfigEnums;
using Leapfrog.Core.Enums.StatusEnums;

namespace Leapfrog.Infrastructure.Services
{
    public class RadioConfigService : IRadioConfigService
    {
        private readonly Lazy<ILoRaWANInterfaceService> _loRaWANInterfaceService;
        private readonly IStatusService _statusService;

        public RadioConfigService(Lazy<ILoRaWANInterfaceService> loRaWANInterfaceService, IStatusService statusService)
        {
            _loRaWANInterfaceService=loRaWANInterfaceService;
            _statusService=statusService;
        }

        public RadioConfigModel RadioConfig { get; set; } = new();

        public byte[] GetRadioConfigCommand()
        {
            return [(byte)'R', (byte)RadioConfigCmd.GET_DATA, 3];
        }

        public void Init()
        {
            RadioConfig.Bandwidths.Add("125KHz");
            RadioConfig.Bandwidths.Add("250KHz");
            RadioConfig.Bandwidths.Add("500KHz (default)");
            RadioConfig.Bandwidth = RadioConfig.DefaultBandwidth;

            MakeChannels();
            SetFrequency(RadioConfig.DefaultFreq);
            RadioConfig.Freq = RadioConfig.CurrentFreq.ToString();

            RadioConfig.CurrentTxPower = RadioConfig.DefaultTxPower;
            RadioConfig.TxPower = RadioConfig.CurrentTxPower.ToString();

            RadioConfig.SpreadingFactors.Add("6: 64 chirps");
            RadioConfig.SpreadingFactors.Add("7: 128 chirps (default)");
            RadioConfig.SpreadingFactors.Add("8: 256 chirps");
            RadioConfig.SpreadingFactors.Add("9: 512 chirps");
            RadioConfig.SpreadingFactors.Add("10: 1024 chirps");
            RadioConfig.SpreadingFactors.Add("11: 2048 chirps");
            RadioConfig.SpreadingFactors.Add("12: 4096 chirps");
            RadioConfig.SpreadingFactor = RadioConfig.DefaultSpreadingFactor;

            RadioConfig.Codings.Add("1: 4/5 (default)");
            RadioConfig.Codings.Add("2: 4/6");
            RadioConfig.Codings.Add("3: 4/7");
            RadioConfig.Codings.Add("4: 4/8");
            RadioConfig.Coding = RadioConfig.DefaultCoding;

            RadioConfig.CurrentPreambleLen = RadioConfig.DefaultPreambleLen;
            RadioConfig.PreambleLen = RadioConfig.CurrentPreambleLen.ToString();

            RadioConfig.IqInversions.Add("Not inverted (default)");
            RadioConfig.IqInversions.Add("Inverted");
            RadioConfig.IqInversionIndex = RadioConfig.DefaultIqInversionIndex;

            RadioConfig.CurrentTxTimeout = RadioConfig.DefaultTxTimeout;
            RadioConfig.TxTimeout = RadioConfig.CurrentTxTimeout.ToString();
        }

        public void ParsePacket(byte[] data)
        {
            if (data.Length < RadioConfigModel.NUM_VALUES * 4) return;

            int[] values = new int[RadioConfigModel.NUM_VALUES];
            for (int i = 0, index = 3; i < RadioConfigModel.NUM_VALUES; i++)
            {
                values[i] = BitConverter.ToInt32(data, index);
                index += 4;
            }

            RadioConfig.FreqDevice = values[(int)ConfigIndex.FREQ];
            RadioConfig.TxPowerDevice = values[(int)ConfigIndex.TX_POWER];
            RadioConfig.BandwidthDevice = values[(int)ConfigIndex.BANDWIDTH];
            RadioConfig.SpreadingFactorDevice = values[(int)ConfigIndex.SPREADING_FACTOR] - RadioConfigModel.OFFSET_SPREADING_FACTOR;
            RadioConfig.CodingDevice = values[(int)ConfigIndex.CODING] - RadioConfigModel.OFFSET_CODING;
            RadioConfig.PreambleLenDevice = values[(int)ConfigIndex.PREAMBLE_LEN];
            RadioConfig.IqInversionDevice = values[(int)ConfigIndex.IQ_INVERSION];
            RadioConfig.TxTimeoutDevice = values[(int)ConfigIndex.TX_TIMEOUT];

            double incr = GetBandwidthIncr();
            double freq = RadioConfig.FreqDevice / RadioConfigModel.FREQUENCY_MULTIPLIER - RadioConfigModel.START_FREQ;
            RadioConfig.ChannelIndexDevice = freq % incr != 0 ? RadioConfig.Channels.Count - 1 : (int)(freq / incr);

            _statusService.SetStatus(StringConstants.RETRIEVE_CONFIG_MESSAGE);
        }

        public async Task<bool> SetDefaultRadioConfig()
        {
            return await SetRadioConfigCommand(RadioConfig.DefaultFreq, RadioConfig.DefaultTxPower, (int)RadioConfig.DefaultBandwidth, RadioConfig.DefaultSpreadingFactor, RadioConfig.DefaultCoding, RadioConfig.DefaultPreambleLen, (int)RadioConfig.DefaultIqInversionIndex, RadioConfig.DefaultTxTimeout);
        }

        public async Task SetRadioConfig()
        {
            if (RadioConfig.FreqStatus == ConfigStatus.INVALID || RadioConfig.PreambleLenStatus == ConfigStatus.INVALID || RadioConfig.TxPowerStatus == ConfigStatus.INVALID || RadioConfig.TxTimeoutStatus == ConfigStatus.INVALID)
            {
                _statusService.SetStatus(StatusType.ERROR, "Request Failed: Please correct values highlighted in red");
                return;
            }

            await SetRadioConfigCommand(RadioConfig.CurrentFreq, RadioConfig.CurrentTxPower, (int)RadioConfig.Bandwidth, RadioConfig.SpreadingFactor, RadioConfig.Coding, RadioConfig.CurrentPreambleLen, (int)RadioConfig.IqInversionIndex, RadioConfig.CurrentTxTimeout);
        }

        public void SetValue(RadioConfigSetting setting)
        {
            switch (setting)
            {
                case RadioConfigSetting.CHANNEL:
                    if (RadioConfig.ChannelIndexDevice != RadioConfigModel.NEVER_SET) RadioConfig.ChannelIndex = RadioConfig.ChannelIndexDevice;
                    break;

                case RadioConfigSetting.FREQUENCY:
                    if (RadioConfig.FreqDevice != RadioConfigModel.NEVER_SET) RadioConfig.Freq = RadioConfig.FreqDevice.ToString();
                    break;

                case RadioConfigSetting.BANDWIDTH:
                    if (RadioConfig.BandwidthDevice != RadioConfigModel.NEVER_SET) RadioConfig.BandwidthIndex = RadioConfig.BandwidthDevice;
                    break;

                case RadioConfigSetting.TX_POWER:
                    if (RadioConfig.TxPowerDevice != RadioConfigModel.NEVER_SET) RadioConfig.TxPower = RadioConfig.TxPowerDevice.ToString();
                    break;

                case RadioConfigSetting.SPREADING_FACTOR:
                    if (RadioConfig.SpreadingFactorDevice != RadioConfigModel.NEVER_SET) RadioConfig.SpreadingFactor = RadioConfig.SpreadingFactorDevice;
                    break;

                case RadioConfigSetting.CODING:
                    if (RadioConfig.CodingDevice != RadioConfigModel.NEVER_SET) RadioConfig.Coding = RadioConfig.CodingDevice;
                    break;

                case RadioConfigSetting.PREAMBLE_LENGTH:
                    if (RadioConfig.PreambleLenDevice != RadioConfigModel.NEVER_SET) RadioConfig.PreambleLen = RadioConfig.PreambleLenDevice.ToString();
                    break;

                case RadioConfigSetting.IQ_INVERSION:
                    if (RadioConfig.IqInversionDevice != RadioConfigModel.NEVER_SET) RadioConfig.IqInversionIndex = (IqInversionSetting)RadioConfig.IqInversionDevice;
                    break;

                case RadioConfigSetting.TX_TIMEOUT:
                    if (RadioConfig.TxTimeoutDevice != RadioConfigModel.NEVER_SET) RadioConfig.TxTimeout = RadioConfig.TxTimeoutDevice.ToString();
                    break;
            }
        }

        private void MakeChannels()
        {
            double chFreq = RadioConfigModel.START_FREQ;
            double incr = GetBandwidthIncr();
            int index = 0;

            RadioConfig.Channels.Clear();

            while (chFreq < RadioConfigModel.END_FREQ)
            {
                RadioConfig.Channels.Add($"{index}: {chFreq}MHz");
                index++;
                chFreq += incr;
            }

            RadioConfig.Channels.Add("Custom Frequency");
            RadioConfig.Channels[0] += " (default)";
        }

        private void SetFrequency(int freq)
        {
            double chFreq = freq / RadioConfigModel.FREQUENCY_MULTIPLIER - RadioConfigModel.START_FREQ;
            double incr = GetBandwidthIncr();

            RadioConfig.ChannelIndex = chFreq % incr != 0 ? RadioConfig.Channels.Count - 1 : (int)(chFreq / incr);

            if (RadioConfig.ChannelIndexDevice != RadioConfigModel.NEVER_SET)
            {
                chFreq = RadioConfig.FreqDevice / RadioConfigModel.FREQUENCY_MULTIPLIER - RadioConfigModel.START_FREQ;
                RadioConfig.ChannelIndexDevice = chFreq % incr != 0 ? RadioConfig.Channels.Count - 1 : (int)(chFreq / incr);
            }

            RadioConfig.CurrentFreq = freq;
            RadioConfig.Freq = RadioConfig.CurrentFreq.ToString();
        }

        private double GetBandwidthIncr()
        {
            return RadioConfig.Bandwidth switch
            {
                BandwidthSetting.BAND_125KHZ => 0.125,
                BandwidthSetting.BAND_250KHZ => 0.250,
                _ => 0.5,
            };
        }

        private async Task<bool> SetRadioConfigCommand(int freqSetting, int txPowerSetting, int bandwidthSetting, int spreadingFactorSetting, int codingSetting, int preambleLenSetting, int iqInversionSetting, int txTimeoutSetting)
        {
            int[] values = new int[RadioConfigModel.NUM_VALUES];
            values[(int)ConfigIndex.FREQ] = freqSetting;
            values[(int)ConfigIndex.TX_POWER] = txPowerSetting;
            values[(int)ConfigIndex.BANDWIDTH] = bandwidthSetting;
            values[(int)ConfigIndex.SPREADING_FACTOR] = spreadingFactorSetting + RadioConfigModel.OFFSET_SPREADING_FACTOR;
            values[(int)ConfigIndex.CODING] = codingSetting + RadioConfigModel.OFFSET_CODING;
            values[(int)ConfigIndex.PREAMBLE_LEN] = preambleLenSetting;
            values[(int)ConfigIndex.IQ_INVERSION] = iqInversionSetting;
            values[(int)ConfigIndex.TX_TIMEOUT] = txTimeoutSetting;

            List<byte> cmd = new List<byte> { (byte)'R', (byte)RadioConfigCmd.SET_DATA, (byte)(3 + 4 * RadioConfigModel.NUM_VALUES) };
            foreach (int value in values)
            {
                cmd.AddRange(BitConverter.GetBytes(value));
            }

            _statusService.SetStatus("Radio config updated on device");

            return await _loRaWANInterfaceService.Value.SendRawCommand(cmd.ToArray());
        }
    }
}
