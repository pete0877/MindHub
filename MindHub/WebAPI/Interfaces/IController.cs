using System;
using System.Web.UI;
using System.Collections.Specialized;
using Engine;

namespace WebAPI
{
	/// <summary>
	/// Controller interface. todo: Controller model will need to be redisigned keeping in mind complex UI model (e.g. Windows, dialogs, etc).
	/// </summary>
	public interface IController : ICreatable
	{
		/// <summary>
		/// Entry controller execution method.
		/// </summary>
		/// <param name="input">Web request input (comes from Page.Request.Params)</param>
		/// <param name="output">HTML output writer</param>
		/// <param name="session">Valid user session</param>
		void Execute(NameValueCollection input, HtmlTextWriter output, Engine.Session session);
	}
}
