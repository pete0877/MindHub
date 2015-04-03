using System;
using System.Collections;
using Common;

namespace Engine
{
	/// <summary>
	/// Eval(string expression)
	/// Returns Evald string expression.
	/// </summary>
	public class Eval : ITemplateFunction
	{
		/// <summary>
		/// Function name registered with the factory.
		/// </summary>
		protected static string g_functionName = "eval";	

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
			return new Eval();
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
		public Eval()
		{
		}


		/// <summary>
		/// Evals the function into DataValue.
		/// </summary>
		/// <param name="arguments">List of Evald function arguments.</param>
		/// <param name="parser">The parser object from which additional inforamtion about context can be acquired.</param>
		/// <param name="session">User session (null if template not parsed for a user).</param>
		/// <returns>Evald function as data value</returns>
		public DataValue Evaluate(ArrayList arguments, Transaction transaction, IParser parser, Engine.Session session)
		{		
			if (arguments.Count < 1)
				throw new ESyntaxError(this, parser.GetTemplateName(), g_functionName, "This function requires exactly one parameter.");

			// TODO: template function Eval() should actually Eval entire expression - not just variable

			DataValue expression = (DataValue) arguments[0];
			return parser.EvaluateVariable(expression);
		}
	}
}
