using Leapfrog.Core.Entities;

namespace Leapfrog.Application.Interfaces
{
    public interface ISecurityModelService
    {
        SecurityModel SecurityModel { get; set; }
        void Init();
        Task<byte[]> GetSecurityInfoCommand();
        Task GetSecurityInfo();
        Task SetSecurityInfo();
        Task<bool> SetDefaualtSecurityInfo();
        Task<bool> GenerateKey();
        void ParsePacket(byte[] data);

    }
}
