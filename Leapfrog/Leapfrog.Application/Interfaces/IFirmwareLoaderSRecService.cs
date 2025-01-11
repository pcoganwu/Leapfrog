using Leapfrog.Core.Entities;

namespace Leapfrog.Application.Interfaces
{
    public interface IFirmwareLoaderSRecService
    {
        Task GenerateSRecFile(List<HexRecord> records, uint startAddr, uint endAddr, bool printFinalRecords);
    }
}
