using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.NetFlowv10
{
    public class TemplatesV10
    {
        public List<Template> Templats { get; set; }
        public Int32 Count { get { return this.Templats.Count; } }
        public TemplatesV10()
        {
            this.Templats = new List<Template>();
        }
    }
}
