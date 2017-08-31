namespace MRMPService.Exceptions
{
	/// <summary>
	/// The <see cref="MRPDeploymentPolicyTypeException"/> class
	/// </summary>
	public sealed class MRPDeploymentPolicyTypeException : BaseException
	{
		/// <summary>
		/// Initilaizes the new instance of <see cref="MRPDeploymentPolicyTypeException"/>
		/// </summary>
		/// <param name="message">
		/// The error message
		/// </param>
		public MRPDeploymentPolicyTypeException(string message)
			: base(message)
		{
		}
	}
}
