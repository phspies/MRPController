namespace MRMPService.Exceptions
{
	/// <summary>
	/// The <see cref="MRPDeploymentServerTypeException"/> class
	/// </summary>
	public sealed class MRPDeploymentServerTypeException : BaseException
	{
		/// <summary>
		/// Initilaizes the new instance of <see cref="MRPDeploymentServerTypeException"/>
		/// </summary>
		/// <param name="message">
		/// The error message
		/// </param>
		public MRPDeploymentServerTypeException(string message)
			: base(message)
		{
		}
	}
}
