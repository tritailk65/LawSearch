using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class SearchResult
    {
        public List<KeyPhrase> keyphraseSearch { get; set; }
        public List<ArticalResult> articalResults { get; set; }
        public List<Concept> conceptTop {  get; set; }  
    }
}
