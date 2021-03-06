using System;
using System.Collections;
using Common;

namespace Engine
{
	/// <summary>
	/// GetString(stringID, [setID]) 
	/// Returns string from the StringManager.
	/// </summary>
	public class GetString : ITemplateFunction
	{
		/// <summary>
		/// Function name registered with the factory.
		/// </summary>
		protected static string g_functionName = "getstring";	

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
			return new GetString();
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
		public GetString()
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
			if (arguments.Count != 1 && arguments.Count != 2)
				throw new ESyntaxError(this, parser.GetTemplateName(), g_functionName, "One or two parameters are required for this function. Correct syntax: GetString(stringID, [setID])");

			string stringID = (string) ((DataValue) arguments[0]);

			if (arguments.Count == 1)
				return (string) (new CodeString(transaction, parser.GetLanguageCd(), stringID));
			
			string setID = (string) ((DataValue) arguments[1]);

			return (string) (new CodeString(transaction, parser.GetLanguageCd(), setID, stringID));
		}
	}
}
