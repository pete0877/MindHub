using System;
using System.Web.UI;
using System.Collections;
using System.Collections.Specialized;
using Engine;
using WebAPI;

namespace BaseWebModules
{
	public class TestController : IController
	{
		/// <summary>
		/// Class ID
		/// </summary>
		protected static string g_classID = "BaseWebModules.TestController";

		/// <summary>
		/// Singelton delegator instance reference
		/// </summary>
		public static CreateObjectDelegator g_creator;

		/// <summary>
		/// Configuration variable: Data object class ID
		/// </summary>
		public const string CONFIG_DATAOBJECT					= "dataobject";

		/// <summary>
		/// Configuration variable: Data object config ID
		/// </summary>
		public const string CONFIG_DATAOBJECT_CONFIG			= "dataobjectconfig";

		/// <summary>
		/// Configuration variable: level security guard
		/// </summary>
		public const string CONFIG_LEVEL_SECURITY_GUARD			= "levelsecguard";

		/// <summary>
		/// Configuration variable: level security
		/// </summary>
		public const string CONFIG_LEVEL_SECURITY				= "levelsec";

		/// <summary>
		/// Configuration variable: action security guard
		/// </summary>
		public const string CONFIG_ACTION_SECURITY_GUARD		= "actionguard";

		/// <summary>
		/// Variable name: Data object ID
		/// </summary>
		public const string VAR_DATA_OBJECT_ID					= "vdaid";


		/// <summary>
		/// Creator method. Returns new instance of this class
		/// </summary>
		/// <returns>Instance of this class</returns>
		public static ICreatable CreateInstance()
		{
			return new TestController();
		}


		/// <summary>
		/// Returns class ID.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <return>Class ID</return>
		public virtual string GetClassID()
		{
			return g_classID;
		}


		/// <summary>
		/// Initializes singelton delegator reference and registers it with class factory.
		/// </summary>	
		public static void Register()
		{
			g_creator = new CreateObjectDelegator(TestController.CreateInstance);
			ClassFactory.GetFactory().Register(g_classID, g_creator);
		}


		/// <summary>
		/// Entry controller execution method.
		/// </summary>
		/// <param name="input">Web request input (comes from Page.Request.Params)</param>
		/// <param name="output">HTML output writer</param>
		/// <param name="session">Valid user session</param>
		public void Execute(NameValueCollection input, HtmlTextWriter output, Engine.Session session)
		{
			using (Transaction transaction = new Transaction(session))
			{
				// Identify the object:
				string objectID = input[VAR_DATA_OBJECT_ID];
				if (objectID == null || objectID == "")
					throw new EInvalidParameter(this, VAR_DATA_OBJECT_ID);

				// Identify the process:
				string processConfigID = input[Workspace.VAR_PROCESS_CONFIG_ID];
				Config config = ConfigManager.GetManager().GetConfig(processConfigID, transaction);

				// Fetch other action and object related parameters:
				string dataObjectClassID = config[CONFIG_DATAOBJECT];
				string dataObjectConfigID = config[CONFIG_DATAOBJECT_CONFIG];				
				string levelSecurityGuardID = config[CONFIG_LEVEL_SECURITY_GUARD];
				string levelSecurity = config[CONFIG_LEVEL_SECURITY];
				string actionSecurityGuardID = config[CONFIG_ACTION_SECURITY_GUARD];

				if (dataObjectClassID.Length == 0)
					throw new EInvalidParameter(this, CONFIG_DATAOBJECT, dataObjectClassID, "Process configuration " + processConfigID + " contains empty data object class id");
					
				if (dataObjectConfigID.Length == 0)
					throw new EInvalidParameter(this, CONFIG_DATAOBJECT_CONFIG, dataObjectConfigID, "Process configuration " + processConfigID + " contains empty data object config id");

				// Authorize based on level security:
				if (levelSecurityGuardID.Length > 0 && levelSecurity.Length > 0)
				{
					ILevelSecurityGuard guard  = SecurityManager.GetManager().GetLevelSecurityGuard(levelSecurityGuardID, transaction);
					guard.Authorize(session, levelSecurity);
				}

				// Create and load the data object:
				IDataObject dataObjectLoaded = (IDataObject) ClassFactory.GetFactory().Produce(dataObjectClassID);
				dataObjectLoaded.Initilize(objectID, dataObjectConfigID, transaction);
				dataObjectLoaded.Load(transaction);

				// Testing of template paring:
				Parser parser = new Parser(new FileTemplate("test.html", session), transaction);
				parser.AddInput(input);
				parser.AddInput(dataObjectLoaded);			
				parser.Parse(output);
			}
		}
	}
}
