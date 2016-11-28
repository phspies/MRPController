﻿namespace DD.CBU.Compute.Api.Client.Drs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contracts.Drs;
    using Contracts.General;
    using Contracts.Network20;
    using Contracts.Requests;
    using Contracts.Requests.Drs;
    using Interfaces;
    using Interfaces.Drs;

    /// <summary>
    /// The Consistency Group Accessor type.
    /// </summary>
    public class ConsistencyGroupAccessor: IConsistencyGroupAccessor
    {
        /// <summary>
		/// The _client.
		/// </summary>
		private readonly IWebApi _apiClient;

        /// <summary>
        /// 	Initializes a new instance of the DD.CBU.Compute.Api.Client.Network20.NetworkDomain
        /// 	class.
        /// </summary>
        /// <param name="apiClient">	The client. </param>
        public ConsistencyGroupAccessor(IWebApi apiClient)
        {
            _apiClient = apiClient;
        }

        /// <summary>
        /// The Get Consistency Group method.
        /// </summary>
        /// <param name="filteringOptions">The filtering options.</param>
        /// <returns>List of <see cref="ConsistencyGroupType"/></returns>
        public async Task<IEnumerable<ConsistencyGroupType>> GetConsistencyGroups(ConsistencyGroupListOptions filteringOptions = null)
        {
            var response = await GetConsistencyGroupsPaginated(filteringOptions, null);
            return response.items;
        }

        /// <summary>
        /// The Get Consistency Group menthod.
        /// </summary>
        /// <param name="filteringOptions">The filtering options.</param>
        /// <param name="pagingOptions">The pagination options.</param>
        /// <returns>Paginated result of <see cref="ConsistencyGroupType"/></returns>
        public async Task<PagedResponse<ConsistencyGroupType>> GetConsistencyGroupsPaginated(ConsistencyGroupListOptions filteringOptions = null, PageableRequest pagingOptions = null)
        {
            var response = await _apiClient.GetAsync<consistencyGroups>(ApiUris.GetConsistencyGroups(_apiClient.OrganizationId), pagingOptions, filteringOptions);
            return new PagedResponse<ConsistencyGroupType>
            {
                items = response.consistencyGroup,
                totalCount = response.totalCountSpecified ? response.totalCount : (int?)null,
                pageCount = response.pageCountSpecified ? response.pageCount : (int?)null,
                pageNumber = response.pageNumberSpecified ? response.pageNumber : (int?)null,
                pageSize = response.pageSizeSpecified ? response.pageSize : (int?)null
            };
        }

        /// <summary>
        /// The Create Consistency Group
        /// </summary>
        /// <param name="createConsistencyGroup">The create consistency group type.</param>
        /// <returns>The <see cref="ResponseType"/></returns>
        public async Task<ResponseType> CreateConsistencyGroup(CreateConsistencyGroupType createConsistencyGroup)
        {
            return await _apiClient.PostAsync<CreateConsistencyGroupType, ResponseType>(ApiUris.CreateConsistencyGroups(_apiClient.OrganizationId), createConsistencyGroup);
        }

        /// <summary>
        /// The Get Consistency Group Snapshot method.
        /// </summary>
        /// <param name="filteringOptions">The filtering options.</param>
        /// <returns>List of <see cref="consistencyGroupSnapshots"/></returns>
        public async Task<consistencyGroupSnapshots> GetConsistencyGroupSnapshots(ConsistencyGroupSnapshotListOptions filteringOptions = null)
        {
            return await _apiClient.GetAsync<consistencyGroupSnapshots>(ApiUris.GetConsistencyGroupSnapshots(_apiClient.OrganizationId), null, filteringOptions);
        }

        /// <summary>
        /// The stop preview snapshot of a consistency group.
        /// </summary>
        /// <param name="stopPreviewSnapshotType">The stop preview snapshot type.</param>
        /// <returns>The <see cref="ResponseType"/></returns>
        public async Task<ResponseType> StopPreviewSnapshot(StopPreviewSnapshotType stopPreviewSnapshotType)
        {
            return await _apiClient.PostAsync<StopPreviewSnapshotType, ResponseType>(ApiUris.StopPreviewSnapshot(_apiClient.OrganizationId), stopPreviewSnapshotType);
        }

		/// <summary>
		/// Start preview snapshot of a consistency group.
		/// </summary>
		/// <param name="startPreviewSnapshotType">The start preview snapshot type.</param>
		/// <returns>The <see cref="ResponseType"/></returns>
		public async Task<ResponseType> StartPreviewSnapshot(StartPreviewSnapshotType startPreviewSnapshotType)
		{
			return await _apiClient.PostAsync<StartPreviewSnapshotType, ResponseType>(ApiUris.StartPreviewSnapshot(_apiClient.OrganizationId), startPreviewSnapshotType);
		}

        /// <summary>
        /// The Delete Consistency Group method.
        /// </summary>
        /// <param name="deleteConsistencyGroupType">The delete consistency group.</param>
        /// <returns><see cref="ResponseType"/></returns>
        public async Task<ResponseType> DeleteConsistencyGroup(DeleteConsistencyGroupType deleteConsistencyGroupType)
        {
            return await _apiClient.PostAsync<DeleteConsistencyGroupType, ResponseType>(ApiUris.DeleteConsistencyGroup(_apiClient.OrganizationId), deleteConsistencyGroupType);
        }

        /// <summary>
        /// The initiate failover for a consistency group.
        /// </summary>
        /// <param name="InitiateFailover">The Initiate failover type.</param>
        /// <returns>The <see cref="ResponseType"/></returns>
        public async Task<ResponseType> InitiateFailoverForConsistencyGroup(InitiateFailoverType InitiateFailover)
        {
            return await _apiClient.PostAsync<InitiateFailoverType, ResponseType>(ApiUris.InitiateFailover(_apiClient.OrganizationId), InitiateFailover);
        }

		/// <summary>
		/// Expand journal
		/// </summary>
		/// <param name="expandJournalType">Expand journal type.</param>
		/// <returns>The <see cref="ResponseType"/></returns>
		public async Task<ResponseType> ExpandJournal(ExpandJournalType expandJournalType)
		{
			return await _apiClient.PostAsync<ExpandJournalType, ResponseType>(ApiUris.ExpandJournal(_apiClient.OrganizationId), expandJournalType);
		}
	}
}