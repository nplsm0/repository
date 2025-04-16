using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUtility
{
    [Verb("load", HelpText = "Загрузить данные из БД.")]
    public class LoadOptions
    {
        [Option('i', "id", Required = false, HelpText = "Идентификатор модели в БД.")]
        public int? Id { get; set; }
    }
}
