using Leapfrog.Core.Entities;
using Leapfrog.Core.Enums.LightConfigEnums;
using System.Collections.ObjectModel;

namespace Leapfrog.Application.Interfaces
{
    public interface ILightConfigService
    {
        LightConfigModel LightConfig { get; set; }

        void Init();
        Task UploadAll();
        Task<bool> UploadDefault();
        byte[] GetDownloadCommand();
        void ParsePacket(byte[] data);
        Task<bool> UploadConfigCommand(DimmerSetting dimmerSetting, int flashLenSetting, LightTypeSetting lightTypeSetting, ButtonTypeSetting buttonTypeSetting, ButtonActionSetting buttonActionSetting, int flashDelaySetting, FlashPatternSetting flashPatternSetting);
        void SetValue(LightConfigType setting);
        void HandleGetDataPacket(byte[] data);
    }
}
