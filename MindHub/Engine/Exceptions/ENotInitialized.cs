using System;

namespace Engine
{
	/// <summary>
	/// Implementation of a typical "not initialized" error.
	/// </summary>
	public class ENotInitialized : Exception
	{
		/// <summary>
		/// Creates self with information about the throwing object.
		/// </summary>
		/// <param name="thrower">Object that did not get initialized</param>
		public ENotInitialized(System.Object thrower) : base(thrower.ToString() + " was not initialized")
		{
		}
	}
}
