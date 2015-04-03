using System;

namespace Engine
{
	/// <summary>
	/// Basic system variant data type optimized for type conversions.
	/// Almost all operations on this class are reference safe (set and get by value are done throughout). Expections are: GetStringByReference(), SetStringByReference(), ToString().
	/// </summary>
	[Serializable]
	public class DataValue
	{
		/// <summary>
		/// Data types
		/// </summary>
		public enum DataType { Uknown, String, Long, Double, DateTime, Flag };

		// Boolean logic representations for conversion types other then bool
		static public byte TRUE_BYTE = 1;
		static public byte FALSE_BYTE = 0;
		static public string TRUE_STRING = "1";
		static public string FALSE_STRING = "0";

		// Default variant values
		static public string DEFAULT_STRING = "";
		static public long DEFAULT_LONG = 0;
		static public double DEFAULT_DOUBLE = 0;
		static public bool DEFAULT_FLAG = false;
		static public DateTime DEFAULT_DATETIME = DateTime.MinValue;

		// Variant representations
		protected string m_string;
		protected long m_long;
		protected double m_double;
		protected DateTime m_datetime;
		protected bool m_flag;
		
		/// <summary>
		/// Current data type of the variant
		/// </summary>
		protected DataType m_type;

		// Flags for if different types of values have already been converted to from the current value and type
		protected bool m_convertedString;
		protected bool m_convertedLong;
		protected bool m_convertedDouble;
		protected bool m_convertedDateTime;
		protected bool m_convertedFlag;

		/// <summary>
		/// Default constructor. Resets the variant.
		/// </summary>
		public DataValue()
		{
			Reset();
		}

		/// <summary>
		/// String-initializing contructor.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="val">Initial value</param>
		public DataValue(string val)
		{
			SetString(val);
		}


		/// <summary>
		/// Long-initializing contructor. 
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="val">Initiali value</param>
		public DataValue(long val)
		{
			SetLong(val);
		}


		/// <summary>
		/// Double-initializing contructor. 
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="val">Initiali value</param>
		public DataValue(double val)
		{
			SetDouble(val);
		}


		/// <summary>
		/// DateTime-initializing contructor.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="val">Initiali value</param>
		public DataValue(DateTime val)
		{
			SetDateTime(val);
		}


		/// <summary>
		/// bool-initializing contructor.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="val">Initiali value</param>
		public DataValue(bool val)
		{
			SetFlag(val);			
		}


		/// <summary>
		/// Copy contructor. 
		/// Performs copy by value (safe)
		/// </summary>
		/// <param name="datavalue">Copy-from</param>
		public DataValue(DataValue datavalue)
		{
			m_type = datavalue.GetDataType();
			switch (m_type)
			{
				case DataType.String:
					SetString(datavalue.m_string);
					break;
				case DataType.DateTime:					
					SetDateTime(datavalue.m_datetime);
					break;
				case DataType.Flag:
					SetFlag(datavalue.m_flag);
					break;
				case DataType.Double:
					SetDouble(datavalue.m_double);
					break;
				case DataType.Long:
					SetLong(datavalue.m_long);
					break;
				default: // Uknown
					Reset();
					break;
			}

		}


		/// <summary>
		/// Returns current data type
		/// </summary>
		/// <returns>Current data type of the variant</returns>
		public DataType GetDataType()
		{
			return m_type;
		}


		/// <summary>
		/// Resets itself to type DataType. Uknown and default values.
		/// </summary>
		public void Reset()
		{
			m_convertedString = false;
			m_convertedLong = false;
			m_convertedDouble = false;
			m_convertedDateTime = false;
			m_convertedFlag = false;

			m_type = DataType.Uknown;
		}


		/// <summary>
		/// Returns current value. Converts between data types if needed. 
		/// Performs get by reference (unsafe).
		/// This function should be used only to gain performance and to avoid value-coping when not needed.
		/// </summary>
		/// <returns>Current value</returns>
		public string GetStringByReference()
		{
			// If we already have the string converted to, we just return its copy
			if (m_convertedString)
				return m_string;

			try 
			{
				// Else we need to convert:
				switch (m_type)
				{
					case DataType.DateTime:
						m_string = m_datetime.ToLongDateString();
						break;
					case DataType.Flag:
						if (m_flag)
							m_string = TRUE_STRING;
						else
							m_string = FALSE_STRING;
						break;
					case DataType.Double:
						m_string = m_double.ToString();
						break;
					case DataType.Long:
						m_string = m_long.ToString();
						break;
					default: // DataType.Uknown
						m_string = DataValue.DEFAULT_STRING;
						break;
				}
			}
			catch 
			{
				m_string = DEFAULT_STRING;				
			}
			
			m_convertedString = true;
			return m_string;
		}
		

		/// <summary>
		/// Returns current value. Converts between data types if needed. 
		/// Performs get by value (safe).
		/// </summary>
		/// <returns>Current value</returns>
		public string GetString()
		{
			return string.Copy(GetStringByReference());
		}


		/// <summary>
		/// Returns current value. Converts between data types if needed. 
		/// Performs get by value (safe).
		/// </summary>
		/// <returns>Current value</returns>
		public long GetLong()
		{
			// Check if the value of the type has already been converted
			if (m_convertedLong)
				return m_long;

			try 
			{

				// Else we need to convert:
				switch (m_type)
				{
					case DataType.String:
						m_long = long.Parse(m_string);
						break;
					case DataType.DateTime:
						m_long = m_datetime.Ticks;
						break;
					case DataType.Flag:
						if (m_flag)
							m_long = (long) TRUE_BYTE;
						else
							m_long = (long) FALSE_BYTE;
						break;
					case DataType.Double:
						m_long = (long) m_double;
						break;
					default: // DataType.Uknown
						m_long = DEFAULT_LONG;
						break;
				}
			}
			catch 
			{
				m_long = DEFAULT_LONG;
			}

			m_convertedLong = true;				
			return m_long;
		}


		/// <summary>
		/// Returns current value. Converts between data types if needed. 
		/// Performs get by value (safe).
		/// </summary>
		/// <returns>Current value</returns>
		public DateTime GetDateTime()
		{
			if (m_convertedDateTime)
				return new DateTime(m_datetime.Ticks);

			try 
			{
				// Else we need to convert:
				switch (m_type)
				{
					case DataType.String:
						m_datetime = DateTime.Parse(m_string);
						break;					
					case DataType.Long:
						m_datetime = new DateTime(m_long);
						break;
					default: // DataType.Uknown and other
						m_datetime = DEFAULT_DATETIME;
						break;
				}
			}
			catch 
			{
				m_datetime = DEFAULT_DATETIME;
			}

			m_convertedDateTime = true;
			return m_datetime;
		}


		/// <summary>
		/// Returns current value. Converts between data types if needed. 
		/// Performs get by value (safe).
		/// </summary>
		/// <returns>Current value</returns>
		public bool GetFlag()
		{
			// Check if the value of the type has already been converted
			if (m_convertedFlag)
				return m_flag;

			try
			{

				// Else we need to convert:
				switch (m_type)
				{
					case DataType.String:
						if (m_string == TRUE_STRING)
							m_flag = true;
						else
							m_flag = false;
						break;
					case DataType.Double:
						m_flag = (m_double == TRUE_BYTE);
						break;
					case DataType.Long:
						m_flag = (m_long == TRUE_BYTE);
						break;
					default: // DataType.Uknown and other
						m_flag = DEFAULT_FLAG;
						break;
				}
			}
			catch 
			{
				m_flag = DEFAULT_FLAG;
			}

			m_convertedFlag = true;
			return m_flag;
		}


		/// <summary>
		/// Returns current value. Converts between data types if needed. 
		/// Performs get by value (safe).
		/// </summary>
		/// <returns>Current value</returns>
		public double GetDouble()
		{
			// Check if the value of the type has already been converted
			if (m_convertedDouble)
				return m_double;

			try
			{
				// Else we need to convert:
				switch (m_type)
				{
					case DataType.String:
						m_double = double.Parse(m_string);
						break;
					case DataType.DateTime:
						m_double = m_datetime.Ticks;
						break;
					case DataType.Flag:						
						if (m_flag)
							m_double = TRUE_BYTE;
						else
							m_double = FALSE_BYTE;
						break;
					case DataType.Long:
						m_double = m_long;
						break;
					default:
						m_double = DEFAULT_DOUBLE;
						break;
				}
			}
			catch 
			{
				m_double = DEFAULT_DOUBLE;
			}

			m_convertedDouble = true;
			return m_double;
		}


		/// <summary>
		/// Contructs new instance of this class and sets current value.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="val">Source value</param>
		/// <returns>New instance of DataValue</returns>
		public static implicit operator DataValue(string val)
		{
			return new DataValue(val);
		}


		/// <summary>
		/// Contructs new instance of this class and sets current value.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="val">Source value</param>
		/// <returns>New instance of DataValue</returns>
		public static implicit operator DataValue(long val)
		{
			return new DataValue(val);
		}


		/// <summary>
		/// Contructs new instance of this class and sets current value.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="val">Source value</param>
		/// <returns>New instance of DataValue</returns>
		public static implicit operator DataValue(double val)
		{
			return new DataValue(val);
		}


		/// <summary>
		/// Contructs new instance of this class and sets current value.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="val">Source value</param>
		/// <returns>New instance of DataValue</returns>
		public static implicit operator DataValue(bool val)
		{
			return new DataValue(val);
		}


		/// <summary>
		/// Contructs new instance of this class and sets current value.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="val">Source value</param>
		/// <returns>New instance of DataValue</returns>
		public static implicit operator DataValue(DateTime val)
		{
			return new DataValue(val);
		}


		/// <summary>
		/// Converts instance of data value to string.
		/// Performs get by value (safe).
		/// </summary>
		/// <param name="val">Data value to be converted</param>
		/// <returns>Value returned by conversion</returns>
		public static implicit operator string(DataValue val) 
		{
			return val.GetString();
		}


		/// <summary>
		/// Converts instance of data value to long.
		/// Performs get by value (safe).
		/// </summary>
		/// <param name="val">Data value to be converted</param>
		/// <returns>Value returned by conversion</returns>
		public static implicit operator long(DataValue val) 
		{
			return val.GetLong();
		}


		/// <summary>
		/// Converts instance of data value to double.
		/// Performs get by value (safe).
		/// </summary>
		/// <param name="val">Data value to be converted</param>
		/// <returns>Value returned by conversion</returns>
		public static implicit operator double(DataValue val) 
		{
			return val.GetDouble();
		}


		/// <summary>
		/// Converts instance of data value to bool.
		/// Performs get by value (safe).
		/// </summary>
		/// <param name="val">Data value to be converted</param>
		/// <returns>Value returned by conversion</returns>
		public static implicit operator bool(DataValue val) 
		{
			return val.GetFlag();
		}


		/// <summary>
		/// Converts instance of data value to DateTime.
		/// Performs get by value (safe).
		/// </summary>
		/// <param name="val">Data value to be converted</param>
		/// <returns>Value returned by conversion</returns>
		public static implicit operator DateTime(DataValue val) 
		{
			return val.GetDateTime();
		}

		
		/// <summary>
		/// Sets the data value.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="val">Source value</param>
		public void SetString(string val)
		{
			Reset();
			m_string = string.Copy(val);
			m_type = DataType.String;
			m_convertedString = true;
		}


		/// <summary>
		/// Sets the data value.
		/// Performs set by reference (unsafe).
		/// </summary>
		/// <param name="val">Source value</param>
		public void SetStringByReference(string val)
		{
			Reset();
			m_string = val;
			m_type = DataType.String;
			m_convertedString = true;
		}


		/// <summary>
		/// Sets the data value.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="val">Source value</param>
		public void SetLong(long val)
		{
			Reset();
			m_long = val;
			m_type = DataType.Long;
			m_convertedLong = true;
		}


		/// <summary>
		/// Sets the data value.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="val">Source value</param>
		public void SetDouble(double val)
		{
			Reset();
			m_double = val;
			m_type = DataType.Double;
			m_convertedDouble = true;
		}


		/// <summary>
		/// Sets the data value.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="val">Source value</param>
		public void SetDateTime(DateTime val)
		{
			Reset();
			m_datetime = new DateTime(val.Ticks);
			m_type = DataType.DateTime;
			m_convertedDateTime = true;
		}


		/// <summary>
		/// Sets the data value.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="val">Source value</param>
		public void SetFlag(bool val)
		{
			Reset();
			m_flag = val;
			m_type = DataType.Flag;
			m_convertedFlag = true;
		}


		/// <summary>
		/// Returns true if this instance is equal to the other data value.
		/// </summary>
		/// <param name="other">Data value</param>
		/// <returns>Comparison flag</returns>
		public bool IsEqual(DataValue other) 
		{
			if (other == null)
				throw new EInvalidParameter(this, "other");

			switch (m_type)
			{
				case DataType.DateTime:
					return GetLong() == other.GetLong();
				case DataType.Double:
					return GetDouble() == other.GetDouble();
				case DataType.Flag:
					return GetFlag() == other.GetFlag();
				case DataType.Long:
					return GetLong() == other.GetLong();
				case DataType.String:
					return GetString() == other.GetString();
			}

			return false;
		}

		/// <summary>
		/// Returns true if this instance is greater than the other data value.
		/// </summary>
		/// <param name="other">Data value</param>
		/// <returns>Comparison flag</returns>
		public bool IsGreaterThan(DataValue other) 
		{
			if (other == null)
				throw new EInvalidParameter(this, "other");

			switch (m_type)
			{
				case DataType.DateTime:
					return GetLong() > other.GetLong();
				case DataType.Double:
					return GetDouble() > other.GetDouble();
				case DataType.Flag:
					return false;
				case DataType.Long:
					return GetLong() > other.GetLong();
				case DataType.String:
					return false;
			}

			return false;
		}


		/// <summary>
		/// Overrides Object.ToString.
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <returns>Data value representation as string.</returns>
		override public string ToString()
		{
			return GetStringByReference();
		}
	}
}
