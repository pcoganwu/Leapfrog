using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Entities;

namespace Leapfrog.Infrastructure.Services
{
    public class FirmwareLoaderSRecService(ILoRaWANInterfaceService loRaWANInterfaceService, IFirmwareLoaderUtilsService firmwareLoaderUtilsService) : IFirmwareLoaderSRecService
    {
        public async Task GenerateSRecFile(List<HexRecord> records, uint startAddr, uint endAddr, bool printFinalRecords)
        {
            // Create the string list we will write to our new image
            var image = new List<string>();

            // Add start rec
            image.Add(MakeSREC(0, 0, ""));

            // Add records as strings to our list
            for (int i = 0; i < records.Count; i++)
            {
                HexRecord record = (HexRecord)records[i];
                // Check for boot loader record (nothing comes before the boot loader)
                if (record.address < startAddr || record.address > endAddr)
                {
                    // Skip boot loader records
                    continue;
                }

                // Get string that contains all the bytes
                var hexStr = string.Format("{0:X8}", record.address) + record.bytes;

                // Get the bytes of the hex string
                var bytes = firmwareLoaderUtilsService.StringToByteArray(hexStr);

                // Calculate checksum
                byte checksum = (byte)(bytes.Length + 1);

                foreach (var b in bytes)
                {
                    checksum += b;
                }

                // One's compliment
                checksum = (byte)~checksum;

                // Add the completed hex record to the string list
                image.Add("S3" + string.Format("{0:X2}", (bytes.Length + 1)) + hexStr + string.Format("{0:X2}", checksum));
            }

            // Add stop rec
            image.Add(MakeSREC(7, 0, ""));

            // Should we print the records?
            if (printFinalRecords)
            {
                Console.WriteLine("\n\nFinished creating hex image:");
                foreach (var line in image)
                {
                    Console.WriteLine("  " + line);
                }
            }

            // Convert to string array
            var imageStrings = image.ToArray();

            // Write all the strings to a srec file
            File.WriteAllLines("leapfrog_lib.srec", imageStrings);

            // Convert HexRecord list to List<byte[]>
            List<byte[]> byteRecords = records.Select(record => firmwareLoaderUtilsService.StringToByteArray(record.bytes)).ToList();

            // Send records using LoRaWAN
            await loRaWANInterfaceService.SendMultiCommand(byteRecords);
        }

        private string MakeSREC(uint type, uint addr, string data)
        {
            // Get string that contains all the bytes
            var hexStr = string.Format("{0:X8}", addr) + data;

            // Get the bytes of the hex string
            var bytes = firmwareLoaderUtilsService.StringToByteArray(hexStr);

            // Calculate checksum
            byte checksum = (byte)(bytes.Length + 1);

            foreach (var b in bytes)
            {
                checksum += b;
            }

            // One's compliment
            checksum = (byte)~checksum;

            // Add the completed hex record to the string list
            return "S" + string.Format("{0:X1}", type) + string.Format("{0:X2}", (bytes.Length + 1)) + hexStr + string.Format("{0:X2}", checksum);
        }
    }
}
