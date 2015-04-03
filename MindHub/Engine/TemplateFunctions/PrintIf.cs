using System;
using System.Collections;
using Common;

namespace Engine
{
	/// <summary>
	/// PrintIf(condition, output, [else-output])
	/// Returns true if all arguments are equal. Requires at least two arguments.
	/// </summary>
	public class PrintIf : ITemplateFunction
	{
		/// <summary>
		/// Function name registered with the factory.
		/// </summary>
		protected static string g_functionName = "printif";	

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
			return new PrintIf();
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
		public PrintIf()
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

			DataValue condition = (DataValue) arguments[0];
			if ((int) condition > 0)
				return (DataValue) arguments[1];
			else
			{
				if (arguments.Count == 3)
					return (DataValue) arguments[2];
			}

			return "";
		}
	}
}
