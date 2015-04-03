using System;
using System.Collections;
using Common;

namespace Engine
{
	/// <summary>
	/// UIText(name, class, size, maxsize, [flags, other attributes, default value])
	/// Generages a single-line text input element (based on HTML input type 'text').
	/// Parameters:
	///		name (required) - name of the input element (see HTML 'name' attribute)
	///		class (required) - CSS class of the input element (see HTML 'class' attribute)
	///		size (required) - size of the input element in characters (see HTML 'size' attribute)
	///		maxlength (required) - size of the input element in characters (see HTML 'maxlength' attribute)
	///		flags - bitmap of flags (see BaseWebModules.Def.UIFLAG_* flags)
	///		other attributes - additional HTML attributes appended to the HTML tag
	///		default value- default value if value for name is not found in input hashes
	///		read-only class - CSS class of the value displayed if element is rendered in read-only mode (see HTML 'class' attribute)
	/// </summary>
	public class UIText : ITemplateFunction
	{
		/// <summary>
		/// Function name registered with the factory.
		/// </summary>
		protected static string g_functionName = "uitext";	

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
			return new UIText();
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
		public UIText()
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
			if (arguments.Count < 4)
				throw new ESyntaxError(this, parser.GetTemplateName(), g_functionName, "At least 4 parameters are needed for this function.");

			// Initialize the parameters
			DataValue paramName = "";
			DataValue paramClass = "";
			DataValue paramSize = "";
			DataValue paramMaxLen = "";
			DataValue paramFlags = Def.UIFLAG_NONE;
			DataValue paramOtherHTML = "";
			DataValue paramDefault = "";
			DataValue paramReadOnlyClass = "";

			if (arguments.Count > 0)
				paramName = (DataValue) arguments[0];

			if (arguments.Count > 1)
				paramClass = (DataValue) arguments[1];

			if (arguments.Count > 2)
				paramSize = (DataValue) arguments[2];

			if (arguments.Count > 3)
				paramMaxLen = (DataValue) arguments[3];

			if (arguments.Count > 4)
				paramFlags = (DataValue) arguments[4];

			if (arguments.Count > 5)
				paramOtherHTML = (DataValue) arguments[5];

			if (arguments.Count > 6)
				paramDefault = (DataValue) arguments[6];

			if (arguments.Count > 7)
				paramReadOnlyClass = (DataValue) arguments[7];
			

			// Validate parameters:
			if (paramName == "")
				throw new ESyntaxError(this, parser.GetTemplateName(), g_functionName, "First parameter cannot be an empty string for this function.");

			// Determine current value
			DataValue currentValue = parser.EvaluateVariable(paramName);
			if (currentValue == "")
				currentValue = paramDefault;

			string output;
			if ((paramFlags & Def.UIFLAG_READONLY) > 0)
				output = "<font class=\"" + paramReadOnlyClass + "\">" + currentValue + "</font>";
			else
				output = "<input type=\"text\" name=\"" + paramName + "\" value=\"" + currentValue + "\" class=\"" + paramClass + "\" size=\"" + paramSize + "\" maxlength=\"" + paramMaxLen + "\" " + paramOtherHTML + ">";

			return output;
		}
	}
}
