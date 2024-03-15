using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Services
{
    public class ArticalService : IArticalService
    {
        private readonly IDbConnectionService _dbConnection;

        public ArticalService(IDbConnectionService dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public DataTable GetAllArtical()
        {
            try
            {
                _dbConnection.OpenConnection();

                string sql = "SELECT * FROM ARTICAL";

                DataTable rs = _dbConnection.ExecuteReaderCommand(sql, "");
                return rs;

            }catch 
            {
                throw;
            } finally
            {
                _dbConnection.CloseConnection();
            }
        }
    }
}
