﻿using LawSearch_Core.Common;
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
                int LawID = 16; //Default

                //Check input
                if (keyPhrase.Keyphrase == null || keyPhrase.Keyphrase == "")
                {
                    throw new BadRequestException("KeyPhrase không được bỏ trống", 400, 400);
                }

                command.CommandText = $"select * from keyphrase where keyphrase = N'{keyPhrase.Keyphrase}'";
                var checkKeyphrase = _db.ExecuteReaderCommand(command, "");
                if (checkKeyphrase.Rows.Count > 0)
                {
                    throw new BadRequestException("Keyphrase đã tồn tại!", 400, 400);
                }

                #region Step 1: Add Keyphrase
                command.CommandText = string.Format("exec GetKeyPhrase N'{0}'", keyPhrase.Keyphrase);
                int id = _db.ExecuteScalarCommand<int>(command);

                command.CommandText = $"select * from [KeyPhrase] where id = {id}";
                DataTable dt = _db.ExecuteReaderCommand(command, "");
                KeyPhrase rsAddKeyphrase = new KeyPhrase
                {
                    ID = Globals.GetIDinDT(dt, 0, "ID"),
                    Keyphrase = Globals.GetinDT_String(dt, 0, "KeyPhrase"),
                    NumberArtical = Globals.GetIDinDT(dt, 0, "NumberArtical"),
                    KeyNorm = Globals.GetinDT_String(dt, 0, "KeyNorm"),
                    LawID = Globals.GetIDinDT(dt, 0, "LawID")
                };
                #endregion

                #region Step 2: Mapping Keyphrase

                #region Get List Artical
                List<Artical> lstArtical = new List<Artical>();
                command.CommandText = "select Content, Content, ID, ChapterID, ChapterItemID from Artical where LawID = " + LawID;
                Console.WriteLine("Start load all artical...\n");
                var dtArticals = _db.ExecuteReaderCommand(command, "");
                for (var i = 0; i < dtArticals.Rows.Count; i++)
                {
                    lstArtical.Add(new Artical
                    {
                        ID = Globals.GetIDinDT(dtArticals, i, "ID"),
                        ChapterID = Globals.GetIDinDT(dtArticals, i, "ChapterID"),
                        ChapterItemID = Globals.GetIDinDT(dtArticals, i, "ChapterItemID"),
                        Content = Globals.GetNormText(Globals.GetinDT_String(dtArticals, i, "Content"))
                    });
                }
                Console.WriteLine("Done load all artical\n");
                #endregion

                //thread-safe bag for load data in multithreading
                ConcurrentBag<KeyphraseMapping> dataCollection = new ConcurrentBag<KeyphraseMapping>();

                Parallel.ForEach(lstArtical, a =>
                {
                    Console.WriteLine($"Thread {Task.CurrentId}: Processing article {a.ID} Mapping keyphrase {rsAddKeyphrase.KeyNorm}");

                    string Normtext = a.Content;
                    int lenghtNormtext = Normtext.Length;
                    string tmp = Normtext.Replace(rsAddKeyphrase.KeyNorm, "");
                    int lengthTMP = tmp.Length;
                    int total = (Normtext.Length - tmp.Length) / rsAddKeyphrase.KeyNorm.Length;
                    if (total > 0)
                    {
                        dataCollection.Add(new KeyphraseMapping(rsAddKeyphrase.ID, a.ChapterID, a.ID, a.ChapterItemID, LawID, total));
                    }
                });

                Console.WriteLine("Total: " + dataCollection.Count);

                List<KeyphraseMapping> keyphraseMappings = new List<KeyphraseMapping>();
                keyphraseMappings = keyphraseMappings.OrderBy(x => x.KeyPhraseID).ToList();

                foreach (var data in keyphraseMappings)
                {
                    command.CommandText = $"insert into KeyPhraseMapping(KeyPhraseID, ChapterID,ChapterItemID,ArticalID, LawID,NumCount) " +
                                          $"values ({data.KeyPhraseID},  {data.ChapterID},  {data.ChapterItemID}, {data.ArticalID}, {data.LawID}, {data.NumCount})";
                    _db.ExecuteNonQueryCommand(command);
                }

                #endregion

                transaction.Commit();

                return rsAddKeyphrase;

            } catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public void DeleteKeyPhrase(int id)
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
                //Check ID
                command.CommandText = $"Select * from keyphrase where id = {id}";
                var checkID = _db.ExecuteReaderCommand(command, "");
                if (checkID.Rows.Count == 0 )
                {
                    throw new BadRequestException("Không tìm thấy ID Keyphrase !", 400, 400);
                }

                //Delete key
                command.CommandText = $"exec DeleteKeyPhrase N'{Globals.GetinDT_String(checkID, 0, "KeyPhrase")}'";
                _db.ExecuteNonQueryCommand(command);

                //Delete mapping
                command.CommandText = $"delete KeyphraseMapping where KeyphraseID = {id}";
                _db.ExecuteNonQueryCommand(command);

                transaction.Commit();

            } catch
            {
                transaction.Rollback() ; 
                throw;
            } finally
            {
                connection.Close() ;
            }
        }

        /*public void GenerateKeyPhraseMapping(int LawID)
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
                command.CommandText = "Select * from keyphrase";
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

                //thread-safe bag for load data in multithreading
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
        }*/

    }
}
