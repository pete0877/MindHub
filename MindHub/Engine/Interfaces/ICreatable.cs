using System;

namespace Engine
{
	/// <summary>
	/// Base inteface for ClassFactory creatable objects
	/// </summary>
	public interface ICreatable
	{
		/// <summary>
		/// Returns class ID.
		/// Performs get by reference (unsafe)
		/// </summary>
		/// <return>Class ID</return>
		string GetClassID();
	}
}
