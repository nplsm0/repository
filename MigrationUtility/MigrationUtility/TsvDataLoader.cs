using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUtility
{
    /// <summary>
    /// Представляет собой загрузчик TSV-фалов с буферизацией
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class TsvDataLoader<T> where T : class
    {
        private const string _separator = "\t";
        private int _countOfRow = 1;
        private List<string> _headers;
        private FileLoader _bufferedLinesLoader;
        private PropertyInfo[] _properties;
        private DataValidator<T> _validator;
        private ErrorMessenger _errorMessager;

        public int BufferSize { get; set; } = 500;

        public TsvDataLoader(FileLoader loader, DataValidator<T> validator, ErrorMessenger errorMessenger)
        {
            _bufferedLinesLoader = loader;
            _headers = _bufferedLinesLoader.ReadLine().Split(_separator).ToList();
            _properties = typeof(T).GetProperties();
            _validator = validator;
            _errorMessager = errorMessenger;
        }

        public bool EndOfBuffer => _bufferedLinesLoader.EndOfStream;

        public IEnumerable<T> LoadBuffer()
        {
            List<T> lines = new List<T>();
            int bufferIndex = 0;
            while (bufferIndex != BufferSize && _bufferedLinesLoader.EndOfStream == false)
            {
                _countOfRow++;
                string? line = _bufferedLinesLoader.ReadLine();
                if(line?.Split(_separator).Count() != _headers.Count())
                {
                    _errorMessager.Send($"Ошибка разделителей в строке: {_countOfRow}");
                    break;
                }    
                T model = ConvertToModel(line);
                if (_validator.Validate(model))
                {
                    lines.Add(model);
                    bufferIndex++;
                }
                else
                    _errorMessager.Send($"Ошибка валидации данных в строке: {_countOfRow}");
            }
            return lines;
        }

        private T ConvertToModel(string record)
        {
            string[] values = record.Split(_separator);
            object newModel = CreateObject();
            foreach (PropertyInfo property in _properties)
            {
                ModelNameAttribute attribute = Attribute.GetCustomAttribute(property, typeof(ModelNameAttribute)) as ModelNameAttribute;
                if (attribute == null)
                    break;
                int index = _headers.IndexOf(attribute.Name);
                property.SetValue(newModel, values[index], null);
            }
            return newModel as T;
        }

        private object CreateObject()
        {
            return Activator.CreateInstance(typeof(T));
        }
    }
}
