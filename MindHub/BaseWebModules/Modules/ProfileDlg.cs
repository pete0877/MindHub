using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using Engine;

namespace BaseWebModules
{
	/// <summary>
	/// Dialog for display and update of user profile informtion.
	/// </summary>
	public class ProfileDlg : GUIDialog
	{
		/// <summary>
		/// Class ID.
		/// </summary>
		protected static string g_classID = "BaseWebModules.ProfileDlg";	

		/// <summary>
		/// Singelton delegator instance reference.
		/// </summary>
		public static CreateObjectDelegator g_creator;

		/// <summary>
		/// Contente template.
		/// </summary>
		static public string TEMPLATE_CONTENT			= "profilecontent.html";

		/// <summary>
		/// User data object being viewed / updated.
		/// </summary>
		protected UserDataObject m_user;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ProfileDlg()
		{
			m_classID = g_classID;
		}


		/// <summary>
		/// Creator method. Returns new instance of this class
		/// </summary>
		/// <returns>Instance of this class</returns>
		public static ICreatable CreateInstance()
		{
			return new ProfileDlg();
		}


		/// <summary>
		/// Initializes singelton delegator reference and registers it with class factory.
		/// </summary>	
		public static void Register()
		{
			g_creator = new CreateObjectDelegator(ProfileDlg.CreateInstance);
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
			
			m_contentTemplate = TEMPLATE_CONTENT;
			m_title = CodeStrings.TITLE_USERPROFILE;
		}


		/// <summary>
		/// Implementation for the open command.
		/// </summary>
		override public void Open(ArrayList eventArguments)
		{
			base.Open(eventArguments);

			using (Transaction transaction = new Transaction(m_session)) 
			{
				UserDataObject user = new UserDataObject();
				user.Initilize(m_session.UserID, null, transaction);
				user.Load(transaction);
				m_user = user;
				m_user.PushData(m_variables, transaction);				
			}
		}


		/// <summary>
		/// Event handler for OnClick event of the Ok button
		/// </summary>
		override public void OnOk(ArrayList eventArguments)
		{
			using (Transaction transaction = new Transaction(m_session)) 
			{
				UserDataObject user = new UserDataObject();
				user.Initilize(m_session.UserID, null, transaction);
				m_user = user;
				m_user.PullData(m_input, transaction);				
				m_user.Save(transaction);
				transaction.Commit();
			}

			DisplayResults(null);
		}
	}
}
