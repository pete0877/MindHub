using System;

namespace Engine
{
	public class ESyntaxError : Exception
	{
		public ESyntaxError(System.Object thrower, string context, string badSyntax, string syntaxError) : base(thrower.ToString() + " reported sytax error in context of '" + context + "'. Expression '" + badSyntax + "' failed syntax check / parsing because: " + syntaxError)
		{
		}
	}
}
