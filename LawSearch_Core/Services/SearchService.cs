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
    public class SearchService : ISearchService
    {
        private readonly IDbService _db;

        public SearchService(IDbService db)
        {
            _db = db;
        }

        public SearchResult SearchLawByText(string searchInput)
        {
            try
            {
                _db.OpenConnection();
                SearchArticalResult rs = new();

                #region Get List KeyPhrase form query
                List<KeyPhrase> lstKeyPhrases_Searched = new();
                string sKey = Globals.GetKeyJoin(searchInput);
                string sqlGetKeyPhrases = "exec GetKeyPharesFromText N'" + sKey + "'";
                DataTable dtKeyPhrases = _db.ExecuteReaderCommand(sqlGetKeyPhrases, "");
                if(dtKeyPhrases.Rows != null && dtKeyPhrases.Rows.Count > 0)
                {
                    for (int i = 0; i < dtKeyPhrases.Rows.Count; i++)
                    {
                        lstKeyPhrases_Searched.Add(new KeyPhrase
                        {
                            ID = Globals.GetIDinDT(dtKeyPhrases, i, "ID"),
                            Key = Globals.GetinDT_String(dtKeyPhrases, i, "KeyPhrase"),
                            Source = (KeyPhraseSource)(Globals.GetIDinDT(dtKeyPhrases, i, "source")),
                            NumberArtical = Globals.GetIDinDT(dtKeyPhrases, i, "NumberArtical"),
                            PosTag = Globals.GetinDT_String(dtKeyPhrases, i, "PosTag"),
                            WordClassWeight = Globals.GetIDinDT(dtKeyPhrases, i, "WordClassWeight"),
                            PositionWeight = 1 // title postion
                        });
                    }
                }

                foreach (var item in lstKeyPhrases_Searched)
                {
                    item.Count = Globals.CountTerm(searchInput, item.Key == null ? "" : item.Key.Replace("_", " "));
                }
                rs.KeyPhrases = lstKeyPhrases_Searched;
                #endregion

                #region Get List Concepts
                List<Concept> lstAllConcept = new List<Concept>();
                string sqlGetConcepts = "select * from [Concept]  with(nolock)";
                DataTable dtConcepts = _db.ExecuteReaderCommand(sqlGetConcepts, "");
                for (int i = 0; i < dtConcepts.Rows.Count; i++)
                {
                    lstAllConcept.Add(new Concept
                    {
                        ID = Globals.GetIDinDT(dtConcepts, i, "ID"),
                        Name = Globals.GetinDT_String(dtConcepts, i, "Name"),
                        Content = Globals.GetinDT_String(dtConcepts, i, "Description"),
                    });
                }
                #endregion

                #region Create candidate concepts
                List<KeyPhraseResult> lstCandidate_Concepts = new();
                for (int i = 0; i < lstAllConcept.Count; i++)
                {
                    lstCandidate_Concepts.Add(new KeyPhraseResult { 
                        ID = lstAllConcept[i].ID, 
                        keys = GetKeyPhrasesByConceptID(lstAllConcept[i].ID) 
                    });
                }
                #endregion

                #region Set i top
                int itopConcept = 3;
                int itopArticals = 10;
                double minScoreConcept = 0.01;
                double minScoreArtical = 0.01;
                #endregion

                #region Get concepts relate to query
                // rs.lstConcepts = TF_IDF.FindNearestNeighbors(lstCandidate_Concepts, lstKeyPhrases_Searched, minScoreConcept, itopConcept);
                rs.lstConcepts = TF_IDF_Improved.FindNearestNeighbors(lstCandidate_Concepts, lstKeyPhrases_Searched, minScoreConcept, itopConcept);
                #endregion

                #region Create candidate articles
                List<KeyPhraseResult> lstCandidates = GetListArticalByConceptID(rs.lstConcepts.Select(x => x.ID).ToList());
                #endregion

                #region Get articles relate to query
                // rs.lstArticals = TF_IDF.FindNearestNeighbors(lstCandidates, lstKeyPhrases_Searched, minScoreArtical, itopArticals);
                rs.lstArticals = TF_IDF_Improved.FindNearestNeighbors(lstCandidates, lstKeyPhrases_Searched, minScoreArtical, itopArticals);

                foreach (var artical in rs.lstArticals)
                {
                    DataTable dsDetail = _db.ExecuteReaderCommand("exec GetArticalDetail2 " + artical.ID,"");
                    artical.Title = Globals.GetinDT_String(dsDetail, 0, "ChapterName") + " - " + Globals.GetinDT_String(dsDetail, 0, "Name");
                    artical.Content = Globals.GetinDT_String(dsDetail, 0, "Title");
                    artical.LawName = Globals.GetinDT_String(dsDetail, 0, "LawName");
                }
                #endregion

                #region Return keyphrases, concepts, articles relate to query
                List<ArticalResult> aRS = new();
                foreach (var item in rs.lstArticals)
                {
                    aRS.Add(new ArticalResult
                    {
                        Title = item.Title,
                        Content = item.Content,
                        LawName = item.LawName,
                        ID = item.ID,
                        Distance = item.distance,
                    });
                }

                List<Concept> cRS = new();             
                foreach(var item in rs.lstConcepts)
                {
                    Concept concept = new();
                    var dt = _db.ExecuteReaderCommand("select * from concept where id = " + item.ID, "");
                    if(dt.Rows.Count > 0)
                    {
                        concept = new Concept
                        {
                            ID = Globals.GetIDinDT(dt,0,"ID"),
                            Name = Globals.GetinDT_String(dt,0,"Name")
                        };
                        cRS.Add(concept);
                    }
                }

                SearchResult sr = new()
                {
                    keyphraseSearch = rs.KeyPhrases,
                    conceptTop = cRS,
                    articalResults = aRS
                };
                #endregion

                return sr;

            } catch
            {
                throw;
            }
            finally
            {
                _db.CloseConnection();
            }
        }

        private List<KeyPhrase> GetKeyPhrasesByConceptID(int conceptID)
        {
            List<KeyPhrase> lst = new();
            DataTable dt = _db.ExecuteReaderCommand("exec GetKeyPharesByConceptID " + conceptID, "");
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string key = Globals.GetinDT_String(dt, i, "KeyPhrase");
                lst.Add(new KeyPhrase
                {
                    ID = Globals.GetIDinDT(dt, i, "ID"),
                    Key = key,
                    Count = Globals.GetIDinDT(dt, i, "Count"),
                    PosTag = Globals.GetinDT_String(dt, i, "PosTag"),
                    PositionWeight = 1, // title position
                    WordClassWeight = Globals.GetIDinDT(dt, i, "WordClassWeight")
                });
            }
            return lst;
        }

        private List<KeyPhraseResult> GetListArticalByConceptID(List<int> lst)
        {
            List<KeyPhraseResult> lstResult = new();
            DataTable dt = _db.ExecuteReaderCommand("Exec GetListArticalKeyPhraseByConceptID '" + string.Join(",", lst) + "'", "");
            
            SortedDictionary<int, List<KeyPhrase>> dic = new();
            KeyPhrase ph;
            for (int i = 0; i < Globals.DTCount(dt); i++)
            {
                int articalID = Globals.GetIDinDT(dt, i, "ArticalID");
                ph = new KeyPhrase
                {
                    ID = Globals.GetIDinDT(dt, i, "KeyPhraseID"),
                    Key = Globals.GetinDT_String(dt, i, "KeyPhrase"),
                    Count = Math.Max(1, Globals.GetIDinDT(dt, i, "NumCount")),
                    PosTag = Globals.GetinDT_String(dt, i, "PosTag"),
                    PositionWeight = Globals.GetIDinDT(dt, i, "PositionWeight"),
                    WordClassWeight = Globals.GetIDinDT(dt, i, "WordClassWeight")
                };

                if (!dic.ContainsKey(articalID))
                {
                    dic.Add( articalID, new List<KeyPhrase> { ph } );
                }
                else
                {
                    dic[articalID].Add(ph);
                }

            }
            foreach (var item in dic.Keys)
            {
                lstResult.Add(new KeyPhraseResult { ID = item, keys = dic[item] });
            }
            return lstResult;
        }
    }
}
