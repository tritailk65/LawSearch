using LawSearch_Core.Common;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
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


    }
}
