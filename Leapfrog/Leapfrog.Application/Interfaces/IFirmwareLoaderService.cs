namespace Leapfrog.Application.Interfaces
{
    public interface IFirmwareLoaderService
    {
        Task LoadFirmware(int versionMaj, int versionMin, int versionRev);
    }
}
