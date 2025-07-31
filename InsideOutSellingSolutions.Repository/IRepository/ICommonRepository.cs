using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsideOutSellingSolutions.Repository.IRepository
{
    public interface ICommonRepository
    {
        public void Errorlog(string ProcedureName, string StackTrace, string ErrorMessage);
    }
}
