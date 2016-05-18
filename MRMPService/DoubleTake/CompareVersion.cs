using System;
using System.Collections.Generic;
using System.Linq;

namespace MRMPService.DoubleTake
{
    class Versions
    {
        public static int Compare(string sa, string sb)
        {
            Func<string, int?> parse = s => { int ret; return int.TryParse(s, out ret) ? (int?)ret : null; };

            Func<string, IEnumerable<int>> f = s => s.Split('.').Select(t => int.Parse(t));

            var diff = f(sa).Zip(f(sb), (a, b) => new { a, b }).FirstOrDefault(x => x.a != x.b);

            return diff == null ? 0 : diff.a < diff.b ? -1 : 1;
        }
    }
}
