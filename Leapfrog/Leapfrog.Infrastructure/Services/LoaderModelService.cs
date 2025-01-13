using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Constants;
using Leapfrog.Core.Entities;
using Leapfrog.Core.Enums.LoaderEnums;
using Leapfrog.Core.Enums.StatusEnums;
using Leapfrog.Core.FirmwareImages;
using System.Diagnostics;

namespace Leapfrog.Infrastructure.Services
{
    public class LoaderModelService : ILoaderModelService
    {
        private readonly IFirmwareBase _firmwareBase;
        private readonly IFirmwareLoaderService _firmwareLoaderService;
        private readonly Lazy<ILoRaWANInterfaceService> _loRaWANInterfaceService;
        private readonly IStatusService _statusService;

        public LoaderModelService(IFirmwareBase firmwareBase, IFirmwareLoaderService firmwareLoaderService, Lazy<ILoRaWANInterfaceService> loRaWANInterfaceService, IStatusService statusService)
        {
           _firmwareBase=firmwareBase;
           _firmwareLoaderService=firmwareLoaderService;
           _loRaWANInterfaceService=loRaWANInterfaceService;
           _statusService=statusService;
        }

        public LoaderModel LoaderModel { get; set; } = new();

        public async Task GenerateFirmware()
        {
            await Task.Run(() => _firmwareLoaderService.LoadFirmware(LoaderModel.FirmwareVersionMaj, LoaderModel.FirmwareVersionMin, LoaderModel.FirmwareVersionRev));
        }

        public byte[] GetDeviceVersionCommand()
        {
            List<byte> cmd = new List<byte>
            {
                (byte)'V',
                0,
                3
            };

            return cmd.ToArray();

        }

        public void LoadFirmwareList()
        {
            _firmwareBase.LoadRecords();
            LoaderModel.FirmwareUpgrade.Add("Version" + _firmwareBase.Version + " (" + _firmwareBase.Date + ")");

        }

        public void ParsePacket(byte[] data)
        {
            if (data[(int)VersionIndex.LENGTH] != LoaderModel.FW_LENGTH)
            {
                _statusService.SetStatus("Firmware library isn't loaded or active, unable to get firmware version");
                LoaderModel.FwMaj = LoaderModel.NEVER_SET;
                LoaderModel.FwMin = LoaderModel.NEVER_SET;
                LoaderModel.FwRev = LoaderModel.NEVER_SET;
                return;
            }

            LoaderModel.FwMaj = data[(int)VersionIndex.FW_MAJ];
            LoaderModel.FwMin = data[(int)VersionIndex.FW_MIN];
            LoaderModel.FwRev = data[(int)VersionIndex.FW_REV];

            _statusService.SetStatus(StringConstants.RETRIEVE_CONFIG_MESSAGE);

        }

        public void ProgramFirmware()
        {
            using Process pProcess = new Process();
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            string output = pProcess.StandardOutput.ReadToEnd();
            pProcess.WaitForExit();
        }

        public async Task UploadFirmware()
        {
            List<byte[]> records;

            switch (LoaderModel.FirmwareUpgradeIndex)
            {
                case 0:
                    records = _firmwareBase.SRecords;
                    break;

                default:
                    _statusService.SetStatus(StatusType.ERROR, "Sending request for firmwareVersion. Waiting for response...");
                    return;
            }

            await _loRaWANInterfaceService.Value.SendMultiCommand(records);

        }
    }
}
