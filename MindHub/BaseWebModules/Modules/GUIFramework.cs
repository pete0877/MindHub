using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using Engine;
using WebAPI;

namespace BaseWebModules
{
	/// <summary>
	/// Set of web functions responsible for handlign the GUI framework functions.
	/// </summary>
	public class GUIFramework
	{
		// Web function delegators
		public static WebFunctionDelegator g_guiDelegator;

		/// <summary>
		/// Template: GUI framework header wrapper
		/// </summary>
		static public string TEMPLATE_HEAD_WRAPPER			= "gui/guihead.html";

		/// <summary>
		/// Template: GUI framework footer wrapper
		/// </summary>
		static public string TEMPLATE_FOOT_WRAPPER			= "gui/guifoot.html";

		/// <summary>
		/// Variable: Class ID of GUI element
		/// </summary>
		static public string VAR_GUI_CLASSID			= "vguicid";

		/// <summary>
		/// Variable: Command to be given to the GUI element
		/// </summary>
		static public string VAR_GUI_INSTANCEID			= "vguiiid";


		/// <summary>
		/// Variable: Command to be given to the GUI element
		/// </summary>
		static public string VAR_GUI_COMMAND			= "vguic";

		/// <summary>
		/// Variable: Command arguments to be given to the GUI element
		/// </summary>
		static public string VAR_GUI_COMMANDARGUMENTS	= "vguia";

		/// <summary>
		/// Function: Basic GUI call. All commands passed to GUI Elements go through this method.
		/// </summary>
		static public string FUNCTION_GUIFUNCTION		= "gui";

		/// <summary>
		/// Static module initializer. Registers all web functions and controllers.
		/// </summary>
		static public void Initialize(WebFunctionRegistry registry)
		{
			// Register web functions:
			g_guiDelegator = new WebFunctionDelegator(GUI);
			registry.RegisterFunction(FUNCTION_GUIFUNCTION, g_guiDelegator);

			// Register base GUI classes:
			GUIDOListDialog.Register();
			GUIDODialog.Register();
		}


		/// <summary>
		/// Web function: Basic GUI call. All commands passed to GUI Elements go through this method.
		/// </summary>
		/// <param name="request">Web request (comes from Page.Request)</param>
		/// <param name="input">Web request input (comes from Page.Request.Params)</param>
		/// <param name="output">HTML output writer</param>
		/// <param name="session">Valid user session</param>
		/// <param name="response">HTTP response object</param>
		static public void GUI(HttpRequest request, NameValueCollection input, Engine.Session session, HtmlTextWriter output, HttpResponse response)
		{
			// This test function's temporary role is to open up a processor based on configuration ID
			string classID = input[VAR_GUI_CLASSID];
			if (classID == null || classID == "")
				throw new EInvalidParameter(FUNCTION_GUIFUNCTION, VAR_GUI_CLASSID);

			string instanceID = input[VAR_GUI_INSTANCEID];
			if (instanceID == null || instanceID == "")
				instanceID = classID;

			string command = input[VAR_GUI_COMMAND];
			if (command == null || command == "")
				throw new EInvalidParameter(FUNCTION_GUIFUNCTION, VAR_GUI_COMMAND);

			ArrayList argumentsList = new ArrayList();			
			string arguments = input[VAR_GUI_COMMANDARGUMENTS];
			if (arguments != null && arguments.Length > 0)
			{
				string[] argumentsSet = arguments.Split(',');
				
				IEnumerator itr = argumentsSet.GetEnumerator();
				while (itr.MoveNext())
				{
					string argument = (string) itr.Current;
					DataValue dataValue = new DataValue(argument);
					argumentsList.Add(dataValue);
				}
			}
			
			using (Transaction transaction = new Transaction(session))
			{			
				GUIElement element = (GUIElement) ClassFactory.GetFactory().Produce(classID);			
				element.Initialize(instanceID, request, input, output, session, response);
				element.ProcessCommand(command, argumentsList);

				if (element.IsRendarable())
				{
					Parser parserHead = new Parser(new FileTemplate(TEMPLATE_HEAD_WRAPPER, session), transaction);
					parserHead.AddInput(input);
					parserHead.Parse(output);

					element.Render(transaction);

					Parser parserFoot = new Parser(new FileTemplate(TEMPLATE_FOOT_WRAPPER, session), transaction);
					parserFoot.AddInput(input);
					parserFoot.Parse(output);
				}
			}
		}



		static public void OpenWindow(string classID, string instanceID, HttpRequest request, ArrayList argumentsList, NameValueCollection input, Engine.Session session, HtmlTextWriter output, HttpResponse response)
		{
			input[VAR_GUI_CLASSID] = classID;
			using (Transaction transaction = new Transaction(session))
			{			
				GUIElement element = (GUIElement) ClassFactory.GetFactory().Produce(classID);			
				element.Initialize(instanceID, request, input, output, session, response);
				element.ProcessCommand(GUIWindow.COMMAND_OPEN, argumentsList);

				if (element.IsRendarable())
				{
					Parser parserHead = new Parser(new FileTemplate(TEMPLATE_HEAD_WRAPPER, session), transaction);
					parserHead.AddInput(input);
					parserHead.Parse(output);

					element.Render(transaction);

					Parser parserFoot = new Parser(new FileTemplate(TEMPLATE_FOOT_WRAPPER, session), transaction);
					parserFoot.AddInput(input);
					parserFoot.Parse(output);
				}
			}
		}
	}
}
