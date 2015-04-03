using System;
using System.IO;

namespace Engine
{
	/// <summary>
	/// Base file template class. Encapsulates reading of a file template to be parsed.
	/// </summary>
	public class FileTemplate : ITemplate
	{
		/// <summary>
		/// FileStream buffer size
		/// </summary>
		public static int BUFFER_SIZE = 4096;

		/// <summary>
		/// File stream used to read the file.
		/// </summary>
		protected FileStream m_fileStream;

		/// <summary>
		/// Stream reader used to read the file.
		/// </summary>
		protected StreamReader m_streamReader;

		/// <summary>
		/// This template's metadata.
		/// </summary>
		protected TemplateMetaData m_metaData;

		/// <summary>
		/// Language code used within the template.
		/// </summary>
		protected string m_languageCd;

		/// <summary>
		/// Style code used within the template.
		/// </summary>
		protected string m_styleCd;

		/// <summary>
		/// OS path to the UI directory, including language and style folder names (e.g. 'D:\MindHub\Dev\WebRoot\Fake\eng\orange'). No slash at the end.
		/// </summary>
		protected string m_uiPathOS;

		/// <summary>
		/// Web path to the UI directory, including language and style folder names (e.g. '/fake/eng/orange/html'). No slash at the end.
		/// </summary>
		protected string m_uiPathWeb;

		/// <summary>
		/// Name of the file being parsed (e.g. 'mainmenu.htm').
		/// </summary>
		protected string m_fileName;

		/// <summary>
		/// Content of the template (Unicode)
		/// </summary>
 		protected char[] m_content;


		/// <summary>
		/// Constructor.
		/// Language and style codes are found in session object.
		/// </summary>
		/// <param name="fileName">Name of the template to be read (e.g. 'mainmenu.htm'). Should not include the 'html' part of the path (Automatically assumed)</param>
		/// <param name="session">User session</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public FileTemplate(string fileName, Engine.Session session)
		{
			if (fileName == null)
				throw new EInvalidParameter(this, "fileName");

			if (session == null)
				throw new EInvalidParameter(this, "session");

			Initialize(fileName, session.EnvironmentID, session.LanguageCd, session.StyleCd);
		}


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fileName">Name of the template to be read (e.g. 'mainmenu.htm'). Should not include the 'html' part of the path (Automatically assumed)</param>
		/// <param name="session">User session</param>
		/// <param name="languageCd">Language code (e.g. 'eng')</param>
		/// <param name="styleCd">Style code (e.g. 'eng')</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public FileTemplate(string fileName, Transaction transaction, string languageCd, string styleCd)
		{
			if (fileName == null)
				throw new EInvalidParameter(this, "fileName");

			if (transaction == null)
				throw new EInvalidParameter(this, "transaction");

			if (languageCd == null)
				throw new EInvalidParameter(this, "languageCd");

			if (styleCd == null)
				throw new EInvalidParameter(this, "styleCd");

			Initialize(fileName, transaction.EnvironmentID, languageCd, styleCd);
		}


		/// <summary>
		/// Initializes instance of the file template (reads it in or grabs it from the cached depending on modification date).
		/// </summary>
		/// <param name="fileName">Name of the template to be read (e.g. 'mainmenu.htm'). Should not include the 'html' part of the path (Automatically assumed)</param>
		/// <param name="session">User session</param>
		/// <param name="languageCd">Language code (e.g. 'eng')</param>
		/// <param name="styleCd">Style code (e.g. 'eng')</param>
		protected void Initialize(string fileName, string environmentID, string languageCd, string styleCd)
		{
			m_content = null;

			m_languageCd = languageCd;
			m_styleCd = styleCd;
			m_fileName = fileName;

			// Find UI paths (OS and web):
			Environment environment = EnvironmentManager.GetManager().GetEnvironment(environmentID);						
			m_uiPathOS = environment.UIPathOS + m_languageCd + "\\" + styleCd;
			m_uiPathWeb = environment.UIPathWeb + m_languageCd + "/" + styleCd;

			// Establish temporary variables that will be used for fetching cached template data and reading the file:
			string relativeUIPathOS = m_languageCd + "\\" + styleCd + "\\html\\" + fileName;
			string fullUIPathOS = m_uiPathOS + "\\html\\" + fileName;

			// Find the modification date on the template and the modification time 
			// template was cached by the template file manager. 
			TemplateManager templateManager = TemplateManager.GetManager();
			
			DateTime lastModified = File.GetLastWriteTime(fullUIPathOS);
			DateTime lastCached = templateManager.GetCacheStamp(relativeUIPathOS, environmentID);

			if (lastCached > lastModified)
			{
				m_content = templateManager.GetCachedContent(relativeUIPathOS, environmentID);
				m_metaData = templateManager.GetCachedMetaData(relativeUIPathOS, environmentID);
			}
			else
			{
				m_fileStream = new FileStream(fullUIPathOS, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE, false);

				if (m_fileStream == null)
					throw new EInvalidParameter(this, "fileName", fullUIPathOS, "Could not open template file for reading");

				m_streamReader = new StreamReader(m_fileStream);
				if (m_streamReader == null)
					throw new EInvalidParameter(this, "fileName", fullUIPathOS, "Could not open template file for reading");

				m_content = m_streamReader.ReadToEnd().ToCharArray();

				if (m_streamReader != null)
				{
					m_streamReader.Close();
					m_streamReader = null;
				}

				if (m_fileStream != null)
				{
					m_fileStream.Close();
					m_fileStream = null;
				}

				// Create and process meta-data
				m_metaData = new TemplateMetaData();
				m_metaData.Initialize(this);

				// Once both the content and meta-data are created for this template we can cache it:
				templateManager.CacheTemplate(relativeUIPathOS, environmentID, m_content, m_metaData);
			}
		}


		/// <summary>
		/// Returns name if this template (e.g. 'mainmenu.html').
		/// Perfors get by reference (unsafe).
		/// </summary>
		/// <returns>Template name</returns>
		public string GetName()
		{
			return m_fileName;
		}


		/// <summary>
		/// Returns the OS path to the UI folder (see m_uiPathOS).
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <returns>OS path to the UI folder</returns>
		public string GetUIPathOS()
		{
			return m_uiPathOS;
		}


		/// <summary>
		/// Returns the Web path to the UI folder (see m_uiPathWeb).
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <returns>Web path to the UI folder</returns>
		public string GetUIPathWeb()
		{
			return m_uiPathWeb;
		}


		/// <summary>
		/// Returns language code of this template.
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <returns>Language code in which this template has been written.</returns>
		public string GetLanguageCd()
		{
			return m_languageCd;
		}


		/// <summary>
		/// Returns style code of this template.
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <returns>Style code in which this template has been written.</returns>
		public string GetStyleCd()
		{
			return m_styleCd;
		}
		

		/// <summary>
		/// Returns content of the template.
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <returns>Template content (unparsed)</returns>
		public char[] GetContent()
		{		
			return m_content;
		}


		/// <summary>
		/// Returns meta-data compiled over this template.
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <returns>Meta-data for this template.</returns>
		public TemplateMetaData GetMetaData()
		{
			return m_metaData;
		}
	}
}
