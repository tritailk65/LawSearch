using LawSearch_Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Services
{
    public class ConceptService : IConceptService
    {
        private readonly IDbService dbService;

        public ConceptService(IDbService connectionService)
        {
            dbService = connectionService;
        }

        public DataTable GetListConcept()
        {
            try
            {
                dbService.OpenConnection();
                var sql = "select * from [Concept]  with(nolock)";
                var rs = dbService.ExecuteReaderCommand(sql, "");

                return rs;
            }
            catch
            {
                throw;
            }
            finally
            {
                dbService.CloseConnection();
            }
        }
    }
}
