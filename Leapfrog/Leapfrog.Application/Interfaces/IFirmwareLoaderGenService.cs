using Leapfrog.Core.Entities;

namespace Leapfrog.Application.Interfaces
{
    public interface IFirmwareLoaderGenService
    {
        Task GenerateCodeFile(List<HexRecord> records, uint startAddr, uint endAddr, string version, string name, bool printFinalRecords);
    }
}
