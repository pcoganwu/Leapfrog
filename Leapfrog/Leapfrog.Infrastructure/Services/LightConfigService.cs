using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Entities;
using Leapfrog.Core.Enums.LightConfigEnums;
using Leapfrog.Core.Enums.RadioConfigEnums;
using Leapfrog.Core.Enums.StatusEnums;
using ConfigIndex = Leapfrog.Core.Enums.LightConfigEnums.ConfigIndex;

namespace Leapfrog.Infrastructure.Services
{
    public class LightConfigService(ILoRaWANInterfaceService loRaWANInterfaceService, IStatusService statusService) : ILightConfigService
    {
        private const byte CONFIG_SET_LENGTH = 17;
        public LightConfigModel LightConfig { get; set; } = new();

        public byte[] GetDownloadCommand()
        {
            return [(byte)'A', (byte)ConfigCommandType.CONFIG_GET, 3];
        }

        public void HandleGetDataPacket(byte[] data)
        {
            LightConfig.DimmerDevice = data[(int)ConfigIndex.DIMMER].ToString();

            LightConfig.FlashLenDevice =
                ((data[(int)ConfigIndex.FLASH_LEN1] << 24) +
                (data[(int)ConfigIndex.FLASH_LEN2] << 16) +
                (data[(int)ConfigIndex.FLASH_LEN3] << 8) +
                data[(int)ConfigIndex.FLASH_LEN4]).ToString();
            LightConfig.FlashLenDevice = (int.Parse(LightConfig.FlashLenDevice) / 1000).ToString();

            LightConfig.LightTypeDevice = data[(int)ConfigIndex.LIGHT_TYPE].ToString();
            LightConfig.ButtonTypeDevice = data[(int)ConfigIndex.BUTTON_TYPE].ToString();
            LightConfig.ButtonActionDevice = data[(int)ConfigIndex.BUTTON_ACTION].ToString();

            LightConfig.FlashDelayDevice =
                ((data[(int)ConfigIndex.FLASH_DELAY1] << 24) +
                (data[(int)ConfigIndex.FLASH_DELAY2] << 16) +
                (data[(int)ConfigIndex.FLASH_DELAY3] << 8) +
                data[(int)ConfigIndex.FLASH_DELAY4]).ToString();
            LightConfig.FlashDelayDevice = (int.Parse(LightConfig.FlashDelayDevice) / 1000).ToString();

            LightConfig.FlashPatternDevice = data[(int)ConfigIndex.FLASH_PATTERN].ToString();

            statusService.SetStatus("Configuration retrieved successfully.");
        }

        public void Init()
        {
            LightConfig.Init();
        }

        public void ParsePacket(byte[] data)
        {
            switch ((ConfigCommandType)data[1])
            {
                case ConfigCommandType.CONFIG_GET:
                    HandleGetDataPacket(data);
                    break;

                case ConfigCommandType.CONFIG_SET:
                    if (data[3] == 0x00)
                    {
                        statusService.SetStatus("Light config updated on device");
                    }
                    else
                    {
                        statusService.SetStatus(StatusType.ERROR, "Light config failed to update on device");
                    }
                    break;
                default:
                    break;
            }
        }

        public void SetValue(LightConfigType setting)
        {
            if (LightConfig.DimmerDevice == LightConfigModel.NEVER_SET.ToString() && setting == LightConfigType.ConfigDimmer) return;
            if (LightConfig.FlashLenDevice == LightConfigModel.NEVER_SET.ToString() && setting == LightConfigType.ConfigFlashLength) return;
            if (LightConfig.LightTypeDevice == LightConfigModel.NEVER_SET.ToString() && setting == LightConfigType.ConfigLightType) return;
            if (LightConfig.ButtonTypeDevice == LightConfigModel.NEVER_SET.ToString() && setting == LightConfigType.ConfigButtonType) return;
            if (LightConfig.ButtonActionDevice == LightConfigModel.NEVER_SET.ToString() && setting == LightConfigType.ConfigButtonAction) return;
            if (LightConfig.FlashDelayDevice == LightConfigModel.NEVER_SET.ToString() && setting == LightConfigType.ConfigFlashDelay) return;
            if (LightConfig.FlashPatternDevice == LightConfigModel.NEVER_SET.ToString() && setting == LightConfigType.ConfigFlashPattern) return;

            switch (setting)
            {
                case LightConfigType.ConfigDimmer:
                    LightConfig.DimmerIndex = int.Parse(LightConfig.DimmerDevice);
                    break;

                case LightConfigType.ConfigFlashLength:
                    LightConfig.FlashLen = LightConfig.FlashLenDevice.ToString();
                    break;

                case LightConfigType.ConfigLightType:
                    LightConfig.LightTypeIndex = int.Parse(LightConfig.LightTypeDevice);
                    break;

                case LightConfigType.ConfigButtonType:
                    LightConfig.ButtonTypeIndex = int.Parse(LightConfig.ButtonTypeDevice);
                    break;

                case LightConfigType.ConfigButtonAction:
                    LightConfig.ButtonActionIndex = int.Parse(LightConfig.ButtonActionDevice);
                    break;

                case LightConfigType.ConfigFlashDelay:
                    LightConfig.FlashDelay = LightConfig.FlashDelayDevice.ToString();
                    break;

                case LightConfigType.ConfigFlashPattern:
                    LightConfig.FlashPatternIndex = int.Parse(LightConfig.FlashPatternDevice);
                    break;
            }
        }

        public async Task UploadAll()
        {
            if (LightConfig.FlashLenStatus == ConfigStatus.INVALID || LightConfig.FlashDelayStatus == ConfigStatus.INVALID)
            {
                statusService.SetStatus(StatusType.ERROR, "Request Failed: Please correct values highlighted in red");
                return;
            }

            await UploadConfigCommand(LightConfig.Dimmer, LightConfig.CurrentFlashLen, LightConfig.LightType, LightConfig.ButtonType, LightConfig.ButtonAction, LightConfig.CurrentFlashDelay, LightConfig.FlashPattern);
        }

        public async Task<bool> UploadConfigCommand(DimmerSetting dimmerSetting, int flashLenSetting, LightTypeSetting lightTypeSetting, ButtonTypeSetting buttonTypeSetting, ButtonActionSetting buttonActionSetting, int flashDelaySetting, FlashPatternSetting flashPatternSetting)
        {
            var cmd = new List<byte>
            {
                (byte)'A',
                (byte)ConfigCommandType.CONFIG_SET,
                CONFIG_SET_LENGTH,
                0, // Add a dummy byte for the version
                (byte)dimmerSetting
            };

            int value = flashLenSetting * 1000;
            cmd.AddRange(BitConverter.GetBytes(value).Reverse());

            cmd.Add((byte)lightTypeSetting);
            cmd.Add((byte)buttonTypeSetting);
            cmd.Add((byte)buttonActionSetting);

            value = flashDelaySetting * 1000;
            cmd.AddRange(BitConverter.GetBytes(value).Reverse());

            cmd.Add((byte)flashPatternSetting);

            byte[] cmdBytes = cmd.ToArray();

            statusService.SetStatus("Sending request for Light Config. Waiting for response...");

            return await loRaWANInterfaceService.SendRawCommand(cmdBytes);
        }

        public async Task<bool> UploadDefault()
        {
            try
            {
                return await UploadConfigCommand(DimmerSetting.LIGHT_SENSOR, 45, LightTypeSetting.ENCOM_LIGHTBAR, ButtonTypeSetting.POLARA_BULLDOG, ButtonActionSetting.RESET_TIMER, 0, FlashPatternSetting.MUTCD);
            }
            catch
            {
                statusService.SetStatus(StatusType.ERROR, "Request Failed: Default values are corrupt");
                return false;
            }
        }
    }
}
