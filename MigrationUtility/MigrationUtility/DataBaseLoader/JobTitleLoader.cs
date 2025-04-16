using DataBaseModel.Context;
using DataBaseModel.Model;
using MigrationUtility.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUtility.DataBaseLoader
{
    /// <summary>
    /// Представляет собо загрузчик должностей в БД
    /// </summary>
    internal class JobTitleLoader : IDataBaseLoader<JobTitleModel>
    {
        private PostgresContext _context;
        public JobTitleLoader(PostgresContext context)
        {
            _context = context;
        }

        public void Put(IEnumerable<JobTitleModel> jobTitles)
        {
            foreach(var jobTitle in jobTitles)
            {
                InsertOrUpdate(jobTitle);
            }
            _context.SaveChanges();
        }

        private void InsertOrUpdate(JobTitleModel jobTitleModel)
        {
            if (_context.JobTitles.FirstOrDefault(x => x.Name == jobTitleModel.Name) == null)
            {
                JobTitle jobTitle = new JobTitle() { Name = jobTitleModel.Name };
                _context.JobTitles.Add(jobTitle);
            }
        }
    }
}
