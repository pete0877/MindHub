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
	public class GUIDOListDialog : GUIWindow
	{
		/// <summary>
		/// Class ID.
		/// </summary>
		protected static string g_classID = "BaseWebModules.GUIDOListDialog";	

		/// <summary>
		/// Singelton delegator instance reference.
		/// </summary>
		public static CreateObjectDelegator g_creator;

		/// <summary>
		/// Default state template. 
		/// </summary>
		static public string TEMPLATE_STATE						= "gui/guidolistdialogstate.html";

		/// <summary>
		/// Variable: Configuration ID
		/// </summary>
		static public string VAR_CONFIG_ID						= "vcid";

		/// <summary>
		/// Variable: Title code
		/// </summary>
		static public string CONFIG_TITLE						= "title";

		/// <summary>
		/// Variable: Class name fo the data object
		/// </summary>
		static public string CONFIG_OBJECT_CLASS				= "objclass";

		/// <summary>
		/// Variable: Data object configuration ID
		/// </summary>
		static public string CONFIG_OBJECT_CONFIG_ID			= "objconfig";

		/// <summary>
		/// Variable: Class ID of the operator dialog to be called when user clicks on Edit or Add
		/// </summary>
		static public string CONFIG_OPERATOR_DIALOG				= "operator";

		/// <summary>
		/// Variable: Config ID of the operator dialog to be called when user clicks on Edit or Add
		/// </summary>
		static public string CONFIG_OPERATOR_DIALOG_CONFIG		= "operatorconfig";

		/// <summary>
		/// Variable: Name of the item template to be used
		/// </summary>
		static public string CONFIG_TEMLATE_ITEM				= "item";

		/// <summary>
		/// Variable: Name of the column header template to be used
		/// </summary>
		static public string CONFIG_TEMLATE_HEADER				= "header";

		/// <summary>
		/// Variable: Security level required to operate on the list
		/// </summary>
		static public string CONFIG_SECURITY_LEVEL				= "level";

		/// <summary>
		/// Variable: Security guard to authorize the operation on this list
		/// </summary>
		static public string CONFIG_SECURITY_LEVEL_GUARD		= "levelguard";
		

		/// <summary>
		/// 'Add' button
		/// </summary>
		GUIButton m_addButton;

		/// <summary>
		/// 'Cancel' button
		/// </summary>
		GUIButton m_cancelButton;

		/// <summary>
		/// 'Edit' button
		/// </summary>
		GUIButton m_editButton;

		/// <summary>
		/// 'View' button
		/// </summary>
		GUIButton m_viewButton;

		/// <summary>
		/// 'Delete' button
		/// </summary>
		GUIButton m_deleteButton;

		/// <summary>
		/// Configuration of the data object being displayed
		/// </summary>
		protected Config m_dataObjectConfig;

		/// <summary>
		/// Class ID of the data object type being displayed
		/// </summary>
		protected string m_dataObjectClassID;

		/// <summary>
		/// Config ID for the data object type being displayed
		/// </summary>
		protected string m_dataObjectConfigID;

		/// <summary>
		/// GUI class ID of the operator class to be opened when user clicks on Edit or Add
		/// </summary>
		protected string m_operatorDialogClassID;

		/// <summary>
		/// Config ID of the operator class to be opened when user clicks on Edit or Add
		/// </summary>
		protected string m_operatorDialogConfigID;

		/// <summary>
		/// Name of the template to be used to parse templates
		/// </summary>
		protected string m_templateItemName;

		/// <summary>
		/// Name of the template to be used to parse header
		/// </summary>
		protected string m_templateHeaderName;


		/// <summary>
		/// Constructor.
		/// </summary>
		public GUIDOListDialog()
		{
			m_classID = g_classID;
		}


		/// <summary>
		/// Creator method. Returns new instance of this class
		/// </summary>
		/// <returns>Instance of this class</returns>
		public static ICreatable CreateInstance()
		{
			return new GUIDOListDialog();
		}


		/// <summary>
		/// Initializes singelton delegator reference and registers it with class factory.
		/// </summary>	
		public static void Register()
		{
			g_creator = new CreateObjectDelegator(GUIDOListDialog.CreateInstance);
			ClassFactory.GetFactory().Register(g_classID, g_creator);
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

			m_addButton = new GUIButton();			
			m_addButton.Initialize("addButton", request, input, output, session, response);
			m_addButton.Label = CodeStrings.BUTTON_ADD;
			m_addButton.AddObserver(EVENT_CLICK, this);
			AddActionBarElement(m_addButton);

			m_cancelButton = new GUIButton();
			m_cancelButton.Initialize("cancelButton", request, input, output, session, response);
			m_cancelButton.Label = CodeStrings.BUTTON_CANCEL;
			m_cancelButton.AddObserver(EVENT_CLICK, this);
			AddActionBarElement(m_cancelButton);

			m_editButton = new GUIButton();
			m_editButton.Initialize("editButton", request, input, output, session, response);
			m_editButton.Label = CodeStrings.BUTTON_EDIT;
			m_editButton.AddObserver(EVENT_CLICK, this);
			AddChildElement(m_editButton);

			m_viewButton = new GUIButton();
			m_viewButton.Initialize("viewButton", request, input, output, session, response);
			m_viewButton.Label = CodeStrings.BUTTON_VIEW;
			m_viewButton.AddObserver(EVENT_CLICK, this);
			AddChildElement(m_viewButton);


			m_deleteButton = new GUIButton();
			m_deleteButton.Initialize("deleteButton", request, input, output, session, response);
			m_deleteButton.Label = CodeStrings.BUTTON_DELETE;
			m_deleteButton.AddObserver(EVENT_CLICK, this);
			AddChildElement(m_deleteButton);

			using (Transaction transaction = new Transaction(session)) 
			{
				string configID = input[VAR_CONFIG_ID];
				if (configID == null || configID == "")
					throw new EInvalidParameter(this, VAR_CONFIG_ID);

				ConfigManager configManager = ConfigManager.GetManager();
				Config config = configManager.GetConfig(configID, transaction);

				m_title = config[CONFIG_TITLE];

				m_dataObjectConfigID = config[CONFIG_OBJECT_CONFIG_ID];
				if (m_dataObjectConfigID == "")
					throw new EInvalidParameter(this, CONFIG_OBJECT_CONFIG_ID);

				m_dataObjectClassID = config[CONFIG_OBJECT_CLASS];
				if (m_dataObjectClassID == "")
					throw new EInvalidParameter(this, CONFIG_OBJECT_CLASS);

				m_dataObjectConfig = configManager.GetConfig(m_dataObjectConfigID, transaction);

				m_operatorDialogClassID = config[CONFIG_OPERATOR_DIALOG];
				if (m_operatorDialogClassID == "")
					throw new EInvalidParameter(this, CONFIG_OPERATOR_DIALOG);

				m_operatorDialogConfigID = config[CONFIG_OPERATOR_DIALOG_CONFIG];
				if (m_operatorDialogConfigID == "")
					throw new EInvalidParameter(this, CONFIG_OPERATOR_DIALOG_CONFIG);

				m_templateItemName = config[CONFIG_TEMLATE_ITEM];
				if (m_templateItemName == "")
					throw new EInvalidParameter(this, CONFIG_TEMLATE_ITEM);

				m_templateHeaderName = config[CONFIG_TEMLATE_HEADER];
				if (m_templateHeaderName == "")
					throw new EInvalidParameter(this, CONFIG_TEMLATE_HEADER);

				string securityLevel = config[CONFIG_SECURITY_LEVEL];
				string securityLevelGuard = config[CONFIG_SECURITY_LEVEL_GUARD];
				if (securityLevel != "" && securityLevelGuard != "")
				{
					SecurityManager securityManager = SecurityManager.GetManager();
					ILevelSecurityGuard guard  = SecurityManager.GetManager().GetLevelSecurityGuard(securityLevelGuard, transaction);
					guard.Authorize(session, securityLevel);
				}
			}
		}


		/// <summary>
		/// Renders main window content.
		/// </summary>
		/// <param name="output">Output writer</param>
		/// <param name="transaction">Transaction</param>
		override public void RenderContent(HtmlTextWriter output, Transaction transaction)
		{
			// Determine schema being listed:
			string schema = m_dataObjectConfig[DataObject.CONFIG_TABLE];

			Parser parser = new Parser(new FileTemplate(m_templateHeaderName, m_session), transaction);	
			parser.AddInput(m_variables);
			parser.AddInput(m_input);
			parser.Parse(output, GUIConstants.VAR_STOP_TAG);

			// List all objects
			// TODO: Need to implement features for list dialog:
			//  - selecting only certain columns
			//  - specifying search criteria for the where clause
			//  - ordering			
			DataReader reader = new DataReader(schema);
			reader += DataOperator.SQL_WILDCARD;
			reader.Load(transaction); 
			while (reader.Read())
			{
				Parser itemParser = new Parser(new FileTemplate(m_templateItemName, m_session), transaction);
				itemParser.AddInput(reader);
				itemParser.Parse(output);
			}

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
				if (elementID == m_addButton.GetID())
					OnAdd(eventArguments);
				else if (elementID == m_cancelButton.GetID())
					OnCancel(eventArguments);
				else if (elementID == m_editButton.GetID())
					OnEdit(eventArguments);
				else if (elementID == m_viewButton.GetID())
					OnView(eventArguments);
				else if (elementID == m_deleteButton.GetID())
					OnDelete(eventArguments);
			}
		}


		/// <summary>
		/// Event handler for on-click on the Edit button.
		/// </summary>
		virtual public void OnEdit(ArrayList eventArguments)
		{
			if (eventArguments.Count != 1)
				throw new EInvalidParameter(this, "first event argument");

			DataValue dataObjectID = (DataValue) eventArguments[0];
			if (dataObjectID == "")
				throw new EInvalidParameter(this, "first event argument");

			Close(false);

			// TODO: m_request contains old URL .. need to do something about that so that history does not get screwed up
			NameValueCollection input = new NameValueCollection();
			input[GUIDODialog.VAR_DATA_OBJECT_ID] = dataObjectID;
			input[GUIDODialog.VAR_CONFIG_ID] = m_operatorDialogConfigID;			
			GUIFramework.OpenWindow(m_operatorDialogClassID, m_operatorDialogClassID, m_request, new ArrayList(), input, m_session, m_output, m_response);
		}


		/// <summary>
		/// Event handler for on-click on the View button.
		/// </summary>
		virtual public void OnView(ArrayList eventArguments)
		{
			if (eventArguments.Count != 1)
				throw new EInvalidParameter(this, "first event argument");

			DataValue dataObjectID = (DataValue) eventArguments[0];
			if (dataObjectID == "")
				throw new EInvalidParameter(this, "first event argument");

			Close(false);

			// TODO: m_request contains old URL .. need to do something about that so that history does not get screwed up
			NameValueCollection input = new NameValueCollection();
			input[GUIDODialog.VAR_DATA_OBJECT_ID] = dataObjectID;
			input[GUIDODialog.VAR_CONFIG_ID] = m_operatorDialogConfigID;			
			input[GUIDODialog.VAR_GUI_FLAG] = new DataValue((long) Def.UIFLAG_READONLY);
			GUIFramework.OpenWindow(m_operatorDialogClassID, m_operatorDialogClassID, m_request, new ArrayList(), input, m_session, m_output, m_response);
		}


		/// <summary>
		/// Event handler for on-click on the Delete button.
		/// </summary>
		virtual public void OnDelete(ArrayList eventArguments)
		{
			if (eventArguments.Count != 1)
				throw new EInvalidParameter(this, "first event argument");

			DataValue dataObjectID = (DataValue) eventArguments[0];
			if (dataObjectID == "")
				throw new EInvalidParameter(this, "first event argument");

			using (Transaction transaction = new Transaction(m_session)) 
			{			
				IDataObject dataObject = (IDataObject) ClassFactory.GetFactory().Produce(m_dataObjectClassID);
				dataObject.Initilize(dataObjectID, m_dataObjectConfigID, transaction);
				dataObject.Delete(transaction);
				transaction.Commit();
			}

			Close(true);
		}


		/// <summary>
		/// Event handler for on-click on the Delete button.
		/// </summary>
		virtual public void OnAdd(ArrayList eventArguments)
		{
			Close(false);

			// TODO: m_request contains old URL .. need to do something about that so that history does not get screwed up
			NameValueCollection input = new NameValueCollection();
			input[GUIDODialog.VAR_CONFIG_ID] = m_operatorDialogConfigID;			
			GUIFramework.OpenWindow(m_operatorDialogClassID, m_operatorDialogClassID, m_request, new ArrayList(), input, m_session, m_output, m_response);
		}


		/// <summary>
		/// Event handler for on-click on the Cancel button.
		/// </summary>
		virtual public void OnCancel(ArrayList eventArguments)
		{
			Close(true);
		}

	}
}

