using System;
using System.Collections;
using Engine;


namespace ServerTD
{
	/// <summary>
	/// Summary description for TestController.
	/// </summary>
	public class TestController
	{
		public const string CONFIG_DATAOBJECT					= "dataobject";
		public const string CONFIG_DATAOBJECT_CONFIG			= "dataobjectconfig";
		public const string CONFIG_LEVEL_SECURITY_GUARD			= "levelsecguard";
		public const string CONFIG_LEVEL_SECURITY				= "levelsec";
		public const string CONFIG_ACTION_SECURITY_GUARD		= "actionguard";

		public void Open()
		{
			// Normally things like session, web request variables and other information would be
			// coming to this method. This is just a simplistic test:

			// Data from request:
			// string objectID = "a";				// HARDCODED for test
			string processConfigID = "2";		// HARDCODED for test
			// string action = "open";				// HARDCODED for test

			// Data from session
			Engine.Session session = new Session("fake", "s1");
			session.UserID = "1";					// HARDCODED for test
			session.AddToGroup("admins");		// HARDCODED for test
			session.AddToGroup("public");		// HARDCODED for test

			// HARDCODED SECION ENDS

			using (Transaction transaction = new Transaction(session))
			{
				SecurityManager.GetManager().CacheSecurity(session, transaction);

				// Get the process configuration based on processConfigID
				Config config = ConfigManager.GetManager().GetConfig(processConfigID, transaction);

				// From the process configuration we determine class ID of the 
				// data object and the configuration ID:
				string dataObjectClassID = config[CONFIG_DATAOBJECT];
				string dataObjectConfigID = config[CONFIG_DATAOBJECT_CONFIG];				
				string levelSecurityGuardID = config[CONFIG_LEVEL_SECURITY_GUARD];
				string levelSecurity = config[CONFIG_LEVEL_SECURITY];
				string actionSecurityGuardID = config[CONFIG_ACTION_SECURITY_GUARD];

				if (dataObjectClassID.Length == 0)
					throw new EInvalidParameter(this, CONFIG_DATAOBJECT, dataObjectClassID, "Process configuration " + processConfigID + " contains empty data object class id");
					
				if (dataObjectConfigID.Length == 0)
					throw new EInvalidParameter(this, CONFIG_DATAOBJECT_CONFIG, dataObjectConfigID, "Process configuration " + processConfigID + " contains empty data object config id");

				if (levelSecurityGuardID.Length > 0 && levelSecurity.Length > 0)
				{
					ILevelSecurityGuard guard  = SecurityManager.GetManager().GetLevelSecurityGuard(levelSecurityGuardID, transaction);
					guard.Authorize(session, levelSecurity);
				}

			}

			/*

					
			// Load the test object 
			DateTime start = DateTime.Now;
			int lObjectsToRead = 100;
			for (int a = 0; a < lObjectsToRead; a++)
			{
				using (Transaction tloop = new Transaction(session))
				{
					IDataObject dataObjectLoaded = (IDataObject) ClassFactory.GetFactory().Produce(dataObjectClassID);
					dataObjectLoaded.Initilize(objectID, dataObjectConfigID, tloop);
					dataObjectLoaded.Load(tloop);
					// Console.WriteLine("got : " + a);
				}				
			}
			DateTime end = DateTime.Now;

			long tix = (end.Ticks - start.Ticks) / 10000;			
			Console.WriteLine("objects read : " + lObjectsToRead.ToString());
			Console.WriteLine("duration     : " + tix.ToString());
	
			*/

			/*
			using (Transaction tloop = new Transaction(session))
			{
				IDataObject dataObjectLoaded = (IDataObject) ClassFactory.GetFactory().Produce(dataObjectClassID);
				dataObjectLoaded.Initilize(objectID, dataObjectConfigID, tloop);
				dataObjectLoaded.Load(tloop);
				dataObjectLoaded["nametxt"] = "~~~ YES! ~~~";
				dataObjectLoaded.Save(tloop);
				tloop.Commit();
			}
			*/


			/*
			if (actionSecurityGuardID.Length > 0)
			{
				IObjectActionSecurityGuard guard  = SecurityManager.GetManager().GetObjectActionSecurityGuard(actionSecurityGuardID, transaction);
				guard.Authorize(session, action, dataObjectLoaded);
			}

			IDictionaryEnumerator itr = dataObjectLoaded.GetEnumerator();
			while (itr.MoveNext())
			{
				DataElement e = (DataElement) itr.Value;
				Console.WriteLine(e.GetColumn() + ": " + (string) e.GetValue());
			}
			*/

		}
	}
}
