using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;

namespace Engine
{
	/// <summary>
	/// Basic configuration class reusable throughout the system
	/// </summary>
	[Serializable]
	public class Config
	{
		/// <summary>
		/// Configuration ID.
		/// </summary>
		protected string m_configID;

		/// <summary>
		/// Internal configuration element hash
		/// </summary>
		public NameValueCollection m_hash;

		/// <summary>
		/// Internal configuration element list hash (for list of values for one element name)
		/// </summary>
		protected Hashtable m_hashLists;

		/// <summary>
		/// Flag indicating if the object was initialized
		/// </summary>
		protected bool m_initialized;

		/// <summary>
		/// Basic contructor
		/// </summary>
		public Config()
		{
			m_hash = new NameValueCollection();
			m_hashLists = new Hashtable();
		}


		/// <summary>
		/// Initializes the configuration with an ID and configuration string in XML format
		/// </summary>
		/// <param name="configID">Configuration ID</param>
		/// <param name="configXML">Configuration string in XML format</param>
		/// <exception cref="EInvalidParameter">If either of the parameters are null</exception>
		public void Initialize(string configID, string configXML)
		{
			if (configID == null)
				throw new EInvalidParameter(this, "classID");

			if (configXML == null)
				throw new EInvalidParameter(this, "configXML");

			SetConfigID(configID);
			SetConfig(configXML);

			m_initialized = true;
		}


		/// <summary>
		/// Returns the configuration ID string.
		/// Performs get by value (safe)
		/// </summary>
		/// <returns>Configuration ID</returns>
		public string GetConfigID()
		{
			return string.Copy(m_configID);
		}


		/// <summary>
		/// Sets the current configuration ID to given ID string.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="configID">New configuration ID</param>
		/// <exception cref="EInvalidParameter">If configID is null</exception>
		public void SetConfigID(string configID)
		{
			if (configID == null)
				throw new EInvalidParameter(this, "configID");

			m_configID = string.Copy(configID);
		}

		/// <summary>
		/// Sets the current configuration object to new XML configuration string.
		/// Performs set by value (safe).
		/// </summary>
		/// <param name="configXML">Configuration string in XML format</param>	
		/// <exception cref="EInvalidParameter">If config XML is null</exception>	
		public void SetConfig(string configXML)
		{
			if (configXML == null)
				throw new EInvalidParameter(this, "configXML");

			// Clear the current configuration and walk through the XML string to retreive all elements:
			m_hash.Clear();
			StringReader stringReader = new StringReader(configXML);
			XmlTextReader xmlReader = new XmlTextReader(stringReader);
			string elementName = "";
			while (xmlReader.Read())
			{
				switch (xmlReader.NodeType)
				{
					case XmlNodeType.Element:
						elementName = xmlReader.Name;
						break;						
					case XmlNodeType.Text:
						if (m_hash[elementName] != null)
						{							
							ArrayList array;
							if (!m_hashLists.ContainsKey(elementName))
							{								
								array = new ArrayList();
								m_hashLists[elementName] = array;
								string val = m_hash[elementName];
								array.Add(val);
							}
							else
								array = (ArrayList) m_hashLists[elementName];

							array.Add(xmlReader.Value);
						}
						else
							m_hash.Add(elementName, xmlReader.Value);
						break;						
				}

			}
		}


		/// <summary>
		/// Basic access indexer.
		/// Performs set/get by value (safe).
		/// </summary>
		/// <param name="element">Configuration element name to operate on</param>		
		/// <exception cref="EInvalidParameter">If indexing string is null</exception>
		public string this[string element]
		{
			get
			{				
				if (element == null)
					throw new EInvalidParameter(this, "element");

				if (m_hash[element] == null)
					return "";
				return string.Copy(m_hash[element]);
			}
			set
			{				
				if (element == null)
					throw new EInvalidParameter(this, "element");

				m_hash[element] = string.Copy(value);
			}			
		}


		/// <summary>
		/// Returns array of values for given element name. If there is only one value for the element name in the configuration, the array will contain that one element.
		/// </summary>
		/// <param name="element">Configuration element name to operate on</param>		
		/// <exception cref="EInvalidParameter">If indexing string is null</exception>
		public ArrayList GetParamList(string element)
		{
			if (element == null)
				throw new EInvalidParameter(this, "element");

			ArrayList arrayReturned = new ArrayList();
			ArrayList arrayInternal;
			if (!m_hashLists.ContainsKey(element))
			{								
				string val = (string) this[element];
				if (val != null)
					arrayReturned.Add(val);
			}
			else
			{
				arrayInternal = (ArrayList) m_hashLists[element];
				// Iterate through all elements of the internal array and copy 
				// its values over to the returned array:
				IEnumerator itr = arrayInternal.GetEnumerator();
				while(itr.MoveNext())
				{
					string val = (string) itr.Current;
					arrayReturned.Add(string.Copy(val));
				}
			}

			return arrayReturned; 
		}
	}
}
