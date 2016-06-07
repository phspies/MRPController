
using MRMPService.Utiliies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wadltocsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            WadlProcess _process = new WadlProcess("application.wadl");
        }
    }
}
