using System;
using System.Collections;

namespace Engine
{
	/// <summary>
	/// Base interface for all security managers
	/// </summary>
	public interface ISecurityGuard : ICreatable
	{
		/// <summary>
		/// Initialization function.
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		void Initialize(Transaction transaction);


		/// <summary>
		/// Returns list of environment IDs for which the security guard is applicable.
		/// </summary>
		/// <return>List of environment IDs or null if the security guard applies to all environments</return>
		IDictionaryEnumerator GetEnvironments();


		/// <summary>
		/// Caches security into user session.
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="transaction">Transaction</param>
		/// <remarks>Session object with cached security</remarks>
		/// <exception cref="EInvalidParameter">If either of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the guard has not been initialized.</exception>
		void CacheSecurity(Engine.Session session, Transaction transaction);		
	}
}
