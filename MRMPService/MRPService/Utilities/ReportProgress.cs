using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.Utilities
{
    class ReportProgress
    {
        public static float Progress(float _start, float _end, float _current)
        {
            float _range = _end - _start;
            float _result = ((_range / 100) * _current) + _start;
            return _result;
        }
    }
}
