namespace Leapfrog.Application.Interfaces
{
    public interface ILoRaWANInterfaceService
    {
        Task RefreshAllConfigurations();
        Task<bool> SendMultiCommand(List<byte[]> records);
        Task<bool> SendRawCommand(byte[] cmd);
        void RefreshEndpoints();
    }
}
