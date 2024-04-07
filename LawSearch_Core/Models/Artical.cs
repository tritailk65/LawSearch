using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class Artical
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public string Content { get; set; }
        public int ChapterItemID { get; set; }
        public int ChapterID { get; set; }
        public int LawID { get; set; }
        public int ChapterNumber { get; set; }
        public double score { get; set; }
        public List<Clause> lstClause { get; set; }
    }

    public class ArticalResult
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public string LawName { get; set; } 
        public double Score { get; set; }
        public double Distance { get; set; }
        public double[] Vector { get; set; }

        public List<KeyPhrase> KeyPhrases { get; set; }
    }
}
