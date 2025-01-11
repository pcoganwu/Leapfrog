using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Enums.LoRaWANInterfaceEnums;
using Leapfrog.Core.Enums.StatusEnums;

namespace Leapfrog.Infrastructure.Services
{
    public class LoRaWANInterfaceService(ILoRaWANService loRaWANService, IRadioConfigService radioConfigService, ILightConfigService lightConfigService , IStatusService statusService) : ILoRaWANInterfaceService
    {
        public async Task RefreshAllConfigurations()
        {
            List<byte[]> commands =
            [
                // Add commands for each configuration retrieval
                lightConfigService.GetDownloadCommand(),
                //SecurityModel.GetSecurityInfoCommand(),
                radioConfigService.GetRadioConfigCommand(),
                //LoaderModel.GetDeviceVersionCommand()
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
                await loRaWANService.SendMessageAsync(cmd);
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
                lightConfigService.ParsePacket(data);
                return;
            }

            // Is this a security packet?
            if (data[0] == 'X')
            {
                //SecurityModel.ParsePacket(data);
                return;
            }

            // Is this a radio config packet?
            if (data[0] == 'R')
            {
                //RadioConfigModel.ParsePacket(data);
                return;
            }

            if (data[0] == 'V')
            {
                //LoaderModel.ParsePacket(data);
                return;
            }

            switch ((SerialCommandType)data[2])
            {
                case SerialCommandType.SerialConfigGet:
                    lightConfigService.ParsePacket(data);
                    break;

                default:
                    statusService.SetStatus(StatusType.ERROR, "Request Failed: Got bad command");
                    break;
            }
        }
    }
}
