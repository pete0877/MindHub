using System;
using Engine;

namespace ServiceStartTD
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			ServiceDirector m_serviceDirector = new ServiceDirector();
			m_serviceDirector.Initialize();
			m_serviceDirector.StartServices();

			Console.WriteLine("Started ... ");
			string confirm = Console.ReadLine();
		}
	}
}
