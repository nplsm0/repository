using System;
using System.Collections.Generic;
using System.Data;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUtility
{
    /// <summary>
    /// Представляет собой построчный считыватель файла
    /// </summary>
    internal class FileLoader : IDisposable
    {
        private FileStream _fileStream;
        private BufferedStream _bufferedStream;
        private StreamReader _streamReader;

        public FileLoader(string filepath)
        {
            _fileStream = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _bufferedStream = new BufferedStream(_fileStream);
            _streamReader = new StreamReader(_bufferedStream);
        }

        public bool EndOfStream => _streamReader.EndOfStream;

        public string? ReadLine() => _streamReader.ReadLine();

        public void Dispose()
        {
            _fileStream.Dispose();
            _bufferedStream.Dispose();
            _streamReader.Dispose();
        }
    }
}
