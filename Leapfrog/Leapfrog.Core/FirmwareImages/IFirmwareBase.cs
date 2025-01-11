namespace Leapfrog.Core.FirmwareImages
{
    public interface IFirmwareBase
    {
        string Version { get; }
        string Date { get; }
        List<byte[]> SRecords { get; }

        void LoadRecords();
    }
}
