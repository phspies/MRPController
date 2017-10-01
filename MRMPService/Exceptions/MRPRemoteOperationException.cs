namespace MRMPService.Exceptions
{
	/// <summary>
	/// The <see cref="MRPDeploymentException"/> class
	/// </summary>
	public sealed class MRPRemoteOperationException : BaseException
	{
        /// <summary>
        /// Initilaizes the new instance of <see cref="MRPRemoteOperationException"/>
        /// </summary>
        /// <param name="message">
        /// The error message
        /// </param>
        public MRPRemoteOperationException(string message) : base(message)
		{
		}
	}
}
