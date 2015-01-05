using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubleTakeProxyService.DimensionData.Models
{
    class Geos
    {
        public List<Geo> Geos { get; set; }
    }
    class Geo
    {
        public String id { get; set; }
        public String geoKey { get; set; }
        public String name { get; set; }
        public String cloudUiUrl { get; set; }
        public String cloudApiUrl { get; set; }
        public String isHome { get; set; }
        public String pricingUrl { get; set; }
        public String state { get; set; }
    }
}
