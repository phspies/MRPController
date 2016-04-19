using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.NetFlowv9
{
    public class TemplatesV9
    {
        public List<Template> Templats { get; set; }
        public Int32 Count { get { return this.Templats.Count; } }
        public TemplatesV9()
        {
            this.Templats = new List<Template>();
        }
    }
}
