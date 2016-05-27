using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Eurotrash.GrimDawn.Core.IO
{
    /// <summary>
    ///     Gives easy access to all content of an .arc file.
    /// </summary>
    /// <remarks>
    ///     .arc files have the following chunks in order:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>Header</description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 File parts (Possibly compressed, can change per part).
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 File parts info (Info about the state of a part of a file).
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 File names (List of all file names).
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 File content info (Detailed information about any one file stored inside the .arc file)
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public class GrimDawnArcFile : IDisposable
    {
        protected GrimDawnArcFile()
        {
            ArcFileNames = new List<string>();
        }

        protected FileInfo ArcFileInfo { get; set; }
        protected MemoryMappedFile ArcMemoryMap { get; set; }

        /// <summary>
        ///     Cache for storing all file names that were read from the .arc file.
        /// </summary>
        protected List<string> ArcFileNames { get; set; }

        public void Dispose()
        {
            ArcMemoryMap.Dispose();
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

            // Opening a view of 2kb should be large enough to include all header data.
            using (var view = file.ArcMemoryMap.CreateViewAccessor(0, ArcFileHeader.Length))
            {
                // Store data into structs.
                view.Read(0, out header);
            }

            // Assign structs to current object.
            file.Header = header;

            return file;
        }

        /// <summary>
        ///     Reads the internal file names inside the .arc file. Returns immediately if the file names were already read.
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> ReadFileNamesAsync()
        {
            if (ArcFileNames?.Count > 0) return ArcFileNames;

            var stringTableOffset = Header.RecordTableOffset + Header.RecordTableSize;

            // Create view around string table containing file names.
            using (var view = ArcMemoryMap.CreateViewStream(stringTableOffset, Header.StringTableSize))
            {
                var buffer = new byte[Header.StringTableSize];
                await view.ReadAsync(buffer, 0, buffer.Length);
                ArcFileNames = Encoding.ASCII.GetString(buffer).Split('\0').ToList();
            }

            return ArcFileNames;
        }

        #region Data

        public ArcFileHeader Header { get; private set; }

        /// <summary>
        ///     Header data structure that contains global information about the content and position of the .arc file.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ArcFileHeader
        {
            /// <summary>
            ///     Unknown and probably not important.
            /// </summary>
            public readonly uint Magic;

            /// <summary>
            ///     Version of the file format.
            /// </summary>
            public readonly uint Version;

            /// <summary>
            ///     Number of files (or part of files) that are packed in this file.
            /// </summary>
            public readonly uint NumberOfFileEntries;

            /// <summary>
            ///     Number of data records that this file contains.
            /// </summary>
            public readonly uint NumberOfDataRecords;

            /// <summary>
            ///     Size in bytes of the whole data record table.
            /// </summary>
            public readonly uint RecordTableSize;

            /// <summary>
            ///     Size in bytes of the whole string table.
            /// </summary>
            public readonly uint StringTableSize;

            /// <summary>
            ///     Offset pointer that points to the start of the record table.
            /// </summary>
            public readonly uint RecordTableOffset;

            /// <summary>
            ///     Length in bytes of the header.
            /// </summary>
            public const int Length = 2024;
        }

        /// <summary>
        ///     File (or part of file) structure that can occur multiple times in a .arc file.
        ///     It contains data about where to find it and if it is compressed.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ArcFilePart
        {
            /// <summary>
            ///     Offset of the file part from the start of the file.
            /// </summary>
            public readonly uint Offset;

            /// <summary>
            ///     Size of the part as it is in the .arc file.
            /// </summary>
            public readonly uint CompressedSize;

            /// <summary>
            ///     Uncompressed size when decompressed and extracted into a new file.
            /// </summary>
            public readonly uint DecompressedSize;

            /// <summary>
            ///     True if this file (or part of a file) is compressed using the LZ4 algorithm.
            /// </summary>
            public bool IsCompressed => CompressedSize == DecompressedSize;
        }

        /// <summary>
        ///     Stores information about a complete file (possibly composed of multiple parts) inside the .arc file.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ArcFileContentEntry
        {
            /// <summary>
            ///     Storage method used. Apparently this is 1 if the file is not compressed but it seems to not be used anymore.
            /// </summary>
            public readonly uint EntryType;

            /// <summary>
            ///     Offset of the first part of this file.
            /// </summary>
            public readonly uint FirstPartOffset;

            /// <summary>
            ///     Size of the file when it is compressed using the LZ4 algorithm.
            /// </summary>
            public readonly uint CompressedSize;

            /// <summary>
            ///     Size of the file when it is uncompressed.
            /// </summary>
            public readonly uint DecompressedSize;

            /// <summary>
            ///     Timestamp of the file.
            /// </summary>
            public readonly uint Timestamp;

            /// <summary>
            ///     File time of the file.
            /// </summary>
            public readonly ulong FileTime;

            /// <summary>
            ///     Number of parts that this file is broken into.
            /// </summary>
            public readonly uint Parts;

            /// <summary>
            ///     First index of the part that this file starts on.
            /// </summary>
            public readonly uint FirstPartIndex;

            /// <summary>
            ///     Number of characters that make up the file name of this file in the string table.
            /// </summary>
            public readonly uint StringEntryLength;

            /// <summary>
            ///     Offset of the string that is the file name of this file in the string table.
            /// </summary>
            public readonly uint StringEntryOffset;
        }

        #endregion
    }
}