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
	/// Base GUI element class. All GUI classes derive from this class. 
	/// GUIFramework.GUI() web function utilizes this class.
	/// GUI elements have following responsibilities:
	/// 1. Know how to render itself
	/// 2. Know to process various actions (e.g. GUI windows know how to process Open command, base GUIElement knows how to process generated events).
	/// </summary>
	public class GUIElement : ICreatable
	{
		/// <summary>
		/// Standard on-click event name.
		/// </summary>
		static public string EVENT_CLICK				= "click";

		/// <summary>
		/// Variable for storing element ID.
		/// </summary>
		public const string VAR_GUI_ELEMENT_ID			= "guielementid";

		/// <summary>
		/// Variable for rendering flags (e.g. readonly). See Def.UIFLAG_*
		/// </summary>
		public const string VAR_GUI_FLAG				= "guif";

		/// <summary>
		/// Command name for processing invoked events.
		/// </summary>
		public const string COMMAND_HANDLEEVENT			= "handleevent";

		/// <summary>
		/// This instance's element ID.
		/// </summary>
		protected string m_ID;

		/// <summary>
		/// This instance's class ID.
		/// </summary>
		protected string m_classID;

		/// <summary>
		/// Web input variable collection. 
		/// Cached from GUIFramework.GUI web function call.
		/// </summary>
		protected NameValueCollection m_input;

		/// <summary>
		/// User session variable.
		/// Cached from GUIFramework.GUI web function call.
		/// </summary>
		protected Engine.Session m_session;

		/// <summary>
		/// HTTP response object.
		/// Cached from GUIFramework.GUI web function call.
		/// </summary>
		protected HttpResponse m_response;

		/// <summary>
		/// HTTP output target object.
		/// </summary>
		protected HtmlTextWriter m_output;
        
		/// <summary>
		/// HTTP request object.
		/// Cached from GUIFramework.GUI web function call.
		/// </summary>
		protected HttpRequest m_request;

		/// <summary>
		/// List of children GUI elements contained and managed by this element.
		/// </summary>
		protected ArrayList m_childElements;

		/// <summary>
		/// Hash of event observers. Each event that this element produces has array of observers.
		/// Registered observers are notified when this instance produces particuar event.
		/// </summary>
		protected Hashtable m_eventObserversList;

		/// <summary>
		/// Main content template name. Derived classes need to set this variable if they want to utilize Render() method implemented in this class.
		/// </summary>
		protected string m_contentTemplate;

		/// <summary>
		/// Set of standard variables with which all templates which compose this element should be parsed with.
		/// (e.g. contains m_ID (element's ID) in VAR_GUI_ELEMENT_ID variable).
		/// </summary>
		protected NameValueCollection m_variables;

		/// <summary>
		/// Set of data objects that can be used when parsing element's content template.
		/// </summary>
		protected ArrayList m_inputDataObjects;

		/// <summary>
		/// Flag for if this element should be rendered at all. This flag can be used to disable (completely) GUI element.
		/// </summary>
		protected bool m_renderable;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GUIElement()
		{
			m_childElements = new ArrayList();
			m_eventObserversList = new Hashtable();
			m_variables = new NameValueCollection();
			m_inputDataObjects = new ArrayList();
			m_renderable = true;
		}


		/// <summary>
		/// Returns the class ID of this GUI element.
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <returns></returns>
		virtual public string GetClassID()
		{
			return m_classID;
		}


		/// <summary>
		/// Returns GUI element ID.
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <returns></returns>
		public string GetID()
		{
			return m_ID;
		}


		/// <summary>
		/// Initializes instance of this GUIElement.
		/// Caches away all given parameters as local members.
		/// Initializes m_variables with base variables.
		/// </summary>
		/// <param name="elementID">Element ID</param>
		/// <param name="input">Web input</param>
		/// <param name="session">User session</param>
		/// <param name="response">Response object</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		virtual public void Initialize(string elementID, HttpRequest request, NameValueCollection input, HtmlTextWriter output, Engine.Session session, HttpResponse response)
		{
			if (elementID == null || elementID.Length == 0)
				throw new EInvalidParameter(this, "elementID");

			if (input == null)
				throw new EInvalidParameter(this, "input");

			if (output == null)
				throw new EInvalidParameter(this, "output");

			if (session == null)
				throw new EInvalidParameter(this, "session");

			if (response == null)
				throw new EInvalidParameter(this, "response");			

			if (request == null)
				throw new EInvalidParameter(this, "request");		

			m_ID = elementID;
			m_input = input;
			m_output = output;
			m_session = session;
			m_response = response;		
			m_request = request;
	
			m_variables[VAR_GUI_ELEMENT_ID] = elementID;
		}		


		/// <summary>
		/// Adds reference to contained child GUI element to list of known child elements.
		/// Perfors set (add) by reference (unsafe).
		/// </summary>
		/// <param name="element">Child GUI element</param>
		/// <exception cref="EInvalidParameter">If element parameter is null</exception>
		protected void AddChildElement(GUIElement element)
		{
			if (element == null)
				throw new EInvalidParameter(this, "element");

			m_childElements.Add(element);
		}


		/// <summary>
		/// Depth-first traversal routine for finding element of given ID among the children elements as well as self.
		/// Perfors get by reference (unsafe). 
		/// </summary>
		/// <param name="elementID">Searched element ID</param>
		/// <returns>Found element or null if element is not found</returns>
		/// <exception cref="EInvalidParameter">If elementID parameter is null</exception>
		public GUIElement FindElement(string elementID)
		{
			if (elementID == null)
				throw new EInvalidParameter(this, "elementID");

			if (m_ID == elementID)
				return this;

			IEnumerator itr = m_childElements.GetEnumerator();
			while (itr.MoveNext())
			{
				GUIElement childElement = (GUIElement) itr.Current;
				GUIElement foundElement = childElement.FindElement(elementID);
				if (foundElement != null)
					return foundElement;
			}

			return null;
		}


		/// <summary>
		/// Adds event observer.
		/// Perfors set (add) by reference (unsafe).
		/// </summary>
		/// <param name="eventName">Event name</param>
		/// <param name="observer">GUI element which wants to observe given event on this instance</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public void AddObserver(string eventName, GUIElement observer)
		{
			if (eventName == null)
				throw new EInvalidParameter(this, "eventName");

			if (observer == null)
				throw new EInvalidParameter(this, "observer");

			ArrayList observers = (ArrayList) m_eventObserversList[eventName];
			if (observers == null)
			{
				observers = new ArrayList();
				m_eventObserversList[eventName] = observers;
			}

			observers.Add(observer);
		}


		/// <summary>
		/// Processes command to be executed on this GUI element.
		/// </summary>
		/// <param name="command">Command name</param>
		/// <param name="arguments">List of arguments</param>
		/// <returns>Flag for if the request has been processed. If this element does not know how to process given command, flase is returned. Sucessful processing causes this method to return true.</returns>
		/// <exception cref="EInvalidParameter">If any of the parameters are null or if processing of the command fails due to invalid command arguments.</exception>
		virtual public bool ProcessCommand(string command, ArrayList arguments)
		{
			if (command == null)
				throw new EInvalidParameter(this, "command");

			if (arguments == null)
				throw new EInvalidParameter(this, "arguments");

			if (command == COMMAND_HANDLEEVENT)
			{
				ProcessEventCommand(arguments);
				return true;
			}
			return false;
		}


		/// <summary>
		/// Command implementation for COMMAND_HANDLEEVENT. Searches for the event producer among its children and self to call ProcessEvent method.
		/// At least two arguments are expected for this command:
		/// 1. event sender (element ID of the sender)
		/// 2. event name (e.g. 'click')
		/// This method will iterate through all contained sub-elements to find the 
		/// </summary>
		/// <param name="arguments">List of arguments</param>
		/// <exception cref="EInvalidParameter">If processing of the command fails due to invalid command arguments or if the sender element is not found.</exception>
		protected void ProcessEventCommand(ArrayList arguments)
		{
			if (arguments.Count < 2)
				throw new EInvalidParameter(this, "arguments", "", "Processing [processevent] command requires provision of at least two arguments: sender's ID and event name"); 

			// First argument is sender ID
			// Second argument is event name
			// Rest of the arguments are event parameters (event arguments)
			DataValue senderID = (DataValue) arguments[0];
			DataValue eventName = (DataValue) arguments[1];
			// Rewrite list of event arguments so that the sender ID and event name are not included:
			ArrayList eventArguments = new ArrayList();
			for (int n = 2; n < arguments.Count; n++)
				eventArguments.Add(arguments[n]);

			// If the event was rissen by this instance of the class, we need to notify observers. 
			// Else, seek the correct element from the list of child elemetns and forward the event 
			// to the one that matches the ID:
			GUIElement sender = FindElement(senderID);
			if (sender == null)
				throw new EInvalidParameter(this, "senderID", senderID, "Event sender not found among self and children of: " + m_classID + "." + m_ID); 

			sender.ProcessEvent(eventName, eventArguments);
		}


		/// <summary>
		/// Event processing method. In this base class, this method simply notifies all event observers.
		/// </summary>
		/// <param name="eventName">Event name</param>
		/// <param name="eventArguments">Event arguments (could be empty)</param>
		virtual protected void ProcessEvent(string eventName, ArrayList eventArguments)
		{
			NotifyObservers(eventName, eventArguments);
		}


		/// <summary>
		/// Sends notification to all event observers.
		/// </summary>
		/// <param name="eventName">Event name</param>
		/// <param name="eventArguments">Event arguments (could be empty)</param>
		protected void NotifyObservers(string eventName, ArrayList eventArguments)
		{
			ArrayList observers = (ArrayList) m_eventObserversList[eventName];
			if (observers == null)
				return;

			IEnumerator itr = observers.GetEnumerator();
			while (itr.MoveNext())
			{
				GUIElement observer = (GUIElement) itr.Current;
				observer.ObserveEvent(m_ID, eventName, eventArguments);
			}
		}


		/// <summary>
		/// Main event-observing method that gets called by NotifyObservers when event is produced on observed element.
		/// </summary>
		/// <param name="elementID">Sender element ID</param>
		/// <param name="eventName">Event name</param>
		/// <param name="eventArguments">Event arguments (could be empty)</param>
		virtual public void ObserveEvent(string elementID, string eventName, ArrayList eventArguments)
		{		
		}


		/// <summary>
		/// Returns flag if this element can be rendered.
		/// </summary>
		/// <returns>Flag m_renderable</returns>
		public bool IsRendarable()
		{
			return m_renderable;
		}


		/// <summary>
		/// Responsible for rendering of this GUI element.
		/// If m_contentTemplate is set and not empty, it will be parsed.
		/// </summary>
		/// <param name="output">Output writer</param>
		/// <param name="transaction">Transaction</param>
		virtual public void Render(Transaction transaction)
		{
			if (!IsRendarable())
				return;

			if (m_contentTemplate != null && m_contentTemplate.Length > 0)
			{
				Parser parser = new Parser(new FileTemplate(m_contentTemplate, m_session), transaction);
				parser.AddInput(m_variables);
				parser.AddInput(m_input);
				IEnumerator dataObjects = m_inputDataObjects.GetEnumerator();
				while (dataObjects.MoveNext())
				{
					IDataObject dataObject = (IDataObject) dataObjects.Current;
					parser.AddInput(dataObject);
				}
				
				parser.Parse(m_output);
			}	
		}
	}
}
