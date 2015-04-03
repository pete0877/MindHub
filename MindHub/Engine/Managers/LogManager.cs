using System;
using System.Threading;
using System.IO;

// TODO: Check this class - sometimes produces exceptions on TextWriter object uninitialization (usually when exception is caught)

namespace Engine
{
	/// <summary>
	/// Log manager. Allows any System.Object to log information into a text log file
	/// </summary>
	public class LogManager : Manager
	{
		/// <summary>
		/// Default target file name for the log file.
		/// </summary>
		static public string DEFAULT_FILENAME			= Def.ENGINE_NAME + ".log";

		/// <summary>
		/// Config param: Web plugins path.
		/// </summary>
		static public string SETTING_LOGPATH		= "logPath";

		/// <summary>
		/// FileStream buffer side
		/// </summary>
		public static int BUFFER_SIZE = 4096;

		/// <summary>
		/// Level of importance of a log entry
		/// </summary>
		public enum Level { DEBUG = 2, NORMAL = 1, ERROR = 0};

		/// <summary>
		/// Singelton manager reference
		/// </summary>
		static protected LogManager g_manager;

		/// <summary>
		/// Filename of the 
		/// </summary>
		protected string m_fileName;

		/// <summary>
		/// Current level of logging
		/// </summary>
		protected Level m_level;

		/// <summary>
		/// Provides lock on the log
		/// </summary>
		protected ReaderWriterLock m_logLock;

		/// <summary>
		/// Log file handle 
		/// </summary>
		protected FileStream m_log;

		/// <summary>
		/// Log file stream writer
		/// </summary>
		protected StreamWriter m_logStream;
	
		/// <summary>
		/// Static constructor. Creates the singelton instance.
		/// </summary>
		static LogManager()
		{
			g_manager = new LogManager();
		}


		/// <summary>
		/// Default constructor. Creates empty environment schema sets list
		/// </summary>
		protected LogManager()
		{			
			m_fileName = DEFAULT_FILENAME;
		}


		/// <summary>
		/// Destructor - closes the stream to log file
		/// </summary>
		~LogManager()
		{			
			m_logLock.AcquireWriterLock(Int32.MaxValue);
			
			if (m_logStream != null)
			{
				m_logStream.Flush();
				m_logStream.Close();
			}

			m_logLock.ReleaseLock();
		}


		/// <summary>
		/// Returns referece to the singelton manager
		/// </summary>
		/// <returns>Reference to sigelton manager</returns>
		public static LogManager GetManager()
		{			
			return g_manager;
		}

		public void SetFileName(string fileName)
		{
			if (fileName == null)
				throw new EInvalidParameter(this, "fileName");

			m_fileName = fileName;
		}

		/// <summary>
		/// Initializes instance of log manager:
		/// </summary>
		/// <exception cref="EInvalidParameter">When either one of the arguments is null or empty</exception>
		public override void Initialize()
		{	
			if (m_fileName == null)
				throw new EInvalidParameter(this, "m_fileName");

			m_logLock = new ReaderWriterLock();
			m_level = Level.NORMAL;

			SettingManager settingMgr = SettingManager.GetManager();
			string logFolderPath = settingMgr[SETTING_LOGPATH];
			if (logFolderPath == "")
				throw new EInvalidParameter(this, "SETTING_LOGPATH", "", "Configuration missing log folder path");

			// Create the file with asynchronous access for performance:			
			m_log = new FileStream(logFolderPath + "\\" + m_fileName, FileMode.Append, FileAccess.Write, FileShare.Read, BUFFER_SIZE, true);
		
			if (m_log == null)
				throw new EInvalidParameter(this, "m_fileName", m_fileName, "Could not open log file for append");

			m_logStream = new StreamWriter(m_log);
			if (m_logStream == null)
				throw new EInvalidParameter(this, "m_fileName", m_fileName, "Could not open log file for append");

			base.Initialize();
			LogManager.Log(this, "Initialized");
		}


		/// <summary>
		/// UnInitializes instance of log manager:
		/// </summary>
		public override void UnInitialize()
		{
			LogManager.Log(this, "Un-initialized");

			m_logLock.AcquireWriterLock(Int32.MaxValue);
			
			if (m_logStream != null)
			{
				m_logStream.Flush();
				m_logStream.Close();
				m_logStream = null;
			}

			m_logLock.ReleaseLock();

			base.UnInitialize();
		}


		/// <summary>
		/// Sets the current log level of the manager
		/// </summary>
		/// <param name="level">New log level</param>
		/// <exception cref="ENotInitialized">When manager is not initialized</exception>
		public void SetLogLevel(Level level)
		{
			m_level = level;
		}
		

		/// <summary>
		/// Returns current log level.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <returns>Current log level</returns>
		/// <exception cref="ENotInitialized">When manager is not initialized</exception>
		public Level GetLogLevel()
		{
			return m_level;
		}


		/// <summary>
		/// Static shortcut to the AddEntry() method of the singelton
		/// </summary>
		/// <param name="originator">Object creating the log entry</param>
		/// <param name="entry">Entry</param>
		/// Sets default importance level to Normal
		public static void Log(System.Object originator, string entry)
		{
			GetManager().AddEntry(originator, entry, Level.NORMAL);
		}


		/// <summary>
		/// Static shortcut to the AddEntry() method of the singelton
		/// </summary>
		/// <param name="originator">Object creating the log entry</param>
		/// <param name="entry">Entry</param>
		/// <param name="level">Level</param>
		public static void Log(System.Object originator, string entry, Level level)
		{
			GetManager().AddEntry(originator, entry, level);
		}


		/// <summary>
		/// Adds entry to the log. If the current level being used for logging is equal or higher then the [level] parameter, entry will be made to the log file. Else, this method returns with no action.
		/// </summary>
		/// <param name="originator">Object creating the log entry</param>
		/// <param name="entry">Entry</param>
		/// <param name="level">Level</param>
		/// <exception cref="ENotInitialized">When manager is not initialized</exception>
		/// <exception cref="EInvalidParameter">When either one of the arguments is null or empty</exception>
		protected void AddEntry(System.Object originator, string entry, Level level)
		{
			if (!m_initialized)
				throw new ENotInitialized(this);

			if (originator == null)
				throw new EInvalidParameter(this, "originator");

			if (entry == null || entry.Length == 0)
				throw new EInvalidParameter(this, "entry");

			// Only perform the logging if the level of importantce matches the current level
			if (m_level >= level)
			{
				string entryLine = DateTime.Now.ToString();
				entryLine += " | " + originator.ToString();
				entryLine += " | " + entry;

				m_logLock.AcquireWriterLock(Int32.MaxValue);
				m_logStream.WriteLine(entryLine);				
				m_logStream.Flush();
				m_logLock.ReleaseLock();
			}
		}		
	}
}
