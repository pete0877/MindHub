using System;
using System.Collections;
using System.Threading;

namespace Engine
{
	/// <summary>
	/// Singelton manager base class.
	/// </summary>
	public class Manager : MarshalByRefObject
	{
		/// <summary>
		/// Hash for all manager singeltons (by class ID).
		/// </summary>
		protected static Hashtable m_registryHash;

		/// <summary>
		/// Provides lock on the hash.
		/// </summary>
		protected static ReaderWriterLock m_registryLock;

		/// <summary>
		/// Flag for if the manager has been initialized.
		/// </summary>
		protected bool m_initialized;	

		/// <summary>
		/// Default constructor. 
		/// </summary>
		static Manager()
		{			
			m_registryHash = new Hashtable();
			m_registryLock = new ReaderWriterLock();			
		}

		protected Manager()
		{			
			RegisterManager(GetType().ToString(), this);
		}		

		/// <summary>
		/// Adds manager to registry.
		/// </summary>
		/// <param name="classID">Class ID (e.g. "Server.DataManager") </param>
		/// <param name="manager">Singelton manager</param>
		/// <exception cref="EInvalidParameter">If either of the parameters are null</exception>
		public static void RegisterManager(string classID, Manager manager)		
		{
			if (classID == null)
				throw new EInvalidParameter("Manager", "classID");

			if (manager == null)
				throw new EInvalidParameter("Manager", "manager");

			// Aquire writer lock and set the entry:
			m_registryLock.AcquireWriterLock(Int32.MaxValue);
			m_registryHash[classID] = manager;
			m_registryLock.ReleaseLock();
		}


		/// <summary>
		/// Returns pointer to the manager based on class ID.
		/// </summary>
		/// <param name="classID">Class ID</param>
		/// <returns>Pointer to the instance of given class ID Manager or null if class not registered.</returns>
		/// <exception cref="EInvalidParameter">If class ID is null</exception>
		public static Manager GetManager(string classID)
		{
			if (classID == null)
				throw new EInvalidParameter("Manager", "classID");

			// Aquire the lock and get the delegator reference:
			m_registryLock.AcquireReaderLock(Int32.MaxValue);			
			Manager manager =  (Manager) m_registryHash[classID];			
			m_registryLock.ReleaseLock();
           
			return manager;
		}


		/// <summary>
		/// Initializes instance of the manager.
		/// </summary>
		public virtual void Initialize()
		{			
			m_initialized = true;
		}


		/// <summary>
		/// UnInitializes instance of the manager.
		/// </summary>
		public virtual void UnInitialize()
		{
			m_initialized = false;
		}


		/// <summary>
		/// Returns flag if the manager has been initialized.
		/// </summary>
		/// <returns></returns>
		public bool IsInitialized()
		{
			return m_initialized;
		}
	}
}
