﻿namespace DD.CBU.Compute.Api.Client.Network20
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DD.CBU.Compute.Api.Client.Interfaces;
    using DD.CBU.Compute.Api.Client.Interfaces.Network20;
    using DD.CBU.Compute.Api.Contracts.General;
    using DD.CBU.Compute.Api.Contracts.Network20;
    using DD.CBU.Compute.Api.Contracts.Requests;
    using DD.CBU.Compute.Api.Contracts.Requests.Network20;

    /// <summary>
    /// The network domain.
    /// </summary>
    public class NetworkDomainAccessor : INetworkDomainAccessor
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
		public NetworkDomainAccessor(IWebApi apiClient)
		{
			_apiClient = apiClient;
		}

        /// <summary>
        /// The get network domains.
        /// </summary>
		/// <param name="filteringOptions">
		/// The filtering options.
		/// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IEnumerable<NetworkDomainType>> GetNetworkDomains(NetworkDomainListOptions filteringOptions = null)
        {
            var response = await GetNetworkDomainsPaginated(filteringOptions, null);
            return response.items;
        }

        /// <summary>
        /// The get network domains.
        /// </summary>
		/// <param name="filteringOptions">
		/// The filtering options.
		/// </param>
        /// <param name="pagingOptions">
        /// The paging options.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<PagedResponse<NetworkDomainType>> GetNetworkDomainsPaginated(NetworkDomainListOptions filteringOptions = null, PageableRequest pagingOptions = null)
        {
            var response = await _apiClient.GetAsync<networkDomains>(ApiUris.NetworkDomains(_apiClient.OrganizationId), pagingOptions, filteringOptions);
            return new PagedResponse<NetworkDomainType>
            {
                items = response.networkDomain,
                totalCount = response.totalCountSpecified ? response.totalCount : (int?)null,
                pageCount = response.pageCountSpecified ? response.pageCount : (int?)null,
                pageNumber = response.pageNumberSpecified ? response.pageNumber : (int?)null,
                pageSize = response.pageSizeSpecified ? response.pageSize : (int?)null
            };
        }

        /// <summary>
        /// 	This function gets a network domain from Cloud.
        /// </summary>
        /// <param name="networkDomainId">
        /// 	Network domain id. 
        /// </param>
        /// <returns>
        /// 	The network domain with the supplied id. 
        /// </returns>
        public async Task<NetworkDomainType> GetNetworkDomain(Guid networkDomainId)
        {
            return await _apiClient.GetAsync<NetworkDomainType>(
                ApiUris.NetworkDomain(_apiClient.OrganizationId, networkDomainId));
        }

        /// <summary>
        /// 	This function gets a network domain from Cloud.
        /// </summary>
        /// <param name="networkDomainName">
        /// 	The network domain name. 
        /// </param>
        /// <returns>
        /// 	The network domain with the supplid name.
        /// </returns>
        public async Task<NetworkDomainType> GetNetworkDomain(string networkDomainName)
        {
            var networkDomains = await GetNetworkDomains(new NetworkDomainListOptions { Name = networkDomainName });
            return (networkDomains != null)
                ? networkDomains.FirstOrDefault()
                : null;
        }

        /// <summary>
        /// This function deploys a new network domains to Cloud
        /// </summary>
        /// <param name="networkDomain">
        /// The network Domain.
        /// </param>
        /// <returns>
        /// Response containing status.
        /// </returns>
        public async Task<ResponseType> DeployNetworkDomain(DeployNetworkDomainType networkDomain)
		{
			return await _apiClient.PostAsync<DeployNetworkDomainType, ResponseType>(
                ApiUris.CreateNetworkDomain(_apiClient.OrganizationId),
                networkDomain);
		}

	    /// <summary>
	    /// The modify network domain.
	    /// </summary>
	    /// <param name="networkDomain">
	    /// The network domain.
	    /// </param>
	    /// <returns>
	    /// The <see cref="Task"/>.
	    /// </returns>
	    public async Task<ResponseType> ModifyNetworkDomain(EditNetworkDomainType networkDomain)
	    {
			return await _apiClient.PostAsync<EditNetworkDomainType, ResponseType>(
                ApiUris.ModifyNetworkDomain(_apiClient.OrganizationId),
                networkDomain);
	    }

        /// <summary>
        /// Delete the network domain. 
        /// </summary>
        /// <param name="id">
        /// The identifier of the network domain. 
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
		public async Task<ResponseType> DeleteNetworkDomain(Guid id)
		{
			return await _apiClient.PostAsync<DeleteNetworkDomainType, ResponseType>(
			    ApiUris.DeleteNetworkDomain(_apiClient.OrganizationId),
                new DeleteNetworkDomainType { id = id.ToString() });
		}
	}
}
