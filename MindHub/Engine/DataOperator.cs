using System;

namespace Engine
{
	/// <summary>
	/// Base class for data readers and writers. Encapsulates constants and some logic common to both types of data operators
	/// </summary>
	public abstract class DataOperator
	{
		/// <summary>
		/// Column specifying if record has been deleted
		/// </summary>
		public const string COLUMN_DELETEDFLG	= "deletedflg";

		/// <summary>
		/// Column specifying when record has been deleted
		/// </summary>
		public const string COLUMN_DELETEDDT	= "deleteddt";

		/// <summary>
		/// SQL syntax wild card string;
		/// </summary>
		public static string SQL_WILDCARD = "*";

		/// <summary>
		/// Name of the schema object being read from
		/// </summary>
		protected string m_schema;

		/// <summary>
		/// Column definition for the schema from which this reader will read from
		/// </summary>
		protected Schema m_schemaDefinition;

		/// <summary>
		/// Sets the name of the schema being operated on.
		/// Perform set by value (safe)
		/// </summary>
		/// <param name="schema">Name of the schema being operated on</param>
		/// <exception cref="EInvalidParameter">If schema is null</exception>		
		public DataOperator(string schema)		
		{
			if (schema == null)
				throw new EInvalidParameter(this, "schema");

			m_schema = string.Copy(schema);
		}

		/// <summary>
		/// Returns name of the schema being operated on.
		/// Performs get by value (safe).
		/// </summary>
		/// <returns>Schema name</returns>
		public string GetSchemaName()
		{
			return string.Copy(m_schema);
		}
	}
}
