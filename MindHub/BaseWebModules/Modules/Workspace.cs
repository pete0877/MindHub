using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using Engine;
using WebAPI;

namespace BaseWebModules
{
	public class Workspace
	{
		// Web function delegators
		public static WebFunctionDelegator g_testFunctionDelegator;
		public static WebFunctionDelegator g_testListFunctionDelegator;

		public static WebFunctionDelegator g_DisplayMenuDelegator;
		public static WebFunctionDelegator g_DisplayTodayDelegator;
		public static WebFunctionDelegator g_DisplayDelegator;

		/// <summary>
		/// Variable: Process configuration ID
		/// </summary>
		static public string VAR_PROCESS_CONFIG_ID		= "vpcid";

		/// <summary>
		/// Configuration variable: Controller class ID
		/// </summary>
		static public string CONFIG_CONTROLLER			= "controller";

		/// <summary>
		/// Function: Temporary test function
		/// </summary>
		static public string FUNCTION_TESTFUNCTION		= "testfunction";
		static public string FUNCTION_TESTLISTFUNCTION	= "testlistfunction";

		static public string FUNCTION_DISPLAYMENU		= "displaymenu";
		static public string FUNCTION_DISPLAYTODAY		= "displaytoday";
		static public string FUNCTION_DISPLAY			= "display";
		

		/// <summary>
		/// Static module initializer. Registers all web functions and controllers.
		/// </summary>
		static public void Initialize(WebFunctionRegistry registry)
		{
			// Register web functions:
			g_testFunctionDelegator = new WebFunctionDelegator(TestFunction);
			registry.RegisterFunction(FUNCTION_TESTFUNCTION, g_testFunctionDelegator);

			g_testListFunctionDelegator = new WebFunctionDelegator(TestListFunction);
			registry.RegisterFunction(FUNCTION_TESTLISTFUNCTION, g_testListFunctionDelegator);

			
			g_DisplayMenuDelegator = new WebFunctionDelegator(DisplayMenu);
			registry.RegisterFunction(FUNCTION_DISPLAYMENU, g_DisplayMenuDelegator);

			g_DisplayTodayDelegator = new WebFunctionDelegator(DisplayToday);
			registry.RegisterFunction(FUNCTION_DISPLAYTODAY, g_DisplayTodayDelegator);

			g_DisplayDelegator = new WebFunctionDelegator(Display);
			registry.RegisterFunction(FUNCTION_DISPLAY, g_DisplayDelegator);

			// Register controllers:
			TestController.Register();
		}


		/// <summary>
		/// Web function: Temporary test function. Executes controller.
		/// </summary>
		/// <param name="request">Web request (comes from Page.Request)</param>
		/// <param name="input">Web request input (comes from Page.Request.Params)</param>
		/// <param name="output">HTML output writer</param>
		/// <param name="session">Valid user session</param>
		/// <param name="response">HTTP response object</param>
		static public void TestFunction(HttpRequest request, NameValueCollection input, Engine.Session session, HtmlTextWriter output, HttpResponse response)
		{
			// This test function's temporary role is to open up a processor based on configuration ID
			string processConfigID = input[VAR_PROCESS_CONFIG_ID];
			if (processConfigID == null || processConfigID == "")
				if (processConfigID == null)
					throw new EInvalidParameter(FUNCTION_TESTFUNCTION, VAR_PROCESS_CONFIG_ID);

			// Get the configuration:
			Config config = ConfigManager.GetManager().GetConfig(processConfigID, session.EnvironmentID);
			string controllerClass = config[CONFIG_CONTROLLER];
			if (controllerClass == "")
				throw new EInvalidParameter(FUNCTION_TESTFUNCTION, CONFIG_CONTROLLER, controllerClass, "Configuration " + processConfigID + " contains no reference to controller class");
			
			// Create and execute the controller:
			IController controller;
			try
			{
				controller = (IController) ClassFactory.GetFactory().Produce(controllerClass);
			}
			catch (Exception e)
			{
				throw new EInvalidParameter(FUNCTION_TESTFUNCTION, CONFIG_CONTROLLER, controllerClass, "Configuration " + processConfigID + " contains invalid controller reference (could not create the controller) : " + e.ToString());
			}

			controller.Execute(input, output, session);
		}


		/// <summary>
		/// Web function: Temporary test function. Lists objects.
		/// </summary>
		/// <param name="request">Web request (comes from Page.Request)</param>
		/// <param name="input">Web request input (comes from Page.Request.Params)</param>
		/// <param name="output">HTML output writer</param>
		/// <param name="session">Valid user session</param>
		/// <param name="response">HTTP response object</param>
		static public void TestListFunction(HttpRequest request, NameValueCollection input, Engine.Session session, HtmlTextWriter output, HttpResponse response)
		{
			using (Transaction transaction = new Transaction(session))
			{
				Parser pageParser = new Parser(new FileTemplate("caseslist.html", session), transaction);
				pageParser.AddInput(session);
				pageParser.Parse(output, "stophere");

				// Find the user:
				DataReader reader = new DataReader("TCase");
				reader += DataOperator.SQL_WILDCARD;
				reader.Load(transaction); 
				while (reader.Read())
				{
					Parser itemParser = new Parser(new FileTemplate("caseslistitem.html", session), transaction);
					itemParser.AddInput(reader);
					itemParser.Parse(output);		
				}

				pageParser.Parse(output);
			}
		}


		/// <summary>
		/// Web function: Displays main menu.
		/// </summary>
		/// <param name="request">Web request (comes from Page.Request)</param>
		/// <param name="input">Web request input (comes from Page.Request.Params)</param>
		/// <param name="output">HTML output writer</param>
		/// <param name="session">Valid user session</param>
		/// <param name="response">HTTP response object</param>
		static public void DisplayMenu(HttpRequest request, NameValueCollection input, Engine.Session session, HtmlTextWriter output, HttpResponse response)
		{			
			using (Transaction transaction = new Transaction(session))
			{
				Parser parser = new Parser(new FileTemplate("mainmenu.html", session), transaction);
				parser.AddInput(session);
				parser.Parse(output);
			}
		}


		/// <summary>
		/// Web function: Displays Today panel
		/// </summary>
		/// <param name="request">Web request (comes from Page.Request)</param>
		/// <param name="input">Web request input (comes from Page.Request.Params)</param>
		/// <param name="output">HTML output writer</param>
		/// <param name="session">Valid user session</param>
		/// <param name="response">HTTP response object</param>
		static public void DisplayToday(HttpRequest request, NameValueCollection input, Engine.Session session, HtmlTextWriter output, HttpResponse response)
		{			
			using (Transaction transaction = new Transaction(session))
			{
				Parser parser = new Parser(new FileTemplate("today.html", session), transaction);
				parser.AddInput(session);
				parser.Parse(output);
			}

			URLHistory urlHistory = new URLHistory(session, response);
			urlHistory.RecordVisit(request.RawUrl);
		}




		/// <summary>
		/// Web function: Parses given template
		/// </summary>
		/// <param name="request">Web request (comes from Page.Request)</param>
		/// <param name="input">Web request input (comes from Page.Request.Params)</param>
		/// <param name="output">HTML output writer</param>
		/// <param name="session">Valid user session</param>
		/// <param name="response">HTTP response object</param>
		static public void Display(HttpRequest request, NameValueCollection input, Engine.Session session, HtmlTextWriter output, HttpResponse response)
		{			
			string templatePath = input["page"];
			if (templatePath == "" || templatePath == null)
				throw new EInvalidParameter(FUNCTION_DISPLAY, "page");

			using (Transaction transaction = new Transaction(session))
			{
				Parser parser = new Parser(new FileTemplate(templatePath, session), transaction);
				parser.AddInput(session);
				parser.Parse(output);
			}
		}
	}
}
