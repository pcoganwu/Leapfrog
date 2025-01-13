using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Constants;
using Leapfrog.Core.Entities;
using Leapfrog.Core.Enums.RadioConfigEnums;
using Leapfrog.Core.Enums.StatusEnums;

namespace Leapfrog.Infrastructure.Services
{
    public class SecurityModelService : ISecurityModelService
    {
        private readonly Lazy<ILoRaWANInterfaceService> _loRaWANInterfaceService;
        private readonly IStatusService _statusService;

        public SecurityModelService(Lazy<ILoRaWANInterfaceService> loRaWANInterfaceService, IStatusService statusService)
        {
           _loRaWANInterfaceService=loRaWANInterfaceService;
           _statusService=statusService;
        }

        public SecurityModel SecurityModel { get; set; } = new();
        public async Task<bool> GenerateKey()
        {
            List<byte> cmd = await GetDefaultPacket(SecurityCmd.GEN_KEY);

            if ((cmd.Count - 3) != cmd[2])
            {
                return false;
            }

            byte[] cmdBytes = cmd.ToArray();
            _statusService.SetStatus("Successfully enabled the library on the device the device");

            return await _loRaWANInterfaceService.Value.SendRawCommand(cmdBytes);
        }


        public async Task GetSecurityInfo()
        {
            List<byte> cmd = await GetDefaultPacket(SecurityCmd.GET_DATA);

            if ((cmd.Count - 3) != cmd[2])
            {
                return;
            }

            byte[] cmdBytes = cmd.ToArray();
            await _loRaWANInterfaceService.Value.SendRawCommand(cmdBytes);

            cmd = await GetDefaultPacket(SecurityCmd.STATUS);

            if ((cmd.Count - 3) != cmd[2])
            {
                return;
            }

            cmdBytes = cmd.ToArray();
            await _loRaWANInterfaceService.Value.SendRawCommand(cmdBytes);
        }

        public async Task<byte[]> GetSecurityInfoCommand()
        {
            List<byte> cmd = await GetDefaultPacket(SecurityCmd.GET_DATA);

            if ((cmd.Count - 3) != cmd[2])
            {
                return null;
            }

            return cmd.ToArray();

        }

        public void Init()
        {
            SecurityModel.DevID1 = 0;
            SecurityModel.DevID2 = 0;
            SecurityModel.DevID3 = 0;
            SecurityModel.Password = SecurityModel.DEFAULT_PASSWORD;

        }

        public void ParsePacket(byte[] data)
        {
            int index = 0;

            switch ((SecurityCmd)data[1])
            {
                case SecurityCmd.GET_DATA:
                    index = 3;

                    if (data[index] > 3)
                    {
                        return;
                    }
                    SecurityModel.Valid = data[index++];

                    SecurityModel.DevID1 = (uint)(data[index++] << 24);
                    SecurityModel.DevID1 += (uint)(data[index++] << 16);
                    SecurityModel.DevID1 += (uint)(data[index++] << 8);
                    SecurityModel.DevID1 += (uint)(data[index++] << 0);

                    SecurityModel.DevID2 = (uint)(data[index++] << 24);
                    SecurityModel.DevID2 += (uint)(data[index++] << 16);
                    SecurityModel.DevID2 += (uint)(data[index++] << 8);
                    SecurityModel.DevID2 += (uint)(data[index++] << 0);

                    SecurityModel.DevID3 = (uint)(data[index++] << 24);
                    SecurityModel.DevID3 += (uint)(data[index++] << 16);
                    SecurityModel.DevID3 += (uint)(data[index++] << 8);
                    SecurityModel.DevID3 += (uint)(data[index++] << 0);

                    index += 32;

                    string pass = "";
                    for (int i = index; i < (index + 32); i++)
                    {
                        if (data[i] == 0x00)
                        {
                            break;
                        }
                        pass += (char)data[i];
                    }

                    SecurityModel.Password = pass;
                    _statusService.SetStatus(StringConstants.RETRIEVE_CONFIG_MESSAGE);
                    break;

                case SecurityCmd.STATUS:
                    SecurityModel.LibGood = (data[3] != 0) ? true : false;
                    SecurityModel.HashGood = (data[4] != 0) ? true : false;
                    break;

                case SecurityCmd.SET_DATA:
                    break;

                case SecurityCmd.ACK:
                    return;

                case SecurityCmd.NACK:
                    _statusService.SetStatus(StatusType.ERROR, "Device failed command");
                    return;

                default:
                    _statusService.SetStatus(StatusType.ERROR, "Got unknown response from device");
                    return;
            }
        }

        public async Task<bool> SetDefaualtSecurityInfo()
        {
            return await SetSecurityInfoCommand(SecurityModel.DEFAULT_PASSWORD);
        }

        public async Task SetSecurityInfo()
        {
            await SetSecurityInfoCommand(SecurityModel.Password);
        }

        private async Task<List<byte>> GetDefaultPacket(SecurityCmd cmdType)
        {
            List<byte> cmd = new List<byte>();

            cmd.Add((byte)'X');
            cmd.Add((byte)cmdType);
            cmd.Add(1 + 12 + 32 + 32);

            cmd.Add(0);

            for (int i = 0; i < 12; i++)
            {
                cmd.Add(0);
            }

            cmd.Add((byte)'E');
            cmd.Add((byte)'n');
            cmd.Add((byte)'c');
            cmd.Add((byte)'o');
            cmd.Add((byte)'m');
            cmd.Add((byte)'W');
            cmd.Add((byte)'i');
            cmd.Add((byte)'r');
            cmd.Add((byte)'e');
            cmd.Add((byte)'l');
            cmd.Add((byte)'e');
            cmd.Add((byte)'s');
            cmd.Add((byte)'s');
            cmd.Add((byte)'I');
            cmd.Add((byte)'n');
            cmd.Add((byte)'t');
            cmd.Add((byte)'e');
            cmd.Add((byte)'r');
            cmd.Add((byte)'n');
            cmd.Add((byte)'a');
            cmd.Add((byte)'l');

            while ((cmd.Count - 3) < cmd[2])
            {
                cmd.Add(0);
            }

            return cmd;
        }

        private async Task<bool> SetSecurityInfoCommand(string pass)
        {
            int index = 0;
            List<byte> cmd = await GetDefaultPacket(SecurityCmd.SET_DATA);

            cmd[3] = (byte)SecurityValid.VALID;

            index = 3 + 1 + 12 + 32;
            for (int i = 0; i < pass.Length; i++)
            {
                cmd[index++] = (byte)pass[i];
            }

            if ((cmd.Count - 3) != cmd[2])
            {
                _statusService.SetStatus(StatusType.ERROR, "Failed to verify password length");
                return false;
            }

            byte[] cmdBytes = cmd.ToArray();
            _statusService.SetStatus("Successfully set Security data from the device");

            return await _loRaWANInterfaceService.Value.SendRawCommand(cmdBytes);
        }

    }
}
