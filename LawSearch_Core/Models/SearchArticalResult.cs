using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class SearchArticalResult
    {
        public List<KeyPhrase> KeyPhrases;
        public List<KeyPhraseResult> lstConcepts;
        public List<KeyPhraseResult> lstArticals;
    }
}
