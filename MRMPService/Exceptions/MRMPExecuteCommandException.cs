namespace MRMPService.Exceptions
{
	/// <summary>
	/// The <see cref="MRPDeploymentException"/> class
	/// </summary>
	public sealed class MRMPExecuteCommandException : BaseException
	{
        /// <summary>
        /// Initilaizes the new instance of <see cref="MRMPExecuteCommandException"/>
        /// </summary>
        /// <param name="message">
        /// The error message
        /// </param>
        public MRMPExecuteCommandException(string message)
			: base(message)
		{
		}
	}
}
