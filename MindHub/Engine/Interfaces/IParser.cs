using System;
using System.Web.UI;
using System.Collections;
using System.Collections.Specialized;


namespace Engine
{
	public interface IParser
	{
		void AddInput(Engine.Session input);
		void AddInput(Hashtable input);
		void AddInput(IDataObject input);
		void AddInput(NameValueCollection input);		
		void Parse(HtmlTextWriter output);
		void Parse(out string output);
		string GetTemplateName();
		string GetLanguageCd();
		DataValue EvaluateExpression(Expression expression);
		DataValue EvaluateVariable(string varibleName);
		void SetVariable(string variableName, DataValue variableValue);
	}
}

