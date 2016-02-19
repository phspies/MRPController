using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.LocalDatabase
{
    public interface IPerformanceSet
    {
        IGenericRepository<Performance> ModelRepository { get; }
        void Save();
    }
}
