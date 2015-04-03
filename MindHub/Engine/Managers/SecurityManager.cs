using System;
using System.Collections;

namespace Engine
{
	/// <summary>
	/// Security manager. Creates, initializes and distributes security guards
	/// </summary>
	public class SecurityManager : Manager
	{
		/// <summary>
		/// Singelton manager reference
		/// </summary>
		static protected SecurityManager g_manager;

		/// <summary>
		/// Hash of guard sets (one set per environment)
		/// </summary>
		protected Hashtable m_enviromentGuards;

		/// <summary>
		/// Static constructor. Creates the singelton instance.
		/// </summary>
		static SecurityManager()
		{
			g_manager = new SecurityManager();
		}


		/// <summary>
		/// Default constructor. 
		/// </summary>
		protected SecurityManager()
		{			
			m_enviromentGuards = new Hashtable();
		}


		/// <summary>
		/// Returns referece to the singelton manager
		/// </summary>
		/// <returns>Reference to sigelton manager</returns>
		public static SecurityManager GetManager()
		{			
			return g_manager;
		}


		/// <summary>
		/// Initializes the security guards and all control lists
		/// </summary>
		public override void Initialize()
		{			
			EnvironmentManager envMgr = EnvironmentManager.GetManager();

			if (envMgr.GetLocation() != Location.WEB_SERVER)
				throw new Exception("SecurityManager can only be utilized on web server side");
			
			base.Initialize();
			LogManager.Log(this, "Initialized");
		}


		/// <summary>
		/// Registers existance of security guard with the manager. Instaces of the guard class will be initialized within each environment according to ISecurityGuard.GetEnvironments()
		/// </summary>
		/// <param name="classID">Class ID of the guard </param>
		/// <exception cref="ENotInitialized">If manager has not bee initialized</exception>
		/// <exception cref="EInvalidParameter">If classID is null</exception>
		public void RegisterSecurityGuard(string classID)
		{
			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (classID == null)
				throw new EInvalidParameter(this, "classID");

			// Create new instance of the security guard:
			ISecurityGuard guard;
			try
			{
				guard = (ISecurityGuard) ClassFactory.GetFactory().Produce(classID);
			}
			catch (Exception exception)
			{
				throw new Exception("Class referenced by ID [" + classID + "] is not a security guard", exception);
			}	

			// Find out for what environments this guard is meant to operate on:
			IDictionaryEnumerator itrEnvironments = guard.GetEnvironments();
			if (itrEnvironments == null)
			{
				// Initialize in all environments
				EnvironmentManager envMgr = EnvironmentManager.GetManager();
				itrEnvironments = envMgr.GetEnvironments();
			}
			
			// Initialize the guard in each environment it is meant to work in:
			while (itrEnvironments.MoveNext())
			{
				string environmentID = (string) itrEnvironments.Key;
				using (Transaction transaction = new Transaction(environmentID))
				{
					// Find guard set for referenced environment:
					Hashtable guards = (Hashtable) m_enviromentGuards[environmentID];
					if (guards == null)
					{
						guards = new Hashtable();
						m_enviromentGuards[environmentID] = guards;			
					}

					// Create new instance of the security guard and initialize it:
					guard = (ISecurityGuard) ClassFactory.GetFactory().Produce(classID);
					guard.Initialize(transaction);

					guards[classID] = guard;
				}
			}
		}


		/// <summary>
		/// Returns reference to security manager of given class within 
		/// the environment on which given transaction operates
		/// </summary>
		/// <param name="classID">Class ID of the guard</param>
		/// <param name="transaction">Transaction</param>
		/// <returns>Security guard object reference</returns>
		/// <exception cref="EInvalidParameter">If either of the parameters are null or the guard is not found</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public ILevelSecurityGuard GetLevelSecurityGuard(string classID, Transaction transaction)
		{
			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (classID == null)
				throw new EInvalidParameter(this, "classID");

			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			// Find guard set for referenced environment:
			Hashtable guards = (Hashtable) m_enviromentGuards[transaction.EnvironmentID];
			if (guards == null)
			{
				guards = new Hashtable();
				m_enviromentGuards[transaction.EnvironmentID] = guards;			
			}

			// Find already-initilized guard:
			ILevelSecurityGuard levelGuard;
			try
			{
				levelGuard = (ILevelSecurityGuard) guards[classID];
			}
			catch (Exception exception)
			{
				throw new Exception("Class referenced by ID [" + classID + "] is not a level security guard", exception);
			}
			if (levelGuard == null)
				throw new EInvalidParameter(this, "classID", classID, "Security guard not registered with SecurityManager");				

			return levelGuard;
		}


		/// <summary>
		/// Returns reference to security manager of given class within the environment on which given transaction operates.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <param name="classID">Class ID of the guard</param>
		/// <param name="transaction">Transaction</param>
		/// <returns>Security guard object reference</returns>
		/// <exception cref="EInvalidParameter">If either of the parameters are null or the guard is not found</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public IObjectActionSecurityGuard GetObjectActionSecurityGuard(string classID, Transaction transaction)
		{
			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (classID == null)
				throw new EInvalidParameter(this, "classID");

			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			// Find guard set for referenced environment:
			Hashtable guards = (Hashtable) m_enviromentGuards[transaction.EnvironmentID];
			if (guards == null)
			{
				guards = new Hashtable();
				m_enviromentGuards[transaction.EnvironmentID] = guards;			
			}

			// Try creatig the security guard and casting it into object-action-based security guard:
			IObjectActionSecurityGuard objecActionGuard;
			try
			{
				objecActionGuard = (IObjectActionSecurityGuard) guards[classID];
			}
			catch (Exception exception)
			{
				throw new Exception("Class referenced by ID [" + classID + "] is not a object-aciton security guard", exception);
			}
			if (objecActionGuard == null)
				throw new EInvalidParameter(this, "classID", classID, "Security guard not registered with SecurityManager");

			return objecActionGuard;
		}


		/// <summary>
		/// Caches security settings for given user session. 
		/// </summary>
		/// <param name="session">Sesion within which caching should take place</param>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		/// <exception cref="ENotInitialized">If the manager is not initialized.</exception>
		public void CacheSecurity(Engine.Session session, Transaction transaction)
		{
			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (session == null)
				throw new EInvalidParameter(this, "session");

			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			// Find guard set for referenced environment:
			Hashtable guards = (Hashtable) m_enviromentGuards[transaction.EnvironmentID];
			if (guards == null)
				return;

			IDictionaryEnumerator itrGuards = guards.GetEnumerator();
			while (itrGuards.MoveNext())
			{				
				ISecurityGuard guard = (ISecurityGuard) itrGuards.Value;
				guard.CacheSecurity(session, transaction);
			}
		}
	}
}
