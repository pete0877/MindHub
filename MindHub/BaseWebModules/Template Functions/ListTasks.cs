using System;
using System.Collections;
using System.Globalization; 
using Engine;
using Common;


namespace BaseWebModules
{
	/// <summary>
	/// ListTasks(pageTemplate, itemTemplate)
	/// List calendar entries using the given template. By default this function will list calendar entries for today only.
	/// Parameters:
	/// 	pageTemplate(required) - name of the page template used to wrap the list
	///		itemTemplate(required) - name of the item template used to list calendar items
	/// </summary>
	public class ListTasks : ITemplateFunction
	{
		/// <summary>
		/// Function name registered with the factory.
		/// </summary>
		protected static string g_functionName = "listtasks";	

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
			return new ListTasks();
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
		public ListTasks()
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

			if (arguments.Count < 1)
				throw new ESyntaxError(this, parser.GetTemplateName(), g_functionName, "At one parameter is needed for this function.");

			DataValue itemTemplate = (DataValue) arguments[0];
			if (itemTemplate == "")
				throw new ESyntaxError(this, parser.GetTemplateName(), g_functionName, "The first parameter of this function cannot be empty.");

			string ownerID = session.UserID;
			string output = "";
			
			// List all calendar events that start today:
			DataReader reader = new DataReader(TaskDataObject.SCHEMA_DEFAULT);
			reader += DataOperator.SQL_WILDCARD;
			reader.Where(TaskDataObject.COLUMN_OWNERID + " = '" + ownerID + "'"); 
			//reader.OrderBy(TaskDataObject.COLUMN_STARTDT); 
			//reader.OrderBy(TaskDataObject.COLUMN_ENDDT); 
			reader.Load(transaction); 
			while (reader.Read())
			{
				Parser itemParser = new Parser(new FileTemplate(itemTemplate, session), transaction);
				itemParser.AddInput(reader);
				string itemOutput;
				itemParser.Parse(out itemOutput);
				output += itemOutput;
			}

			return output;
		}
	}
}
