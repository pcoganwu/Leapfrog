using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Entities;

namespace Leapfrog.Infrastructure.Services
{
    public class LoaderModelService() : ILoaderModelService
    {
        public LoaderModel LoaderModel { get; set; } = new();

        public Task GenerateFirmware()
        {
            throw new NotImplementedException();
        }

        public byte[] GetDeviceVersionCommand()
        {
            throw new NotImplementedException();
        }

        public void LoadFirmwareList()
        {
            throw new NotImplementedException();
        }

        public void ParsePacket(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void ProgramFirmware()
        {
            throw new NotImplementedException();
        }

        public Task UploadFirmware()
        {
            throw new NotImplementedException();
        }
    }
}
