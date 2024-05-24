using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Interfaces
{
    public interface IArticalService
    {
        ArticalDetail GetArticalDetail(int id);
        List<Artical> GetAllArtical();
        List<Artical> GetListArticalByLawID(int lawID);
        void EditContentArtical(Artical artical);
    }
}
