using Leapfrog.Core.Entities;

namespace Leapfrog.Application.Interfaces
{
    public interface ILoaderModelService
    {
        LoaderModel LoaderModel { get; set; }

        void LoadFirmwareList();
        Task GenerateFirmware();
        Task UploadFirmware();
        byte[] GetDeviceVersionCommand();
        void ParsePacket(byte[] data);
        void ProgramFirmware();
    }
}
