namespace MRMPService.Exceptions
{
	/// <summary>
	/// The <see cref="MRPDeploymentException"/> class
	/// </summary>
	public sealed class MRMPIOException : BaseException
	{
        /// <summary>
        /// Initilaizes the new instance of <see cref="MRMPIOException"/>
        /// </summary>
        /// <param name="message">
        /// The error message
        /// </param>
        public MRMPIOException(string message)
			: base(message)
		{
		}
	}
}
