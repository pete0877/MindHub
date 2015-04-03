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
	/// Window class of GUI framework. 
	/// </summary>
	public class GUIWindow : GUIElement
	{
		/// <summary>
		/// Variable for window title.
		/// </summary>
		public const string VAR_GUI_WINDOW_TITLE			= "guiwindowtitle";

		/// <summary>
		/// Default header template. Contains title bar.
		/// </summary>
		static public string TEMPLATE_HEAD					= "gui/guiwindowhead.html";

		/// <summary>
		/// Default footer template.
		/// </summary>
		static public string TEMPLATE_FOOT					= "gui/guiwindowfoot.html";

		/// <summary>
		/// Default action bar template. Contains all action buttons at the bottom of the window.
		/// </summary>
		static public string TEMPLATE_ACTION_BAR			= "gui/guiwindowactionbar.html";

		/// <summary>
		/// Default tool bar template. Contains things like menus, tool icons, etc.
		/// </summary>
		static public string TEMPLATE_TOOL_BAR				= "gui/guiwindowtoolbar.html";

		/// <summary>
		/// Command for opening up a window
		/// </summary>
		public const string COMMAND_OPEN					= "open";

		/// <summary>
		/// Command for closing up a window
		/// </summary>
		public const string COMMAND_CLOSE					= "close";

		/// <summary>
		/// Window title string code.
		/// </summary>
		protected string m_title;

		/// <summary>
		/// Top header template name (parsed before head template).
		/// </summary>
		protected string m_topTemplate;

		/// <summary>
		/// Bottom header template name (parsed after foot template).
		/// </summary>
		protected string m_bottomTemplate;

		/// <summary>
		/// Head template name.
		/// </summary>
		protected string m_headTemplate;

		/// <summary>
		/// Foot template name.
		/// </summary>
		protected string m_footTemplate;		

		/// <summary>
		/// State template name.
		/// </summary>
		protected string m_stateTemplate;	

		/// <summary>
		/// Action bar template name.
		/// </summary>
		protected string m_actionBarTemplate;

		/// <summary>
		/// Tool bar template name.
		/// </summary>
		protected string m_toolBarTemplate;

		/// <summary>
		/// Tool bar list. Contains GUIElements.
		/// </summary>
		protected ArrayList m_toolBar;

		/// <summary>
		/// Action bar list. Contains GUIElements.
		/// </summary>
		protected ArrayList m_actionBar;

		/// <summary>
		/// GUI Flags
		/// </summary>
		protected long m_GUIFlags;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GUIWindow()
		{
			m_toolBar = new ArrayList();
			m_actionBar = new ArrayList();
		}


		/// <summary>
		/// Initializes instance of this GUIElement.
		/// </summary>
		/// <param name="elementID">Element ID</param>
		/// <param name="input">Web input</param>
		/// <param name="session">User session</param>
		/// <param name="response">Response object</param>
		override public void Initialize(string elementID, HttpRequest request, NameValueCollection input, HtmlTextWriter output, Engine.Session session, HttpResponse response)
		{
			base.Initialize(elementID, request, input, output, session, response);
			m_headTemplate			= TEMPLATE_HEAD;
			m_footTemplate			= TEMPLATE_FOOT;
			m_actionBarTemplate		= TEMPLATE_ACTION_BAR;
			m_toolBarTemplate		= TEMPLATE_TOOL_BAR;
			if (input[VAR_GUI_FLAG] != null)
				m_GUIFlags				= new DataValue(input[VAR_GUI_FLAG]);
		}


		/// <summary>
		/// Adds GUI element to the tool bar.
		/// </summary>
		/// <param name="element">GUI element to be added.</param>
		/// <exception cref="EInvalidParameter">If element is null.</exception>
		protected void AddToolBarElement(GUIElement element)
		{
			if (element == null)
				throw new EInvalidParameter(this, "element");

			m_toolBar.Add(element);
			AddChildElement(element);
		}


		/// <summary>
		/// Adds GUI element to the action bar.
		/// </summary>
		/// <param name="element">GUI element to be added.</param>
		/// <exception cref="EInvalidParameter">If element is null.</exception>
		protected void AddActionBarElement(GUIElement element)
		{
			if (element == null)
				throw new EInvalidParameter(this, "element");

			m_actionBar.Add(element);
			AddChildElement(element);
		}


		/// <summary>
		/// Responsible for rendering of this GUI element.
		/// If m_contentTemplate is set and not empty, it will be parsed.
		/// </summary>
		/// <param name="output">Output writer</param>
		/// <param name="transaction">Transaction</param>
		override public void Render(Transaction transaction)
		{
			if (!IsRendarable())
				return;

			// If title code is given, find the actual string:
			if (m_title != null)
			{
				string titleText = new CodeString(m_session, m_title);
				m_variables[VAR_GUI_WINDOW_TITLE] = titleText;
			}

			// Parse top template if it is given:
			if (m_topTemplate != null)
			{
				Parser parser = new Parser(new FileTemplate(m_topTemplate, m_session), transaction);
				parser.AddInput(m_variables);
				parser.AddInput(m_input);				
				parser.Parse(m_output);
			}
				
			// Parse header template:
			if (m_headTemplate != null)
			{
				Parser parser = new Parser(new FileTemplate(m_headTemplate, m_session), transaction);
				parser.AddInput(m_variables);
				parser.AddInput(m_input);
				parser.Parse(m_output);
			}

			// Parse state template:
			if (m_stateTemplate != null)
			{
				Parser parser = new Parser(new FileTemplate(m_stateTemplate, m_session), transaction);
				parser.AddInput(m_variables);
				parser.AddInput(m_input);
				parser.Parse(m_output);
			}

			// Parse tool bar, main content of the window and action bar:
			RenderToolBar(m_output, transaction);
			RenderContent(m_output, transaction);
			RenderActionBar(m_output, transaction);

			// Parse footer of the window.
			if (m_footTemplate != null)
			{
				Parser parser = new Parser(new FileTemplate(m_footTemplate, m_session), transaction);
				parser.AddInput(m_variables);
				parser.AddInput(m_input);
				parser.Parse(m_output);
			}

			// Parse bottom of the page.
			if (m_bottomTemplate != null)
			{
				Parser parser = new Parser(new FileTemplate(m_bottomTemplate, m_session), transaction);
				parser.AddInput(m_variables);
				parser.AddInput(m_input);
				parser.Parse(m_output);
			}
		}		


		/// <summary>
		/// Renders tool bar portion of the window.
		/// </summary>
		/// <param name="output">Output writer</param>
		/// <param name="transaction">Transaction</param>
		virtual public void RenderToolBar(HtmlTextWriter output, Transaction transaction)
		{
			if (m_toolBar.Count == 0 || m_toolBarTemplate == null && m_toolBarTemplate == "")
				return;

			Parser parser = new Parser(new FileTemplate(m_toolBarTemplate, m_session), transaction);				
			parser.AddInput(m_variables);
			parser.AddInput(m_input);
			parser.Parse(output, GUIConstants.VAR_STOP_TAG);


			IEnumerator itr = m_toolBar.GetEnumerator();
			while (itr.MoveNext())
			{
				GUIElement element = (GUIElement) itr.Current;
				element.Render(transaction);
			}

			parser.Parse(output);
		}


		/// <summary>
		/// Renders main window content.
		/// </summary>
		/// <param name="output">Output writer</param>
		/// <param name="transaction">Transaction</param>
		virtual public void RenderContent(HtmlTextWriter output, Transaction transaction)
		{
			if (m_contentTemplate != null)
			{
				Parser parser = new Parser(new FileTemplate(m_contentTemplate, m_session), transaction);				
				parser.AddInput(m_variables);
				parser.AddInput(m_input);
				parser.Parse(output);
			}		
		}


		/// <summary>
		/// Renders action bar portion of the window.
		/// </summary>
		/// <param name="output">Output writer</param>
		/// <param name="transaction">Transaction</param>
		virtual public void RenderActionBar(HtmlTextWriter output, Transaction transaction)
		{
			if (m_actionBar.Count == 0 || m_actionBarTemplate == null && m_actionBarTemplate == "")
				return;

			Parser parser = new Parser(new FileTemplate(m_actionBarTemplate, m_session), transaction);				
			parser.AddInput(m_variables);
			parser.AddInput(m_input);
			parser.Parse(output, GUIConstants.VAR_STOP_TAG);

			IEnumerator itr = m_actionBar.GetEnumerator();
			while (itr.MoveNext())
			{
				GUIElement element = (GUIElement) itr.Current;
				element.Render(transaction);
			}	

			parser.Parse(output);
		}


		/// <summary>
		/// Processes command to be executed on this GUI element.
		/// </summary>
		/// <param name="command">Command name</param>
		/// <param name="arguments">List of arguments</param>
		/// <returns>Flag for if the request has been processed. If this element does not know how to process given command, flase is returned. Sucessful processing causes this method to return true.</returns>
		/// <exception cref="EInvalidParameter">If any of the parameters are null or if processing of the command fails due to invalid command arguments.</exception>
		override public bool ProcessCommand(string command, ArrayList arguments)
		{
			// If base class handled this command, don't do anything.
			if (base.ProcessCommand(command, arguments))
				return true;

			switch (command)
			{
				case COMMAND_OPEN:
					Open(arguments);
					return true;
			}
			return false;
		}


		/// <summary>
		/// Implementation for the open command.
		/// </summary>
		virtual public void Open(ArrayList arguments)
		{
			URLHistory urlHistory = new URLHistory(m_session, m_response);
			urlHistory.RecordVisit(m_request.RawUrl);
		}


		/// <summary>
		/// Command implementation for the open command.
		/// </summary>
		virtual public void Close(bool historyReturn)
		{
			m_renderable = false;	

			if (historyReturn)
			{
				URLHistory urlHistory = new URLHistory(m_session, m_response);
				urlHistory.Return();
			}
		}
	}
}
