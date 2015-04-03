using Engine;

using System;
using System.Collections;

namespace EngineTD
{
	/// <summary>
	/// Directs tests on the Engine project
	/// </summary>
	class MainTD
	{
		[STAThread]
		static void Main(string[] args)
		{
			try
			{
				// When application server starts:

				// Initialize managers:
				LogManager logMgr = LogManager.GetManager();
				logMgr.Initialize("app.log");

				LockManager	lockMgr = LockManager.GetManager();
				lockMgr.Initialize();

				EnvironmentManager environmentMgr = EnvironmentManager.GetManager();
				environmentMgr.Initialize(Engine.Location.APP_SERVER);

				SecurityManager securityMgr = SecurityManager.GetManager();
				securityMgr.Initialize();

				DBConnectionManager dbConnectionMgr = DBConnectionManager.GetManager();
				dbConnectionMgr.Initialize();

				SchemaManager schemaMgr = SchemaManager.GetManager();
				schemaMgr.Initialize();

				ConfigManager configMgr = ConfigManager.GetManager();
				configMgr.Initialize();

			
				// Right before DLLs are loaded:
				DataObject.Register();
				LevelSecurityGuard.Register();
				ObjectActionSecurityGuard.Register();				


				// When request comes in to view data object through a view using a controller:				
				TestController testContorller = new TestController();
				testContorller.Open();

				logMgr.UnInitialize();
	
			} 
			catch (Exception ex)
			{
				Console.WriteLine("WOOPS. EXPECTION: \n           " + ex.ToString());
			}
			string confirm = Console.ReadLine();

			
		}
	}
}

