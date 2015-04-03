using System;
using System.Collections;
using System.Threading;

namespace Engine
{
	/// <summary>
	/// Lock manager for any type of object that has an ID
	/// </summary>
	public class LockManager : Manager
	{
		/// <summary>
		/// Defines types of locks that this manager can handle
		/// </summary>
		public enum LockType { Shared, Exclusive };

		/// <summary>
		/// Static singelton instance of the manager
		/// </summary>
		static protected LockManager g_manager;

		/// <summary>
		/// Indicates location of the instance (e.g. Location.APP_SERVER)
		/// </summary>
		protected Location m_location; 		

		/// <summary>
		/// Hash responsible for storing current locks
		/// </summary>
		protected Hashtable m_hash;

		/// <summary>
		/// Provides lock on the hash
		/// </summary>
		protected ReaderWriterLock m_lock;

		/// <summary>
		/// Static constructor. Creates new singelton manager.
		/// </summary>
		static LockManager()
		{
			g_manager = new LockManager();
		}


		/// <summary>
		/// Concrete constructor. Creates empty list of locks.
		/// </summary>
		protected LockManager()
		{			
			m_hash = new Hashtable();
			m_lock = new ReaderWriterLock();
		}


		/// <summary>
		/// Returns the singelton manager reference
		/// </summary>
		/// <returns>Singelton manager reference</returns>
		public static LockManager GetManager()
		{			
			return g_manager;
		}


		/// <summary>
		/// Initializes instance of lock manager:
		/// </summary>
		public override void Initialize()
		{	
			EnvironmentManager envMgr = EnvironmentManager.GetManager();
			m_location = envMgr.GetLocation();
			
			base.Initialize();
			LogManager.Log(this, "Initialized");
		}


		/// <summary>
		/// Returns when a lock of type lockType can be aquired on object of given ID
		/// </summary>
		/// <param name="id">Object ID</param>
		/// <param name="lockType">Lock type</param>
		/// <exception cref="ENotInitialized">If manager is not initialized.</exception>
		/// <exception cref="EInvalidParameter">If object ID is null</exception>
		public void Lock(string id, LockType lockType)
		{
			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (id == null)
				throw new EInvalidParameter(this, "id");

			m_lock.AcquireWriterLock(Int32.MaxValue);

			m_lock.ReleaseLock();

			// TODO: Using m_hash of LockType, mark the fact that ID has been locked with given
			// type or wait till lock can be aquired.
			throw new System.NotImplementedException();
		}


		/// <summary>
		/// Releases lock on a given object
		/// </summary>
		/// <param name="id">Object ID</param>
		/// <exception cref="ENotInitialized">If manager is not initialized.</exception>
		/// <exception cref="EInvalidParameter">If object ID is null</exception>
		public void Release(string id)
		{
			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (id == null)
				throw new EInvalidParameter(this, "id");

			m_lock.AcquireWriterLock(Int32.MaxValue);

			if (m_hash.ContainsKey(id))
				m_hash.Remove(id);

			m_lock.ReleaseLock();
		}


		/// <summary>
		/// Returns true if lock of given type could be at the time aquired for object with given ID
		/// </summary>
		/// <param name="id">Object ID</param>
		/// <param name="lockType">Lock type</param>
		/// <returns>True iff lock can be aquired</returns>
		/// <exception cref="ENotInitialized">If manager is not initialized.</exception>
		/// <exception cref="EInvalidParameter">If object ID is null</exception>
		public bool TryLock(string id, LockType lockType)
		{
			// Validate:
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (id == null)
				throw new EInvalidParameter(this, "id");

			m_lock.AcquireWriterLock(Int32.MaxValue);

			// Unless we find existing lock, we can lock:
			bool canLock = true;

			if (m_hash.ContainsKey(id))
			{
				// If any type of lock exists there already, we can't lock exclusivly:
				if (lockType == LockType.Exclusive)
					canLock = false;
				else
				{
					// If excluising lock already exists, we can't lock:
					if ((LockType) m_hash[id] == LockType.Exclusive)
						canLock = false;				
				}
			}
			
			m_lock.ReleaseLock();

			return canLock;
		}
	}
}
