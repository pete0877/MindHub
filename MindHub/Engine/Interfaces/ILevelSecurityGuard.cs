using System;

namespace Engine
{
	/// <summary>
	/// Interface for a generic security guard responsible for authorizations based purly on
	/// user's level rights (componded taken into account group memberships)
	/// </summary>
	public interface ILevelSecurityGuard : ISecurityGuard
	{
		/// <summary>
		/// Authorized users's access to given level of security
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="level">Level being autorized</param>
		/// <exception cref="EInvalidParameter">If either of the parameters are null or if the action is not recognized or expected for this call</exception>
		/// <exception cref="ENotInitialized">If the guard has not been initialized.</exception>
		/// <exception cref="EUnauthorized">If the guard rejects authorization</exception>
		void Authorize(Engine.Session session, string level);
	}
}

