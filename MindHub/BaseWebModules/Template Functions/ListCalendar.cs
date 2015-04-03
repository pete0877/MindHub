using System;
using System.Collections;
using System.Globalization; 
using Engine;
using Common;


namespace BaseWebModules
{
	/// <summary>
	/// ListCalendar(itemTemplate)
	/// List calendar entries using the given template. By default this function will list calendar entries for today only.
	/// Parameters:
	///		itemTemplate(required) - name of the item template used to list calendar items
	/// </summary>
	public class ListCalendar : ITemplateFunction
	{
		/// <summary>
		/// Function name registered with the factory.
		/// </summary>
		protected static string g_functionName = "listcalendar";	

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
			return new ListCalendar();
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
		public ListCalendar()
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
			if (session == null)
				return "";

			if (arguments.Count < 2)
				throw new ESyntaxError(this, parser.GetTemplateName(), g_functionName, "At two parameters are needed for this function.");

			DataValue pageTemplate = (DataValue) arguments[0];
			if (pageTemplate == "")
				throw new ESyntaxError(this, parser.GetTemplateName(), g_functionName, "The first parameter of this function cannot be empty.");

			DataValue itemTemplate = (DataValue) arguments[1];
			if (itemTemplate == "")
				throw new ESyntaxError(this, parser.GetTemplateName(), g_functionName, "The second parameter of this function cannot be empty.");

			string ownerID = session.UserID;
			string output = "";
			
			string pageOutput = "";
			Parser pageParser = new Parser(new FileTemplate(pageTemplate, session), transaction);
			pageParser.Parse(out pageOutput, GUIConstants.VAR_STOP_TAG);
			output += pageOutput;

			// List all calendar events that start today:
			DataReader reader = new DataReader(CalendarDataObject.SCHEMA_DEFAULT);
			reader += DataOperator.SQL_WILDCARD;
			reader.Where(CalendarDataObject.COLUMN_OWNERID + " = '" + ownerID + "'"); 
			reader.OrderBy(CalendarDataObject.COLUMN_STARTDT); 
			reader.OrderBy(CalendarDataObject.COLUMN_ENDDT); 
			reader.Load(transaction); 
			while (reader.Read())
			{
				Parser itemParser = new Parser(new FileTemplate(itemTemplate, session), transaction);
				itemParser.AddInput(reader);
				string itemOutput;
				itemParser.Parse(out itemOutput);
				output += itemOutput;
			}
			pageParser.Parse(out pageOutput);
			output += pageOutput;			

			return output;
		}
	}
}
