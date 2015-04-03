using System;

namespace Common
{
	/// <summary>
	/// SmartInt class wraps some extended features surrounding integer processing
	/// </summary>
	public class SmartInt
	{
		/// <summary>
		/// Returns number from the string if the [from] string is non-empty and non-null and is parsable, else returns 0
		/// </summary>
		/// <param name="from">Source string</param>
		/// <returns>0 if any parsing problem occurs, else returns the parsed string into integer</returns>
		public static int Parse(string from)
		{
			if (from == null || from.Length == 0)
				return 0;

			try
			{
				int result = int.Parse(from);
				return result;
			}
			catch (Exception e) {}

			return 0;
		}
	}
}
