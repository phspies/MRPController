using MRMPService.Utilities;
using System;
using System.Collections.Generic;

namespace MRMPService.Modules.Netflow.v9
{
    public class TemplatesV9
    {
        public SyncronisedList<Template> Templates { get; set; }
        public Int32 Count { get { return this.Templates.Count; } }
        public TemplatesV9()
        {
            this.Templates = new SyncronisedList<Template>();
        }
    }
}
