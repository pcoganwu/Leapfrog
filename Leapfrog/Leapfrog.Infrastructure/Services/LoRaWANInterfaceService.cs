using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Enums.LoRaWANInterfaceEnums;
using Leapfrog.Core.Enums.StatusEnums;

namespace Leapfrog.Infrastructure.Services
{
    public class LoRaWANInterfaceService : ILoRaWANInterfaceService
    {
        private readonly ILoaderModelService _loaderModelService;
        private readonly ISecurityModelService _securityModelService;
        private readonly ILoRaWANService _loRaWANService;
        private readonly IRadioConfigService _radioConfigService;
        private readonly ILightConfigService _lightConfigService;
        private readonly IStatusService _statusService;

        public LoRaWANInterfaceService(ILoaderModelService loaderModelService, ISecurityModelService securityModelService, ILoRaWANService loRaWANService, IRadioConfigService radioConfigService, ILightConfigService lightConfigService, IStatusService statusService)
        {
           _loaderModelService=loaderModelService;
           _securityModelService=securityModelService;
           _loRaWANService=loRaWANService;
           _radioConfigService=radioConfigService;
           _lightConfigService=lightConfigService;
           _statusService=statusService;

            // Subscribe to the MQTT message event
            _loRaWANService.OnMessageReceived += (data) =>
            {
                ParseIncomingData(data);
            };
        }
        public async Task RefreshAllConfigurations()
        {
            List<byte[]> commands =
            [
                // Add commands for each configuration retrieval
                _lightConfigService.GetDownloadCommand(),
                await _securityModelService.GetSecurityInfoCommand(),
                _radioConfigService.GetRadioConfigCommand(),
                _loaderModelService.GetDeviceVersionCommand()
            ];

            foreach (var command in commands)
            {
                await SendRawCommand(command);
            }
        }

        public void RefreshEndpoints()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SendMultiCommand(List<byte[]> records)
        {
            bool status = false;

            try
            {
                for (int i = 0; i < records.Count; i++)
                {
                    byte[]? command = records[i] as byte[];
                    if (command == null)
                    {
                        throw new ArgumentNullException(nameof(command), "Command cannot be null");
                    }
                    await SendRawCommand(command);

                    // Simulate receiving an ACK
                    byte[] ack = [(byte)SerialStatus.STATUS_OK];
                    ParseIncomingData(ack);

                    // Check if we received the ACK
                    if (ack[0] != (byte)SerialStatus.STATUS_OK)
                    {
                        status = false;
                        break;
                    }
                    else
                    {
                        status = true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return status;
        }

        // Method to send a raw command using MQTT
        public async Task<bool> SendRawCommand(byte[] cmd)
        {
            try
            {
                await _loRaWANService.SendMessageAsync(cmd);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        // Method to handle incoming data from MQTT
        private void ParseIncomingData(byte[] data)
        {
            // Is this a light config packet?
            if (data[0] == 'A')
            {
                _lightConfigService.ParsePacket(data);
                return;
            }

            // Is this a security packet?
            if (data[0] == 'X')
            {
                _securityModelService.ParsePacket(data);
                return;
            }

            // Is this a radio config packet?
            if (data[0] == 'R')
            {
                _radioConfigService.ParsePacket(data);
                return;
            }

            if (data[0] == 'V')
            {
                _loaderModelService.ParsePacket(data);
                return;
            }

            switch ((SerialCommandType)data[2])
            {
                case SerialCommandType.SerialConfigGet:
                    _lightConfigService.ParsePacket(data);
                    break;

                default:
                    _statusService.SetStatus(StatusType.ERROR, "Request Failed: Got bad command");
                    break;
            }
        }
    }
}
