using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUtility
{
    [Verb("upload", HelpText = "Загрузить данные в БД.")]
    internal class UploadOptions
    {
        [Option('m', "model", Required = true, Default = "department", HelpText = "Модель данных.")]
        public string Model { get; set; }
        [Option('p', "filepath", Required = true, HelpText = "Путь к файлу.")]
        public string Filepath { get; set; }
    }
}
