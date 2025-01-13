using Leapfrog.Application.Interfaces;
using Leapfrog.Core.Entities;

namespace Leapfrog.Infrastructure.Services
{
    public class FirmwareLoaderService : IFirmwareLoaderService
    {
        // Debug prints
        private bool printOriginalRecords = false;
        private bool printContiguousRecords = false;
        private bool printOrderedRecords = false;
        private bool printFinalRecords = false;


        private readonly Lazy<ILoRaWANInterfaceService> _loRaWANInterfaceService;
        private readonly IFirmwareLoaderUtilsService _firmwareLoaderUtilsService;
        private readonly IStatusService _statusService;
        private readonly IFirmwareLoaderSRecService _firmwareLoaderSRecService;
        private readonly IFirmwareLoaderGenService _firmwareLoaderGenService;

        public FirmwareLoaderService(Lazy<ILoRaWANInterfaceService> loRaWANInterfaceService,
        IFirmwareLoaderUtilsService firmwareLoaderUtilsService,
        IStatusService statusService, IFirmwareLoaderSRecService
        firmwareLoaderSRecService, IFirmwareLoaderGenService firmwareLoaderGenService)
        {
            this._loRaWANInterfaceService=loRaWANInterfaceService;
            this._firmwareLoaderUtilsService=firmwareLoaderUtilsService;
            this._statusService=statusService;
            this._firmwareLoaderSRecService=firmwareLoaderSRecService;
            this._firmwareLoaderGenService=firmwareLoaderGenService;
        }

        // Program constants
        const uint RECORD_BYTES = 200;
        const uint LIBRARY_ADDR_START = 0x08030000;
        const uint LIBRARY_ADDR_END = 0x08040000;

        public async Task LoadFirmware(int versionMaj, int versionMin, int versionRev)
        {
            try
            {
                string filename = string.Empty; // Upload file
                if (!File.Exists(filename))
                {
                    _statusService.SetStatus("File doesn't exist, firmware gen aborted");
                    return;
                }

                List<HexRecord> records = GetHexRecords(filename);

                if (records == null)
                {
                    _statusService.SetStatus("File is not an SRec file, firmware gen aborted");
                    return;
                }

                records = MakeContiguousRecords(records);
                records = MakeOrderedRecords(records);

                await _firmwareLoaderSRecService.GenerateSRecFile(records, LIBRARY_ADDR_START, LIBRARY_ADDR_END, printFinalRecords);

                string version = $"{versionMaj}.{versionMin}.{versionRev}";
                string name = $"{versionMaj}_{versionMin}_{versionRev}";

                await _firmwareLoaderGenService.GenerateCodeFile(records, LIBRARY_ADDR_START, LIBRARY_ADDR_END, version, name, printFinalRecords);

                List<byte[]> byteRecords = records.Select(record => _firmwareLoaderUtilsService.StringToByteArray(record.bytes)).ToList();

                await _loRaWANInterfaceService.Value.SendMultiCommand(byteRecords);
            }
            catch
            {
                _statusService.SetStatus("File can't be loaded, the file maybe corrupted, firmware load aborted");
                return;
            }
        }

        private List<HexRecord> GetHexRecords(string filename)
        {
            var records = new List<HexRecord>();
            string[] lines = File.ReadAllLines(filename);
            for (int i = 0; i < (lines.Length - 1); i++)
            {
                if (lines[i][0] != 'S')
                {
                    Console.WriteLine("Line {0} isn't a srec line. Expected 'S' got {1}", i, lines[i][0]);
                    return null;
                }

                if (lines[i][1] != '3')
                {
                    continue;
                }

                HexRecord record = new HexRecord();
                uint length = Convert.ToUInt32(lines[i].Substring(2, 2), 16);
                string addrStr = lines[i].Substring(4, 8);
                record.address = Convert.ToUInt32(addrStr, 16);
                record.bytes = lines[i].Substring(12, (int)((length - 4 - 1) * 2));

                int index = 0;
                for (; index < records.Count; index++)
                {
                    HexRecord check = (HexRecord)records[index];
                    if (check.address > record.address)
                    {
                        break;
                    }
                }

                records.Insert(index, record);
            }

            if (printOriginalRecords)
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Finished creating hex records:");
                for (int i = 0; i < records.Count; i++)
                {
                    HexRecord printRecord = (HexRecord)records[i];
                    Console.WriteLine("  Addr(0x{0:X4}): " + printRecord.bytes, printRecord.address);
                }
            }

            return records;
        }

        private List<HexRecord> MakeContiguousRecords(List<HexRecord> records)
        {
            List<HexRecord> contiguousRecords = new List<HexRecord>();
            string contiguousBytes = "";
            uint currentAddr = 0;
            uint startAddr = 0;

            for (int i = 0; i < records.Count; i++)
            {
                HexRecord record = (HexRecord)records[i];

                if (contiguousBytes.Length != 0 && record.address != currentAddr)
                {
                    HexRecord contiguousRecord = new HexRecord();
                    contiguousRecord.address = startAddr;
                    contiguousRecord.bytes = contiguousBytes;
                    contiguousRecords.Add(contiguousRecord);
                    contiguousBytes = "";
                }

                if (contiguousBytes.Length == 0)
                {
                    startAddr = record.address;
                }

                contiguousBytes += record.bytes;
                currentAddr = record.address + (uint)(record.bytes.Length / 2);
            }

            HexRecord lastRecord = new HexRecord();
            lastRecord.address = startAddr;
            lastRecord.bytes = contiguousBytes;
            contiguousRecords.Add(lastRecord);

            if (printContiguousRecords)
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Finished creating consecutive hex records:");
                for (int i = 0; i < contiguousRecords.Count; i++)
                {
                    HexRecord printRecord = (HexRecord)contiguousRecords[i];
                    Console.WriteLine("  Addr(0x{0:X4}): " + printRecord.bytes.Length + " bytes in length", printRecord.address);
                }
            }

            return contiguousRecords;
        }

        private List<HexRecord> MakeOrderedRecords(List<HexRecord> records)
        {
            var orderedRecords = new List<HexRecord>();

            for (int i = 0; i < records.Count; i++)
            {
                HexRecord record = (HexRecord)records[i];
                uint currentIndex = 0;

                while (currentIndex < record.bytes.Length)
                {
                    HexRecord orderedRecord = new HexRecord();
                    if ((record.bytes.Length - currentIndex) > (RECORD_BYTES * 2))
                    {
                        orderedRecord.address = record.address + (currentIndex / 2);
                        orderedRecord.bytes = record.bytes.Substring((int)currentIndex, (int)RECORD_BYTES * 2);
                        currentIndex += RECORD_BYTES * 2;
                    }
                    else
                    {
                        orderedRecord.address = record.address + (currentIndex / 2);
                        orderedRecord.bytes = record.bytes.Substring((int)currentIndex);
                        currentIndex = (uint)record.bytes.Length;
                    }

                    orderedRecords.Add(orderedRecord);
                }
            }

            if (printOrderedRecords)
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Finished creating ordered hex records:");
                for (int i = 0; i < orderedRecords.Count; i++)
                {
                    HexRecord printRecord = (HexRecord)orderedRecords[i];
                    Console.WriteLine("  Addr(0x{0:X4}): " + printRecord.bytes, printRecord.address);
                }
            }

            return orderedRecords;
        }
    }
}
