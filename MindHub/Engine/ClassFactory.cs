using System;
using System.Collections;
using System.Threading;

namespace Engine
{
	/// <summary>
	/// Delegate for ICreatable creator functions
	/// </summary>
	public delegate ICreatable CreateObjectDelegator();

	/// <summary>
	/// Class factory for ICreatable. Responsible for storing object creator delegates.
	/// </summary>
	public class ClassFactory
	{
		/// <summary>
		/// Singelton pointer
		/// </summary>
		static protected ClassFactory g_factory;
        
		/// <summary>
		/// Hash for storing pairs of (class ID, creator delegate)
		/// </summary>
		protected Hashtable m_hash;

		/// <summary>
		/// Provides lock on the hash
		/// </summary>
		protected ReaderWriterLock m_lock;

		/// <summary>
		/// Default constructor. Creates empty class factory.
		/// </summary>
		protected ClassFactory()
		{			
			m_hash = new Hashtable();
			m_lock = new ReaderWriterLock();
		}


		/// <summary>
		/// Static constructor that creates the singelton
		/// </summary>
		static ClassFactory()
		{
			g_factory = new ClassFactory();
		}


		/// <summary>
		/// Returns singelton instance of the class factory
		/// </summary>
		/// <returns>Class factory singelton</returns>
		public static ClassFactory GetFactory()
		{			
			return g_factory;
		}


		/// <summary>
		/// Adds creator delegate to the class factory for given class ID
		/// </summary>
		/// <param name="classID">Class ID</param>
		/// <param name="creator">Creator delegator</param>
		/// <exception cref="EInvalidParameter">If either of the parameters are null</exception>
		public void Register(string classID, CreateObjectDelegator creator)		
		{
			if (classID == null)
				throw new EInvalidParameter(this, "classID");

			if (creator == null)
				throw new EInvalidParameter(this, "creator");

			// Aquire writer lock and set the entry:
			m_lock.AcquireWriterLock(Int32.MaxValue);
			m_hash[classID] = creator;
			m_lock.ReleaseLock();
		}


		/// <summary>
		/// Returns new instance of a class of given class ID
		/// </summary>
		/// <param name="classID">Class ID</param>
		/// <returns>Created instance of given class ID</returns>
		/// <exception cref="EInvalidParameter">If class ID is null or not registered</exception>
		public ICreatable Produce(string classID)
		{
			if (classID == null)
				throw new EInvalidParameter(this, "classID");

			// Aquire the lock and get the delegator reference:
			m_lock.AcquireReaderLock(Int32.MaxValue);			
			CreateObjectDelegator creator = (CreateObjectDelegator) m_hash[classID];			
			m_lock.ReleaseLock();

			// Validate delegator
			if (creator == null)
				throw new EInvalidParameter(this, "classID", classID, "Unregistered class ID");
            
			// Create the instance and validate the type
			System.Object obj = creator();
			if (obj.GetType().ToString() != classID)
				throw new EInvalidParameter(this, "classID", classID, "Class ID did not match class type: " + obj.GetType().ToString());

			return (ICreatable) obj;
		}
	}
}
