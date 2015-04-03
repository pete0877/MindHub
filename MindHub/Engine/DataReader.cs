using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// todo: redesign WHERE clause addition on the DataReader
// Right now you have to say .Where("column operand argument") .. e.g. Where("objid = '123'")
// It would be nice to have: .Where("column" + OperandClass + "argument")
// Not all C# operators can be overriden so this might look funky
// Maybe having .Where(new OperandClass("column", "agument")) would be good. 
// This way each operand can override .toString and there is a lot of flexibility in SQL function usage (e.g. In(), Not(), etc)

namespace Engine
{
	/// <summary>
	/// Basic DataReader responsible for reading data from a database object
	/// </summary>
	public class DataReader : DataOperator, IStreamStorable
	{
		/// <summary>
		/// Average data object size (used to optimize memory stream creation)
		/// </summary>
		public static int AVERAGE_SIZE = 16384;

		/// <summary>
		/// Maximum number of rows that can be retreived from the database
		/// </summary>
		public static int MAX_ROWS = 100;

		/// <summary>
		/// Flag for if the reader is at a valid position (one from which data can be pulled)
		/// </summary>
		protected bool m_validPosition;

		/// <summary>
		/// Flag for if the data has been read yet
		/// </summary>
		protected bool m_loaded;

		/// <summary>
		/// Flag for if any columns have been specified yet
		/// </summary>
		protected bool m_columsSpecified;

		/// <summary>
		/// Current row number, starting with 0
		/// </summary>
		protected int m_currentRowPosition;

		/// <summary>
		/// Array of loaded records
		/// </summary>
		protected ArrayList m_rows;

		/// <summary>
		/// Array for storing "where" clauses
		/// </summary>
		protected ArrayList m_where;

		/// <summary>
		/// Array for storing "order by" clauses
		/// </summary>
		protected ArrayList m_orderBy;

		/// <summary>
		/// Header hash for storing information about selected columns
		/// </summary>
		protected Hashtable m_header;

		/// <summary>
		/// Basic constructor that initializes the schema name
		/// </summary>
		/// <param name="schema">Name of the database schema from which to draw records</param>
		/// <exception cref="EInvalidParameter">If schema is null</exception>		
		public DataReader(string schema) : base(schema)
		{
			m_where = new ArrayList();
			m_header = new Hashtable();
			m_orderBy = new ArrayList();
		}


		/// <summary>
		/// Static handler for + operator. Adds column to the reader's schema
		/// </summary>
		/// <param name="reader">Reader reference</param>
		/// <param name="column">Column name</param>
		/// <returns>Reference to the same reader</returns>
		public static DataReader operator +(DataReader reader, string column) 
		{			
			if (reader == null)
				throw new Exception("DataReader.operator+() received null reader argument");

			if (column == null)
				throw new Exception("DataReader.operator+() received null column argument");

			if (reader.m_loaded)
				throw new Exception("Cannot add columns to DataReader after load. Schema: " + reader.m_schema);

			reader.AddColumn(column);
			return reader;
		}


		/// <summary>
		/// Where clauase addition function.
		/// Performs set by reference (unsafe).
		/// </summary>
		/// <param name="clause">Where clause to be AND'ed with other where clasuses. If the caluse contains OR'ed sub-clauses, it needs to wrap the whole string with parens </param>
		/// <exception cref="EInvalidParameter">If clause is null</exception>				
		/// <example>reader.Where("(salary = 10 or salary = 11)");</example>
		public void Where(string clause)
		{
			if (clause == null)
				throw new EInvalidParameter(this, "clause");

			if (m_loaded)
				throw new Exception("Cannot add criteria clause to DataReader after load. Schema: " + m_schema);

			m_where.Add(clause);
		}


		/// <summary>
		/// Order-by clause addition function. Order OrderBy function calls will determine the sorting.
		/// Performs set by reference (unsafe).
		/// </summary>
		/// <param name="clause">Order by clause to be appended</param>
		/// <exception cref="EInvalidParameter">If clause is null</exception>		
		public void OrderBy(string clause)
		{
			if (clause == null)
				throw new EInvalidParameter(this, "clause");

			if (m_loaded)
				throw new Exception("Cannot add order by clause to DataReader after load. Schema: " + m_schema);

			m_orderBy.Add(string.Copy(clause));
		}


		/// <summary>
		/// Adds a column to the result set.
		/// Performs set by reference (unsafe).
		/// </summary>
		/// <param name="column">Column name</param>
		protected void AddColumn(string column)
		{
			m_columsSpecified = true;

			if (column == SQL_WILDCARD)
				m_header.Clear();

			m_header[column] = true;
		}


		/// <summary>
		/// Loads the data from the schema using the list of columns and criteria
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If transaction is null</exception>		
		public void Load(Transaction transaction)
		{
			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			transaction.LoadDataReader(this);
		}


		/// <summary>
		/// Loads the data from the schema using the list of columns and criteria
		/// </summary>
		/// <param name="connection">Database connection</param>
		/// <exception cref="EInvalidParameter">If connection is null</exception>		
		public void LoadFromDB(DBConnection connection)
		{
			if (connection == null)
				throw new EInvalidParameter(this, "connection");

			if (m_loaded)
				throw new Exception("Cannot reload data to DataReader after load. Schema: " + m_schema);

			if (!m_columsSpecified)
				throw new Exception("Cannot load until at least one column is specified. Schema: " + m_schema);

			// Create new list of rows
			m_rows = new ArrayList();

			string sql = ComposeSQLCommand();
				
			// Using the connection, retreive data:
			System.Data.IDataReader reader = connection.ExecuteRead(sql);
			LoadFromDBDataReader(reader, connection.EnvironmentID);

			m_validPosition = false;
			m_currentRowPosition = -1;
			m_loaded = true;
		}


		/// <summary>
		/// Loads the data from the schema using the specified select statement
		/// </summary>
		/// <param name="connection">Database connection</param>
		/// <param name="sql">SQL statement to be used to load the data</param>
		/// <exception cref="EInvalidParameter">If any parameters is null</exception>
		public void LoadFromDB(DBConnection connection, string sql)
		{
			if (connection == null)
				throw new EInvalidParameter(this, "connection");

			if (sql == null)
				throw new EInvalidParameter(this, "sql");

			if (m_loaded)
				throw new Exception("Cannot reload data to DataReader after load. Schema: " + m_schema);

			// Create new list of rows
			m_rows = new ArrayList();
				
			// Using the connection, retreive data:
			System.Data.IDataReader reader = connection.ExecuteRead(sql);
			LoadFromDBDataReader(reader, connection.EnvironmentID);

			m_validPosition = false;
			m_currentRowPosition = -1;
			m_loaded = true;
		}


		/// <summary>
		/// Internal method used to load itself from a database reader in given environment
		/// </summary>
		/// <param name="reader">Database reader (system class)</param>
		/// <param name="environmentID">Environment ID</param>
		protected void LoadFromDBDataReader(System.Data.IDataReader reader, string environmentID)
		{
			m_schemaDefinition = SchemaManager.GetManager().GetSchema(m_schema, environmentID);

			int rowCount = 0;
			while(reader.Read())
			{
				rowCount++;
				Hashtable row = new Hashtable();
				m_rows.Add(row);

				for (int count = 0; count < reader.FieldCount; count++) 
				{
					string col = reader.GetName(count).ToLower();
					DataValue val = null;
					DataValue.DataType columnType = m_schemaDefinition[col];
					object obj = reader[col];
					if (obj.GetType() == Type.GetType("System.DBNull"))
					{
						if (columnType == DataValue.DataType.String)
							val = DataValue.DEFAULT_STRING;
						else if (columnType == DataValue.DataType.DateTime)
							val = DataValue.DEFAULT_DATETIME;
						else if (columnType == DataValue.DataType.Double)
							val = DataValue.DEFAULT_DOUBLE;								
						else if (columnType == DataValue.DataType.Long)
							val = DataValue.DEFAULT_LONG;
						else if (columnType == DataValue.DataType.Flag)
							val = DataValue.DEFAULT_FLAG;
						else 
							val = new DataValue();
					}
					else
					{
						if (columnType == DataValue.DataType.String)
							val = (DataValue) ((string) reader[col]);
						else if (columnType == DataValue.DataType.DateTime)
							val = (DataValue) ((DateTime) reader[col]);
						else if (columnType == DataValue.DataType.Double)
							val = (DataValue) ((double) reader[col]);
						else if (columnType == DataValue.DataType.Long)
							val = (DataValue) ((Int32) reader[col]);
						else if (columnType == DataValue.DataType.Flag)
							val = (DataValue) ((Int16) reader[col] > 0 ? true : false);
						else 
							val = new DataValue();
					}

					row[col.ToLower()] = val;
				}

				// If we have reached the maximum number of rows we can get back, we shall stop:
				if (rowCount == MAX_ROWS)
					break;
			}	
			reader.Close();		
		}


		/// <summary>
		/// Composes and returns select statement that should be executed in order to retreive the data
		/// </summary>
		/// <returns>SQL statement (e.e. 'SELECT * FROM TCase')</returns>
		public string ComposeSQLCommand()
		{
			if (!m_columsSpecified)
				throw new Exception("Columns to be selected have not been specified");

			// Compose SQL statement:
			string sql = "SELECT ";

			// Iterate through selected columns:
			IDictionaryEnumerator itrColsSelect = m_header.GetEnumerator();
			bool first = true;
			while (itrColsSelect.MoveNext())
			{
				string col = (string) itrColsSelect.Key;
				if (col == DataOperator.SQL_WILDCARD)
					sql += DataOperator.SQL_WILDCARD;
				else
				{
					if (first)
						sql += "[" + col + "]";
					else
						sql += ", [" + col + "]";
				}

				first = false;
			}
			if (first)
				sql += SQL_WILDCARD;

			// Add FROM SCHEME statement 
			sql += " FROM " + m_schema;

			// Add condition on not selecting deleted records:
			m_where.Add(COLUMN_DELETEDFLG + " <> 1");

			// Iterate through WHERE clauses
			IEnumerator itrColsWhere = m_where.GetEnumerator();
			bool firstWhere = true;			
			while (itrColsWhere.MoveNext())
			{
				string where = (string) itrColsWhere.Current;
				if (firstWhere)
					sql += " WHERE (" + where + ")";
				else
					sql += " AND (" + where + ")";
				firstWhere = false;
			}

			// Iterate through ORDER BY clauses
			IEnumerator itrColsOrderBy = m_orderBy.GetEnumerator();
			bool firstOrderBy = true;
			while (itrColsOrderBy.MoveNext())
			{
				string order = (string) itrColsOrderBy.Current;
				if (firstOrderBy)
					sql += " ORDER BY " + order;
				else
					sql += ", " + order;
				firstOrderBy = false;
			}		

			return sql;		
		}


		/// <summary>
		/// Returns total row count (starting from 0)
		/// </summary>
		/// <returns>Number of rows loaded</returns>		
		public long GetRowCount()
		{
			if (!m_loaded)
				throw new Exception("DataReader not loaded. Cannot access data. Schema: " + m_schema);

			return m_rows.Count;
		}


		/// <summary>
		/// Returns current row nubmer (starting with 0). This function cannot be used until first Read() is called.
		/// </summary>
		/// <returns>Number of the current row</returns>
		public int GetCurrentRowNumber()
		{
			if (!m_validPosition)
				throw new Exception("DataReader not loaded or not at valid position. Cannot access data. Schema: " + m_schema);
			
			return m_currentRowPosition;
		}


		/// <summary>
		/// Basic access indexer. 
		/// Performs get by reference (unsafe). 
		/// This means that if calling code modifies it, it will affect the reader's internal representation of the data set.
		/// </summary>
		/// <param name="column">Configuration element name to operate on</param>		
		/// <exception cref="EInvalidParameter">If column is null</exception>
		public DataValue this[string column]
		{
			get
			{
				if (column == null)
					throw new EInvalidParameter(this, "column");

				if (!m_validPosition)
					throw new Exception("DataReader not loaded or not at valid position. Cannot access data. Schema: " + m_schema);

				Hashtable currentRow = (Hashtable) m_rows[m_currentRowPosition];
				if (currentRow == null)
					throw new Exception("Internal DataReader rrror: null pointer returned instead of record object" + m_schema);
				
				DataValue val = (DataValue) currentRow[column];
				if (val == null)
					return new DataValue("");

				return val;
			}
			set
			{				
				throw new Exception("Cannot update DataReader columns. Schema: " + m_schema);
			}			
		}


		/// <summary>
		/// Returns flag for if given column name is present in the record set.
		/// Performs get by valeu (safe). 
		/// </summary>
		/// <param name="column">Column name</param>		
		/// <exception cref="EInvalidParameter">If column is null</exception>
		public bool Contains(string column)
		{
			if (column == null)
				throw new EInvalidParameter(this, "column");

			if (!m_validPosition)
				throw new Exception("DataReader not loaded or not at valid position. Cannot access data. Schema: " + m_schema);

			Hashtable currentRow = (Hashtable) m_rows[m_currentRowPosition];
			if (currentRow == null)
				throw new Exception("Internal DataReader rrror: null pointer returned instead of record object" + m_schema);
				
			DataValue val = (DataValue) currentRow[column];
			if (val == null)
				return false;

			return true;
		}


		/// <summary>
		/// Returns flag for if the reader sits on a valid position (the indexer for column values can be used)
		/// Performs get by value (safe). 
		/// </summary>
		public bool IsOnValidPosition()
		{
			return m_validPosition;
		}


		/// <summary>
		/// Returns hash table of DataElements representing all data values of the current row. 
		/// All returned DataElements reference DataReader's internal DataValue (No copies are created).
		/// </summary>
		/// <returns>Hash of DateElements</returns>
		public Hashtable GetDataElements()
		{
			if (!m_validPosition)
				throw new Exception("DataReader not loaded or not at valid position. Cannot access data. Schema: " + m_schema);

			Hashtable currentRow = (Hashtable) m_rows[m_currentRowPosition];
			if (currentRow == null)
				throw new Exception("Internal DataReader rrror: null pointer returned instead of record object" + m_schema);

			Hashtable result = new Hashtable();
			IDictionaryEnumerator itr = currentRow.GetEnumerator();	
			while (itr.MoveNext())
			{
				DataValue val = (DataValue) itr.Value;
				DataElement element = new DataElement(m_schema, (string) itr.Key, val);
				result.Add(itr.Key, element);                
			}

			return result;
		}


		/// <summary>
		/// Resets the data set to it's first posistion. (before first call to Read() has been made)
		/// </summary>
		public void Reset()
		{
			if (!m_loaded)
				throw new Exception("Cannot move to first until DataReader is loaded. Schema: " + m_schema);

			// Reset position and eof attributes:
			m_validPosition = false;
			m_currentRowPosition = -1;
		}


		/// <summary>
		/// Moves to the next record in the loaded record set. If this causes the cursor to step outside of the record set, EOF flag is set.
		/// </summary>
		public bool Read()
		{
			if (!m_loaded)
				throw new Exception("Cannot move to next until DataReader is loaded. Schema: " + m_schema);

			// Check if there are no records returned from Load()
			if (m_rows.Count == 0)
				return false;

			// Case: After Load() or Reset() - move to first record:
			if (m_currentRowPosition < 0)
			{
				m_currentRowPosition = 0;
				m_validPosition = true;
				return true;
			}
			
			// Case: Before this call reader was already on last readable record. Move beyond readable area.
			if (m_currentRowPosition == m_rows.Count - 1)
			{
				m_validPosition = false;
				return false;
			}

			// Case: In middle of readable records
			m_currentRowPosition++;
			return true;			
		}


		/// <summary>
		/// Saves self to stream. Reader must be loaded.
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		virtual public void SaveToStream(MemoryStream stream)
		{
			if (stream == null)
				throw new EInvalidParameter(this, "stream");

			if (!m_loaded)
				throw new Exception("Cannot serialize data reader to stream until it is loaded");
			
			BinaryFormatter serializer = new BinaryFormatter();
			serializer.Serialize(stream, m_rows);			
		}

	
		/// <summary>
		/// Loads self from stream.
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <exception cref="EInvalidParameter">If any of the parameters are null</exception>
		virtual public void LoadFromStream(MemoryStream stream)
		{
			m_loaded = false;

			if (stream == null)
				throw new EInvalidParameter(this, "stream");

			if (stream.Length == 0)
				throw new Exception("Cannot load data reader from stream because stream is 0 in lenght");

			BinaryFormatter deserializer = new BinaryFormatter();
			m_rows = (ArrayList)(deserializer.Deserialize(stream));

			m_validPosition = false;
			m_currentRowPosition = -1;
			m_loaded = true;
		}
	}
}

