using System;
using System.Collections;
using Common;

namespace Engine
{
	/// <summary>
	/// Add(arg1, arg2, ... argN) 
	/// Adds all arguments together. Type of the first argument deternimes the type of returned value (float / integer)
	/// </summary>
	public class Add : ITemplateFunction
	{
		/// <summary>
		/// Function name registered with the factory.
		/// </summary>
		protected static string g_functionName = "add";	

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
			return new Add();
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
		public Add()
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
			if (arguments.Count == 0)
				return 0;

			DataValue dataValue = (DataValue) arguments[0];
			if (DataValue.DataType.Double == dataValue.GetDataType())
			{
				double result = 0;
				IEnumerator itrArguments = arguments.GetEnumerator();
				while (itrArguments.MoveNext())
				{
					DataValue argument = (DataValue) itrArguments.Current;
					result += argument.GetDouble();
				}
				return result;
			}
			
			if (DataValue.DataType.Long == dataValue.GetDataType())
			{
				long result = 0;
				IEnumerator itrArguments = arguments.GetEnumerator();
				while (itrArguments.MoveNext())
				{
					DataValue argument = (DataValue) itrArguments.Current;
					result += argument.GetLong();
				}
				return result;
			}
			
			return 0;
		}
	}
}
