using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Entities;
using System.Text;

namespace Leapfrog.Infrastructure.Services
{
    public class FirmwareLoaderGenService : IFirmwareLoaderGenService
    {
        private readonly List<string> Code = [];
        private int LineIndex;
        private readonly List<HexRecord> LibRecords = [];
        private readonly Lazy<ILoRaWANInterfaceService> _loRaWANInterfaceService;
        private readonly IStatusService _statusService;
        private readonly IFirmwareLoaderUtilsService _firmwareLoaderUtilsService;

        public FirmwareLoaderGenService(Lazy<ILoRaWANInterfaceService> loRaWANInterfaceService, IStatusService statusService, IFirmwareLoaderUtilsService firmwareLoaderUtilsService)
        {
            _loRaWANInterfaceService=loRaWANInterfaceService;
            _statusService=statusService;
            _firmwareLoaderUtilsService=firmwareLoaderUtilsService;
        }

        public async Task GenerateCodeFile(List<HexRecord> records, uint startAddr, uint endAddr, string version, string name, bool printFinalRecords)
        {
            // Reset our line index
            LineIndex = 0;

            // Filter records based on start address
            for (int i = 0; i < records.Count; i++)
            {
                HexRecord record = (HexRecord)records[i];
                if (record.address >= startAddr && record.address <= endAddr)
                {
                    LibRecords.Add(record);
                }
            }

            // Add header
            CreateHeader(version, name);

            // Add start record
            AddLine(0, LibRecords.Count + 2, 0, 0, new byte[0]);

            // Add records as strings to our list
            foreach (var record in LibRecords)
            {
                // Get string that contains all the bytes
                var hexStr = string.Format("{0:X8}", record.address) + record.bytes;

                // Get the bytes of the hex string
                var bytes = _firmwareLoaderUtilsService.StringToByteArray(hexStr);

                // Add the line, remove 4 bytes for the address at the beginning
                AddLine(3, LibRecords.Count + 2, bytes.Length - 4, record.address, bytes);
            }

            // Add stop record
            AddLine(7, LibRecords.Count + 2, 0, 0, new byte[0]);

            // Create the file footer
            CreateFooter();

            // Should we print the records?
            if (printFinalRecords)
            {
                Console.WriteLine("\n\nFinished creating code file:");
                foreach (var line in Code)
                {
                    Console.WriteLine("  " + line);
                }
            }

            // Convert to string array
            var codeStrings = Code.ToArray();

            // Write all the strings to the code file
            File.WriteAllLines($"FirmwareV{name}.cs", codeStrings);

            _statusService.SetStatus($"Successfully generated file: FirmwareV{name}.cs");

            // Convert HexRecord list to List<byte[]>
            List<byte[]> byteRecords = records.Select(record => _firmwareLoaderUtilsService.StringToByteArray(record.bytes)).ToList();

            // Send records using LoRaWAN
            await _loRaWANInterfaceService.Value.SendMultiCommand(byteRecords);
        }

        private void CreateHeader(string version, string name)
        {
            // The first part of the file
            Code.Add("using System.Collections;");
            Code.Add("");
            Code.Add("namespace LeapfrogApp.FirmwareImages");
            Code.Add("{");
            Code.Add($"    class Firmware{name}");
            Code.Add("    {");
            Code.Add($"        public static string Version = \"{version}\";");
            Code.Add("");
            Code.Add($"        public static string Date = \"{DateTime.Now:dd\\/MMM\\/yyyy}\";");
            Code.Add("");
            Code.Add("        public static List<byte[]> SRecords = new List<byte[]>();");
            Code.Add("");
        }

        private void AddLine(int type, int totalLines, int len, uint addr, byte[] data)
        {
            var sb = new StringBuilder();

            Code.Add($"        private static byte[] Line{LineIndex} =");
            Code.Add("        {");
            Code.Add("            (byte)'S',");                                         // ID byte
            Code.Add($"            0x{type:X2},");                                    // Record type
            Code.Add($"            0x{(len + 10):X2},");                              // Total length
            Code.Add($"            0x{LineIndex:X2},");                               // Packet window
            Code.Add($"            0x{totalLines:X2},");                              // Total packets
            Code.Add($"            0x{len:X2},");                                     // Data length
            sb.Append($"            0x{((addr >> 24) & 0xFF):X2}, 0x{((addr >> 16) & 0xFF):X2}, 0x{((addr >> 8) & 0xFF):X2}, 0x{((addr >> 0) & 0xFF):X2}");

            if (data.Length == 0)
            {
                // Add this as the final line
                Code.Add(sb.ToString());
            }
            else
            {
                // Add a comma to the address line
                sb.Append(",");
                Code.Add(sb.ToString());

                sb.Clear();
                sb.Append("            "); // Spacing before line

                // Add bytes, we start at index 4 to skip the four address bytes
                for (int i = 4; i < data.Length; i++)
                {
                    // Add value
                    sb.Append($"0x{data[i]:X2}");

                    // Add commas to every entry except the last
                    if (i < data.Length - 1)
                    {
                        sb.Append(", ");
                    }
                }

                // Add data line
                Code.Add(sb.ToString());
            }

            // Add finishing lines
            Code.Add("        };");
            Code.Add("");

            // Increment the line index
            LineIndex++;
        }

        private void CreateFooter()
        {
            // The first part of the file
            Code.Add("        public static void LoadRecords()");
            Code.Add("        {");

            // Add lines that create the list
            for (int i = 0; i < LineIndex; i++)
            {
                Code.Add($"            SRecords.Add(Line{i});");
            }

            // Add closing brackets
            Code.Add("        }");
            Code.Add("");
            Code.Add("    }");
            Code.Add("}");
            Code.Add("");
        }
    }
}
