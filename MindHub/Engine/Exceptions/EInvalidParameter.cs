using System;

namespace Engine
{
	/// <summary>
	/// Implementation of a typical "invalid parameter" error.
	/// Note: not using ArgumentNullException class because EInvalidParameter emcompases not only method calls but also configuration variables, etc.
	/// </summary>
	public class EInvalidParameter : Exception
	{
		/// <summary>
		/// Creates self with information about the throwing object and parameter that was missing or invalid. Includes parameter value and verbal reason for why value is invalid.
		/// </summary>
		/// <param name="thrower">Throwing object</param>
		/// <param name="paramName">Parameter name</param>
		/// <param name="paramValue">Parameter value</param>
		/// <param name="reason">Verbal description of the reason why parameter is invalid</param>
		public EInvalidParameter(System.Object thrower, string paramName, string paramValue, string reason) : base(thrower.ToString() + " received invalid parameter [" + paramName + "] with value [" + paramValue + "]. " + reason)
		{
		}

		
		/// <summary>
		/// Creates self with information about the throwing object and parameter that was missing or invalid.
		/// </summary>
		/// <param name="thrower">Throwing object</param>
		/// <param name="paramName">Name of the parameter</param>
		public EInvalidParameter(System.Object thrower, string paramName) : base(thrower.ToString() + " received null or empty parameter [" + paramName + "]")
		{
		}
	}
}
