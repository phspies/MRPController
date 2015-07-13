using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.CaaS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CloudMoveyWorkerService.CaaS1
{
    class ServerObject : Core
    {
        public ServerObject(DimensionData _dimensiondata) : base(_dimensiondata) { }

        /// <summary>
        /// List platform servers
        /// Available options:
        ///     Paging/Ordering Optional Parameters:
        ///         &pageSize=
        ///         &pageNumber=
        ///         &orderBy=
        ///     Filter Optional Parameters:
        ///         &id=
        ///         &location=
        ///         &name=
        ///         &created=
        ///         &state=
        ///         &operatingSystemId=
        ///         &operatingSystemFamily=
        /// </summary>
        /// <param name="Options"></param>
        /// <returns></returns>
        public ServersWithBackup platformservers(List<Option> options = null)
        {
            orgendpoint("/serverWithBackup");
            urloptions = options;
            ServersWithBackup softwarelabels = get<ServersWithBackup>(null, true) as ServersWithBackup;
            return softwarelabels;
        }
    }
}