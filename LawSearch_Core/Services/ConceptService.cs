using LawSearch_Core.Common;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LawSearch_Core.Services
{
    public class ConceptService : IConceptService
    {
        private readonly IDbService _db;

        public ConceptService(IDbService db)
        {
            _db = db;
        }

        public Concept AddConcept(Concept concept)
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
                string name = concept.Name;
                string content = concept.Content;
                Concept conceptResult = new Concept();
                
                if(name == "" || content == "")
                {
                    throw new BadRequestException("Name or Content is null!", 400, 400);
                }

                command.CommandText = "select id from Concept where name = N'" + name + "'";
                var checkConcept = _db.ExecuteReaderCommand(command, "");
                if (checkConcept.Rows.Count > 0)
                {
                    throw new BadRequestException("Concept đã tồn tại!", 400, 400);
                }

                command.CommandText = string.Format("insert into Concept(Name, Description) values (N'{0}', N'{1}')", name, content);
                command.ExecuteNonQuery();

                transaction.Commit();

                return conceptResult;            
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<Concept> GetListConcept()
        {
            try
            {
                string sql = "select * from [Concept]  with(nolock)";
                _db.OpenConnection();
                DataTable dt = _db.ExecuteReaderCommand(sql, "");

                List<Concept> lst = new List<Concept>();
                for (int i = 0; i < Globals.DTCount(dt); i++)
                {
                    lst.Add(new Concept
                    {
                        ID = Globals.GetIDinDT(dt, i, "ID"),
                        Name = Globals.GetinDT_String(dt, i, "Name"),
                        Content = Globals.GetinDT_String(dt, i, "Description"),
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

        public Concept UpdateConcept(Concept concept)
        {
            try
            {
                int ID = concept.ID;
                string Name = concept.Name;
                string Content = concept.Content;

                //Check them id o day

                _db.OpenConnection();
                string sql = string.Format("update concept set name = N'{0}',Description =  N'{1}' where ID = {2}", Name, Content, ID);

                _db.ExecuteNonQueryCommand(sql);
                Concept rs = new Concept();
                string queryGetConcept = "select * from [Concept]  with(nolock) where id = " + concept.ID;
                DataTable dt = _db.ExecuteReaderCommand(queryGetConcept, "");
                if(dt.Rows.Count == 0)
                {
                    throw new BadRequestException("Update concept fail !", 400, 400);
                }
                else
                {
                    Concept c = new Concept
                    {
                        ID = Globals.GetIDinDT(dt, 0, "ID"),
                        Name = Globals.GetinDT_String(dt, 0, "Name"),
                        Content = Globals.GetinDT_String(dt, 0, "Description"),
                    };
                    return c;
                }
            }
            catch
            {
                throw;
            } finally
            {
                _db.CloseConnection();
            }
        }

        public void DeleteConcept(int id)
        {
            try
            {
                _db.OpenConnection();
                //check co concept voi id do hay khong

                string sql = string.Format(string.Format("delete from concept where ID = {0}", id));
                _db.ExecuteNonQueryCommand(sql);
            }
            catch
            {
                throw ;
            } finally
            {
                _db.CloseConnection();
            }
        }

        public List<KeyPhrase> GetKeyPhrasesFormConceptID(int id)
        {
            try
            {
                _db.OpenConnection();
                
                //Check concept
                string sql = "exec GetKeyPharesByConceptID " + id;
                DataTable rs = _db.ExecuteReaderCommand(sql, "");
                List<KeyPhrase> keyphraselst = new List<KeyPhrase>();
                for (int i = 0; i < Globals.DTCount(rs); i++)
                {
                    keyphraselst.Add(new KeyPhrase
                    {
                        ID = Globals.GetIDinDT(rs, i, "ID"),
                        Keyphrase = Globals.GetinDT_String(rs, i ,"KeyPhrase"),
                        Count = Globals.GetIDinDT(rs, i, "Count"),
                    });
                }
                return keyphraselst;
                
            } catch
            {
                throw;
            }
            finally
            {
                _db.CloseConnection();
            }
        }

        /// <summary>
        /// Hàm tự sinh ConceptKeyphrase
        /// </summary>
        /// <param name="lawID">ID văn bản luật</param>
        /// <returns></returns>
        public async Task GenerateKeyPhrase(int lawID)
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
                command.CommandText = $"select * from Law where ID = {lawID}";
                var checkIDLaw = _db.ExecuteReaderCommand(command,"");
                if(checkIDLaw.Rows.Count == 0)
                {
                    throw new BadRequestException("ID Law not found !", 400, 400);
                }

                command.CommandText = "select Name, ID, Description from concept";
                DataTable ds = _db.ExecuteReaderCommand(command, "");
                if(ds.Rows.Count > 0)
                {
                    for(var i = 0; i < ds.Rows.Count; i++)
                    {
                        string concept = Globals.GetinDT_String(ds, i, "Name").ToLower();
                        string description = Globals.GetinDT_String(ds, i, "Description").ToLower();
                        int conceptID = Globals.GetIDinDT(ds, i, "ID");

                        //Add name concept to keyphrase
                        command.CommandText = "exec GetKeyPhrase N'" + Globals.GetKeyJoin(concept) + "'";
                        _db.ExecuteNonQueryCommand(command);
                        var keys = await Globals.GetKeyPhraseFromPhoBERT(description);

                        foreach(var key in keys)
                        {
                            int Count = Globals.CountTerm(description, key.Replace("_"," "));
                            command.CommandText = "exec GetKeyPhrase N'" + key + "'";
                            var rsKeyDT = _db.ExecuteReaderCommand(command,"");
                            int idKey = Globals.GetIDinDT(rsKeyDT, 0, "ID");
                            command.CommandText = "exec UpdateConcept_KeyPhrase " + conceptID + "," + Convert.ToInt32(idKey) + "," + lawID + "," + Count;
                            _db.ExecuteNonQueryCommand(command);
                        }
                    }
                }
                transaction.Commit();
            } catch
            {
                transaction.Rollback();
                throw;
            } finally{ _db.CloseConnection(); }
        }

        public List<ConceptKeyphrase> AddConceptKeyphrase(int concept_id, string keyphrase)
        {
            List<ConceptKeyphrase> lst = new();
            int defaultLawID = 16;
            try
            {
                _db.OpenConnection();

                string sql = "select Name, ID, Description from concept where ID = "+concept_id;
                DataTable ds = _db.ExecuteReaderCommand(sql, "");

                if (ds.Rows.Count == 0) return lst;

                sql = "select top 1 KeyPhrase.ID \r\n\tfrom KeyPhrase \r\n\twhere KeyPhrase.KeyPhrase = N'" + keyphrase.Replace(" ", "_") + "'";
                DataTable ds1 = _db.ExecuteReaderCommand(sql, "");
                var keyphraseID = ds1.Rows[0]["ID"];

                if (keyphraseID == null) return lst;

                sql = "select top 1 Concept_KeyPhrase.KeyPhraseID from Concept_KeyPhrase where Concept_KeyPhrase.KeyPhraseID = " + keyphraseID;
                DataTable ds2 = _db.ExecuteReaderCommand(sql, "");
                var existKeyphraseID = Globals.GetIDinDT(ds2, 0, "KeyPhraseID");

                if (existKeyphraseID >= 0) return lst;

                string description = Globals.GetinDT_String(ds, 0, "Description").ToLower();
                int count = Globals.CountTerm(description, keyphrase.Replace("_", " "));
                _db.ExecuteNonQueryCommand("exec UpdateConcept_KeyPhrase " + concept_id + "," + Convert.ToInt32(keyphraseID) + ","+ defaultLawID + "," + count);

                sql = "select * from Concept_KeyPhrase where KeyPhraseID = " + keyphraseID;
                DataTable ds3 = _db.ExecuteReaderCommand(sql, "");

                if (ds3.Rows.Count == 0) return lst;

                lst.Add(
                    new ConceptKeyphrase()
                    {
                        ID = Globals.GetIDinDT(ds3, 0, "ID"),
                        ConceptID = Globals.GetIDinDT(ds3, 0, "ConceptID"),
                        KeyPhraseID = Globals.GetIDinDT(ds3, 0, "KeyPhraseID"),
                        LawID = Globals.GetIDinDT(ds3, 0, "LawID"),
                        Count = Globals.GetIDinDT(ds3, 0, "Count"),
                    }
                );
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

        public class ConceptMapLaw
        {
            public int ConceptID;
            public int LawID;
            public int ChapterID;
            public int ChapterItemID;
            public int ArticalID;
        }

        public void AddConceptMapping (int keyphrase_id)
        {
            try
            {
                _db.OpenConnection();

                string sql = "select DISTINCT Concept_KeyPhrase.ConceptID, KeyPhraseMapping.LawID, " +
                    "KeyPhraseMapping.ChapterID, KeyPhraseMapping.ChapterItemID, KeyPhraseMapping.ArticalID  " +
                    "\r\nfrom KeyPhraseMapping\r\ninner join Concept_KeyPhrase on Concept_KeyPhrase.KeyPhraseID " +
                    "= Concept_KeyPhrase.KeyPhraseID\r\n" +
                    "where Concept_KeyPhrase.KeyPhraseID = " + keyphrase_id + "\r\nORDER BY ConceptID ASC";
                DataTable ds = _db.ExecuteReaderCommand(sql, ""); // [ConceptID, LawID, ChapterID, ChapterItemID, ArticalID]

                if(ds.Rows.Count == 0)
                {
                    throw new BadRequestException("This keyphrase is created in [Concept_Keyphrase] yet", 400);
                }

                List<ConceptMapLaw> lst = new();

                for (int i = 0; i < ds.Rows.Count; i++)
                {
                    lst.Add(new ConceptMapLaw()
                    {
                        ConceptID = Globals.GetIDinDT(ds, i, "ConceptID"),
                        LawID = Globals.GetIDinDT(ds, i, "LawID"),
                        ChapterID = Globals.GetIDinDT(ds, i, "ChapterID"),
                        ChapterItemID = Globals.GetIDinDT(ds, i, "ChapterItemID"),
                        ArticalID = Globals.GetIDinDT(ds, i, "ArticalID")
                    });
                }

                foreach(var cml in lst)
                {
                    sql = "select * from ConceptMapping " +
                        "where ConceptID = " + cml.ConceptID + " and LawID = " + cml.LawID + " and ChapterID = " + cml.ChapterID +
                        " and ChapterItemID = " + cml.ChapterItemID + " and ArticalID = " + cml.ArticalID;
                    DataTable ds1 = _db.ExecuteReaderCommand(sql, "");

                    if(ds1.Rows.Count == 0)
                    {
                        sql = "INSERT INTO [dbo].[ConceptMapping] ([ConceptID] ,[ArticalID] ,[ChapterID] ,[ChapterItemID] ,[ClaustID] ,[PointID] ,[LawID] ,[KeyPhraseID]) " +
                            "VALUES ("+ cml.ConceptID + ","+ cml.ArticalID + " ,"+ cml.ChapterID + " ,"+ cml.ChapterItemID + " ,0 ,0 ,"+ cml.LawID + " , NULL)";
                        _db.ExecuteReaderCommand(sql, "");
                    }
                }
            }
        }

        /// <summary>
        /// Hàm tự sinh ConceptMapping (Multithreading task)
        /// </summary>
        /// <param name="LawID">ID văn bản luật</param>
        /// <exception cref="BadRequestException"></exception>
        public void GenerateConceptMapping(int LawID)
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
                if(checkLawID.Rows.Count == 0)
                {
                    throw new BadRequestException("LawID not found!", 400, 400);
                }
                #endregion

                #region Get List Concept
                Console.WriteLine("Start load Concept...");
                List<Concept> lstConcepts = new List<Concept>();
                command.CommandText = "select dbo.getnormtext(Name) Name, ID from Concept";
                var dtConcepts = _db.ExecuteReaderCommand(command, "");
                if(dtConcepts.Rows.Count > 0)
                {
                    for (var i = 0;i< dtConcepts.Rows.Count; i++)
                    {
                        lstConcepts.Add(new Concept
                        {
                            ID = Globals.GetIDinDT(dtConcepts, i, "ID"),
                            Name = Globals.GetinDT_String(dtConcepts, i, "Name"),
                        });
                    }
                }
                Console.WriteLine("Done load all Concept");
                #endregion

                #region Get List Artical
                Console.WriteLine("Start load Artical...");
                List<Artical> lstArticals = new List<Artical>();
                command.CommandText = $"select dbo.getnormtext(Content) Content, ID, ChapterID, ChapterItemID from Artical where LawID = {LawID}";
                var dtArtical = _db.ExecuteReaderCommand(command, "");               
                if(dtArtical.Rows.Count > 0)
                {
                    for(var i = 0; i < dtArtical.Rows.Count; i++)
                    {
                        lstArticals.Add(new Artical
                        {
                            ID = Globals.GetIDinDT(dtArtical, i, "ID"),
                            ChapterID = Globals.GetIDinDT(dtArtical, i, "ChapterID"),
                            ChapterItemID = Globals.GetIDinDT(dtArtical, i, "ChapterItemID"),
                            Content = Globals.GetinDT_String(dtArtical, i, "Content")
                        });
                    }
                }
                Console.WriteLine("Done load all artical\n");
                #endregion

                //thread-safe bag for load data in multithreading
                ConcurrentBag<ConceptMapping> dataCollection = new ConcurrentBag<ConceptMapping>();

                foreach (Concept concept in lstConcepts)
                {
                    command.CommandText = $"select top 1 id from ConceptMapping where ConceptID = {concept.ID} and LawID = {LawID}";
                    var checkIfMapped = _db.ExecuteReaderCommand(command, "");
                    if(checkIfMapped.Rows.Count > 0)
                    {
                        continue;
                    }

                    Parallel.ForEach(lstArticals, a =>
                    {
                        Console.WriteLine($"Thread {Task.CurrentId}: Processing article {a.ID} Mapping Concept {concept.Name}");

                        string Normtext = a.Content;
                        int lenghtNormtext = Normtext.Length;
                        string tmp = Normtext.Replace(concept.Name, "");
                        int lengthTMP = tmp.Length;
                        int total = (Normtext.Length - tmp.Length) / concept.Name.Length;
                        if (total > 0)
                        {
                            dataCollection.Add(new ConceptMapping(concept.ID,a.ID,a.ChapterID,a.ChapterItemID,0,0,LawID));
                        }
                    });
                }

                Console.WriteLine("Total: " + dataCollection.Count);
                foreach (ConceptMapping concept in dataCollection)
                {
                    Console.WriteLine(""+concept.ID+"/"+concept.ArticalID+"/"+concept.ChapterID+"/"+concept.ChapterItemID+"/");
                }

                foreach (var data in dataCollection)
                {
                    command.CommandText = $"insert into ConceptMapping (ConceptID, ChapterID, ChapterItemID, ArticalID, LawID, ClaustID, PointID) " +
                                          $"values ({data.ConceptID},  {data.ChapterID},  {data.ChapterItemID}, {data.ArticalID}, {LawID}, 0,0)";
                    _db.ExecuteNonQueryCommand(command);
                }
                transaction.Commit();
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

        /// <summary>
        /// Xóa tất cả ConceptMapping
        /// </summary>
        public void DeleteAllConceptMapping()
        {
            try
            {
                _db.OpenConnection();
                string sql = $"delete ConceptMapping where id in (select id from ConceptMapping)";
                _db.ExecuteNonQueryCommand(sql);           
            }catch
            {
                throw;
            } finally { _db.CloseConnection(); }
        }

        /// <summary>
        /// Xóa ConceptMapping theo ID
        /// </summary>
        /// <param name="ConceptID"></param>
        public void DeleteConceptMappingByConceptID(int ConceptID)
        {
            try
            {
                _db.OpenConnection();

                var checkIDConcept = _db.ExecuteReaderCommand($"Select * from Concept where id = {ConceptID}","");
                if(checkIDConcept.Rows.Count == 0)
                {
                    throw new BadRequestException("ConceptID not found!", 400, 400);
                }

                string sql = $"delete ConceptMapping where id in (select id from ConceptMapping where ConceptID = {ConceptID})";
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
