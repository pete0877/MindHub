using System;
using System.Collections;

namespace Engine
{
	/// <summary>
	/// Basic expression class. Encapsulates expression information that can be used to quickly evaluated the expression.
	/// </summary>
	public class Expression
	{
		/// <summary>
		/// Expression types.
		/// </summary>
		public enum ExpressionKind { DataValue, Variable, Function };

		/// <summary>
		/// Type of expression.
		/// </summary>
		public ExpressionKind Kind;

		/// <summary>
		/// Expression name (e.g. variable name or function name).
		/// </summary>
		public string Name;

		/// <summary>
		/// Evaluated data value of the expression (if constant).
		/// </summary>
		public DataValue Value;

		/// <summary>
		/// List of children expressions (e.g. function arguments)
		/// </summary>
		public ArrayList Children;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Expression()
		{
		}

		/// <summary>
		/// Full access constructor. 
		/// Initializes all vital data members.
		/// </summary>
		/// <param name="kind">Expression kind</param>
		/// <param name="name">Expression name (e.g. variable name or function name)</param>
		/// <param name="dataValue">Constant value</param>
		/// <param name="children">List of children expressions</param>
		public Expression(ExpressionKind kind, string name, DataValue dataValue, ArrayList children)
		{
			Kind = kind;
			Name = name;
			Value = dataValue;
			Children = children;
		}
	}
}
