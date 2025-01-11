using Leapfrog.Core.Entities;
using Leapfrog.Core.Enums.RadioConfigEnums;

namespace Leapfrog.Application.Interfaces
{
    public interface IRadioConfigService
    {
        RadioConfigModel RadioConfig { get; set; }

        void Init();
        void SetValue(RadioConfigSetting setting);
        byte[] GetRadioConfigCommand();
        Task SetRadioConfig();
        Task<bool> SetDefaultRadioConfig();
        void ParsePacket(byte[] data);
    }
}
