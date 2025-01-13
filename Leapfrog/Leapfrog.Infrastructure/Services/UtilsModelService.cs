using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Enums.UtilsEnums;

namespace Leapfrog.Infrastructure.Services
{
    public class UtilsModelService : IUtilsModelService
    {
        private readonly ILoRaWANInterfaceService _loRaWANInterfaceService;
        private readonly IStatusService _statusService;

        public UtilsModelService(ILoRaWANInterfaceService loRaWANInterfaceService, IStatusService statusService)
        {
            _loRaWANInterfaceService=loRaWANInterfaceService;
            _statusService=statusService;
        }
        public async Task FlashLights()
        {
            List<byte> cmd = new List<byte>();

            // Flash lights command
            cmd.Add((byte)'A');
            cmd.Add((byte)UtilCommandType.FLASH_LIGHTS);
            cmd.Add(3);

            // Convert to array
            byte[] cmdBytes = cmd.ToArray();

            // Update status
            _statusService.SetStatus("Flash request sent");

            // Send data
            await _loRaWANInterfaceService.SendRawCommand(cmdBytes);
        }
    }
}
