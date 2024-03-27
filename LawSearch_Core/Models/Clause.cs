using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class Clause
    {
        public int ID { get; set; }
        public int ArticalID { get; set; }
        public int Number { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public List<Point> lstPoints { get; set; }
    }
}
