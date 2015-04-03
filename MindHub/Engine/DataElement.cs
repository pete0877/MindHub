using System;

namespace Engine
{
	/// <summary>
	/// Represents database element stored within a table. This class is only used internally by the DataObject class.
	/// All operations on this object are unsafe (set by reference and get by reference is used).
	/// </summary>
	[Serializable]
	public class DataElement
	{
		/// <summary>
		/// Table name
		/// </summary>
		protected string m_table;

		/// <summary>
		/// Column name
		/// </summary>
		protected string m_column;

		/// <summary>
		/// Element value
		/// </summary>
		protected DataValue m_value;

		/// <summary>
		/// Flag for if this element has been modified since
		/// </summary>
		protected bool m_modified;

		/// <summary>
		/// Basic constuctor. Initiates the data element and begins with the modified flag at false;
		/// </summary>
		/// <param name="column">Column name</param>
		/// <param name="val">Data value</param>
		public DataElement(string table, string column, DataValue val)
		{
			m_table = table;
			m_column = column;
			m_value = val;
			m_modified = false;
		}


		/// <summary>
		/// Returns current data value of the data element.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <returns></returns>
		public DataValue GetValue()
		{
			return m_value;
		}


		/// <summary>
		/// Sets the current data value of the data element.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <param name="val">New value</param>
		public void SetValue(DataValue val)
		{
			m_value = val;
			m_modified = true;
		}


		/// <summary>
		/// Gets the m_modified flag (which tells us if the element value has been modified).
		/// Performs get by value (safe).
		/// </summary>
		/// <returns>m_modified</returns>
		public bool GetModified()
		{
			return m_modified;
		}

	
		/// <summary>
		/// Sets the m_modified flag (which tells us if the element value has been modified).
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="modifiedFlag">New flag value</param>
		public void SetModified(bool modifiedFlag)
		{
			m_modified = modifiedFlag;
		}


		/// <summary>
		/// Returns reference to the column name.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <returns>Column name</returns>
		public string GetColumn()
		{
			return m_column;
		}


		/// <summary>
		/// Returns reference to the table name.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <returns>Table name</returns>
		public string GetTable()
		{
			return m_table;
		}
	}
}
