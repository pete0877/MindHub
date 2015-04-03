using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using Engine;

namespace BaseWebModules
{
	/// <summary>
	/// Typical GUI button.
	/// </summary>
	public class GUIButton : GUIElement
	{
		/// <summary>
		/// Default content template.
		/// </summary>
		static public string TEMPLATE_CONTENT					= "gui/guibuttoncontent.html";

		/// <summary>
		/// Variable for displaying button label.
		/// </summary>
		public const string VAR_GUI_BUTTON_LABEL				= "guibuttonlabel";

		/// <summary>
		/// String code for button label
		/// </summary>
		public string Label;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GUIButton()
		{
		}


		/// <summary>
		/// Initializes instance of this GUIElement.
		/// </summary>
		/// <param name="elementID">Element ID</param>
		/// <param name="input">Web input</param>
		/// <param name="session">User session</param>
		/// <param name="response">Response object</param>
		override public void Initialize(string elementID, HttpRequest request, NameValueCollection input, HtmlTextWriter output, Engine.Session session, HttpResponse response)
		{
			base.Initialize(elementID, request, input, output, session, response);
			m_contentTemplate = TEMPLATE_CONTENT;
		}
		

		/// <summary>
		/// Responsible for rendering of this GUI element.
		/// If m_contentTemplate is set and not empty, it will be parsed.
		/// </summary>
		/// <param name="output">Output writer</param>
		/// <param name="transaction">Transaction</param>
		override public void Render(Transaction transaction)
		{
			if (!IsRendarable())
				return;

			if (Label != null && Label.Length > 0)
			{
				string labelText = new CodeString(transaction.EnvironmentID, m_session.LanguageCd, Label);
				m_variables[VAR_GUI_BUTTON_LABEL] = labelText;
			}

			Parser parser = new Parser(new FileTemplate(m_contentTemplate, m_session), transaction);
			parser.AddInput(m_variables);
			parser.AddInput(m_input);
			parser.Parse(m_output);
		}
	}
}
