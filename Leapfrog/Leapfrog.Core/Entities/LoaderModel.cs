using System.Collections.ObjectModel;

namespace Leapfrog.Core.Entities
{
    public class LoaderModel
    {
        public const int NEVER_SET = -1;
        public const int FW_LENGTH = 9;

        public ObservableCollection<string> FirmwareUpgrade { get; } = new ObservableCollection<string>();

        public int FirmwareUpgradeIndex { get; set; } = 0;

        // Firmware versions
        public int FirmwareVersionMaj { get; set; } = 1;
        public int FirmwareVersionMin { get; set; } = 0;
        public int FirmwareVersionRev { get; set; } = 2;

        public int FwMaj { get; set; } = NEVER_SET;
        public int FwMin { get; set; } = NEVER_SET;
        public int FwRev { get; set; } = NEVER_SET;

        // The version string from the device
        public string DeviceFirmwareVersion => FwMaj == NEVER_SET ? "Unknown" : $"{FwMaj}.{FwMin}.{FwRev}";
    }
}
