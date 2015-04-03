using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace Engine
{
	[Serializable]
	public class CalendarDataObject : DataObject
	{		
		/// <summary>
		/// Default config ID for any instances of this class
		/// </summary>
		static public string CONFIGID_DEFAULT			= "2";

		/// <summary>
		/// Schema in default configuration.
		/// </summary>
		static public string SCHEMA_DEFAULT				= "TCalendar";

		/// <summary>
		/// Column used to store owner of the calendar item.
		/// </summary>
		static public string COLUMN_OWNERID				= "ownerid";

		/// <summary>
		/// Column used to store owner of the calendar item.
		/// </summary>
		static public string COLUMN_STARTDT				= "startdt";

		/// <summary>
		/// Column used to store owner of the calendar item.
		/// </summary>
		static public string COLUMN_ENDDT				= "enddt";

		/// <summary>
		/// Singelton delegator instance reference
		/// </summary>
		new public static CreateObjectDelegator g_creator;

		/// <summary>
		/// Class ID
		/// </summary>
		new protected static string g_classID			= "Engine.CalendarDataObject";	

		/// <summary>
		/// Creator method. Returns new instance of this class
		/// </summary>
		/// <returns>Instance of this class</returns>
		new public static ICreatable CreateInstance()
		{
			return new CalendarDataObject();
		}


		/// <summary>
		/// Initializes singelton delegator reference and registers it with class factory.
		/// </summary>	
		new public static void Register()
		{
			g_creator = new CreateObjectDelegator(CalendarDataObject.CreateInstance);
			ClassFactory.GetFactory().Register(g_classID, g_creator);
		}


		/// <summary>
		/// Returns class ID.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <return>Class ID</return>
		override public string GetClassID()
		{
			return g_classID;
		}


		/// <summary>
		/// Initializes instance (assumes standard configuration ID)
		/// Assumes new object is being created. CONFIGID_DEFAULT will be used for configuration ID.
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		virtual public void Initilize(Transaction transaction)
		{
			base.Initilize(CONFIGID_DEFAULT, transaction);
		}


		/// <summary>
		/// Initializes instance with given configuration ID.
		/// Assumes new object is being created.
		/// </summary>
		/// <param name="configID">Configuration ID. If null CONFIGID_DEFAULT will be used.</param>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		override public void Initilize(string configID, Transaction transaction)
		{
			if (configID == null)
				base.Initilize(CONFIGID_DEFAULT, transaction);
			else
				base.Initilize(configID, transaction);
		}


		/// <summary>
		/// Initializes instance with given object ID and configuration ID.
		/// Assumes existing object will be loaded or deleted.
		/// </summary>
		/// <param name="objectID">Object ID</param>
		/// <param name="configID">Configuration ID. If null CONFIGID_DEFAULT will be used.</param>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		override public void Initilize(string objectID, string configID, Transaction transaction)
		{
			if (configID == null)
				base.Initilize(objectID, CONFIGID_DEFAULT, transaction);
			else
				base.Initilize(objectID, configID, transaction);
		}
	}
}
