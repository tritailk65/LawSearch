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
        /// Hàm tự sinh Concept_Keyphrase (Multithreading task)
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
