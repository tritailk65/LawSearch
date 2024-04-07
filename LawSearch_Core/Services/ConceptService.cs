using LawSearch_Core.Common;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using System;
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
            try
            {
                _db.OpenConnection();

                string name = concept.Name;
                string content = concept.Content;

                string sql = string.Format("Exec GetConcept N'{0}',N'{1}'", name, content);
                var rs = _db.ExecuteReaderCommand(sql,"");
                int idNewConcept = 0;
                if(rs.Rows.Count > 0)
                {
                    idNewConcept = Globals.GetIDinDT(rs, 0, 0);
                }
                if(idNewConcept == -1)
                {
                    throw new BadRequestException("Name or content is null !", 400, 400);
                }else if(idNewConcept != 0)
                {
                    string query = "select * from [Concept]  with(nolock) where id = " + idNewConcept;
                    DataTable dt = _db.ExecuteReaderCommand(query, "");
                    Concept c = new Concept
                    {
                        ID = Globals.GetIDinDT(dt, 0, "ID"),
                        Name = Globals.GetinDT_String(dt, 0, "Name"),
                        Content = Globals.GetinDT_String(dt, 0, "Description"),
                    };
                    return c;
                }
                return null;
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

        //Generate từ name và description
        public async Task GenerateKeyPhrase()
        {
            try
            {
                _db.OpenConnection();

                string sql = "select Name, ID, Description from concept";
                DataTable ds = _db.ExecuteReaderCommand(sql, "");
                if(ds.Rows.Count > 0)
                {
                    for(var i = 0; i < ds.Rows.Count; i++)
                    {
                        string concept = Globals.GetinDT_String(ds, i, "Name").ToLower();
                        string description = Globals.GetinDT_String(ds, i, "Description").ToLower();
                        int conceptID = Globals.GetIDinDT(ds, i, "ID");

                        //Add name concept to keyphrase
                        _db.ExecuteNonQueryCommand("exec GetKeyPhrase N'" + Globals.GetKeyJoin(concept) + "'");
                        var data = await Globals.GetKeyPhraseFromPhoBERT(description);
                        var keys = data;
                        foreach(var key in keys)
                        {
                            int Count = Globals.CountTerm(description, key.Replace("_"," "));
                            var rsKeyDT = _db.ExecuteReaderCommand("exec GetKeyPhrase N'" + key + "'","");
                            int idKey = Globals.GetIDinDT(rsKeyDT, 0, "ID");
                            _db.ExecuteNonQueryCommand("exec UpdateConcept_KeyPhrase " + conceptID + "," + Convert.ToInt32(idKey) + ",16," + Count);
                        }
                    }
                }
            } catch
            {
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
    }
}
