using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization; 
using Engine;
using Common;


namespace BaseWebModules
{
	/// <summary>
	/// ListSecurityLevels(userID, itemTemplate)
	/// List currently assigned security levels for given user.
	/// Parameters:
	///		userID(required) - ID of the user for whom we are doing the listing
	///		itemTemplate(required) - name of the item template used to list items
	/// </summary>
	public class ListSecurityLevels : ITemplateFunction
	{
		/// <summary>
		/// Function name registered with the factory.
		/// </summary>
		protected static string g_functionName = "listsecuritylevels";	

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
			return new ListSecurityLevels();
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
		public ListSecurityLevels()
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
			string output = "";

			// Initialize and validate all parameters:
			if (session == null)
				return "";

			if (arguments.Count < 2)
				throw new ESyntaxError(this, parser.GetTemplateName(), g_functionName, "At two parameters are needed for this function.");

			DataValue userID = (DataValue) arguments[0];
			if (userID == "")
				throw new ESyntaxError(this, parser.GetTemplateName(), g_functionName, "The first parameter of this function cannot be empty.");

			DataValue itemTemplate = (DataValue) arguments[1];
			if (itemTemplate == "")
				throw new ESyntaxError(this, parser.GetTemplateName(), g_functionName, "The second parameter of this function cannot be empty.");
			
			NameValueCollection parseInput = new NameValueCollection();

			// Attempt to find the user record:
			DataReader reader = new DataReader(LevelSecurityGuard.SCHEMA_USERLEVELS);
			reader += DataOperator.SQL_WILDCARD;
			reader.Where(TaskDataObject.COLUMN_OBJID + " = '" + userID + "'"); 
			reader.Load(transaction); 
			bool foundRecord = false;
			if (reader.Read())
				foundRecord = true;

			// Roll through string set:
			CodeStringSet codeSet = new CodeStringSet(session, LevelSecurityGuard.CODESTRINGSET_SECURITYLEVELS);
			IEnumerator codes = codeSet.GetEnumerator();
			while (codes.MoveNext())
			{
				Parser itemParser = new Parser(new FileTemplate(itemTemplate, session), transaction);

				string itemOutput;
				string level = (string) codes.Current;
				string description = codeSet[level];

				parseInput["vinputname"] = level.ToLower();
				parseInput["vdescription"] = description; 
				
				// Forward GUI Flags:
				parseInput[GUIElement.VAR_GUI_FLAG] = parser.EvaluateVariable(GUIElement.VAR_GUI_FLAG);
				
				itemParser.AddInput(parseInput);				
				itemParser.AddInput(session);			
				
				if (foundRecord)
					itemParser.AddInput(reader);					

				itemParser.Parse(out itemOutput);
				output += itemOutput;
			}

			return output;
		}
	}
}
