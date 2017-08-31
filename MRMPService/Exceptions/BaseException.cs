using System;
using System.Runtime.Serialization;

namespace MRMPService.Exceptions
{
	[Serializable]
	public abstract class BaseException : Exception, ISerializable
	{
		/// <summary>
		/// Initialises a new instance of the <see cref="BaseException"/> class.
		/// </summary>
		protected BaseException()
		{
		}

		/// <summary>
		/// Initialises a new instance of the <see cref="BaseException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		public BaseException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initialises a new instance of the <see cref="BaseException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="innerException">
		/// The inner exception.
		/// </param>
		public BaseException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initialises a new instance of the <see cref="BaseException"/> class.
		/// </summary>
		/// <param name="info">
		/// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		/// <exception cref="SerializationException">The class name is null or <see cref="P:System.Exception.HResult" /> is zero (0). </exception>
		/// <exception cref="ArgumentNullException">The given parameter or argument is null</exception>
		public BaseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
