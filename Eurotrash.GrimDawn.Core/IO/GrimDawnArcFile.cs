using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Eurotrash.GrimDawn.Core.IO
{
    public class GrimDawnArcFile : IDisposable
    {
        protected FileInfo ArcFileInfo { get; set; }
        protected MemoryMappedFile ArcMemoryMap { get; set; }

        #region Data

        public ArcFileHeader Header { get; private set; }

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

        #endregion

        public void Dispose()
        {
            ArcMemoryMap.Dispose();
        }

        protected GrimDawnArcFile()
        {
        }

        /// <summary>
        ///     Loads the table of contents page from the .arc file for further referencing content chunks inside the .arc file.
        /// </summary>
        /// <param name="filename">Location of the .arc file.</param>
        /// <returns>Arc file wrapper that provides direct access to the files and records inside the .arc file.</returns>
        public static GrimDawnArcFile Load(string filename)
        {
            if (!File.Exists(filename)) throw new FileNotFoundException($"Path '{filename}' was not found.");

            var file = new GrimDawnArcFile();

            // Structs to write into.
            ArcFileHeader header;

            file.ArcMemoryMap = MemoryMappedFile.CreateFromFile(filename);
            using (var view = file.ArcMemoryMap.CreateViewAccessor())
            {
                // Store data into structs.
                view.Read(0, out header);
            }

            // Assign structs to current object.
            file.Header = header;

            return file;
        }
    }
}