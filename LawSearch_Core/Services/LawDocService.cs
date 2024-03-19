using LawSearch_Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Services
{
    public class LawDocService : ILawDocService
    {
        private readonly IDbService db;

        public LawDocService(IDbService db)
        {
            this.db = db;
        }

        public DataTable GetLawHTML(int ID)
        {
            try
            {
                db.OpenConnection();
                var sql = "select top 1 ContentHTML from [LawHTML] where LawID = " + ID;
                DataTable rs = db.ExecuteReaderCommand(sql, "");
                return rs;
            }
            catch
            {
                throw;
            }
            finally
            {
                db.CloseConnection();
            }
        }

        public DataTable GetListLawDoc()
        {
            try
            {
                db.OpenConnection();
                var sql = "select ID, name from Law where status = 1 order by Name";
                DataTable rs = db.ExecuteReaderCommand(sql, "");
                return rs;
            }catch
            {
                throw;
            }
            finally
            {
                db.CloseConnection();
            }
        }
    }
}
