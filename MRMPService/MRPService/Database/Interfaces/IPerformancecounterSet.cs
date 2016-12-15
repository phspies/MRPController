using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.LocalDatabase
{
    public interface IPerformancecounterSet
    {
        IGenericRepository<Performancecounter> ModelRepository { get; }
        void Save();
    }
}
