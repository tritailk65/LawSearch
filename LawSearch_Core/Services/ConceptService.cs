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

        public List<ConceptKeyphraseShow> GetConceptKeyphraseByConceptID(int id)
        {
            try
            {
                _db.OpenConnection();

                string sql = "select Concept_KeyPhrase.ID, KeyPhrase.KeyPhrase, KeyPhrase.KeyNorm from Concept_KeyPhrase inner join KeyPhrase on Concept_KeyPhrase.KeyPhraseID = KeyPhrase.ID where Concept_KeyPhrase.ConceptID = " + id;
                DataTable rs = _db.ExecuteReaderCommand(sql, "");
                List<ConceptKeyphraseShow> lst = new List<ConceptKeyphraseShow>();
                for (int i = 0; i < Globals.DTCount(rs); i++)
                {
                    lst.Add(new ConceptKeyphraseShow
                    {
                        ID = Globals.GetIDinDT(rs, i, "ID"),
                        KeyPhrase = Globals.GetinDT_String(rs, i, "KeyPhrase"),
                        KeyNorm = Globals.GetinDT_String(rs, i, "KeyNorm")
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

        public async Task GenerateKeyphraseDescript(int lawID)
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
                        command.CommandText = $"exec GetKeyPhrase N'{Globals.GetKeyJoin(concept)}', 'N'";
                        _db.ExecuteNonQueryCommand(command);

                        //Get keyphrase from VNCoreNLP
                        List<KeyphraseGenerateResponse> keyphraseGenerateResponses = new List<KeyphraseGenerateResponse>();
                        var keys = await Globals.GetKeyPhraseFromPhoBERT(description);
                        keyphraseGenerateResponses = keys;

                        foreach (var key in keyphraseGenerateResponses)
                        {
                            int Count = Globals.CountTerm(description, key.Key.Replace("_", " "));
                            command.CommandText = $"exec GetKeyPhrase N'{key.Key}', '{key.Pos}'";
                            var rsKeyDT = _db.ExecuteReaderCommand(command,"");
                            int idKey = Globals.GetIDinDT(rsKeyDT, 0, "ID");
                            command.CommandText = $"exec UpdateConcept_KeyPhrase {conceptID}, {Convert.ToInt32(idKey)}, {lawID}, {Count}";
                            _db.ExecuteNonQueryCommand(command);
                        }
                    }
                }
                transaction.Commit();
            } catch
            {
                transaction.Rollback();
                throw;
            } finally{ connection.Close(); }
        }

        public async void AddConceptKeyphrase(int ConceptID, string Keyphrase)
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
                int LawID = 16;

                //Check ID concept 
                command.CommandText = $"select * from concept where id = {ConceptID}";
                var checkConceptID = _db.ExecuteReaderCommand(command, "");
                if (checkConceptID.Rows.Count == 0)
                {
                    throw new BadRequestException("ConceptID not found!", 400, 400);
                }

                #region Step 1: Add KeyPhrase
                if (Keyphrase == null || Keyphrase == "")
                {
                    throw new BadRequestException("KeyPhrase is null or empty", 400, 400);
                }
                command.CommandText = string.Format("exec GetKeyPhrase N'"+ Keyphrase + "', 'N'");
                DataTable dt = _db.ExecuteReaderCommand(command, "");
                int id = Globals.GetIDinDT(dt, 0, 0);

                command.CommandText = $"select * from [KeyPhrase] where id = {id}";
                dt = _db.ExecuteReaderCommand(command, "");
                KeyPhrase rsAddKeyphrase = new KeyPhrase
                {
                    ID = Globals.GetIDinDT(dt, 0, "ID"),
                    Keyphrase = Globals.GetinDT_String(dt, 0, "KeyPhrase"),
                    NumberArtical = Globals.GetIDinDT(dt, 0, "NumberArtical"),
                    KeyNorm = Globals.GetinDT_String(dt, 0, "KeyNorm"),
                    LawID = Globals.GetIDinDT(dt, 0, "LawID")
                };
                #endregion

                #region Step 2: Add ConceptKeyphrase
                command.CommandText = $"exec UpdateConcept_KeyPhrase {ConceptID}, {rsAddKeyphrase.ID}, {LawID}, 1";
                _db.ExecuteNonQueryCommand(command);
                #endregion

                #region Step 3: Generate KeyphraseMapping and ConceptMapping

                command.CommandText = $"select * from KeyphraseMapping where keyphraseID = {rsAddKeyphrase.ID}";
                var checkKeyphrase = _db.ExecuteReaderCommand(command, "");
                if (checkKeyphrase.Rows.Count > 0)
                {
                    transaction.Commit();
                    return;
                }

                #region Get List Artical
                List<Artical> lstArtical = new List<Artical>();
                command.CommandText = $"select Name, Title, Content, ID, ChapterID, ChapterItemID from Artical where LawID = {LawID}";
                Console.WriteLine("Start load all artical...\n");
                var dtArticals = _db.ExecuteReaderCommand(command, "");
                for (var i = 0; i < dtArticals.Rows.Count; i++)
                {
                    string title = Globals.GetinDT_String(dtArticals, i, "Name") + " " + Globals.GetinDT_String(dtArticals, i, "Title");

                    lstArtical.Add(new Artical
                    {
                        ID = Globals.GetIDinDT(dtArticals, i, "ID"),
                        ChapterID = Globals.GetIDinDT(dtArticals, i, "ChapterID"),
                        ChapterItemID = Globals.GetIDinDT(dtArticals, i, "ChapterItemID"),
                        Content = Globals.GetNormText(Globals.GetinDT_String(dtArticals, i, "Content")),
                        Title = Globals.GetNormText(title)
                    });
                }
                Console.WriteLine("Done load all artical\n");
                #endregion

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
                        //Calculate bt
                        string contentTMP = a.Content.Replace(a.Title, "");
                        int countTermTitle = Globals.CountTerm(a.Title, rsAddKeyphrase.KeyNorm);
                        int countTermContent = Globals.CountTerm(contentTMP, rsAddKeyphrase.KeyNorm);
                        float positionWeight = (countTermTitle + (countTermContent * 0.5F)) / total;

                        dataCollection.Add(new KeyphraseMapping(rsAddKeyphrase.ID, a.ChapterID, a.ID, a.ChapterItemID, LawID, total));
                    }
                });

                Console.WriteLine("Total: " + dataCollection.Count);

                List<KeyphraseMapping> keyphraseMappings = new List<KeyphraseMapping>();
                keyphraseMappings = dataCollection.OrderBy(x => x.KeyPhraseID).ToList();

                foreach (var data in keyphraseMappings)
                {
                    command.CommandText = $"insert into KeyPhraseMapping(KeyPhraseID, ChapterID, ChapterItemID, ArticalID, LawID, NumCount, PositionWeight) " +
                                          $"values ({data.KeyPhraseID},  {data.ChapterID},  {data.ChapterItemID}, {data.ArticalID}, {LawID}, {data.NumCount}, {data.PositionWeight})";
                    _db.ExecuteNonQueryCommand(command);

                    command.CommandText = $"insert into ConceptMapping (ConceptID, ChapterID, ChapterItemID, ArticalID, LawID, ClauseID, PointID, KeyphraseID) " +
                                          $"values ({ConceptID},  {data.ChapterID},  {data.ChapterItemID}, {data.ArticalID}, {LawID}, 0,0, {rsAddKeyphrase.ID})";
                    _db.ExecuteNonQueryCommand(command);
                }
                #endregion

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                transaction.Rollback();
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public void DeleteConceptKeyphrase(int ConceptKeyphraseID)
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
                command.CommandText = $"delete Concept_Keyphrase where ID = {ConceptKeyphraseID}";
                _db.ExecuteNonQueryCommand(command);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            } finally
            {
                connection.Close(); 
            }
        }

        public void DeleteAllInfoRelateToConceptKeyphrase(int KeyphraseID)
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
                //Check KeyphraseID
                command.CommandText = $"Select * from Keyphrase where id = {KeyphraseID}";
                var checkKeyphraseID = _db.ExecuteReaderCommand(command, "");
                if (checkKeyphraseID.Rows.Count == 0)
                {
                    throw new BadRequestException("KeyphraseID not found!", 400, 400);
                }

                command.CommandText = $"delete Keyphrase where id = {KeyphraseID}";
                _db.ExecuteNonQueryCommand(command);

                command.CommandText = $"delete Concept_Keyphrase where keyphraseID = {KeyphraseID}";
                _db.ExecuteNonQueryCommand(command);

                command.CommandText = $"delete KeyphraseMapping where keyphraseID = {KeyphraseID}";
                _db.ExecuteNonQueryCommand(command);

                command.CommandText = $"delete ConceptMapping where keyphraseID = {KeyphraseID}";
                _db.ExecuteNonQueryCommand(command);

                transaction.Commit();
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

        /// <summary>
        /// Hàm tự sinh ConceptMapping với đầu vào là ID văn bản luật
        /// Note: Dùng khi muốn mapping concept(name) với những văn bản khác ngoài luật bđs 2024
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
                if (checkLawID.Rows.Count == 0)
                {
                    throw new BadRequestException("LawID not found!", 400, 400);
                }
                #endregion

                #region Get List Concept
                Console.WriteLine("Start load Concept...");
                List<Concept> lstConcepts = new List<Concept>();
                command.CommandText = "select dbo.getnormtext(Name) Name, ID from Concept";
                var dtConcepts = _db.ExecuteReaderCommand(command, "");
                if (dtConcepts.Rows.Count > 0)
                {
                    for (var i = 0; i < dtConcepts.Rows.Count; i++)
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
                command.CommandText = $"select Content, Content, ID, ChapterID, ChapterItemID from Artical where LawID = {LawID}";
                var dtArtical = _db.ExecuteReaderCommand(command, "");
                if (dtArtical.Rows.Count > 0)
                {
                    for (var i = 0; i < dtArtical.Rows.Count; i++)
                    {
                        lstArticals.Add(new Artical
                        {
                            ID = Globals.GetIDinDT(dtArtical, i, "ID"),
                            ChapterID = Globals.GetIDinDT(dtArtical, i, "ChapterID"),
                            ChapterItemID = Globals.GetIDinDT(dtArtical, i, "ChapterItemID"),
                            Content = Globals.GetNormText(Globals.GetinDT_String(dtArtical, i, "Content"))
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
                    if (checkIfMapped.Rows.Count > 0)
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
                            dataCollection.Add(new ConceptMapping(concept.ID, a.ID, a.ChapterID, a.ChapterItemID, 0, 0, LawID));
                        }
                    });
                }

                Console.WriteLine("Total: " + dataCollection.Count);

                List<ConceptMapping> conceptMappings = new List<ConceptMapping>();
                conceptMappings = dataCollection.OrderBy(x => x.ConceptID).ToList();

                foreach (var data in conceptMappings)
                {
                    command.CommandText = $"insert into ConceptMapping (ConceptID, ChapterID, ChapterItemID, ArticalID, LawID, ClauseID, PointID) " +
                                          $"values ({data.ConceptID},  {data.ChapterID},  {data.ChapterItemID}, {data.ArticalID}, {LawID}, 0,0)";
                    _db.ExecuteNonQueryCommand(command);
                }
                transaction.Commit();
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
    }
}
