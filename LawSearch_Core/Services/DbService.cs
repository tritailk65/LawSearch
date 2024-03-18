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
    public class DbService : IDbService
    {
        private readonly IDbConnection _dbConnection;

        public DbService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void OpenConnection()
        {
            try
            {
                _dbConnection.Open();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message.ToString(), 500);
            }
        }

        public IDbConnection GetDbConnection() { return _dbConnection; }

        public void CloseConnection()
        {
            _dbConnection.Close();
        }

        public IDbTransaction BeginTransaction()
        {
            try
            {
                return _dbConnection.BeginTransaction();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message.ToString(), 500);
            }
        }

        public IDbCommand CreateCommand(string? sql = null)
        {
            try
            {
                var command = _dbConnection.CreateCommand();
                command.CommandText = sql;
                return command;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message.ToString(), 500);
            }
        }

        public void ExecuteNonQueryCommand(string sql)
        {
            try
            {
                IDbCommand command = CreateCommand(sql);
                var result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message.ToString(), 500);
            }
        }

        public void ExecuteNonQueryCommand(IDbCommand command)
        {
            try
            {
                var result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message.ToString(), 500);
            }
        }

        public DataTable ExecuteReaderCommand(string sql, string tableName)
        {
            DataTable dtResult = new DataTable(tableName);
            try
            {
                IDbCommand command = CreateCommand(sql);
                IDataReader reader = command.ExecuteReader();
                dtResult.Load(reader);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message.ToString(), 500);
            }
            return dtResult;
        }

        public DataTable ExecuteReaderCommand(IDbCommand command, string tableName)
        {
            DataTable dtResult = new DataTable(tableName);
            try
            {
                IDataReader reader = command.ExecuteReader();
                dtResult.Load(reader);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message.ToString(), 500);
            }
            return dtResult;
        }

        public T ExecuteScalarCommand<T>(string sql)
        {
            try
            {
                IDbCommand command = CreateCommand(sql);
                var result = command.ExecuteScalar();
                if (result != null && result.GetType() != typeof(DBNull))
                    return (T)result;
                else
                    throw new BadRequestException("Data not found", 404);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message.ToString(), 500);
            }

        }

        public T ExecuteScalarCommand<T>(IDbCommand command)
        {
            try
            {
                var result = command.ExecuteScalar();
                if (result != null && result.GetType() != typeof(DBNull))
                    return (T)result;
                else
                    throw new BadRequestException("Data not found", 404);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message.ToString(), 500);
            }

        }

        public void AddParameterWithValue(IDbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }
    }
}
