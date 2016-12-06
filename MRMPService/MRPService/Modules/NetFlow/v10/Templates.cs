using System;
using System.Collections.Generic;

namespace MRMPService.Modules.Netflow.v10
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
