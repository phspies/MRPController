namespace MRMPService.Exceptions
{
	/// <summary>
	/// The <see cref="MRPDeploymentException"/> class
	/// </summary>
	public sealed class MRPDeploymentException : BaseException
	{
		/// <summary>
		/// Initilaizes the new instance of <see cref="MRPDeploymentException"/>
		/// </summary>
		/// <param name="message">
		/// The error message
		/// </param>
		public MRPDeploymentException(string message)
			: base(message)
		{
		}
	}
}
