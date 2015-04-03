using System;
using System.Collections;
using System.Globalization; 
using Common;


namespace Engine
{
	/// <summary>
	/// FormatTime(dateValue, formatDetail)
	/// Formats given date according to formatting detail. If [formatDetail] is not give, the date is formatted using the default format.
	/// </summary>
	public class FormatTime : ITemplateFunction
	{
		/// <summary>
		/// Function name registered with the factory.
		/// </summary>
		protected static string g_functionName = "formattime";	

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
			return new FormatTime();
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
		public FormatTime()
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
			if (arguments.Count < 1)
				throw new ESyntaxError(this, parser.GetTemplateName(), g_functionName, "At one parameter is needed for this function.");

			DataValue dateValue = (DataValue) arguments[0];
			DateTime dateTime = dateValue.GetDateTime();
			if (dateTime == DataValue.DEFAULT_DATETIME)
				return "";

			string formatDetaill;
			if (arguments.Count > 1) 
				formatDetaill = (DataValue) arguments[1];
			else
				formatDetaill = "t";

			string output = dateTime.ToString(formatDetaill, DateTimeFormatInfo.InvariantInfo);

			return output;
		}
	}
}
