﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComputeApiClientBackupExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   Extension methods for the backup section of the CaaS API.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DD.CBU.Compute.Api.Client.Backup
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using DD.CBU.Compute.Api.Client.Interfaces;
	using DD.CBU.Compute.Api.Contracts.Backup;
	using DD.CBU.Compute.Api.Contracts.General;

	/// <summary>
	/// Extension methods for the backup section of the CaaS API.
	/// </summary>
	public static class ComputeApiClientBackupExtensions
	{
		/// <summary>
		/// Enables the backup with a specific service plan.
		/// </summary>
		/// <param name="client">
		/// The <see cref="ComputeApiClient"/> object
		/// </param>
		/// <param name="serverId">
		/// The server id
		/// </param>
		/// <param name="plan">
		/// The enumerated service plan
		/// </param>
		/// <returns>
		/// The status of the request
		/// </returns>
		[Obsolete("Use IComputeApiClient.Backup methods")]
		public static async Task<Status> EnableBackup(this IComputeApiClient client, string serverId, ServicePlan plan)
		{
			return await client.Backup.EnableBackup(serverId, plan);
		}

		/// <summary>
		/// Disable the backup service from the server.
		///     <remarks>
		/// Note the server MUST not have any clients
		/// </remarks>
		/// </summary>
		/// <param name="client">
		/// The <see cref="ComputeApiClient"/> object
		/// </param>
		/// <param name="serverId">
		/// The server id
		/// </param>
		/// <returns>
		/// The status of the request
		/// </returns>
		[Obsolete("Use IComputeApiClient.Backup methods")]
		public static async Task<Status> DisableBackup(this IComputeApiClient client, string serverId)
		{
			return await client.Backup.DisableBackup(serverId);
		}

		/// <summary>
		/// Modify the backup service plan.
		/// </summary>
		/// <param name="client">
		/// The <see cref="ComputeApiClient"/> object
		/// </param>
		/// <param name="serverId">
		/// The server id
		/// </param>
		/// <param name="plan">
		/// The plan to change to
		/// </param>
		/// <returns>
		/// The status of the request
		/// </returns>
		[Obsolete("Use IComputeApiClient.Backup methods")]
		public static async Task<Status> ChangeBackupPlan(this IComputeApiClient client, string serverId, ServicePlan plan)
		{
			return await client.Backup.ChangeBackupPlan(serverId, plan);
		}

		/// <summary>
		/// List the client types for a specified server
		/// </summary>
		/// <param name="client">
		/// The <see cref="ComputeApiClient"/> object
		/// </param>
		/// <param name="serverId">
		/// The server id
		/// </param>
		/// <returns>
		/// The status of the request
		/// </returns>
		[Obsolete("Use IComputeApiClient.Backup methods")]
		public static async Task<IEnumerable<BackupClientType>> GetBackupClientTypes(
			this IComputeApiClient client,
			string serverId)
		{
			return await client.Backup.GetBackupClientTypes(serverId);
		}

		/// <summary>
		/// List the storage policies for a specified server
		/// </summary>
		/// <param name="client">
		/// The <see cref="ComputeApiClient"/> object
		/// </param>
		/// <param name="serverId">
		/// The server id
		/// </param>
		/// <returns>
		/// The status of the request
		/// </returns>
		[Obsolete("Use IComputeApiClient.Backup methods")]
		public static async Task<IEnumerable<BackupStoragePolicy>> GetBackupStoragePolicies(
			this IComputeApiClient client,
			string serverId)
		{
			return await client.Backup.GetBackupStoragePolicies(serverId);
		}

		/// <summary>
		/// List the schedule policies for a specified server
		/// </summary>
		/// <param name="client">
		/// The <see cref="ComputeApiClient"/> object
		/// </param>
		/// <param name="serverId">
		/// The server id
		/// </param>
		/// <returns>
		/// The status of the request
		/// </returns>
		[Obsolete("Use IComputeApiClient.Backup methods")]
		public static async Task<IEnumerable<BackupSchedulePolicy>> GetBackupSchedulePolicies(
			this IComputeApiClient client,
			string serverId)
		{
			return await client.Backup.GetBackupSchedulePolicies(serverId);
		}

		/// <summary>
		/// Gets a list of backup clients.
		/// </summary>
		/// <param name="client">
		/// The <see cref="ComputeApiClient"/> object
		/// </param>
		/// <param name="serverId">
		/// The server id
		/// </param>
		/// <returns>
		/// A list of backup clients
		/// </returns>
		[Obsolete("Use IComputeApiClient.Backup methods")]
		public static async Task<IEnumerable<BackupClientDetailsType>> GetBackupClients(
			this IComputeApiClient client,
			string serverId)
		{
			return await client.Backup.GetBackupClients(serverId);
		}

		/// <summary>
		/// Adds a backup client to a specified server.
		/// </summary>
		/// <param name="client">
		/// The <see cref="ComputeApiClient"/> object
		/// </param>
		/// <param name="serverId">
		/// The server id
		/// </param>
		/// <param name="clientType">
		/// The backup client type to add
		/// </param>
		/// <param name="storagePolicy">
		/// The backup storage policy
		/// </param>
		/// <param name="schedulePolicy">
		/// The backup schedule policy
		/// </param>
		/// <param name="alertingType">
		/// The alerting type
		/// </param>
		/// <returns>
		/// The status of the request
		/// </returns>
		[Obsolete("Use IComputeApiClient.Backup methods")]
		public static async Task<Status> AddBackupClient(
			this IComputeApiClient client,
			string serverId,
			BackupClientType clientType,
			BackupStoragePolicy storagePolicy,
			BackupSchedulePolicy schedulePolicy,
			AlertingType alertingType)
		{
			return await client.Backup.AddBackupClient(serverId, clientType, storagePolicy, schedulePolicy, alertingType);
		}

		/// <summary>
		/// Removes the backup client from a specified server.
		/// </summary>
		/// <param name="client">
		/// The <see cref="ComputeApiClient"/> object
		/// </param>
		/// <param name="serverId">
		/// The server id
		/// </param>
		/// <param name="backupClient">
		/// The backup client to remove
		/// </param>
		/// <returns>
		/// The status of the request
		/// </returns>
		[Obsolete("Use IComputeApiClient.Backup methods")]
		public static async Task<Status> RemoveBackupClient(
			this IComputeApiClient client,
			string serverId,
			BackupClientDetailsType backupClient)
		{
			return await client.Backup.RemoveBackupClient(serverId, backupClient);
		}

		/// <summary>
		/// Modifies the backup client on the specified server.
		/// </summary>
		/// <param name="client">
		/// The <see cref="ComputeApiClient"/> object
		/// </param>
		/// <param name="serverId">
		/// The server id
		/// </param>
		/// <param name="backupClient">
		/// The backup client to modify
		/// </param>
		/// <param name="storagePolicy">
		/// The storage policy to modify
		/// </param>
		/// <param name="schedulePolicy">
		/// The schedule policy to modify
		/// </param>
		/// <param name="alertingType">
		/// The alerting type to modify
		/// </param>
		/// <returns>
		/// The status of the request
		/// </returns>
		[Obsolete("Use IComputeApiClient.Backup methods")]
		public static async Task<Status> ModifyBackupClient(
			this IComputeApiClient client,
			string serverId,
			BackupClientDetailsType backupClient,
			BackupStoragePolicy storagePolicy,
			BackupSchedulePolicy schedulePolicy,
			AlertingType alertingType)
		{
			return await client.Backup.ModifyBackupClient(serverId, backupClient, storagePolicy, schedulePolicy, alertingType);
		}

		/// <summary>
		/// Requests an immediate Backup for a Backup Client
		/// </summary>
		/// <param name="client">
		/// The <see cref="ComputeApiClient"/> object
		/// </param>
		/// <param name="serverId">
		/// The server id
		/// </param>
		/// <param name="backupClient">
		/// The backup client to modify
		/// </param>
		/// <returns>
		/// The status of the request
		/// </returns>
		[Obsolete("Use IComputeApiClient.Backup methods")]
		public static async Task<Status> InitiateBackup(
			this IComputeApiClient client,
			string serverId,
			BackupClientDetailsType backupClient)
		{
			return await client.Backup.InitiateBackup(serverId, backupClient);
		}

		/// <summary>
		/// Requests a cancellation for any running job for a backup client
		/// </summary>
		/// <param name="client">
		/// The <see cref="ComputeApiClient"/> object
		/// </param>
		/// <param name="serverId">
		/// The server id
		/// </param>
		/// <param name="backupClient">
		/// The backup client to modify
		/// </param>
		/// <returns>
		/// The status of the request
		/// </returns>
		[Obsolete("Use IComputeApiClient.Backup methods")]
		public static async Task<Status> CancelBackupJob(
			this IComputeApiClient client,
			string serverId,
			BackupClientDetailsType backupClient)
		{
			return await client.Backup.CancelBackupJob(serverId, backupClient);
		}
	}
}