using System;
using System.Collections;
using Common;

namespace Engine
{
	/// <summary>
	/// EQ(arg1, arg2, ... argN) 
	/// Returns true if all arguments are equal. Requires at least two arguments.
	/// </summary>
	public class EQ : ITemplateFunction
	{
		/// <summary>
		/// Function name registered with the factory.
		/// </summary>
		protected static string g_functionName = "eq";	

		/// <summary>
		/// Template function creator delegagor.
		/// </summary>
		public static CreateTemplateFunctionDelegator g_creator;


		/// <summary>
		/// Creator method. Returns new instance of this class.
		/// </summary>
		/// <returns>Instance of this class</returns>
		public static ITemplateFunction CreateInstance()
		{
			return new EQ();
		}


		/// <summary>
		/// Registers self with class factory.
		/// </summary>	
		public static void Register()
		{
			g_creator = new CreateTemplateFunctionDelegator(CreateInstance);
			TemplateFunctionFactory.GetFactory().Register(g_functionName, g_creator);
		}


		/// <summary>
		/// Default contructor.
		/// </summary>
		public EQ()
		{
		}


		/// <summary>
		/// Evaluates the function into DataValue.
		/// </summary>
		/// <param name="arguments">List of evaluated function arguments.</param>
		/// <param name="parser">The parser object from which additional inforamtion about context can be acquired.</param>
		/// <param name="session">User session (null if template not parsed for a user).</param>
		/// <returns>Evaluated function as data value</returns>
		public DataValue Evaluate(ArrayList arguments, Transaction transaction, IParser parser, Engine.Session session)
		{		
			if (arguments.Count < 2)
				throw new ESyntaxError(this, parser.GetTemplateName(), g_functionName, "At least two parameters are needed for this function.");

			DataValue compareTo = (DataValue) arguments[0];
			IEnumerator itrArguments = arguments.GetEnumerator();
			itrArguments.MoveNext(); // Skip first argument
			while (itrArguments.MoveNext())
			{
				DataValue other = (DataValue) itrArguments.Current;
				if (!compareTo.IsEqual(other))
					return false;
			}

			return true;
		}
	}
}
