using System;
using System.Web;
using System.Web.UI;
using System.Collections.Specialized;
using Engine;
using WebAPI;

namespace BaseWebModules
{
	/// <summary>
	/// Web Service plugin class responsible for loading various components (web functions, template functions, etc) 
	/// and registering any other classes (e.g. controllers, custom ICreatable objects, etc).
	/// </summary>
	public class WebPlugin
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public WebPlugin() 
		{
		}
		

		/// <summary>
		/// Plugin loader function.
		/// </summary>
		public void Load() 
		{
			WebFunctionRegistry registry = WebFunctionRegistry.GetRegistry();

			// Initilize modules:
			Workspace.Initialize(registry);			
			GUIFramework.Initialize(registry);

			// Register GUI classes:
			ProfileDlg.Register();			

			// Register standard web functions:
			ListCalendar.Register();
			ListTasks.Register();
			ListSecurityLevels.Register();
		}
	}
}
