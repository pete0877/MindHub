using System;

namespace Engine
{
	public interface ITemplate
	{
		char[] GetContent();
		string GetName();
		TemplateMetaData GetMetaData();
		string GetUIPathOS();
		string GetUIPathWeb();
		string GetLanguageCd();
		string GetStyleCd();
	}
}
