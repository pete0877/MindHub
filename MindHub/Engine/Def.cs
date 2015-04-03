using System;

namespace Engine
{
	/// <summary>
	/// Location type definition. This enumeration defines execution point (web server or application server).
	/// </summary>
	public enum Location { APP_SERVER, WEB_SERVER };

	/// <summary>
	/// Application class. Defines system-wide constats and configuration.
	/// </summary>
	public class Def
	{
		/// <summary>
		/// ID of the TConfiguration record storing general application configuration (for whole environment)
		/// </summary>
		static public string CONFIGID_ENVIRONMENT	= "0";

		/// <summary>
		/// Shortcut name for the engine (used when creating remote references)
		/// </summary>
		static public string ENGINE_NAME		= "MindHub";	
	
		/// <summary>
		/// UI Flag corresponding to no special alternations in UI style / behavior
		/// </summary>
		static public int UIFLAG_NONE				= 0;

		/// <summary>
		/// UI Flag corresponding to UI element being read-only
		/// </summary>
		static public int UIFLAG_READONLY			= 1;
	}
}
