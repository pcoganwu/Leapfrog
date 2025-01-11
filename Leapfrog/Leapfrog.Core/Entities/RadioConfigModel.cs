using Leapfrog.Core.Enums.RadioConfigEnums;
using System.Collections.ObjectModel;

namespace Leapfrog.Core.Entities
{
    public class RadioConfigModel
    {
        public const int NEVER_SET = -1;

        public const double START_FREQ = 902;
        public const double END_FREQ = 928;
        public const double FREQUENCY_MULTIPLIER = 1000000;

        public const int MIN_TX_POWER = 0;
        public const int MAX_TX_POWER = 22;

        public const int MIN_PREAMBLE_LEN = 4;
        public const int MAX_PREAMBLE_LEN = 20;

        public const int MIN_TX_TIMEOUT = 500;
        public const int MAX_TX_TIMEOUT = 20000;

        public const int OFFSET_SPREADING_FACTOR = 6;
        public const int OFFSET_CODING = 1;

        public const int NUM_VALUES = 15;

        public ObservableCollection<string> Channels { get; set; } = new ObservableCollection<string>();

        public int ChannelIndex { get; set; }
        public int ChannelIndexDevice { get; set; } = NEVER_SET;
        public string ChannelDevice => ChannelIndexDevice == NEVER_SET ? "Unknown" : Channels.ElementAtOrDefault(ChannelIndexDevice) ?? "";

        public string Freq { get; set; } = string.Empty;
        public int CurrentFreq { get; set; }
        public int DefaultFreq { get; set; } = (int)(START_FREQ * FREQUENCY_MULTIPLIER);
        public ConfigStatus FreqStatus { get; set; } = ConfigStatus.VALID;
        public int FreqDevice { get; set; } = NEVER_SET;
        public string FreqDeviceString => FreqDevice == NEVER_SET ? "Unknown" : FreqDevice.ToString();
        public readonly string MinFreq = START_FREQ.ToString();
        public readonly string MaxFreq = END_FREQ.ToString();

        public ObservableCollection<string> Bandwidths { get; set; } = new ObservableCollection<string>();
        public BandwidthSetting Bandwidth { get; set; }
        public int BandwidthIndex { get; set; }
        public BandwidthSetting DefaultBandwidth { get; set; } = BandwidthSetting.BAND_500KHZ;
        public int BandwidthDevice { get; set; } = NEVER_SET;
        public string BandwidthDeviceString => BandwidthDevice == NEVER_SET ? "Unknown" : Bandwidths.ElementAtOrDefault(BandwidthDevice) ?? "";

        public string TxPower { get; set; } = string.Empty;
        public int CurrentTxPower { get; set; }
        public int DefaultTxPower { get; set; } = 14;
        public ConfigStatus TxPowerStatus { get; set; } = ConfigStatus.VALID;
        public int TxPowerDevice { get; set; } = NEVER_SET;
        public string TxPowerDeviceString => TxPowerDevice == NEVER_SET ? "Unknown" : TxPowerDevice.ToString();

        public ObservableCollection<string> SpreadingFactors { get; set; } = new ObservableCollection<string>();
        public int SpreadingFactor { get; set; }
        public int SpreadingFactorIndex { get; set; }
        public int DefaultSpreadingFactor { get; set; } = 1;
        public int SpreadingFactorDevice { get; set; } = NEVER_SET;
        public string SpreadingFactorDeviceString => SpreadingFactorDevice == NEVER_SET ? "Unknown" : SpreadingFactors.ElementAtOrDefault(SpreadingFactorDevice) ?? "";

        public ObservableCollection<string> Codings { get; set; } = new ObservableCollection<string>();
        public int Coding { get; set; }
        public int CodingIndex { get; set; }
        public int DefaultCoding { get; set; } = 0;
        public int CodingDevice { get; set; } = NEVER_SET;
        public string CodingDeviceString => CodingDevice == NEVER_SET ? "Unknown" : Codings.ElementAtOrDefault(CodingDevice) ?? "";

        public string PreambleLen { get; set; } = string.Empty;
        public int CurrentPreambleLen { get; set; }
        public int DefaultPreambleLen { get; set; } = 8;
        public ConfigStatus PreambleLenStatus { get; set; } = ConfigStatus.VALID;
        public int PreambleLenDevice { get; set; } = NEVER_SET;
        public string PreambleLenDeviceString => PreambleLenDevice == NEVER_SET ? "Unknown" : PreambleLenDevice.ToString();

        public ObservableCollection<string> IqInversions { get; set; } = new ObservableCollection<string>();
        public IqInversionSetting IqInversionIndex { get; set; }
        public int IqInversionIndexValue { get; set; }
        public IqInversionSetting DefaultIqInversionIndex { get; set; } = IqInversionSetting.NOT_INVERTED;
        public int IqInversionDevice { get; set; } = NEVER_SET;
        public string IqInversionDeviceString => IqInversionDevice == NEVER_SET ? "Unknown" : IqInversions.ElementAtOrDefault(IqInversionDevice) ?? "";

        public string TxTimeout { get; set; } = string.Empty;
        public int CurrentTxTimeout { get; set; }
        public int DefaultTxTimeout { get; set; } = 3000;
        public ConfigStatus TxTimeoutStatus { get; set; } = ConfigStatus.VALID;
        public int TxTimeoutDevice { get; set; } = NEVER_SET;
        public string TxTimeoutDeviceString => TxTimeoutDevice == NEVER_SET ? "Unknown" : TxTimeoutDevice.ToString();
    }
}
