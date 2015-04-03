using System;

namespace Engine
{
	/// <summary>
	/// Implementation of a typical "not authorized" error.
	/// </summary>
	public class EUnauthorized : Exception
	{
		/// <summary>
		/// Creates self with information about the throwing object.
		/// </summary>
		/// <param name="thrower">Object that rejected attempt to perform given action</param>
		/// <param name="attemptedAction">Action as described by the thrower</param>
		public EUnauthorized(System.Object thrower, string reason) : base(thrower.ToString() + " could not authorize: " + reason)
		{
		}
	}
}
