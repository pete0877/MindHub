using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using Engine;

namespace BaseWebModules
{
	/// <summary>
	/// Typical GUI dialog. Contains Ok and Cancel buttons.
	/// </summary>
	public class GUIDODialog : GUIWindow
	{
		/// <summary>
		/// Class ID.
		/// </summary>
		protected static string g_classID = "BaseWebModules.GUIDODialog";

		/// <summary>
		/// Singelton delegator instance reference.
		/// </summary>
		public static CreateObjectDelegator g_creator;

		/// <summary>
		/// Default template for displaying sucessful operatoin message after Ok is clicked and operation suceeds.
		/// </summary>
		static public string TEMPLATE_DATA_OBJECT_SAVED			= "gui/guidatasaved.html";

		/// <summary>
		/// Default state template. 
		/// </summary>
		static public string TEMPLATE_STATE						= "gui/guidodialogstate.html";

		/// <summary>
		/// Variable: Configuration ID
		/// </summary>
		static public string VAR_CONFIG_ID						= "vcid";

		/// <summary>
		/// Variable: Data object ID
		/// </summary>
		public const string VAR_DATA_OBJECT_ID					= "vdaid";

		/// <summary>
		/// Variable: Title code
		/// </summary>
		static public string CONFIG_TITLE						= "title";

		/// <summary>
		/// Variable: Class name fo the data object
		/// </summary>
		static public string CONFIG_OBJECT_CLASS				= "objclass";

		/// <summary>
		/// Variable: Config ID of the data object
		/// </summary>
		static public string CONFIG_OBJECT_CONFIG_ID			= "objconfig";
		
		/// <summary>
		/// Variable: Name of the item template to be used
		/// </summary>
		static public string CONFIG_TEMLATE_CONTENT				= "content";

		/// <summary>
		/// Variable: Security level required to operate on the list
		/// </summary>
		static public string CONFIG_SECURITY_LEVEL				= "level";

		/// <summary>
		/// Variable: Security guard to authorize the operation on this list
		/// </summary>
		static public string CONFIG_SECURITY_LEVEL_GUARD		= "levelguard";

		/// <summary>
		/// Variable: Security guard to authorize the operation on this list
		/// </summary>
		static public string CONFIG_SECURITY_ACTION_GUARD		= "actionguard";


		/// <summary>
		/// 'Add' button
		/// </summary>
		GUIButton m_saveButton;

		/// <summary>
		/// 'Cancel' button
		/// </summary>
		GUIButton m_cancelButton;
		
		/// <summary>
		/// Configuration ID of the data object being displayed
		/// </summary>
		protected string m_dataObjectConfigID;

		/// <summary>
		/// ID of the data object being displayed
		/// </summary>
		protected string m_dataObjectID;

		/// <summary>
		/// Class ID of the data object being displayed
		/// </summary>
		protected string m_dataObjectClassID;
		
		/// <summary>
		/// Data object being displayed.
		/// </summary>
		protected IDataObject m_dataObject;


		/// <summary>
		/// Constructor.
		/// </summary>
		public GUIDODialog()
		{
			m_classID = g_classID;
		}


		/// <summary>
		/// Creator method. Returns new instance of this class
		/// </summary>
		/// <returns>Instance of this class</returns>
		public static ICreatable CreateInstance()
		{
			return new GUIDODialog();
		}


		/// <summary>
		/// Initializes singelton delegator reference and registers it with class factory.
		/// </summary>	
		public static void Register()
		{
			g_creator = new CreateObjectDelegator(GUIDODialog.CreateInstance);
			ClassFactory.GetFactory().Register(g_classID, g_creator);
		}


		/// <summary>
		/// Loads data object using given transaciton
		/// </summary>
		/// <param name="transaction">Transaction</param>
		protected void LoadDataObject(Transaction transaction)
		{
			m_dataObject = (IDataObject) ClassFactory.GetFactory().Produce(m_dataObjectClassID);
			m_dataObject.Initilize(m_dataObjectID, m_dataObjectConfigID, transaction);
			m_dataObject.Load(transaction);
		}


		/// <summary>
		/// Creates data object using given transaciton
		/// </summary>
		/// <param name="transaction">Transaction</param>
		protected void CreateDataObject(Transaction transaction)
		{
			m_dataObject = (IDataObject) ClassFactory.GetFactory().Produce(m_dataObjectClassID);
			m_dataObject.Initilize(m_dataObjectConfigID, transaction);
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

			m_stateTemplate = TEMPLATE_STATE;

			m_saveButton = new GUIButton();			
			m_saveButton.Initialize("saveButton", request, input, output, session, response);
			m_saveButton.Label = CodeStrings.BUTTON_SAVE;
			m_saveButton.AddObserver(EVENT_CLICK, this);
			if ((m_GUIFlags & Def.UIFLAG_READONLY) == 0)
				AddActionBarElement(m_saveButton);

			m_cancelButton = new GUIButton();
			m_cancelButton.Initialize("cancelButton", request, input, output, session, response);
			m_cancelButton.Label = CodeStrings.BUTTON_CANCEL;
			m_cancelButton.AddObserver(EVENT_CLICK, this);
			AddActionBarElement(m_cancelButton);

			using (Transaction transaction = new Transaction(session)) 
			{
				string configID = input[VAR_CONFIG_ID];
				if (configID == null || configID == "")
					throw new EInvalidParameter(this, VAR_CONFIG_ID);

				m_dataObjectID = input[VAR_DATA_OBJECT_ID];
				if (m_dataObjectID == "")
					m_dataObjectID = null;

				ConfigManager configManager = ConfigManager.GetManager();
				Config config = configManager.GetConfig(configID, transaction);

				m_title = config[CONFIG_TITLE];

				m_dataObjectConfigID = config[CONFIG_OBJECT_CONFIG_ID];
				if (m_dataObjectConfigID == "")
					throw new EInvalidParameter(this, CONFIG_OBJECT_CONFIG_ID);

				m_dataObjectClassID = config[CONFIG_OBJECT_CLASS];
				if (m_dataObjectClassID == "")
					throw new EInvalidParameter(this, CONFIG_OBJECT_CLASS);

				m_contentTemplate = config[CONFIG_TEMLATE_CONTENT];
				if (m_contentTemplate == "")
					throw new EInvalidParameter(this, CONFIG_TEMLATE_CONTENT);

				string securityLevel = config[CONFIG_SECURITY_LEVEL];
				string securityGuard = config[CONFIG_SECURITY_LEVEL_GUARD];
				if (securityLevel != "" && securityGuard != "")
				{
					SecurityManager securityManager = SecurityManager.GetManager();
					ILevelSecurityGuard guard  = SecurityManager.GetManager().GetLevelSecurityGuard(securityGuard, transaction);
					guard.Authorize(session, securityLevel);
				}

				// TODO: Implement action-based security check
			}
		}


		/// <summary>
		/// Renders main window content.
		/// </summary>
		/// <param name="output">Output writer</param>
		/// <param name="transaction">Transaction</param>
		override public void RenderContent(HtmlTextWriter output, Transaction transaction)
		{
			if (m_dataObjectID != null)
				LoadDataObject(transaction);
			else
				CreateDataObject(transaction);

			m_dataObject.PushData(m_variables, transaction);

			Parser parser = new Parser(new FileTemplate(m_contentTemplate, m_session), transaction);				
			parser.AddInput(m_variables);
			parser.AddInput(m_input);
			parser.AddInput(m_session);
			parser.Parse(output);
		}


		/// <summary>
		/// Main event-observing method that gets called by NotifyObservers when event is produced on observed element.
		/// Dispatches call to appropriate specific event handlers depending on the event name and who sent it.
		/// </summary>
		/// <param name="elementID">Sender element ID</param>
		/// <param name="eventName">Event name</param>
		/// <param name="eventArguments">Event arguments (could be empty)</param>
		override public void ObserveEvent(string elementID, string eventName, ArrayList eventArguments)
		{		
			base.ObserveEvent(elementID, eventName, eventArguments);

			if (eventName == EVENT_CLICK)
			{
				if (elementID == m_saveButton.GetID())
					OnSave(eventArguments);
				else if (elementID == m_cancelButton.GetID())
					OnCancel(eventArguments);
			}
		}


		/// <summary>
		/// Event handler for on-click on the Save button.
		/// </summary>
		virtual public void OnSave(ArrayList eventArguments)
		{
 			using (Transaction transaction = new Transaction(m_session))
			{
				if (m_dataObjectID != null)
					LoadDataObject(transaction);
				else
					CreateDataObject(transaction);

				m_dataObject.PullData(m_input, transaction);
				m_dataObject.Save(transaction);
				transaction.Commit();
			}

			DisplayResults(null);
		}


		/// <summary>
		/// Event handler for on-click on the Cancel button.
		/// </summary>
		virtual public void OnCancel(ArrayList eventArguments)
		{
			// Process Close() command.
			Close(true);
		}


		/// <summary>
		/// Redirects the user to the results page indicating operation result. 
		/// </summary>
		/// <param name="resultsPage">Custom template name under /html/ to be displayed. If resultsPage is null or empty, TEMPLATE_DATA_OBJECT_SAVED will be used.</param>
		virtual public void DisplayResults(string resultsPage)
		{	
			if (resultsPage == null || resultsPage.Length == 0)
				m_response.Redirect(m_session.GetUIPathWeb() + "/html/" + TEMPLATE_DATA_OBJECT_SAVED);
			else
				m_response.Redirect(m_session.GetUIPathWeb() + "/html/" + resultsPage);
		}
	}
}
