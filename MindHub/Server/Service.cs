using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.Remoting;
using Engine;

namespace Server
{
	public class Service : System.ServiceProcess.ServiceBase
	{
		/// <summary>
		/// Service dirctor (reponsible for startping and stopping the services)
		/// </summary>
		private ServiceDirector m_serviceDirector;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary>
		/// Default contructor
		/// </summary>
		public Service()
		{
			// This call is required by the Windows.Forms Component Designer.
			InitializeComponent();
		}


		// The main entry point for the process
		static void Main()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun;
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new Service() };
			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}


		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			this.ServiceName = Def.ENGINE_NAME + " Application Service";
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			// Any exceptions thrown here are caught by the base class and can be found in the EventViewer
			m_serviceDirector = new ServiceDirector();
			m_serviceDirector.Initialize();
			m_serviceDirector.StartServices();
		}
 

		/// <summary>
		/// Stops this service.
		/// </summary>
		protected override void OnStop()
		{
			// Any exceptions thrown here are caught by the base class and can be found in the EventViewer
			m_serviceDirector.StopServices();
		}
	}
}
