using System.Runtime.InteropServices;

namespace Eurotrash.GrimDawn.Import.Common.IO.Data
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ArcFileHeader
    {
        public readonly uint Magic;
        public readonly uint Version;
        public readonly uint NumberOfFileEntries;
        public readonly uint NumberOfDataRecords;
        public readonly uint RecordTableSize;
        public readonly uint StringTableSize;
        public readonly uint RecordTableOffset;
    }
}