using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class Concept
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int type { get; set; }

        public double[] vector;

        public double distance;
    }
}
