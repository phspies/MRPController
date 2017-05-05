//using Amazon;
//using Amazon.EC2;
//using Amazon.EC2.Model;
//using Amazon.Runtime;
//using Amazon.Util;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPService.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MRMPService.MRPService.Scheduler.PlatformInventory.AWS
{
    class UpdateAWSPlatform
    {
        //public static async Task Do(MRPPlatformType _platform, bool full = true)
        //{
        //    Logger.log(String.Format("Started inventory process for {0} : {1}", _platform.platformtype, _platform.platformdatacenter.moid), Logger.Severity.Info);
        //    Stopwatch sw = Stopwatch.StartNew();

        //    //define object lists
        //    MRPCredentialType _credential = _platform.credential;
        //    _platform = MRMPServiceBase._mrmp_api.platform().get_by_id(_platform.id);
        //    MRPPlatformType _update_platform = new MRPPlatformType() { id = _platform.id };

        //    List<MRPWorkloadType> _mrp_workloads = new List<MRPWorkloadType>();

        //    MRPWorkloadListType _paged_workload = MRMPServiceBase._mrmp_api.workload().list_paged_filtered_brief(new MRPWorkloadFilterPagedType() { platform_id = _platform.id });
        //    while (_paged_workload.pagination.page_size > 0)
        //    {
        //        _mrp_workloads.AddRange(_paged_workload.workloads);
        //        if (_paged_workload.pagination.next_page > 0)
        //        {
        //            _paged_workload = MRMPServiceBase._mrmp_api.workload().list_paged_filtered_brief(new MRPWorkloadFilterPagedType() { platform_id = _platform.id, page = _paged_workload.pagination.next_page });
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //    _update_platform.platformdatacenters = _platform.platformdatacenters;
        //    _update_platform.platformdomains = _platform.platformdomains;

        //    BasicAWSCredentials _aws_credentials = new BasicAWSCredentials(_platform.credential.username, _platform.credential.encrypted_password);
        //    AmazonEC2Config _aws_config = new AmazonEC2Config();
        //    //todo: add aws and azure url to datastructures
        //    _aws_config.ServiceURL = _platform.url;

        //    AmazonEC2Client _ec2_client = new AmazonEC2Client(_aws_credentials, _aws_config);

        //    DescribeAvailabilityZonesResponse response = await _ec2_client.DescribeAvailabilityZonesAsync();
        //    foreach (AvailabilityZone _aws_zone in response.AvailabilityZones)
        //    {
        //        MRPPlatformdatacenterType _mrmp_dc = new MRPPlatformdatacenterType();
        //        if (_update_platform.platformdatacenters.Exists(x => x.moid == _aws_zone.ZoneName))
        //        {
        //            _mrmp_dc = _update_platform.platformdatacenters.FirstOrDefault(x => x.moid == _aws_zone.ZoneName);
        //        }
        //        else
        //        {
        //            _update_platform.platformdatacenters.Add(_mrmp_dc);
        //        }
        //        _mrmp_dc.moid = _aws_zone.ZoneName;
        //        _mrmp_dc.deleted = false;
        //    }

        //    var _ec2_vpcs = await _ec2_client.DescribeVpcsAsync();
        //    foreach (var _ec2_vpc in _ec2_vpcs.Vpcs)
        //    {
        //        MRPPlatformdomainType _mrmp_domain = new MRPPlatformdomainType();
        //        if (_update_platform.platformdomains.Exists(x => x.moid == _ec2_vpc.VpcId))
        //        {
        //            _mrmp_domain = _update_platform.platformdomains.FirstOrDefault(x => x.moid == _ec2_vpc.VpcId);
        //        }
        //        else
        //        {
        //            _update_platform.platformdomains.Add(_mrmp_domain);
        //        }
        //        _mrmp_domain.moid = _ec2_vpc.VpcId;
        //        if (_mrmp_domain.domain == null) { _mrmp_domain.domain = String.Format("vpc-{0}", _ec2_vpc.VpcId); }
        //        _mrmp_domain.deleted = false;

        //        var _vpc_filter = new List<Filter>();
        //        var _vpc_id = new List<String>();
        //        _vpc_id.Add(_ec2_vpc.VpcId);
        //        _vpc_filter.Add(new Filter() { Name = "vpc-id", Values = _vpc_id });


        //        var _ec2_securitygoups = await _ec2_client.DescribeSecurityGroupsAsync(new DescribeSecurityGroupsRequest() { Filters = _vpc_filter });
        //        foreach (var _securitygroup in _ec2_securitygoups.SecurityGroups)
        //        {
        //            MRPPlatformnetworkType _mrp_network = new MRPPlatformnetworkType();
        //            if (_mrmp_domain.platformnetworks.Any(y => y.moid == _securitygroup.GroupId))
        //            {
        //                _mrp_network = _mrmp_domain.platformnetworks.FirstOrDefault(y => y.moid == _securitygroup.GroupId);
        //            }
        //            else
        //            {
        //                _mrmp_domain.platformnetworks.Add(_mrp_network);
        //            }
        //            _mrp_network.moid = _securitygroup.GroupId;
        //            _mrp_network.network = _securitygroup.GroupName;
        //            _mrp_network.description = _securitygroup.Description;
        //            _mrp_network.provisioned = true;
        //            _mrp_network.deleted = false;
        //        }
        //    }
        //    List<string> _avalabilty_zone = new List<string>();
        //    _avalabilty_zone.Add(_platform.platformdatacenter.moid);
        //    var _ec2_instances = await _ec2_client.DescribeInstancesAsync();
        //    var _ec2_workloads = _ec2_instances.Reservations.SelectMany(x => x.Instances);
        //    foreach (var _ec2_workload in _ec2_workloads)
        //    {
        //        MRPWorkloadType _mrmp_workload = new MRPWorkloadType();
        //        if (_update_platform.workloads.Exists(x => x.moid == _ec2_workload.InstanceId))
        //        {
        //            _mrmp_workload = _update_platform.workloads.FirstOrDefault(x => x.moid == _ec2_workload.InstanceId);
        //        }
        //        else
        //        {
        //            _update_platform.workloads.Add(_mrmp_workload);
        //        }
        //        _mrmp_workload.moid = _ec2_workload.InstanceId;
        //        _mrmp_workload.hostname = _ec2_workload.PrivateDnsName;
        //        _mrmp_workload.deleted = false;
        //    }
        //}
    }
}
