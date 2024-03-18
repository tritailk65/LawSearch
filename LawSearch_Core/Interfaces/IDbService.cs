using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Interfaces
{
    public interface IDbService
    {
        IDbCommand CreateCommand(string? sql = null);

        IDbTransaction BeginTransaction();

        DataTable ExecuteReaderCommand(string sql, string tableName);

        DataTable ExecuteReaderCommand(IDbCommand command, string tableName);

        T ExecuteScalarCommand<T>(string sql);

        T ExecuteScalarCommand<T>(IDbCommand command);

        void ExecuteNonQueryCommand(string sql);

        void ExecuteNonQueryCommand(IDbCommand command);

        void OpenConnection();

        IDbConnection GetDbConnection();

        void CloseConnection();

        void AddParameterWithValue(IDbCommand command, string name, object value);
    }
}
