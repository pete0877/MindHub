using System.Web;
using System.Web.UI;
using System;
using Engine;

namespace WebAPI
{
	/// <summary>
	/// Keeps track of URLs visited by the user using session.
	/// Allows compotents to get history of URLs and hence allowing web functions to return to previously visited screens.
	/// </summary>
	public class URLHistory
	{
		public static string URL_HISTORY_ITEM_PREFIX		= "urlhistoryitem";	
		public static string URL_HISTORY_POSITION			= "urlhistoryposition";	
		public static int URL_HISTORY_SIZE					= 10;	
		public static string URL_DEFAULT_REDIRECT			= "/webapi/process.aspx?vf=displaytoday";			

		/// <summary>
		/// HTTP response object.
		/// </summary>
		protected HttpResponse m_response;

		/// <summary>
		/// Session object.
		/// </summary>
		protected Engine.Session m_session;

		/// <summary>
		/// Current position within the URL history. Goes from 0 to URL_HISTORY_SIZE - 1
		/// </summary>
		protected int m_currentPosition;

		/// <summary>
		/// Default constructor. Sets the current history item position to 0.
		/// </summary>
		/// <param name="session">User session</param>
		public URLHistory(Engine.Session session, HttpResponse response)
		{	
			m_session = session;
			m_response = response;

			m_currentPosition = 0;
			if (m_session[URL_HISTORY_POSITION] != null)
				m_currentPosition = (int) session[URL_HISTORY_POSITION];
		}


		/// <summary>
		/// Records visit to given URL. If the url is same as the last one recorder, then nothing is done.
		/// </summary>
		/// <param name="url">Return URL</param>
		public void RecordVisit(string url)
		{		
			// Check if we are just refreshing same URL:
			int peekPosition = m_currentPosition - 1;
			if (peekPosition < 0)
				peekPosition = URL_HISTORY_SIZE - 1;

			if ((string) m_session[URL_HISTORY_ITEM_PREFIX + peekPosition.ToString()] == url)
				return;

			// Else, new URL came into play, record it:
			m_session[URL_HISTORY_ITEM_PREFIX + m_currentPosition.ToString()] = url;
			m_currentPosition++;
			m_currentPosition = m_currentPosition % URL_HISTORY_SIZE;
			m_session[URL_HISTORY_POSITION] = m_currentPosition;
		}


		/// <summary>
		/// Redirects to second previously recorded URL. Skips over the last (most recent) URL assuming that it was registered
		/// by the component which is calling this function.
		/// </summary>
		public void Return()
		{
			// Move the current position back in the history.
			m_currentPosition--;
			if (m_currentPosition < 0)
				m_currentPosition = URL_HISTORY_SIZE - 1;			

			int returnURLPosition = m_currentPosition - 1;
			if (returnURLPosition < 0)
				returnURLPosition = URL_HISTORY_SIZE - 1;

			string url = (string) m_session[URL_HISTORY_ITEM_PREFIX + returnURLPosition.ToString()];
			if (url == null || url.Length == 0)
				url = URL_DEFAULT_REDIRECT;
			
			m_response.Redirect(url);				
		}


		/// <summary>
		/// Redirects to last previously recorded URL. 
		/// </summary>
		public void Refresh()
		{
			// Move the current position back in the history.
			int returnURLPosition = m_currentPosition - 1;
			if (returnURLPosition < 0)
				returnURLPosition = URL_HISTORY_SIZE - 1;

			string url = (string) m_session[URL_HISTORY_ITEM_PREFIX + returnURLPosition.ToString()];
			if (url == null || url.Length == 0)
				url = URL_DEFAULT_REDIRECT;
			
			m_response.Redirect(url);				
		}
	}
}
