using DD.CBU.Compute.Api.Client;
using MRMPService.MRMPAPI.Contracts;
using System;
using System.Net;
using MRMPService.Utilities;
using System.Threading.Tasks;
using System.Linq;
using DD.CBU.Compute.Api.Contracts.Requests.Tagging;
using DD.CBU.Compute.Api.Contracts.Network20;
using System.Collections.Generic;
using DD.CBU.Compute.Api.Contracts.Requests.Server20;
using MRMPService.MRMPAPI;

namespace MRMPService.Tasks.MCP
{
    partial class MCP_Platform
    {
        public static async Task ApplyMetaInformation(String _task_id, List<MRPWorkloadPairType> _workload_pairs, MRPPlatformType _source_platform, MRPPlatformType _target_platform, MRPWorkloadType _source_workload, MRPWorkloadType _target_workload, float _start_progress, float _end_progress)
        {

            ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_target_workload.platform.url), new NetworkCredential(_target_workload.platform.credential.username, _target_workload.platform.credential.encrypted_password));
            var _account = await CaaS.Login();

            if ((bool)_target_workload.sync_tag_rules)
            {
                await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Applying workload tags to {0} from {1}", _target_workload.hostname, _source_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 5));
                //first map MCP to MCP tags
                IEnumerable<TagType> _caas_tags = await CaaS.Tagging.GetTags();
                IEnumerable<TagType> _source_tags = await CaaS.Tagging.GetTags(new TagListOptions() { AssetId = _source_workload.moid });
                IEnumerable<TagType> _target_tags = await CaaS.Tagging.GetTags(new TagListOptions() { AssetId = _target_workload.moid });
                if (_source_tags != null)
                {
                    List<ApplyTagByIdType> _add_platform_tags = new List<ApplyTagByIdType>();
                    foreach (var _source_tag in _source_tags)
                    {
                        //if the tag does not exist in target workload
                        if (!_target_tags.ToList().Exists(x => x.assetId == _source_tag.assetId))
                        {
                            _add_platform_tags.Add(new ApplyTagByIdType() { tagKeyId = _source_tag.tagKeyId, value = _source_tag.value, valueSpecified = true });
                        }
                    }
                    if (_add_platform_tags.Count > 0)
                    {

                        ResponseType _tag_platform_task = new ResponseType();
                        try
                        {
                            _tag_platform_task = await CaaS.Tagging.ApplyTags(new applyTags() { assetId = _target_workload.moid, assetType = "SERVER", tagById = _add_platform_tags.ToArray() });
                        }
                        catch (Exception)
                        {
                            throw new Exception(String.Format("Error applying user tags for {0} : {1}", _target_workload.hostname, _tag_platform_task.message));
                        }
                        if (_tag_platform_task.responseCode == "OK")
                        {
                            await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Successfully applied {0} platform tags to {1}", _add_platform_tags.Count(), _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 10));
                        }
                        else
                        {
                            throw new Exception(String.Format("Error applying user tags for {0} : {1}", _target_workload.hostname, _tag_platform_task.message));
                        }
                    }
                }
                //sync user defined tags
                List<MRPOrganizationTagType> _org_tags = (await MRMPServiceBase._mrmp_api.organization().get()).organizationtags;
                _target_tags = await CaaS.Tagging.GetTags(new TagListOptions() { AssetId = _target_workload.moid });
                foreach (var _org_tag in _org_tags)
                {
                    //tag does not exist in organization
                    string _tag_moid = "";
                    bool _create_tag = false;
                    if (String.IsNullOrEmpty(_org_tag.tagkeyid))
                    {
                        _create_tag = true;
                    }
                    else
                    {
                        try
                        {
                            var _get_result = CaaS.Tagging.GetTagKey(new Guid(_org_tag.tagkeyid)).Result;
                        }
                        catch (Exception)
                        {
                            _create_tag = true;
                        }
                    }
                    if (_create_tag)
                    {
                        var _create_org_tag = await CaaS.Tagging.CreateTagKey(new createTagKeyType() { name = _org_tag.tagkeyname, displayOnReport = (bool)_org_tag.tagdisplayreport, valueRequired = (bool)_org_tag.tagvaluerequired });
                        if (_create_org_tag.responseCode == "OK")
                        {
                            _tag_moid = _create_org_tag.info[0].value;
                            await MRMPServiceBase._mrmp_api.organization().update(new MRPOrganizationCRUDType()
                            {
                                organization = new MRPOrganizationType()
                                {
                                    id = MRMPServiceBase.organization_id,
                                    organizationtags = new List<MRPOrganizationTagType>() {
                                            new MRPOrganizationTagType() {
                                                id = _org_tag.id,
                                                tagkeyid = _tag_moid } }
                                }
                            });
                        }
                        else
                        {
                            throw new Exception(String.Format("Failed to create used defined tag : {0}", _create_org_tag.message));
                        }
                    }
                }
                //refresh org tags with moid's in
                _org_tags = (await MRMPServiceBase._mrmp_api.organization().get()).organizationtags;

                var _add_user_tags = new List<ApplyTagByIdType>();
                foreach (MRPWorkloadTagType _target_tag in _target_workload.workloadtags)
                {
                    var _org_tag = _org_tags.FirstOrDefault(x => x.id == _target_tag.organizationtag_id);
                    if (_target_tags == null)
                    {
                        _add_user_tags.Add(new ApplyTagByIdType() { tagKeyId = _org_tag.tagkeyid, value = _target_tag.tagvalue, valueSpecified = true });
                    }
                    else if (!_target_tags.ToList().Exists(x => x.tagKeyId == _org_tag.tagkeyid))
                    {
                        _add_user_tags.Add(new ApplyTagByIdType() { tagKeyId = _org_tag.tagkeyid, value = _target_tag.tagvalue, valueSpecified = true });
                    }
                }
                if (_add_user_tags.Count > 0)
                {
                    ResponseType _tag_task = new ResponseType();
                    try
                    {
                        _tag_task = await CaaS.Tagging.ApplyTags(new applyTags() { assetId = _target_workload.moid, assetType = "SERVER", tagById = _add_user_tags.ToArray() });
                    }
                    catch (Exception)
                    {
                        throw new Exception(String.Format("Error applying user tags for {0} : {1}", _target_workload.hostname, _tag_task.message));
                    }
                    if (_tag_task.responseCode == "OK")
                    {
                        await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Successfully applied {0} user tags to {1}", _add_user_tags.Count(), _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 15));
                    }
                    else
                    {
                        throw new Exception(String.Format("Error applying user tags for {0} : {1}", _target_workload.hostname, _tag_task.message));
                    }
                }
            }
            await MRMPServiceBase._mrmp_api.task().progress(_task_id, "Successfully configured workload meta data", ReportProgress.Progress(_start_progress, _end_progress, 29));


            await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Applying workload affinity rules to {0}", _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 30));

            if ((bool)_target_workload.sync_affinity_rules)
            {
                //user defines affinity rules
                var _target_platform_domain = _target_platform.platformdomains.FirstOrDefault(x => x.id == _target_workload.workloadinterfaces[0].platformnetwork.platformdomain_id);
                var _target_affinity_rules = _target_platform_domain.domainaffinityrules;
                var _source_platform_domain = _source_platform.platformdomains.FirstOrDefault(x => x.id == _source_workload.workloadinterfaces[0].platformnetwork.platformdomain_id);
                var _source_affinity_rules = _source_platform_domain.domainaffinityrules;
                var _source_affinity_pair = _source_affinity_rules.FirstOrDefault(x => x.workload1_id == _source_workload.id || x.workload2_id == _source_workload.id);
                var _target_caas_affinity_rules = await CaaS.ServerManagement.AntiAffinityRule.GetAntiAffinityRulesForNetworkDomain(new Guid(_target_platform_domain.moid));

                foreach (var _target_rule in _target_affinity_rules.Where(x => x.affinitytype == "user"))
                {
                    bool _create_user_rule = false;
                    if (String.IsNullOrEmpty(_target_rule.moid))
                    {
                        _create_user_rule = true;
                    }
                    else
                    {
                        try
                        {
                            var _caas_rule = CaaS.ServerManagement.AntiAffinityRule.GetAntiAffinityRulesForServer(new Guid(_target_rule.moid));
                        }
                        catch (Exception)
                        {
                            _create_user_rule = true;
                        }
                    }
                    if (_create_user_rule)
                    {
                        try
                        {
                            var server1_moid = _target_platform.workloads.FirstOrDefault(x => x.id == _target_rule.workload1_id).moid;
                            var server2_moid = _target_platform.workloads.FirstOrDefault(x => x.id == _target_rule.workload2_id).moid;
                            var _create_caas_task = await CaaS.ServerManagementLegacy.Server.CreateServerAntiAffinityRule(server1_moid, server2_moid);
                        }
                        catch (Exception ex)
                        {
                            await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Error applying affinity rule between {0} and {1} : {2}", _target_rule.workload1_id, _target_rule.workload2_id, ex.GetBaseException().Message), ReportProgress.Progress(_start_progress, _end_progress, _target_affinity_rules.IndexOf(_target_rule) + 31));
                        }
                    }
                }


                //First do source platform rules
                //we know now that this source server is in a affinity rule
                if (_source_affinity_pair != null)
                {
                    var _target_side_workload1 = _workload_pairs.FirstOrDefault(x => x.source_workload.id == _source_affinity_pair.workload1_id).target_workload;
                    var _target_side_workload2 = _workload_pairs.FirstOrDefault(x => x.source_workload.id == _source_affinity_pair.workload2_id).target_workload;

                    if (_target_side_workload1 != null && _target_side_workload2 != null)
                    {
                        bool _create_rule = false;
                        AntiAffinityRuleType _current_affinity_rule;
                        MRPDomainAffinityRuleType _current_mrmp_affinity_rule = new MRPDomainAffinityRuleType();
                        if (_target_caas_affinity_rules == null)
                        {
                            _create_rule = true;
                        }
                        else
                        {
                            if (!_target_caas_affinity_rules.ToList().Exists(x => x.serverSummary[0].id == _target_side_workload1.moid || x.serverSummary[1].id == _target_side_workload2.moid || x.serverSummary[0].id == _target_side_workload2.moid || x.serverSummary[1].id == _target_side_workload1.moid))
                            {
                                _create_rule = true;
                            }
                            else
                            {
                                _current_affinity_rule = _target_caas_affinity_rules.ToList().FirstOrDefault(x => x.serverSummary[0].id == _target_side_workload1.moid || x.serverSummary[1].id == _target_side_workload2.moid || x.serverSummary[0].id == _target_side_workload2.moid || x.serverSummary[1].id == _target_side_workload1.moid);
                                if (_target_affinity_rules.Exists(x => x.moid == _current_affinity_rule.id))
                                {
                                    _current_mrmp_affinity_rule = _target_affinity_rules.FirstOrDefault(x => x.moid == _current_affinity_rule.id);
                                }
                            }
                        }
                        if (_create_rule)
                        {
                            try
                            {
                                var _create_task = await CaaS.ServerManagementLegacy.Server.CreateServerAntiAffinityRule(_target_side_workload1.moid, _target_side_workload2.moid);
                                if (_create_task.result == "SUCCESS")
                                {
                                    MRPPlatformType _update_platform = new MRPPlatformType()
                                    {
                                        id = _target_platform_domain.platform_id,
                                        platformdomains = new List<MRPPlatformdomainType>() {
                                            new MRPPlatformdomainType() {
                                                id = _target_platform_domain.id, domainaffinityrules = new List<MRPDomainAffinityRuleType>() {
                                                    new MRPDomainAffinityRuleType() {
                                                        id = _current_mrmp_affinity_rule.id,
                                                        affinitytype = "platform",
                                                        moid = _create_task.additionalInformation[0].value,
                                                        workload1_id = _target_side_workload1.id,
                                                        workload2_id = _target_side_workload2.id }
                                                }
                                            }
                                        }
                                    };
                                    await MRMPServiceBase._mrmp_api.platform().update(_update_platform);
                                }
                                else
                                {
                                    throw new Exception(String.Format("Error creating server affinity rule: {0}", _create_task.result));
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(String.Format("Error creating server affinity rule: {0}", ex.GetBaseException().Message));
                            }

                        }

                    }

                }
            }
        }
    }
}

