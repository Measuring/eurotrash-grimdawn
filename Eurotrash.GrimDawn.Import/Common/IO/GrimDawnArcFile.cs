using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using Eurotrash.GrimDawn.Import.Common.IO.Data;

namespace Eurotrash.GrimDawn.Import.Common.IO
{
    public class GrimDawnArcFile : IDisposable
    {
        protected MemoryMappedFile MemoryMapFile { get; set; }
        public ArcFileHeader Header { get; private set; }

        public void Dispose()
        {
            MemoryMapFile.Dispose();
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
            var header = new ArcFileHeader();

            file.MemoryMapFile = MemoryMappedFile.CreateFromFile(filename);
            file.MemoryMapFile.CreateViewAccessor().Read(0, out header);

            file.Header = header;

            return file;
        }
    }
}