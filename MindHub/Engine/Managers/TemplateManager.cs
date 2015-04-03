using System;
using System.Collections;
using System.Threading;

namespace Engine
{
	/// <summary>
	/// Template manager. Caches template contents (unparsed) and their meta-data (parsing helper structures).
	/// Also keeps track of when each template was cached so that it can be compared with whatever is in file system (that however 
	/// is responsibility of client code).
	/// </summary>
	public class TemplateManager : Manager
	{
		/// <summary>
		/// Singelton manager reference
		/// </summary>
		static protected TemplateManager g_manager;
	
		/// <summary>
		/// Environment-keyed hash of file contents
		/// </summary>
		protected Hashtable m_environmentContents;

		/// <summary>
		/// Environment-keyed hash of file meta-datas
		/// </summary>
		protected Hashtable m_environmentMetaDatas;

		/// <summary>
		/// Environment-keyed hash of file caching time stamps
		/// </summary>
		protected Hashtable m_environmentCacheStamp;

		/// <summary>
		/// Environment-keyed hash of reader-writer locks
		/// </summary>
		protected Hashtable m_environmentLocks;

		/// <summary>
		/// Static constructor. Creates the singelton instance.
		/// </summary>
		static TemplateManager()
		{
			g_manager = new TemplateManager();
		}


		/// <summary>
		/// Default constructor. Creates empty environment sets
		/// </summary>
		protected TemplateManager()
		{			
			m_environmentContents = new Hashtable();
			m_environmentMetaDatas = new Hashtable();
			m_environmentCacheStamp = new Hashtable();
			m_environmentLocks = new Hashtable();
		}


		/// <summary>
		/// Returns referece to the singelton manager
		/// </summary>
		/// <returns>Reference to sigelton manager</returns>
		public static TemplateManager GetManager()
		{			
			return g_manager;
		}


		/// <summary>
		/// Initializes instance of manager:
		/// </summary>
		public override void Initialize()
		{
			// Create locks and hashes for each environment:
			EnvironmentManager envMgr = EnvironmentManager.GetManager();
			IDictionaryEnumerator itr = envMgr.GetEnvironments();
			while (itr.MoveNext())
			{
				Environment environment = (Environment) itr.Value;
				m_environmentLocks[environment.ID] = new ReaderWriterLock();
				m_environmentContents[environment.ID] = new Hashtable();
				m_environmentMetaDatas[environment.ID] = new Hashtable();
				m_environmentCacheStamp[environment.ID] = new Hashtable();
			}
			
			base.Initialize();
			LogManager.Log(this, "Initialized");
		}


		/// <summary>
		/// Returns caching date/time stamp for given template within given environment.
		/// If the template was not cached, the returned time stamp will be DateTime(0) (min date).
		/// Performs get by value (safe).
		/// </summary>
		/// <param name="templateID">Template ID (e.g. file path)</param>
		/// <param name="environmentID">Environment ID</param>
		/// <returns>Time stamp of when caching took place.</returns>
		public DateTime GetCacheStamp(string templateID, string environmentID)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (environmentID == null)
				throw new EInvalidParameter(this, "environmentID");

			if (templateID == null)
				throw new EInvalidParameter(this, "templateID");

			ReaderWriterLock cacheLock = (ReaderWriterLock) m_environmentLocks[environmentID];
			if (cacheLock == null)
				throw new EInvalidParameter(this, "environmentID");

			cacheLock.AcquireReaderLock(int.MaxValue);
			Hashtable cacheHash = (Hashtable) m_environmentCacheStamp[environmentID];
			DateTime dateCached;
			if (cacheHash.ContainsKey(templateID))
				dateCached = (DateTime) cacheHash[templateID];
			else
				dateCached = new DateTime(0); // This is a bit of a hack. But we can't compare DateTime to null in .NET because it is a value type

			cacheLock.ReleaseReaderLock();

			return dateCached;
		}


		/// <summary>
		/// Returns cached template content (unparsed character array).
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <param name="templateID">Template ID (e.g. file path)</param>
		/// <param name="environmentID">Environment ID</param>
		/// <returns>Template content</returns>
		public char[] GetCachedContent(string templateID, string environmentID)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (environmentID == null)
				throw new EInvalidParameter(this, "environmentID");

			if (templateID == null)
				throw new EInvalidParameter(this, "templateID");

			ReaderWriterLock cacheLock = (ReaderWriterLock) m_environmentLocks[environmentID];
			if (cacheLock == null)
				throw new EInvalidParameter(this, "environmentID");

			cacheLock.AcquireReaderLock(int.MaxValue);
			Hashtable cacheHash = (Hashtable) m_environmentContents[environmentID];
			char[] content = (char[]) cacheHash[templateID];
			cacheLock.ReleaseReaderLock();

			if (content == null)
				throw new Exception("Template manager does not have cached template [" + templateID + "] in environment [" + environmentID + "]");

			return content;
		}


		/// <summary>
		/// Returns cached template metadata.
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <param name="templateID">Template ID (e.g. file path)</param>
		/// <param name="environmentID">Environment ID</param>
		/// <returns>Template meta-data</returns>
		public TemplateMetaData GetCachedMetaData(string templateID, string environmentID)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (environmentID == null)
				throw new EInvalidParameter(this, "environmentID");

			if (templateID == null)
				throw new EInvalidParameter(this, "templateID");

			ReaderWriterLock cacheLock = (ReaderWriterLock) m_environmentLocks[environmentID];
			if (cacheLock == null)
				throw new EInvalidParameter(this, "environmentID");

			cacheLock.AcquireReaderLock(int.MaxValue);
			Hashtable cacheHash = (Hashtable) m_environmentMetaDatas[environmentID];
			TemplateMetaData metaData = (TemplateMetaData) cacheHash[templateID];
			cacheLock.ReleaseReaderLock();

			if (metaData == null)
				throw new Exception("Template manager does not have cached template [" + templateID + "] in environment [" + environmentID + "]");

			return metaData;
		}


		/// <summary>
		/// Caches template specs away.
		/// Performs set by reference (unsafe).
		/// </summary>
		/// <param name="templateID">Template ID (e.g. file path)</param>
		/// <param name="environmentID">Environment ID</param>
		/// <param name="content">Template content</param>
		/// <param name="metaData">Template meta-data</param>
		public void CacheTemplate(string templateID, string environmentID, char[] content, TemplateMetaData metaData)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (environmentID == null)
				throw new EInvalidParameter(this, "environmentID");

			if (templateID == null)
				throw new EInvalidParameter(this, "templateID");

			if (content == null)
				throw new EInvalidParameter(this, "content");

			if (metaData == null)
				throw new EInvalidParameter(this, "metaData");

			ReaderWriterLock cacheLock = (ReaderWriterLock) m_environmentLocks[environmentID];
			if (cacheLock == null)
				throw new EInvalidParameter(this, "environmentID");

			cacheLock.AcquireWriterLock(int.MaxValue);
			
			Hashtable stampsHash = (Hashtable) m_environmentCacheStamp[environmentID];
			Hashtable contentsHash = (Hashtable) m_environmentContents[environmentID];
			Hashtable metaDataHash = (Hashtable) m_environmentMetaDatas[environmentID];

			stampsHash[templateID] = DateTime.Now;
			contentsHash[templateID] = content;
			metaDataHash[templateID] = metaData;
			
			cacheLock.ReleaseWriterLock();
		}
	}
}
