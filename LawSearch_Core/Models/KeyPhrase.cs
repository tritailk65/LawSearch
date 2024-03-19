using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class KeyPhrase
    {
        public int ID { get; set; }
        public string Keyphrase {  get; set; }
        public int? source { get; set; } = null;
        public int? ConceptID { get; set; } = null;
        public int? NumberArtical { get; set; } = null;
        public string KeyNorm { get; set; }
        public int? LawID { get; set; } = null;

    }

    public class KeyPhraseRelate
    {
        public int KeyPhraseID { get; set; }
        public string KeyPhrase { get; set; }
        public string ArticalName { get; set; }
        public int ArticalID { get; set; }
        public int NumCount { get; set; }
        public string ChapterName { get; set; }
    }
}
