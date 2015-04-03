using System;

namespace Engine
{
	/// <summary>
	/// Interface for a generic security guard responsible for authorizations based purly on
	/// user's allowed data object actions (componded taken into account group memberships)
	/// </summary>
	public interface IObjectActionSecurityGuard : ISecurityGuard
	{
		/// <summary>
		/// Authozes users's action on object
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="action">Action</param>
		/// <param name="dataObject">Data object</param>
		/// <exception cref="EInvalidParameter">If either of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the guard has not been initialized.</exception>
		/// <exception cref="EUnauthorized">If the guard rejects authorization</exception>
		void Authorize(Engine.Session session, string action, IDataObject dataObject);
	}
}
