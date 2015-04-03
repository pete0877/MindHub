using System;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Web;
using System.Web.UI;
using Engine;

namespace WebAPI
{
	/// <summary>
	/// Web function delegator.
	/// </summary>
	public delegate void WebFunctionDelegator(HttpRequest request, NameValueCollection input, Engine.Session session, HtmlTextWriter output, HttpResponse response);

	/// <summary>
	/// Web function registry. All web modules use this class to register web functions executable through: process.aspx?vf=(function name)
	/// </summary>
	public class WebFunctionRegistry
	{
		/// <summary>
		/// Singelton manager reference
		/// </summary>
		static protected WebFunctionRegistry g_registry;

		/// <summary>
		/// Web function pointer hash
		/// </summary>
		protected Hashtable m_hash;

		/// <summary>
		/// Lock on m_hash
		/// </summary>
		protected ReaderWriterLock m_lock;

		
		/// <summary>
		/// Static constructor. Creates the singelton instance.
		/// </summary>
		static WebFunctionRegistry()
		{
			g_registry = new WebFunctionRegistry();
		}


		/// <summary>
		/// Default constructor. Creates empty web functions hash and the lock.
		/// </summary>
		protected WebFunctionRegistry()
		{
			m_hash = new Hashtable();
			m_lock = new ReaderWriterLock();
		}


		/// <summary>
		/// Returns referece to the singelton registry
		/// </summary>
		/// <returns>Reference to sigelton registry</returns>
		public static WebFunctionRegistry GetRegistry()
		{			
			return g_registry;
		}


		/// <summary>
		/// Registers function. 
		/// Perfors set by reference (unsafe).
		/// </summary>
		/// <param name="functionName">Name of the function</param>
		/// <param name="implementation">Delegator (function pointer)</param>
		/// <exception cref="EInvalidParameter">If any paraters are null</exception>
		public void RegisterFunction(string functionName, WebFunctionDelegator implementation)		
		{
			if (functionName == null)
				throw new EInvalidParameter(this, "functionName");

			if (implementation == null)
				throw new EInvalidParameter(this, "implementation");

			// Aquire writer lock and set the entry:
			m_lock.AcquireWriterLock(Int32.MaxValue);
			m_hash[functionName] = implementation;
			m_lock.ReleaseLock();
		}

		
		/// <summary>
		/// Returns function delegator (pointer) referenced be previously registered function name.
		/// Does not validate the arguments as they would never be null or empty.
		/// Perfors get by reference (unsafe).
		/// </summary>
		/// <param name="functionName"></param>
		/// <returns></returns>
		public WebFunctionDelegator GetFunction(string functionName)		
		{
			// Aquire writer lock and set the entry:
			m_lock.AcquireReaderLock(Int32.MaxValue);			
			WebFunctionDelegator implementation = (WebFunctionDelegator) m_hash[functionName];
			m_lock.ReleaseLock();

			if (implementation == null)
				throw new Exception("Web function [" + functionName + "] was not registered");

			return implementation;
		}
	}
}
