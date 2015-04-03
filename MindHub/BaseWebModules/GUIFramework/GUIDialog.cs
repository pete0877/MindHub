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
	public class GUIDialog : GUIWindow
	{
		/// <summary>
		/// Default template for displaying sucessful operatoin message after Ok is clicked and operation suceeds.
		/// </summary>
		static public string TEMPLATE_OPERATION_COMPLETE			= "gui/guioperationcomplete.html";

		/// <summary>
		/// 'Ok' button
		/// </summary>
		GUIButton m_okButton;

		/// <summary>
		/// 'Cancel' button
		/// </summary>
		GUIButton m_cancelButton;

		
		/// <summary>
		/// Constructor.
		/// </summary>
		public GUIDialog()
		{
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

			m_okButton = new GUIButton();			
			m_okButton.Initialize("okButton", request, input, output, session, response);
			m_okButton.Label = CodeStrings.BUTTON_OK;
			m_okButton.AddObserver(EVENT_CLICK, this);
			AddActionBarElement(m_okButton);

			m_cancelButton = new GUIButton();
			m_cancelButton.Initialize("cancelButton", request, input, output, session, response);
			m_cancelButton.Label = CodeStrings.BUTTON_CANCEL;
			m_cancelButton.AddObserver(EVENT_CLICK, this);
			AddActionBarElement(m_cancelButton);
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
				if (elementID == m_okButton.GetID())
					OnOk(eventArguments);
				else if (elementID == m_cancelButton.GetID())
					OnCancel(eventArguments);
			}
		}


		/// <summary>
		/// Event handler for on-click on the Ok button.
		/// </summary>
		virtual public void OnOk(ArrayList eventArguments)
		{
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
		/// <param name="resultsPage">Custom template name under /html/ to be displayed. If resultsPage is null or empty, TEMPLATE_OPERATION_COMPLETE will be used.</param>
		virtual public void DisplayResults(string resultsPage)
		{	
			if (resultsPage == null || resultsPage.Length == 0)
				m_response.Redirect(m_session.GetUIPathWeb() + "/html/" + TEMPLATE_OPERATION_COMPLETE);
			else
				m_response.Redirect(m_session.GetUIPathWeb() + "/html/" + resultsPage);
		}
	}
}
