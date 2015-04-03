using System;
using System.Collections;
using System.Data;

namespace Engine
{
	/// <summary>
	/// Basic DataWriter responsible for writing data to database (using only UPDATE statement)
	/// </summary>
	public class DataWriter : DataOperator
	{
		/// <summary>
		/// Flag for if the set statement has been specified yet (at least one clause)
		/// </summary>
		protected bool m_setSpecifiedFixed;
		protected bool m_setSpecifiedCustom;

		/// <summary>
		/// Flag for if the update has already been executed
		/// </summary>
		protected bool m_executed;

		/// <summary>
		/// Array for storing "where" clauses
		/// </summary>
		protected ArrayList m_where;

		/// <summary>
		/// Hash for storing "set" clauses
		/// </summary>
		protected ArrayList m_set;

		/// <summary>
		/// Hash for storing "set" clauses using data values
		/// </summary>
		protected Hashtable m_setDataValues;

		/// <summary>
		/// Basic constructor that initializes the schema name
		/// </summary>
		/// <param name="schema">Name of the database schema that will be updated</param>
		/// <exception cref="EInvalidParameter">If schema is null</exception>		
		public DataWriter(string schema) : base(schema)
		{			
			m_where = new ArrayList();
			m_set = new ArrayList();
			m_setDataValues = new Hashtable();		
		}


		/// <summary>
		/// Static handler for + operator. Adds string-based set clauses to the UPDATE statement
		/// </summary>
		/// <param name="writer">Writer reference</param>
		/// <param name="clause">Set clause</param>
		/// <returns>Reference to the same writer</returns>
		public static DataWriter operator +(DataWriter writer, string clause) 
		{
			if (writer == null)
				throw new Exception("DataWriter.operator+() received null writer argument");

			if (clause == null)
				throw new Exception("DataWriter.operator+() received null clause argument");

			if (writer.m_executed)
				throw new Exception("Cannot add where clauses to DataWriter after Save was called. Schema: " + writer.m_schema);

			writer.AddSetClause(clause);
			return writer;
		}

		
		/// <summary>
		/// Adds 'set' clause to the writer. 
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="element">Name of the element (column)</param>
		/// <param name="val">DataValue object</param>
		/// <exception cref="EInvalidParameter">If either of the parameters are null </exception>		
		public void Set(string element, DataValue val) 
		{
			SetByReference(element, new DataValue(val));
		}


		/// <summary>
		/// Adds 'set' clause to the writer.
		/// Performs set by reference (unsafe).
		/// </summary>
		/// <param name="element">Name of the element (column)</param>
		/// <param name="val">DataValue object</param>
		/// <exception cref="EInvalidParameter">If either of the parameters are null </exception>		
		public void SetByReference(string element, DataValue val) 
		{
			if (element == null)
				throw new EInvalidParameter(this, "element");

			if (val == null)
				throw new EInvalidParameter(this, "val");

			if (m_executed)
				throw new Exception("Cannot add where clauses to DataWriter after Save was called. Schema: " + m_schema);

			m_setDataValues[element] = val;

			m_setSpecifiedFixed = true;
		}

		
		/// <summary>
		/// Adds a set caluse to the update statement.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="clause">String-based clause</param>
		protected void AddSetClause(string clause)
		{
			m_setSpecifiedCustom = true;
			m_set.Add(string.Copy(clause));
		}


		/// <summary>
		/// Where clauase addition function
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="clause">Where clause to be AND'ed with other where clasuses. If the caluse contains OR'ed sub-clauses, it needs to wrap the whole string with parens </param>
		/// <exception cref="EInvalidParameter">If clause is null</exception>		
		/// <example>writer.Where("(salary = 10 or salary = 11)");</example>
		public void Where(string clause)
		{
			if (clause == null)
				throw new EInvalidParameter(this, "clause");

			if (m_executed)
				throw new Exception("Cannot add criteria clause to DataWriter after save. Schema: " + m_schema);

			m_where.Add(string.Copy(clause));
			
		}


		/// <summary>
		/// Saves the data by using given transaction (using UPDATE statement)
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If transaction is null</exception>		
		public void Update(Transaction transaction)
		{
			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			// todo: use the same methodology as DataReader ... execute transaction.UpdateDataWriter()...
			if (transaction.GetLocation() == Location.APP_SERVER)
				Update(transaction.GetDBConnection());
			else
			{
				// TODO: Remote call
				throw new System.NotImplementedException();
			}
		}


		/// <summary>
		/// Saves the data by using given DB connection (using UPDATE statement)
		/// </summary>
		/// <param name="connection">Database connection</param>
		/// <exception cref="EInvalidParameter">If connection is null</exception>		
		public void Update(DBConnection connection)
		{
			if (connection == null)
				throw new EInvalidParameter(this, "connection");

			if (m_executed)
				throw new Exception("Cannot resave data to DataWriter after save. Schema: " + m_schema);

			if (!m_setSpecifiedFixed && !m_setSpecifiedCustom)
				throw new Exception("Cannot save to DB connection until at least one update statement has been specified. Schema: " + m_schema);

			// Get the schema definition:
			m_schemaDefinition = SchemaManager.GetManager().GetSchema(m_schema, connection.EnvironmentID);

			// Compose SQL statement:
			string sql = "UPDATE " + m_schema + " SET ";

			// Iterate through set clauses:
			IEnumerator itrSets = m_set.GetEnumerator();
			bool first = true;
			while (itrSets.MoveNext())
			{
				string setClause = (string) itrSets.Current;
				if (first)
					sql += setClause;
				else
					sql += ", " + setClause;
				first = false;
			}

			// Iterate through element + data value pairs:
			IDictionaryEnumerator itrSetDataValues = m_setDataValues.GetEnumerator();
			while (itrSetDataValues.MoveNext())
			{
				string element = (string) itrSetDataValues.Key;
				DataValue val = (DataValue) itrSetDataValues.Value;
				string setClause = "[" + element + "] = ";
		
				// Determine data type and format the set statement accordingly:
				DataValue.DataType dataType = m_schemaDefinition[element];
				switch (dataType)
				{
					case DataValue.DataType.String:
						setClause += "'" + val.GetStringByReference() + "'";
						break;
					case DataValue.DataType.DateTime:					
						setClause += "'" + val.GetDateTime().ToString() + "'";
						break;
					case DataValue.DataType.Flag:
						setClause += val.GetLong().ToString();
						break;
					case DataValue.DataType.Double:
						setClause += val.GetDouble().ToString();
						break;
					case DataValue.DataType.Long:
						setClause += val.GetLong().ToString();
						break;
					default: // Uknown
						setClause += "'" + val.GetStringByReference() + "'";
						break;					
				}


				if (first)
					sql += setClause;
				else
					sql += ", " + setClause;
				first = false;
			}

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

			if (firstWhere)
				throw new Exception("DataWriter cannot update all records in given schema. Where clause must be provided");
				
			// Using the connection, execute the statement:
			connection.ExecuteSQL(sql);
			m_executed = true;
		}


		/// <summary>
		/// Deletes the data by using given transaction (using UPDATE statement)
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If transaction is null</exception>		
		public void Delete(Transaction transaction)
		{
			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			// todo: use the same methodology as DataReader ... execute transaction.UpdateDataWriter()...
			if (transaction.GetLocation() == Location.APP_SERVER)
				Delete(transaction.GetDBConnection());
			else
			{
				// TODO: Remote call
				throw new System.NotImplementedException();
			}
		}


		/// <summary>
		/// Deletes the data by using given transaction (using UPDATE statement)
		/// </summary>
		/// <param name="connection">Database connection</param>
		/// <exception cref="EInvalidParameter">If connection is null</exception>		
		public void Delete(DBConnection connection)
		{
			if (connection == null)
				throw new EInvalidParameter(this, "connection");

			if (m_executed)
				throw new Exception("Cannot resave data to DataWriter after save. Schema: " + m_schema);

			SetByReference(COLUMN_DELETEDFLG, 1);
			SetByReference(COLUMN_DELETEDDT, new DataValue(DateTime.Now));

			// Compose SQL statement:
			string sql = "UPDATE " + m_schema + " SET ";
			sql += COLUMN_DELETEDFLG + " = 1, ";
			sql += COLUMN_DELETEDDT + " = '" + DateTime.Now.ToString() + "'";

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

			// If there were no where clauses, throw error:
			if (firstWhere)
				throw new Exception("DataWriter cannot delete all records from given schema. Where clause must be provided");
				
			// Using the connection, execute the statement:
			connection.ExecuteSQL(sql);
			m_executed = true;
		}


		/// <summary>
		/// Saves the data by using given transaction (using INSERT statement)
		/// </summary>
		/// <param name="transaction">Transaction</param>
		/// <exception cref="EInvalidParameter">If transaction is null</exception>		
		public void Insert(Transaction transaction)
		{
			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			if (transaction.GetLocation() == Location.APP_SERVER)
				Insert(transaction.GetDBConnection());
			else
			{
				// TODO: Remote call
				throw new System.NotImplementedException();
			}
		}


		/// <summary>
		/// Saves the data by using given DB connection (using INSERT statement)
		/// </summary>
		/// <param name="connection">Database connection</param>
		/// <exception cref="EInvalidParameter">If connection is null</exception>		
		public void Insert(DBConnection connection)
		{
			if (connection == null)
				throw new EInvalidParameter(this, "connection");

			if (m_executed)
				throw new Exception("Cannot resave data to DataWriter after save. Schema: " + m_schema);

			if (!m_setSpecifiedFixed)
				throw new Exception("Cannot save to DB connection until at least one update statement has been specified. Schema: " + m_schema);

			// Get the schema definition:
			m_schemaDefinition = SchemaManager.GetManager().GetSchema(m_schema, connection.EnvironmentID);

			// Compose SQL statement:
			string sql = "INSERT INTO " + m_schema + " (";

			// Iterate through element + data value pairs to get the '(col list)' and 'values (val list)' strings:
			string columnList = "";
			string valueList = "";
			IDictionaryEnumerator itrSetDataValues = m_setDataValues.GetEnumerator();
			while (itrSetDataValues.MoveNext())
			{
				string element = (string) itrSetDataValues.Key;
				DataValue val = (DataValue) itrSetDataValues.Value;

				if (columnList.Length == 0)
					columnList += element;
				else
					columnList += ", " + element;

				if (valueList.Length != 0)
					valueList += ", ";

				// Determine data type and format the set statement accordingly:
				DataValue.DataType dataType = m_schemaDefinition[element];
				switch (dataType)
				{
					case DataValue.DataType.String:
						valueList += "'" + val.GetStringByReference() + "'";
						break;
					case DataValue.DataType.DateTime:					
						valueList += "'" + val.GetDateTime().ToString() + "'";
						break;
					case DataValue.DataType.Flag:
						valueList += val.GetLong().ToString();
						break;
					case DataValue.DataType.Double:
						valueList += val.GetDouble().ToString();
						break;
					case DataValue.DataType.Long:
						valueList += val.GetLong().ToString();
						break;
					default: // Uknown
						valueList += "'" + val.GetStringByReference() + "'";
						break;					
				}
			}

			columnList += ", " + COLUMN_DELETEDFLG;
			valueList += ", 0";

			sql += columnList + ") values (" + valueList + ")";
				
			// Using the connection, execute the statement:
			connection.ExecuteSQL(sql);
			m_executed = true;
		}
	}
}
