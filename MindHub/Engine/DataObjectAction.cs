using System;
using System.IO;

namespace Engine
{
	/// <summary>
	/// Encapsulates action performed on an object. Stores the data object pointer.
	/// </summary>
	[Serializable]
	public class DataObjectAction
	{
		/// <summary>
		/// Type of action (E.g. Saving object, deleting, etc)
		/// </summary>
		public enum ActionType { Insert, Update, Delete };

		/// <summary>
		/// Instance data object action type
		/// </summary>
		protected ActionType m_action;

		/// <summary>
		/// Data object class ID
		/// </summary>
		protected string	m_dataObjectClassID;

		/// <summary>
		/// Data object config ID
		/// </summary>
		protected string	m_dataObjectConfigID;

		/// <summary>
		/// Data object ID
		/// </summary>
		protected string	m_dataObjectID;

		/// <summary>
		/// Data object being saved or deleted
		/// </summary>
		protected DataObject m_dataObject;

		/// <summary>
		/// Minimal data object state in case entire object is not stored in m_dataObject
		/// </summary>
		protected byte[] m_dataObjectState;

		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="action">ActionType switch</param>
		/// <param name="dataObject">Data object being saved or deleted</param>
		/// <param name="storeMinimal">Switch for if minimal information about the object should be stored instead of storing the reference to the object</param>
		/// <exception cref="EInvalidParameter">If dataObject is null</exception>
		public DataObjectAction(ActionType action, DataObject dataObject, bool storeMinimal)
		{
			if (dataObject == null)
				throw new EInvalidParameter(this, "dataObject");

			m_action = action;

			m_dataObjectClassID = dataObject.GetClassID();
			m_dataObjectConfigID = dataObject.GetConfigID();
			m_dataObjectID = dataObject.GetID();

			// If we are to store minimal info about the object, we face two possibilities:
			// If we are saving, we need to stream the object into byte array
			// If we are to delete it, we already have all the info we need (IDs above)
			if (storeMinimal)
			{
				m_dataObject = null;
				if (m_action == ActionType.Insert || m_action == ActionType.Update)
				{
					MemoryStream stream = new MemoryStream(DataObject.AVERAGE_SIZE); // todo: figure out a way to create stream of exact size
					dataObject.SaveToStream(stream);						
					m_dataObjectState = stream.GetBuffer();
				}
			}
			else
				m_dataObject = dataObject;

		}

		/// <summary>
		/// Returns reference to the data object
		/// Performs return by reference (unsafe)
		/// </summary>
		/// <returns>Reference to the data object</returns>
		public IDataObject GetDataObject()
		{
			return m_dataObject;
		}

		
		/// <summary>
		/// Sets reference to the data object.
		/// Performs set by reference (unsafe)
		/// </summary>
		/// <param name="dataObject">Reference to the data object</param>
		public void SetDataObject(IDataObject dataObject)
		{
			m_dataObject = (DataObject) dataObject;
		}


		/// <summary>
		/// Returns the action type.
		/// Performs return by value (safe)
		/// </summary>
		/// <returns>Action type</returns>
		public ActionType GetAction()
		{
			return m_action;
		}


		/// <summary>
		/// Returns reference to the data object's state
		/// Performs return by reference (unsafe)
		/// </summary>
		/// <returns>Reference to the data object's state</returns>
		public byte[] GetDataObjectState()
		{
			return m_dataObjectState;
		}


		/// <summary>
		/// Returns the reference to data objects's class ID
		/// Performs return by reference (unsafe)
		/// </summary>
		/// <returns>Reference to data objects's class ID</returns>
		public string GetDataObjectClassID ()
		{
			return m_dataObjectClassID;
		}


		/// <summary>
		/// Returns the reference to data objects's config ID
		/// Performs return by reference (unsafe)
		/// </summary>
		/// <returns>Reference to data objects's config ID</returns>
		public string GetDataObjectConfigID ()
		{
			return m_dataObjectConfigID;
		}


		/// <summary>
		/// Returns the reference to data objects's ID
		/// Performs return by reference (unsafe)
		/// </summary>
		/// <returns>Reference to data objects's ID</returns>
		public string GetDataObjectID ()
		{
			return m_dataObjectID;
		}
	}
}
