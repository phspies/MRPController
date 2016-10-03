using DD.CBU.Compute.Api.Client;
using DD.CBU.Compute.Api.Contracts.Datacenter;
using DD.CBU.Compute.Api.Contracts.Network20;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace mcptags
{
    class Program
    {
        static void Main(string[] args)
        {


            ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri("https://api-mea.dimensiondata.com"), new NetworkCredential("dd_drs_dev", "T*3fj7WdmY"));
            CaaS.Login().Wait();

            string _protectiongroup = "Application 1";

            //create MCP tags
            IEnumerable<TagKeyType> _mcp_tags = CaaS.Tagging.GetTagKeys().Result;
            String _tag_id = null;
            IEnumerable<Geo> _geos = CaaS.Account.GetListOfMultiGeographyRegions().Result;
            Geo _home_geo = _geos.FirstOrDefault(x => x.isHome.ToLower() == "true");
            Uri _home_api_url = new Uri("https://" + new Uri(_home_geo.cloudApiUrl).Host);
            
            using (ComputeApiClient _home_caas = ComputeApiClient.GetComputeApiClient(_home_api_url, new NetworkCredential("dd_drs_dev", "T*3fj7WdmY")))
            {
                _home_caas.Login().Wait();
                if (_mcp_tags != null)
                {
                    TagKeyType _protectiongroup_tag = _mcp_tags.FirstOrDefault(x => x.name == _protectiongroup);
                    if (_protectiongroup_tag == null)
                    {
                        createTagKeyType _create_tag = new createTagKeyType();
                        _create_tag.name = _protectiongroup;
                        _create_tag.description = String.Format("DRS Protection Group {0}", _protectiongroup);
                        _create_tag.displayOnReport = true;
                        ResponseType _tag_response = _home_caas.Tagging.CreateTagKey(_create_tag).Result;
                        _tag_id = _tag_response.info.First().value;
                    }
                    else
                    {
                        _tag_id = _protectiongroup_tag.id;
                    }
                }
                else
                {
                    createTagKeyType _create_tag = new createTagKeyType();
                    _create_tag.name = _protectiongroup;
                    _create_tag.description = String.Format("DRS Protection Group {0}", _protectiongroup);
                    _create_tag.displayOnReport = true;
                    ResponseType _tag_response = _home_caas.Tagging.CreateTagKey(_create_tag).Result;
                    _tag_id = _tag_response.info.First().value;
                }

                if (_tag_id != null)
                {
                    applyTags _apply_tag = new applyTags();
                    _apply_tag.assetId = "792e7f5a-4df1-48aa-99aa-ca150ad0f360";
                    _apply_tag.assetType = "SERVER";
                    _apply_tag.tagById = new ApplyTagByIdType[1];
                    _apply_tag.tagById[0] = new ApplyTagByIdType() { tagKeyId = _tag_id };
                    var _response = CaaS.Tagging.ApplyTags(_apply_tag).Result;
                }
            }
        }
    }
}
