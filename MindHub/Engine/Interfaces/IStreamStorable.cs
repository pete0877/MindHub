using System;
using System.IO;

namespace Engine
{
	/// <summary>
	/// Interface for object that can store itself in a database using datbase connection.
	/// </summary>
	public interface IStreamStorable	
	{
		/// <summary>
		/// Saves self to stream
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		void SaveToStream(MemoryStream stream);


		/// <summary>
		/// Loads self from stream
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the instance is not yet initialized</exception>
		void LoadFromStream(MemoryStream stream);
	}
}
