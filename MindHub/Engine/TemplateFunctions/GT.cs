using System;
using System.Collections;
using Common;

namespace Engine
{
	/// <summary>
	/// GT(arg1, arg2) 
	/// Returns true if arg1 is greater then arg2. (Returns arg1 > arg2)
	/// </summary>
	public class GT : ITemplateFunction
	{
		/// <summary>
		/// Function name registered with the factory.
		/// </summary>
		protected static string g_functionName = "gt";	

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
			return new GT();
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
		public GT()
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
			if (arguments.Count != 2)
				throw new ESyntaxError(this, parser.GetTemplateName(), g_functionName, "Two parameters are needed for this function.");

			DataValue arg1 = (DataValue) arguments[0];
			DataValue arg2 = (DataValue) arguments[1];

			return (arg1.IsGreaterThan(arg2));
		}
	}
}
