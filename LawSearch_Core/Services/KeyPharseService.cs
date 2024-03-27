using LawSearch_Core.Common;
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

        public List<KeyPhraseRelate> GetKeyPhraseRelateDetailsByID(int ID)
        {
            try
            {
                _db.OpenConnection();
                string sql = "exec GetKeyPhraseRelateDetailsByID " + ID;
                var rs = _db.ExecuteReaderCommand(sql, "");

                List<KeyPhraseRelate> lst = new List<KeyPhraseRelate> ();
                for (int i = 0; i< Globals.DTCount(rs); i++)
                {
                    lst.Add(new KeyPhraseRelate
                    {
                        KeyPhraseID = Globals.GetIDinDT(rs, i, "KeyPhraseID"),
                        KeyPhrase = Globals.GetinDT_String(rs, i,  "KeyPhrase"),
                        ArticalID = Globals.GetIDinDT(rs, i, "ArticalID"),
                        ArticalName = Globals.GetinDT_String(rs, i, "ArticalName"),
                        NumCount = Globals.GetIDinDT(rs, i, "NumCount"),
                        ChapterName = Globals.GetinDT_String(rs, i, "ChapterName")
                    });
                }
                return lst;
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

        public List<KeyPhrase> GetListKeyPhrase()
        {
            try
            {
                _db.OpenConnection();
                string sql = "select * from [KeyPhrase] with(nolock) order by KeyPhrase.ID";
                var rs = _db.ExecuteReaderCommand(sql, "");

                List<KeyPhrase> lst = new List<KeyPhrase>();
                for (int i = 0; i< Globals.DTCount(rs); i++)
                {
                    lst.Add(new KeyPhrase
                    {
                        ID = Globals.GetIDinDT(rs, i, "ID"),
                        Keyphrase = Globals.GetinDT_String(rs, i,  "KeyPhrase"),
                        NumberArtical = Globals.GetIDinDT(rs, i, "NumberArtical"),
                        KeyNorm = Globals.GetinDT_String(rs, i, "KeyNorm"),
                        LawID = Globals.GetIDinDT(rs, i, "LawID")
                    });
                }
                return lst;
            }
            catch
            {
                throw;

            } finally
            {
                _db.CloseConnection();
            }
        }

        public KeyPhrase AddKeyPhrase(KeyPhrase keyPhrase)
        {
            try
            {
                _db.OpenConnection();

                if (keyPhrase.Keyphrase == null || keyPhrase.Keyphrase == "")
                {
                    throw new BadRequestException("KeyPhrase không được bỏ trống", 400, 400);
                }
                string sql = string.Format("exec GetKeyPhrase N'{0}'", keyPhrase.Keyphrase);
                int id = _db.ExecuteScalarCommand<int>(sql);

                DataTable dt = _db.ExecuteReaderCommand("select * from [KeyPhrase] where id = "+id, "");
                KeyPhrase rs = new KeyPhrase
                {
                    ID = Globals.GetIDinDT(dt, 0, "ID"),
                    Keyphrase = Globals.GetinDT_String(dt, 0, "KeyPhrase"),
                    NumberArtical = Globals.GetIDinDT(dt, 0, "NumberArtical"),
                    KeyNorm = Globals.GetinDT_String(dt, 0, "KeyNorm"),
                    LawID = Globals.GetIDinDT(dt, 0, "LawID")
                };

                return rs;
            } catch
            {
                throw;
            }
            finally
            {
                _db.CloseConnection();
            }
        }

        public void DeleteKeyPhrase(int id)
        {
            try
            {
                _db.OpenConnection();

                //Check ID
                var checkID = _db.ExecuteReaderCommand("Select * from keyphrase where id = " + id, "");
                if (checkID.Rows.Count == 0 )
                {
                    throw new BadRequestException("Không tìm thấy ID Keyphrase !", 400, 400);
                }
                string sql = "exec DeleteKeyPhrase " + id;
                _db.ExecuteNonQueryCommand(sql);

            } catch
            {
                throw;
            } finally
            {
                _db.CloseConnection(); 
            }
        }
    }
}
