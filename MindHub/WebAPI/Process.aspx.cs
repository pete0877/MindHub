using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Threading;
using Engine;

namespace WebAPI
{
	/// <summary>
	/// Summary description for Process.
	/// </summary>
	public class Process : System.Web.UI.Page
	{
		/// <summary>
		/// Web function - login
		/// </summary>
		static public string FUNCTION_LOGIN		= "login";

		/// <summary>
		/// Web function - log off
		/// </summary>
		static public string FUNCTION_LOGOFF	= "logoff";

		/// <summary>
		/// Variable name - function
		/// </summary>
		static public string VAR_FUNCTION			= "vf";

		/// <summary>
		/// Variable name - environment ID
		/// </summary>
		static public string VAR_ENVIRONMENT_ID		= "venvid";

		/// <summary>
		/// Variable name - username
		/// </summary>
		static public string VAR_USERNAME			= "vu";

		/// <summary>
		/// Variable name - password
		/// </summary>
		static public string VAR_PASSWORD			= "vp";

		/// <summary>
		/// Variable name - password
		/// </summary>
		static public string VAR_INVALIDLOGINURL	= "vilurl";
		

		/// <summary>
		/// Relative URL to the workspace page:
		/// </summary>
		static public string URL_TERMINAL			= "html/terminal.html";

		/// <summary>
		/// Relative URL to the login page:
		/// </summary>
		static public string URL_LOGIN				= "html";

		/// <summary>
		/// Relative URL to the no-session page:
		/// </summary>
		static public string URL_NO_SESSION			= "nosession.html";


		/// <summary>
		/// Delgagor for the logoff function
		/// </summary>
		public static WebFunctionDelegator g_logoffFunctionDelegator;

		/// <summary>
		/// Main web application entry point. Responsible for rendering all HTML.
		/// </summary>
		/// <param name="output">HTML output writer</param>
		protected override void Render(HtmlTextWriter output) 
		{
			try
			{

				// Find function name:
				string functionName = Request.Params[VAR_FUNCTION];
				if (functionName == null || functionName == "")
					throw new EInvalidParameter(this, VAR_FUNCTION);	

				// Process special request functions:
				if (functionName == FUNCTION_LOGIN)
					LogIn(this.Request, this.Request.Params, output, Session, this.Response);
				else
				{
					// Find the pointer to the web function:
					WebFunctionRegistry registry = WebFunctionRegistry.GetRegistry();
					WebFunctionDelegator function = registry.GetFunction(functionName);
					if (function == null)
						throw new EInvalidParameter(this, VAR_FUNCTION, functionName, "Web function was not registered. Ensure plugins loaded correctly.");

					// Find the session:
					string sessionID = Session.SessionID;
					string environmentID = (string) Session[VAR_ENVIRONMENT_ID];

					if (sessionID == null || environmentID == null)
						throw new ENoSession();

					Engine.Session session = SessionManager.GetManager().GetSession(sessionID, environmentID);
					if (session == null)
						throw new ENoSession();
					
					// Execute the web function:
					function(
						Request,
						Request.Params, 
						session,
						output, 
						Response);	
				}
			}
			catch(ThreadAbortException e)
			{
				e.ToString(); // This is just so that warnings don't appear upon compilation. No error is taking places here.
			}
			catch(ENoSession e)
			{
				e.ToString(); // This is just so that warnings don't appear upon compilation. No error is taking places here.
				Response.Redirect("/" + URL_NO_SESSION);					
			}
			catch(Exception e)
			{
				// Output error message:
				output.Write("<h2>Service Error</h2>");
				output.Write(e.Message);
				output.Write("<p>");
				output.Write("<b>Details:</b><pre>" + e.ToString() + "</pre>");

				// Log error message:
				LogManager.Log(this, e.ToString(), LogManager.Level.ERROR);
			}			
		}


		/// <summary>
		/// Static base web function initialization.
		/// </summary>
		static public void Initialize()
		{
			WebFunctionRegistry registry = WebFunctionRegistry.GetRegistry();

			g_logoffFunctionDelegator = new WebFunctionDelegator(LogOff);
			registry.RegisterFunction(FUNCTION_LOGOFF, g_logoffFunctionDelegator);		
		} 


		/// <summary>
		/// Specialized web function for login processing (requires to see the HTTP session object)
		/// </summary>
		/// <param name="input">Web request input (comes from Page.Request.Params)</param>
		/// <param name="output">HTML output writer</param>
		/// <param name="httpSession">HTTP session</param>
		/// <exception cref="EInvalidParameter">If any of the required web variables are null or empty: VAR_ENVIRONMENT_ID, VAR_USERNAME, VAR_PASSWORD</exception>
		static public void LogIn(HttpRequest request, NameValueCollection input, HtmlTextWriter output, HttpSessionState httpSession, HttpResponse response)
		{
			string environmentID = input[VAR_ENVIRONMENT_ID];
			string username = input[VAR_USERNAME];
			string password = input[VAR_PASSWORD];
			string invalidLoginURL = input[VAR_INVALIDLOGINURL];

			try
			{
				if (environmentID == null || environmentID == "")
					throw new EInvalidParameter(FUNCTION_LOGIN, VAR_ENVIRONMENT_ID);

				if (username == null || username == "")
					throw new EInvalidParameter(FUNCTION_LOGIN, VAR_USERNAME);

				if (password == null || password == "")
					throw new EInvalidParameter(FUNCTION_LOGIN, VAR_PASSWORD);

				using (Transaction transaction = new Transaction(environmentID))
				{
					UserDataObject user = new UserDataObject();
					user.Initilize(transaction);
					user.LoadFromLogin(username, password, transaction);

					// Create the session
					Engine.Session newSession = new Engine.Session(environmentID, httpSession.SessionID);
					newSession.AttachTo(user);

					// Perform any necessary security caching:
					SecurityManager.GetManager().CacheSecurity(newSession, transaction);

					// Add the session to global list:
					SessionManager.GetManager().AddSession(newSession);			
		
					// Record environment ID in the HTTP session
					httpSession[VAR_ENVIRONMENT_ID] = newSession.EnvironmentID;
					
					Engine.Environment environment = EnvironmentManager.GetManager().GetEnvironment(transaction.EnvironmentID);
					response.Redirect("/" + environment.ID + "/" + newSession.LanguageCd + "/" + newSession.StyleCd + "/" + URL_TERMINAL);	
				}			
			}
			catch (EInvalidLogin e)
			{
				e.ToString(); // This is just so that warnings don't appear upon compilation. No error is taking places here.
				response.Redirect("/" + environmentID + "/" + invalidLoginURL);				
			}
		}


		/// <summary>
		/// Web function - logoff
		/// </summary>
		/// <param name="request">Web request (comes from Page.Request)</param>
		/// <param name="input">Web request input (comes from Page.Request.Params)</param>
		/// <param name="output">HTML output writer</param>
		/// <param name="session">Valid user session</param>
		/// <param name="response">HTTP response object</param>
		static public void LogOff(HttpRequest request, NameValueCollection input, Engine.Session session, HtmlTextWriter output, HttpResponse response)
		{		
			SessionManager.GetManager().RemoveSession(session);
			response.Redirect("/" + session.EnvironmentID + "/" + session.LanguageCd + "/" + session.StyleCd + "/" + URL_LOGIN);	
		}


		/// <summary>
		/// Overriden method used to identify instances of this class. Used mostly during error handling.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "Process.aspx";
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{  	
		}
		#endregion
	}
}
