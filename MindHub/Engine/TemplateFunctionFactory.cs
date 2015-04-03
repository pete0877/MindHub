using System;
using System.Collections;
using System.Threading;

namespace Engine
{
	/// <summary>
	/// Delegate for ITemplateFunction creator functions
	/// </summary>
	public delegate ITemplateFunction CreateTemplateFunctionDelegator();

	/// <summary>
	/// Class factory for ITemplateFunction. Responsible for storing template function creator delegates.
	/// </summary>
	public class TemplateFunctionFactory
	{
		/// <summary>
		/// Singelton pointer
		/// </summary>
		static protected TemplateFunctionFactory g_factory;
        
		/// <summary>
		/// Hash for storing pairs of (function name, creator delegate)
		/// </summary>
		protected Hashtable m_hash;

		/// <summary>
		/// Provides lock on the hash
		/// </summary>
		protected ReaderWriterLock m_lock;
 

		/// <summary>
		/// Default constructor. Creates empty class factory.
		/// </summary>
		protected TemplateFunctionFactory()
		{			
			m_hash = new Hashtable();
			m_lock = new ReaderWriterLock();
		}


		/// <summary>
		/// Static constructor that creates the singelton
		/// </summary>
		static TemplateFunctionFactory()
		{
			g_factory = new TemplateFunctionFactory();

			// Register standard web functions:
			Echo.Register();
			ConCat.Register();
			Add.Register();
			EQ.Register();
			NEQ.Register();
			GT.Register();
			LT.Register();
			GetString.Register();
			PrintIf.Register();
			FormatDate.Register();
			FormatTime.Register();
			Now.Register();
			Set.Register();
			Eval.Register();

			// Register UI web functions:
			UIText.Register();
			UITextArea.Register();
			UIDropDown.Register();	
			UICheckBox.Register();
		}


		/// <summary>
		/// Returns singelton instance of the class factory
		/// </summary>
		/// <returns>Class factory singelton</returns>
		public static TemplateFunctionFactory GetFactory()
		{			
			return g_factory;
		}


		/// <summary>
		/// Adds creator delegate to the class factory for given class ID
		/// </summary>
		/// <param name="functionName">Function name (e.g. 'echo')</param>
		/// <param name="creator">Creator delegator</param>
		/// <exception cref="EInvalidParameter">If either of the parameters are null</exception>
		public void Register(string functionName, CreateTemplateFunctionDelegator creator)		
		{
			if (functionName == null)
				throw new EInvalidParameter(this, "functionName");

			if (creator == null)
				throw new EInvalidParameter(this, "creator");

			// Aquire writer lock and set the entry:
			m_lock.AcquireWriterLock(Int32.MaxValue);
			m_hash[functionName] = creator;
			m_lock.ReleaseLock();
		}


		/// <summary>
		/// Returns new instance of a class of given class ID
		/// </summary>
		/// <param name="functionName">Function name (e.g. 'echo')</param>
		/// <returns>Created instance of template function of given function name</returns>
		/// <exception cref="EInvalidParameter">If functionName is null or not registered</exception>
		public ITemplateFunction Produce(string functionName)
		{
			if (functionName == null)
				throw new EInvalidParameter(this, "functionName");

			// Aquire the lock and get the delegator reference:
			m_lock.AcquireReaderLock(Int32.MaxValue);			
			CreateTemplateFunctionDelegator creator = (CreateTemplateFunctionDelegator) m_hash[functionName];			
			m_lock.ReleaseLock();

			// Validate delegator
			if (creator == null)
				throw new EInvalidParameter(this, "functionName", functionName, "Unregistered template funciton name");
            
			// Create the instance
			ITemplateFunction function = creator();
			return function;
		}
	}
}
