using Leapfrog.Application.Interfaces;

namespace Leapfrog.Infrastructure.Services
{
    public class FirmwareLoaderUtilsService : IFirmwareLoaderUtilsService
    {
        public byte[] StringToByteArray(string hex)
        {
            if (hex.Length % 2 != 0)
            {
                throw new ArgumentException(string.Format("The binary key cannot have an odd number of digits: {0}", hex));
            }

            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
