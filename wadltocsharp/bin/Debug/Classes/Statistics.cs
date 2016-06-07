using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Statistics : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public RestStringSet getAllStatisticsCategories_Method()
{
	endpoint = "/statistics/all_statistics_categories";
	mediatype="application/json";
	return get<RestStringSet>();
}


public TransactionID exportStatistics_Method(exportStatisticsParams exportStatisticsParams_object)
{
	endpoint = "/statistics/export";
	mediatype="application/json";
	return post<TransactionID>(exportStatisticsParams_object);
}


public void clearLongTermStatistics_Method()
{
	endpoint = "/statistics/clear_long_term_statistics";
	mediatype="*/*";
	put();
}


public TransactionID exportStatisticsToDefaultFile_Method(statisticsFilter statisticsFilter_object)
{
	endpoint = "/statistics/export_to_default_file";
	mediatype="application/json";
	return post<TransactionID>(statisticsFilter_object);
}


public LongTermStatisticsTimeFrames getAvailableLongTermStatisticsTimeFrames_Method()
{
	endpoint = "/statistics/long_term/time_frames";
	mediatype="application/json";
	return get<LongTermStatisticsTimeFrames>();
}


public TransactionID exportConsolidatedStatisticsByTimeFrame_Method(longTermStatisticsTimeFrame longTermStatisticsTimeFrame_object)
{
	endpoint = "/statistics/export_consolidate_by_time_frame";
	mediatype="application/json";
	return post<TransactionID>(longTermStatisticsTimeFrame_object);
}


public TransactionID exportConsolidatedStatisticsByTimeFrames_Method(longTermStatisticsTimeFrames longTermStatisticsTimeFrames_object)
{
	endpoint = "/statistics/export_consolidate_by_time_frames";
	mediatype="application/json";
	return post<TransactionID>(longTermStatisticsTimeFrames_object);
}




}
}
