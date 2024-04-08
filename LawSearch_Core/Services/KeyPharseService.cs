using LawSearch_Core.Common;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using System;
using System.Collections.Concurrent;
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
                string sql = "exec DeleteKeyPhrase N'" + Globals.GetinDT_String(checkID,0,"KeyPhrase") + "'";
                _db.ExecuteNonQueryCommand(sql);

            } catch
            {
                throw;
            } finally
            {
                _db.CloseConnection(); 
            }
        }

        public void GenerateKeyPhraseMapping(int LawID)
        {
            #region Transaction init
            IDbConnection connection = _db.GetDbConnection();
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message.ToString(), 500);
            }
            IDbCommand command = _db.CreateCommand();
            IDbTransaction transaction = _db.BeginTransaction();
            command.Connection = connection;
            command.Transaction = transaction;
            #endregion

            try
            {
                #region Check Data
                command.CommandText = $"select * from Law where id = {LawID}";
                var checkLawID = _db.ExecuteReaderCommand(command, "");
                if (checkLawID.Rows.Count == 0)
                {
                    throw new BadRequestException("LawID not found!", 400, 400);
                }
                #endregion

                #region Get List KeyPhrase
                List<KeyPhrase> keyPhrases = new List<KeyPhrase>();               
                command.CommandText = "Select * from keyphrase where id <= 1543  ";
                var dtKeyPhrase = _db.ExecuteReaderCommand(command,"");
                if(dtKeyPhrase.Rows.Count > 0)
                {
                    for (int i = 0; i < dtKeyPhrase.Rows.Count; i++)
                    {
                        keyPhrases.Add(new KeyPhrase
                        {
                            ID = Globals.GetIDinDT(dtKeyPhrase, i, "ID"),
                            Keyphrase = Globals.GetinDT_String(dtKeyPhrase, i, "Keyphrase"),
                            KeyNorm = Globals.GetinDT_String(dtKeyPhrase, i, "KeyNorm")
                        });
                    }
                }
                #endregion

                #region Get List Artical
                List<Artical> lstArtical = new List<Artical>();
                command.CommandText = "select dbo.getnormtext(Content) Content, ID, ChapterID, ChapterItemID from Artical where LawID = " + LawID;
                Console.WriteLine("Start load all artical...\n");
                var dtArticals = _db.ExecuteReaderCommand(command, "");
                for (var i = 0; i < dtArticals.Rows.Count; i++)
                {
                    lstArtical.Add(new Artical
                    {
                        ID = Globals.GetIDinDT(dtArticals, i, "ID"),
                        ChapterID = Globals.GetIDinDT(dtArticals, i, "ChapterID"),
                        ChapterItemID = Globals.GetIDinDT(dtArticals, i, "ChapterItemID"),
                        Content = Globals.GetinDT_String(dtArticals, i, "Content")
                    });
                }
                Console.WriteLine("Done load all artical\n");
                #endregion

                ConcurrentBag<KeyphraseMapping> dataCollection = new ConcurrentBag<KeyphraseMapping>();

                foreach (var keyPhrase in keyPhrases)
                {
                    command.CommandText = $"Select top 1 id from KeyPhraseMapping where KeyPhraseID = {keyPhrase.ID} and LawID = {LawID}";
                    var checkIfKeyMapped = _db.ExecuteReaderCommand(command, "");
                    if (checkIfKeyMapped.Rows.Count > 0)
                    {
                        continue;
                    }

                    Parallel.ForEach(lstArtical, a =>
                    {
                        Console.WriteLine($"Thread {Task.CurrentId}: Processing article {a.ID} Mapping keyphrase {keyPhrase.KeyNorm}");

                        string Normtext = a.Content;
                        int lenghtNormtext = Normtext.Length;
                        string tmp = Normtext.Replace(keyPhrase.KeyNorm, "");
                        int lengthTMP = tmp.Length;
                        int total = (Normtext.Length - tmp.Length) / keyPhrase.KeyNorm.Length;
                        if (total > 0)
                        {
                            dataCollection.Add(new KeyphraseMapping(keyPhrase.ID, a.ChapterID, a.ID,a.ChapterItemID,16,total));
                        }
                    });
                }
                Console.WriteLine("Total: "+dataCollection.Count);

                foreach (var data in dataCollection)
                {
                    command.CommandText = $"insert into KeyPhraseMapping(KeyPhraseID, ChapterID,ChapterItemID,ArticalID, LawID,NumCount) " +
                                          $"values ({data.KeyPhraseID},  {data.ChapterID},  {data.ChapterItemID}, {data.ArticalID}, {LawID}, {data.NumCount})";
                    _db.ExecuteNonQueryCommand(command);
                }


                /*  #region Tested
                    int KeyID = generateKeyphrase.KeyID;
                    int LawID = generateKeyphrase.LawID;

                    command.CommandText = $"select top 1 ID from KeyPhraseMapping where KeyPhraseID = {KeyID} and LawID = {LawID}";
                    var checkIfKeyMapped = _db.ExecuteReaderCommand(command, "");
                    if(checkIfKeyMapped.Rows.Count > 0)
                    {
                        return;
                    }

                    #region Get Keyphrase
                    KeyPhrase keyphrase = new KeyPhrase();

                    command.CommandText = "select * from Keyphrase where id = " + KeyID;
                    var dtKeyPhrase = _db.ExecuteReaderCommand(command,"");
                    if (dtKeyPhrase.Rows.Count == 0)
                    {
                        throw new BadRequestException("KeyphraseID not found!", 400, 400);
                    }
                    keyphrase = new KeyPhrase
                    {
                        ID = Globals.GetIDinDT(dtKeyPhrase, 0, "ID"),
                        Keyphrase = Globals.GetinDT_String(dtKeyPhrase, 0, "Keyphrase"),
                        KeyNorm = Globals.GetinDT_String(dtKeyPhrase, 0, "KeyNorm")
                    };
                    #endregion

                    #region Get List Artical
                    List<Artical> lstArtical = new List<Artical>();
                    command.CommandText = "select * from Artical where LawID = " + LawID;
                    var dtArticals = _db.ExecuteReaderCommand(command, "");
                    if(dtArticals.Rows.Count == 0)
                    {
                        throw new BadRequestException("LawID not found!", 400, 400);
                    }
                    for(var i = 0; i < dtArticals.Rows.Count; i++)
                    {
                        lstArtical.Add(new Artical
                        {
                            ID = Globals.GetIDinDT(dtArticals, i, "ID"),
                            ChapterID = Globals.GetIDinDT(dtArticals, i, "ChapterID"),
                            ChapterItemID = Globals.GetIDinDT(dtArticals,i,"ChapterItemID"),
                            Content = Globals.GetinDT_String(dtArticals,i, "Content")
                        });
                    }
                    #endregion

                    foreach(var a in lstArtical)
                    {
                        command.CommandText = "select dbo.getnormtext(N'" + a.Content + "') normtext";
                        string Normtext = Globals.GetinDT_String(_db.ExecuteReaderCommand(command, ""), 0, "normtext");
                        int lenghtNormtext = Normtext.Length;

                        string tmp = Normtext.Replace(keyphrase.KeyNorm, "");
                        int lengthTMP = tmp.Length;
                        int total = (Normtext.Length - tmp.Length) / keyphrase.KeyNorm.Length;
                        if(total > 0)
                        {
                            command.CommandText = $"insert into KeyPhraseMapping(KeyPhraseID, ChapterID,ChapterItemID,ArticalID, LawID,NumCount) " +
                                                    $"values ({keyphrase.ID},  {a.ChapterID},  {a.ChapterItemID}, {a.ID}, {LawID}, {total})";
                            _db.ExecuteNonQueryCommand(command) ;
                        }
                    }
                    #endregion */

                transaction.Commit(); 
            }
            catch
            {
                transaction.Rollback();
                throw;
            }finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Xóa tất cả KeyphraseMapping
        /// </summary>
        public void DeleteAllKeyphraseMapping()
        {
            try
            {
                _db.OpenConnection();
                string sql = $"delete KeyphraseMapping where id in (select id from KeyphraseMapping)";
                _db.ExecuteNonQueryCommand(sql);
            }
            catch
            {
                throw;
            }
            finally { _db.CloseConnection(); }
        }

        /// <summary>
        /// Xóa KeyphraseMapping theo KeyphraseID
        /// </summary>
        /// <param name="KeyphraseID"></param>
        public void DelettKeyphraseMapping(int KeyphraseID)
        {
            try
            {
                _db.OpenConnection();

                var checkIDConcept = _db.ExecuteReaderCommand($"Select * from Keyphrase where id = {KeyphraseID}", "");
                if (checkIDConcept.Rows.Count == 0)
                {
                    throw new BadRequestException("KeyphraseID not found!", 400, 400);
                }

                string sql = $"delete KeyphraseMapping where id in (select id from KeyphraseMapping where KeyphraseID = {KeyphraseID})";
                _db.ExecuteNonQueryCommand(sql);
            }
            catch
            {
                throw;
            }
            finally { _db.CloseConnection(); }
        }
    }
}
