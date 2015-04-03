using System;
using System.Collections;
using System.IO;

namespace Engine
{
	/// <summary>
	/// Manager called to operate purely on transactions.
	/// </summary>
	public class DataManager : Manager
	{
		/// <summary>
		/// Singelton manager reference.
		/// </summary>
		static protected DataManager g_manager;

		/// <summary>
		/// Static constructor. Creates the singelton instance.
		/// </summary>
		static DataManager()
		{
			g_manager = new DataManager();
		}


		/// <summary>
		/// Default constructor. 
		/// </summary>
		protected DataManager()
		{			
		}


		/// <summary>
		/// Returns referece to the singelton manager
		/// </summary>
		/// <returns>Reference to sigelton manager</returns>
		public static DataManager GetManager()
		{			
			return g_manager;
		}


		/// <summary>
		/// Initializes instance of this manager.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			LogManager.Log(this, "Initialized");
		}


		/// <summary>
		/// Loads and serializes specified data object into byte array.
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <param name="dataObjectClassID">Class ID of the data object</param>
		/// <param name="dataObjectID">ID of the data object</param>
		/// <param name="dataObjectConfigID">Data object's config ID</param>
		/// <returns>Byte array</returns>
		public byte[] LoadDataObject(Transaction transaction, string dataObjectClassID, string dataObjectID, string dataObjectConfigID)
		{
			// Load the object
			IDataObject dataObjectLoaded = (IDataObject) ClassFactory.GetFactory().Produce(dataObjectClassID);
			dataObjectLoaded.Initilize(dataObjectID, dataObjectConfigID, transaction);
			dataObjectLoaded.Load(transaction);

			MemoryStream stream = new MemoryStream(DataObject.AVERAGE_SIZE); // todo: figure out a way to create stream of exact size
			dataObjectLoaded.SaveToStream(stream);						

			return stream.GetBuffer();			
		}


		/// <summary>
		/// Loads and serializes data reader based on SQL statement
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <param name="schema">Name of the schema being readed from</param>
		/// <param name="sql">SQL statement</param>
		/// <returns>Byte array</returns>
		public byte[] LoadDataReader(Transaction transaction, string schema, string sql)
		{
			return transaction.StreamDataReader(schema, sql).GetBuffer();
		}

 
		/// <summary>
		/// Commits list of data object actions within given environment.
		/// </summary>
		/// <param name="environmentID">Environment ID</param>
		/// <param name="dataObjectActions">List of data object actions</param>
		public void CommitTransaction(Transaction transaction, ArrayList dataObjectActions)
		{
			try
			{
				// Prep the list of data object actions:
				IEnumerator itr = dataObjectActions.GetEnumerator();
				while(itr.MoveNext())
				{
					DataObjectAction action = (DataObjectAction) itr.Current;
					string dataObjectClassID = action.GetDataObjectClassID();
					string dataObjectConfigID = action.GetDataObjectConfigID();
					string dataObjectID = action.GetDataObjectID();

					// Create the instance of the data object
					IDataObject dataObjectLoaded = (IDataObject) ClassFactory.GetFactory().Produce(dataObjectClassID);
					dataObjectLoaded.Initilize(dataObjectID, dataObjectConfigID, transaction);
					action.SetDataObject(dataObjectLoaded);
					if (
						action.GetAction() == DataObjectAction.ActionType.Insert ||
						action.GetAction() == DataObjectAction.ActionType.Update
					)
					{
						MemoryStream stream = new MemoryStream(action.GetDataObjectState());
						dataObjectLoaded.LoadFromStream(stream);

						if (action.GetAction() == DataObjectAction.ActionType.Insert)
							dataObjectLoaded.IsNew = true;

					}

					// If we are deleting a data object, we don't need to load it's state (action.GetDataObjectState() would return null as the state is not conveyed over the remote connection)
				}

				// Once the list of actions is all set, we commit the transaction to the database:
				transaction.SetDataObjectActions(dataObjectActions);
				transaction.Commit();				
			}
			catch(Exception e)
			{
				LogManager.Log(this, "Request to commit transaction failed: " + e.ToString());
				throw e;
			}		
		}

	}
}
