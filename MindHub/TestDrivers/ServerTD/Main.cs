using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Engine;

namespace ServerTD
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			// Create remote channel
			BinaryClientFormatterSinkProvider clnt = new BinaryClientFormatterSinkProvider();
			TcpClientChannel chan = new TcpClientChannel(Def.ENGINE_NAME + "Channel", clnt);
			ChannelServices.RegisterChannel(chan);

			try
			{
				// Initialize managers:
				LogManager logMgr = LogManager.GetManager();
				logMgr.SetFileName("ServerTD.log");
				logMgr.Initialize();

				LockManager	lockMgr = LockManager.GetManager();
				lockMgr.Initialize();

				ServerConnectionManager serverConnectionMgr = ServerConnectionManager.GetManager();
				serverConnectionMgr.Initialize();

				EnvironmentManager environmentMgr = EnvironmentManager.GetManager();
				environmentMgr.Initialize();

				SecurityManager securityMgr = SecurityManager.GetManager();
				securityMgr.Initialize();

				SchemaManager schemaMgr = SchemaManager.GetManager();
				schemaMgr.Initialize();

				ConfigManager configMgr = ConfigManager.GetManager();
				configMgr.Initialize();

	
				// Registration and initialization of objects below might require remote calls (need for transactions):
				DataObject.Register();
				LevelSecurityGuard.Register();
				ObjectActionSecurityGuard.Register();	
			


				// When request comes in to view data object through a view using a controller:				
				TestController testContorller = new TestController();
				testContorller.Open();


				// When web application is about to terminate (e.g. through stopping IIS):
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
