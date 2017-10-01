namespace MRMPService.Exceptions
{
	/// <summary>
	/// The <see cref="MRPDeploymentException"/> class
	/// </summary>
	public sealed class MRMPDatamoverException : BaseException
	{
		/// <summary>
		/// Initilaizes the new instance of <see cref="MRPDeploymentException"/>
		/// </summary>
		/// <param name="message">
		/// The error message
		/// </param>
		public MRMPDatamoverException(string message)
			: base(message)
		{
		}
	}
}
