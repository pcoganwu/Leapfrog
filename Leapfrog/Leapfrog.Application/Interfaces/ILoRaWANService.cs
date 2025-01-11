namespace Leapfrog.Application.Interfaces
{
    public interface ILoRaWANService
    {
        event Action<byte[]>? OnMessageReceived;
        Task ConnectAsync();
        Task SendMessageAsync(byte[] message);
    }
}
