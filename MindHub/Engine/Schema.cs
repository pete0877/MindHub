using System;
using System.Collections;

namespace Engine
{
	/// <summary>
	/// Generic schema definition
	/// </summary>
	[Serializable]
	public class Schema
	{
		// Name of the schema object
		public string Name;

		// Hash of filed names to field types (of DataValue.DataType type)
		protected Hashtable m_fieldTypes;		
		
		/// <summary>
		/// Constructor.
		/// Performs set of name by value (safe)
		/// </summary>
		/// <param name="name">Name of the table which this instance represents</param>
		/// <exception cref="EInvalidParameter">If schema name is null</exception>
		public Schema(string name)
		{
			if (name == null)
				throw new EInvalidParameter(this, "name");

			m_fieldTypes = new Hashtable();
			Name = string.Copy(name);
		}


		/// <summary>
		/// Basic access indexer.
		/// Performs set / get by value (safe)
		/// </summary>
		/// <param name="column">Column name</param>	
		/// <exception cref="EInvalidParameter">If column name is null or it the column does not exist in the definition</exception>	
		public DataValue.DataType this[string column]
		{
			get
			{				
				if (column == null)
					throw new EInvalidParameter(this, "column");

				string formattedColumn = column.ToLower();

				if (!m_fieldTypes.ContainsKey(formattedColumn))
					throw new EInvalidParameter(this, "columnReference", column, "Schema " + Name + " does not contain the column");

				return (DataValue.DataType) m_fieldTypes[formattedColumn];
			}
			set
			{				
				if (column == null)
					throw new EInvalidParameter(this, "column");

				string formattedColumn = column.ToLower();

				m_fieldTypes[formattedColumn] = value;
			}			
		}	
	

		/// <summary>
		/// Returns enumeration for columns without making copy of values within the list (unsafe)
		/// </summary>
		/// <returns>List of references to schema columns</returns>
		public IDictionaryEnumerator GetEnumerator()
		{
			return m_fieldTypes.GetEnumerator();
		}
	}
}
