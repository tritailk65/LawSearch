﻿using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Interfaces
{
    public interface IClauseService
    {
        List<Clause> GetListClauseByLawID(int id);
    }
}
