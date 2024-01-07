using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteS4E.Services
{
    internal interface IDbService
    {
        void Commit();
        void Rollback();
    }
}
