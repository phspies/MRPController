using System;
using MRMPService.MRMPService.Log;

namespace MRMPService.Exceptions
{
	/// <summary>
	/// The common exception factory class holds the all the exceptions to be logged and raised
	/// </summary>
	public static class ExceptionFactory
	{
		/// <summary>
		/// The error message
		/// </summary>
		private static string errorMessage;

		/// <summary>
		/// Throws <see cref="ArgumentNullException"/> if the given parameter is null or empty.
		/// </summary>
		/// <param name="paramater">
		/// The parameter
		/// </param>
		/// <exception cref="System.ArgumentNullException">Parameter Name</exception>
		public static void CheckArgumentIsNullOrEmpty(string paramater)
		{
			if (string.IsNullOrWhiteSpace(paramater))
			{
				errorMessage = $"The given argument or parameter cannot be null or empty, parameterName: {nameof(paramater)}";
				Logger.log(errorMessage, Logger.Severity.Error);
				throw new ArgumentNullException(errorMessage);
			}
		}

		/// <summary>
		/// Throws MRP deployment policy type queue scheme is not supported exception.
		/// </summary>
		/// <param name="queueScheme">
		///	The queue scheme.
		/// </param>
		/// <returns>
		/// The MRP deployment type queue scheme not supported exception <see cref="MRPDeploymentPolicyTypeException"/>
		/// </returns>
		public static MRPDeploymentException MRPDeploymentPolicyTypeQueueSchemeNotSupported(string queueScheme)
		{
			if (string.IsNullOrWhiteSpace(queueScheme))
				errorMessage = $"The MRP deployment queue scheme cannot be null or empty";
			else
				errorMessage = $"The given MRP deployment queue scheme is not supported, queue scheme: {queueScheme}";

			Logger.log(errorMessage, Logger.Severity.Error);
			return new MRPDeploymentException(errorMessage);
		}

		/// <summary>
		/// Throws MRP deployment server type is not supported exception.
		/// </summary>
		/// <param name="serverType">
		///	The server type.
		/// </param>
		/// <returns>
		/// The MRP deployment server type is not supported exception <see cref="MRPDeploymentException"/>
		/// </returns>
		public static MRPDeploymentException MRPDeploymentServerTypeNotSupported(string serverType)
		{
			if (string.IsNullOrWhiteSpace(serverType))
				errorMessage = $"The MRP deployment server type cannot be null or empty";
			else
				errorMessage = $"The given MRP deployment server type is not supported, server type: {serverType}";

			Logger.log(errorMessage, Logger.Severity.Error);
			return new MRPDeploymentException(errorMessage);
		}
	}
}
