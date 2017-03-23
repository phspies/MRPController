using MRMPService.Utilities;
using System;
using System.Collections.Generic;

namespace MRMPService.Modules.Netflow.v10
{
    public class TemplatesV10
    {
        public SyncronisedList<Template> Templates { get; set; }
        public Int32 Count { get { return this.Templates.Count; } }
        public TemplatesV10()
        {
            this.Templates = new SyncronisedList<Template>();
        }
    }
}
