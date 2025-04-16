using DataBaseModel.Context;
using DataBaseModel.Model;
using Microsoft.Extensions.Configuration;
using MigrationUtility.DataBaseLoader;
using MigrationUtility.Model;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Configuration;
using Microsoft.Extensions.Logging;
using CommandLine;
using Microsoft.Extensions.Options;

namespace MigrationUtility
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ErrorMessenger errorMessenger = new ErrorMessenger();
            errorMessenger.ErrorSended += (o, e) => Console.Error.WriteLine($"ERROR: {e}");
            try
            {
                PostgresContext.SetConnectionString(ConfigurationManager.AppSettings["ConnectionString"]);
                Parser.Default.ParseArguments<LoadOptions, UploadOptions>(args).WithParsed<LoadOptions>(o => Load(o.Id, errorMessenger))
                                                                               .WithParsed<UploadOptions>(o => Upload(o.Model, o.Filepath, errorMessenger));
            }
            catch (Exception e)
            {
                errorMessenger.Send(e.Message);
            }
            Console.WriteLine("Нажмите любую клавишу для завершения...");
            Console.ReadKey();
        }

        private static void Load(int? id, ErrorMessenger errorMessenger)
        {
            Console.WriteLine("Начало загрузки");
            string result;
            if(id == null)
                result = LoadDepartmentHierarchy();
            else
                result = LoadDepartmentHierarchy((int)id, errorMessenger);
            Console.WriteLine(result);
        }

        private static void Upload(string model, string path, ErrorMessenger errorMessenger)
        {
            Console.WriteLine("Начало загрузки");
            if (File.Exists(path) == false)
            {
                errorMessenger.Send("Нет файла по заданному пути");
                return;
            }
            if (model == "department")
                LoadDepartments(path, errorMessenger);
            else if(model == "employee")
                LoadEmployees(path, errorMessenger);
            else if(model == "jobtitle")
                LoadJobTitles(path, errorMessenger);
            else
            {
                errorMessenger.Send($"Неизвестная модель БД: {model}");
                return;
            }
            Console.WriteLine("Окончание загрузки");
        }

        private static string LoadDepartmentHierarchy()
        {
            using (PostgresContext db = new PostgresContext())
            {
                DepartmentHierarchyLoader loader = new DepartmentHierarchyLoader(db);
                return string.Join('\n', loader.Get());
            }
        }

        private static string LoadDepartmentHierarchy(int id, ErrorMessenger errorMessenger)
        {
            using (PostgresContext db = new PostgresContext())
            {
                DepartmentHierarchyLoader loader = new DepartmentHierarchyLoader(db, errorMessenger);
                return string.Join('\n', loader.Get(id) ?? Array.Empty<string>());
            }
        }

        private static void LoadJobTitles(string filepath, ErrorMessenger errorMessenger)
        {
            string fieldPattern = "^\\s|\\s{2,}|\\s$";
            DataValidator<JobTitleModel> validator = new DataValidator<JobTitleModel>();
            validator.SetFilter(x => !Regex.IsMatch(x.Name, fieldPattern));
            using (PostgresContext db = new PostgresContext())
            {
                IDataBaseLoader<JobTitleModel> loader = new JobTitleLoader(db);
                Load<JobTitleModel>(filepath, loader, validator, errorMessenger);
            }
        }

        private static void LoadDepartments(string filepath, ErrorMessenger errorMessenger)
        {
            string fieldPattern = "^\\s|\\s{2,}|\\s$";
            string phoneNumberPattern = "[^\\d\\s()-]|\\s{2,}";
            DataValidator<DepartmentModel> validator = new DataValidator<DepartmentModel>();
            validator.SetFilter(x => !Regex.IsMatch(x.Name, fieldPattern)
                                  && !Regex.IsMatch(x.Parent, fieldPattern)
                                  && !Regex.IsMatch(x.Manager, fieldPattern)
                                  && !Regex.IsMatch(x.Phone, phoneNumberPattern));
            using (PostgresContext db = new PostgresContext())
            {
                IDataBaseLoader<DepartmentModel> loader = new DepartmentLoader(db, errorMessenger);
                Load<DepartmentModel>(filepath, loader, validator, errorMessenger);
            }
        }

        private static void LoadEmployees(string filepath, ErrorMessenger errorMessenger)
        {
            string fieldPattern = "^\\s|\\s{2,}|\\s$";
            string loginPattern = "^\\s|\\s+|\\s$";
            DataValidator<EmployeeModel> validator = new DataValidator<EmployeeModel>();
            validator.SetFilter(x => !Regex.IsMatch(x.Department, fieldPattern)
                                  && !Regex.IsMatch(x.FullName, fieldPattern)
                                  && !Regex.IsMatch(x.Login, loginPattern)
                                  && !Regex.IsMatch(x.JobTitle, fieldPattern));
            using (PostgresContext db = new PostgresContext())
            {
                IDataBaseLoader<EmployeeModel> loader = new EmployeeLoader(db);
                Load<EmployeeModel>(filepath, loader, validator, errorMessenger);
            }
        }

        private static void Load<T>(string filepath, IDataBaseLoader<T> loader, DataValidator<T> validator, ErrorMessenger errorMessager) where T : class
        {
            using (FileLoader buferedLoader = new FileLoader(filepath))
            {
                TsvDataLoader<T> dataLoader = new TsvDataLoader<T>(buferedLoader, validator, errorMessager);
                while (dataLoader.EndOfBuffer == false)
                {
                    loader.Put(dataLoader.LoadBuffer());
                }
            }
        }
    }
}