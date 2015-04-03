using System;
using System.Collections;

namespace Engine
{
	public interface ITemplateFunction
	{
		/// <summary>
		/// Evaluates the function into DataValue.
		/// </summary>
		/// <param name="arguments">List of evaluated function arguments</param>
		/// <returns>Evaluated function as data value</returns>
		DataValue Evaluate(ArrayList arguments, Transaction transaction, IParser parser, Engine.Session session);
	}
}
