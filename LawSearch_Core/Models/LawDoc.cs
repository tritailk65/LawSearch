﻿using LawSearch_Core.Common;
using LawSearch_Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class LawDoc
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int LawType { get; set; }
        public string LawNumber {  get; set; }
        public List<Chapter> lstChapters { get; set; }
    }

    public class LawHTML
    {
        public int LawID { get; set; } 
        public string contentHTML { get; set; }
    }

    public class LawImport
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public string LawNumber { get; set; }
        public int LawType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
