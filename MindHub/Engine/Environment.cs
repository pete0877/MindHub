using System;

namespace Engine
{
	/// <summary>
	/// Responsible for ecapsulating application-environment parameters such as DB connection specs, UI paths, etc.
	/// </summary>
	[Serializable]
	public class Environment 
	{
		/// <summary>
		/// Environment ID
		/// </summary>
		public string ID;

		/// <summary>
		/// DNS connection string used by ADO.NET to establish connection
		/// </summary>
		public string DNS;

		/// <summary>
		/// Path to UI temaplates in the operating system (e.g. "D:\MindHub\Dev\WebRoot\Fake\"). Should end with slash.
		/// </summary>
		public string UIPathOS;

		/// <summary>
		/// Path to UI temaplates on the web server (e.g. "/Fake/"). Should end with slash.
		/// </summary>
		public string UIPathWeb;

		/// <summary>
		/// DB connection type of the environment's database
		/// </summary>
		public DBConnection.DBType DBType;

		/// <summary>
		/// Constructor. Initializes the environment properties.
		/// </summary>
		/// <param name="envID">Environment ID</param>
		/// <param name="envDBType">DB connection type</param>
		/// <param name="envDNS">DB connection DNS for ADO.NET</param>
		/// <param name="envUIPath">Path to UI templates</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		public Environment(string environmentID, DBConnection.DBType envDBType, string environmentDNS, string environmentUIPathOS, string environmentUIPathWeb)
		{
			if (environmentID == null)
				throw new EInvalidParameter(this, "ID");

			if (environmentDNS == null)
				throw new EInvalidParameter(this, "environmentDNS");

			if (environmentUIPathOS == null)
				throw new EInvalidParameter(this, "environmentUIPathOS");

			if (environmentUIPathWeb == null)
				throw new EInvalidParameter(this, "environmentUIPathWeb");

			ID = string.Copy(environmentID);
			DBType = envDBType;
			DNS = string.Copy(environmentDNS);
			UIPathOS = string.Copy(environmentUIPathOS);
			UIPathWeb = string.Copy(environmentUIPathWeb);
		}


		/// <summary>
		/// Constructor. Initializes the environment properties based on another environment.
		/// </summary>
		/// <param name="environment">Copy-from environment instance</param>
		/// <exception cref="EInvalidParameter">If environment is null</exception>
		public Environment(Environment environment)
		{
			if (environment == null)
				throw new EInvalidParameter(this, "environment");

			ID = string.Copy(environment.ID);
			DBType = environment.DBType;
			DNS = string.Copy(environment.DNS);
			UIPathOS = string.Copy(environment.UIPathOS);
			UIPathWeb = string.Copy(environment.UIPathWeb);
		}
	}
}
