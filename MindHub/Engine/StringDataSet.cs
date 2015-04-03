using System;
using System.Collections;
using System.Collections.Specialized;

// TODO: Implement XML representation for StringDataSet

namespace Engine
{
	/// <summary>
	/// String value collection programmed with conversions between different 
	/// formats (e.g. Hash of strings to (and from) web-like data representation)
	/// </summary>
	public class StringDataSet
	{
		/// <summary>
		/// Type of data being stored or converted from/to in form of a string
		/// </summary>
		public enum RepresentationType { AmpersandBased };

		/// <summary>
		/// Flag for if data has already been converted to ampersand-based representation
		/// </summary>
		protected bool m_convertedAmpersandBased;

		/// <summary>
		/// Flag for if data has already been converted to hash representation
		/// </summary>
		protected bool m_convertedHashBased;

		/// <summary>
		/// Representation of this data set as ampersand-delimited string
		/// </summary>
		protected string m_ampersendRepresentation;

		/// <summary>
		/// Representation of this data set as hash dictionary (string to string)
		/// </summary>
		protected NameValueCollection m_hashRepresentation;

		/// <summary>
		/// Contructs instance with no data set
		/// </summary>
		public StringDataSet()
		{	
			Clear();
		}


		/// <summary>
		/// Constructs data set with given data of given type. 
		/// Performs set by reference (unsafe)
		/// </summary>
		/// <param name="data">String representation of the data set in given type</param>
		/// <param name="type">Data set type</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public StringDataSet(string data, RepresentationType type)
		{
			SetDataByReference(data, type);
		}


		/// <summary>
		/// Constructs data set from hash type.
		/// Performs set by reference (unsafe)
		/// </summary>
		/// <param name="data">String-to-string hash</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public StringDataSet(NameValueCollection data)
		{
			SetDataByReference(data);
		}


		/// <summary>
		/// Basic access indexer. Get returns empty string if key is not found.
		/// Performs set/get by value (safe)
		/// </summary>
		/// <param name="key">Variable name</param>		
		/// <exception cref="EInvalidParameter">If indexing string is null or ""</exception>
		public string this[string key]
		{
			get
			{				
				ConvertToHashBased();

				if (key == null || key == "")
					throw new EInvalidParameter(this, "key");

				if (m_hashRepresentation[key] == null)
					return "";

				return string.Copy(m_hashRepresentation[key]);
			}
			set
			{			
				ConvertToHashBased();

				if (key == null || key == "")
					throw new EInvalidParameter(this, "key");

				m_hashRepresentation[key] = string.Copy(value);

				// After this operation, representations other then hash are no longer correct
				m_convertedAmpersandBased = false;
				m_ampersendRepresentation = "";
			}			
		}


		/// <summary>
		/// Clears state of this instance
		/// </summary>
		public void Clear()
		{
			m_convertedAmpersandBased = false;
			m_convertedHashBased = false;		
			m_ampersendRepresentation = "";
			m_hashRepresentation = new NameValueCollection();
		}


		/// <summary>
		/// Sets the state of this instance to given data value type.  
		/// Performs set by value (safe)
		/// </summary>
		/// <param name="data">String representation of the data</param>
		/// <param name="type">Data set type</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public void SetData(string data, RepresentationType type)
		{ 
			SetDataByReference(string.Copy(data), type);
		}


		/// <summary>
		/// Sets the state of this instance to given data value type.  
		/// Performs set by reference (unsafe)
		/// </summary>
		/// <param name="data">String representation of the data</param>
		/// <param name="type">Data set type</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public void SetDataByReference(string data, RepresentationType type)
		{ 
			if (data == null)
				throw new EInvalidParameter(this, "data");

			Clear();
			switch (type)
			{
				case RepresentationType.AmpersandBased:
				{
					m_ampersendRepresentation = data;
					m_convertedAmpersandBased = true;
					break;
				}
			}
		}


		/// <summary>
		/// Sets the state of this instance to given data set from hash representation (string-to-string).
		/// Performs set by value (safe)
		/// </summary>
		/// <param name="data">Hash representation of the data (string-to-string)</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public void SetData(NameValueCollection data)
		{ 
			if (data == null)
				throw new EInvalidParameter(this, "data");
 
			// In this case we can't just use Hashtable.Clone because it is shallow
			IEnumerator itr = data.GetEnumerator();
			NameValueCollection copyHash = new NameValueCollection();
			while (itr.MoveNext())
			{
				string key = (string) itr.Current;
				string val = copyHash[key];
				copyHash[string.Copy(key)] = string.Copy(val);
			}
			SetDataByReference(copyHash);
		}


		/// <summary>
		/// Sets the state of this instance to given data set from hash representation (string-to-string).
		/// Performs set by reference (unsafe)
		/// </summary>
		/// <param name="data">Hash representation of the data (string-to-string)</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public void SetDataByReference(NameValueCollection data)
		{ 
			if (data == null)
				throw new EInvalidParameter(this, "data");
 
			Clear();	
			m_convertedHashBased = true;
			m_hashRepresentation = data;			
		}


		/// <summary>
		/// Returns representation of the data value set in given type in string format.
		/// Performs return by reference (unsafe)
		/// </summary>
		/// <param name="type">Type of data set to return</param>
		/// <returns>State representation</returns>		
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public string GetData(RepresentationType type)
		{
			if (type == RepresentationType.AmpersandBased)
			{
				if (!m_convertedAmpersandBased)
					ConvertToAmpersandBased();
				return m_ampersendRepresentation;
			}

			return "";
		}


		/// <summary>
		/// Returns representation of the data value set in given type in string format.
		/// Performs return by reference (unsafe)
		/// </summary>
		/// <returns>State representation</returns>				
		public NameValueCollection GetData()
		{
			if (!m_convertedHashBased)
				ConvertToHashBased();

			return m_hashRepresentation;
		}


		/// <summary>
		/// Converts current state of the instance to ampersand-based
		/// If conversion already took place, no action is taken.
		/// </summary>		
		public void ConvertToAmpersandBased()
		{
			if (m_convertedAmpersandBased)
				return;

			m_ampersendRepresentation = "";

			// First convert to hash so that we can interate through values:
			ConvertToHashBased();

			IEnumerator itr = m_hashRepresentation.GetEnumerator();
			while(itr.MoveNext())
			{
				string key = (string) itr.Current;
				string val = m_hashRepresentation[key];
				
				if (m_ampersendRepresentation.Length == 0)
					m_ampersendRepresentation = key + "=" + val;
				else
					m_ampersendRepresentation += "&" + key + "=" + val;
			}

			m_convertedAmpersandBased = true;
		}


		/// <summary>
		/// Converts current state of the instance to hash-based
		/// If conversion already took place, no action is taken.
		/// </summary>		
		public void ConvertToHashBased()
		{
			if (m_convertedHashBased)
				return;

			m_hashRepresentation = new NameValueCollection();

			if (m_convertedAmpersandBased)
			{
				string[] parameters = m_ampersendRepresentation.Split('&');
				IEnumerator itr = parameters.GetEnumerator();
				while (itr.MoveNext())
				{
					string parameter = (string) itr.Current;
					string[] keyValuePair = parameter.Split('=');
					if (keyValuePair.Length == 2)
					{
						string key = keyValuePair[0];
						string val = keyValuePair[1];
						m_hashRepresentation[key] = val;
					}
				}
			}
		}


		/// <summary>
		/// Returns dictionary enumeration of all string values.
		/// Members of the enumerations are unsafe references.
		/// </summary>
		/// <returns>Enumeration of all string values</returns>
		public IEnumerator GetValues()
		{
			return GetValues(null);
		}


		/// <summary>
		/// Returns dictionary enumeration of string values that begin with prefixFilter
		/// Performs return by reference (unsafe)
		/// </summary>
		/// <param name="prefixFilter">String name prefix. If null or equal to "", no filter will be used and all string values will be returned</param>
		/// <returns>Enumeration of filtered string values</returns>		
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public IEnumerator GetValues(string prefixFilter)
		{
			ConvertToHashBased();	
	
			if (prefixFilter == null || prefixFilter == "")
				return m_hashRepresentation.GetEnumerator();

			Hashtable result = new Hashtable();

			IEnumerator itr = m_hashRepresentation.GetEnumerator();
			while (itr.MoveNext())
			{
				string key = (string) itr.Current;				
				if (key.IndexOf(prefixFilter) == 0)
				{
					string val = m_hashRepresentation[key];
					result[key] = val;
				}
			}

			return result.GetEnumerator();
		}
	}
}
