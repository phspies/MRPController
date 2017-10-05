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
        /// Throws MRP deployment policy type queue scheme is not supported exception.
        /// </summary>
        /// <param name="queueScheme">
        ///	The queue scheme.
        /// </param>
        /// <returns>
        /// The MRP deployment type queue scheme not supported exception <see cref="MRPDeploymentPolicyTypeException"/>
        /// </returns>
        public static MRMPDatamoverException JobOptionsVerify(string[] _verify_errors = null)
        {
            if (_verify_errors == null)
                errorMessage = $"Job verification process failed";
            else
                errorMessage = $"Job verification process failed with the following erros {String.Join(", ",_verify_errors)}";

            Logger.log(errorMessage, Logger.Severity.Error);
            return new MRMPDatamoverException(errorMessage);
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
        /// <summary>
        /// Throws MRP deployment server type is not supported exception.
        /// </summary>
        /// <param name="serverType">
        ///	The server type.
        /// </param>
        /// <returns>
        /// The MRP deployment server type is not supported exception <see cref="MRPDeploymentException"/>
        /// </returns>
        public static MRMPIOException MRMPIOFileNotFound(string _file)
        {
            errorMessage = string.IsNullOrWhiteSpace(_file) ? $"Local file not found" : $"Local file not found: {_file}";
            Logger.log(errorMessage, Logger.Severity.Error);
            return new MRMPIOException(errorMessage);
        }
        public static MRMPIOException MRMPIOException(Exception _ex, string _file)
        {
            errorMessage = string.IsNullOrWhiteSpace(_file) ? $"Local file not found : {_ex.GetBaseException().Message}" : $"Local file not found : {_file} : {_ex.GetBaseException().Message}";
            Logger.log($"{errorMessage} : {_ex.ToString()}", Logger.Severity.Error);
            return new MRMPIOException(errorMessage);
        }
        public static MRPRemoteOperationException MRMPExecuteCommandException(Exception _ex, string _host, string _command, string _message = null)
        {
            errorMessage = string.IsNullOrWhiteSpace(_message) ? $"Remote command execution threw an exception on {_host} : {_host} : {_ex.GetBaseException().Message} " : $"Remote command execution threw an exception on {_host} : {_host} : {_message} : {_ex.GetBaseException().Message}";
            Logger.log($"{errorMessage} : {_ex.ToString()}", Logger.Severity.Error);
            return new MRPRemoteOperationException(errorMessage);
        }
        public static MRPRemoteOperationException MRMPExecuteCommandException(string _host, string _command, string _message = null)
        {
            errorMessage = string.IsNullOrWhiteSpace(_message) ? $"Remote command execution threw an exception on {_host} : {_command}" : $"Remote command execution threw an exception on {_host} : {_command} : {_message}";
            Logger.log($"{errorMessage}", Logger.Severity.Error);
            return new MRPRemoteOperationException(errorMessage);
        }
        public static MRPRemoteOperationException MRMPRemoteRegistryException(Exception _ex, string _host, string _reg_location, string _message = null)
        {
            errorMessage = string.IsNullOrWhiteSpace(_message) ? $"Reading remote registry threw an exception on {_host} : {_reg_location} : {_ex.GetBaseException().Message}" : $"Reading remote registry threw an exception on {_host} : {_reg_location} : {_message} : {_ex.GetBaseException().Message}";
            Logger.log($"{errorMessage}", Logger.Severity.Error);
            return new MRPRemoteOperationException(errorMessage);
        }
    }
}
