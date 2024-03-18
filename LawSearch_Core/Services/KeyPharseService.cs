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
    public class KeyPharseService : IKeyPhraseService
    {
        private readonly IDbService _db;

        public KeyPharseService(IDbService db)
        {
            _db = db;
        }

        /// <summary>
        /// Lấy KeyPhraseRelateByID (TriTai)
        /// </summary>
        /// <param name="ID">Mã KeyPhrase</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public DataTable GetKeyPhraseRelateDetailsByID(int ID)
        {
            try
            {
                _db.OpenConnection();
                string sql = "exec GetKeyPhraseRelateDetailsByID " + ID;
                var rs = _db.ExecuteReaderCommand(sql, "");
                return rs;
            }
            catch
            {
                throw;
            }
            finally
            {
                _db.CloseConnection();
            }
        }

        /// <summary>
        /// Lấy tất cả KeyPhrase (TriTai)
        /// </summary>
        /// <returns></returns>
        public DataTable GetListKeyPhrase()
        {
            try
            {
                _db.OpenConnection();
                string sql = "select * from [KeyPhrase] with(nolock) order by KeyPhrase";
                var rs = _db.ExecuteReaderCommand(sql, "");
                return rs;
            }
            catch
            {
                throw;

            } finally
            {
                _db.CloseConnection();
            }
        }


    }
}
